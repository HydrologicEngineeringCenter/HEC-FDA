using Geospatial.GDALAssist;
using HEC.FDA.ViewModel.Tabs;
using HEC.FDA.ViewModel.Utilities;
using OxyPlot;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.Reporting;
using HEC.FDA.ViewModel.Storage;
using Visual.Observables;
using Utility;

namespace HEC.FDA.ViewModel.Study
{
    public class FdaStudyVM : BaseViewModel
    {

        #region Fields
        private StudyElement _StudyElement;

        private string _SaveStatus;
        private bool _reportRunning;
        public bool CanGenerateStudyReport => CurrentStudyElement?.StudyLoaded == true && !_reportRunning;
        #endregion
        #region Properties

        public string SaveStatus
        {
            get { return _SaveStatus; }
            set { _SaveStatus = value; NotifyPropertyChanged(); }
        }

        public StudyElement CurrentStudyElement
        {
            get { return _StudyElement; }
            set { _StudyElement = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Currently this is ugly but meets the mail. I will need to refactor this to be prettier in the UI. Microsoft throws a bunch of alphanumeris behind the version I wasn't expecting. 
        /// Need to just show numeric version in title bar. Hide a bigger version elsewhere. 
        /// </summary>
        public string Version
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                return $"HEC-FDA {version.Major}.{version.Minor}.{version.Build}{(version.Revision > 0 ? $".{version.Revision} Beta" : "")}";
            }
        }

        /// <summary>
        /// This needs to be here so that the UI has something to bind to.
        /// </summary>
        public TabController TabFactoryInstance
        {
            get; set;
        }

        #endregion
        #region Constructors
        public FdaStudyVM() : base()
        {
            TabController tabFactory = TabController.Instance;
            TabFactoryInstance = tabFactory;
            tabFactory.RequestNavigation += Navigate;

            CurrentStudyElement = new StudyElement();
            _StudyElement.RequestNavigation += Navigate;
            _StudyElement.PropertyChanged += _StudyElement_PropertyChanged;

            GDALSetup.InitializeMultiplatform();
        }

        private void _StudyElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
            NotifyPropertyChanged(nameof(CanGenerateStudyReport));
        }
        #endregion

        private void UpdateSaveStatus(object sender, EventArgs e)
        {
            SaveStatus = (string)sender;
        }

        public async Task GenerateStudyReportAsync()
        {
            if (!CanGenerateStudyReport) return;
            string studyPath = Connection.Instance.ProjectFile;
            var dialog = new SaveFileDialog
            {
                Title = "Generate Study Report",
                Filter = "CSV report (*.csv)|*.csv",
                DefaultExt = ".csv",
                AddExtension = true,
                FileName = Path.GetFileNameWithoutExtension(studyPath) + "_results_report.csv",
                InitialDirectory = Path.GetDirectoryName(studyPath)
            };
            if (dialog.ShowDialog() != true) return;
            _reportRunning = true;
            NotifyPropertyChanged(nameof(CanGenerateStudyReport));
            try
            {
                ISynchronizationContext context = new Utility.SynchronizationContext(
                    action => Application.Current.Dispatcher.BeginInvoke(action));
                var vm = new StudyReportLogVM(new BatchJob(uiThreadSyncContext: context));
                Task work = vm.RunAsync(new StudyReportService(StudyCache), CurrentStudyElement.Name, dialog.FileName);
                // A modal log keeps the active study and its saved definitions stable while workers compute.
                var tab = new DynamicTabVM("Compute Log: Study Report", vm, Guid.NewGuid().ToString(), false, false);
                Navigate(tab, true, true);
                await work;
            }
            finally
            {
                _reportRunning = false;
                NotifyPropertyChanged(nameof(CanGenerateStudyReport));
            }
        }

        public void LaunchSplashScreen()
        {
            //don't throw up the splash screen if the t&c were already accepted
            if (SplashScreenVM.TermsAndConditionsAccepted)
            {
                return;
            }
            SplashScreenVM vm = new();
            string header = "Terms and Conditions";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "splashscreen", true, false);
            Navigate(tab, true, true);
        }

    }
}
