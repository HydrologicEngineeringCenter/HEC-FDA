using Geospatial.GDALAssist;
using HEC.FDA.ViewModel.Tabs;
using HEC.FDA.ViewModel.Utilities;
using OxyPlot;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace HEC.FDA.ViewModel.Study
{
    public class FdaStudyVM : BaseViewModel
    {

        #region Fields
        private StudyElement _StudyElement;

        private string _SaveStatus;
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

            InitializeGDAL();
        }

        private void _StudyElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }
        #endregion

        private void InitializeGDAL()
        {
            //string gdalPath = @"GDAL\";
            //if (!Directory.Exists(gdalPath))
            //{
            //    Console.WriteLine("GDAL directory not found: " + gdalPath);
            //    return;
            //}
            GDALSetup.InitializeMultiplatform();
        }

        private void UpdateSaveStatus(object sender, EventArgs e)
        {
            SaveStatus = (string)sender;
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
