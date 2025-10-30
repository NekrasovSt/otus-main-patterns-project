using System.Security.Claims;
using Auth;
using Auth.Dto;
using Auth.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
Startup.SetupServices(builder);
var app = builder.Build();

await Startup.SetupMiddleware(app);

app.MapPost("/token",
    async (ITokenService tokenService, [FromBody] CredentialsDto credentialsDto,
        CancellationToken cancellationToken) =>
    {
        var token = await tokenService.GetTokenAsync(credentialsDto.Login, credentialsDto.Password, cancellationToken);
        if (token == null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(new { Token = token });
    });
app.MapPost("/change-password",
    async (ITokenService tokenService, HttpContext context, [FromBody] ChangePasswordDto credentialsDto,
        CancellationToken cancellationToken) =>
    {
        var login = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (login == null)
        {
            return Results.Unauthorized();
        }

        var result = await tokenService.ChangePassword(login, credentialsDto.Password,
            credentialsDto.NewPassword, cancellationToken);
        if (!result)
        {
            return Results.BadRequest();
        }

        return Results.Ok();
    }).RequireAuthorization("Default");
app.MapPost("/add-user",
    async (ITokenService tokenService, [FromBody] NewUserDto newUserDto,
        CancellationToken cancellationToken) =>
    {
        await tokenService.AddUser(newUserDto.Login, newUserDto.Password, cancellationToken);
        return Results.Ok(new NewUserDto
        {
            Login = newUserDto.Login,
        });
    }).RequireAuthorization("Default");


app.Run();