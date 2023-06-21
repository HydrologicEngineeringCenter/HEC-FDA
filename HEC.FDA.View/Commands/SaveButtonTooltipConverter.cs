using HEC.FDA.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace HEC.FDA.View.Commands
{
    class SaveButtonTooltipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //this fails sometimes. I am not sure why. I am putting it in a try catch
            //seems to happen when initializing and not everything is set up in time
            try
            {
                bool hasFatalError = (bool)values[0];
                bool HasChanges = (bool)values[1];
                string errorMsg = (string)values[2];

                if (hasFatalError)
                {
                    return errorMsg;
                }
                else if (HasChanges == false)
                {
                    return "No changes to save";
                }
                else
                {
                    return null;
                }
            }
            catch {
                return null;
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
