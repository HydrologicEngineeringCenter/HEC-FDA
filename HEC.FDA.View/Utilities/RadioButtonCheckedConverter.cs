using System;
using System.Windows.Data;
using System.Globalization;
using HEC.FDA.ViewModel.ImpactArea;

namespace HEC.FDA.View.Utilities
{
    //[Author("q0heccdm", "10 / 18 / 2016 9:03:42 AM")]
    public class RadioButtonConverterLeft : IValueConverter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/18/2016 9:03:42 AM
        #endregion
        #region Functions
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) 
            { 
                return false; 
            }
            else if (value.Equals(ImpactAreaVM.Bank.left)) 
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
            if (value == null || parameter == null) 
            { 
                return ImpactAreaVM.Bank.right; 
            }
            else
            {
                if (bool.Parse(value.ToString()) == true)
                {
                    return ImpactAreaVM.Bank.left;
                }
                else
                {
                    return ImpactAreaVM.Bank.right;
                }
            }
        }

        #endregion

    }
}
