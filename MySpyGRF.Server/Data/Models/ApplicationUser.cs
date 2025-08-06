using Microsoft.AspNetCore.Identity;
namespace MySpyGRF.Server.Data.Models;

public class ApplicationUser : IdentityUser
{
    public string ProfilesJson { get; set; } = string.Empty;
    public string GrfUrl { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
    public DateTime DateOfCreation { get; set; } = DateTime.UtcNow;
}