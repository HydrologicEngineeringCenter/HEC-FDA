using System;
using System.Globalization;

namespace HEC.FDA.View.Commands
{
    class OkCommandConverter : System.Windows.Data.IMultiValueConverter
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        #endregion
        #region Voids
        #endregion
        #region Functions
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
                HEC.FDA.ViewModel.BaseViewModel vm = (HEC.FDA.ViewModel.BaseViewModel)values[0];
            if (vm != null)
            {
                //todo: Cody commented out on 1/2/20. I am not sure if it was needed.
               // vm.Validate();
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
