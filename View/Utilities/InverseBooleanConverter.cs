using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace View.Utilities
{
    //[Author(q0heccdm, 6 / 29 / 2017 2:22:42 PM)]
    public class InverseBooleanConverter : System.Windows.Data.IValueConverter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/29/2017 2:22:42 PM
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
        #endregion
        public object Convert(object value, Type targetType, object parameter,
           System.Globalization.CultureInfo culture)
        {
            //if (targetType != typeof(Boolean))
              //  throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            //throw new NotSupportedException();
            return !(bool)value;
        }
    }
}
