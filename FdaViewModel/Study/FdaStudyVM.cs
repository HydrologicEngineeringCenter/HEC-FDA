using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities.Transactions;
using System.Collections.ObjectModel;
using FdaViewModel.Conditions;
using FdaViewModel.Utilities;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace FdaViewModel.Study
{
    public class FdaStudyVM : BaseFdaElement, IDisposable, Utilities.Transactions.ITransactionsAndMessages
    {
        #region Notes
        #endregion
        #region Fields
        private List<ParentElement> _MainStudyTree;
        private StudyElement _StudyElement;
        private int _SelectedTab = 0;
        private int _SelectedTabIndex;
        private ObservableCollection<Utilities.DynamicTabVM> _Tabs;

        private List<TransactionRowItem> _TransactionRows;
        private List<MessageRowItem> _MessageRows;
        private bool _TransactionsMessagesVisible;

        private MapWindowMapTreeViewConnector _MWMTVConn;

        #endregion
        #region Properties

        public MapWindowMapTreeViewConnector MWMTVConn
        {
            get { return _MWMTVConn; }
            set { _MWMTVConn = value; }
        }


        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                _SelectedTabIndex = value; NotifyPropertyChanged();
                if (_SelectedTabIndex == 0)
                {
                    _MWMTVConn.UpdateMapWindow();
                    //UpdateMapWindow();
                }
            }
        }
        public ObservableCollection<Utilities.DynamicTabVM> Tabs
        {
            get { return _Tabs; }
            set { _Tabs = value; NotifyPropertyChanged(); }
        }

        public int SelectedTab
        {
            get { return _SelectedTab; }
            set { _SelectedTab = value; TabChangedEvent(value); }
        }
        public override string GetTableConstant()
        {
            return TableName;
        }
        public List<ParentElement> MainStudyTree
        {
            get { return _MainStudyTree; }
            set { _MainStudyTree = value;  NotifyPropertyChanged(nameof(MainStudyTree)); }
        }
       

        public StudyElement StudyElement
        {
            get { return _StudyElement; }
            set { _StudyElement = value; NotifyPropertyChanged(); }
        }

        public override string TableName
        {
            get
            {
                return "";
            }
        }

        public List<TransactionRowItem> TransactionRows
        {
            get { return _TransactionRows; }
            set { _TransactionRows = value; NotifyPropertyChanged(); }
        }

        public List<MessageRowItem> MessageRows
        {
            get { return _MessageRows; }
            set { _MessageRows = value; NotifyPropertyChanged(); }
        }

        public bool TransactionsMessagesVisible
        {
            get { return _TransactionsMessagesVisible; }
            set { _TransactionsMessagesVisible = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public FdaStudyVM(): base()
        {
            
            
            //fill the main study tree
            _MainStudyTree = new List<Utilities.ParentElement>();
            StudyElement = new StudyElement(this);
            _StudyElement.RequestNavigation += Navigate;
            _StudyElement.RequestShapefilePaths += ShapefilePaths;
            _StudyElement.RequestShapefilePathsOfType += ShapefilePathsOfType;
            _StudyElement.RequestAddToMapWindow += AddToMapWindow;
            _StudyElement.RequestRemoveFromMapWindow += RemoveFromMapWindow;
            _StudyElement.TransactionEvent += WriteTransactions;
            _StudyElement.UpdateTransactionsAndMessages += UpdateTransactionsAndMessages;
            _StudyElement.LoadMapLayers += LoadMapLayers;
           //_StudyElement.ClearStudy += ClearCurrentStudy;
            _StudyElement.AddBaseElements();
            _MainStudyTree.Add(_StudyElement);

            FdaModel.Utilities.Messager.Logger.Instance.RequestFlushLogFile += Instance_RequestFlushLogFile;
            FdaModel.Utilities.Initialize.InitializeGDAL();


            //testing the new tabs stuff
            if (Tabs == null)
            {
                Tabs = new ObservableCollection<DynamicTabVM>();
            }
            //add the map window tab
            //MapWindow = MapWindowControlVM.MapWindow 
            


        }

        public FdaStudyVM(string filepath) : base()
        {
            _MainStudyTree = new List<Utilities.ParentElement>();
            StudyElement s = new StudyElement(this);
            s.RequestNavigation += Navigate;
            s.RequestShapefilePaths += ShapefilePaths;
            s.RequestShapefilePathsOfType += ShapefilePathsOfType;
            s.RequestAddToMapWindow += AddToMapWindow;
            s.RequestRemoveFromMapWindow += RemoveFromMapWindow;
            s.AddBaseElements();
            _MainStudyTree.Add(s);

        }
        #endregion
        #region Voids

        

        public void AddMapsTab(OpenGLMapping.MapTreeView mapTreeView)
        {
            _MWMTVConn = MapWindowMapTreeViewConnector.Instance;
            _MWMTVConn.MapTreeView = mapTreeView;

            MapWindowControlVM vm = new MapWindowControlVM(_MWMTVConn);
            //vm.SetMapWindowProperty += SetMapWindowProperty;
            vm.Name = "map window vm";
            Tabs.Add(new DynamicTabVM("Map", vm, false));
        }

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
            if (File.Exists(path))
            {

                XElement mapLayers = XElement.Load(path);
                string messageOut = "";
                _MWMTVConn.MapTreeView.LoadFromXElement(mapLayers,ref messageOut);
                        
            }
        }

        private void UpdateTransactionsAndMessages(object sender, EventArgs e)
        {
            AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(_StudyElement.Name, TransactionEnum.EditExisting,
                "Openning " + _StudyElement.Name + " for editing.", nameof(StudyElement)));

            FdaModel.Utilities.Messager.ErrorMessage err = new FdaModel.Utilities.Messager.ErrorMessage("Test message when opening", FdaModel.Utilities.Messager.ErrorMessageEnum.Report, nameof(StudyElement));

            TransactionHelper.LoadTransactionsAndMessages(this, StudyElement);
        }
        //private void ClearCurrentStudy(object sender, EventArgs e)
        //{
        //    _MainStudyTree[0].Elements.Clear();
        //    _StudyElement = new StudyElement(this);
        //}
        private void Instance_RequestFlushLogFile(object sender, EventArgs e)
        {
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                FdaModel.Utilities.Messager.Logger.Instance.Flush(Storage.Connection.Instance.Reader);
            }
        }

        private void WriteTransactions(object sender, TransactionEventArgs args)
        {
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable("Transactions");
                if (dtv == null)
                {
                    Storage.Connection.Instance.CreateTable("Transactions", new string[] { "Element Name", "Element Type", "Action", "Date", "User", "Notes" }, new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) });
                }
                dtv = Storage.Connection.Instance.GetTable("Transactions");
                dtv.AddRow(new object[] { args.OriginatorName, args.OriginatorType, args.Action.ToString(), args.TransactionDate, args.User, args.Notes });
                dtv.ApplyEdits();
            }
        }
        public override void AddValidationRules()
        {
            AddRule(nameof(StudyElement), () => { return AreConditionsValid(); }, GetConditionErrors());
        }

        private bool AreConditionsValid()
        {
            return false;
        }

        private string GetConditionErrors()
        {
            return "Condition 1 is in error\nCondition 2 is in error.";
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Functions
        public override BaseFdaElement GetElementOfTypeAndName(Type t, string name)
        {
            return _MainStudyTree[0].GetElementOfTypeAndName(t,name);
        }
        public override List<T> GetElementsOfType<T>()
        {
            return _MainStudyTree[0].GetElementsOfType<T>();
        }
        public override void Dispose()
        {
            FdaModel.Utilities.Messager.Logger.Instance.Flush(Storage.Connection.Instance.Reader);
            FdaModel.Utilities.Initialize.DisposeGDAL();
            WriteMapLayersXMLFile();
        }

        /// <summary>
        /// The integer value will be 0 for the conditions tree tab
        /// and 1 for the study tree tab
        /// </summary>
        /// <param name="value"></param>
        public void TabChangedEvent(int value)
        {
            if (value == 0)
            {
                StudyElement.UpdateTheConditionsTree(this, new EventArgs());
            }       
        }

        


        #endregion

    }
}
