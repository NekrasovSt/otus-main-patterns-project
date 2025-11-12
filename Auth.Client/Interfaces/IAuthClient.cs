using Auth.Client.Dto;

namespace Auth.Client.Interfaces;

public interface IAuthClient
{
    Task GetTokenAsync(LoginDto dto);
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task AddUserAsync(NewUserDto dto);
    Task ChangePasswordAsync(ChangePasswordDto dto);
}