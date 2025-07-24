using Microsoft.EntityFrameworkCore;
using MySpyGRF.Server.Data.Models;
namespace MySpyGRF.Server.Repositories;

public interface IUserRepository
{
    IEnumerable<UserEntity> GetAll();
    Task<IEnumerable<UserEntity>> GetAllAsync();
    Task<UserEntity?> GetUserIdByName(string username);
    Task<UserEntity?> GetByIdAsync(int id);
    Task<bool> AddAsync(UserEntity user);
    Task<bool> Update(UserEntity user);
    Task<bool> Delete(int id);
}
