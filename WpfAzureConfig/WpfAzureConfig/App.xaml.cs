using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;
using System.Windows;
using WpfAzureConfig.Services;
using WpfAzureConfig.ViewModels;

namespace WpfAzureConfig;

/// <summary>
/// Main application class using .NET Generic Host for dependency injection,
/// configuration management, and logging with Serilog.
/// Demonstrates modern WPF architecture with Azure App Configuration integration.
/// </summary>
public partial class App : Application
{
    private IHost? _host;
    internal static IConfigurationRefresher? _refresher; 

    /// <summary>
    /// Application startup - configures host before showing UI
    /// </summary>
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Build and start the host
        _host = CreateHostBuilder(e.Args).Build();
        await _host.StartAsync();

        // Resolve and show the main window
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    /// <summary>
    /// Application shutdown - gracefully stop the host
    /// </summary>
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

    /// <summary>
    /// Creates and configures the application host with:
    /// - Configuration from Azure App Configuration, Key Vault, command line, and config.json
    /// - Serilog for structured logging
    /// - Dependency injection for services and ViewModels
    /// </summary>
    private IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, config) =>
        {
            // Clear default configuration sources
            config.Sources.Clear();

            // 1. Base configuration from appsettings.json (always available)
            var basePath = Directory.GetCurrentDirectory();
            config.SetBasePath(basePath);
            config.AddJsonFile("config.json", optional: true, reloadOnChange: true);
            config.AddJsonFile($"config.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            // Build intermediate configuration to read Azure settings
            var intermediateConfig = config.Build();
            var azureConnectionString = intermediateConfig["AzureAppConfiguration:ConnectionString"];
            //var keyVaultEndpoint = intermediateConfig["AzureAppConfiguration:KeyVaultEndpoint"];
            var configLabel = intermediateConfig["AzureAppConfiguration:Label"];

            // 2. Try to add Azure App Configuration if connection string is available
            if (!string.IsNullOrEmpty(azureConnectionString))
            {
                try
                {
                    var keyVaultCredential = new AzureCliCredential();

                    config.AddAzureAppConfiguration(options =>
                    {
                        options.Connect(azureConnectionString)
                            // Select configuration with optional label filtering
                            .Select("Gopas:TechEd26:SuperApp:*", configLabel ?? LabelFilter.Null)
                            // Configure refresh for dynamic configuration updates
                            .ConfigureRefresh(refresh =>
                            {
                                refresh.Register("Gopas:TechEd26:SuperApp:Name", refreshAll: true)
                                    .SetRefreshInterval(TimeSpan.FromMinutes(1));
                            });

                        _refresher = options.GetRefresher();

                        // Use DefaultAzureCredential for authentication
                        // This supports multiple authentication methods: Managed Identity, Azure CLI, Visual Studio, etc.
                        options.ConfigureKeyVault(kv =>
                        {
                            kv.SetSecretResolver(async secretUri =>
                            {
                                try
                                {
                                    var client = new SecretClient(
                                        new Uri($"{secretUri.Scheme}://{secretUri.Host}"),
                                        keyVaultCredential);

                                    KeyVaultSecret secret =
                                        await client.GetSecretAsync(
                                            GetSecretName(secretUri),
                                            GetSecretVersion(secretUri));

                                    return secret.Value;
                                }
                                catch
                                {
                                    // Entra ID není dostupné / uživatel nemá práva / secret nejde přečíst
                                    return "__KEY_VAULT_SECRET_NOT_AVAILABLE__";
                                }
                            });
                        });
                    });

                    Log.Information("Azure App Configuration connected successfully");
                }
                catch (Exception ex)
                {
                    // If Azure connection fails, fall back to local config.json
                    Log.Warning(ex, "Failed to connect to Azure App Configuration. Using local config.json as fallback.");
                }
            }
            else
            {
                Log.Information("No Azure App Configuration connection string found. Using local config.json.");
            }

            // 4. Command line arguments (highest priority - can override everything)
            config.AddCommandLine(args);
        })
        .UseSerilog((context, services, configuration) =>
        {
            // Configure Serilog from appsettings and with default settings
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/app-.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");

            Log.Information("Serilog configured successfully");
        })
        .ConfigureServices((context, services) =>
        {
            // Register application services
            services.AddSingleton<IConfigurationService, ConfigurationService>();

            // Register ViewModels for dependency injection
            services.AddTransient<MainViewModel>();

            // Register Views (Windows)
            services.AddSingleton<MainWindow>();

            Log.Information("Services registered successfully");
        });

    static string GetSecretName(Uri secretUri)
    {
        // /secrets/Gopas-TE2026-DB/{version?}
        var parts = secretUri.AbsolutePath
            .Split('/', StringSplitOptions.RemoveEmptyEntries);

        return parts[1];
    }

    static string? GetSecretVersion(Uri secretUri)
    {
        var parts = secretUri.AbsolutePath
            .Split('/', StringSplitOptions.RemoveEmptyEntries);

        return parts.Length >= 3 ? parts[2] : null;
    }
}
