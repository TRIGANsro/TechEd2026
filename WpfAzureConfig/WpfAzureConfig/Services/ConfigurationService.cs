using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WpfAzureConfig.Models;

namespace WpfAzureConfig.Services;

/// <summary>
/// Service responsible for managing and providing access to application configuration.
/// Demonstrates real-time configuration values from multiple sources.
/// </summary>
public interface IConfigurationService
{
    Task<AppSettings> GetCurrentSettingsAsync();
    string GetConfigurationSource();
    Task<IEnumerable<ConfigurationItem>> GetAllConfigurationItemsAsync();
}

/// <summary>
/// Represents a single configuration key-value pair with its source
/// </summary>
public record ConfigurationItem(string Key, string? Value, string Source);

public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        _logger.LogInformation("ConfigurationService initialized");
    }

    /// <summary>
    /// Retrieves current application settings from configuration
    /// </summary>
    public async Task<AppSettings> GetCurrentSettingsAsync()
    {
        if (App._refresher is not null)
        {
            await App._refresher.TryRefreshAsync();
        }

        var settings = new AppSettings();
        _configuration.GetSection("Gopas:TechEd26:SuperApp").Bind(settings);
        
        _logger.LogDebug("Retrieved current settings: {@Settings}", settings);
        return settings;
    }

    /// <summary>
    /// Determines which configuration source is being used
    /// </summary>
    public string GetConfigurationSource()
    {
        var azureConfig = _configuration.GetSection(AzureAppConfigSettings.SectionName)
            .Get<AzureAppConfigSettings>();
        
        if (!string.IsNullOrEmpty(azureConfig?.ConnectionString))
        {
            return "Azure App Configuration + Key Vault";
        }
        
        return "Local config.json (Fallback)";
    }

    /// <summary>
    /// Gets all configuration items for display in UI
    /// </summary>
    public async Task<IEnumerable<ConfigurationItem>> GetAllConfigurationItemsAsync()
    {
        var items = new List<ConfigurationItem>();
        
        // Add application settings
        var settings = await GetCurrentSettingsAsync();
        items.Add(new ConfigurationItem("ApplicationName", settings.Name, "Azure/Config"));
        items.Add(new ConfigurationItem("Environment", settings.Environment, "Azure/Config"));
        items.Add(new ConfigurationItem("EnableFeatures", settings.EnableFeatures.ToString(), "Azure/Config"));
        items.Add(new ConfigurationItem("ApiEndpoint", settings.ApiEndpoint ?? "Not Set", "Azure/Config"));
        items.Add(new ConfigurationItem("DBpass", settings.DBpass , "Key Vault/Config"));
        
        // Add source information
        items.Add(new ConfigurationItem("ConfigurationSource", GetConfigurationSource(), "System"));
        
        _logger.LogInformation("Retrieved {Count} configuration items", items.Count);
        return items;
    }
}