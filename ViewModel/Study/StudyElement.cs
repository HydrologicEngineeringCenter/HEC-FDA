using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HEC.FDA.ViewModel.AlternativeComparisonReport;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Tabs;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using HEC.FDA.ViewModel.WaterSurfaceElevation;

namespace HEC.FDA.ViewModel.Study
{
    public class StudyElement : ParentElement
    {

        public event EventHandler RenameTreeViewElement;
        public event EventHandler AddBackInTreeViewElement;
        public event EventHandler OpeningADifferentStudy;
        public event EventHandler SaveTheOpenTabs;
        public event EventHandler UpdateTransactionsAndMessages;
        public event EventHandler LoadMapLayers;
        private List<string> _RegistryStudies = new List<string>();
        private ObservableCollection<ParentElement> _ConditionsTree;

        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        public ObservableCollection<ParentElement> ConditionsTree
        {
            get { return _ConditionsTree; }
            set { _ConditionsTree = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public StudyElement() : base()
        {
            PopulateRecentStudies();

            FontSize = 18;
            Name = "Study";
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/Terrain.png");
            _Elements = new ObservableCollection<BaseFdaElement>();

            NamedAction open = new NamedAction();
            open.Header = "Open Study";
            open.Action = OpenStudy;

            NamedAction importStudyFromOldFda = new NamedAction();
            importStudyFromOldFda.Header = StringConstants.IMPORT_FROM_OLD_FDA;
            importStudyFromOldFda.Action = ImportStudyFromOldFda;

            NamedAction create = new NamedAction();
            create.Header = "Create Study";
            create.Action = CreateStudyFromWindow;

            NamedAction properties = new NamedAction();
            properties.Header = "Study Properties";
            properties.Action = StudyProperties;
            properties.IsEnabled = false;

            NamedAction save = new NamedAction();
            save.Header = "Save Study";
            save.Action = SaveStudy;
            save.IsEnabled = false;

            NamedAction transactions = new NamedAction();
            transactions.Header = "View Transactions";
            transactions.Action = ViewTransactions;
            transactions.IsEnabled = false;

            List<NamedAction> localactions = new List<NamedAction>();
            localactions.Add(create);
            localactions.Add(open);
            localactions.Add(importStudyFromOldFda);
            localactions.Add(properties);
            localactions.Add(save);
            localactions.Add(transactions);

            NamedAction seperator = new NamedAction();
            seperator.Header = "seperator";
            localactions.Add(seperator);

            int i = 1;
            foreach(string path in _RegistryStudies)
            {
                RecentFileNamedAction recentPath = new RecentFileNamedAction();
                recentPath.Header =i+": "+ System.IO.Path.GetFileNameWithoutExtension(path);
                recentPath.FilePath = path;
                recentPath.Action = OpenStudyFromRecent;
                localactions.Add(recentPath);
                i++;
            }

            localactions.Add(seperator);

            Actions = localactions;
        }

        private void ViewTransactions(object arg1, EventArgs arg2)
        {
            string header = "Transactions";
            DynamicTabVM tab = new DynamicTabVM(header, new Utilities.Transactions.TransactionVM(), "Transactions");
            Navigate(tab );
        }

        #endregion
        #region Voids
        private void RenameStudy(object sender, EventArgs e)
        {
            //todo: how to rename a study
            //RenameVM renameViewModel = new RenameVM(this, CloneElement);
            //renameViewModel.ParentGUID = this.GUID;
            //Navigate(renameViewModel, false, true, "Rename");
        }
        private void OpenStudyFromRecent(object sender, EventArgs e)
        {
            string filePath = ((RecentFileNamedAction)sender).FilePath;
            OpenStudyFromFilePath(System.IO.Path.GetFileNameWithoutExtension(filePath), filePath);
        }

        private void PopulateRecentStudies()
        {
            string appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().ToString();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            var companyName = fvi.CompanyName;
            var productNAme = fvi.ProductName;
            var productVersion = fvi.ProductVersion;

            string subKey = companyName + "\\" + appName;
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(subKey);
            if (registryKey == null)
            {
                registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(subKey);
            }
            string registryNextLine = "";
            int idx = 0;
            while (idx < registryKey.ValueCount && _RegistryStudies.Count < 5)
            {
                registryNextLine = registryKey.GetValue(idx.ToString()).ToString();
                if(System.IO.File.Exists(registryNextLine))
                {
                    _RegistryStudies.Add(registryNextLine);
                }
                idx++;
            }
        }

        private void UpdateRecentStudiesFile(string filepath)
        {
            string appname = System.Reflection.Assembly.GetExecutingAssembly().GetName().ToString();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            var companyname = fvi.CompanyName;
            var productNAme = fvi.ProductName;
            var productVersion = fvi.ProductVersion;


            string subkey = companyname + "\\" + appname;
            Microsoft.Win32.RegistryKey registrykey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(subkey, true);
            if ((registrykey == null))
            {
                registrykey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(subkey, false);
            }
            List<string> registrystudies = new List<string>();
            string registrynextline = "";
            int idx = 0;
            while (idx < registrykey.ValueCount && registrystudies.Count < 5)
            {
                object regKey = registrykey.GetValue(idx.ToString());
                if(regKey != null) { registrynextline = regKey.ToString(); }
                 
                if (System.IO.File.Exists(registrynextline))
                {
                    registrystudies.Add(registrynextline);
                }
                idx += 1;
            }
            if (registrystudies.Count == 0)
            {
                registrystudies.Add(filepath);
            }
            else
            {
                if (registrystudies.Contains(filepath))
                {
                    registrystudies.RemoveAt(registrystudies.IndexOf(filepath));
                    registrystudies.Insert(0, filepath);
                }
                else
                {
                    registrystudies.Insert(0, filepath);
                }
            }
            for (int i = 0; i <= registrystudies.Count - 1; i++)
            {
                if (registrystudies[i] != null)
                {
                    registrykey.SetValue(i.ToString(), registrystudies[i]);
                }
            }
        }

        private void SaveStudy(object arg1, EventArgs arg2)
        {
            SaveTheOpenTabs?.Invoke(arg1, arg2);
        }
        private void CreateStudyFromWindow(object arg1, EventArgs arg2)
        {
            NewStudyVM vm = new NewStudyVM(this);
            string header = "Create New Study";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "StudyElement");
            Navigate( tab, false,false);
        }
      
        public void CreateStudyFromViewModel(string studyName, string folderPathForNewStudy, string description)
        {
            Name = studyName;
            UpdateTreeViewHeader(Name);
            //check if file exists.
            string newStudyPath = folderPathForNewStudy + "\\" + studyName + "\\" + studyName + ".sqlite";
            if (!System.IO.File.Exists(newStudyPath))
            {
                Connection.Instance.ProjectFile = newStudyPath;
                UpdateRecentStudiesFile(newStudyPath);

            }
            else
            {
                Connection.Instance.ProjectFile = folderPathForNewStudy + "\\" + studyName + "\\" + studyName + ".sqlite";
            }
            AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(studyName, Utilities.Transactions.TransactionEnum.CreateNew, "Initialize study"));
            foreach (NamedAction action in Actions)
            {
                if (action.Header == "Save Study")
                {
                    action.IsEnabled = true;
                }
                else if (action.Header == "Study Properties")
                {
                    action.IsEnabled = true;
                }
                else if (action.Header == "View Transactions")
                {
                    action.IsEnabled = true;
                }
                else if (action.Header == "Open Study")
                {
                    action.IsEnabled = true;
                }
                else if (action.Header == "Create Study")
                {
                    action.IsEnabled = true;
                }
            }
            StudyCache = null;
            AddBaseElements();
            SaveDefaultStudyProperties(studyName, folderPathForNewStudy, description);
        }
        private void SaveDefaultStudyProperties(string studyName, string folderPathForNewStudy, string description)
        {
            StudyPropertiesElement elemToSave = new StudyPropertiesElement(studyName, folderPathForNewStudy, description);
            PersistenceFactory.GetStudyPropertiesPersistenceManager().SaveNew(elemToSave);
        }
        private void StudyProperties(object arg1, EventArgs arg2)
        {
            List<StudyPropertiesElement> studyProps = StudyCache.GetChildElementsOfType<StudyPropertiesElement>();
            if(studyProps.Count>0)
            {
                PropertiesVM prop =  new PropertiesVM(studyProps[0]);
                string header = "Study Properties";
                DynamicTabVM tab = new DynamicTabVM(header, prop, "Properties");
                Navigate(tab, true, true);
            } 
        }

