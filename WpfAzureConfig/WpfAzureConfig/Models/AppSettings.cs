namespace WpfAzureConfig.Models;

/// <summary>
/// Represents the application settings loaded from various configuration sources.
/// This model demonstrates how configuration can come from Azure App Configuration,
/// Key Vault, command line, or local config.json as fallback.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// General application configuration section
    /// </summary>
    public string Name { get; set; } = "WPF Azure Config Demo";
    
    /// <summary>
    /// Feature flag from Azure App Configuration
    /// </summary>
    public bool EnableFeatures { get; set; }
    
    /// <summary>
    /// Sample connection string (could be from Key Vault)
    /// </summary>
    public string? DBpass { get; set; }
    
    /// <summary>
    /// API endpoint configuration
    /// </summary>
    public string? ApiEndpoint { get; set; }
    
    /// <summary>
    /// Environment indicator
    /// </summary>
    public string Environment { get; set; } = "Development";
}

/// <summary>
/// Azure App Configuration specific settings
/// </summary>
public class AzureAppConfigSettings
{
    public const string SectionName = "AzureAppConfiguration";
    
    /// <summary>
    /// Connection string to Azure App Configuration
    /// </summary>
    public string? ConnectionString { get; set; }
    
    /// <summary>
    /// Key Vault endpoint for secret references
    /// </summary>
    public string? KeyVaultEndpoint { get; set; }
    
    /// <summary>
    /// Label for configuration filtering (e.g., "Development", "Production")
    /// </summary>
    public string? Label { get; set; }
}