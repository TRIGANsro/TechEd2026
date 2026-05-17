using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF2DBinding.Converters;

public class RectToCroppedImageConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || values[0] is not BitmapSource source || values[1] is not Rect rect)
            return null;

        if (rect.Width <= 0 || rect.Height <= 0 || source.PixelWidth == 0 || source.PixelHeight == 0)
            return null;

        try
        {
            var croppedRect = new Int32Rect(
                (int)Math.Max(0, rect.X),
                (int)Math.Max(0, rect.Y),
                (int)Math.Min(rect.Width, source.PixelWidth - rect.X),
                (int)Math.Min(rect.Height, source.PixelHeight - rect.Y)
            );

            if (croppedRect.Width <= 0 || croppedRect.Height <= 0)
                return null;

            var cropped = new CroppedBitmap(source, croppedRect);
            return cropped;
        }
        catch
        {
            return null;
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}