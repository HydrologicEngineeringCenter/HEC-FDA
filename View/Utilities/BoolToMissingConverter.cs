using System;
using System.Globalization;
using System.Windows.Data;

namespace View.Utilities
{
    public class BoolToMissingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (System.Convert.ToBoolean(value) == true)
            {
                return "Missing";
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
