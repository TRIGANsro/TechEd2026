using System.Globalization;
using System.Windows.Data;

namespace WPF2DBinding.Converters;

public class ZoomToStrokeThicknessConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double zoom && zoom > 0)
        {
            return Math.Clamp(2.0 / zoom, 0.5, 5.0);
        }
        return 2.0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}