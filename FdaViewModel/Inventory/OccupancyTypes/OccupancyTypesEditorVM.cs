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

        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/14/2017 1:55:50 PM
        #endregion
        #region Fields
        private List<OccupancyTypesElement> _OccTypeGroups;
        private OccupancyTypesElement _SelectedOccTypeGroup;
        private IOccupancyType _SelectedOccType;
        private Dictionary<string,DepthDamage.DepthDamageCurve> _DepthDamageCurveDictionary;
        //this dictionary is to keep track of the checkboxes that have been clicked in the tabs for each occtype
        //private Dictionary<string, bool[]> _OcctypeTabsSelectedDictionary;
        //private string _SelectedStructureDepthDamage;

        private ICoordinatesFunction _StructureDepthDamageCurve;// = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);// Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        private ICoordinatesFunction _ContentDepthDamageCurve;// = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        private ICoordinatesFunction _VehicleDepthDamageCurve;// = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        private ICoordinatesFunction _OtherDepthDamageCurve;// = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

        private List<string> _StructureDepthDamageStringNames = new List<string>();
        private List<string> _ContentDepthDamageStringNames = new List<string>();
        private List<string> _VehicleDepthDamageStringNames = new List<string>();
        private List<string> _OtherDepthDamageStringNames = new List<string>();

        private List<string> _DamageCategoriesList = new List<string>();
        private string _Description;

        private string _Year;
        private string _Module;
        private CoordinatesFunctionEditorVM _StructureEditorVM;
        private CoordinatesFunctionEditorVM _ContentEditorVM;
        private CoordinatesFunctionEditorVM _VehicleEditorVM;
        private CoordinatesFunctionEditorVM _OtherEditorVM;


        #endregion
        #region Properties

        public ValueUncertaintyVM StructureValueUncertainty
        {
            get;
            set;
        }
        public ValueUncertaintyVM ContentValueUncertainty
        {
            get;
            set;
        }
        public ValueUncertaintyVM VehicleValueUncertainty
        {
            get;
            set;
        }
        public ValueUncertaintyVM OtherValueUncertainty
        {
            get;
            set;
        }
        public ValueUncertaintyVM FoundationHtUncertainty
        {
            get;
            set;
        }

        //public string SelectedOccTypeName
        //{
        //    get { if (SelectedOccType == null) { return ""; } return SelectedOccType.Name; }
        //   // set { UpdateKeyInTabsDictionary(_SelectedOccType.Name, value); SelectedOccType.Name = value; NotifyPropertyChanged(); }
        //}

        //public bool IsStructureTabChecked
        //{
        //    get
        //    {
        //        if(SelectedOccType == null) { return false; }
        //        if(_OcctypeTabsSelectedDictionary.ContainsKey(_SelectedOccType.Name))
        //        {
        //            return (bool)_OcctypeTabsSelectedDictionary[_SelectedOccType.Name].GetValue(0);
        //        }
        //        else
        //        {
        //            return true; // it should only hit this when the form is first open because the dictionary hasn't been loaded with values yet.
        //        }
                
        //    }
        //    set
        //    {
        //        if (SelectedOccType == null) { return; }
        //        _OcctypeTabsSelectedDictionary[_SelectedOccType.Name].SetValue(value,0); 
        //    }
        //}

        //public bool IsContentTabChecked
        //{
        //    get
        //    {
        //        if (SelectedOccType == null) { return false; }
        //        if (_OcctypeTabsSelectedDictionary.ContainsKey(_SelectedOccType.Name))
        //        {
        //            return (bool)_OcctypeTabsSelectedDictionary[_SelectedOccType.Name].GetValue(1);
        //        }
        //        else
        //        {
        //            return true; // it should only hit this when the form is first open because the dictionary hasn't been loaded with values yet.
        //        }
        //    }
        //    set
        //    {
        //        if (SelectedOccType == null) { return; }
        //        _OcctypeTabsSelectedDictionary[_SelectedOccType.Name].SetValue(value, 1);
        //    }
        //}
        //public bool IsVehicleTabChecked
        //{
        //    get
        //    {
        //        if (SelectedOccType == null) 
        //        { 
        //            return false; 
        //        }
        //        if (_OcctypeTabsSelectedDictionary.ContainsKey(_SelectedOccType.Name))
        //        {
        //            return (bool)_OcctypeTabsSelectedDictionary[_SelectedOccType.Name].GetValue(2);
        //        }
        //        else
        //        {
        //            return true; // it should only hit this when the form is first open because the dictionary hasn't been loaded with values yet.
        //        }
        //    }
        //    set
        //    {
        //        if (SelectedOccType == null) { return; }
        //        _OcctypeTabsSelectedDictionary[_SelectedOccType.Name].SetValue(value, 2);
        //    }
        //}
        //public bool IsOtherTabChecked
        //{
        //    get
        //    {
        //        if (SelectedOccType == null) { return false; }
        //        if (_OcctypeTabsSelectedDictionary.ContainsKey(_SelectedOccType.Name))
        //        {
        //            return (bool)_OcctypeTabsSelectedDictionary[_SelectedOccType.Name].GetValue(3);
        //        }
        //        else
        //        {
        //            return true; // it should only hit this when the form is first open because the dictionary hasn't been loaded with values yet.
        //        }
        //    }
        //    set
        //    {
        //        if (SelectedOccType == null) { return; }
        //        _OcctypeTabsSelectedDictionary[_SelectedOccType.Name].SetValue(value, 3);
        //    }
        //}
        //public new string Description
        //{
        //    get
        //    {
        //        if (SelectedOccType != null)
        //        {
        //            return SelectedOccType.Description;
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    set { if (value == null) { return; } SelectedOccType.Description = value; }
        //}
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
        public string SelectedDamageCategory
        {
            get { if (_SelectedOccType == null) { return ""; } return _SelectedOccType.DamageCategory.Name; }
            set { if (_SelectedOccType != null && value != null) { _SelectedOccType.DamageCategory.Name = value; } NotifyPropertyChanged(); }
        }

        public IOccupancyType SelectedOccType
        {
            get { return _SelectedOccType; }
            set
            {
                if (_SelectedOccType != null)
                {
                    //set the values on the occtype that we are leaving
                    UpdateOcctypeValues(_SelectedOccType);
                }
                _SelectedOccType = value;
                //set the new curves
                SetValuesForNewlySelectedOcctype();
                SetDamageCategory();
                SetTheCheckboxesOnTheTabs();
                NotifyPropertyChanged("SelectedOccTypeName");
                NotifyPropertyChanged();
            }
        }

        #region structures
        //public List<string> StructureDepthDamageStringNames
        //{
        //    get { return _StructureDepthDamageStringNames; }
        //    set { _StructureDepthDamageStringNames = value; NotifyPropertyChanged(); }
        //}
        //public string SelectedStructureDepthDamage
        //{
        //    get {
        //        if (_SelectedOccType == null) { return ""; }
        //        if(StructureDepthDamageStringNames.Contains(_SelectedOccType.StructureDepthDamageName))
        //        {
        //            return _SelectedOccType.StructureDepthDamageName;

        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    set
        //    {
        //        if(value == null) { return; }
        //        _SelectedOccType.StructureDepthDamageName = value;
        //        if (DepthDamageCurveDictionary.ContainsKey(_SelectedOccType.StructureDepthDamageName))
        //        {
        //            StructureDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.StructureDepthDamageName].Curve;
        //        }
        //        NotifyPropertyChanged();
        //    }
        //}
        //public ICoordinatesFunction StructureDepthDamageCurve
        //{
        //    get{return _StructureDepthDamageCurve;}
        //    set 
        //    {
        //        _StructureDepthDamageCurve = value; 
        //        if (_SelectedOccType != null) 
        //        { 
        //            _SelectedOccType.StructureDepthDamageFunction = value; 
        //        } 
        //        NotifyPropertyChanged(); }
        //}

      

        #endregion
        #region Content
        //public List<string> ContentDepthDamageStringNames
        //{
        //    get { return _ContentDepthDamageStringNames; }
        //    set { _ContentDepthDamageStringNames = value; NotifyPropertyChanged(); }
        //}
        //public string SelectedContentDepthDamage
        //{
        //    get
        //    {
        //        if (_SelectedOccType == null) { return ""; }
        //        if (ContentDepthDamageStringNames.Contains(_SelectedOccType.ContentDepthDamageName))
        //        {
        //            return _SelectedOccType.ContentDepthDamageName;

        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    set
        //    {
        //        if (value == null) { return; }
        //        _SelectedOccType.ContentDepthDamageName = value;
        //        if (DepthDamageCurveDictionary.ContainsKey(SelectedOccType.ContentDepthDamageName))
        //        {
        //            ContentDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.ContentDepthDamageName].Curve;
        //        }
        //        NotifyPropertyChanged();
        //    }
        //}
        //public ICoordinatesFunction ContentDepthDamageCurve
        //{
        //    get{ return _ContentDepthDamageCurve; }
        //    set 
        //    { 
        //        _ContentDepthDamageCurve = value; 
        //        if (_SelectedOccType != null) 
        //        { 
        //            _SelectedOccType.ContentDepthDamageFunction = value; 
        //        }
        //        NotifyPropertyChanged(); }
        //}

        #endregion
        #region Vehicle
        //public List<string> VehicleDepthDamageStringNames
        //{
        //    get { return _VehicleDepthDamageStringNames; }
        //    set { _VehicleDepthDamageStringNames = value; NotifyPropertyChanged(); }
        //}
        //public string SelectedVehicleDepthDamage
        //{
        //    get
        //    {
        //        if (_SelectedOccType == null) { return ""; }
        //        if (VehicleDepthDamageStringNames.Contains(_SelectedOccType.VehicleDepthDamageName))
        //        {
        //            return _SelectedOccType.VehicleDepthDamageName;

        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    set
        //    {
        //        if (value == null) { return; }
        //        _SelectedOccType.VehicleDepthDamageName = value;
        //        if (DepthDamageCurveDictionary.ContainsKey(SelectedOccType.VehicleDepthDamageName))
        //        {
        //            VehicleDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.VehicleDepthDamageName].Curve;
        //        }
        //        NotifyPropertyChanged();
        //    }
        //}

        //public ICoordinatesFunction VehicleDepthDamageCurve
        //{
        //    get {return _VehicleDepthDamageCurve;}
        //    set 
        //    {
        //        _VehicleDepthDamageCurve = value; 
        //        if (_SelectedOccType != null) 
        //        {
        //            _SelectedOccType.VehicleDepthDamageFunction = value; 
        //        }
        //        NotifyPropertyChanged(); }
        //}

        #endregion
        #region Other
        //public List<string> OtherDepthDamageStringNames
        //{
        //    get { return _OtherDepthDamageStringNames; }
        //    set { _OtherDepthDamageStringNames = value; NotifyPropertyChanged(); }
        //}

        //public string SelectedOtherDepthDamage
        //{
        //    get
        //    {
        //        if (_SelectedOccType == null) { return ""; }
        //        if (OtherDepthDamageStringNames.Contains(_SelectedOccType.OtherDepthDamageName))
        //        {
        //            return _SelectedOccType.OtherDepthDamageName;

        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    set
        //    {
        //        if (value == null) { return; }
        //        _SelectedOccType.OtherDepthDamageName = value;
        //        if (DepthDamageCurveDictionary.ContainsKey(SelectedOccType.OtherDepthDamageName))
        //        {
        //            OtherDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.OtherDepthDamageName].Curve;
        //        }
        //        NotifyPropertyChanged();
        //    }
        //}
        //public ICoordinatesFunction OtherDepthDamageCurve
        //{
        //    get{return _OtherDepthDamageCurve;}
        //    set 
        //    { 
        //        _OtherDepthDamageCurve = value; 
        //        if (_SelectedOccType != null) 
        //        { 
        //            _SelectedOccType.OtherDepthDamageFunction = value; 
        //        } 
        //        NotifyPropertyChanged(); }
        //}

        #endregion


        //public Dictionary<string, DepthDamage.DepthDamageCurve> DepthDamageCurveDictionary
        //{
        //    get { return _DepthDamageCurveDictionary; }
        //    set { _DepthDamageCurveDictionary = value; NotifyPropertyChanged(); }
        //}




        public OccupancyTypesElement SelectedOccTypeGroup
        {
            get { return _SelectedOccTypeGroup; }
            set { if (value == null) { return; } _SelectedOccTypeGroup = value; UpdateTheIsSelectedBoolOnEachOccTypeGroup(); LoadDamageCategoriesList();  NotifyPropertyChanged(); }
        }
        public List<OccupancyTypesElement> OccTypeGroups
        {
            get { return _OccTypeGroups; }
            set { _OccTypeGroups = value; NotifyPropertyChanged(); }
        }

        public CoordinatesFunctionEditorVM StructureEditorVM
        {
            get { return _StructureEditorVM; }
            set { _StructureEditorVM = value; NotifyPropertyChanged(); }
        }
        public CoordinatesFunctionEditorVM ContentEditorVM
        {
            get { return _ContentEditorVM; }
            set { _ContentEditorVM = value; NotifyPropertyChanged(); }
        }
        public CoordinatesFunctionEditorVM VehicleEditorVM
        {
            get { return _VehicleEditorVM; }
            set { _VehicleEditorVM = value; NotifyPropertyChanged(); }
        }
        public CoordinatesFunctionEditorVM OtherEditorVM
        {
            get { return _OtherEditorVM; }
            set { _OtherEditorVM = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public OccupancyTypesEditorVM(OccupancyTypesElement selectedOccTypeElement, Editors.EditorActionManager manager):base(manager)
        {

            Name = "OccTypeEditor";//I just needed some name so that it doesn't fail the empty name test that is now universal.
            //this call will load the default DD curves dictionary
            DepthDamage.DepthDamageCurveData ddcd = new DepthDamage.DepthDamageCurveData(); 
            //DepthDamageCurveDictionary = DepthDamage.DepthDamageCurveData.CurveDictionary;
            //LoadDepthDamageCurveNames();
            OccTypeGroups = StudyCache.GetChildElementsOfType<OccupancyTypesElement>();

         
            AddEmptyCurvesToEmptyDepthDamages();
            //this has to be before the selectedOcctype gets set.
            StructureValueUncertainty = new ValueUncertaintyVM(new Constant(0));
            ContentValueUncertainty = new ValueUncertaintyVM(new Constant(0));
            VehicleValueUncertainty = new ValueUncertaintyVM(new Constant(0));
            OtherValueUncertainty = new ValueUncertaintyVM(new Constant(0));

            //set the selected occtype group
            SelectedOccTypeGroup = selectedOccTypeElement;
            SelectedOccType = SelectedOccTypeGroup.ListOfOccupancyTypes.FirstOrDefault();
            StudyCache.OccTypeElementAdded += OccTypeElementAdded;
            StudyCache.OccTypeElementRemoved += OccTypeElementRemoved;

            LoadTheEditorVMs();
        }

        #endregion
        #region Voids

        /// <summary>
        /// This method sets all the values that need to get set on an occtype when switching
        /// to a new occtype or before saving the editor. Some values do not need to get updated
        /// here becuase the binding updates them automatically.
        /// </summary>
        /// <param name="ot"></param>
        private void UpdateOcctypeValues(IOccupancyType ot)
        {
            //todo: what if the tables can't create a function. Do we not allow the user to 
            //move to another occtype?
            ot.StructureDepthDamageFunction = StructureEditorVM.CreateFunctionFromTables();
            ot.ContentDepthDamageFunction = ContentEditorVM.CreateFunctionFromTables();
            ot.VehicleDepthDamageFunction = VehicleEditorVM.CreateFunctionFromTables();
            ot.OtherDepthDamageFunction = OtherEditorVM.CreateFunctionFromTables();

            //the check boxes for if the ot tab is selected gets updated automatically
            //there is no need to update it here.
            ot.StructureValueUncertainty = StructureValueUncertainty.CreateOrdinate();
            ot.ContentValueUncertainty = ContentValueUncertainty.CreateOrdinate();
            ot.VehicleValueUncertainty = VehicleValueUncertainty.CreateOrdinate();
            ot.OtherValueUncertainty = OtherValueUncertainty.CreateOrdinate();


        }

        private void SetValuesForNewlySelectedOcctype()
        {
            string xLabel = "xlabel";
            string yLabel = "ylabel";
            string chartTitle = "chartTitle";
            if (SelectedOccType != null)
            {
                StructureEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.StructureDepthDamageFunction, xLabel, yLabel, chartTitle);
                ContentEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.ContentDepthDamageFunction, xLabel, yLabel, chartTitle);
                VehicleEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.VehicleDepthDamageFunction, xLabel, yLabel, chartTitle);
                OtherEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.OtherDepthDamageFunction, xLabel, yLabel, chartTitle);

                StructureValueUncertainty.ValueUncertainty = SelectedOccType.StructureValueUncertainty;
                ContentValueUncertainty.ValueUncertainty = SelectedOccType.ContentValueUncertainty;
                VehicleValueUncertainty.ValueUncertainty = SelectedOccType.VehicleValueUncertainty;
                OtherValueUncertainty.ValueUncertainty = SelectedOccType.OtherValueUncertainty;

                //set the new value uncertainties 
                //StructureValueUncertainty = new ValueUncertaintyVM(SelectedOccType.StructureValueUncertainty.Type);
                //ContentValueUncertainty = new ValueUncertaintyVM(SelectedOccType.ContentValueUncertainty.Type);
                //VehicleValueUncertainty = new ValueUncertaintyVM(SelectedOccType.VehicleValueUncertainty.Type);
                //OtherValueUncertainty = new ValueUncertaintyVM(SelectedOccType.OtherValueUncertainty.Type);


                // StructureEditorVM.UpdateChartViewModel();
                //StructureDepthDamageCurve = null;
                //ContentDepthDamageCurve = null;
                //VehicleDepthDamageCurve = null;
                //OtherDepthDamageCurve = null;
                //return;
            }

            //StructureDepthDamageCurve = _SelectedOccType.StructureDepthDamageFunction;
            //ContentDepthDamageCurve = _SelectedOccType.ContentDepthDamageFunction;
            //VehicleDepthDamageCurve = _SelectedOccType.VehicleDepthDamageFunction;
            //OtherDepthDamageCurve = _SelectedOccType.OtherDepthDamageFunction;




            //if ( DepthDamageCurveDictionary.ContainsKey(_SelectedOccType.StructureDepthDamageName) )
            //{
            //    StructureDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.StructureDepthDamageName].Curve; 
            //}
            //else
            //{
            //    StructureDepthDamageCurve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            //}

            //if (DepthDamageCurveDictionary.ContainsKey(SelectedOccType.ContentDepthDamageName))
            //{
            //    ContentDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.ContentDepthDamageName].Curve;
            //}
            //if (DepthDamageCurveDictionary.ContainsKey(SelectedOccType.VehicleDepthDamageName))
            //{
            //    VehicleDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.VehicleDepthDamageName].Curve;
            //}
            //if (DepthDamageCurveDictionary.ContainsKey(SelectedOccType.OtherDepthDamageName))
            //{
            //    OtherDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.OtherDepthDamageName].Curve;
            //}



        }

        private void LoadTheEditorVMs()
        {
            string xLabel = "xlabel";
            string yLabel = "ylabel";
            string chartTitle = "chartTitle";
            if (SelectedOccType != null)
            {
                StructureEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.StructureDepthDamageFunction, xLabel, yLabel, chartTitle);
                ContentEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.ContentDepthDamageFunction, xLabel, yLabel, chartTitle);
                VehicleEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.VehicleDepthDamageFunction, xLabel, yLabel, chartTitle);
                OtherEditorVM = new CoordinatesFunctionEditorVM(SelectedOccType.OtherDepthDamageFunction, xLabel, yLabel, chartTitle);
            }
            else
            {
                ICoordinatesFunction defaultFunc = ICoordinatesFunctionsFactory.DefaultOccTypeFunction();

                StructureEditorVM = new CoordinatesFunctionEditorVM(defaultFunc, xLabel, yLabel, chartTitle);
                ContentEditorVM = new CoordinatesFunctionEditorVM(defaultFunc, xLabel, yLabel, chartTitle);
                VehicleEditorVM = new CoordinatesFunctionEditorVM(defaultFunc, xLabel, yLabel, chartTitle);
                OtherEditorVM = new CoordinatesFunctionEditorVM(defaultFunc, xLabel, yLabel, chartTitle);
            }

        }

        private void OccTypeElementAdded(object sender, Saving.ElementAddedEventArgs e)
        {
            List<OccupancyTypesElement> tempList = new List<OccupancyTypesElement>();
            foreach(OccupancyTypesElement elem in OccTypeGroups)
            {
                tempList.Add(elem);
            }
             //tempList =   OccTypeGroups;
            tempList.Add((OccupancyTypesElement)e.Element);
            OccTypeGroups = tempList;//this is to hit the notify prop changed
        }

        private void OccTypeElementRemoved(object sender, Saving.ElementAddedEventArgs e)
        {
            OccupancyTypesElement elementToRemove = (OccupancyTypesElement)e.Element;
            List<OccupancyTypesElement> tempList = new List<OccupancyTypesElement>();
            foreach(OccupancyTypesElement elem in OccTypeGroups)
            {
                tempList.Add(elem);
            }
            tempList.Remove(elementToRemove);
            //we know that the one we are removing is the selected group, so we need to switch to a different one
            int indexInList = OccTypeGroups.IndexOf(elementToRemove);
            if (OccTypeGroups.Count == 1)//then its about to be zero
            {
                //clear everything

            }
            else if (indexInList > 0)//display the one before it
            {
                SelectedOccTypeGroup = OccTypeGroups[indexInList -1];
            }
            else//they are deleting the first group
            {
                SelectedOccTypeGroup = OccTypeGroups[1];
            }
            
            //if(OccTypeGroups.IndexOf(elementToRemove) == 0)
            OccTypeGroups = tempList;
        }
        private void UpdateTheIsSelectedBoolOnEachOccTypeGroup()
        {
            foreach(OccupancyTypesElement elem in OccTypeGroups)
            {
                if(elem == SelectedOccTypeGroup)
                {
                    elem.IsSelected = true;
                }
                else
                {
                    elem.IsSelected = false;
                }
            }
        }
        private void AddEmptyCurvesToEmptyDepthDamages()
        {
            List<double> xs = new List<double>() { 0 };
            List<double> ys = new List<double>() { 0};
            ICoordinatesFunction newCurve = ICoordinatesFunctionsFactory.Factory(xs,ys);
            //newCurve.Add(0, new Statistics.None(0));

            foreach (OccupancyTypesElement element in OccTypeGroups)
            {
                foreach( IOccupancyType ot in element.ListOfOccupancyTypes)
                {
                    if (ot.StructureDepthDamageFunction.Coordinates.Count == 0)
                    {
                        ot.StructureDepthDamageFunction = newCurve;
                    }
                    if (ot.ContentDepthDamageFunction.Coordinates.Count == 0)
                    {
                        ot.ContentDepthDamageFunction = newCurve;
                    }
                    if (ot.VehicleDepthDamageFunction.Coordinates.Count == 0)
                    {
                        ot.VehicleDepthDamageFunction = newCurve;
                    }
                    if (ot.OtherDepthDamageFunction.Coordinates.Count == 0)
                    {
                        ot.OtherDepthDamageFunction = newCurve;
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


        public void LoadTheIsTabsCheckedDictionary()
        {
            //_OcctypeTabsSelectedDictionary.Clear();
            //foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in _SelectedOccTypeGroup.ListOfOccupancyTypes)
            //{
            //    bool[] tabsCheckedArray = new bool[] { true, true, true, false };
            //    _OcctypeTabsSelectedDictionary.Add(ot.Name, tabsCheckedArray);
                
            //}

        }
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

        private void SetTheCheckboxesOnTheTabs()
        {
            NotifyPropertyChanged("IsStructureTabChecked");
            NotifyPropertyChanged("IsContentTabChecked");
            NotifyPropertyChanged("IsVehicleTabChecked");
            NotifyPropertyChanged("IsOtherTabChecked");
        }

        public void LaunchNewDamCatWindow()
        {
            if(_SelectedOccType == null) { return; }
            CreateNewDamCatVM vm = new CreateNewDamCatVM(DamageCategoriesList);
            string header = "New Damage Category";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "NewDamageCategory");
            Navigate(tab, true, true);
            if(vm.WasCanceled == false)
            {
                if(vm.HasError == false)
                {
                    //store the new damage category
                    _SelectedOccType.DamageCategory = DamageCategoryFactory.Factory(vm.Name);

                    SetDamageCategory();
                    LoadDamageCategoriesList();
                    

                }
            }
        }
        private List<string> GetAllOccTypeNames()
        {
            List<string> occtypeNames = new List<string>();
            if(SelectedOccTypeGroup == null) { return occtypeNames; }
            foreach (IOccupancyType ot in SelectedOccTypeGroup.ListOfOccupancyTypes)
            {
                occtypeNames.Add(ot.Name);
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
                    //create the new occupancy type
                    IOccupancyType newOT = OccupancyTypeFactory.Factory(vm.Name, SelectedDamageCategory);

                    //load all the values you can
                    //newOT.StructureDepthDamageName = SelectedStructureDepthDamage;
                    //newOT.ContentDepthDamageName = SelectedContentDepthDamage;
                    //newOT.VehicleDepthDamageName = SelectedVehicleDepthDamage;
                   // newOT.OtherDepthDamageName = SelectedOtherDepthDamage;

                    ICoordinatesFunction newCurve = ICoordinatesFunctionsFactory.DefaultOccTypeFunction();

                    IDistributedOrdinate distOrd = IDistributedOrdinateFactory.FactoryNormal(0, 0);

                    newOT.StructureValueUncertainty = distOrd;
                    newOT.ContentValueUncertainty = distOrd;
                    newOT.VehicleValueUncertainty = distOrd;
                    newOT.OtherValueUncertainty = distOrd;
                    newOT.FoundationHeightUncertainty = distOrd;

                    //newCurve.Add(0, new Statistics.None(0));

                    newOT.StructureDepthDamageFunction = newCurve;
                    newOT.ContentDepthDamageFunction = newCurve;
                    newOT.OtherDepthDamageFunction = newCurve;
                    newOT.VehicleDepthDamageFunction = newCurve;


                    //_OcctypeTabsSelectedDictionary.Add(newOT.Name, new bool[] { true, true, true, false });

                    //add the occtype to the list and select it
                    SelectedOccTypeGroup.ListOfOccupancyTypes.Add(newOT);
                    SelectedOccType = newOT;

                    SetDamageCategory();
                    LoadDamageCategoriesList();

                }
            }
        }

        public void LaunchCopyOccTypeWindow()
        {
            if(_SelectedOccType == null) { return; }
            CreateNewDamCatVM vm = new CreateNewDamCatVM(SelectedOccType.Name + "_Copy", GetAllOccTypeNames());
            string header = "Copy Occupancy Type";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CopyOccupancyType");
            Navigate(tab, true, true);
            if (vm.WasCanceled == false)
            {
                if (vm.HasError == false)
                {
                    //create the new occupancy type
                    IOccupancyType newOT = OccupancyTypeFactory.Factory(vm.Name, SelectedDamageCategory);

                    //load all the values you can
                    //newOT.StructureDepthDamageName = SelectedOccType.StructureDepthDamageFunction;
                   // newOT.ContentDepthDamageName = SelectedContentDepthDamage;
                   // newOT.VehicleDepthDamageName = SelectedVehicleDepthDamage;
                   // newOT.OtherDepthDamageName = SelectedOtherDepthDamage;

                    newOT.StructureValueUncertainty = SelectedOccType.StructureValueUncertainty;
                    newOT.ContentValueUncertainty = SelectedOccType.ContentValueUncertainty;
                    newOT.VehicleValueUncertainty = SelectedOccType.VehicleValueUncertainty;
                    newOT.OtherValueUncertainty = SelectedOccType.OtherValueUncertainty;
                    newOT.FoundationHeightUncertainty = SelectedOccType.FoundationHeightUncertainty;


                    newOT.StructureDepthDamageFunction = SelectedOccType.StructureDepthDamageFunction;
                    newOT.ContentDepthDamageFunction = SelectedOccType.ContentDepthDamageFunction;
                    newOT.OtherDepthDamageFunction = SelectedOccType.OtherDepthDamageFunction;
                    newOT.VehicleDepthDamageFunction = SelectedOccType.VehicleDepthDamageFunction;

                    //bool[] values = _OcctypeTabsSelectedDictionary[SelectedOccType.Name];
                    //_OcctypeTabsSelectedDictionary.Add(newOT.Name, values);


                    //add the occtype to the list and select it
                    SelectedOccTypeGroup.ListOfOccupancyTypes.Add(newOT);
                    SelectedOccType = newOT;

                    SetDamageCategory();
                    LoadDamageCategoriesList();


                }
            }
        }

        public void DeleteOccType()
        {
            int selectedIndex = SelectedOccTypeGroup.ListOfOccupancyTypes.IndexOf(SelectedOccType);
            SelectedOccTypeGroup.ListOfOccupancyTypes.Remove(SelectedOccType);
            //_OcctypeTabsSelectedDictionary.Remove(SelectedOccType.Name);
            //set the selected occtype to be the one before, unless at 0
            if (selectedIndex>0)
            {
                IOccupancyType ot = SelectedOccTypeGroup.ListOfOccupancyTypes[selectedIndex - 1];
                SelectedOccType = ot;
            }
            else //we just deleted the zeroth item
            {
                if (SelectedOccTypeGroup.ListOfOccupancyTypes.Count > 0)
                {
                    SelectedOccType = SelectedOccTypeGroup.ListOfOccupancyTypes[0];
                }
                else //there are no more occtypes
                {
                    SelectedOccType = null;
                    //SetBindingsToNull();
                }
            }

        }

        public void LaunchImportNewOccTypeGroup()
        {
            //get the parent from the studycache and launch the importer
            OccupancyTypesOwnerElement owner = StudyCache.GetParentElementOfType<OccupancyTypesOwnerElement>();
            owner.ImportFromFile(this, new EventArgs());
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
                    OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();
                    ChildElement oldElement = SelectedOccTypeGroup;
                    ChildElement newElement = new OccupancyTypesElement(newName, SelectedOccTypeGroup.ListOfOccupancyTypes, SelectedOccTypeGroup.OccTypesSelectedTabsDictionary);
                    manager.SaveExisting(oldElement, newElement, 0);
                    if (OccTypeGroups.Contains((OccupancyTypesElement)newElement))
                    {
                        SelectedOccTypeGroup = (OccupancyTypesElement)newElement;
                    }
                }
            }
        }

        public void DeleteOccTypeGroup()
        {
            //call the pers manager to delete the occtype element. Make sure to remove from cache
            OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();
            manager.Remove(SelectedOccTypeGroup);
            // removing from the cache should hit an event on the owner to remove it from its list.

            //also add a listener in here to remove the occtype group if it is deleted that will remove from OccTypegroups
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

        private void SetDamageCategory()
        {
            if (_SelectedOccType != null)
            {
                SelectedDamageCategory = _SelectedOccType.DamageCategory.Name;
            }
           
        }
        private void LoadDamageCategoriesList()
        {
            if(_SelectedOccTypeGroup == null) { return; }
            List<string> uniqueDamCats = new List<string>();
            foreach(IOccupancyType ot in _SelectedOccTypeGroup.ListOfOccupancyTypes)
            {
                if(uniqueDamCats.Contains(ot.DamageCategory.Name))
                { }
                else
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
            List<ChildElement> tmp = OccTypeGroups.ToList<ChildElement>();
            Saving.PersistenceFactory.GetOccTypeManager().SaveExisting(tmp);   //SaveNew(ListOfOccupancyTypes, OccTypesSelectedTabsDictionary, Name);

            //foreach (OccupancyTypesElement elem in OccupancyTypesOwnerElement.ListOfOccupancyTypesGroups)
            //{
            //    elem.Save();
            //}
        }
        #endregion
        #region Functions

        #endregion



    }
}
