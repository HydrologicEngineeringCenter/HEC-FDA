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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if OK was clicked indicating user wants to continue with the close and save even though there are errors.</returns>
        //private bool DisplayErrors(BaseViewModel vm, Window window)
        //{
        //    MessageVM messagevm = new MessageVM(vm.Error);
        //    string header = "Error";
        //    DynamicTabVM tab = new DynamicTabVM(header, messagevm, "ErrorMessage");
        //    WindowVM newvm = new WindowVM(tab);
        //    newvm.Title = "Non-Fatal Errors";

        //    string headerMessage = "The following non-fatal errors were discovered:";
        //    string footerMessage = "Saving data in an error state can cause issues later on. \nDo you want to continue?";
        //    CustomMessageBoxVM customVM = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.Yes_No, headerMessage + Environment.NewLine + Environment.NewLine + messagevm.Message + Environment.NewLine + footerMessage);
        //    header = "Error";
        //    tab = new DynamicTabVM(header, customVM, "ErrorMessage");
        //    ViewWindow newWindow = new ViewWindow(new WindowVM(tab));
        //    newWindow.Owner = window;
        //    newWindow.ShowDialog();
        //    return customVM.ClickedButton == CustomMessageBoxVM.ButtonsEnum.Yes;
        //}
        public void Execute(object parameter)
        {
            //if we bind the "isenabled" of the ok button to the "hasFatalError" property of the vm, then the ok button will be disabled if there is any fatal errors
            // but will be enabled if there are any regular errors. By default any rule will be a fatal error. If we want the user to be able to close the form
            // and save the information in the form even if it has errors then use a "false" when declaring the rule. The ok button will be enabled and this method 
            // will be called.
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
