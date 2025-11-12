using System.ComponentModel.DataAnnotations;
using Auth.Client.Pages;

namespace Auth.Client.Dto;

public class LoginDto
{
    [Required(ErrorMessage = "Поле 'Логин' обязательно")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Поле 'Пароль' обязательно")]
    public string Password { get; set; } = string.Empty;
}