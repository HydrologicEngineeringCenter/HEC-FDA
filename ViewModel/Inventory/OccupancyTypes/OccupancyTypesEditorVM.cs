using FdaLogging;
using HEC.Plotting.SciChart2D.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using ViewModel.Editors;
using ViewModel.Saving;
using ViewModel.Saving.PersistenceManagers;
using ViewModel.Utilities;

namespace ViewModel.Inventory.OccupancyTypes
{
    //[Author(q0heccdm, 7 / 14 / 2017 1:55:50 PM)]
    public class OccupancyTypesEditorVM : BaseEditorVM
    {
        public event EventHandler CloseEditor;
        //private List<IOccupancyTypeEditable> _NewlyCreatedOcctypes = new List<IOccupancyTypeEditable>();
        //private List<IOccupancyTypeEditable> _OcctypesToDelete = new List<IOccupancyTypeEditable>();
        private List<IOccupancyTypeGroupEditable> _GroupsToUpdateInParentTable = new List<IOccupancyTypeGroupEditable>();
        private List<IOccupancyTypeGroupEditable> _NewGroupsAdded = new List<IOccupancyTypeGroupEditable>();

        //private List<IOccupancyTypeGroupEditable> _GroupsToDelete = new List<IOccupancyTypeGroupEditable>();

        private IOccupancyTypeEditable _SelectedOccType;
        private ObservableCollection<IOccupancyTypeGroupEditable> _OccTypeGroups;

        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/14/2017 1:55:50 PM
        #endregion
        #region Fields
        private IOccupancyTypeGroupEditable _SelectedOccTypeGroup;
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
        //private CoordinatesFunctionEditorVM _StructureEditorVM;
        //private CoordinatesFunctionEditorVM _ContentEditorVM;
        //private CoordinatesFunctionEditorVM _VehicleEditorVM;
        //private CoordinatesFunctionEditorVM _OtherEditorVM;


