using HEC.FDA.ViewModel.Tabs;
using System;
using System.Windows;
using System.Windows.Input;

namespace HEC.FDA.View.Commands
{
    class CloseCommand : ICommand
    {
        #region Notes
        #endregion

        #region Events
        public event EventHandler CanExecuteChanged { add { } remove { } }
        #endregion

        #region Voids
        public void Execute(object parameter)
        {
            if (parameter is Window window)
            {
                TabController.Instance.CloseTabOrWindow(window);
            }
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