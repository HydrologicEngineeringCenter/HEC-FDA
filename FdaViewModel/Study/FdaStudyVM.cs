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
using System.Collections;
using System.Windows;

namespace FdaViewModel.Study
{
    public class FdaStudyVM : BaseViewModel, IDisposable, Utilities.Transactions.ITransactionsAndMessages
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
        private List<MessageRowItem> _MessageRows;
        private bool _TransactionsMessagesVisible;

        private MapWindowMapTreeViewConnector _MWMTVConn;

        private Dictionary<Guid, IDynamicTab> _TabsDictionary;
        private string _SaveStatus;
        #endregion
        #region Properties
        //public static readonly DependencyProperty FilterStringProperty = DependencyProperty.Register("SaveStatus", typeof(string), typeof(FdaStudyVM), new UIPropertyMetadata("no version!"));
        //public string SaveStatus
        //{
        //    get { return (string)GetValue(FilterStringProperty); }
        //    set { SetValue(FilterStringProperty, value); }
        //}
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

        

        public ObservableCollection<IDynamicTab> Tabs
        {
            get { return _Tabs; }
            set { _Tabs = value; NotifyPropertyChanged(); }
        }

        //public ObservableCollection<Utilities.DynamicTabVM> PoppedOutTabs
        //{
        //    get { return _PoppedOutTabs; }
        //    set { _PoppedOutTabs = value; NotifyPropertyChanged(); }
        //}

        public int SelectedTab
        {
            get { return _SelectedTab; }
            set { _SelectedTab = value; TabChangedEvent(value); }
        }
     
        public List<ParentElement> MainStudyTree
        {
            get { return _MainStudyTree; }
            set { _MainStudyTree = value;  NotifyPropertyChanged(nameof(MainStudyTree)); }
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
            //load elements
            //put elements in cent repo
            //pass repo to studyelement
            
            //fill the main study tree
            _MainStudyTree = new List<Utilities.ParentElement>();
            CurrentStudyElement = new StudyElement();
            _StudyElement.SaveTheOpenTabs += SaveTheTabs;
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
                Tabs = new ObservableCollection<IDynamicTab>();
                _TabsDictionary = new Dictionary<Guid, IDynamicTab>();
            }

