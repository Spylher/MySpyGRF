
using Microsoft.EntityFrameworkCore;
using MySpyGRF.Server.Data;
using MySpyGRF.Server.Repositories;
using MySpyGRF.Server.Services;

namespace MySpyGRF.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure endpoint antes de Build
            builder.WebHost.UseUrls("http://0.0.0.0:25776");

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(u => u.UseSqlite("DataSource=app.db"));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();
            app.MapGet("/", () => $"Hello World!");
            app.Run();
        }
    }
}
