using System.IO.Compression;
using System.Net.Http.Json;
using MySpyGRF.Core;
namespace MySpyGRF;

internal class Program
{
    private static async Task Main()
    {
        string zipUrl;

        var login = new LoginRequest
        {
            Username = "spylher",
            Password = "spylher",
        };

        Console.WriteLine("Authenticating...");
        try
        {
            using var client = new HttpClient();
            const string apiUrl = "https://localhost:25776/v1/User/login"; // Altere para a URL real da sua API

            var response = await client.PostAsJsonAsync(apiUrl, login);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result is null)
                throw new Exception("Error to connect server.");

            zipUrl = result.Message;
            //Console.WriteLine($"Login realizado com sucesso! Bem-vindo, {result.Username}");
            //Console.WriteLine($"Mensagem: {result.Message}");
            //Console.WriteLine($"Token: {result.Token}");
        }
        catch (HttpRequestException)
        {
            //Console.WriteLine($"Erro ao fazer login: {ex.Message}");
            Console.ReadKey();
            return;
        }

        var http = new HttpClient();
        var downloadTo = Path.Combine(Path.GetTempPath(), @"MS\MySpyGRF.zip");
        var extractPath = Path.Combine(Path.GetTempPath(), @"MS");

        //@"C:\GRF\MSMemory.zip";
        //const string extractPath = @"C:\GRF";

        Console.WriteLine("Downloading...");
        using (var resp = await http.GetAsync(zipUrl))
        {
            resp.EnsureSuccessStatusCode();
            await using (var fs = File.Create(downloadTo))
                await resp.Content.CopyToAsync(fs);
        }
        Console.WriteLine("ZIP baixado em " + downloadTo);

        Console.WriteLine("Installing...");
        ZipFile.ExtractToDirectory(downloadTo, extractPath);
        Console.WriteLine("Extraído em " + extractPath);


    }
}
