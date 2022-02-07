
using FdaLogging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using ViewModel.Tabs;
using ViewModel.Utilities;
using ViewModel.Utilities.Transactions;


namespace ViewModel.Study
{
    public class FdaStudyVM : BaseViewModel, IDisposable, IDisplayLogMessages
    {
        #region Notes
        #endregion
        #region Fields
        private List<ParentElement> _MainStudyTree;
        private StudyElement _StudyElement;
        private int _SelectedTab = 0;
        private int _SelectedTabIndex;
        private ObservableCollection<Utilities.IDynamicTab> _Tabs;
        private ObservableCollection<TransactionRowItem> _TransactionRows;
        private ObservableCollection<FdaLogging.LogItem> _MessageRows = new ObservableCollection<LogItem>();
        private bool _TransactionsMessagesVisible;
        private MapWindowMapTreeViewConnector _MWMTVConn;
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
        public MapWindowMapTreeViewConnector MWMTVConn
        {
            get { return _MWMTVConn; }
            set { _MWMTVConn = value; }
        }
        public int SelectedDynamicTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                _SelectedTabIndex = value; NotifyPropertyChanged();
                if (_SelectedTabIndex == 0)
                {
                    _MWMTVConn.UpdateMapWindow();
                }
            }
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
            //todo: i don't remember what this did, but i think it had to do with John's functions which don't exist anymore.
            //Sampler.RegisterSampler(new ConstantSampler());
            //Sampler.RegisterSampler(new DistributionSampler());
            //Sampler.RegisterSampler(new LinkedFunctionsSampler());

            TabController tabFactory = TabController.Instance;
            TabFactoryInstance = tabFactory;
            tabFactory.RequestNavigation += Navigate;
            
            //load elements
            //put elements in cent repo
            //pass repo to studyelement

            //fill the main study tree
            _MainStudyTree = new List<Utilities.ParentElement>();
            CurrentStudyElement = new StudyElement();
            _StudyElement.RenameTreeViewElement += RenameTheMapTreeViewItem;
            _StudyElement.AddBackInTreeViewElement += AddTheMapTreeViewItemBackIn;           
            _StudyElement.RequestNavigation += Navigate;
            _StudyElement.RequestShapefilePaths += ShapefilePaths;
            _StudyElement.RequestShapefilePathsOfType += ShapefilePathsOfType;
            _StudyElement.RequestAddToMapWindow += AddToMapWindow;
            _StudyElement.RequestRemoveFromMapWindow += RemoveFromMapWindow;
            _StudyElement.TransactionEvent += WriteTransactions;
            _StudyElement.UpdateTransactionsAndMessages += UpdateTransactionsAndMessages;
            _StudyElement.LoadMapLayers += LoadMapLayers;
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
        public static void DisposeGDAL()
        {
            GDALAssist.GDALSetup.Dispose();
        }

        #region Voids

        private void OpenADifferentStudy(object sender, EventArgs e)
        {
           // _TabsDictionary.Clear();
            //Tabs.Clear();
            //AddMapsTab(_MWMTVConn.MapTreeView);
        }
        private void AddTheMapTreeViewItemBackIn(object sender, EventArgs e)
        {
            //OpenGLMapping.RasterFeatureNode newNode = (OpenGLMapping.RasterFeatureNode)sender;
            //_MWMTVConn.MapTreeView.AddGisData(newNode);

        }
        private void RenameTheMapTreeViewItem(object sender, EventArgs e)
        {
            ChildElement elem = (ChildElement)sender;

            if(elem.GetType() == typeof(Watershed.TerrainElement))
            {
                List<OpenGLMapping.RasterFeatureNode> rasters = _MWMTVConn.MapTreeView.GetRasterFeatureNodes();
                foreach(OpenGLMapping.RasterFeatureNode raster in rasters)
                {
                    if(raster.DisplayName.Equals(elem.Name))
                    {
                        //then we found it
                        
                        raster.RemoveLayerEventRaiser(true);

                        if (elem.GetType() == typeof(Watershed.TerrainElement))
                        {

                        raster.SetDisplayName("cody");
                        OpenGLMapping.MapRasters mr = (OpenGLMapping.MapRasters)raster.GetBaseFeature;
                        OpenGLMapping.RasterFeatureNode newNode = new OpenGLMapping.RasterFeatureNode(mr, "new Name");
                            ((Watershed.TerrainElement)elem).NodeToAddBackToMapWindow = newNode;
                        }
                    }
                }
            }
            List<OpenGLMapping.FeatureNodeHeader> headers = _MWMTVConn.MapTreeView.GetAllFeatureNodes();//loop through and change the name
            foreach(OpenGLMapping.FeatureNodeHeader header in headers)
            {
                if(header.DisplayName.Equals(elem.Name))
                {
                    header.SetDisplayName("cody");
                }
            }
        }
        private void SetMapWindowInConnector(object sender, EventArgs e)
        {
        }
        private void UpdateSaveStatus(object sender, EventArgs e)
        {
            SaveStatus = (string)sender;

        }

