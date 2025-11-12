using System.ComponentModel.DataAnnotations;

namespace Auth.Dto;

/// <summary>
/// Смена пароля
/// </summary>
public class ChangePasswordDto
{
    /// <summary>
    /// Старый пароль
    /// </summary>
    [Required]
    public required string Password { get; set; }
    /// <summary>
    /// Новый пароль
    /// </summary>
    [Required]
    public required string NewPassword { get; set; }
}