using HEC.FDA.ViewModel;
using HEC.MVVMFramework.Base.Enumerations;
using System;
using System.Globalization;
using System.Windows.Data;

namespace HEC.FDA.View.Commands
{
    class SaveButtonEnabledConverter : IMultiValueConverter
    {
        /// <summary>
        /// This is a multi value converter. There are two properties that can disable the Save button:
        /// 1.) If the editor has a Fatal Error.
        /// 2.) If there are no changes to the class after a save.
        /// </summary>
        /// <param name="values">The values are in the order that was set in the xaml. (1 = HasFatalError, 2 = HasChanges)</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool hasErrors = (bool)values[0];
            object dataContext = ((System.Windows.Controls.Button)values[1]).DataContext;

            if (hasErrors)
            {
                if (dataContext is BaseViewModel baseVM)
                {
                    ErrorLevel errorLevel = baseVM.ErrorLevel;
                    if (errorLevel >= ErrorLevel.Fatal)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
