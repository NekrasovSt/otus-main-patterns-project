using Mapster;
using RuleEditor;
using RuleEditor.Dto;
using RuleEditor.Interface;
using RuleEditor.Models;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);
Startup.SetupServices(builder);
TypeAdapterConfig<Rule, RuleDto>
    .NewConfig();

var app = builder.Build();
await Startup.SetupMiddleware(app);

app.MapGet("/rules/all", [SwaggerOperation("Получить все правила")]
        async (IRuleService service, CancellationToken token) =>
        {
            var result = await service.GetAllAsync(token);
            var dtos = result.Adapt<IEnumerable<RuleDto>>();
            return Results.Ok(dtos);
        })
    .WithName("GetAllRules")
    .WithOpenApi()
    .Produces<IEnumerable<RuleDto>>(StatusCodes.Status200OK);

app.MapGet("/rules/{id}", [SwaggerOperation("Получить правило по ид")]
        async (string id, IRuleService service, CancellationToken token) =>
        {
            var result = await service.GetAsync(id, token);
            return Results.Ok(result.Adapt<RuleDto>());
        })
    .WithName("GetRule")
    .WithOpenApi()
    .Produces<RuleDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/rules/{id}", [SwaggerOperation("Удалить правило по ид")]
        async (string id, IRuleService service, CancellationToken token) =>
        {
            await service.DeleteAsync(id, token);
            return Results.NoContent();
        })
    .RequireAuthorization("Default")
    .WithName("DeleteRule")
    .WithOpenApi()
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized);

app.MapPost("/rules", [SwaggerOperation("Создать правило")]
        async (RuleDto rule, IRuleService service, CancellationToken token) =>
        {
            var newRule = await service.AddAsync(rule.Adapt<Rule>(), token);
            return Results.Created();
        })
    .WithName("AddRule")
    .WithOpenApi()
    .RequireAuthorization("Default")
    .Produces(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized);

app.MapPut("/rules", [SwaggerOperation("Обновить правила")]
        async (RuleDto rule, IRuleService service, CancellationToken token) =>
        {
            var newRule = await service.UpdateAsync(rule.Adapt<Rule>(), token);
            return Results.Ok(newRule.Adapt<RuleDto>());
        })
    .WithName("UpdateRule")
    .WithOpenApi()
    .RequireAuthorization("Default")
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized);
app.Run();