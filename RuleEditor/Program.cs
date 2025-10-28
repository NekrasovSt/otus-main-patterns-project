using Mapster;
using RuleEditor;
using RuleEditor.Dto;
using RuleEditor.Interface;
using RuleEditor.Midleware;
using RuleEditor.Models;

var builder = WebApplication.CreateBuilder(args);
Startup.SetupServices(builder);
TypeAdapterConfig<Rule, RuleDto>
    .NewConfig();

var app = builder.Build();
await Startup.SetupMiddleware(app);

app.MapGet("/rules/all", async (IRuleService service, CancellationToken token) =>
    {
        var result = await service.GetAllAsync(token);
        var dtos = result.Adapt<IEnumerable<RuleDto>>();
        return Results.Ok(dtos);
    })
    .WithName("GetAllRules")
    .WithOpenApi();

app.MapGet("/rules/{id}", async (string id, IRuleService service, CancellationToken token) =>
    {
        var result = await service.GetAsync(id, token);
        return Results.Ok(result.Adapt<RuleDto>());
    })
    .WithName("GetRule")
    .WithOpenApi();

app.MapDelete("/rules/{id}", async (string id, IRuleService service, CancellationToken token) =>
    {
        await service.DeleteAsync(id, token);
        return Results.NoContent();
    })
    .WithName("DeleteRule")
    .WithOpenApi();

app.MapPost("/rules", async (RuleDto rule, IRuleService service, CancellationToken token) =>
    {
        var newRule = await service.AddAsync(rule.Adapt<Rule>(), token);
        return Results.Created();
    })
    .WithName("AddRule")
    .WithOpenApi();

app.Run();