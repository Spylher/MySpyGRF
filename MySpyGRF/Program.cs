using Figgle.Fonts;
using Microsoft.Win32;
using MySpyGRF.Core;
using Spectre.Console;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
namespace MySpyGRF;
using static WinMessages;

internal class Program
{
    internal static string ZipUrl = string.Empty;
    internal static string RagnarokPath = @"C:\Gravity\Ragnarok";
    internal static string ExistingDataPath = Path.Combine(RagnarokPath, "data");
    internal static string ExistingSkinPath = Path.Combine(RagnarokPath, "skin");

    internal static LoginRequest LoginRequest = new()
    {
        Username = "Giio",
        Password = "Giio",
    };

    private static async Task Main()
    {
        _ = Task.Run(() =>
        {
            var process = Process.GetProcessesByName("ragexe");

            foreach (var proc in process)
                MakeWindowBorderless(GetMainWindowHandleFromPid(proc.Id));
        });

        try
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
                    var ragnarokApp = GetInstalledApplications().FirstOrDefault(app => app.Name.ToLower().Contains("ragnarok"));

                    if (ragnarokApp is not null)
                    {
                        RagnarokPath = ragnarokApp.InstallLocation;
                        if (!Directory.Exists(RagnarokPath))
                        {
                            AnsiConsole.MarkupLine("[bold red]❌ Existing GRF data not found![/]");
                            AnsiConsole.MarkupLine("[bold yellow]Please ensure Ragnarok is installed correctly.[/]");
                            Console.ReadKey();
                            return;
                        }
                    }
                }
            }

            // Authenticating
            Authenticating();

            // Login Message
            AnsiConsole.MarkupLine($"[bold yellow]Login successful![/]");
            AnsiConsole.MarkupLine($"👤 Welcome back, [bold cyan]{LoginRequest.Username}![/]");

            // Downloading
            await Downloading();

            AnsiConsole.MarkupLine("[yellow]GRF installation completed![/]");
            AnsiConsole.MarkupLine("[bold magenta]✔  Enjoy your new GRF features![/]");
            Console.ReadKey();
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine("[bold red]❌ Existing GRF data not found![/]");
            AnsiConsole.MarkupLine("[bold yellow]Please ensure Ragnarok is installed correctly.[/]");
            Console.WriteLine(e);
        }

        Console.ReadLine();
    }

    public class InstalledApp
    {
        public string Name { get; set; }
        public string InstallLocation { get; set; }
    }

    public static List<InstalledApp> GetInstalledApplications()
    {
        var result = new List<InstalledApp>();

        string[] registryKeys = new[]
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };

        foreach (var keyPath in registryKeys)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath))
            {
                if (key == null) continue;

                foreach (var subkeyName in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                    {
                        string name = subkey?.GetValue("DisplayName") as string;
                        string location = subkey?.GetValue("InstallLocation") as string;

                        if (!string.IsNullOrEmpty(name))
                        {
                            result.Add(new InstalledApp
                            {
                                Name = name,
                                InstallLocation = location
                            });
                        }
                    }
                }
            }
        }

        return result;
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

                var http = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true });
                http.Timeout = TimeSpan.FromMinutes(5);
                http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; SpyGRFDownloader/1.0)");

                var downloadTo = Path.Combine(Path.GetTempPath(), @"MySpyGRF.zip");
                if (!Directory.Exists(extractPath))
                    Directory.CreateDirectory(extractPath);

                using (var resp = await http.GetAsync(ZipUrl, HttpCompletionOption.ResponseHeadersRead))
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

    #region WinApi Methods
    public static IntPtr GetMainWindowHandleFromPid(int targetPid)
    {
        var process = Process.GetProcessById(targetPid);
        var found = IntPtr.Zero;

        foreach (ProcessThread thread in process.Threads)
        {
            EnumThreadWindows(thread.Id, (hWnd, _) =>
            {
                found = hWnd;
                return false;
            }, IntPtr.Zero);

            if (found != IntPtr.Zero)
                break;
        }

        return found;
    }

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    private delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

    [DllImport("psapi.dll", SetLastError = true)]
    internal static extern bool EnumProcessModules(nint hProcess, [Out] nint[] lphModule, uint cb, out uint lpcbNeeded);

    [DllImport("user32.dll")]
    static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

    public static void MakeWindowBorderless(IntPtr hWnd)
    {
        // Obter o estilo atual da janela
        uint SWP_FRAMECHANGED = 0x0020;
        uint SWP_NOSIZE = 0x0001;
        uint SWP_NOMOVE = 0x0002;
        var GWL_STYLE = -16;

        int style = GetWindowLong(hWnd, GWL_STYLE);

        // Remover as bordas
        style &= ~(int)WS_CAPTION;
        style &= ~(int)WS_THICKFRAME;
        style &= ~(int)WS_MINIMIZEBOX;
        style &= ~(int)WS_MAXIMIZEBOX;
        style &= ~(int)WS_SYSMENU;

        // Definir o novo estilo
        SetWindowLong(hWnd, GWL_STYLE, style);

        // Atualizar a posição da janela para aplicar o novo estilo
        SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED);
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    #endregion
}
