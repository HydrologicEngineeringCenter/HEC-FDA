using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace HEC.FDA.View.Utilities
{
    //[Author(q0heccdm, 6 / 30 / 2017 10:54:24 AM)]
    public class BoolToBoldConverter : IValueConverter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/30/2017 10:54:24 AM
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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
            {
                return System.Windows.FontWeights.Bold;
            }
            else
            {
                return System.Windows.FontWeights.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // throw new NotImplementedException();
            return null;
        }
    }
}
