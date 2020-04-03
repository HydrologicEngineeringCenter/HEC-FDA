using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using FdaViewModel.Utilities;
using FdaViewModel.Saving.PersistenceManagers;
using Functions;
using FunctionsView.ViewModel;
using Model.Inputs.Functions.ImpactAreaFunctions;
using FdaViewModel.Inventory.DamageCategory;
using Functions.Ordinates;
using HEC.Plotting.SciChart2D.Charts;
using System.Collections.ObjectModel;
using FdaViewModel.Saving;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    //[Author(q0heccdm, 7 / 14 / 2017 1:55:50 PM)]
    public class OccupancyTypesEditorVM : Editors.BaseEditorVM
    {
        //i need a way to modify all the occtypes. 
        //i should make a copy of the occtypes when they come in.
        //then modify them as the user modifies them
        //i could do this when the user changes tabs
        //or do this when the user changes occtypes.
        //i think i will do the later.

        private List<IOccupancyTypeEditable> _NewlyCreatedOcctypes = new List<IOccupancyTypeEditable>();
        private List<IOccupancyTypeEditable> _OcctypesToDelete = new List<IOccupancyTypeEditable>();
        private List<IOccupancyTypeGroupEditable> _GroupsToUpdateInParentTable = new List<IOccupancyTypeGroupEditable>();
        private List<IOccupancyTypeGroupEditable> _GroupsToDelete = new List<IOccupancyTypeGroupEditable>();

        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/14/2017 1:55:50 PM
        #endregion
        #region Fields
        private ObservableCollection<IOccupancyTypeGroupEditable> _OccTypeGroups;
        private IOccupancyTypeGroupEditable _SelectedOccTypeGroup;
        private IOccupancyTypeEditable _SelectedOccType;
        //private Dictionary<string,DepthDamage.DepthDamageCurve> _DepthDamageCurveDictionary;
        //this dictionary is to keep track of the checkboxes that have been clicked in the tabs for each occtype
        //private Dictionary<string, bool[]> _OcctypeTabsSelectedDictionary;
        //private string _SelectedStructureDepthDamage;

        //private ICoordinatesFunction _StructureDepthDamageCurve;// = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);// Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        //private ICoordinatesFunction _ContentDepthDamageCurve;// = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        //private ICoordinatesFunction _VehicleDepthDamageCurve;// = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        //private ICoordinatesFunction _OtherDepthDamageCurve;// = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

        //private List<string> _StructureDepthDamageStringNames = new List<string>();
        //private List<string> _ContentDepthDamageStringNames = new List<string>();
        //private List<string> _VehicleDepthDamageStringNames = new List<string>();
        //private List<string> _OtherDepthDamageStringNames = new List<string>();

        private List<string> _DamageCategoriesList = new List<string>();
        //private string _Description;

        private string _Year;
        private string _Module;
        private CoordinatesFunctionEditorVM _StructureEditorVM;
        private CoordinatesFunctionEditorVM _ContentEditorVM;
        private CoordinatesFunctionEditorVM _VehicleEditorVM;
        private CoordinatesFunctionEditorVM _OtherEditorVM;


        #endregion
        #region Properties
        public Chart2D Chart { get; set; }
     
        public string Year
        {
            get { return _Year; }
            set { _Year = value; NotifyPropertyChanged(); }
        }

        public string Module
        {
            get { return _Module; }
            set { _Module = value; }
        }
       
        public List<string> DamageCategoriesList
        {
            get { return _DamageCategoriesList; }
            set { _DamageCategoriesList = value; NotifyPropertyChanged(); }
        }


        public IOccupancyTypeEditable SelectedOccType
        {
            get { return _SelectedOccType; }
            set  
            {   
                _SelectedOccType = value;
                if(_SelectedOccType == null)
                {
                    return;
                }
                //for now let us just say that the occtype is modified so that it will save
                //in the future we could add something fancier that tracks if a value was 
                //actually changed
                _SelectedOccType.OccType.IsModified = true;
                NotifyPropertyChanged(); 
            }
        }

        public IOccupancyTypeGroupEditable SelectedOccTypeGroup
        {
            get { return _SelectedOccTypeGroup; }
            set 
            { 
                if (value == null) 
                { 
                    return; 
                } 
                _SelectedOccTypeGroup = value; 
                //todo: do i really need this one?
                UpdateTheIsSelectedBoolOnEachOccTypeGroup(); 
                NotifyPropertyChanged(); 
            }
        }
        public ObservableCollection<IOccupancyTypeGroupEditable> OccTypeGroups
        {
            get { return _OccTypeGroups; }
            set { _OccTypeGroups = value; NotifyPropertyChanged(); }
        }

        #endregion
        #region Constructors
        public OccupancyTypesEditorVM(ObservableCollection<IOccupancyTypeGroupEditable> occtypeGroups, Editors.EditorActionManager manager) : base(manager)
        {

            Chart = new Chart2D();

            Name = "OccTypeEditor";//I just needed some name so that it doesn't fail the empty name test that is now universal.
            OccTypeGroups = occtypeGroups;
         //todo: do i need this call?
            AddEmptyCurvesToEmptyDepthDamages();

            //set the selected occtype group
            SelectedOccTypeGroup = occtypeGroups.First();
            SelectedOccType = SelectedOccTypeGroup.Occtypes.FirstOrDefault();

            //set all the "isModified" flags to false.
            ClearAllModifiedLists();
            //StudyCache.OccTypeElementAdded += UpdateGroupList;
        }

        #endregion
        #region Voids

        /// <summary>
        /// This method sets all the values that need to get set on an occtype when switching
        /// to a new occtype or before saving the editor. Some values do not need to get updated
        /// here becuase the binding updates them automatically.
        /// </summary>
        /// <param name="ot"></param>
        //private void UpdateOcctypeValues(IOccupancyTypeEditable ot)
        //{
        //    //todo: what if the tables can't create a function. Do we not allow the user to 
        //    //move to another occtype?
        //    ot.StructureDepthDamageFunction = StructureEditorVM.CreateFunctionFromTables();
        //    ot.ContentDepthDamageFunction = ContentEditorVM.CreateFunctionFromTables();
        //    ot.VehicleDepthDamageFunction = VehicleEditorVM.CreateFunctionFromTables();
        //    ot.OtherDepthDamageFunction = OtherEditorVM.CreateFunctionFromTables();

        //    //the check boxes for if the ot tab is selected gets updated automatically
        //    //there is no need to update it here.
        //    ot.StructureValueUncertainty = StructureValueUncertainty.CreateOrdinate();
        //    ot.ContentValueUncertainty = ContentValueUncertainty.CreateOrdinate();
        //    ot.VehicleValueUncertainty = VehicleValueUncertainty.CreateOrdinate();
        //    ot.OtherValueUncertainty = OtherValueUncertainty.CreateOrdinate();


        //}

        //private void SetValuesForNewlySelectedOcctype()
        //{
        //    string xLabel = "xlabel";
        //    string yLabel = "ylabel";
        //    string chartTitle = "chartTitle";
        //    if (SelectedOccType != null)
        //    {
        //        StructureEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.StructureDepthDamageFunction, xLabel, yLabel, chartTitle);
        //        ContentEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.ContentDepthDamageFunction, xLabel, yLabel, chartTitle);
        //        VehicleEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.VehicleDepthDamageFunction, xLabel, yLabel, chartTitle);
        //        OtherEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.OtherDepthDamageFunction, xLabel, yLabel, chartTitle);

        //        StructureValueUncertainty.ValueUncertainty = SelectedOccType.StructureValueUncertainty;
        //        ContentValueUncertainty.ValueUncertainty = SelectedOccType.ContentValueUncertainty;
        //        VehicleValueUncertainty.ValueUncertainty = SelectedOccType.VehicleValueUncertainty;
        //        OtherValueUncertainty.ValueUncertainty = SelectedOccType.OtherValueUncertainty;

        //        //set the new value uncertainties 
        //        //StructureValueUncertainty = new ValueUncertaintyVM(SelectedOccType.StructureValueUncertainty.Type);
        //        //ContentValueUncertainty = new ValueUncertaintyVM(SelectedOccType.ContentValueUncertainty.Type);
        //        //VehicleValueUncertainty = new ValueUncertaintyVM(SelectedOccType.VehicleValueUncertainty.Type);
        //        //OtherValueUncertainty = new ValueUncertaintyVM(SelectedOccType.OtherValueUncertainty.Type);


        //        // StructureEditorVM.UpdateChartViewModel();
        //        //StructureDepthDamageCurve = null;
        //        //ContentDepthDamageCurve = null;
        //        //VehicleDepthDamageCurve = null;
        //        //OtherDepthDamageCurve = null;
        //        //return;
        //    }

        //    //StructureDepthDamageCurve = _SelectedOccType.StructureDepthDamageFunction;
        //    //ContentDepthDamageCurve = _SelectedOccType.ContentDepthDamageFunction;
        //    //VehicleDepthDamageCurve = _SelectedOccType.VehicleDepthDamageFunction;
        //    //OtherDepthDamageCurve = _SelectedOccType.OtherDepthDamageFunction;




        //    //if ( DepthDamageCurveDictionary.ContainsKey(_SelectedOccType.StructureDepthDamageName) )
        //    //{
        //    //    StructureDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.StructureDepthDamageName].Curve; 
        //    //}
        //    //else
        //    //{
        //    //    StructureDepthDamageCurve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        //    //}

        //    //if (DepthDamageCurveDictionary.ContainsKey(SelectedOccType.ContentDepthDamageName))
        //    //{
        //    //    ContentDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.ContentDepthDamageName].Curve;
        //    //}
        //    //if (DepthDamageCurveDictionary.ContainsKey(SelectedOccType.VehicleDepthDamageName))
        //    //{
        //    //    VehicleDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.VehicleDepthDamageName].Curve;
        //    //}
        //    //if (DepthDamageCurveDictionary.ContainsKey(SelectedOccType.OtherDepthDamageName))
        //    //{
        //    //    OtherDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.OtherDepthDamageName].Curve;
        //    //}



        //}

        //private void LoadTheEditorVMs()
        //{
        //    string xLabel = "xlabel";
        //    string yLabel = "ylabel";
        //    string chartTitle = "chartTitle";
        //    if (SelectedOccType != null)
        //    {
        //        StructureEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.StructureDepthDamageFunction, xLabel, yLabel, chartTitle);
        //        ContentEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.ContentDepthDamageFunction, xLabel, yLabel, chartTitle);
        //        VehicleEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.VehicleDepthDamageFunction, xLabel, yLabel, chartTitle);
        //        OtherEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.OtherDepthDamageFunction, xLabel, yLabel, chartTitle);
        //    }
        //    else
        //    {
        //        ICoordinatesFunction defaultFunc = ICoordinatesFunctionsFactory.DefaultOccTypeFunction();

        //        StructureEditorVM = new CoordinatesFunctionEditorVM(defaultFunc, xLabel, yLabel, chartTitle);
        //        ContentEditorVM = new CoordinatesFunctionEditorVM(defaultFunc, xLabel, yLabel, chartTitle);
        //        VehicleEditorVM = new CoordinatesFunctionEditorVM(defaultFunc, xLabel, yLabel, chartTitle);
        //        OtherEditorVM = new CoordinatesFunctionEditorVM(defaultFunc, xLabel, yLabel, chartTitle);
        //    }

        //}

        private void OccTypeElementAdded(object sender, Saving.ElementAddedEventArgs e)
        {
            //List<OccupancyTypesElement> tempList = new List<OccupancyTypesElement>();
            //foreach(OccupancyTypesElement elem in OccTypeGroups)
            //{
            //    tempList.Add(elem);
            //}
            // //tempList =   OccTypeGroups;
            //tempList.Add((OccupancyTypesElement)e.Element);
            //OccTypeGroups = tempList;//this is to hit the notify prop changed
        }

        private void OccTypeElementRemoved(object sender, Saving.ElementAddedEventArgs e)
        {
            //OccupancyTypesElement elementToRemove = (OccupancyTypesElement)e.Element;
            //List<OccupancyTypesElement> tempList = new List<OccupancyTypesElement>();
            //foreach(OccupancyTypesElement elem in OccTypeGroups)
            //{
            //    tempList.Add(elem);
            //}
            //tempList.Remove(elementToRemove);
            ////we know that the one we are removing is the selected group, so we need to switch to a different one
            //int indexInList = OccTypeGroups.IndexOf(elementToRemove);
            //if (OccTypeGroups.Count == 1)//then its about to be zero
            //{
            //    //clear everything

            //}
            //else if (indexInList > 0)//display the one before it
            //{
            //    SelectedOccTypeGroup = OccTypeGroups[indexInList -1];
            //}
            //else//they are deleting the first group
            //{
            //    SelectedOccTypeGroup = OccTypeGroups[1];
            //}
            
            ////if(OccTypeGroups.IndexOf(elementToRemove) == 0)
            //OccTypeGroups = tempList;
        }
        private void UpdateTheIsSelectedBoolOnEachOccTypeGroup()
        {
            //foreach(IOccupancyTypeGroupEditable elem in OccTypeGroups)
            //{
            //    if(elem == SelectedOccTypeGroup)
            //    {
            //        elem.IsSelected = true;
            //    }
            //    else
            //    {
            //        elem.IsSelected = false;
            //    }
            //}
        }
        private void AddEmptyCurvesToEmptyDepthDamages()
        {
            List<double> xs = new List<double>() { 0 };
            List<double> ys = new List<double>() { 0};
            ICoordinatesFunction newCurve = ICoordinatesFunctionsFactory.Factory(xs,ys);
            //newCurve.Add(0, new Statistics.None(0));

            foreach (IOccupancyTypeGroupEditable element in OccTypeGroups)
            {
                foreach( IOccupancyTypeEditable ot in element.Occtypes)
                {
                    if (ot.OccType.StructureDepthDamageFunction.Coordinates.Count == 0)
                    {
                        ot.OccType.StructureDepthDamageFunction = newCurve;
                    }
                    if (ot.OccType.ContentDepthDamageFunction.Coordinates.Count == 0)
                    {
                        ot.OccType.ContentDepthDamageFunction = newCurve;
                    }
                    if (ot.OccType.VehicleDepthDamageFunction.Coordinates.Count == 0)
                    {
                        ot.OccType.VehicleDepthDamageFunction = newCurve;
                    }
                    if (ot.OccType.OtherDepthDamageFunction.Coordinates.Count == 0)
                    {
                        ot.OccType.OtherDepthDamageFunction = newCurve;
                    }
                }
            }
        }
        private DataTable CreateDataTable(OccupancyTypesElement ote)
        {
            //define all the columns
            DataTable dt = new System.Data.DataTable(ote.Name);
            DataColumn dc = new DataColumn("Name", typeof(string));
            dt.Columns.Add(dc);
            dc = new DataColumn("Description", typeof(string));
            dt.Columns.Add(dc);
            dc = new DataColumn("VarInFoundHtType", typeof(string));
            dt.Columns.Add(dc);


            //assign each row
            foreach(IOccupancyType ot in ote.ListOfOccupancyTypes)
            {
                DataRow dr = dt.NewRow();
                dr[0] = ot.Name;
                dr[1] = ot.Description;
                dr[2] = "None";
                dt.Rows.Add(dr);

            }



            return dt;
        }


        //public void LoadTheIsTabsCheckedDictionary()
        //{
        //    //_OcctypeTabsSelectedDictionary.Clear();
        //    //foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in _SelectedOccTypeGroup.ListOfOccupancyTypes)
        //    //{
        //    //    bool[] tabsCheckedArray = new bool[] { true, true, true, false };
        //    //    _OcctypeTabsSelectedDictionary.Add(ot.Name, tabsCheckedArray);
                
        //    //}

        //}
        /// <summary>
        /// This gets called when someone changes an occtypes name. This method will delete the old key and value and will replace it with the new key (occtype name).
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        //public void UpdateKeyInTabsDictionary(string oldName, string newName)
        //{
        //   if( _OcctypeTabsSelectedDictionary.ContainsKey(oldName) == true)
        //    {
        //        bool[] value = _OcctypeTabsSelectedDictionary[oldName];
        //        //delete old one
        //        _OcctypeTabsSelectedDictionary.Remove(oldName);
        //        _OcctypeTabsSelectedDictionary.Add(newName, value);
        //    }
        //}

        //private void SetTheCheckboxesOnTheTabs()
        //{
        //    NotifyPropertyChanged("IsStructureTabChecked");
        //    NotifyPropertyChanged("IsContentTabChecked");
        //    NotifyPropertyChanged("IsVehicleTabChecked");
        //    NotifyPropertyChanged("IsOtherTabChecked");
        //}

        public void LaunchNewDamCatWindow()
        {
            //if(_SelectedOccType == null) { return; }
            //CreateNewDamCatVM vm = new CreateNewDamCatVM(DamageCategoriesList);
            //string header = "New Damage Category";
            //DynamicTabVM tab = new DynamicTabVM(header, vm, "NewDamageCategory");
            //Navigate(tab, true, true);
            //if(vm.WasCanceled == false)
            //{
            //    if(vm.HasError == false)
            //    {
            //        //store the new damage category
            //        _SelectedOccType.DamageCategory = DamageCategoryFactory.Factory(vm.Name);

            //        SetDamageCategory();
            //        LoadDamageCategoriesList();
                    

            //    }
            //}
        }
        private List<string> GetAllOccTypeNames()
        {
            List<string> occtypeNames = new List<string>();
            if (SelectedOccTypeGroup == null) { return occtypeNames; }
            foreach (IOccupancyTypeEditable ot in SelectedOccTypeGroup.Occtypes)
            {
                occtypeNames.Add(ot.OccType.Name);
            }
            return occtypeNames;
        }
        public void LaunchNewOccTypeWindow()
        {

            CreateNewDamCatVM vm = new CreateNewDamCatVM(GetAllOccTypeNames());
            string header = "New Occupancy Type";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "NewOccupancyType");
            Navigate(tab, true, true);
            if (vm.WasCanceled == false)
            {
                if (vm.HasError == false)
                {
                    //just use the damcat name of whatever occtype is selected.
                    string damCatName = SelectedOccType.OccType.DamageCategory.Name;
                    //create the new occupancy type
                    IOccupancyType newOT = OccupancyTypeFactory.Factory(vm.Name, damCatName, SelectedOccTypeGroup.ID);
                    OccupancyTypeEditable otEditable = new OccupancyTypeEditable(newOT);

                    //add the occtype to the list and select it
                    SelectedOccTypeGroup.Occtypes.Add(otEditable);
                    SelectedOccType = otEditable;

                    //add the occtype to the list of newly added occtypes so that it will save
                    _NewlyCreatedOcctypes.Add(otEditable);


                }
            }
        }

        public void LaunchCopyOccTypeWindow()
        {
            if (_SelectedOccType == null) { return; }
            CreateNewDamCatVM vm = new CreateNewDamCatVM(SelectedOccType.OccType.Name + "_Copy", GetAllOccTypeNames());
            string header = "Copy Occupancy Type";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CopyOccupancyType");
            Navigate(tab, true, true);
            if (vm.WasCanceled == false)
            {
                if (vm.HasError == false)
                {
                    //create the new occupancy type
                    IOccupancyType newOT = Saving.PersistenceFactory.GetOccTypeManager().CloneOccType(SelectedOccType.OccType);
                    newOT.Name = vm.Name;
                    newOT.ID = Saving.PersistenceFactory.GetOccTypeManager().GetIdForNewOccType(newOT.GroupID);

                    OccupancyTypeEditable otEditable = new OccupancyTypeEditable(newOT);

                    SelectedOccTypeGroup.Occtypes.Add(otEditable);
                    SelectedOccType = otEditable;

                    //add the occtype to the list of newly added occtypes so that it will save
                    _NewlyCreatedOcctypes.Add(otEditable);



                }
            }
        }

        public void DeleteOccType()
        {
            if(SelectedOccType == null)
            {
                return;
            }

            int selectedIndex = SelectedOccTypeGroup.Occtypes.IndexOf(SelectedOccType);
            SelectedOccTypeGroup.Occtypes.Remove(SelectedOccType);
            _OcctypesToDelete.Add(SelectedOccType);
            //set the selected occtype to be the one before, unless at 0
            if (selectedIndex > 0)
            {
                SelectedOccType = SelectedOccTypeGroup.Occtypes[selectedIndex - 1];
            }
            else //we just deleted the zeroth item
            {
                if (SelectedOccTypeGroup.Occtypes.Count > 0)
                {
                    SelectedOccType = SelectedOccTypeGroup.Occtypes[0];
                }
                else //there are no more occtypes
                {
                    //todo: this seems dangerous. Should we instead do something else. Create a new default occtype?
                    SelectedOccType = null;
                }
            }

        }

        public void DeleteOccTypeGroup()
        {
            if(SelectedOccTypeGroup == null)
            {
                return;
            }

            int selectedIndex = OccTypeGroups.IndexOf(SelectedOccTypeGroup);
            OccTypeGroups.Remove(SelectedOccTypeGroup);
            _GroupsToDelete.Add(SelectedOccTypeGroup);

            //set the selected occtype to be the one before, unless at 0
            if (selectedIndex > 0)
            {
                SelectedOccTypeGroup = OccTypeGroups[selectedIndex - 1];
            }
            else //we just deleted the zeroth item
            {
                if (OccTypeGroups.Count > 0)
                {
                    SelectedOccTypeGroup = OccTypeGroups[0];
                }
                else //there are no more occtypes
                {
                    //todo: this seems dangerous. Should we instead do something else. Create a new default occtype?
                    SelectedOccTypeGroup = null;
                }
            }
        }

        /// <summary>
        /// Updates the list of groups from the study cache after new occtypes have been imported
        /// </summary>
        public void AddGroup(IOccupancyTypeGroupEditable group)
        {
            //because the importing of groups gets done on a background thread, i can't
            //update this list on this thread.
            //OccupancyTypesElement newElement = (OccupancyTypesElement)e.Element;
            //int groupID = PersistenceFactory.GetOccTypeManager().GetGroupId(newElement.Name);
            //List<IOccupancyTypeEditable> editableOcctypes = new List<IOccupancyTypeEditable>();
            //foreach (IOccupancyType ot in newElement.ListOfOccupancyTypes)
            //{
            //    editableOcctypes.Add(new OccupancyTypeEditable(ot));
            //}
            //IOccupancyTypeGroupEditable newGroup = new OccupancyTypeGroupEditable(groupID, newElement.Name, editableOcctypes);
            //OccTypeGroups.Add(newGroup);
            OccTypeGroups.Add(group);
        }

        public void LaunchImportNewOccTypeGroup()
        {
            //get the parent from the studycache and launch the importer
            OccupancyTypesOwnerElement owner = StudyCache.GetParentElementOfType<OccupancyTypesOwnerElement>();
            owner.ImportFromFile(this, new EventArgs());
            //i need to update the group list in here. The owner will import like normal and then
            //check to see if the occtype editor (this) is open. If it is then it will update the list
            //in here.
        }


        public void LaunchRenameOcctypeGroup()
        {
            CreateNewDamCatVM vm = new CreateNewDamCatVM(SelectedOccTypeGroup.Name, DamageCategoriesList);
            string header = "Rename Occupancy Type Group";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "RenameOccupancyTypeGroup");
            Navigate(tab, true, true);
            if (vm.WasCanceled == false)
            {
                if (vm.HasError == false)
                {
                    string newName = vm.Name;
                    SelectedOccTypeGroup.Name = newName;

                    if (!_GroupsToUpdateInParentTable.Contains(SelectedOccTypeGroup))
                    {
                        _GroupsToUpdateInParentTable.Add(SelectedOccTypeGroup);
                    }
                }
            }
        }





        //private void SetBindingsToNull()
        //{
        //     SelectedStructureDepthDamage = null;
        //    SelectedContentDepthDamage = null;
        //   SelectedVehicleDepthDamage = null;
        //   SelectedOtherDepthDamage = null;

        //    //SelectedDamageCategory = null;

        //    SelectedOccType = null;         

        //}

        //private void SetDamageCategory()
        //{
        //    //if (_SelectedOccType != null)
        //    //{
        //    //    SelectedDamageCategory = _SelectedOccType.DamageCategory.Name;
        //    //}

        //}
        //private void LoadDamageCategoriesList()
        //{
        //    if (_SelectedOccTypeGroup == null) 
        //    { 
        //        return; 
        //    }

        //    List<string> uniqueDamCats = new List<string>();
        //    foreach (IOccupancyTypeEditable ot in _SelectedOccTypeGroup.Occtypes)
        //    {
        //        if (!uniqueDamCats.Contains(ot.OccType.DamageCategory.Name))
        //        {               
        //            uniqueDamCats.Add(ot.OccType.DamageCategory.Name);
        //        }
        //    }
        //    DamageCategoriesList = uniqueDamCats;
        //}
        //private void LoadDepthDamageCurveNames()
        //{
        //    List<string> structureDDnames = new List<string>();
        //    List<string> contentDDnames = new List<string>();
        //    List<string> vehicleDDnames = new List<string>();
        //    List<string> otherDDnames = new List<string>();


        //    foreach (string key in _DepthDamageCurveDictionary.Keys.ToList())
        //    {
        //        switch(_DepthDamageCurveDictionary[key].DamageType)
        //        {
        //            case DepthDamage.DepthDamageCurve.DamageTypeEnum.Structural:
        //                {
        //                    structureDDnames.Add(key);
        //                    break;
        //                }
        //            case DepthDamage.DepthDamageCurve.DamageTypeEnum.Content:
        //                {
        //                    contentDDnames.Add(key);
        //                    break;
        //                }
        //            case DepthDamage.DepthDamageCurve.DamageTypeEnum.Vehicle:
        //                {
        //                    vehicleDDnames.Add(key);
        //                    break;
        //                }
        //            case DepthDamage.DepthDamageCurve.DamageTypeEnum.Other:
        //                {
        //                    otherDDnames.Add(key);
        //                    break;
        //                }
        //        }
        //    }

        //    StructureDepthDamageStringNames = structureDDnames;
        //    ContentDepthDamageStringNames = contentDDnames;
        //    VehicleDepthDamageStringNames = vehicleDDnames;
        //    OtherDepthDamageStringNames = otherDDnames;

        //}


        //public void LaunchDepthDamageEditor()
        //{
        //    DepthDamage.DepthDamageCurveEditorVM vm = new DepthDamage.DepthDamageCurveEditorVM();
        //    string header = "Depth Damage Curve Editor";
        //    DynamicTabVM tab = new DynamicTabVM(header, vm, "DepthDamageCurveEditor");
        //    Navigate(tab, true, true);
        //    if (vm.WasCanceled == false)
        //    {
        //        if (vm.HasError == false)
        //        {
        //            //this is called if the user clicks "OK" button on the form

        //            //store the new dd curves in the dd curves dictionary
        //            Dictionary<string, DepthDamage. DepthDamageCurve> tempDictionary = new Dictionary<string, DepthDamage.DepthDamageCurve>();

        //            foreach (DepthDamage.DepthDamageCurveEditorControlVM row in vm.ListOfDepthDamageVMs)
        //            {
        //                DepthDamage.DepthDamageCurve tempCurve = new DepthDamage.DepthDamageCurve(row.Name, row.Description, row.Curve, row.DamageType);
        //                tempDictionary.Add(row.Name, tempCurve);
        //            }
        //            //update the newly edited curves at the source
        //            DepthDamage.DepthDamageCurveData.CurveDictionary = tempDictionary;
        //            //bring the curves back into my own dictionary
        //            DepthDamageCurveDictionary = DepthDamage.DepthDamageCurveData.CurveDictionary;
        //            //in case new dd curves were added or removed, update the list of options
        //            LoadDepthDamageCurveNames();
        //        }
        //    }
        //}

        public override void Save()
        {
            //foreach (OccupancyTypesElement ote in ListOfOccupancyTypesGroups)
            //for (int i = 0; i <OccupancyTypesOwnerElement.ListOfOccupancyTypesGroups.Count; i++)
            //{
            //    //foreach (OccupancyTypesElement ote in vm.OccTypeGroups)
            //    for (int j = 0; j < OccTypeGroups.Count; j++)
            //    {
            //        if (OccupancyTypesOwnerElement.ListOfOccupancyTypesGroups[i].Name == OccTypeGroups[j].Name)
            //        {
            //            OccupancyTypesOwnerElement.ListOfOccupancyTypesGroups[i] = OccTypeGroups[j];
            //        }
            //    }
            //}
            //now save the changes
            //CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "", "Loading...");

            //SaveFilesOnBackgroundThread(this, new DoWorkEventArgs(ListOfOccupancyTypesGroups));



            //because i am lumping all the elements together in one editor, then it is difficult to keep track of old names vs new names vs adding new ones etc.
            //i think it is best to just delete all previous tables (all rows in parent table and all individual tables) and then resave everything.
            //List<ChildElement> tmp = OccTypeGroups.ToList<ChildElement>();
            //Saving.PersistenceFactory.GetOccTypeManager().SaveExisting(tmp);   //SaveNew(ListOfOccupancyTypes, OccTypesSelectedTabsDictionary, Name);

            //foreach (OccupancyTypesElement elem in OccupancyTypesOwnerElement.ListOfOccupancyTypesGroups)
            //{
            //    elem.Save();
            //}


            //there may have been newly created occtypes. Those need to be saved as well.
            //todo: these occtypes might get caught in the save modified occtypes. Will that fail?
            //List<IOccupancyTypeEditable> occtypesToSave = new List<IOccupancyTypeEditable>();
            //foreach(IOccupancyTypeEditable ot in occtypesToSave)
            //{

            //}


            //get the groups that have changed their names
            //foreach (IOccupancyTypeGroupEditable group in OccTypeGroups)
            //{
            //    if (group.IsModified)
            //    {
            //        _GroupsToUpdateInParentTable.Add(group);
            //    }
            //}

            bool areValidCoordinatesFunction = AssignCoordinatesFunctions();
            if(!areValidCoordinatesFunction)
            {
                return;
            }


            //get the occtypes that have been modified
            List<IOccupancyTypeEditable> occtypesToUpdateInOcctypesTable = new List<IOccupancyTypeEditable>();

            foreach (IOccupancyTypeGroupEditable group in OccTypeGroups)
            {
                foreach (IOccupancyTypeEditable ot in group.Occtypes)
                {
                    if (ot.IsModified)
                    {
                        occtypesToUpdateInOcctypesTable.Add(ot);
                    }
                }
            }

            OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();
            manager.SaveModifiedGroups(_GroupsToUpdateInParentTable);
            manager.SaveModifiedOcctypes(occtypesToUpdateInOcctypesTable);
            CheckForNewAddsBeingDeleted();
            UpdateNewlyCreatedOcctypeIDs();
            manager.SaveNewOcctypes(_NewlyCreatedOcctypes);
            manager.DeleteOcctypes(_OcctypesToDelete);

            //todo: need to delete the occtype groups and the occtypes that were in them
            //need to add new occtype groups? and remove any that were added and deleted.
            foreach (IOccupancyTypeGroupEditable group in _GroupsToDelete)
            {
                manager.DeleteOcctypeGroup(group);
            }

            //we just saved so set all the "isModified" flags back to false
            ClearAllModifiedLists();

            //i need to update the elements in the study cache which is what gets pulled in, the next time the 
            //occupancy types editor is opened.
            //todo: for now i will just update all the groups in the cache. In the future we could get smarter about
            //it and just do the ones that have changed. That probably isn't hard to do with the lists that i already
            //am tracking at the top of this class.
            manager.UpdateStudyCache(OccTypeGroups.ToList());



        }

        //i don't actually assign any of the coordinates functions to the occtypes until now
        private bool AssignCoordinatesFunctions()
        {
            bool success = true;
            StringBuilder errorMsg = new StringBuilder("Occupancy Types could not be saved because of the following errors:");


            foreach (IOccupancyTypeGroupEditable group in OccTypeGroups)
            {
                errorMsg.Append(Environment.NewLine).Append('\t').Append("Occupancy Type Group: ").Append(group.Name);
                foreach (IOccupancyTypeEditable ot in group.Occtypes)
                {
                    errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append("Occupancy Type: ").Append(ot.OccType.Name);
                    //structure depth damage function
                    try
                    {
                        ot.OccType.StructureDepthDamageFunction = ot.StructureEditorVM.CreateFunctionFromTables();
                    }
                    catch (Exception e)
                    {
                        success = false;
                        errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
                            .Append("Structure Depth Damage Function: ").Append(e.Message);
                    }
                    //content depth damage function
                    try
                    {
                        ot.OccType.ContentDepthDamageFunction = ot.ContentEditorVM.CreateFunctionFromTables();
                    }
                    catch (Exception e)
                    {
                        success = false;
                        errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
                                .Append("Content Depth Damage Function: ").Append(e.Message);
                    }

                    //vehicle depth damage function
                    try
                    {
                        ot.OccType.VehicleDepthDamageFunction = ot.VehicleEditorVM.CreateFunctionFromTables();
                    }
                    catch (Exception e)
                    {
                        success = false;
                        errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
                                .Append("Vehicle Depth Damage Function: ").Append(e.Message);
                    }

                    //other depth damage function
                    try
                    {
                        ot.OccType.OtherDepthDamageFunction = ot.OtherEditorVM.CreateFunctionFromTables();
                    }
                    catch (Exception e)
                    {
                        success = false;
                        errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
                                .Append("Vehicle Depth Damage Function: ").Append(e.Message);
                    }

                }
            }

            if (!success)
            {
                CustomMessageBoxVM msgBoxVM = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, errorMsg.ToString());
                string header = "Occupancy Type Errors";
                DynamicTabVM tab = new DynamicTabVM(header, msgBoxVM, "Occupancy_Type_Errors");
                Navigate(tab, true, true);
            }
            return success;
        }

        private void CheckForNewAddsBeingDeleted()
        {
            //if an occtype is in the newly added list and in the delete list
            //then just remove it from both lists.
            List<IOccupancyTypeEditable> occtypesToRemoveFromBothLists = new List<IOccupancyTypeEditable>();

            foreach(IOccupancyTypeEditable ot in _NewlyCreatedOcctypes)
            {
                if (_OcctypesToDelete.Contains(ot))
                {
                    //this occtype is in both lists
                    occtypesToRemoveFromBothLists.Add(ot);
                }
            }

            //remove them from the lists
            foreach(IOccupancyTypeEditable ot in occtypesToRemoveFromBothLists)
            {
                _NewlyCreatedOcctypes.Remove(ot);
                _OcctypesToDelete.Remove(ot);
            }

        }

        private void UpdateNewlyCreatedOcctypeIDs()
        {
            //we are about to save the newly created occtypes. These occtypes are given the max occtype id + 1 
            //from the database. If there are more than one occtype in this list, they will all have the same id
            //because none of them are in the database yet.
            if (_NewlyCreatedOcctypes.Count == 0)
            {
                return;
            }
            int startingId = _NewlyCreatedOcctypes[0].OccType.ID;
            for(int i = 0;i<_NewlyCreatedOcctypes.Count;i++)
            {
                _NewlyCreatedOcctypes[i].OccType.ID = startingId + i;
            }
        }

        /// <summary>
        /// Sets all the "IsModified" flags back to false and clears all the newly created lists.
        /// </summary>
        private void ClearAllModifiedLists()
        {
            _NewlyCreatedOcctypes.Clear();
            _OcctypesToDelete.Clear();
            foreach (IOccupancyTypeGroupEditable group in OccTypeGroups)
            {
                group.IsModified = false;
                foreach (IOccupancyTypeEditable ot in group.Occtypes)
                {
                    ot.OccType.IsModified = false;
                }
            }
        }
        #endregion
        #region Functions

        #endregion



    }
}
