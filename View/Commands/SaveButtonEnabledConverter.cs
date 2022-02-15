using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            //this is to answer the question Is Enabled. If HasFatalError is true or HasChanges is False,
            //then we want to return false.
            bool hasFatalError = (bool)values[0];
            bool hasChanges = (bool)values[1];

            if(hasFatalError == true || hasChanges == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