        #endregion
        #region Properties
        public ObservableCollection<IOccupancyTypeGroupEditable> OccTypeGroups
        {
            get { return _OccTypeGroups; }
            set { _OccTypeGroups = value; NotifyPropertyChanged(); }
        }
        public IOccupancyTypeEditable SelectedOccType
        {
            get { return _SelectedOccType; }
            set
            {
                _SelectedOccType = value;
                if (_SelectedOccType == null)
                {
                    return;
                }
                NotifyPropertyChanged();
            }
        }
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
                //UpdateTheIsSelectedBoolOnEachOccTypeGroup(); 
                NotifyPropertyChanged(); 
            }
        }
       

       

        #endregion
        #region Constructors
        public OccupancyTypesEditorVM(EditorActionManager manager) : base(manager)
        {
            Name = "OccTypeEditor";//I just needed some name so that it doesn't fail the empty name test that is now universal.
            Chart = new Chart2D();
            OccTypeGroups = new ObservableCollection<IOccupancyTypeGroupEditable>();
            SetDimensions(950, 600, 400, 400);

            //todo: do i need this call?
            //AddEmptyCurvesToEmptyDepthDamages();


            //StudyCache.OccTypeElementAdded += UpdateGroupList;
        }

        public void FillEditor(List<OccupancyTypesElement> occtypeElements)
        {
            if (occtypeElements.Count == 0)
            {
                CreateDefaultOccTypeGroup();
            }
            else
            {
                ObservableCollection<IOccupancyTypeGroupEditable> editableGroups = CreateEditableOcctypeGroups(occtypeElements);
                OccTypeGroups = editableGroups;
                //set the selected occtype group
                SelectedOccTypeGroup = OccTypeGroups.First();
                //set all the "isModified" flags to false.
                LoadDamageCategoriesList();
            }
            SelectedOccType = SelectedOccTypeGroup.Occtypes.FirstOrDefault();
            ClearAllModifiedLists();
        }

        #endregion
        #region Voids

        #region create editable occtype objects
        //group id to list of dam cat names
        private Dictionary<int, ObservableCollection<string>> _GroupsToDamcats = new Dictionary<int, ObservableCollection<string>>();

        private ObservableCollection<IOccupancyTypeGroupEditable> CreateEditableOcctypeGroups(List<OccupancyTypesElement> occtypeElements)
        {
            ObservableCollection<IOccupancyTypeGroupEditable> editableGroups = new ObservableCollection<IOccupancyTypeGroupEditable>();
            foreach (OccupancyTypesElement group in occtypeElements)
            {
                editableGroups.Add(CreateEditableGroup(group));
            }
            return editableGroups;
        }

        private IOccupancyTypeGroupEditable CreateEditableGroup(OccupancyTypesElement group)
        {
            ObservableCollection<string> uniqueDamCatsForGroup = GetDamageCategoriesList(group);
            _GroupsToDamcats.Add(group.ID, uniqueDamCatsForGroup);
            List<IOccupancyTypeEditable> editableOcctypes = new List<IOccupancyTypeEditable>();

            foreach (IOccupancyType ot in group.ListOfOccupancyTypes)
            {
                OccupancyTypeEditable otEdit = new OccupancyTypeEditable(ot, ref uniqueDamCatsForGroup);
                otEdit.RequestNavigation += this.Navigate;
                editableOcctypes.Add(otEdit);
            }

            //now we have a list of all the occtypes. They get cloned in the OccupancyTypeEditable ctor.
            int groupID = PersistenceFactory.GetOccTypeManager().GetGroupId(group.Name);
            IOccupancyTypeGroupEditable occTypeGroup = new OccupancyTypeGroupEditable(groupID, group.Name, editableOcctypes);
            if (occTypeGroup.Occtypes.Count == 0)
            {
                occTypeGroup.Occtypes.Add(CreateDefaultOcctype(group.ID));
            }
            return occTypeGroup;
        }
        private ObservableCollection<string> GetDamageCategoriesList(OccupancyTypesElement group)
        {
            ObservableCollection<string> uniqueDamCats = new ObservableCollection<string>();
            foreach (IOccupancyType ot in group.ListOfOccupancyTypes)
            {
                if (!uniqueDamCats.Contains(ot.DamageCategory.Name))
                {
                    uniqueDamCats.Add(ot.DamageCategory.Name);
                }
            }
            return uniqueDamCats;
        }

        #endregion

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
        //private void AddEmptyCurvesToEmptyDepthDamages()
        //{
        //    List<double> xs = new List<double>() { 0 };
        //    List<double> ys = new List<double>() { 0};
        //    ICoordinatesFunction newCurve = ICoordinatesFunctionsFactory.Factory(xs,ys);
        //    //newCurve.Add(0, new Statistics.None(0));

        //    //foreach (IOccupancyTypeGroupEditable element in OccTypeGroups)
        //    //{
        //    //    foreach( IOccupancyTypeEditable ot in element.Occtypes)
        //    //    {
        //    //        if (ot.OccType.StructureDepthDamageFunction.Coordinates.Count == 0)
        //    //        {
        //    //            ot.OccType.StructureDepthDamageFunction = newCurve;
        //    //        }
        //    //        if (ot.OccType.ContentDepthDamageFunction.Coordinates.Count == 0)
        //    //        {
        //    //            ot.OccType.ContentDepthDamageFunction = newCurve;
        //    //        }
        //    //        if (ot.OccType.VehicleDepthDamageFunction.Coordinates.Count == 0)
        //    //        {
        //    //            ot.OccType.VehicleDepthDamageFunction = newCurve;
        //    //        }
        //    //        if (ot.OccType.OtherDepthDamageFunction.Coordinates.Count == 0)
        //    //        {
        //    //            ot.OccType.OtherDepthDamageFunction = newCurve;
        //    //        }
        //    //    }
        //    //}

        //    //todo: i just commented this out 4/9/20. is it necessary?
        //}
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

        //public void LaunchNewDamCatWindow()
        //{
        //    if (_SelectedOccType == null) { return; }
        //    CreateNewDamCatVM vm = new CreateNewDamCatVM(DamageCategoriesList);
        //    string header = "New Damage Category";
        //    DynamicTabVM tab = new DynamicTabVM(header, vm, "NewDamageCategory");
        //    Navigate(tab, true, true);
        //    if (vm.WasCanceled == false)
        //    {
        //        if (vm.HasError == false)
        //        {
        //            //store the new damage category
        //            _SelectedOccType.DamageCategory = DamageCategoryFactory.Factory(vm.Name);

        //            SetDamageCategory();
        //            LoadDamageCategoriesList();


        //        }
        //    }
        //}
        private List<string> GetAllOccTypeNames()
        {
            List<string> occtypeNames = new List<string>();
            if (SelectedOccTypeGroup == null) { return occtypeNames; }
            foreach (IOccupancyTypeEditable ot in SelectedOccTypeGroup.Occtypes)
            {
                occtypeNames.Add(ot.Name);
            }
            return occtypeNames;
        }
        public void LaunchNewOccTypeWindow()
        {
                    //we want a new occtype. If there is an occtype and occtype group then we can use
                    //some of that data. We don't want to just copy the other occtype, however, that is
                    //another option. It seems to me that if there is no occtype group, a user shouldn't be
                    //allowed to create an occtype. The user is able to delete all groups and/or all occtypes.

            if(SelectedOccTypeGroup == null)
            {
                MessageBox.Show("An occupancy type group must first be imported in order to create a new occupancy type.", "No Occupancy Type Group", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            CreateNewDamCatVM vm = new CreateNewDamCatVM(GetAllOccTypeNames());
            string header = "New Occupancy Type";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "NewOccupancyType");
            Navigate(tab, true, true);
            if (vm.WasCanceled == false)
            {
                if (vm.HasError == false)
                {
                    ObservableCollection<string> damCatOptions = new ObservableCollection<string>();
                    string damCatName = "";
                    if(SelectedOccType != null)
                    {
                        damCatName = SelectedOccType.DamageCategory.Name;
                        damCatOptions = SelectedOccType.DamageCategoriesList;
                    }
                    
                    //create the new occupancy type
                    IOccupancyType newOT = OccupancyTypeFactory.Factory(vm.Name, damCatName, SelectedOccTypeGroup.ID);
                    OccupancyTypeEditable otEditable = new OccupancyTypeEditable(newOT,ref damCatOptions, false);
                    otEditable.RequestNavigation += this.Navigate;

                    //add the occtype to the list and select it
                    SelectedOccTypeGroup.Occtypes.Add(otEditable);
                    SelectedOccType = otEditable;
                    otEditable.IsModified = true;
                    //add the occtype to the list of newly added occtypes so that it will save
                    //_NewlyCreatedOcctypes.Add(otEditable);


                }
            }
        }

        public void LaunchCopyOccTypeWindow()
        {
            if (SelectedOccType == null) { return; }

            List<LogItem> errors = new List<LogItem>();
            IOccupancyType newOT = SelectedOccType.CreateOccupancyType(out errors);
            if(newOT == null)
            {
                StringBuilder sb = new StringBuilder().AppendLine("A copy of the selected occupancy type cannot be completed until the following errors are fixed:");
                foreach(LogItem li in errors)
                {
                    sb.AppendLine(li.Message);
                }
                MessageBox.Show(sb.ToString(), "Occupancy Type is in Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            CreateNewDamCatVM vm = new CreateNewDamCatVM(SelectedOccType.Name + "_Copy", GetAllOccTypeNames());
            string header = "Copy Occupancy Type";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CopyOccupancyType");
            Navigate(tab, true, true);
            if (vm.WasCanceled == false)
            {
                if (vm.HasError == false)
                {
                    //create the new occupancy type
                     // Saving.PersistenceFactory.GetOccTypeManager().CloneOccType(SelectedOccType.OccType);
                    
                    
                    newOT.Name = vm.Name;
                    ObservableCollection<string> damcats = _GroupsToDamcats[newOT.GroupID];
                    OccupancyTypeEditable otEditable = new OccupancyTypeEditable(newOT, ref damcats, false);
                    otEditable.RequestNavigation += this.Navigate;

                    SelectedOccTypeGroup.Occtypes.Add(otEditable);
                    SelectedOccType = otEditable;





                }
            }
        }

        public void DeleteOccType()
        {
            if(SelectedOccType == null || SelectedOccTypeGroup == null || SelectedOccTypeGroup.Occtypes == null)
            {
                return;
            }

            //pop up a message box that asks the user if they want to permanently delete the occtype
            if (MessageBox.Show("Do you want to permanently delete this occupancy type?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                //then permanently delete it.

                OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();
                manager.DeleteOcctype(SelectedOccType);

                int selectedIndex = SelectedOccTypeGroup.Occtypes.IndexOf(SelectedOccType);
                SelectedOccTypeGroup.Occtypes.Remove(SelectedOccType);
               // _OcctypesToDelete.Add(SelectedOccType);
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
                        if (MessageBox.Show("This is the last occupancy type in this group. Do you want to permanently delete the group?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            //delete the group
                            DeleteOccTypeGroup();
                        }
                        else
                        {
                            //create a new default occtype and add it to the group
                            IOccupancyTypeEditable otEditable = CreateDefaultOcctype(SelectedOccTypeGroup.ID);

                            //add the occtype to the list and select it
                            SelectedOccTypeGroup.Occtypes.Add(otEditable);
                            SelectedOccType = otEditable;
                            otEditable.IsModified = true;
                        }
                        //todo: this seems dangerous. Should we instead do something else. Create a new default occtype?
                        //SelectedOccType = null;
                    }
                }

            }
            else
            {
                return;
            }


        }

        private IOccupancyTypeEditable CreateDefaultOcctype(int groupID)
        {
            IOccupancyType newOT = OccupancyTypeFactory.Factory("New Occupancy Type", "", groupID);
            ObservableCollection<string> damCatOptions = new ObservableCollection<string>();
            OccupancyTypeEditable otEditable = new OccupancyTypeEditable(newOT, ref damCatOptions, false);
            otEditable.RequestNavigation += this.Navigate;
            return otEditable;
        }

        public void DeleteOccTypeGroup()
        {
            if(SelectedOccTypeGroup == null)
            {
                return;
            }

            int selectedIndex = OccTypeGroups.IndexOf(SelectedOccTypeGroup);
            if(_GroupsToDamcats.ContainsKey(SelectedOccTypeGroup.ID))
            {
                _GroupsToDamcats.Remove(SelectedOccTypeGroup.ID);
            }
            OccTypeGroups.Remove(SelectedOccTypeGroup);
            //_GroupsToDelete.Add(SelectedOccTypeGroup);

            OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();
            manager.DeleteOcctypeGroup(SelectedOccTypeGroup.ID);
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
                else //there are no more occtype groups
                {
                    //if no, then create a new default group? If yes, then exit?
                    if (MessageBox.Show("There are no more occtype groups. Do you want to exit the editor.", "Confirm",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        CloseEditor?.Invoke(this, new EventArgs());
                    }
                    else
                    {
                        //create a default occtype group?
                        CreateDefaultOccTypeGroup();
                        //IOccupancyTypeGroupEditable group = CreateEditableGroup(element);
                        //OccTypeGroups.Add(newGroup);
                        //SelectedOccTypeGroup = newGroup;
                    }
                    //todo: this seems dangerous. Should we instead do something else. Create a new default occtype?
                    //SelectedOccTypeGroup = null;
                }
            }
        }

        /// <summary>
        /// This creates and then saves a new occtype element. When this element gets saved,
        /// the study cache gets updated which then tells the owner element a new element has been added.
        /// The owner element will see if this editor is open and will add it in "AddGroup()".
        /// </summary>
        public void CreateDefaultOccTypeGroup()
        {
            OccTypePersistenceManager manager = PersistenceFactory.GetOccTypeManager();
            int groupID = manager.GetUnusedId();
            string groupName = "Occupancy Type Group";
            OccupancyTypesElement elem = new OccupancyTypesElement(groupName, groupID, new List<IOccupancyType>());
            //calling the save here should add it to the cache, which tells the occtype owner to add it to this editor
            //if it is open. see AddGroup() in this class.
            manager.SaveNewElement(elem);
        }

        //private IOccupancyTypeGroupEditable CreateDefaultEditableGroup()
        //{
        //    //i think it is just safer to save the occtype group right away. If i try to wait until the user saves
        //    //it gets complicated. The occtype needs an occtype group number and so if the user tries to save the 
        //    //occtype, then the occtype won't belong to any group yet. 
        //    OccTypePersistenceManager manager = PersistenceFactory.GetOccTypeManager();
        //    int groupID = manager.GetUnusedId();
        //    List<IOccupancyTypeEditable> occtypes = new List<IOccupancyTypeEditable>() { CreateDefaultOcctype(groupID) };
        //    string groupName = "Occupancy Type Group";
        //    IOccupancyTypeGroupEditable newGroup = new OccupancyTypeGroupEditable(groupID, groupName , occtypes, false);
        //    //todo: i think this will save the default occtype which we don't want to do. Maybe just pass in an empty list.
        //    OccupancyTypesElement elem = new OccupancyTypesElement(groupName, groupID, new List<IOccupancyType>());
        //    manager.SaveNewElement(elem);
        //    return newGroup;
        //}

        /// <summary>
        /// Updates the list of groups from the study cache after new occtypes have been imported
        /// </summary>
        public void AddGroup(OccupancyTypesElement element)
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

            IOccupancyTypeGroupEditable group = CreateEditableGroup(element);
            if(group.Occtypes.Count == 0)
            {
                group.Occtypes.Add(CreateDefaultOcctype(group.ID));
            }
            OccTypeGroups.Add(group);
            SelectedOccTypeGroup = group;
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
        private void LoadDamageCategoriesList()
        {
            if (_SelectedOccTypeGroup == null)
            {
                return;
            }

            List<string> uniqueDamCats = new List<string>();
            foreach (IOccupancyTypeEditable ot in _SelectedOccTypeGroup.Occtypes)
            {
                if (!uniqueDamCats.Contains(ot.DamageCategory.Name))
                {
                    uniqueDamCats.Add(ot.DamageCategory.Name);
                }
            }
            DamageCategoriesList = uniqueDamCats;
        }
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

        /// <summary>
        /// Saves all modified occupancy types from all of the occupancy type groups.
        /// </summary>
        public void SaveAll()
        {
            List<string> originalGroupNames = new List<string>();
            List<string> newGroupNames = new List<string>();

            foreach (IOccupancyTypeGroupEditable group in _GroupsToUpdateInParentTable)
            {
                originalGroupNames.Add(group.OriginalName);
                newGroupNames.Add(group.Name);
            }
            OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();
            manager.SaveModifiedGroups(_GroupsToUpdateInParentTable);

            //just make a master list of all the occtypes that have been modified
            //then call the save on each one of them.
            //List<IOccupancyTypeEditable> modifiedOcctypes = new List<IOccupancyTypeEditable>();
            List<SaveAllReportGroupVM> groupReports = new List<SaveAllReportGroupVM>();
            foreach(IOccupancyTypeGroupEditable group in OccTypeGroups)
            {
                groupReports.Add( group.SaveAll());
                //modifiedOcctypes.AddRange(group.ModifiedOcctypes);
            }

            //we have all the group messages lumped into one list,
            //now sort them out
            List<SaveAllReportGroupVM> goodGroups = new List<SaveAllReportGroupVM>();
            List<SaveAllReportGroupVM> badGroups = new List<SaveAllReportGroupVM>();
            foreach(SaveAllReportGroupVM group in groupReports)
            {
                if(group.SuccessfulList.Count>0)
                {
                    goodGroups.Add(group);
                }
                if(group.UnsuccessfulList.Count>0)
                {
                    badGroups.Add(group);
                }
            }


            SaveAllReportVM report = new SaveAllReportVM(originalGroupNames, newGroupNames, goodGroups, badGroups);
            string header = "Save All Report";
            DynamicTabVM tab = new DynamicTabVM(header, report, "SaveAllReport");
            Navigate(tab, true,true);

            //StringBuilder sb = new StringBuilder().AppendLine("Saved Successfully:");
            //foreach (IOccupancyTypeGroupEditable group in OccTypeGroups)
            //{
            //    sb.Append(group.PrintSuccessfullySavedOcctypes());
            //}
            //sb.AppendLine().AppendLine("Saved Unsuccessfully:");
            //foreach (IOccupancyTypeGroupEditable group in OccTypeGroups)
            //{
            //    sb.Append(group.PrintUnsuccessfullySavedOcctypes());
            //}


            //manager.DeleteOcctypes(_OcctypesToDelete);

            //foreach (IOccupancyTypeGroupEditable group in _GroupsToDelete)
            //{
            //    manager.DeleteOcctypeGroup(group.ID);
            //}

           // MessageBox.Show(sb.ToString(), "Saving Results", MessageBoxButton.OK, MessageBoxImage.Information);

            ////i guess i should have a list of saved successful and a list of unsuccessful
            //foreach(IOccupancyTypeEditable otEditable in modifiedOcctypes)
            //{
            //    otEditable.Save();
            //}

                //if (!IsModified)
                //{
                //    //nothing new to save
                //    String time = DateTime.Now.ToString();

                //    LogItem li = LogItemFactory.FactoryTemp(LoggingLevel.Info, "No new changes to save." + time);
                //    TempErrors.Insert(0, li);
                //    SaveStatusLevel = LoggingLevel.Debug;
                //    //TempErrors = new List<LogItem>() { li };
                //    UpdateMessages();
                //    return;

                //}

                //OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();

                //List<LogItem> errors = new List<LogItem>();
                //IOccupancyType ot = CreateOccupancyType(this, out errors);
                //if (ot == null)
                //{
                //    //if the occtype is null then it failed. There should be errors to add.
                //    TempErrors.AddRange(errors);
                //    UpdateMessages(true);
                //    return;
                //}
                //else if (HasBeenSaved)
                //{
                //    //update the existing occtype
                //    manager.SaveModifiedOcctype(ot);
                //}
                //else if (!HasBeenSaved)
                //{
                //    //save this as a new occtype
                //    manager.SaveNewOccType(ot);
                //}

                //this.IsModified = false;
                //string lastEditDate = DateTime.Now.ToString("G");
                //SavingText = "Last Saved: " + lastEditDate;
                //UpdateMessages(true);
                ////this will disable the save button.
                //HasChanges = false;

                //bool areValidCoordinatesFunction = AssignCoordinatesFunctions();
                //if (!areValidCoordinatesFunction)
                //{
                //    return;
                //}


                ////get the occtypes that have been modified
                //List<IOccupancyTypeEditable> occtypesToUpdateInOcctypesTable = new List<IOccupancyTypeEditable>();

                //foreach (IOccupancyTypeGroupEditable group in OccTypeGroups)
                //{
                //    foreach (IOccupancyTypeEditable ot in group.Occtypes)
                //    {
                //        if (ot.IsModified)
                //        {
                //            occtypesToUpdateInOcctypesTable.Add(ot);
                //        }
                //    }
                //}

                //OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();
                //manager.SaveModifiedGroups(_GroupsToUpdateInParentTable);
                //manager.SaveModifiedOcctypes(occtypesToUpdateInOcctypesTable);
                //CheckForNewAddsBeingDeleted();
                //UpdateNewlyCreatedOcctypeIDs();
                //manager.SaveNewOcctypes(_NewlyCreatedOcctypes);
                //manager.DeleteOcctypes(_OcctypesToDelete);

                ////todo: need to delete the occtype groups and the occtypes that were in them
                ////need to add new occtype groups? and remove any that were added and deleted.
                //foreach (IOccupancyTypeGroupEditable group in _GroupsToDelete)
                //{
                //    manager.DeleteOcctypeGroup(group);
                //}

                ////we just saved so set all the "isModified" flags back to false
                //ClearAllModifiedLists();

                ////i need some way to update the UI so that the '*' goes away

                ////i need to update the elements in the study cache which is what gets pulled in, the next time the 
                ////occupancy types editor is opened.
                ////todo: for now i will just update all the groups in the cache. In the future we could get smarter about
                ////it and just do the ones that have changed. That probably isn't hard to do with the lists that i already
                ////am tracking at the top of this class.
                //manager.UpdateOccTypeGroupsInStudyCache(OccTypeGroups.ToList());
        }

        /// <summary>
        /// This will just save the occupancy type that is selected. It will not save all of the groups or all of the occtypes. To do that see "SaveAll()"
        /// </summary>
        public override void Save()
        {
            //i think this method has to be here for one of the interfaces. 
            //the individual occtype save is handled by the OccupancyTypeEditable.

            //if (!SelectedOccType.IsModified)
            //{
            //    //nothing new to save
            //    String time = DateTime.Now.ToString();

            //    LogItem li = LogItemFactory.FactoryTemp(LoggingLevel.Info, "No new changes to save." + time);
            //    TempErrors.Insert(0, li);
            //    SaveStatusLevel = LoggingLevel.Debug;
            //    //TempErrors = new List<LogItem>() { li };
            //    UpdateMessages();
            //    return;

            //}

            ////is the curve in a valid state to save
            ////String isOTValid = AssignCoordinateFunction(SelectedOccType);

            //////todo get this message to go to the messages control
            ////if(isOTValid != null)
            ////{
            ////    MessageBox.Show(isOTValid, "Could not save occupancy type", MessageBoxButton.OK);
            ////    return;
            ////}

            ////bool areValidCoordinatesFunction = AssignCoordinatesFunctions();
            ////if(!areValidCoordinatesFunction)
            ////{
            ////    return;
            ////}

            ////get the occtypes that have been modified
            ////List<IOccupancyTypeEditable> occtypesToUpdateInOcctypesTable = new List<IOccupancyTypeEditable>();


            ////    //foreach (IOccupancyTypeEditable ot in SelectedOccTypeGroup.Occtypes)
            ////    {
            ////        if (SelectedOccType.IsModified)
            ////        {
            ////            occtypesToUpdateInOcctypesTable.Add(SelectedOccType);                        
            ////        }
            ////    }


            //OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();

            //List<LogItem> errors = new List<LogItem>();
            //IOccupancyType ot = CreateOccupancyType(SelectedOccType, out errors);
            //if (ot == null)
            //{
            //    //if the occtype is null then it failed. There should be errors to add.
            //    TempErrors.AddRange(errors);
            //    UpdateMessages(true);
            //    return;
            //}
            //else
            //{
            //    manager.SaveModifiedOcctype(ot);

            //}




            ////we just saved so set all the "isModified" flags back to false
            //ClearAllModifiedLists();

            ////i need some way to update the UI so that the '*' goes away

            ////i need to update the elements in the study cache which is what gets pulled in, the next time the 
            ////occupancy types editor is opened.
            ////todo: for now i will just update all the groups in the cache. In the future we could get smarter about
            ////it and just do the ones that have changed. That probably isn't hard to do with the lists that i already
            ////am tracking at the top of this class.

            ////todo: i need to tell this group in the cache that an ot has been updated. or just replace it in the cache.
            ////in theory i just need to update this one occtype, but to update the in memory object that the parent is holding, i need to
            ////remove the whole group and add the whole group. At least the way it is written right now.
            ////manager.UpdateOccTypeInStudyCache(SelectedOccTypeGroup, SelectedOccType);
            //// manager.UpdateOccTypeGroupsInStudyCache(new List<IOccupancyTypeGroupEditable>() { ot });



        }

        //private IOccupancyType CreateOccupancyType(IOccupancyTypeEditable occtypeEditable, out List<LogItem> errors)
        //{
        //    bool success = true;
        //    StringBuilder errorMsg = new StringBuilder("Occupancy Type: ").Append(occtypeEditable.Name);
        //    List<LogItem> constructionErrors = new List<LogItem>();

        //    OccupancyType ot = new OccupancyType();
        //    ot.Name = occtypeEditable.Name;
        //    ot.GroupID = occtypeEditable.GroupID;
        //    ot.ID = occtypeEditable.ID;
        //    ot.Description = occtypeEditable.Description;
        //    ot.DamageCategory = occtypeEditable.DamageCategory;
            
        //    ot.CalculateStructureDamage = occtypeEditable.CalculateStructureDamage;
        //    ot.CalculateContentDamage = occtypeEditable.CalculateContentDamage;
        //    ot.CalculateVehicleDamage = occtypeEditable.CalculateVehicleDamage;
        //    ot.CalculateOtherDamage = occtypeEditable.CalculateOtherDamage;

        //    try
        //    {
        //        ot.StructureDepthDamageFunction = occtypeEditable.StructureEditorVM.CreateFunctionFromTables();
        //    }
        //    catch (Exception e)
        //    {
        //        success = false;
        //        errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
        //            .Append("Structure Depth Damage Function: ").Append(e.Message);

        //        string logMessage = "Error constructing structure depth damage function: " + e.Message;
        //        constructionErrors.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, logMessage));

        //    }

        //    try
        //    {
        //        ot.ContentDepthDamageFunction = occtypeEditable.ContentEditorVM.CreateFunctionFromTables();
        //    }
        //    catch (Exception e)
        //    {
        //        success = false;
        //        errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
        //                .Append("Content Depth Damage Function: ").Append(e.Message);

        //        string logMessage = "Error constructing content depth damage function: " + e.Message;
        //        constructionErrors.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, logMessage));
        //    }

        //    try
        //    {
        //        ot.VehicleDepthDamageFunction = occtypeEditable.VehicleEditorVM.CreateFunctionFromTables();
        //    }
        //    catch (Exception e)
        //    {
        //        success = false;
        //        errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
        //                .Append("Vehicle Depth Damage Function: ").Append(e.Message);

        //        string logMessage = "Error constructing vehicle depth damage function: " + e.Message;
        //        constructionErrors.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, logMessage));
        //    }

        //    try
        //    {
        //        ot.OtherDepthDamageFunction = occtypeEditable.OtherEditorVM.CreateFunctionFromTables();
        //    }
        //    catch (Exception e)
        //    {
        //        success = false;
        //        errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
        //                .Append("Vehicle Depth Damage Function: ").Append(e.Message);

        //        string logMessage = "Error constructing other depth damage function: " + e.Message;
        //        constructionErrors.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, logMessage));
        //    }

        //    ot.StructureValueUncertainty = occtypeEditable.StructureValueUncertainty.CreateOrdinate();
        //    ot.ContentValueUncertainty = occtypeEditable.ContentValueUncertainty.CreateOrdinate();
        //    ot.VehicleValueUncertainty = occtypeEditable.VehicleValueUncertainty.CreateOrdinate();
        //    ot.OtherValueUncertainty = occtypeEditable.OtherValueUncertainty.CreateOrdinate();

        //    ot.FoundationHeightUncertainty = occtypeEditable.FoundationHeightUncertainty.CreateOrdinate();

        //    //ot.StructureUncertaintyType = occtypeEditable.asdf;
        //    //ot.ContentUncertaintyType = occtypeEditable.adsf;
        //    //ot.VehicleValueUncertainty = occtypeEditable.asdf;
        //    //ot.OtherValueUncertainty = occtypeEditable.asdf;

        //    //ot.FoundationHtUncertaintyType = occtypeEditable.asdf;
        //    if (success)
        //    {
        //        errors = null;
        //        return ot;
        //    }
        //    else
        //    {
        //        //errors = errorMsg.ToString();
        //        errors = constructionErrors;
        //        return null;
        //    }

        //}

        //private String AssignCoordinateFunction(IOccupancyTypeEditable ot)
        //{
        //    //basically this now just needs to try to make a new occtype.
        //    //the two references come from the save and saveall. i think i need to return the new occtype, and make the 
        //    //error messages an "out". I should probably do some validator like john does but maybe not yet.

        //    bool success = true;
        //    StringBuilder errorMsg = new StringBuilder("Occupancy Type: ").Append(ot.OccType.Name);
        //    //errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append("Occupancy Type: ").Append(ot.OccType.Name);
        //    //structure depth damage function
        //    try
        //    {
        //        ot.OccType.StructureDepthDamageFunction = ot.StructureEditorVM.CreateFunctionFromTables();
        //    }
        //    catch (Exception e)
        //    {
        //        success = false;
        //        errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
        //            .Append("Structure Depth Damage Function: ").Append(e.Message);
        //    }
        //    //content depth damage function
        //    try
        //    {
        //        ot.OccType.ContentDepthDamageFunction = ot.ContentEditorVM.CreateFunctionFromTables();
        //    }
        //    catch (Exception e)
        //    {
        //        success = false;
        //        errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
        //                .Append("Content Depth Damage Function: ").Append(e.Message);
        //    }

        //    //vehicle depth damage function
        //    try
        //    {
        //        ot.OccType.VehicleDepthDamageFunction = ot.VehicleEditorVM.CreateFunctionFromTables();
        //    }
        //    catch (Exception e)
        //    {
        //        success = false;
        //        errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
        //                .Append("Vehicle Depth Damage Function: ").Append(e.Message);
        //    }

        //    //other depth damage function
        //    try
        //    {
        //        ot.OccType.OtherDepthDamageFunction = ot.OtherEditorVM.CreateFunctionFromTables();
        //    }
        //    catch (Exception e)
        //    {
        //        success = false;
        //        errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
        //                .Append("Vehicle Depth Damage Function: ").Append(e.Message);
        //    }

        //    if (success)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return errorMsg.ToString();
        //    }

        //}

        //i don't actually assign any of the coordinates functions to the occtypes until now
        //private bool AssignCoordinatesFunctions()
        //{
        //    bool success = true;
        //    StringBuilder errorMsg = new StringBuilder("Occupancy Types could not be saved because of the following errors:");


        //    foreach (IOccupancyTypeGroupEditable group in OccTypeGroups)
        //    {
        //        errorMsg.Append(Environment.NewLine).Append('\t').Append("Occupancy Type Group: ").Append(group.Name);
        //        foreach (IOccupancyTypeEditable ot in group.Occtypes)
        //        {
        //            String error = AssignCoordinateFunction(ot);
        //            if(error != null)
        //            {
        //                errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append(error);
        //            }

        //        }
        //    }

        //    if (!success)
        //    {
        //        CustomMessageBoxVM msgBoxVM = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, errorMsg.ToString());
        //        string header = "Occupancy Type Errors";
        //        DynamicTabVM tab = new DynamicTabVM(header, msgBoxVM, "Occupancy_Type_Errors");
        //        Navigate(tab, true, true);
        //    }
        //    return success;
        //}

        //private void CheckForNewAddsBeingDeleted()
        //{
        //    //if an occtype is in the newly added list and in the delete list
        //    //then just remove it from both lists.
        //    List<IOccupancyTypeEditable> occtypesToRemoveFromBothLists = new List<IOccupancyTypeEditable>();

        //    foreach(IOccupancyTypeEditable ot in _NewlyCreatedOcctypes)
        //    {
        //        if (_OcctypesToDelete.Contains(ot))
        //        {
        //            //this occtype is in both lists
        //            occtypesToRemoveFromBothLists.Add(ot);
        //        }
        //    }

        //    //remove them from the lists
        //    foreach(IOccupancyTypeEditable ot in occtypesToRemoveFromBothLists)
        //    {
        //        _NewlyCreatedOcctypes.Remove(ot);
        //        _OcctypesToDelete.Remove(ot);
        //    }

        //}

        //private void UpdateNewlyCreatedOcctypeIDs()
        //{
        //    //we are about to save the newly created occtypes. These occtypes are given the max occtype id + 1 
        //    //from the database. If there are more than one occtype in this list, they will all have the same id
        //    //because none of them are in the database yet.
        //    if (_NewlyCreatedOcctypes.Count == 0)
        //    {
        //        return;
        //    }
        //    int startingId = _NewlyCreatedOcctypes[0].ID;
        //    for(int i = 0;i<_NewlyCreatedOcctypes.Count;i++)
        //    {
        //        _NewlyCreatedOcctypes[i].ID = startingId + i;
        //    }
        //}

        /// <summary>
        /// Sets all the "IsModified" flags back to false and clears all the newly created lists.
        /// </summary>
        private void ClearAllModifiedLists()
        {
            //_NewlyCreatedOcctypes.Clear();
            //_OcctypesToDelete.Clear();
            foreach (IOccupancyTypeGroupEditable group in OccTypeGroups)
            {
                group.IsModified = false;
                foreach (IOccupancyTypeEditable ot in group.Occtypes)
                {
                    if (ot.HasBeenSaved)
                    {
                        ot.IsModified = false;
                    }
                }
            }
        }

        //public void UpdateMessages(bool saving)
        //{
        //    throw new NotImplementedException();
        //}

        //public void FilterRowsByLevel(LoggingLevel level)
        //{
        //    throw new NotImplementedException();
        //}

        //public void DisplayAllMessages()
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
        #region Functions

        public override bool IsOkToClose()
        {
            bool areUnsavedChanges = false;
            //also need to check that the group names have been saved
            if(_GroupsToUpdateInParentTable.Count>0)
            {
                areUnsavedChanges = true;
            }
            foreach (IOccupancyTypeGroupEditable group in OccTypeGroups)
            {
                if(group.ModifiedOcctypes.Count>0)
                {
                    //there are occtypes that have not been saved
                    areUnsavedChanges = true;
                    break;
                }
               
            }

            if (areUnsavedChanges)
            {
                if (MessageBox.Show("You have unsaved changes. Are you sure you want to close?", "Are You Sure", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        #endregion


    }
}
