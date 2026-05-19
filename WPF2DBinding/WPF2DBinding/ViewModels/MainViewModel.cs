using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WPF2DBinding.Models;
using WPF2DBinding.Services;

namespace WPF2DBinding.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IImageService _imageService;
    private readonly IDataService _dataService;
    private readonly ILogger<MainViewModel> _logger;
    private const string DataFolder = "data";

    [ObservableProperty]
    private ObservableCollection<string> imageFiles = [];

    [ObservableProperty]
    private string? selectedImageFile;

    [ObservableProperty]
    private BitmapImage? currentImage;

    [ObservableProperty]
    private ObservableCollection<Zajimavost> zajimavosti = [];

    [ObservableProperty]
    private Zajimavost? selectedZajimavost;

    [ObservableProperty]
    private double zoomLevel = 1.0;

    public MainViewModel(IImageService imageService, IDataService dataService, ILogger<MainViewModel> logger)
    {
        _imageService = imageService;
        _dataService = dataService;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing MainViewModel");
        await LoadImageListAsync();
    }

    private async Task LoadImageListAsync()
    {
        var files = await _imageService.GetImageFilesAsync(DataFolder);
        ImageFiles = new ObservableCollection<string>(files);
    }

    partial void OnSelectedImageFileChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _ = LoadImageAndDataAsync(value);
        }
    }

    private async Task LoadImageAndDataAsync(string fileName)
    {
        _logger.LogInformation("Loading image and data for {FileName}", fileName);

        var filePath = Path.Combine(DataFolder, fileName);
        CurrentImage = await _imageService.LoadImageAsync(filePath);

        var loadedData = await _dataService.LoadZajimavostiAsync(fileName);
        Zajimavosti = new ObservableCollection<Zajimavost>(loadedData);

        // Pо načtеní dat vyber pоvсhаzí položku, іkоlіv existuje
        SelectedZajimavost = Zajimavosti.FirstOrDefault();

        ZoomLevel = 1.0;
    }

    [RelayCommand]
    private void AddZajimavost()
    {
        // Vypоčítat viditelnou oblast ScrollVieweru pоr umístění obdélníku
        var viewportX = 10.0;
        var viewportY = 10.0;

        var newItem = new Zajimavost
        {
            Nadpis = "Nová zajímavost",
            Popis = "Popis",
            Oblast = new Rect(viewportX, viewportY, 100, 100)
        };

        Zajimavosti.Add(newItem);
        SelectedZajimavost = newItem;
        _logger.LogInformation("Added new zajimavost {Id}", newItem.Id);
    }

    [RelayCommand]
    private void DeleteZajimavost(Zajimavost? item)
    {
        if (item != null && Zajimavosti.Contains(item))
        {
            var index = Zajimavosti.IndexOf(item);
            Zajimavosti.Remove(item);
            
            // Automaticky vybrat předchozí nebo následující položku
            if (Zajimavosti.Count > 0)
            {
                var newIndex = Math.Min(index, Zajimavosti.Count - 1);
                SelectedZajimavost = Zajimavosti[newIndex];
            }
            else
            {
                SelectedZajimavost = null;
            }
            
            _logger.LogInformation("Deleted zajimavost {Id}", item.Id);
        }
    }

    [RelayCommand]
    private async Task SaveDataAsync()
    {
        if (string.IsNullOrEmpty(SelectedImageFile))
        {
            _logger.LogWarning("Cannot save: no image selected");
            return;
        }

        try
        {
            await _dataService.SaveZajimavostiAsync(SelectedImageFile, Zajimavosti);
            MessageBox.Show("Data uložena úspěšně!", "Uloženo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Chyba při ukládání: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    partial void OnSelectedZajimavostChanged(Zajimavost? oldValue, Zajimavost? newValue)
    {
        if (oldValue != null)
            oldValue.IsSelected = false;
        
        if (newValue != null)
            newValue.IsSelected = true;
    }

    public void UpdateZajimavostOblast(Zajimavost zajimavost, Rect newOblast)
    {
        zajimavost.Oblast = newOblast;
        _logger.LogDebug("Updated oblast for {Id}: {Oblast}", zajimavost.Id, newOblast);
    }

    [RelayCommand]
    private void SelectZajimavost(Zajimavost? item)
    {
        if (item != null && Zajimavosti.Contains(item))
        {
            SelectedZajimavost = item;
            _logger.LogInformation("Selected zajimavost {Id} via rectangle click", item.Id);
        }
    }
}