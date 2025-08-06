using System.Net.NetworkInformation;
namespace MySpyGRF.Core;

public class LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string ProfilesJson { get; set; } = string.Empty;
    public string MachineName { get; set; } = Environment.MachineName;
    public string WindowsUser { get; set; } = Environment.UserName;
    public string MacAddress { get; set; } = NetworkInterface
        .GetAllNetworkInterfaces()
        .FirstOrDefault(n => n.OperationalStatus == OperationalStatus.Up)?
        .GetPhysicalAddress()
        .ToString() ?? "";
}
