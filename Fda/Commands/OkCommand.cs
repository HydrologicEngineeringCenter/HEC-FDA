using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fda.Commands
{
    class OkCommand : System.Windows.Input.ICommand
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
            //if we bind the "isenabled" of the ok button to the "hasFatalError" property of the vm, then the ok button will be disabled if there is any fatal errors
            // but will be enabled if there are any regular errors. By default any rule will be a fatal error. If we want the user to be able to close the form
            // and save the information in the form even if it has errors then use a "false" when declaring the rule. The ok button will be enabled and this method 
            // will be called.
            var values = (object[])parameter;
            FdaViewModel.BaseViewModel vm = (FdaViewModel.BaseViewModel)values[0];
            System.Windows.Window window = (System.Windows.Window)values[1];
            //if (vm.HasChanges)//if the vm is loaded in an error state, the user will not be identified, we should consider not checking for changes.
            {
                vm.Validate();
               
                if (vm.HasError)
                {
                    FdaViewModel.Utilities.MessageVM messagevm = new FdaViewModel.Utilities.MessageVM(vm.Error);
                    FdaViewModel.Utilities.WindowVM newvm = new FdaViewModel.Utilities.WindowVM(messagevm);
                    newvm.Title = "Non-Fatal Errors";
                    //ViewWindow newwindow = new ViewWindow(newvm);
                    //newwindow.Owner = window;
                    //newwindow.ShowDialog();
                    string headerMessage = "The following non-fatal errors were discovered:";
                    string footerMessage = "Saving data in an error state can cause issues later on. \nDo you want to continue?";
                    FdaViewModel.Utilities.CustomMessageBoxVM customVM = new FdaViewModel.Utilities.CustomMessageBoxVM(FdaViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.Yes_No, headerMessage + System.Environment.NewLine + Environment.NewLine + messagevm.Message + Environment.NewLine  + footerMessage);
                    ViewWindow newWindow = new ViewWindow(new FdaViewModel.Utilities.WindowVM(customVM));
                    newWindow.Owner = window;
                    newWindow.ShowDialog();
                    if(customVM.ClickedButton == FdaViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.Yes)
                    {
                        //save. which is done below.
                    }
                    else
                    {
                        return;
                    }
                }
                //call save if its an editor?
                if (vm.GetType().IsSubclassOf(typeof(FdaViewModel.Editors.BaseEditorVM)))
                {
                    if (((FdaViewModel.Editors.BaseEditorVM)vm).RunSpecialValidation() == true)
                    {
                        ((FdaViewModel.Editors.BaseEditorVM)vm).Save();
                    }
                }
            }

            vm.WasCanceled = false;

            if (window is ViewWindow)
            {
                
                FdaViewModel.Utilities.WindowVM winVM = (FdaViewModel.Utilities.WindowVM)window.DataContext;
                if(winVM.StudyVM != null)
                {
                    if(winVM.StudyVM.SelectedTabIndex != -1)
                    {
                        winVM.StudyVM.Tabs.RemoveAt(winVM.StudyVM.SelectedTabIndex);
                    }
                    else
                    {
                        window.Close();
                    }
                }
                else
                {
                    window.Close();
                }

            }
            else
            {
                window.Close();
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
