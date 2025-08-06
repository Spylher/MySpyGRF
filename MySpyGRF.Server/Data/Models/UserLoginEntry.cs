using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySpyGRF.Server.Data.Models
{
    public class UserLoginEntry
    {
        [Key]
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string IpAddress { get; set; }
        public required string MachineName { get; set; }
        public required string WindowsUser { get; set; }
        public required string MacAddress { get; set; }
        public bool Success { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [ForeignKey("User")]
        public required string UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
