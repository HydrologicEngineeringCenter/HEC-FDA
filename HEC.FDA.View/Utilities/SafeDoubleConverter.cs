using SciChart.Core.Extensions;
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

        var dec = culture.NumberFormat.NumberDecimalSeparator;

        // empty, so don't update binding. value will be whatever the last valid number was
        if (s.IsEmpty())
            return Binding.DoNothing;

        // in progress states
        if (s == "-" || s == dec || s.EndsWith(dec))
            return Binding.DoNothing;

        // a decimal with a 0 in it
        // will not update binding if going from 12 -> 12.0, for example, but don't need it to b/c they are the same
        // the try parse will parse into the proper decimal value
        if (s.EndsWith('0') && s.Contains(dec))
            return Binding.DoNothing;

        if (double.TryParse(s, NumberStyles.Float, culture, out var d))
            return d;

        return Binding.DoNothing; // invalid text: don’t update source yet
    }
}
