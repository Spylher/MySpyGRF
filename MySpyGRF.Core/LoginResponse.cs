namespace MySpyGRF.Core;

public class LoginResponse
{
    public required long UserId { get; set; }
    public required string Username { get; set; }
    public required string Message { get; set; }

    // opcional: se for gerar JWT ou outro token
    public required string Token { get; set; }
}