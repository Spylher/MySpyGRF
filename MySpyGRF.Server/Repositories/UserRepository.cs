using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySpyGRF.Server.Data.Models;
namespace MySpyGRF.Server.Repositories;

public class UserRepository(UserManager<ApplicationUser> userManager) : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public IEnumerable<ApplicationUser> GetAll() => _userManager.Users.ToList();
    public async Task<IEnumerable<ApplicationUser>> GetAllAsync() => await _userManager.Users.ToListAsync();

    public Task<ApplicationUser?> GetByIdAsync(string id) => _userManager.FindByIdAsync(id);
    public Task<ApplicationUser?> GetByNameAsync(string username) => _userManager.FindByNameAsync(username);

    public async Task<IdentityResult> AddAsync(ApplicationUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        
        if (result.Succeeded)
            await _userManager.AddToRoleAsync(user, "User");
        
        return result;
    }

    public async Task<bool> UpdateAsync(ApplicationUser updatedUser)
    {
        var user = await _userManager.FindByIdAsync(updatedUser.Id);
        if (user == null) 
            return false;

        user.UserName = updatedUser.UserName;
        user.Email = updatedUser.Email;
        user.Active = updatedUser.Active;
        user.GrfUrl = updatedUser.GrfUrl;
        user.ProfilesJson = updatedUser.ProfilesJson;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) 
            return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}
