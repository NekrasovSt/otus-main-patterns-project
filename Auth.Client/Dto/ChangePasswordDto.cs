using System.ComponentModel.DataAnnotations;

namespace Auth.Client.Dto;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Поле 'Старый пароль' обязательно")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Поле 'Новый пароль' обязательно")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Поле 'Повторить пароль' обязательно")]
    [Compare(nameof(NewPassword), ErrorMessage = "Пароли должны совпадать")]
    public string ConfirmPassword { get; set; }
}