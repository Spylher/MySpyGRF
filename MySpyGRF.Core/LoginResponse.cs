namespace MySpyGRF.Core;

public class LoginResponse
{
    public required Guid UserId { get; set; }
    public required string Username { get; set; }
    public required string Message { get; set; }
    public required string ProfilesJson { get; set; }
    public required string GrfUrl { get; set; }

    // opcional: se for gerar JWT ou outro token
    public required string Token { get; set; }
}