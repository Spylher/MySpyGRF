using Microsoft.AspNetCore.Mvc;
using MySpyGRF.Core;
using MySpyGRF.Server.Data;
using MySpyGRF.Server.Data.Models;
using MySpyGRF.Server.Repositories;
using MySpyGRF.Server.Services;
using LoginRequest = MySpyGRF.Core.LoginRequest;
using RegisterRequest = MySpyGRF.Core.LoginRequest;

namespace MySpyGRF.Server.Controllers;

[Route("v1/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    public readonly AppDbContext DbContext;
    public readonly IUserRepository UserRepository;
    public readonly IUserService UserService;

    public UserController(IUserRepository userRepository, IUserService userService, AppDbContext dbContext)
    {
        UserRepository = userRepository;
        UserService = userService;
        DbContext = dbContext; //for logging... will be removed in a future update.
    }

    [HttpGet()]
    public async Task<IActionResult> GetUsersAsync(CancellationToken ct)
    {
        var users = await UserRepository.GetAllAsync();
        return Ok(users);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteUserAsync(int id, CancellationToken ct)
    {
        if (!await UserRepository.Delete(id))
            return BadRequest("User not found.");

        return Ok();
    }

    [HttpPut()]
    public async Task<IActionResult> UpdateUser(UserEntity userEntity, CancellationToken ct)
    {
        if (!await UserRepository.Update(userEntity))
            return BadRequest("User not found.");

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var user = await UserService
            .AuthenticateAsync(request.Username, request.Password, ct);

        if (user is null)
        {
            var userRequest = await UserRepository.GetUserIdByName(request.Username);

            if (userRequest is not null)
            {
                var userRequestLog = new LoginLogEntity
                {
                    UserId = userRequest.Id,
                    Username = userRequest.Username,
                    IpAddress = GetClientIp(HttpContext) ?? "Unknown",
                    MachineName = request.MachineName,
                    WindowsUser = request.WindowsUser,
                    MacAddress = request.MacAddress,
                    Success = false
                };

                await DbContext.UserLogEntities.AddAsync(userRequestLog, ct);
                await DbContext.SaveChangesAsync(ct);
            }

            return Unauthorized(new { Message = "Username or password is invalid." });
        }

        var userLog = new LoginLogEntity
        {
            UserId = user.Id,
            Username = user.Username,
            IpAddress = GetClientIp(HttpContext) ?? "Unknown",
            MachineName = request.MachineName,
            WindowsUser = request.WindowsUser,
            MacAddress = request.MacAddress,
            Success = true
        };

        await DbContext.UserLogEntities.AddAsync(userLog, ct);
        await DbContext.SaveChangesAsync(ct);

        // 2) generate JWT, e.g:
        // var token = _jwtService.GenerateToken(user);
        var grfUrl = "https://github.com/Spylher/MySpyGRF/archive/refs/heads/grf.zip";

        var response = new LoginResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Message = grfUrl,
            //Token = ""
        };

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req, CancellationToken ct)
    {
        try
        {
            var user = await UserService.RegisterAsync(req.Username, req.Password, ct);
            return Ok(new { user.Id, user.Username });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
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
