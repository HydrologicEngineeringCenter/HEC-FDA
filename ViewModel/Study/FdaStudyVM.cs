using HEC.FDA.ViewModel.Tabs;
using HEC.FDA.ViewModel.Utilities;
using System;


namespace HEC.FDA.ViewModel.Study
{
    public class FdaStudyVM : BaseViewModel, IDisposable
    {
        #region Notes
        #endregion
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

            _StudyElement.AddBaseElements();

            InitializeGDAL();

            StudyStatusBar.SaveStatusChanged += UpdateSaveStatus;
        }

        private void InitializeGDAL()
        {
            try
            {
                Environment.SetEnvironmentVariable("GDAL_TIFF_OVR_BLOCKSIZE", "256");
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                string ToolDir = dir + @"GDAL\bin";
                string DataDir = dir + @"GDAL\data";
                string PluginDir = dir + @"GDAL\bin\gdalplugins";
                string WMSDir = dir + @"GDAL\Web Map Services";
                GDALAssist.GDALSetup.Initialize(ToolDir, DataDir, PluginDir, WMSDir, true);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Voids
        
        private void UpdateSaveStatus(object sender, EventArgs e)
        {
            SaveStatus = (string)sender;
        }

        /// <summary>
        /// Adds the "Create New Study" tab to the main window. We remove the tab if the user loads 
        /// a previously created study.
        /// </summary>
        public void AddCreateNewStudyTab()
        {
            NewStudyVM vm = new NewStudyVM(CurrentStudyElement);
            DynamicTabVM newStudyTab = new DynamicTabVM("Create New Study", vm, "CreateNewStudy", false, true);
            newStudyTab.Name = "CreateStudyTab";
            TabController.Instance.AddTab(newStudyTab);
        }

        #endregion
        


        #endregion

    }
}