            StudyStatusBar.SaveStatusChanged += UpdateSaveStatus;
            //load the cent repo
            //CentralRepository.Instance().RatingCurveElements = 
            // CentralRepository.Create(StudyElement);

        }



        //public FdaStudyVM(string filepath) : base()
        //{
        //    _MainStudyTree = new List<Utilities.ParentElement>();
        //    StudyElement s = new StudyElement(this);
        //    s.RequestNavigation += Navigate;
        //    s.RequestShapefilePaths += ShapefilePaths;
        //    s.RequestShapefilePathsOfType += ShapefilePathsOfType;
        //    s.RequestAddToMapWindow += AddToMapWindow;
        //    s.RequestRemoveFromMapWindow += RemoveFromMapWindow;
        //    s.AddBaseElements();
        //    _MainStudyTree.Add(s);

        //}
        #endregion

        #region Voids

            private void UpdateSaveStatus(object sender, EventArgs e)
        {
            SaveStatus = (string)sender;

        }

        #region Tab Stuff
        private bool DoesUserWantToDeleteTab(IDynamicTab tab)
        {
            if(tab.BaseVM.HasChanges)
            {
                CustomMessageBoxVM vm = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK_Cancel, "Are you sure you want to remove tab '" + tab.Header + "'?");
                Navigate(vm, true, true, "Remove Tab");
                return (vm.ClickedButton == CustomMessageBoxVM.ButtonsEnum.Cancel)?  false : true;
            }
            return true;
        }
        public void RemoveTab(object sender, EventArgs e)
        {
            IDynamicTab tab = (IDynamicTab)sender;
            if (DoesUserWantToDeleteTab(tab))
            {
                Tabs.Remove(tab);
                RemoveTabFromDictionary(tab.BaseVM.TabUniqueID);
            }
           

        }
        public void RemoveTabAtIndex( int index)
        {
            IDynamicTab tab = Tabs[index];
            if (DoesUserWantToDeleteTab(tab))
            {
                Tabs.RemoveAt(index);
                RemoveTabFromDictionary(tab.BaseVM.TabUniqueID);
            }

        }

        public void AddTabToDictionary(Guid id,IDynamicTab tab)
        {
            _TabsDictionary.Add(id, tab);
        }
        public void RemoveTabFromDictionary(Guid id)
        {
            _TabsDictionary.Remove(id);
           
        }
        public void PopTabOut(object sender, EventArgs e)
        {
            DynamicTabVM tabToPopOut = (DynamicTabVM)sender;
            //remove the tab from the tabs list
            RemoveTab(sender, e);            
            //hook up the event for how to remove from the dictionary if the window is closed
            tabToPopOut.BaseVM.RemoveFromTabsDictionary = RemoveTabFromDictionary;
            Navigate(tabToPopOut.BaseVM, true, false,tabToPopOut.Header);
        }
        
        public void AddMapsTab(OpenGLMapping.MapTreeView mapTreeView)
        {
            _MWMTVConn = MapWindowMapTreeViewConnector.Instance;
            _MWMTVConn.MapTreeView = mapTreeView;

            MapWindowControlVM vm = new MapWindowControlVM(_MWMTVConn);
            //vm.SetMapWindowProperty += SetMapWindowProperty;
            vm.Name = "map window vm";
            DynamicTabVM mapTabVM = new DynamicTabVM("Map", vm, false);
            Tabs.Add(mapTabVM);
        }
        public void AddCreateNewStudyTab()
        {
            
            NewStudyVM vm = new NewStudyVM(CurrentStudyElement);
            DynamicTabVM newStudyTab = new DynamicTabVM("Create New Study", vm, true);
            AddTab(newStudyTab);
            SelectedTabIndex = Tabs.Count - 1;
        }

        public DynamicTabVM SelectedDynamicTab { get; set; }
        public void AddTab(DynamicTabVM dynamicTabVM, bool poppingIn = false)
        {

            //some tabs we do not want to allow to be openned multiple times and some we do
            if (dynamicTabVM.BaseVM.CanOpenMultipleTimes == false)
            {
                foreach (IDynamicTab tab in Tabs)
                {
                    if (tab.Header.Equals(dynamicTabVM.Header))
                    {
                        SelectedTabIndex = Tabs.IndexOf(tab);
                        return;
                    }
                }
                //if (poppingIn == false)
                //{
                //    foreach (IDynamicTab tab in PoppedOutTabs)
                //    {
                //        if (tab.Header.Equals(dynamicTabVM.Header))
                //        {
                //            //I wanted to bring the focus to the currect window. This is possible by looping through the open windows and then 
                //            //checking the datacontext to get the vm and maybe comparing. Then set the focus on it. I don't have access to the view from
                //            //here and it seems like more work than its worth.
                //            //foreach (var Window in Application.Current.Windows)
                //            //{
                //            //    int i = 0;
                //            //   //if(Window.)
                //            //    // TODO: write what you want here
                //            //}
                //            return;
                //        }
                //    }
                //}
            }
            StudyStatusBar.SaveStatus = StudyStatusBar.UnsavedChangesMessage;

            Guid uniqueID = Guid.NewGuid();
            dynamicTabVM.BaseVM.TabUniqueID = uniqueID;
            dynamicTabVM.RemoveEvent += RemoveTab;
            dynamicTabVM.PopTabOutEvent += PopTabOut;
            dynamicTabVM.BaseVM.AddPopThisIntoATabAction((dynamicTab, isPoppingIn) => AddTab(dynamicTab, isPoppingIn));
            Tabs.Add(dynamicTabVM);
            _TabsDictionary.Add(uniqueID, dynamicTabVM);
            SelectedTabIndex = Tabs.Count-1;//I want to make the tab we just added be selected. But some crazy stuff
            //happens and it magically sets the selected tab to be count -2. Maybe its an order thing and the tab isn't actually 
            //in yet or something.
        }

        private void SaveTheTabs(object sender, EventArgs e)
        {
            bool allTabsSaved = true;
            foreach (KeyValuePair<Guid, IDynamicTab> entry in _TabsDictionary)
            {
                IDynamicTab tab = entry.Value;
                if (tab.BaseVM.GetType().BaseType == typeof(Editors.BaseEditorVM))
                {
                    tab.BaseVM.Validate();
                    if (tab.BaseVM.HasError)
                    {
                        //do something?
                    }
                    if (tab.BaseVM.HasFatalError)
                    {
                        allTabsSaved = false;
                        TransactionRows.Add(new TransactionRowItem(DateTime.Now.ToString("G"), "Unable to save tab: '" + tab.Header + "' because it is in an error state.", "i forget how to get the user"));
                    }
                    else
                    {
                        ((Editors.BaseEditorVM)tab.BaseVM).Save();
                    }
                }
            }
            if (allTabsSaved)
            {
                SaveStatus = "Saved " + DateTime.Now.ToString("G");
            }
            else
            {
                SaveStatus = "Unsaved Changes";

            }
            //foreach(IDynamicTab tab in Tabs)
            //{
            //    if(tab.BaseVM.GetType().BaseType == typeof(Editors.BaseEditorVM))
            //    {
            //        tab.BaseVM.Validate();
            //        if(tab.BaseVM.HasError)
            //        {
            //            //do something?
            //        }
            //        if(tab.BaseVM.HasFatalError)
            //        {
            //            TransactionRows.Add(new TransactionRowItem(DateTime.Now.ToString("G"), "Unable to save tab: '" + tab.Header + "' because it is in an error state.","i forget how to get the user"));
            //        }
            //        else
            //        {
            //            ((Editors.BaseEditorVM)tab.BaseVM).Save();
            //        }
            //    }
            //}

            ////now save the open windows
            //foreach (IDynamicTab tab in PoppedOutTabs)
            //{
            //    if (tab.BaseVM.GetType().BaseType == typeof(Editors.BaseEditorVM))
            //    {
            //        tab.BaseVM.Validate();
            //        if (tab.BaseVM.HasError)
            //        {
            //            //do something?
            //        }
            //        if (tab.BaseVM.HasFatalError)
            //        {
            //            TransactionRows.Add(new TransactionRowItem(DateTime.Now.ToString("G"), "Unable to save window: '" + tab.Header + "' because it is in an error state.", "i forget how to get the user"));
            //        }
            //        else
            //        {
            //            ((Editors.BaseEditorVM)tab.BaseVM).Save();
            //        }
            //    }
            //}

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

                XElement mapLayers = XElement.Load(path);
                string messageOut = "";
                _MWMTVConn.MapTreeView.LoadFromXElement(mapLayers,ref messageOut);
                        
            }
        }
        
        private void UpdateTransactionsAndMessages(object sender, EventArgs e)
        {
            AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(_StudyElement.Name, TransactionEnum.EditExisting,
                "Openning " + _StudyElement.Name + " for editing.", nameof(CurrentStudyElement)));

            FdaModel.Utilities.Messager.ErrorMessage err = new FdaModel.Utilities.Messager.ErrorMessage("Test message when opening", FdaModel.Utilities.Messager.ErrorMessageEnum.Report, nameof(CurrentStudyElement));

            TransactionHelper.LoadTransactionsAndMessages(this, CurrentStudyElement);
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
            //AddRule(nameof(StudyElement), () => { return AreConditionsValid(); }, GetConditionErrors());
        }

        //private bool AreConditionsValid()
        //{
        //    return false;
        //}

        //private string GetConditionErrors()
        //{
        //    return "Condition 1 is in error\nCondition 2 is in error.";
        //}

       
        #endregion
        #region Functions
        //public override BaseFdaElement GetElementOfTypeAndName(Type t, string name)
        //{
        //    return _MainStudyTree[0].GetElementOfTypeAndName(t,name);
        //}
        //public override List<T> GetElementsOfType<T>()
        //{
        //    return _MainStudyTree[0].GetElementsOfType<T>();
        //}
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
                CurrentStudyElement.UpdateTheConditionsTree(this, new EventArgs());
            }       
        }

      




        #endregion

    }
}
