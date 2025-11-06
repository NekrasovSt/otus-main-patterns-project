using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using RuleEditor.Helpers;
using RuleEditor.Interface;
using RuleEditor.Repositories;
using RuleEditor.Services;
using ServiceUtils.Midleware;

namespace RuleEditor;

public class Startup
{
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
                var store = builder.Services.BuildServiceProvider().GetService<IKeyStore>();
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
    }

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
        var repository = scope.ServiceProvider.GetService<IRuleRepository>();

        await repository.InitDBAsync();
    }
}