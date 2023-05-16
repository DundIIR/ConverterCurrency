using System.Globalization;

namespace ConverterCurrency;

public class ValueInputConvertor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && double.TryParse(stringValue, out double doubleValue))
        {
            return doubleValue.ToString("0.######");
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && double.TryParse(stringValue, out double doubleValue))
        {
            return doubleValue.ToString("0.######");
        }
        return null;
    }
}