using System;
using System.Collections.Generic;
using FdaViewModel.Utilities;
using FdaModel.ComputationPoint;
using FdaModel.Functions.OrdinatesFunctions;
using System.Collections.ObjectModel;
using FdaViewModel.Conditions;

namespace FdaViewModel.Study
{
    public class StudyElement : ParentElement
    {
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
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/Terrain.png");
            _Elements = new System.Collections.ObjectModel.ObservableCollection<BaseFdaElement>();

            NamedAction open = new NamedAction();
            open.Header = "Open Study";
            open.Action = OpenStudy;

            NamedAction create = new NamedAction();
            create.Header = "Create Study";
            create.Action = CreateStudy;

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

            NamedAction testing = new NamedAction();
            testing.Header = "Plot Testing";
            testing.Action = TestingAction;

            //NamedAction otherTesting = new NamedAction();
            //otherTesting.Header = "Other Testing";
            //otherTesting.Action = otherTestingAction;

            //NamedAction recent = new NamedAction();
            //recent.Header = "Recent";
            //recent.Action = OpenStudy;

            List<NamedAction> localactions = new List<NamedAction>();
            localactions.Add(create);
            localactions.Add(open);
            localactions.Add(properties);
            localactions.Add(save);
            localactions.Add(transactions);
            localactions.Add(testing);

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
            Navigate(new Utilities.Transactions.TransactionVM());
        }



