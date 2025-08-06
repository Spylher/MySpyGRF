using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySpyGRF.Server.Data.Models;
namespace MySpyGRF.Server.Repositories;

public interface IUserRepository
{
    IEnumerable<ApplicationUser> GetAll();
    Task<IEnumerable<ApplicationUser>> GetAllAsync();
    Task<ApplicationUser?> GetByNameAsync(string username);
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<IdentityResult> AddAsync(ApplicationUser user, string password);
    Task<bool> UpdateAsync(ApplicationUser user);
    Task<bool> DeleteAsync(string id);
}
