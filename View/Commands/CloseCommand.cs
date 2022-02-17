using HEC.FDA.ViewModel.Tabs;
using System;

namespace HEC.FDA.View.Commands
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
            //ViewModel.BaseViewModel vm = (HEC.FDA.ViewModel.BaseViewModel)values[0];
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