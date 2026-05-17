# WPF Azure App Configuration Demo

This is a demonstration application showing how to use Azure App Configuration with Azure Key Vault in a WPF application using .NET 10.

## Features

1. **Generic Host Integration** - Uses `Microsoft.Extensions.Hosting` for modern application architecture
2. **Serilog Logging** - Structured logging to console and file
3. **Multi-Source Configuration**:
   - Azure App Configuration (primary)
   - Azure Key Vault (for secrets via App Configuration references)
   - Command Line Arguments
   - Local `config.json` (fallback when Azure is unavailable)
4. **MVVM Pattern** - Clean separation using CommunityToolkit.Mvvm
5. **Dependency Injection** - Full DI support for services and ViewModels
6. **Live Configuration Display** - UI shows current configuration values and their sources

## Configuration Priority (Highest to Lowest)

1. Command Line Arguments
2. Azure App Configuration
3. Azure Key Vault (via App Configuration)
4. Local config.json

## Setup Instructions

### Option 1: Use with Azure App Configuration

1. Create an Azure App Configuration resource in Azure Portal
2. (Optional) Create an Azure Key Vault for secrets
3. Update `config.json` with your connection details:
4. Add configuration keys in Azure App Configuration:
- `ApplicationName`
- `Environment`
- `EnableAdvancedFeatures`
- `ApiEndpoint`
- `DatabaseConnection` (can reference Key Vault)

### Option 2: Use Local Configuration Only

1. Leave `AzureAppConfiguration:ConnectionString` empty in `config.json`
2. The app will automatically use local configuration as fallback
3. All settings in`config.json` will be used

### Option 3: Use Command Line Arguments

Run the application with arguments to override configuration:

## Project Structure

## Key Concepts Demonstrated

### 1. ApplicationHost Setup
The app uses .NET Generic Host (`IHost`) for:
- Dependency injection container
- Configuration management
- Logging infrastructure
- Application lifetime management

### 2. Configuration Cascade
Configuration is loaded in order:
1. Base: `config.json`
2. Azure App Configuration (if available)
3. Key Vault secrets (if configured)
4. Command line overrides

If Azure connection fails, the app gracefully falls back to local configuration.

### 3. Serilog Integration
Structured logging with:
- Console output for debugging
- File output (`logs/app-YYYYMMDD.log`) for production
- Configuration-driven log levels

### 4. MVVM with DI
- ViewModels are registered in DI container
- Views receive ViewModels via constructor injection
- Commands use `CommunityToolkit.Mvvm` source generators

### 5. Configuration Display
The UI dynamically displays:
- Current configuration values
- Source of each configuration (Azure/Local/KeyVault)
- Real-time refresh capability

## Running the Demo

1. Restore NuGet packages
2. Build the solution
3. Run the application
4. Click "Refresh Configuration" to reload values
5. Observe the "Configuration Source" to see if Azure is connected or local fallback is used

## Troubleshooting

- **"Using local config.json"**: Azure connection string not configured or connection failed
- **Authentication errors**: Ensure you're logged into Azure CLI or have proper credentials configured
- **Missing values**: Check that keys exist in both Azure App Configuration and local config.json

## License

This is a demo application for educational purposes.