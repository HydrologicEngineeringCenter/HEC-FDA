using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Tabs;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace HEC.FDA.View.Commands
{
    class OkCommand : ICommand
    {
        /// <summary>
        /// This is required by ICommand
        /// </summary>
        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var values = (object[])parameter;
            BaseViewModel vm = (BaseViewModel)values[0];
            Window window = (Window)values[1];
            vm.Validate();
            if (vm.HasFatalError)
            {
                MessageBox.Show(vm.Error, "Cannot Save", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public bool CanExecute(object parameter)
        {
            return true;
        }
    }
}
