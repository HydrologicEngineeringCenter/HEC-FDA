using System;
using System.Globalization;
using System.Windows.Data;

namespace HEC.FDA.View.Utilities;
public class SafeDoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() ?? "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var s = (value as string) ?? "";

        // Keep typing for empty or “unfinished” inputs
        var dec = culture.NumberFormat.NumberDecimalSeparator;

        if (s.Length == 0 || s == "-" || s == dec || s.EndsWith(dec)) // ← this is the key: “12.” is in-progress
        {
            return Binding.DoNothing;
        }

        if (double.TryParse(s, NumberStyles.Float, culture, out var d))
            return d;

        return Binding.DoNothing; // invalid text: don’t update source yet
    }
}
