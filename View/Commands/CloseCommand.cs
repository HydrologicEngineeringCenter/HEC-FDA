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
        public event EventHandler CanExecuteChanged;
        #endregion

        #region Voids
        public void Execute(object parameter)
        {
            Window window = parameter as Window;
            if (window != null)
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