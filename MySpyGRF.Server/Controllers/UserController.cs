using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySpyGRF.Server.Data.Models;
using MySpyGRF.Server.Repositories;
using MySpyGRF.Server.Services;
namespace MySpyGRF.Server.Controllers;

[Route("v1/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    public readonly IUserRepository UserRepository;
    public readonly IUserService UserService;
    public readonly AuthService AuthService;

    public UserController(IUserRepository userRepository, IUserService userService, AuthService authService)
    {
        UserRepository = userRepository;
        UserService = userService;
        AuthService = authService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetUsersAsync(CancellationToken ct)
    {
        var users = await UserRepository.GetAllAsync();
        return Ok(users);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUserAsync(int id, CancellationToken ct)
    {
        if (!await UserRepository.DeleteAsync(id.ToString()))
            return BadRequest("User not found.");

        return Ok();
    }

    [HttpPut()]
    public async Task<IActionResult> UpdateUserAsync(ApplicationUser user, CancellationToken ct)
    {
        if (!await UserRepository.UpdateAsync(user))
            return BadRequest("User not found.");

        return Ok();
    }
}
