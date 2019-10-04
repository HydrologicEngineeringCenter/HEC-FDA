using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fda.Utilities
{
    //[Author(q0heccdm, 11 / 17 / 2016 4:05:23 PM)]
    public partial class DataGrid:System.Windows.Controls.DataGrid
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 11/17/2016 4:05:23 PM
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        #endregion
        #region Voids
        protected override void OnCanExecuteBeginEdit(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
        #endregion
        #region Functions
        #endregion
    }
}
