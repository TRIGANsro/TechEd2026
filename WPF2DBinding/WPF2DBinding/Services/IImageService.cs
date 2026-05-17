namespace WPF2DBinding.Services;

public interface IImageService
{
    Task<List<string>> GetImageFilesAsync(string folderPath);
    Task<System.Windows.Media.Imaging.BitmapImage?> LoadImageAsync(string filePath);
}