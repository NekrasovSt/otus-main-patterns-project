using System.ComponentModel.DataAnnotations;

namespace Auth.Client.Dto;

public class NewUserDto
{
    [Required(ErrorMessage = "Поле 'Пароль' обязательно")]
    public string? Login { get; set; }
    [Required(ErrorMessage = "Поле 'Пароль' обязательно")]
    public string? Password { get; set; }
    [Required(ErrorMessage = "Поле 'Повторить пароль' обязательно")]
    [Compare(nameof(Password), ErrorMessage = "Пароли должны совпадать")]
    public string? ConfirmPassword { get; set; }
}