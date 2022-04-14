using HEC.MVVMFramework.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                ErrorLevel errorLevel = (ErrorLevel)values[0];
                bool HasChanges = (bool)values[1];
                List<string> errorMsg = (List<string>)values[2];
                bool hasErrors = (bool)values[3];

                if(hasErrors)
                {
                    return "cody testing";
                }
                else if (errorLevel >= ErrorLevel.Fatal)
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
            catch(Exception e)
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
