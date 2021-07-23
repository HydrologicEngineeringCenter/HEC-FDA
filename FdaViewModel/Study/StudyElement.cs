using System;
using System.Collections.Generic;
using FdaViewModel.Utilities;
using System.Collections.ObjectModel;
using FdaViewModel.Conditions;
using FdaViewModel.Tabs;
using FdaViewModel.StageTransforms;
using FdaViewModel.Watershed;
using FdaViewModel.Inventory;
using FdaViewModel.AggregatedStageDamage;
using FdaViewModel.GeoTech;
using FdaViewModel.FlowTransforms;
using FdaViewModel.FrequencyRelationships;
using FdaViewModel.WaterSurfaceElevation;
using FdaViewModel.ImpactArea;
using Functions;
using FdaViewModel.AlternativeComparisonReport;

namespace FdaViewModel.Study
{
    public class StudyElement : ParentElement
    {
        private const string IMPORT_FROM_OLD_FDA = "Import Study From Fda 1";

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
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/Terrain.png");
            _Elements = new System.Collections.ObjectModel.ObservableCollection<BaseFdaElement>();

            NamedAction open = new NamedAction();
            open.Header = "Open Study";
            open.Action = OpenStudy;

            NamedAction importStudyFromOldFda = new NamedAction();
            importStudyFromOldFda.Header = IMPORT_FROM_OLD_FDA;
            importStudyFromOldFda.Action = ImportStudyFromOldFda;

            NamedAction create = new NamedAction();
            create.Header = "Create Study";
            create.Action = CreateStudyFromWindow;

            //NamedAction rename = new NamedAction();
            //rename.Header = "Rename Study";
            //rename.Action = RenameStudy;

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

            

            //NamedAction otherTesting = new NamedAction();
            //otherTesting.Header = "Other Testing";
            //otherTesting.Action = otherTestingAction;

            //NamedAction recent = new NamedAction();
            //recent.Header = "Recent";
            //recent.Action = OpenStudy;

            List<NamedAction> localactions = new List<NamedAction>();
            localactions.Add(create);
            localactions.Add(open);
            localactions.Add(importStudyFromOldFda);
           // localactions.Add(rename);
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


