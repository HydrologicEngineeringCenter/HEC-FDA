using System;

namespace HEC.FDA.View.Utilities
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
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}
