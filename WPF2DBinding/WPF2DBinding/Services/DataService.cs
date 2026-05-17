using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using WPF2DBinding.Models;

namespace WPF2DBinding.Services;

public class DataService(ILogger<DataService> logger) : IDataService
{
    private const string DataFolder = "data";

    public async Task SaveZajimavostiAsync(string imageFileName, IEnumerable<Zajimavost> zajimavosti)
    {
        try
        {
            var dataFileName = Path.GetFileNameWithoutExtension(imageFileName) + ".data.json";
            var dataFilePath = Path.Combine(DataFolder, dataFileName);

            var dtos = zajimavosti.Select(ZajimavostDto.FromZajimavost).ToList();
            var json = JsonSerializer.Serialize(dtos, new JsonSerializerOptions { WriteIndented = true });

            await File.WriteAllTextAsync(dataFilePath, json);
            logger.LogInformation("Saved {Count} zajimavosti to {FilePath}", dtos.Count, dataFilePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving zajimavosti for {ImageFileName}", imageFileName);
            throw;
        }
    }

    public async Task<List<Zajimavost>> LoadZajimavostiAsync(string imageFileName)
    {
        try
        {
            var dataFileName = Path.GetFileNameWithoutExtension(imageFileName) + ".data.json";
            var dataFilePath = Path.Combine(DataFolder, dataFileName);

            if (!File.Exists(dataFilePath))
            {
                logger.LogInformation("No data file found for {ImageFileName}", imageFileName);
                return [];
            }

            var json = await File.ReadAllTextAsync(dataFilePath);
            var dtos = JsonSerializer.Deserialize<List<ZajimavostDto>>(json) ?? [];
            var zajimavosti = dtos.Select(dto => dto.ToZajimavost()).ToList();

            logger.LogInformation("Loaded {Count} zajimavosti from {FilePath}", zajimavosti.Count, dataFilePath);
            return zajimavosti;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading zajimavosti for {ImageFileName}", imageFileName);
            return [];
        }
    }
}