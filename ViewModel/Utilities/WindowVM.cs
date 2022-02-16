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
        private double _MinimumScaleFactor = .5;
        private double _MaximumScaleFactor = 2.5;
        private double _InitialScaleFactor = 1;
        private bool _Scalable = false;
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
        public double MinimumScaleFactor
        {
            get { return _MinimumScaleFactor; }
            set
            {
                _MinimumScaleFactor = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(CurrentScaleFactor));
            }
        }
        public double MaximumScaleFactor
        {
            get { return _MaximumScaleFactor; }
            set
            {
                _MaximumScaleFactor = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(CurrentScaleFactor));
            }
        }
        public double InitialScaleFactor
        {
            get { return _InitialScaleFactor; }
            set
            {
                _InitialScaleFactor = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(CurrentScaleFactor));
            }
        }
        public string CurrentScaleFactor
        {
            get { return _InitialScaleFactor.ToString("#0%"); }
        }
        public bool Scalable
        {
            get { return _Scalable; }
            set
            {
                _Scalable = value;
                NotifyPropertyChanged();
            }
        }
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

            MinWidth = 800;
            MinHeight = 500;
            Width = 1200;
            Height = 800;
        }
        public WindowVM(IDynamicTab tab)
        {
            Tab = tab;
            CurrentView = tab.BaseVM;
            Title = tab.Header; //vm.GetType().Name;

            //the view windows size is bound to these properties. Set the 
            //dimensions to be what the CurrentView VM wants to be.
            Width = CurrentView.Width;
            Height = CurrentView.Height;
            MinWidth = CurrentView.MinWidth;
            MinHeight = CurrentView.MinHeight;
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
                    tmp.Scalable = true;
                    tmp.Title = tab.Header;
                    LaunchNewWindow(tmp,asDialog);
                }
                else
                {
                    //if(StudyVM.Tabs == null)
                    //{
                    //    StudyVM.Tabs = new System.Collections.ObjectModel.ObservableCollection<IDynamicTab>();
                    //}
                    //vm.CanPopIn = true;
                    //DynamicTabVM tab = new DynamicTabVM(title, vm, true);
                   
                    //old tab method
                    //StudyVM.AddTab(tab);
                    //new tab method
                    TabController tabFactory = TabController.Instance;
                    tabFactory.AddTab(tab);
                   
                    
                    // StudyVM.SelectedTabIndex = StudyVM.Tabs.Count - 1;
                    //CurrentView = vm;
                }
            }else
            {
                //ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("A new window launch was requested from " + this.GetType().Name + " to " + tab.BaseVM.GetType().Name + " and no handler had been assigned.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel & FdaModel.Utilities.Messager.ErrorMessageEnum.Major));
            }

        }

        

        #endregion
        #region Functions
        #endregion

    }
}
