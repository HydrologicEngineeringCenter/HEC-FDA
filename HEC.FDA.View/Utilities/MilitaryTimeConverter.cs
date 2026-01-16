using HEC.FDA.Model.LifeLoss.Saving;
using System;
using System.Globalization;
using System.Windows.Data;

namespace HEC.FDA.View.Utilities
{
    public class MilitaryTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (!string.IsNullOrEmpty(s))
            {
                if (int.TryParse(s, out int time))
                    return (time * 100).ToString("D4");
            }
            if (s == LifeLossStringConstants.COMBINED_MAGIC_STRING) // only other allowed string
                return s;
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
