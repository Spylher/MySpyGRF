using System.ComponentModel.DataAnnotations;
namespace MySpyGRF.Server.Data.Models;

public class UserEntity
{
    [Key]
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public bool Active { get; set; } = true;
    public DateTime DateOfCreation { get; set; } = DateTime.Now;
}
