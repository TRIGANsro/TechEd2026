using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Windows;
using WPF2DBinding.Services;
using WPF2DBinding.ViewModels;

namespace WPF2DBinding;

public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        EnsureDataFolderExists();

        var builder = Host.CreateApplicationBuilder();

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                path: Path.Combine("logs", "app-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();

        builder.Services.AddSerilog();

        // Register services
        builder.Services.AddSingleton<IImageService, ImageService>();
        builder.Services.AddSingleton<IDataService, DataService>();
        
        // Register ViewModels
        builder.Services.AddSingleton<MainViewModel>();
        
        // Register Views
        builder.Services.AddSingleton<MainWindow>();

        _host = builder.Build();

        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        logger.LogInformation("Application starting");

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private static void EnsureDataFolderExists()
    {
        var dataFolder = "data";
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
    }
}
