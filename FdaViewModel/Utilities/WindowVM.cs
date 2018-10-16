using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
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
            StudyVM = new Study.FdaStudyVM();
            CurrentView = StudyVM;
            Title = "FDA 2.0";
        }
        public WindowVM(BaseViewModel vm)
        {
            CurrentView = vm;
            Title = vm.GetType().Name;
        }
        #endregion
        #region Voids
        private void CurrentView_RequestNavigation(BaseViewModel vm, bool newWindow, bool asDialog, string title = "FDA 2.0")
        {
            if (LaunchNewWindow != null)
            {
                if (newWindow)
                {
                    WindowVM tmp = new WindowVM(vm);
                    tmp.Scalable = true;
                    tmp.Title = title;
                    LaunchNewWindow(tmp,asDialog);
                }
                else
                {
                    if(StudyVM.Tabs == null)
                    {
                        StudyVM.Tabs = new System.Collections.ObjectModel.ObservableCollection<DynamicTabVM>();
                    }
                    StudyVM.Tabs.Add(new DynamicTabVM(title, vm));
                    StudyVM.SelectedTabIndex = StudyVM.Tabs.Count - 1;
                    //CurrentView = vm;
                }
            }else
            {
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("A new window launch was requested from " + this.GetType().Name + " to " + vm.GetType().Name + " and no handler had been assigned.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel & FdaModel.Utilities.Messager.ErrorMessageEnum.Major));
            }

        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Functions
        #endregion

    }
}
