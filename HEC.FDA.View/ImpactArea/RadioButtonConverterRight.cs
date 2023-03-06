using System;
//using FdaModel;
//using FdaModel.Utilities.Attributes;
using System.Windows.Data;
using System.Globalization;

namespace HEC.FDA.View.ImpactArea
{
    //[Author("q0heccdm", "10 / 18 / 2016 11:49:56 AM")]
    public class RadioButtonConverterRight : IValueConverter
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
            else if ((int)value == (int)ViewModel.ImpactArea.ImpactAreaVM.Bank.right) { return true; }
            else { return false; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value == null) { return HEC.FDA.ViewModel.ImpactArea.ImpactAreaVM.Bank.right; }
            //else
            //{
            //    if (bool.Parse(value.ToString()) == true) return HEC.FDA.ViewModel.ImpactArea.ImpactAreaVM.Bank.right; // HEC.FDA.ViewModel.ImpactArea.ImpactAreaVM.Bank.right;
            //    //else return null;// HEC.FDA.ViewModel.ImpactArea.ImpactAreaVM.Bank.right;
            //}
            return parameter;
        }
        #endregion


    }
}
