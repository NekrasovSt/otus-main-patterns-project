using System.ComponentModel.DataAnnotations;

namespace Auth.Dto;

/// <summary>
/// Пользователь
/// </summary>
public class UserDto
{
    /// <summary>
    /// Логин
    /// </summary>
    [Required]
    public required string Login { get; set; }

    /// <summary>
    /// Ид
    /// </summary>
    [Required]
    public required string Id { get; set; }
}