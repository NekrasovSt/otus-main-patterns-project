using LinkServer;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IFiller, IpFiller>();
builder.Services.AddScoped<IFiller, RequestFiller>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/link", (HttpContext context, IServiceProvider provider) =>
    {
        var fillers = provider.GetServices<IFiller>();
        return context.Connection.RemoteIpAddress;
    })
    .WithName("Link")
    .WithOpenApi();

app.Run();