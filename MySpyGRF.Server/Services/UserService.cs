using Microsoft.EntityFrameworkCore;
using MySpyGRF.Server.Data;
using MySpyGRF.Server.Data.Models;
namespace MySpyGRF.Server.Services;

public class UserService(AppDbContext dbContext) : IUserService
{
    public readonly AppDbContext DbContext = dbContext;

    //public async Task<UserEntity> RegisterAsync(string username, string password, CancellationToken ct)
    //{
    //    if (await DbContext.Users.AnyAsync(u => u.Username == username, ct))
    //        throw new Exception("User is already registered.");

    //    var hash = BCrypt.Net.BCrypt.HashPassword(password);
    //    var user = new UserEntity { Username = username, PasswordHash = hash };

    //    DbContext.Users.Add(user);
    //    await DbContext.SaveChangesAsync(ct);

    //    return user;
    //}


    //public async Task<UserEntity?> AuthenticateAsync(string username, string password, CancellationToken ct)
    //{
    //    var user = await DbContext.Users
    //        .SingleOrDefaultAsync(u => u.Username == username && u.Active, ct);

    //    if (user == null)
    //        return null;

    //    if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
    //        return null;

    //    return user;
    //}
}