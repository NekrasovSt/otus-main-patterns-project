using Auth.Model;

namespace Auth.Interfaces;

/// <summary>
/// Доступ к данным пользователей
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Инициализация
    /// </summary>
    Task InitDBAsync();

    /// <summary>
    /// Получить пользователя по ИД
    /// </summary>
    Task<User?> GetUserAsync(string login, CancellationToken token);

    /// <summary>
    /// Получить список пользователей
    /// </summary>
    Task<IEnumerable<User>> GetUsersAsync(CancellationToken token);

    /// <summary>
    /// Обновить пользователя
    /// </summary>
    Task<User?> UpdateUserAsync(User user, CancellationToken token);

    /// <summary>
    /// Добавить пользователя
    /// </summary>
    Task<User> AddUserAsync(User user, CancellationToken token);
}