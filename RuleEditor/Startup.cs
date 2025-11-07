using System.Security.Cryptography;
using System.Text.Json.Serialization;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using RabbitMQ.Client;
using RuleEditor.Helpers;
using RuleEditor.Interface;
using RuleEditor.Repositories;
using RuleEditor.Services;
using ServiceUtils.Broker;
using ServiceUtils.Midleware;

namespace RuleEditor;

/// <summary>
/// Настройка приложения
/// </summary>
public static class Startup
{
    /// <summary>
    /// Регистрация сервисов
    /// </summary>
    public static void SetupServices(WebApplicationBuilder builder)
    {
        
        builder.Services.AddScoped<IRuleRepository, RuleRepository>();
        builder.Services.AddScoped<IRuleService, RuleService>();
        builder.Services.AddSingleton<IMongoClient>((context) =>
        {
            var configuration = context.GetRequiredService<IConfiguration>();
            return new MongoClient(configuration.GetValue<string>("ConnectionStrings:mongo"));
        });
        builder.Services.AddSingleton<IKeyStore, KeyStore>();
        
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthentication(options =>
            {
                // устанавливаем дефолтную схему как JWT
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var store = builder.Services.BuildServiceProvider().GetRequiredService<IKeyStore>();
                // создаем объект RSA
                var publicKey = RSA.Create();
                // импортируем публичный ключ для проверки подписи
                publicKey.ImportFromPem(store.PublicKey);

                // устанавливаем параметры токена
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "otus",
                    ValidAudience = "otus",
                    // указываем ключ для проверки подписи
                    IssuerSigningKey = new RsaSecurityKey(publicKey),
                    CryptoProviderFactory = new CryptoProviderFactory
                    {
                        // отключаем кеширование ключа. Объект RSA — Disposable,
                        // и если его закешировать, возможны ObjectDisposedException
                        CacheSignatureProviders = false
                    }
                };
            });
        builder.Services.AddAuthorization(x =>
        {
            x.AddPolicy("Default", o => o.RequireAuthenticatedUser());
        });
        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            var filePath = Path.Combine(AppContext.BaseDirectory, "RuleEditor.xml");
            c.IncludeXmlComments(filePath);
        });
        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                var configuration = context.GetRequiredService<IConfiguration>();
                var host = configuration.GetValue<string>("ConnectionStrings:notification");
                cfg.Host(host);
                cfg.ConfigureEndpoints(context);

                cfg.Message<RuleChangedDto>(e =>
                {
                    e.SetEntityName("rules");
                });
                cfg.Publish<RuleChangedDto>(e =>
                {
                    e.ExchangeType = ExchangeType.Direct;
                    e.Exclude = true;
                });
            });
        });
    }

    /// <summary>
    /// Настройка Middleware
    /// </summary>
    public static async Task SetupMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ErrorHandlerMiddleware>();

        using var scope = app.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRuleRepository>();

        await repository.InitDBAsync();
    }
}