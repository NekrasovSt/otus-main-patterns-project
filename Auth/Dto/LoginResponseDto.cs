using System.ComponentModel.DataAnnotations;

namespace Auth.Dto;

/// <summary>
/// Ответ на получение токена
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// Токен
    /// </summary>
    [Required]
    public required string Token { get; set; }
    /// <summary>
    /// Время окончания токена
    /// </summary>
    [Required]
    public required DateTime Expires { get; set; }
}