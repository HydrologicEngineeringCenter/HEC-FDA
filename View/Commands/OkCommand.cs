using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Tabs;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace HEC.FDA.View.Commands
{
    class OkCommand : ICommand
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Events
        /// <summary>
        /// This is required by ICommand
        /// </summary>
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
            BaseViewModel vm = (BaseViewModel)values[0];
            Window window = (Window)values[1];
            vm.Validate();
            if (vm.HasErrors)
            {
                List<string> errors = vm.Errors;
                MessageBox.Show(String.Join(Environment.NewLine, errors), "Cannot Save", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (vm.GetType().IsSubclassOf(typeof(BaseEditorVM)))
            {
                FdaValidationResult vr = ((BaseEditorVM)vm).IsValid();
                if (vr.IsValid)
                {
                    ((BaseEditorVM)vm).Save();
                }
                else
                {
                    MessageBox.Show(vr.ErrorMessage, "Could Not Save", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            vm.WasCanceled = false;
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