        #endregion
        #region Voids

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
                registrykey.SetValue(i.ToString(), registrystudies[i]);
            }
        }



        private void otherTestingAction(object arg1, EventArgs arg2)
        {

        }
        private void TestingAction(object arg1, EventArgs arg2)
        {

            double[] peakFlowData = new double[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000 };
            FdaModel.Functions.FrequencyFunctions.LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);

            double[] x = new double[] { 0, 10000 };// 200, 500, 1200, 2000, 10000 };
            double[] y = new double[] { 300, 13000 };// 300, 500, 1000, 2500, 10000 };
            OrdinatesFunction one = new OrdinatesFunction(x, y, FdaModel.Functions.FunctionTypes.InflowOutflow);

            double[] xs = new double[] { 100, 200, 500, 1000, 2000,12000 };
            double[] ys = new double[] { 1, 2, 5, 10, 20 ,25};
            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction three = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(xs, ys, FdaModel.Functions.FunctionTypes.Rating);

            double[] xval = new double[] {  2, 3, 4, 5, 6, 7, 8, 9, 10,20 };
            double[] yval = new double[] {  1, 2, 3, 4, 5, 6, 7, 8, 9,20 };
            OrdinatesFunction five = new OrdinatesFunction(xval, yval, FdaModel.Functions.FunctionTypes.ExteriorInteriorStage);

            double[] x3 = new double[] { .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f };
            double[] y3 = new double[] { 2, 200, 300, 600, 1100, 2000, 3000, 4000 };
            OrdinatesFunction six = new OrdinatesFunction(x3, y3, FdaModel.Functions.FunctionTypes.InteriorStageFrequency);

            double[] x4 = new double[] { 5, 7, 8, 9, 10, 12, 15, 20,25 };
            double[] y4 = new double[] { 600, 1100, 1300, 1800, 2000, 3000, 4000, 4200,10000 };
            OrdinatesFunction seven = new OrdinatesFunction(x4, y4, FdaModel.Functions.FunctionTypes.InteriorStageDamage);

            double[] x5 = new double[] { .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f };
            double[] y5 = new double[] { 2, 200, 300, 600, 1100, 2000, 3000, 4000 };
            OrdinatesFunction eight = new OrdinatesFunction(x5, y5, FdaModel.Functions.FunctionTypes.DamageFrequency);

            double[] x62 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double[] y62 = new double[] { 0, .05f, .1f, .2f, .3f, .4f, .7f, .8f, .9f, .95f, 1 };
            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction nine = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(x62, y62, FdaModel.Functions.FunctionTypes.LeveeFailure);

            double[] x2 = new double[] { .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f };
            double[] y2 = new double[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction four = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(x2, y2, FdaModel.Functions.FunctionTypes.ExteriorStageFrequency);




            //4. Create Threshold
            PerformanceThreshold threshold = new PerformanceThreshold(PerformanceThresholdTypes.InteriorStage, 8);


            //5. Create computable object
            List<FdaModel.Functions.BaseFunction> myListOfBaseFunctions = new List<FdaModel.Functions.BaseFunction>() { zero,one, three,five, seven };
            //InputFunctions = myListOfBaseFunctions;

            LateralStructure myLateralStruct = new LateralStructure(10);

            Condition simpleTest = new Condition(2008, "russian river", myListOfBaseFunctions, threshold, myLateralStruct); //bool call Validate

            Random randomNumberGenerator = new Random(0);

            FdaModel.ComputationPoint.Outputs.Realization simpleTestResult = new FdaModel.ComputationPoint.Outputs.Realization(simpleTest, false, false); //bool oldCompute, bool performance only

            simpleTestResult.Compute(randomNumberGenerator);

            //foreach (FdaModel.Functions.BaseFunction func in simpleTestResult.Functions)
            //{
            //    if( func.FunctionType == FdaModel.Functions.FunctionTypes.ExteriorStageFrequency)
            //    {
            //        List<FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction> listOfOrdFuncs = new List<FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction>() { nine, (FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func };

            //        FdaViewModel.Plots.ParentUserControlVM vm = new Plots.ParentUserControlVM(listOfOrdFuncs);
            //        //NewStudyVM vm = new NewStudyVM();
            //        Navigate(vm, true, true);
            //    }
            //}
            

            //Output.LinkedPlotsVM vm2 = new Output.LinkedPlotsVM(simpleTestResult);
            Plots.LinkedPlotsVM vm = new Plots.LinkedPlotsVM(simpleTestResult);

            Navigate(vm, true, true);










        }

        private void SaveStudy(object arg1, EventArgs arg2)
        {
           // Save();
        }
        private void CreateStudy(object arg1, EventArgs arg2)
        {
            NewStudyVM vm = new NewStudyVM();
            Navigate(vm, true, true);
            if (!vm.HasError)
            {

                Name = vm.StudyName;
                //check if file exists.
                string newStudyPath = vm.Path + "\\" + vm.StudyName + "\\" + vm.StudyName + ".sqlite";
                if (!System.IO.File.Exists(newStudyPath))
                {
                    Storage.Connection.Instance.ProjectFile = newStudyPath;
                    UpdateRecentStudiesFile(newStudyPath);

                }
                else
                {
                    ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("A study with that name already exists.",
                        FdaModel.Utilities.Messager.ErrorMessageEnum.Report | FdaModel.Utilities.Messager.ErrorMessageEnum.View));

                    Storage.Connection.Instance.ProjectFile = vm.Path + "\\" + vm.StudyName + "\\" + vm.StudyName + ".sqlite";
                }
                PropertiesVM properties = new PropertiesVM(vm.StudyName, vm.Path);
                properties.Save();
                AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(vm.StudyName, Utilities.Transactions.TransactionEnum.CreateNew, "Initialize study"));
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
                
                AddBaseElements();
            }
        }
        private void StudyProperties(object arg1, EventArgs arg2)
        {
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                Navigate(new PropertiesVM(Storage.Connection.Instance.GetTable(PropertiesVM.TableName)), true, true);
            }
            else
            {
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("Study Properties was accessed without the study path or study name being defined.", FdaModel.Utilities.Messager.ErrorMessageEnum.Report | FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel));
            }
        }

        private void OpenStudyFromFilePath(string name, string path)
        {
            //Elements.Clear();//if there is already a study, clear it out

            UpdateRecentStudiesFile(path);
            
            Storage.Connection.Instance.ProjectFile = path;

            Name = name;
            UpdateTreeViewHeader(name);
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
        }
        private void OpenStudy(object sender, EventArgs e)
        {
            Study.ExistingStudyVM ESVM = new ExistingStudyVM();
            Navigate(ESVM, true, true, "Open Existing Study");
            if (!ESVM.WasCanceled)
            {
                if (!ESVM.HasError)
                {
                    OpenStudyFromFilePath(ESVM.StudyName, ESVM.Path);
                }
            }



        }
        
        public override void AddValidationRules()
        {

            //throw new NotImplementedException();
        }

     

       
        public void AddBaseElements()
        {

            Elements.Clear();//clear out any existing ones from an existing study
            if (Storage.Connection.Instance.IsConnectionNull) return;

            bool loadStudyCache = false;
            if (StudyCache == null)
            {
                loadStudyCache = true;
                StudyCache = FDACache.Create();
            }

            Watershed.TerrainOwnerElement t = new Watershed.TerrainOwnerElement(this);
            AddElement(t);
            if (loadStudyCache) { StudyCache.TerrainParent = t; }

            ImpactArea.ImpactAreaOwnerElement i = new ImpactArea.ImpactAreaOwnerElement(this);
            AddElement(i);

            WaterSurfaceElevation.WaterSurfaceElevationOwnerElement wse = new WaterSurfaceElevation.WaterSurfaceElevationOwnerElement(this);
            
            AddElement(wse);

            FrequencyRelationships.FrequencyRelationshipsOwnerElement f = new FrequencyRelationships.FrequencyRelationshipsOwnerElement(this);
            f.AddBaseElements();
            AddElement(f);

            FlowTransforms.FlowTransformsOwnerElement ft = new FlowTransforms.FlowTransformsOwnerElement(this);
            ft.AddBaseElements();
            AddElement(ft);

            //InflowOutflow.InflowOutflowOwnerElement inout = new InflowOutflow.InflowOutflowOwnerElement(this);
            //AddElement(inout);

            StageTransforms.StageTransformsOwnerElement s = new StageTransforms.StageTransformsOwnerElement(this);
            s.AddBaseElements();
            AddElement(s);

            Hydraulics.FloodPlainDataOwnerElement h = new Hydraulics.FloodPlainDataOwnerElement(this);
            //this.AddElement(h);

            GeoTech.LateralStructuresOwnerElement ls = new GeoTech.LateralStructuresOwnerElement(this);
            ls.AddBaseElements();
            AddElement(ls);

            //GeoTech.LeveeFeatureOwnerElement l = new GeoTech.LeveeFeatureOwnerElement(this);
            //this.AddElement(l);

            Inventory.InventoryOwnerElement inv = new Inventory.InventoryOwnerElement(this);
            inv.AddBaseElements();
            AddElement(inv);

            Conditions.ConditionsOwnerElement c = new Conditions.ConditionsOwnerElement(this);
            AddElement(c);
            if (loadStudyCache) { StudyCache.ConditionsParent = c; }

            if (loadStudyCache == true)
            {
                StudyCache.LoadFDACache();
            }

            UpdateTheConditionsTree(this, new EventArgs());
            UpdateTransactionsAndMessages?.Invoke(this, new EventArgs());
            LoadMapLayers?.Invoke(this, new EventArgs());

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
            ObservableCollection<BaseFdaElement> conditions = new ObservableCollection<BaseFdaElement>();
            //get all the current conditions
            ConditionsOwnerElement studyTreeCondOwnerElement = null;
            if (Elements.Count > 0)
            {
                foreach (ParentElement owner in Elements)
                {
                    if (owner.GetType() == typeof(ConditionsOwnerElement))
                    {
                        conditions = owner.Elements;
                        studyTreeCondOwnerElement = (ConditionsOwnerElement)owner;
                    }
                }
            }
            else
            {
                return;
            }

            ConditionsTreeOwnerElement condTreeCondOwnerElement = new ConditionsTreeOwnerElement(studyTreeCondOwnerElement);
            condTreeCondOwnerElement.RequestNavigation += Navigate;
            condTreeCondOwnerElement.UpdateConditionsTree += UpdateTheConditionsTree;

            if (conditions.Count > 0)
            {
                foreach (ChildElement elem in conditions)
                {
                    //create a new conditions element and change the way it renames, removes, and edits. The parent node
                    //will then tell the study tree what to do
                    ConditionsElement condElem = new ConditionsElement((ConditionsElement)elem);
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
