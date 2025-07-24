namespace MySpyGRF.Core;

public class LoginResponse
{
    public long UserId { get; set; }
    public string Username { get; set; }
    public string Message { get; set; }

    // opcional: se for gerar JWT ou outro token
    public string Token { get; set; }
}