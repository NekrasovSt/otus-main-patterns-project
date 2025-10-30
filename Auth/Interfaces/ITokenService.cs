using Auth.Model;

namespace Auth.Services;

public interface ITokenService
{
    Task<string?> GetTokenAsync(string login, string password, CancellationToken cancellationToken);

    Task<bool> ChangePassword(string login, string password, string newPassword, CancellationToken cancellationToken);
    Task<User> AddUser(string login, string password, CancellationToken cancellationToken);
}