using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HEC.FDA.View.Utilities
{
    public class BoolToValidationImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
            {
                return ImageSources.GREEN_CHECKMARK_IMAGE;
            }
            else
            {
                return ImageSources.ERROR_IMAGE;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw null;
        }
    }
}
