using Microsoft.EntityFrameworkCore;
using MySpyGRF.Server.Data;
using MySpyGRF.Server.Data.Models;
namespace MySpyGRF.Server.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public readonly AppDbContext DbContext = dbContext;

    public IEnumerable<UserEntity> GetAll() => DbContext.Users.ToList();
    public async Task<IEnumerable<UserEntity>> GetAllAsync() => await DbContext.Users.ToListAsync();

    public Task<UserEntity?> GetByIdAsync(int id) => DbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    public Task<UserEntity?> GetUserIdByName(string username) => DbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<bool> AddAsync(UserEntity userEntity)
    {
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Username == userEntity.Username);
        if (user is not null)
            return false;

        await DbContext.Users.AddAsync(userEntity);
        await DbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Update(UserEntity userEntity)
    {
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == userEntity.Id);

        if (user == null)
            return false;

        DbContext.Users.Update(user);
        await DbContext.SaveChangesAsync();
        return true;

    }

    public async Task<bool> Delete(int id)
    {
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return false;
        
        DbContext.Users.Remove(user);
        await DbContext.SaveChangesAsync();
        return true;
    }
}