        public void OpenStudyFromFilePath(string name, string path)
        {           
            OpeningADifferentStudy?.Invoke(this, new EventArgs());
            //if a study is opened and the create new study tab is still in the tabs, then remove it
            TabController.Instance.RemoveTab("CreateNewStudy");

            UpdateRecentStudiesFile(path);
            
            Connection.Instance.ProjectFile = path;

            Name = name;
            UpdateTreeViewHeader(name);
            StudyCache = null;
            AddBaseElements();
            // add any children based on tables that exist.
            foreach (BaseFdaElement ele in Elements)
            {
                if (ele is ParentElement)
                {         
                    //todo: what is this?
                    //((ParentElement)ele).AddChildrenFromTable();
                }
            }
            foreach (NamedAction action in Actions)
            {
                if (action.Header == "Save Study")
                {
                    action.IsEnabled = true;
                }
                else if (action.Header == "Study Properties")
                {
                    action.IsEnabled = true;
                }
                else if (action.Header == "View Transactions")
                {
                    action.IsEnabled = true;
                }
                else if (action.Header == "Open Study")
                {
                    action.IsEnabled = true;
                }
                else if (action.Header == "Create Study")
                {
                    action.IsEnabled = true;
                }
            }

            StudyStatusBar.SaveStatus = "Study Loaded: " + DateTime.Now.ToString("G");
        }

