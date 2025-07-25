using Figgle.Fonts;
using MySpyGRF.Core;
using Spectre.Console;
using System.IO.Compression;
using System.Net.Http.Json;

namespace MySpyGRF;
internal class Program
{
    internal static string ZipUrl = string.Empty;
    internal static string RagnarokPath = @"C:\Gravity\Ragnarok";
    internal static string ExistingDataPath = Path.Combine(RagnarokPath, "data");
    internal static string ExistingSkinPath = Path.Combine(RagnarokPath, "skin");
    internal static LoginRequest LoginRequest = new()
    {
        Username = "FerraZ",
        Password = "FerraZ",
    };

    private static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var banner = FiggleFonts.Standard.Render("Spy GRF");
        Console.WriteLine(banner);

        // Verify if game is already installed
        if (!Directory.Exists(RagnarokPath))
        {
            RagnarokPath = @"D:\Gravity\Ragnarok";
            if (!Directory.Exists(RagnarokPath))
            {
                Console.WriteLine("❌ Pasta Ragnarok não encontrada.");
                return;
            }
        }

        // Authenticating
        Authenticating();

        // Login Message
        AnsiConsole.MarkupLine($"[bold yellow]Login successful![/]");
        AnsiConsole.MarkupLine($"👤 Welcome back, [bold cyan]{LoginRequest.Username}[/]!");

        // Downloading
        await Downloading();

        AnsiConsole.MarkupLine("[yellow]GRF installation completed![/]");
        AnsiConsole.MarkupLine("[bold magenta]✔  Enjoy your new GRF features![/]");
        Console.ReadKey();
    }

    private static async void Authenticating()
    {
        await AnsiConsole.Status()
            .StartAsync("Authenticating...", _ =>
            {
                try
                {
                    using var client = new HttpClient();
                    const string apiUrl = "http://193.180.213.177:25776/v1/User/login"; // Altere para a URL real da sua API

                    var response = client.PostAsJsonAsync(apiUrl, LoginRequest).Result;

                    response.EnsureSuccessStatusCode();

                    var result = response.Content.ReadFromJsonAsync<LoginResponse>().Result;

                    if (result is null)
                        throw new Exception("Error to connect server.");

                    ZipUrl = result.Message;
                    //Console.WriteLine($"Login realizado com sucesso! Bem-vindo, {result.Username}");
                    //Console.WriteLine($"Mensagem: {result.Message}");
                    //Console.WriteLine($"Token: {result.Token}");
                    Console.WriteLine();
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Erro ao fazer login: {ex.Message}");
                    Console.ReadKey();
                }

                return Task.CompletedTask;
            });

        //zipUrl = @"https://github.com/Spylher/MySpyGRF/archive/refs/heads/main.zip";
        AnsiConsole.MarkupLine($"[bold yellow]Authenticating...[/]");
        AnsiConsole.MarkupLine("[bold green]⚙  Authentication successful![/]");
        Console.WriteLine();
    }

    private static async Task Downloading()
    {
        await AnsiConsole.Status()
            .StartAsync("⏳ Downloading and Installing GRF...", async _ =>
            {
                //Download the zip file
                var extractPath = Path.Combine(Path.GetTempPath(), @"MySpy");

                if (Directory.Exists(extractPath))
                    Directory.Delete(extractPath, true);

                Directory.CreateDirectory(extractPath);

                var http = new HttpClient();
                var downloadTo = Path.Combine(Path.GetTempPath(), @"MySpyGRF.zip");
                if (!Directory.Exists(extractPath))
                    Directory.CreateDirectory(extractPath);

                using (var resp = await http.GetAsync(ZipUrl))
                {
                    resp.EnsureSuccessStatusCode();
                    using (var fs = File.Create(downloadTo))
                        await resp.Content.CopyToAsync(fs);
                }

                //existingDataPath
                ZipFile.ExtractToDirectory(downloadTo, extractPath);

                if (File.Exists(downloadTo))
                    File.Delete(downloadTo);

                if (Directory.Exists(ExistingDataPath))
                    Directory.Delete(ExistingDataPath, recursive: true);

                if (Directory.Exists(ExistingSkinPath))
                    Directory.Delete(ExistingSkinPath, recursive: true);

                var newDataPath = Path.Combine(Path.GetTempPath(), @"MySpy/MySpyGRF-grf/data");
                CopyDirectory(newDataPath, ExistingDataPath);

                var newSkinPath = Path.Combine(Path.GetTempPath(), @"MySpy/MySpyGRF-grf/skin");
                CopyDirectory(newSkinPath, ExistingSkinPath);

                Directory.Delete(newDataPath, recursive: true);
                Directory.Delete(newSkinPath, recursive: true);
                Console.WriteLine();

                return Task.CompletedTask;
            });
    }

    private static void CopyDirectory(string sourceDir, string targetDir)
    {
        Directory.CreateDirectory(targetDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var destFile = Path.Combine(targetDir, Path.GetFileName(file));
            File.Copy(file, destFile, overwrite: true);
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            var destSubDir = Path.Combine(targetDir, Path.GetFileName(dir));
            CopyDirectory(dir, destSubDir);
        }
    }
}
