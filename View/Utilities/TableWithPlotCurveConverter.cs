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
    //[Author(q0heccdm, 8 / 11 / 2017 1:02:18 PM)]
    class TableWithPlotCurveConverter : IValueConverter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 8/11/2017 1:02:18 PM
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
            return value as Statistics.UncertainCurveDataCollection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as Statistics.UncertainCurveIncreasing;
        }
    }
}
