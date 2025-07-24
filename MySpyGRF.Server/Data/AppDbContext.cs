using Microsoft.EntityFrameworkCore;
using MySpyGRF.Server.Data.Models;
namespace MySpyGRF.Server.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<LoginLogEntity> UserLogEntities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite("DataSource=app.db;cache=shared");
    }
}
