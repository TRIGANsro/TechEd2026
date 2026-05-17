using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows.Media.Imaging;

namespace WPF2DBinding.Services;

public class ImageService(ILogger<ImageService> logger) : IImageService
{
    private readonly string[] _supportedExtensions = [".jpg", ".jpeg", ".png", ".bmp", ".gif"];

    public Task<List<string>> GetImageFilesAsync(string folderPath)
    {
        try
        {
            if (!Directory.Exists(folderPath))
            {
                logger.LogWarning("Folder {FolderPath} does not exist", folderPath);
                return Task.FromResult(new List<string>());
            }

            var files = Directory.GetFiles(folderPath)
                .Where(f => _supportedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .Select(Path.GetFileName)
                .Where(f => f != null)
                .Cast<string>()
                .ToList();

            logger.LogInformation("Found {Count} image files in {FolderPath}", files.Count, folderPath);
            return Task.FromResult(files);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading images from {FolderPath}", folderPath);
            return Task.FromResult(new List<string>());
        }
    }

    public Task<BitmapImage?> LoadImageAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                logger.LogWarning("File {FilePath} does not exist", filePath);
                return Task.FromResult<BitmapImage?>(null);
            }

            var fullPath = Path.GetFullPath(filePath);
            var uri = new Uri(fullPath, UriKind.Absolute);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = uri;
            bitmap.EndInit();
            bitmap.Freeze();

            logger.LogInformation("Loaded image {FilePath} ({Width}x{Height})", filePath, bitmap.PixelWidth, bitmap.PixelHeight);
            return Task.FromResult<BitmapImage?>(bitmap);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading image {FilePath}", filePath);
            return Task.FromResult<BitmapImage?>(null);
        }
    }
}