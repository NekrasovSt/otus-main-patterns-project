using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Auth.Helpers;
using Auth.Interfaces;
using Auth.Model;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Services;

public class TokenService : ITokenService
{
    private readonly IKeyStore _keyStore;
    private readonly IUserRepository _userRepository;

    public TokenService(IKeyStore keyStore, IUserRepository userRepository)
    {
        _keyStore = keyStore ?? throw new ArgumentNullException(nameof(keyStore));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<(string? Token, DateTime Expires)> GetTokenAsync(string login, string password, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserAsync(login, cancellationToken);

        if (user == null)
        {
            return (null, default);
        }

        if (!PasswordHasher.VerifyPasswordHash(password, user.Hash, user.Salt))
        {
            return (null, default);
        }

        using var privateKey = RSA.Create();
        privateKey.ImportFromPem(_keyStore.PrivateKey);

        // указываем клеймы. Тут могут быть также кастомные параметры,
        // например телефон или почта пользователя
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, login),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // создаем объект токена с параметрами
        var expires = DateTime.Now.AddHours(1).ToUniversalTime();
        var token = new JwtSecurityToken(
            issuer: "otus",
            audience: "otus",
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(new RsaSecurityKey(privateKey), SecurityAlgorithms.RsaSha256)
        );

        // конвертируем токен в строку
        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public async Task<bool> ChangePassword(string login, string password, string newPassword,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserAsync(login, cancellationToken);

        if (user == null)
        {
            return false;
        }

        if (!PasswordHasher.VerifyPasswordHash(password, user.Hash, user.Salt))
        {
            return false;
        }

        PasswordHasher.CreatePasswordHash(newPassword, out byte[] hash, out var salt);

        user.Hash = hash;
        user.Salt = salt;
        await _userRepository.UpdateUserAsync(user, cancellationToken);
        return true;
    }

    public Task<User> AddUser(string login, string password, CancellationToken cancellationToken)
    {
        var newUser = new User();
        PasswordHasher.CreatePasswordHash(password, out byte[] hash, out var salt);

        newUser.Hash = hash;
        newUser.Salt = salt;
        newUser.Login = login;

        return _userRepository.AddUserAsync(newUser, cancellationToken);
    }

    public Task<IEnumerable<User>> GetUsers(CancellationToken cancellationToken)
    {
        return _userRepository.GetUsersAsync(cancellationToken);
    }
}