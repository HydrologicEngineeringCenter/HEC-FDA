using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace Fda.Utilities
{
    //[Author(q0heccdm, 8 / 10 / 2017 11:04:53 AM)]
    class DepthDamageEnumToStringConverter : IValueConverter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 8/10/2017 11:04:53 AM
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
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FdaViewModel.Inventory.DepthDamage.DepthDamageCurve.DamageTypeEnum newEnum;
            if (Enum.TryParse(value.ToString(), out newEnum))
            {
                return newEnum;
               
            }
            else
            {
                return null;
            }
        }
    }
}