            //Save();
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
            //RenameVM renameViewModel = new RenameVM(this, CloneElement);
            //renameViewModel.ParentGUID = this.GUID;
            //Navigate(renameViewModel, false, true, "Rename");
        }
        private void OpenStudyFromRecent(object sender, EventArgs e)
        {
            //ClearStudy?.Invoke(sender, e);
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
            //if(registryStudies.Count == 0)
            //{

            //}
            //else
            //{
            //    for(int i = 0;i<registryStudies.Count;i++)
            //    {
                    
            //    }
            //}
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



        //private void otherTestingAction(object arg1, EventArgs arg2)
        //{

        //}
        //private void TestingAction(object arg1, EventArgs arg2)
        //{

        //    double[] peakFlowData = new double[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000 };
        //    FdaModel.Functions.FrequencyFunctions.LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);

        //    double[] x = new double[] { 0, 10000 };// 200, 500, 1200, 2000, 10000 };
        //    double[] y = new double[] { 300, 13000 };// 300, 500, 1000, 2500, 10000 };
        //    OrdinatesFunction one = new OrdinatesFunction(x, y, FdaModel.Functions.FunctionTypes.InflowOutflow);

        //    double[] xs = new double[] { 100, 200, 500, 1000, 2000,12000 };
        //    double[] ys = new double[] { 1, 2, 5, 10, 20 ,25};
        //    FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction three = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(xs, ys, FdaModel.Functions.FunctionTypes.Rating);

        //    double[] xval = new double[] {  2, 3, 4, 5, 6, 7, 8, 9, 10,20 };
        //    double[] yval = new double[] {  1, 2, 3, 4, 5, 6, 7, 8, 9,20 };
        //    OrdinatesFunction five = new OrdinatesFunction(xval, yval, FdaModel.Functions.FunctionTypes.ExteriorInteriorStage);

        //    double[] x3 = new double[] { .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f };
        //    double[] y3 = new double[] { 2, 200, 300, 600, 1100, 2000, 3000, 4000 };
        //    OrdinatesFunction six = new OrdinatesFunction(x3, y3, FdaModel.Functions.FunctionTypes.InteriorStageFrequency);

        //    double[] x4 = new double[] { 5, 7, 8, 9, 10, 12, 15, 20,25 };
        //    double[] y4 = new double[] { 600, 1100, 1300, 1800, 2000, 3000, 4000, 4200,10000 };
        //    OrdinatesFunction seven = new OrdinatesFunction(x4, y4, FdaModel.Functions.FunctionTypes.InteriorStageDamage);

        //    double[] x5 = new double[] { .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f };
        //    double[] y5 = new double[] { 2, 200, 300, 600, 1100, 2000, 3000, 4000 };
        //    OrdinatesFunction eight = new OrdinatesFunction(x5, y5, FdaModel.Functions.FunctionTypes.DamageFrequency);

        //    double[] x62 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        //    double[] y62 = new double[] { 0, .05f, .1f, .2f, .3f, .4f, .7f, .8f, .9f, .95f, 1 };
        //    FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction nine = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(x62, y62, FdaModel.Functions.FunctionTypes.LeveeFailure);

        //    double[] x2 = new double[] { .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f };
        //    double[] y2 = new double[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        //    FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction four = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(x2, y2, FdaModel.Functions.FunctionTypes.ExteriorStageFrequency);




        //    //4. Create Threshold
        //    PerformanceThreshold threshold = new PerformanceThreshold(PerformanceThresholdTypes.InteriorStage, 8);


        //    //5. Create computable object
        //    List<FdaModel.Functions.BaseFunction> myListOfBaseFunctions = new List<FdaModel.Functions.BaseFunction>() { zero,one, three,five, seven };
        //    //InputFunctions = myListOfBaseFunctions;

        //    LateralStructure myLateralStruct = new LateralStructure(10);

        //    Condition simpleTest = new Condition(2008, "russian river", myListOfBaseFunctions, threshold, myLateralStruct); //bool call Validate

        //    Random randomNumberGenerator = new Random(0);

        //    FdaModel.ComputationPoint.Outputs.Realization simpleTestResult = new FdaModel.ComputationPoint.Outputs.Realization(simpleTest, false, false); //bool oldCompute, bool performance only

        //    simpleTestResult.Compute(randomNumberGenerator);

        //    //foreach (FdaModel.Functions.BaseFunction func in simpleTestResult.Functions)
        //    //{
        //    //    if( func.FunctionType == FdaModel.Functions.FunctionTypes.ExteriorStageFrequency)
        //    //    {
        //    //        List<FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction> listOfOrdFuncs = new List<FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction>() { nine, (FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func };

        //    //        FdaViewModel.Plots.ParentUserControlVM vm = new Plots.ParentUserControlVM(listOfOrdFuncs);
        //    //        //NewStudyVM vm = new NewStudyVM();
        //    //        Navigate(vm, true, true);
        //    //    }
        //    //}
            

        //    //Output.LinkedPlotsVM vm2 = new Output.LinkedPlotsVM(simpleTestResult);
        //    Plots.LinkedPlotsVM vm = new Plots.LinkedPlotsVM(simpleTestResult);

        //    Navigate( vm, true, true);










        //}

        private void SaveStudy(object arg1, EventArgs arg2)
        {
            // Save();
            SaveTheOpenTabs?.Invoke(arg1, arg2);

        }
        private void CreateStudyFromWindow(object arg1, EventArgs arg2)
        {
            NewStudyVM vm = new NewStudyVM(this);
            string header = "Create New Study";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "StudyElement");
            Navigate( tab, false,false);
            //if (!vm.HasError)
            //{
            //    CreateStudyFromViewModel(vm);
                
            //}
        }

        public void CreateStudyFromOldFdaImportFile(ImportFromOldFdaVM vm)
        {

        }
      

        public void CreateStudyFromViewModel(string studyName, string folderPathForNewStudy)
        {
            Name = studyName;
            UpdateTreeViewHeader(Name);
            //check if file exists.
            string newStudyPath = folderPathForNewStudy + "\\" + studyName + "\\" + studyName + ".sqlite";
            if (!System.IO.File.Exists(newStudyPath))
            {
                Storage.Connection.Instance.ProjectFile = newStudyPath;
                UpdateRecentStudiesFile(newStudyPath);

            }
            else
            {
                Storage.Connection.Instance.ProjectFile = folderPathForNewStudy + "\\" + studyName + "\\" + studyName + ".sqlite";
            }
            PropertiesVM properties = new PropertiesVM(studyName, folderPathForNewStudy);
            properties.Save();
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
        }

