using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.Editors;
using System;
using System.Windows.Input;

namespace HEC.FDA.View.Commands
{
    class SaveCommand : ICommand
    {
        /// <summary>
        /// This has to be here for "ICommand"
        /// </summary>
        public event EventHandler CanExecuteChanged;
        #region Voids

        public void Execute(object parameter)
        {
            //if we bind the "isenabled" of the ok button to the "hasFatalError" property of the vm, then the ok button will be disabled if there is any fatal errors
            // but will be enabled if there are any regular errors. By default any rule will be a fatal error. If we want the user to be able to close the form
            // and save the information in the form even if it has errors then use a "false" when declaring the rule. The ok button will be enabled and this method 
            // will be called.

            if (parameter is BaseViewModel vm)
            {
                vm.Validate();

                if (vm.GetType().IsSubclassOf(typeof(BaseEditorVM)))
                {
                    ((BaseEditorVM)vm).Save();
                }
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
