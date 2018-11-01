using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fda.Commands
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
            FdaViewModel.BaseViewModel vm = (FdaViewModel.BaseViewModel)values[0];
            System.Windows.Window window = (System.Windows.Window)values[1];

            if (window is ViewWindow)
            {
                FdaViewModel.Utilities.WindowVM winVM = (FdaViewModel.Utilities.WindowVM)window.DataContext;
                if (winVM.StudyVM != null)
                {
                    if (winVM.StudyVM.SelectedDynamicTabIndex != -1)
                    {
                        winVM.StudyVM.RemoveTabAtIndex(winVM.StudyVM.SelectedDynamicTabIndex);
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