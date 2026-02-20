using HEC.FDA.Model.LifeLoss.Saving;
using System;
using System.Globalization;
using System.Windows.Data;

namespace HEC.FDA.View.Utilities
{
    public class MilitaryTimeConverter : IValueConverter
    {
        // if it's an int, return it x100 (Military Time). If it's not, but it has the magic string, display as is (combined curve) otherwise.
        // null or empty should return empty. 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (!string.IsNullOrEmpty(s))
            {
                if (int.TryParse(s, out int time))
                    return (time * 100).ToString("D4");
            }
            if (s != null && s.StartsWith(LifeLossStringConstants.COMBINED_MAGIC_STRING)) // combined curves with weights
                return s;
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
