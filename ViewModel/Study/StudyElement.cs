using HEC.FDA.ViewModel.AlternativeComparisonReport;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Tabs;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using HEC.FDA.ViewModel.WaterSurfaceElevation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Controls;

namespace HEC.FDA.ViewModel.Study
{
    public class StudyElement : ParentElement
    {
        /// <summary>
        /// Every study that gets created gets regestered on the local computer. We display the most recent 5 studies
        /// to the user in the File menu.
        /// </summary>
        private List<string> _RegistryStudies = new List<string>();
        private bool _StudyLoaded;

        #region Properties
        public List<string> RegistryStudyPaths
        {
            get { return _RegistryStudies; }
        }

        /// <summary>
        /// Boolean so that i can bind the "Properties" menu item. There is always a study, but the initial one is empty. 
        /// I needed something to bind to once an actual study is created or opened.
        /// </summary>
        public bool StudyLoaded
        {
            get { return _StudyLoaded; }
            set { _StudyLoaded = value; NotifyPropertyChanged(); }
        }

        #endregion
        #region Constructors
        public StudyElement() : base()
        {
            PopulateRecentStudies();
            FontSize = 18;
            Name = "Study";
            CustomTreeViewHeader = new CustomHeaderVM(Name, ImageSources.TERRAIN_IMAGE);
            _Elements = new ObservableCollection<BaseFdaElement>();
        }

        #endregion
        #region Voids
        public void OpenStudyFromRecent(object sender, EventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                string filePath = menuItem.Tag as string;
                OpenStudyFromFilePath(Path.GetFileNameWithoutExtension(filePath), filePath);
            }
        }

        /// <summary>
        /// Reads fda 2.0 studies from the registry so that we can display them to the user to select from in the File menu.
        /// </summary>
        private void PopulateRecentStudies()
        {
            string appName = Assembly.GetExecutingAssembly().GetName().ToString();
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var companyName = fvi.CompanyName;

            string subKey = companyName + "\\" + appName;
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(subKey);
            if (registryKey == null)
            {
                registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(subKey);
            }

            int idx = 0;
            while (idx < registryKey.ValueCount && _RegistryStudies.Count < 5)
            {
                string registryNextLine = registryKey.GetValue(idx.ToString()).ToString();
                if (File.Exists(registryNextLine))
                {
                    _RegistryStudies.Add(registryNextLine);
                }
                idx++;
            }
        }

        /// <summary>
        /// Adds a new study to the top of the recent studies list or updates a previous study to be top of the list
        /// if it has been selected.
        /// </summary>
        /// <param name="filepath"></param>
        private void UpdateRecentStudiesFile(string filepath)
        {
            string appname = Assembly.GetExecutingAssembly().GetName().ToString();

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var companyname = fvi.CompanyName;

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
                 
                if (File.Exists(registrynextline))
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

        public void CreateNewStudyMenuItemClicked()
        {
            NewStudyVM vm = new NewStudyVM(this);
            string header = "Create New Study";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "StudyElement");
            Navigate( tab, false,false);
        }
      
        public void CreateNewStudy(string studyName, string folderPathForNewStudy, string description)
        {
            TabController.Instance.CloseTabsAndWindowsOpeningNewStudy();
            Name = studyName;
            UpdateTreeViewHeader(Name);
            //check if file exists.
            string newStudyPath = folderPathForNewStudy + "\\" + studyName + "\\" + studyName + ".sqlite";
            if (!File.Exists(newStudyPath))
            {
                Connection.Instance.ProjectFile = newStudyPath;
                UpdateRecentStudiesFile(newStudyPath);
            }
            else
            {
                Connection.Instance.ProjectFile = folderPathForNewStudy + "\\" + studyName + "\\" + studyName + ".sqlite";
            }

            StudyCache = null;
            AddBaseElements();
            SaveDefaultStudyProperties(studyName, folderPathForNewStudy, description);
        }
        private void SaveDefaultStudyProperties(string studyName, string folderPathForNewStudy, string description)
        {
            int id = PersistenceFactory.GetStudyPropertiesManager().GetNextAvailableId();
            StudyPropertiesElement elemToSave = new StudyPropertiesElement(studyName, folderPathForNewStudy, description, id);
            PersistenceFactory.GetStudyPropertiesPersistenceManager().SaveNew(elemToSave);
        }
        public void StudyProperties()
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
            TabController.Instance.CloseTabsAndWindowsOpeningNewStudy();

            UpdateRecentStudiesFile(path);
            
            Connection.Instance.ProjectFile = path;

            Name = name;
            UpdateTreeViewHeader(name);
            StudyCache = null;
            AddBaseElements();
        }

        public void ImportStudyFromOldFda()
        {
            ImportStudyFromFda1VM vm = new ImportStudyFromFda1VM(this);
            string header = StringConstants.IMPORT_FROM_OLD_FDA;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportStudy");
            Navigate(tab, false, false);
        }

        public void OpenStudyMenuItemClicked()
        {
            ExistingStudyVM ESVM = new ExistingStudyVM(this);
            string header = "Open Study";
            DynamicTabVM tab = new DynamicTabVM(header, ESVM, "OpenStudy");
            Navigate( tab, false, false);
        }
        
        public void AddBaseElements()
        {
            Elements.Clear();
            if (Connection.Instance.IsConnectionNull) return;

            if (StudyCache == null)
            {
                FDACache cache = FDACache.Create();
                StudyCache = cache;
                PersistenceFactory.StudyCacheForSaving = cache;

                TerrainOwnerElement t = new TerrainOwnerElement();
                AddElement(t);
                cache.TerrainParent = t;

                ImpactAreasOwnerElement i = new ImpactAreasOwnerElement();
                i.AddBaseElements(cache);
                AddElement(i);

                HydraulicsOwnerElement wse = new HydraulicsOwnerElement();
                wse.AddBaseElements(cache);
                AddElement(wse);

                FrequencyRelationships.FrequencyRelationshipsOwnerElement f = new FrequencyRelationships.FrequencyRelationshipsOwnerElement();
                AddElement(f);

                InflowOutflowOwnerElement io = new InflowOutflowOwnerElement();
                AddElement(io);

                StageTransforms.StageTransformsOwnerElement s = new StageTransforms.StageTransformsOwnerElement();
                s.AddBaseElements(cache);
                AddElement(s);

                LeveeFeatureOwnerElement lf = new LeveeFeatureOwnerElement();
                AddElement(lf);
                cache.LeveeFeatureParent = lf;

                Inventory.InventoryOwnerElement inv = new Inventory.InventoryOwnerElement();
                inv.AddBaseElements(cache);
                AddElement(inv);

                IASOwnerElement c = new IASOwnerElement();
                AddElement(c);

                Alternatives.AlternativeOwnerElement plans = new Alternatives.AlternativeOwnerElement();
                AddElement(plans);

                AlternativeComparisonReportOwnerElement altComparisonReportOwner = new AlternativeComparisonReportOwnerElement();
                AddElement(altComparisonReportOwner);

                cache.IASParent = c;
                LoadElementsFromDB();
                StudyLoaded = true;
            }
        }

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
    }
}
