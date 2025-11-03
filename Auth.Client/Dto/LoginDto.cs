using System.ComponentModel.DataAnnotations;
using Auth.Client.Pages;

namespace Auth.Client.Dto;

public class LoginDto
{
    [Required(ErrorMessage = "Поле 'Логин' обязательно")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Поле 'Пароль' обязательно")]
    public string Password { get; set; }
}