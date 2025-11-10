using System.Security.Cryptography;
using HistoryService.Helpers;
using HistoryService.Interfaces;
using HistoryService.Repositories;
using HistoryService.Service;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace HistoryService;

/// <summary>
/// Настройка приложения
/// </summary>
public class Startup
{
    /// <summary>
    /// Регистрация сервисов
    /// </summary>
    public static void SetupServices(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<IMongoClient>((context) =>
        {
            var configuration = context.GetRequiredService<IConfiguration>();
            return new MongoClient(configuration.GetValue<string>("ConnectionStrings:mongo"));
        });
        builder.Services.AddSingleton<IHistoryRepository, HistoryRepository>();
        builder.Services.AddSingleton<IKeyStore, KeyStore>();
        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumer<HistoryConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var configuration = context.GetRequiredService<IConfiguration>();
                var host = configuration.GetValue<string>("ConnectionStrings:notification");
                cfg.Host(host);
                cfg.ReceiveEndpoint("history", (re) =>
                {
                    re.ConfigureConsumeTopology = false;
                    re.ConfigureConsumer<HistoryConsumer>(context);
                    re.ExchangeType = ExchangeType.Direct;
                });
            });
        });
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
        builder.Services.AddAuthorization(x => { x.AddPolicy("Default", o => o.RequireAuthenticatedUser()); });
        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            var filePath = Path.Combine(AppContext.BaseDirectory, "HistoryService.xml");
            c.IncludeXmlComments(filePath);
        });
    }

    /// <summary>
    /// Настройка Middleware
    /// </summary>
    public static void SetupMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
    }
}