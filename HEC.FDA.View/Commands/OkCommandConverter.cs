using System;
using System.Globalization;

namespace HEC.FDA.View.Commands
{
    class OkCommandConverter : System.Windows.Data.IMultiValueConverter
    {
        #region Functions
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ViewModel.BaseViewModel vm = (ViewModel.BaseViewModel)values[0];
            if (vm != null)
            {
                return values.Clone();
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
        #endregion

    }
}
