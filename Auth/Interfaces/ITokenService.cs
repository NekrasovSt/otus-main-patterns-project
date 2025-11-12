using Auth.Model;

namespace Auth.Services;

/// <summary>
/// Сервис работы с токенами
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Получить токен
    /// </summary>
    Task<(string? Token, DateTime Expires)> GetTokenAsync(string login, string password,
        CancellationToken cancellationToken);

    /// <summary>
    /// Сменить пароль
    /// </summary>
    Task<bool> ChangePassword(string login, string password, string newPassword, CancellationToken cancellationToken);

    /// <summary>
    /// Добавить пользователя
    /// </summary>
    Task<User> AddUser(string login, string password, CancellationToken cancellationToken);

    /// <summary>
    /// Получить пользователей
    /// </summary>
    Task<IEnumerable<User>> GetUsers(CancellationToken cancellationToken);
}