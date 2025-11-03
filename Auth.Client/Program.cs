using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Auth.Client;
using Auth.Client.Interfaces;
using Auth.Client.Services;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("auth", (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var url = configuration.GetValue<string>("authService:url");
    client.BaseAddress = new Uri(url);
});
builder.Services.AddScoped<IAuthClient, AuthClient>();
builder.Services.AddBlazoredLocalStorage();
await builder.Build().RunAsync();