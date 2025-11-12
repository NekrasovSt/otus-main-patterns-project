namespace Auth.Client.Dto;

public class LoginResponseDto
{
    public required string Token { get; set; }
    public required DateTime Expires { get; set; }
}