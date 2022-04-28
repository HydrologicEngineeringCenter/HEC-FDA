using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Tabs;
using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using ViewModel;

namespace HEC.FDA.ViewModel.Utilities
{
    public delegate void LaunchNewWindowHandler(WindowVM vm, bool asDialogue);
    public class WindowVM : BaseViewModel
    {
        #region Notes
        #endregion
        #region Fields
        private string _Title;
        private BaseViewModel _currentViewModel;
        #endregion
        #region Events
        public event LaunchNewWindowHandler LaunchNewWindow;
        #endregion
        #region Properties

        public FdaStudyVM StudyVM { get; }

        public BaseViewModel CurrentView
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                NotifyPropertyChanged(nameof(CurrentView));
                CurrentView.RequestNavigation += CurrentView_RequestNavigation;
            }
        }
        /// <summary>
        /// This tab object allows me to grab the baseViewModel as well as handle popping into tabs and windows.
        /// </summary>
        public IDynamicTab Tab { get; set; }

        public string Title
        {
            get  {return _Title;}
            set { _Title = value;  NotifyPropertyChanged(nameof(Title)); }
        }

        #endregion
        #region Constructors
        public WindowVM()
        {
            //Anytime a window is opened in all of FDA it falls into here
            //and recreates the studyVM. This static prop in a static class allows me to not enter if the 
            //study is already open
            if(!ExtentionMethods.IsStudyOpen)
            {
                ExtentionMethods.IsStudyOpen = true;
                StudyVM = new FdaStudyVM();
                StudyVM.PropertyChanged += StudyVM_PropertyChanged;
                CurrentView = StudyVM;
                Title = StringConstants.FDA_VERSION;
            }
        }
        public WindowVM(IDynamicTab tab)
        {
            Tab = tab;
            CurrentView = tab.BaseVM;
            Title = tab.Header;
        }

        #endregion
        #region Voids
        private void StudyVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Name"))
            {
                Title = StringConstants.FDA_VERSION + " - " + StudyVM.CurrentStudyElement.Name; ;
            }
        }

        private void CurrentView_RequestNavigation( IDynamicTab tab, bool newWindow, bool asDialog)
        {
            if (LaunchNewWindow != null)
            {
                if (newWindow)
                {
                    WindowVM window = new WindowVM(tab);
                    window.Title = tab.Header;
                    LaunchNewWindow(window, asDialog);
                }
                else
                {
                    TabController tabFactory = TabController.Instance;
                    tabFactory.AddTab(tab);
                }
            }
        }
        #endregion


    }
}
