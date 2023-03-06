using System;
using System.Windows.Data;
using System.Globalization;

namespace HEC.FDA.ViewModel.ImpactArea
{
    //[Author("q0heccdm", "10 / 18 / 2016 11:49:56 AM")]
    public class RadioButtonConverterRight : IValueConverter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/18/2016 11:49:56 AM
        #endregion
        #region Functions
         public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) 
            { 
                return false; 
            }
            else if ((int)value == (int)ImpactAreaVM.Bank.right) 
            { 
                return true; 
            }
            else 
            { 
                return false; 
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
        #endregion


    }
}
