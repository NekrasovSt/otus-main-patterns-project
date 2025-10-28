using System.Text.Json.Serialization;
using MongoDB.Driver;
using RuleEditor.Interface;
using RuleEditor.Midleware;
using RuleEditor.Repositories;
using RuleEditor.Serialization;
using RuleEditor.Services;

namespace RuleEditor;

public class Startup
{
    public static void SetupServices(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IRuleRepository, RuleRepository>();
        builder.Services.AddScoped<IRuleService, RuleService>();
        builder.Services.AddSingleton<IMongoClient>((context) =>
        {
            var configuration = context.GetRequiredService<IConfiguration>();
            return new MongoClient(configuration.GetValue<string>("ConnectionStrings:mongo"));
        });
        
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    public static async Task SetupMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ErrorHandlerMiddleware>();
        app.UseHttpsRedirection();

        using var scope = app.Services.CreateScope();
        var repository = scope.ServiceProvider.GetService<IRuleRepository>();

        await repository.InitDBAsync();
    }
}