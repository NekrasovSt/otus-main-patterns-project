using Auth.Model;

namespace Auth.Interfaces;

public interface IUserRepository
{
    Task InitDBAsync();
    Task<User?> GetUserAsync(string login, CancellationToken token);
    Task<IEnumerable<User>> GetUsersAsync(CancellationToken token);
    Task<User?> UpdateUserAsync(User user, CancellationToken token);
    Task<User> AddUserAsync(User user, CancellationToken token);
}