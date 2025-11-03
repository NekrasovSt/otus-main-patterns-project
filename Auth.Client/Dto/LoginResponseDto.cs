namespace Auth.Client.Dto;

public class LoginResponseDto
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
}