        private void StudyProperties(object arg1, EventArgs arg2)
        {
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                PropertiesVM prop =  new PropertiesVM(Storage.Connection.Instance.GetTable(PropertiesVM.TableName));
                string header = "Study Properties";
                DynamicTabVM tab = new DynamicTabVM(header, prop, "Properties");
                Navigate(tab, true, true);
            }
            else
            {
                //ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("Study Properties was accessed without the study path or study name being defined.", FdaModel.Utilities.Messager.ErrorMessageEnum.Report | FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel));
            }
        }

        public void OpenStudyFromFilePath(string name, string path)
        {           
            OpeningADifferentStudy?.Invoke(this, new EventArgs());
            //if a study is opened and the create new study tab is still in the tabs, then remove it
            TabController.Instance.RemoveTab("CreateNewStudy");

            UpdateRecentStudiesFile(path);
            
            Storage.Connection.Instance.ProjectFile = path;

            Name = name;
            UpdateTreeViewHeader(name);
            StudyCache = null;
            AddBaseElements();
            // add any children based on tables that exist.
            foreach (BaseFdaElement ele in Elements)
            {
                if (ele is ParentElement)
                {             
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
            string header = "Import Study From Fda 1.0";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportStudy");
            Navigate(tab, false, false);
        }

        private void OpenStudy(object sender, EventArgs e)
        {
            Study.ExistingStudyVM ESVM = new ExistingStudyVM(this);
            string header = "Open Study";
            DynamicTabVM tab = new DynamicTabVM(header, ESVM, "OpenStudy");
            Navigate( tab, false, false);

            //if (!ESVM.WasCanceled)
            //{
            //    if (!ESVM.HasError)
            //    {
            //        OpenStudyFromFilePath(ESVM.StudyName, ESVM.Path);
            //    }
            //}



        }
        
        public override void AddValidationRules()
        {

            //throw new NotImplementedException();
        }




        public void AddBaseElements()
        {

            Elements.Clear();//clear out any existing ones from an existing study
            if (Storage.Connection.Instance.IsConnectionNull) return;

            //the tabs are in the fdastudyvm, i might need to throw an event here that is saying that a new study is opening and then remove all the tabs and 
            //deal with the map window.
            bool loadStudyCache = false;
            FDACache cache = null;
            if (StudyCache == null)
            {
                loadStudyCache = true;
                cache = FDACache.Create();
                StudyCache = cache;
                Saving.PersistenceFactory.StudyCacheForSaving = cache;


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

                //Hydraulics.FloodPlainDataOwnerElement h = new Hydraulics.FloodPlainDataOwnerElement(this);
                //this.AddElement(h);

                GeoTech.LateralStructuresOwnerElement ls = new GeoTech.LateralStructuresOwnerElement();
                ls.AddBaseElements(cache);
                AddElement(ls);



                Inventory.InventoryOwnerElement inv = new Inventory.InventoryOwnerElement();
                inv.AddBaseElements(cache);
                AddElement(inv);

                Conditions.ConditionsOwnerElement c = new Conditions.ConditionsOwnerElement();
                AddElement(c);

                Alternatives.AltervativeOwnerElement plans = new Alternatives.AltervativeOwnerElement();
                AddElement(plans);

                AlternativeComparisonReportOwnerElement altComparisonReportOwner = new AlternativeComparisonReportOwnerElement();
                AddElement(altComparisonReportOwner);

                if (loadStudyCache)
                {
                    cache.ConditionsParent = c;
                    LoadElementsFromDB();
                }

                ConditionsTreeOwnerElement ct = new ConditionsTreeOwnerElement(c);
                cache.ConditionsElementUpdated += ConditionsElementWasUpdated; //ct.ConditionWasUpdated;
                cache.ConditionsElementAdded += UpdateTheConditionsTree;
                if (loadStudyCache)
                {
                    cache.ConditionsTreeParent = ct;
                }

                UpdateTheConditionsTree(this, new EventArgs());
                UpdateTransactionsAndMessages?.Invoke(this, new EventArgs());
                LoadMapLayers?.Invoke(this, new EventArgs());
            }
        }

        #region Load Elements
        private void LoadElementsFromDB()
        {
            Saving.PersistenceFactory.GetRatingManager().Load();
            Saving.PersistenceFactory.GetTerrainManager().Load();
            Saving.PersistenceFactory.GetImpactAreaManager().Load();
            Saving.PersistenceFactory.GetWaterSurfaceManager().Load();
            Saving.PersistenceFactory.GetFlowFrequencyManager().Load();
            Saving.PersistenceFactory.GetInflowOutflowManager().Load();
            Saving.PersistenceFactory.GetExteriorInteriorManager().Load();
            Saving.PersistenceFactory.GetLeveeManager().Load();
            Saving.PersistenceFactory.GetFailureFunctionManager().Load();
            Saving.PersistenceFactory.GetStageDamageManager().Load();
            Saving.PersistenceFactory.GetStructureInventoryManager().Load();
            Saving.PersistenceFactory.GetConditionsManager().Load();
            Saving.PersistenceFactory.GetOccTypeManager().Load();

        }

        #endregion

        /// <summary>
        /// This stuff is getting a little wierd. It was done before the new "StudyCache" stuff. So it seems like i could just go straight to the cache
        /// and not have to get the nodes from the study tree, but i want them to be linked. I don't want the conditions tree to have its own nodes.
        /// when a conditions element gets updated (saved) then it actually gets rid of the old one and creates a new one. This breaks the connection to 
        /// the one in the conditions tree. So i need to call update on the conditions tree, but it was losing the "isExpanded" value, so i am adding
        /// this method inbetween.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ConditionsElementWasUpdated(object sender, Saving.ElementUpdatedEventArgs args)
        {
            UpdateTheConditionsTree(sender, args);
            if(ConditionsTree.Count<= 0) { return; }
            //get the current 
            ConditionsElement oldElem = (ConditionsElement)args.OldElement;
            if(oldElem.IsExpanded == true)
            {
                //i need to expand the new element that was added to the cond tree
                string name = args.NewElement.Name;

                foreach(ConditionsElement elem in ConditionsTree[0].Elements)
                {
                    if(elem.Name.Equals(name))
                    {
                        elem.IsExpanded = true;
                    }
                }
            }
        }


        /// <summary>
        /// The study tree tab shows in real time the state of the study.
        /// When you switch to the conditions tab this method will grab the state of the 
        /// study tree conditions and mirror that in the conditions tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UpdateTheConditionsTree(object sender, EventArgs e)
        {
            //ObservableCollection<BaseFdaElement> conditions = new ObservableCollection<BaseFdaElement>();
            //get all the current conditions
            //ConditionsOwnerElement studyTreeCondOwnerElement = 
            if (Elements.Count <= 0) { return; }
            //{
                //foreach (ParentElement owner in Elements)
                //{
                //    if (owner.GetType() == typeof(ConditionsOwnerElement))
                //    {
                //        conditions = owner.Elements;
                //        studyTreeCondOwnerElement = (ConditionsOwnerElement)owner;
                //    }
                //}
            //}
            //else
            //{
            //    return;
            //}
            //List<ConditionsElement> conditions = StudyCache.GetChildElementsOfType<ConditionsElement>();
            ConditionsOwnerElement studyCondOwner = StudyCache.GetParentElementOfType<ConditionsOwnerElement>();
            ConditionsTreeOwnerElement condTreeCondOwnerElement = new ConditionsTreeOwnerElement(studyCondOwner);
            condTreeCondOwnerElement.RequestNavigation += Navigate;
            condTreeCondOwnerElement.UpdateConditionsTree += UpdateTheConditionsTree;

            if (studyCondOwner.Elements.Count > 0)
            {
                foreach (ConditionsElement elem in studyCondOwner.Elements)
                {
                    //create a new conditions element and change the way it renames, removes, and edits. The parent node
                    //will then tell the study tree what to do
                    ConditionsElement condElem = new ConditionsElement(elem);
                    condElem.EditConditionsTreeElement += condTreeCondOwnerElement.EditCondition;
                    condElem.RemoveConditionsTreeElement += condTreeCondOwnerElement.RemoveElement;
                    condElem.RenameConditionsTreeElement += condTreeCondOwnerElement.RenameElement;
                    condElem.UpdateExpansionValueInTreeElement += condTreeCondOwnerElement.UpdateElementExpandedValue;
                    condTreeCondOwnerElement.AddElement(condElem, false);
                }

            }

            //have to make it new to call the notified prop changed
            ConditionsTree = new ObservableCollection<ParentElement>() { condTreeCondOwnerElement };
        }



        #endregion
        #region Functions
  
        #endregion



    }
}
