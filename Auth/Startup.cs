using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Auth.Helpers;
using Auth.Interfaces;
using Auth.Repositories;
using Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Driver;
using ServiceUtils.Midleware;
namespace Auth;

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
        builder.Services.AddSingleton<IKeyStore, KeyStore>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddSingleton<IMongoClient>((context) =>
        {
            var configuration = context.GetRequiredService<IConfiguration>();
            return new MongoClient(configuration.GetValue<string>("ConnectionStrings:mongo"));
        });
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
        builder.Services.AddCors();
        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            var filePath = Path.Combine(AppContext.BaseDirectory, "Auth.xml");
            c.IncludeXmlComments(filePath);
        });
    }
    /// <summary>
    /// Настройка Middleware
    /// </summary>
    public static async Task SetupMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger( options => options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0);
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
        );
        app.UseMiddleware<ErrorHandlerMiddleware>();

        using var scope = app.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        await repository.InitDBAsync();
    }
}