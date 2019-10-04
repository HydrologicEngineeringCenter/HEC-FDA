using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using FdaModel;
//using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace Fda.ImpactArea
{
    //[Author("q0heccdm", "10 / 18 / 2016 11:49:56 AM")]
    public class RadioButtonConverterBoth : IValueConverter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/18/2016 11:49:56 AM
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
            else if ((int)value == (int)(FdaViewModel.ImpactArea.ImpactAreaVM.Bank.both)) { return true; }
            else { return false; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value == null) { return FdaViewModel.ImpactArea.ImpactAreaVM.Bank.right; }
            //else
            //{
            //    if (bool.Parse(value.ToString()) == true) return FdaViewModel.ImpactArea.ImpactAreaVM.Bank.both;
            //    else return null; //FdaViewModel.ImpactArea.ImpactAreaVM.Bank.right;
            //}
            return parameter;
        }
        #endregion


    }
}
