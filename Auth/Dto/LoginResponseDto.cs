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
    public string Token { get; set; }
    /// <summary>
    /// Время окончания токена
    /// </summary>
    [Required]
    public DateTime Expires { get; set; }
}