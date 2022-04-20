using HEC.FDA.ViewModel.Tabs;
using System;


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

            //_StudyElement.AddBaseElements();

            InitializeGDAL();

            StudyStatusBar.SaveStatusChanged += UpdateSaveStatus;
        }
        #endregion

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
        
        private void UpdateSaveStatus(object sender, EventArgs e)
        {
            SaveStatus = (string)sender;
        }        

    }
}
