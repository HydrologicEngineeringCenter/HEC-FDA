
using FdaLogging;
using HEC.FDA.ViewModel.Tabs;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Utilities.Transactions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace HEC.FDA.ViewModel.Study
{
    public class FdaStudyVM : BaseViewModel, IDisposable
    {
        #region Notes
        #endregion
        #region Fields
        private List<ParentElement> _MainStudyTree;
        private StudyElement _StudyElement;
        private int _SelectedTab = 0;
        private int _SelectedTabIndex;
        private ObservableCollection<TransactionRowItem> _TransactionRows;
        private ObservableCollection<LogItem> _MessageRows = new ObservableCollection<LogItem>();
        private bool _TransactionsMessagesVisible;
        private string _SaveStatus;
        private bool _MapViewVisible;
        private bool _TabsViewVisible;
        #endregion
        #region Properties


        public bool MapViewVisible
        {
            get { return _MapViewVisible; }
            set { _MapViewVisible = value;NotifyPropertyChanged(); }
        }
        public bool TabsViewVisible
        {
            get { return _TabsViewVisible; }
            set { _TabsViewVisible = value; NotifyPropertyChanged(); }
        }
        public string SaveStatus
        {
            get { return _SaveStatus; }
            set { _SaveStatus = value; NotifyPropertyChanged(); }
        }

        public int SelectedDynamicTabIndex
        {
            get { return _SelectedTabIndex; }
            set { _SelectedTabIndex = value; NotifyPropertyChanged(); }
        }

        public int SelectedTab
        {
            get { return _SelectedTab; }
            set { _SelectedTab = value; }
        }

        public List<ParentElement> MainStudyTree
        {
            get { return _MainStudyTree; }
            set { _MainStudyTree = value; NotifyPropertyChanged(nameof(MainStudyTree)); }
        }


        public StudyElement CurrentStudyElement
        {
            get { return _StudyElement; }
            set { _StudyElement = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<TransactionRowItem> TransactionRows
        {
            get { return _TransactionRows; }
            set { _TransactionRows = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<FdaLogging.LogItem> MessageRows
        {
            get { return _MessageRows; }
            set { _MessageRows = value; NotifyPropertyChanged(); }
        }

        public int MessageCount
        {
            get 
            {
                if (_MessageRows != null)
                {
                    return _MessageRows.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool TransactionsMessagesVisible
        {
            get { return _TransactionsMessagesVisible; }
            set { _TransactionsMessagesVisible = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// This needs to be here so that the UI has something to bind to.
        /// </summary>
        public TabController TabFactoryInstance
        {
            get; set;
        }
        //todo: implement error level?
        public LoggingLevel SaveStatusLevel
        {
            get { return LoggingLevel.Debug; }
        }

        public bool IsExpanded
        {
            get;
            set;
        }
        public List<LogItem> TempErrors
        {
            get;
            set;
        }

        #endregion
        #region Constructors
        public FdaStudyVM() : base()
        {
            TabController tabFactory = TabController.Instance;
            TabFactoryInstance = tabFactory;
            tabFactory.RequestNavigation += Navigate;
            
            //load elements
            //put elements in cent repo
            //pass repo to studyelement

            //fill the main study tree
            _MainStudyTree = new List<ParentElement>();
            CurrentStudyElement = new StudyElement();
            //_StudyElement.RenameTreeViewElement += RenameTheMapTreeViewItem;
            //_StudyElement.AddBackInTreeViewElement += AddTheMapTreeViewItemBackIn;           
            _StudyElement.RequestNavigation += Navigate;
            //todo: do we need these shapefile paths without a map window?
            _StudyElement.RequestShapefilePaths += ShapefilePaths;
            _StudyElement.RequestShapefilePathsOfType += ShapefilePathsOfType;
            //_StudyElement.RequestAddToMapWindow += AddToMapWindow;
            //_StudyElement.RequestRemoveFromMapWindow += RemoveFromMapWindow;
            //_StudyElement.TransactionEvent += WriteTransactions;
            //_StudyElement.UpdateTransactionsAndMessages += UpdateTransactionsAndMessages;
            //_StudyElement.LoadMapLayers += LoadMapLayers;
            _StudyElement.OpeningADifferentStudy += OpenADifferentStudy;
            _StudyElement.AddBaseElements();
            _MainStudyTree.Add(_StudyElement);

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
                //Messager.Logger.Instance.ReportMessage(new Messager.ErrorMessage(ex.InnerException.ToString() + "\n Failed to initialize GDAL, check if the GDAL directory is next to the FdaModel.dll", 
                //    Messager.ErrorMessageEnum.Fatal | Messager.ErrorMessageEnum.Model));
                throw;
            }
        }

        #region Voids

        private void OpenADifferentStudy(object sender, EventArgs e)
        {
           // _TabsDictionary.Clear();
            //Tabs.Clear();
            //AddMapsTab(_MWMTVConn.MapTreeView);
        }
        
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
