using HEC.FDA.ViewModel.Tabs;
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

        public Study.FdaStudyVM StudyVM { get; set; }

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
        public IDynamicTab Tab { get; set; }

        public string Title
        {
            get { return _Title; }
            set { _Title = value;  NotifyPropertyChanged(nameof(Title)); }
        }

        #endregion
        #region Constructors
        public WindowVM()
        {
            //this is dumb, but for some reason anytime a window is opened in all of FDA it falls into here
            //and recreates the studyVM. This static prop in a static class allows me to not enter if the 
            //study is already open
            if (ExtentionMethods.IsStudyOpen) 
            { 
                return; 
            }
            else { ExtentionMethods.IsStudyOpen = true; }

            StudyVM = new Study.FdaStudyVM();
            CurrentView = StudyVM;
            Title = "FDA 2.0";

            //MinWidth = 800;
            //MinHeight = 500;
            //Width = 1200;
            //Height = 800;
        }
        public WindowVM(IDynamicTab tab)
        {
            Tab = tab;
            CurrentView = tab.BaseVM;
            Title = tab.Header;

            //the view windows size is bound to these properties. Set the 
            //dimensions to be what the CurrentView VM wants to be.
            //Width = CurrentView.Width;
            //Height = CurrentView.Height;
            //MinWidth = CurrentView.MinWidth;
            //MinHeight = CurrentView.MinHeight;
        }
        #endregion
        #region Voids
        private void CurrentView_RequestNavigation( IDynamicTab tab, bool newWindow, bool asDialog)
        {
            if (LaunchNewWindow != null)
            {
                if (newWindow)
                {
                    WindowVM tmp = new WindowVM(tab);
                    tmp.Title = tab.Header;
                    LaunchNewWindow(tmp,asDialog);
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
