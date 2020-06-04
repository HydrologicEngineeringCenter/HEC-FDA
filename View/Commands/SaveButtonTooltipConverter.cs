using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace View.Commands
{
    class SaveButtonTooltipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
                bool hasFatalError = (bool)values[0];
            bool HasChanges = (bool)values[1];
            string errorMsg = (string)values[2];

            if(hasFatalError)
            {
                return errorMsg;
            }
            else if(HasChanges == false)
            {
                return "No changes to save";
            }
            else
            {
                return null;
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
