using System;
using System.Windows.Data;
using System.Globalization;

namespace HEC.FDA.ViewModel.ImpactArea
{
    //[Author("q0heccdm", "10 / 18 / 2016 11:47:33 AM")]
    public class RadioButtonConverterLeft : IValueConverter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/18/2016 11:47:33 AM
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
            if (value == null) { return false; }
            else if ((int)value ==(int)(ImpactAreaVM.Bank.left)) { return true; }
            else { return false; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }

        #endregion

    }
}
