using LinkServer;
using LinkServer.Middleware;
using LinkServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IFiller, IpFiller>();
builder.Services.AddScoped<IFiller, RequestFiller>();
builder.Services.AddScoped<IFiller, QueryStringFiller>();
builder.Services.AddScoped<IRuleEditorClient, RuleEditorClient>();
builder.Services.AddScoped<ILinkRedirector, LinkRedirector>();
builder.AddServiceDefaults();
builder.Services.AddHttpClient("rule-editor",
    (serviceProvider, client) => { client.BaseAddress = new Uri("http://rule-editor"); });
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    var filePath = Path.Combine(AppContext.BaseDirectory, "LinkServer.xml");
    c.IncludeXmlComments(filePath);
});

var app = builder.Build();

app.UseMiddleware<EnrichMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger( options => options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0);
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.MapGet("/link", [SwaggerOperation("Переход по ссылке согластно правилу")]
        async (HttpContext context, [FromServices] ILinkRedirector service, CancellationToken token) =>
        {
            var url = await service.RedirectAsync(
                (Dictionary<string, object>)context.Items[EnrichMiddleware.ContextKey], token);
            return Results.Redirect(url);
        })
    .WithName("Link")
    .WithOpenApi()
    .Produces(StatusCodes.Status307TemporaryRedirect);

app.Run();