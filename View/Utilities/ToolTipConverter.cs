using System;
using System.Globalization;
using System.Windows.Data;

namespace HEC.FDA.View.Utilities
{
    public class ToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return System.Windows.FontWeights.Bold;
            }
            else
            {
                return System.Windows.FontWeights.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // throw new NotImplementedException();
            return null;
        }
    }
}
