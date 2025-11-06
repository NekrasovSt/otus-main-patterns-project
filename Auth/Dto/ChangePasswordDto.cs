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
    public string Password { get; set; }
    /// <summary>
    /// Новый пароль
    /// </summary>
    [Required]
    public string NewPassword { get; set; }
}