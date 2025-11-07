using System.ComponentModel.DataAnnotations;

namespace Auth.Dto;

/// <summary>
/// Запрос на получения токена
/// </summary>
public class TokenRequest
{
    /// <summary>
    /// Логин
    /// </summary>
    [Required]
    public required string Login { get; set; }

    /// <summary>
    /// Пароль
    /// </summary>
    [Required]
    public required string Password { get; set; }
}