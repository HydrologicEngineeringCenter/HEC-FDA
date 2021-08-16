using ViewModel.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.Commands
{
    class CloseCommand : System.Windows.Input.ICommand
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Events
        public event EventHandler CanExecuteChanged;
        #endregion
        #region Properties
        #endregion
        #region Constructors
        #endregion
        #region Voids
        public void Execute(object parameter)
        {
            var values = (object[])parameter;
            //ViewModel.BaseViewModel vm = (ViewModel.BaseViewModel)values[0];
            System.Windows.Window window = (System.Windows.Window)values[1];
            TabController.Instance.CloseTabOrWindow(window);

        }
        #endregion
        #region Functions
        public bool CanExecute(object parameter)
        {
            return true;
        }
        #endregion
    }
}