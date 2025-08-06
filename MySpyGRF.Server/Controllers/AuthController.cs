using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySpyGRF.Core;
using MySpyGRF.Server.Data;
using MySpyGRF.Server.Data.Models;
using MySpyGRF.Server.Repositories;
using MySpyGRF.Server.Services;
namespace MySpyGRF.Server.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly AppDbContext _appDbContext;
    private const string GrfUrl = "https://github.com/Spylher/MySpyGRF/archive/refs/heads/grf.zip";

    public AuthController(IUserRepository userRepository, AuthService authService, AppDbContext dbContext)
    {
        _authService = authService;
        _userRepository = userRepository;
        _appDbContext = dbContext;
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetSecretData()
    {
        return Ok("🔒 Essa rota está protegida!");
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var user = await _userRepository.GetByNameAsync(req.Username);
        
        var token = await _authService.AuthenticateAsync(req.Username, req.Password);

        if (token == null || user == null)
            return Unauthorized("Usuário ou senha inválidos.");

        var userId = Guid.Parse(user.Id);
        var userLog = new UserLoginEntry
        {
            UserId = user.Id,
            Username = req.Username,
            IpAddress = GetClientIp(HttpContext) ?? "Unknown",
            MachineName = req.MachineName,
            WindowsUser = req.WindowsUser,
            MacAddress = req.MacAddress,
            Success = true,
        };

        if (!string.IsNullOrEmpty(req.ProfilesJson) && req.ProfilesJson.ToLower() != "string")
        {
            Console.WriteLine(req.ProfilesJson);
            user.ProfilesJson = req.ProfilesJson;
            await _userRepository.UpdateAsync(user);
        }

        var response = new LoginResponse
        {
            UserId = userId,
            Username = user.UserName!,
            ProfilesJson = user.ProfilesJson,
            Message = "None",
            GrfUrl = user.GrfUrl,
            Token = token,
        };

        await _appDbContext.UserLoginEntries.AddAsync(userLog);
        await _appDbContext.SaveChangesAsync();
        return Ok(response);
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var user = new ApplicationUser { UserName = req.Username };
        var result = await _userRepository.AddAsync(user, req.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("Usuário criado com sucesso.");
    }

    private string? GetClientIp(HttpContext context)
    {
        // Verifica cabeçalhos X-Forwarded-For (caso tenha proxy)
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            return forwardedFor.FirstOrDefault() ?? context.Connection.RemoteIpAddress?.ToString();

        // Fallback para IP direto
        return context.Connection.RemoteIpAddress?.ToString();
    }
}