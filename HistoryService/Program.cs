using HistoryService;
using HistoryService.Dto;
using HistoryService.Interfaces;
using Mapster;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);
Startup.SetupServices(builder);


var app = builder.Build();
Startup.SetupMiddleware(app);

app.MapGet("/history", [SwaggerOperation("Получить историю")]
        async (IHistoryRepository historyRepository, CancellationToken token) =>
        {
            var result = await historyRepository.GetHistoryAsync(token);
            return Results.Ok(result.Adapt<IEnumerable<RuleHistoryDto>>());
        })
    .WithName("GetHistory")
    .WithOpenApi()
    .Produces<IEnumerable<RuleHistoryDto>>(StatusCodes.Status200OK)
    .RequireAuthorization("Default");

app.Run();