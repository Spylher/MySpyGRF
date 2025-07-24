using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace MySpyGRF.Server.Data.Models
{
    public class LoginLogEntity
    {
        [Key]
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required string Username { get; set; }
        public required string IpAddress { get; set; }
        public required string MachineName { get; set; }
        public required string WindowsUser { get; set; }
        public required string MacAddress { get; set; }
        public bool Success { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
