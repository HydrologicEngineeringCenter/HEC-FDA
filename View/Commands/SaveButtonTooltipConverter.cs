using HEC.FDA.ViewModel;
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
                bool hasErrors = (bool)values[0];
                object dataContext = ((System.Windows.Controls.Button)values[1]).DataContext;

                if (hasErrors)
                {
                    if(dataContext is BaseViewModel baseVM)
                    {
                        //ErrorLevel errorLevel = baseVM.ErrorLevel;
                        List<string> errors = (List<string>)baseVM.GetErrors();
                        string errorMsg = string.Join(Environment.NewLine, errors);
                        return errorMsg;
                    }
                }
                //else if (errorLevel >= ErrorLevel.Fatal)
                //{
                //    return errorMsg;
                //}
                //else if (HasChanges == false)
                //{
                //    return "No changes to save";
                //}
                //else
                //{
                //    return null;
                //}
            }
            catch(Exception e)
            {
                return null;
            }
            return null;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