        private void ImportStudyFromOldFda(object sender, EventArgs e)
        {
            ImportFromOldFdaVM vm = new ImportFromOldFdaVM(this);
            string header = StringConstants.IMPORT_FROM_OLD_FDA;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportStudy");
            Navigate(tab, false, false);
        }

        private void OpenStudy(object sender, EventArgs e)
        {
            Study.ExistingStudyVM ESVM = new ExistingStudyVM(this);
            string header = "Open Study";
            DynamicTabVM tab = new DynamicTabVM(header, ESVM, "OpenStudy");
            Navigate( tab, false, false);

        }
        
        public void AddBaseElements()
        {
            //clear out any existing ones from an existing study
            Elements.Clear();
            if (Connection.Instance.IsConnectionNull) return;

            //the tabs are in the fdastudyvm, i might need to throw an event here that is saying that a new study is opening and then remove all the tabs and 
            //deal with the map window.
            bool loadStudyCache = false;
            FDACache cache = null;
            if (StudyCache == null)
            {
                loadStudyCache = true;
                cache = FDACache.Create();
                StudyCache = cache;
                PersistenceFactory.StudyCacheForSaving = cache;

                TerrainOwnerElement t = new TerrainOwnerElement();
                AddElement(t);
                t.RenameMapTreeViewElement += RenameTreeViewElement;
                t.AddMapTreeViewElementBackIn += AddBackInTreeViewElement;
                cache.TerrainParent = t;

                ImpactAreaOwnerElement i = new ImpactAreaOwnerElement();
                AddElement(i);
                cache.ImpactAreaParent = i;

                WaterSurfaceElevationOwnerElement wse = new WaterSurfaceElevationOwnerElement();

                AddElement(wse);

                FrequencyRelationships.FrequencyRelationshipsOwnerElement f = new FrequencyRelationships.FrequencyRelationshipsOwnerElement();
                f.AddBaseElements(cache);
                AddElement(f);

                FlowTransforms.FlowTransformsOwnerElement ft = new FlowTransforms.FlowTransformsOwnerElement();
                ft.AddBaseElements(cache);
                AddElement(ft);

                StageTransforms.StageTransformsOwnerElement s = new StageTransforms.StageTransformsOwnerElement();
                s.AddBaseElements(cache);
                AddElement(s);

                GeoTech.LateralStructuresOwnerElement ls = new GeoTech.LateralStructuresOwnerElement();
                ls.AddBaseElements(cache);
                AddElement(ls);

                Inventory.InventoryOwnerElement inv = new Inventory.InventoryOwnerElement();
                inv.AddBaseElements(cache);
                AddElement(inv);

                IASOwnerElement c = new IASOwnerElement();
                AddElement(c);

                Alternatives.AlternativeOwnerElement plans = new Alternatives.AlternativeOwnerElement();
                AddElement(plans);

                AlternativeComparisonReportOwnerElement altComparisonReportOwner = new AlternativeComparisonReportOwnerElement();
                AddElement(altComparisonReportOwner);

                if (loadStudyCache)
                {
                    cache.IASParent = c;
                    LoadElementsFromDB();
                }

                UpdateTransactionsAndMessages?.Invoke(this, new EventArgs());
                LoadMapLayers?.Invoke(this, new EventArgs());
            }
        }

        #region Load Elements
        private void LoadElementsFromDB()
        {
            PersistenceFactory.GetRatingManager().Load();
            PersistenceFactory.GetTerrainManager().Load();
            PersistenceFactory.GetImpactAreaManager().Load();
            PersistenceFactory.GetWaterSurfaceManager().Load();
            PersistenceFactory.GetFlowFrequencyManager().Load();
            PersistenceFactory.GetInflowOutflowManager().Load();
            PersistenceFactory.GetExteriorInteriorManager().Load();
            PersistenceFactory.GetLeveeManager().Load();
            PersistenceFactory.GetStageDamageManager().Load();
            PersistenceFactory.GetStructureInventoryManager().Load();
            PersistenceFactory.GetIASManager().Load();
            PersistenceFactory.GetAlternativeManager().Load();
            PersistenceFactory.GetAlternativeCompReportManager().Load();
            PersistenceFactory.GetOccTypeManager().Load();
            PersistenceFactory.GetStudyPropertiesManager().Load();
        }

        #endregion

        #endregion
        #region Functions
        #endregion
    }
}
