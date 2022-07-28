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
            //try
            //{
            //    string gdalPath = "C:\\Programs\\6.x Development\\GDAL\\";
            //    if (!Directory.Exists(gdalPath))
            //    {
            //        Console.WriteLine("GDAL directory not found: " + gdalPath);
            //        return;
            //    }
            //    GDALSetup.InitializeMultiplatform(gdalPath);
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
        }  
        
        private void UpdateSaveStatus(object sender, EventArgs e)
        {
            SaveStatus = (string)sender;
        }        

    }
}
