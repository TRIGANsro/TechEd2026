using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using WpfAzureConfig.Services;

namespace WpfAzureConfig.ViewModels;

/// <summary>
/// Main ViewModel for the application following MVVM pattern.
/// Demonstrates dependency injection and data binding with Azure configuration.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<MainViewModel> _logger;

    [ObservableProperty]
    private string _title = "Azure App Configuration Demo";

    [ObservableProperty]
    private string _configurationSource = "Loading...";

    [ObservableProperty]
    private ObservableCollection<ConfigurationItem> _configurationItems = [];

    [ObservableProperty]
    private string _statusMessage = "Ready";

    public MainViewModel(IConfigurationService configurationService, ILogger<MainViewModel> logger)
    {
        _configurationService = configurationService;
        _logger = logger;
        
        _logger.LogInformation("MainViewModel initialized");
        
        // Load initial configuration
        LoadConfiguration();
    }

    /// <summary>
    /// Loads configuration from the configuration service
    /// </summary>
    [RelayCommand]
    private async void LoadConfiguration()
    {
        try
        {
            _logger.LogInformation("Loading configuration...");
            
            StatusMessage = "Loading configuration...";
            
            // Get configuration source
            ConfigurationSource = _configurationService.GetConfigurationSource();
            
            // Get all configuration items
            var items = await _configurationService.GetAllConfigurationItemsAsync();
            ConfigurationItems = new ObservableCollection<ConfigurationItem>(items);
            
            StatusMessage = $"Configuration loaded successfully from: {ConfigurationSource}";
            _logger.LogInformation("Configuration loaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configuration");
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Refreshes configuration from sources
    /// </summary>
    [RelayCommand]
    private void RefreshConfiguration()
    {
        _logger.LogInformation("Refreshing configuration...");
        LoadConfiguration();
    }
}