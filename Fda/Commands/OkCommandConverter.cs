using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fda.Commands
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
            FdaViewModel.BaseViewModel vm = (FdaViewModel.BaseViewModel)values[0];
            vm.Validate();
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
