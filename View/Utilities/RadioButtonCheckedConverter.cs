using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace View.Utilities
{
    //[Author("q0heccdm", "10 / 18 / 2016 9:03:42 AM")]
    public class RadioButtonConverterLeft : IValueConverter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/18/2016 9:03:42 AM
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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) { return false; }
            else if (value.Equals(FdaViewModel.ImpactArea.ImpactAreaVM.Bank.left)) { return true; }
            else { return false; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) { return FdaViewModel.ImpactArea.ImpactAreaVM.Bank.right; }
            else
            {
                if (bool.Parse(value.ToString()) == true) return FdaViewModel.ImpactArea.ImpactAreaVM.Bank.left;
                else return FdaViewModel.ImpactArea.ImpactAreaVM.Bank.right;
            }
        }

        #endregion

    }
}