        /// <summary>
        /// This is very hacky and i need to come up with a better way, but the map window is not updating
        /// properly when the tabs switch. If i do the update on the selection changed event of the tab, it 
        /// is too early. I have set a timer for 100 miliseconds and it calls this.
        /// </summary>
       public void UpdateMapTabTest()
        {
            _MWMTVConn.UpdateMapWindow();
        }

        public void AddMapsTab(OpenGLMapping.MapTreeView mapTreeView)
        {
            _MWMTVConn = MapWindowMapTreeViewConnector.Instance;
            _MWMTVConn.MapTreeView = mapTreeView;

            MapWindowControlVM vm = new MapWindowControlVM(ref _MWMTVConn);
            vm.Name = "map window vm";
            DynamicTabVM mapTabVM = new DynamicTabVM("Map", vm, "Map", false, false);
            Navigate(mapTabVM, false, false);
            TabController.Instance.MWMTVConnector = _MWMTVConn;
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
        public void WriteMapLayersXMLFile()
        {
            if (Storage.Connection.Instance.ProjectDirectory != "")
            {
                string path = Storage.Connection.Instance.ProjectDirectory + "\\MapLayers.xml";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (XmlWriter writer = XmlWriter.Create(path))
                {

                    _MWMTVConn.MapTreeView.WriteToXElement().WriteTo(writer);
                }
            }
        }

        public void LoadMapLayers(object sender, EventArgs e)
        {
            string path = Storage.Connection.Instance.ProjectDirectory + "\\MapLayers.xml";
            if (File.Exists(path) && _MWMTVConn != null)
            {
                try
                {
                    XElement mapLayers = XElement.Load(path);
                    string messageOut = "";
                    _MWMTVConn.MapTreeView.LoadFromXElement(mapLayers, ref messageOut);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error reading map layers xml file. " + ex.Message);
                }
                        
            }
        }
        
        private void UpdateTransactionsAndMessages(object sender, EventArgs e)
        {
            AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(_StudyElement.Name, TransactionEnum.EditExisting,
                "Openning " + _StudyElement.Name + " for editing.", nameof(CurrentStudyElement)));

        }

        private void WriteTransactions(object sender, TransactionEventArgs args)
        {
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                DatabaseManager.DataTableView dtv = Storage.Connection.Instance.GetTable("Transactions");
                if (dtv == null)
                {
                    Storage.Connection.Instance.CreateTable("Transactions", new string[] { "Element Name", "Element Type", "Action", "Date", "User", "Notes" }, new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) });
                }
                dtv = Storage.Connection.Instance.GetTable("Transactions");
                dtv.AddRow(new object[] { args.OriginatorName, args.OriginatorType, args.Action.ToString(), args.TransactionDate, args.User, args.Notes });
                dtv.ApplyEdits();
            }
        }

       
        #endregion
        #region Functions

        public override void Dispose()
        {
            Disposer.DeleteOldLogs();
            Disposer.DeleteLogsOverMaxNumber();
            WriteMapLayersXMLFile();
            Disposer.Dispose();
        }

        public void FilterRowsByLevel(FdaLogging.LoggingLevel level)
        {
            ObservableCollection<FdaLogging.LogItem> tempList = new ObservableCollection<FdaLogging.LogItem>();
            foreach (FdaLogging.LogItem mri in MessageRows)
            {
                if (mri.LogLevel.Equals(level.ToString()))
                {
                    tempList.Add(mri);
                }
            }

            MessageRows = tempList;
        }
        public void DisplayAllMessages()
        {
            MessageRows = FdaLogging.RetrieveFromDB.GetMessageRowsForType(GetType());
        }

        public void UpdateMessages(bool saving = false)
        {
            ObservableCollection<FdaLogging.LogItem> tempList = new ObservableCollection<FdaLogging.LogItem>();
            foreach (LogItem li in MessageRows)
            {
                //exclude any temp logs
                if (!li.IsTempLog())
                {
                    tempList.Add(li);
                }
            }

            foreach (LogItem li in TempErrors)
            {
                tempList.Insert(0, li);
            }
            MessageRows = tempList;
        }


        #endregion

    }
}
