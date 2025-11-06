using System.Security.Claims;
using Auth;
using Auth.Dto;
using Auth.Services;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);
Startup.SetupServices(builder);
var app = builder.Build();

await Startup.SetupMiddleware(app);

app.MapPost("/token",
        [SwaggerOperation("Получить токен")] async (ITokenService tokenService,
            [FromBody] TokenRequest tokenRequest,
            CancellationToken cancellationToken) =>
        {
            var result =
                await tokenService.GetTokenAsync(tokenRequest.Login, tokenRequest.Password, cancellationToken);
            if (result.Token == null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(new LoginResponseDto { Token = result.Token, Expires = result.Expires });
        })
    .Produces<LoginResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);
app.MapPost("/change-password",
        [SwaggerOperation("Сменить пароль для текущего пользователя")]
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
        })
    .RequireAuthorization("Default")
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status400BadRequest);
app.MapPost("/add-user",
        [SwaggerOperation("Добавить пользователя")]
        async (ITokenService tokenService, [FromBody] NewUserDto newUserDto,
            CancellationToken cancellationToken) =>
        {
            await tokenService.AddUser(newUserDto.Login, newUserDto.Password, cancellationToken);
            return Results.Ok(new NewUserDto
            {
                Login = newUserDto.Login,
            });
        })
    .RequireAuthorization("Default")
    .Produces(StatusCodes.Status200OK);
app.MapGet("/users",
        [SwaggerOperation("Список пользователей")]
        async (ITokenService tokenService, CancellationToken cancellationToken) =>
        {
            var users = await tokenService.GetUsers(cancellationToken);
            return Results.Ok(users.Adapt<IEnumerable<UserDto>>());
        }).RequireAuthorization("Default")
    .Produces(StatusCodes.Status200OK);

app.Run();