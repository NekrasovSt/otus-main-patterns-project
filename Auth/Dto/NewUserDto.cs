using System.ComponentModel.DataAnnotations;

namespace Auth.Dto;

/// <summary>
/// Новый пользователь
/// </summary>
public class NewUserDto
{
    /// <summary>
    /// Логин
    /// </summary>
    [Required]
    public string Login { get; set; }

    /// <summary>
    /// Пароль
    /// </summary>
    [Required]
    public string Password { get; set; }
}