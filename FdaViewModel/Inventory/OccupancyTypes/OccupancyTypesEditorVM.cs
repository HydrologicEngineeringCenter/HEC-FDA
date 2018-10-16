using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using System.Data;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    //[Author(q0heccdm, 7 / 14 / 2017 1:55:50 PM)]
    public class OccupancyTypesEditorVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/14/2017 1:55:50 PM
        #endregion
        #region Fields
        private List<OccupancyTypesElement> _OccTypeGroups;
        private OccupancyTypesElement _SelectedOccTypeGroup;
        private Consequences_Assist.ComputableObjects.OccupancyType _SelectedOccType;
        private Dictionary<string,DepthDamage.DepthDamageCurve> _DepthDamageCurveDictionary;
        //this dictionary is to keep track of the checkboxes that have been clicked in the tabs for each occtype
        private Dictionary<string, bool[]> _OcctypeTabsSelectedDictionary;
        //private string _SelectedStructureDepthDamage;

        private Statistics.UncertainCurveDataCollection _StructureDepthDamageCurve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);// Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        private Statistics.UncertainCurveDataCollection _ContentDepthDamageCurve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        private Statistics.UncertainCurveDataCollection _VehicleDepthDamageCurve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        private Statistics.UncertainCurveDataCollection _OtherDepthDamageCurve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

        private List<string> _StructureDepthDamageStringNames = new List<string>();
        private List<string> _ContentDepthDamageStringNames = new List<string>();
        private List<string> _VehicleDepthDamageStringNames = new List<string>();
        private List<string> _OtherDepthDamageStringNames = new List<string>();

        private List<string> _DamageCategoriesList = new List<string>();
        private string _Description;

        private string _Year;
        private string _Module;

        

        #endregion
        #region Properties
        public string SelectedOccTypeName
        {
            get { if (SelectedOccType == null) { return ""; } return SelectedOccType.Name; }
           // set { UpdateKeyInTabsDictionary(_SelectedOccType.Name, value); SelectedOccType.Name = value; NotifyPropertyChanged(); }
        }

        public bool IsStructureTabChecked
        {
            get
            {
                if(SelectedOccType == null) { return false; }
                if(_OcctypeTabsSelectedDictionary.ContainsKey(_SelectedOccType.Name))
                {
                    return (bool)_OcctypeTabsSelectedDictionary[_SelectedOccType.Name].GetValue(0);
                }
                else
                {
                    return true; // it should only hit this when the form is first open because the dictionary hasn't been loaded with values yet.
                }
                
            }
            set
            {
                if (SelectedOccType == null) { return; }
                _OcctypeTabsSelectedDictionary[_SelectedOccType.Name].SetValue(value,0); 
            }
        }

        public bool IsContentTabChecked
        {
            get
            {
                if (SelectedOccType == null) { return false; }
                if (_OcctypeTabsSelectedDictionary.ContainsKey(_SelectedOccType.Name))
                {
                    return (bool)_OcctypeTabsSelectedDictionary[_SelectedOccType.Name].GetValue(1);
                }
                else
                {
                    return true; // it should only hit this when the form is first open because the dictionary hasn't been loaded with values yet.
                }
            }
            set
            {
                if (SelectedOccType == null) { return; }
                _OcctypeTabsSelectedDictionary[_SelectedOccType.Name].SetValue(value, 1);
            }
        }
        public bool IsVehicleTabChecked
        {
            get
            {
                if (SelectedOccType == null) { return false; }
                if (_OcctypeTabsSelectedDictionary.ContainsKey(_SelectedOccType.Name))
                {
                    return (bool)_OcctypeTabsSelectedDictionary[_SelectedOccType.Name].GetValue(2);
                }
                else
                {
                    return true; // it should only hit this when the form is first open because the dictionary hasn't been loaded with values yet.
                }
            }
            set
            {
                if (SelectedOccType == null) { return; }
                _OcctypeTabsSelectedDictionary[_SelectedOccType.Name].SetValue(value, 2);
            }
        }
        public bool IsOtherTabChecked
        {
            get
            {
                if (SelectedOccType == null) { return false; }
                if (_OcctypeTabsSelectedDictionary.ContainsKey(_SelectedOccType.Name))
                {
                    return (bool)_OcctypeTabsSelectedDictionary[_SelectedOccType.Name].GetValue(3);
                }
                else
                {
                    return true; // it should only hit this when the form is first open because the dictionary hasn't been loaded with values yet.
                }
            }
            set
            {
                if (SelectedOccType == null) { return; }
                _OcctypeTabsSelectedDictionary[_SelectedOccType.Name].SetValue(value, 3);
            }
        }
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
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public List<string> DamageCategoriesList
        {
            get { return _DamageCategoriesList; }
            set { _DamageCategoriesList = value; NotifyPropertyChanged(); }
        }
        public string SelectedDamageCategory
        {
            get { if (_SelectedOccType == null) { return ""; } return _SelectedOccType.DamageCategoryName; }
            set { if (_SelectedOccType != null) { _SelectedOccType.DamageCategoryName = value; } NotifyPropertyChanged(); }
        }


        #region structures
        public List<string> StructureDepthDamageStringNames
        {
            get { return _StructureDepthDamageStringNames; }
            set { _StructureDepthDamageStringNames = value; NotifyPropertyChanged(); }
        }
        public string SelectedStructureDepthDamage
        {
            get {
                if (_SelectedOccType == null) { return ""; }
                if(StructureDepthDamageStringNames.Contains(_SelectedOccType.StructureDepthDamageName))
                {
                    return _SelectedOccType.StructureDepthDamageName;

                }
                else
                {
                    return "";
                }
            }
            set
            {
                if(value == null) { return; }
                _SelectedOccType.StructureDepthDamageName = value;
                if (DepthDamageCurveDictionary.ContainsKey(_SelectedOccType.StructureDepthDamageName))
                {
                    StructureDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.StructureDepthDamageName].Curve;
                }
                NotifyPropertyChanged();
            }
        }
        public Consequences_Assist.ComputableObjects.OccupancyType SelectedOccType
        {
            get { return _SelectedOccType; }
            set { _SelectedOccType = value; SetDepthDamageCurves(); SetDamageCategory(); SetTheCheckboxesOnTheTabs(); NotifyPropertyChanged("SelectedOccTypeName");  NotifyPropertyChanged(); }//SelectedOccTypeName = _SelectedOccType.Name;
        }
        public Statistics.UncertainCurveDataCollection StructureDepthDamageCurve
        {
            get{return _StructureDepthDamageCurve;}
            set { _StructureDepthDamageCurve = value; if (_SelectedOccType != null) { _SelectedOccType.SetStructurePercentDD = (Statistics.UncertainCurveIncreasing)value; } NotifyPropertyChanged(); }
        }

      

        #endregion
        #region Content
        public List<string> ContentDepthDamageStringNames
        {
            get { return _ContentDepthDamageStringNames; }
            set { _ContentDepthDamageStringNames = value; NotifyPropertyChanged(); }
        }
        public string SelectedContentDepthDamage
        {
            get
            {
                if (_SelectedOccType == null) { return ""; }
                if (ContentDepthDamageStringNames.Contains(_SelectedOccType.ContentDepthDamageName))
                {
                    return _SelectedOccType.ContentDepthDamageName;

                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (value == null) { return; }
                _SelectedOccType.ContentDepthDamageName = value;
                if (DepthDamageCurveDictionary.ContainsKey(SelectedOccType.ContentDepthDamageName))
                {
                    ContentDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.ContentDepthDamageName].Curve;
                }
                NotifyPropertyChanged();
            }
        }
        public Statistics.UncertainCurveDataCollection ContentDepthDamageCurve
        {
            get{ return _ContentDepthDamageCurve; }
            set { _ContentDepthDamageCurve = value; if (_SelectedOccType != null) { _SelectedOccType.SetContentPercentDD = (Statistics.UncertainCurveIncreasing)value; } NotifyPropertyChanged(); }
        }

        #endregion
        #region Vehicle
        public List<string> VehicleDepthDamageStringNames
        {
            get { return _VehicleDepthDamageStringNames; }
            set { _VehicleDepthDamageStringNames = value; NotifyPropertyChanged(); }
        }
        public string SelectedVehicleDepthDamage
        {
            get
            {
                if (_SelectedOccType == null) { return ""; }
                if (VehicleDepthDamageStringNames.Contains(_SelectedOccType.VehicleDepthDamageName))
                {
                    return _SelectedOccType.VehicleDepthDamageName;

                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (value == null) { return; }
                _SelectedOccType.VehicleDepthDamageName = value;
                if (DepthDamageCurveDictionary.ContainsKey(SelectedOccType.VehicleDepthDamageName))
                {
                    VehicleDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.VehicleDepthDamageName].Curve;
                }
                NotifyPropertyChanged();
            }
        }

        public Statistics.UncertainCurveDataCollection VehicleDepthDamageCurve
        {
            get {return _VehicleDepthDamageCurve;}
            set { _VehicleDepthDamageCurve = value; if (_SelectedOccType != null) { _SelectedOccType.SetVehiclePercentDD = (Statistics.UncertainCurveIncreasing)value; } NotifyPropertyChanged(); }
        }

        #endregion
        #region Other
        public List<string> OtherDepthDamageStringNames
        {
            get { return _OtherDepthDamageStringNames; }
            set { _OtherDepthDamageStringNames = value; NotifyPropertyChanged(); }
        }

        public string SelectedOtherDepthDamage
        {
            get
            {
                if (_SelectedOccType == null) { return ""; }
                if (OtherDepthDamageStringNames.Contains(_SelectedOccType.OtherDepthDamageName))
                {
                    return _SelectedOccType.OtherDepthDamageName;

                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (value == null) { return; }
                _SelectedOccType.OtherDepthDamageName = value;
                if (DepthDamageCurveDictionary.ContainsKey(SelectedOccType.OtherDepthDamageName))
                {
                    OtherDepthDamageCurve = DepthDamageCurveDictionary[_SelectedOccType.OtherDepthDamageName].Curve;
                }
                NotifyPropertyChanged();
            }
        }
        public Statistics.UncertainCurveDataCollection OtherDepthDamageCurve
        {
            get{return _OtherDepthDamageCurve;}
            set { _OtherDepthDamageCurve = value; NotifyPropertyChanged(); }
        }

        #endregion


        public Dictionary<string, DepthDamage.DepthDamageCurve> DepthDamageCurveDictionary
        {
            get { return _DepthDamageCurveDictionary; }
            set { _DepthDamageCurveDictionary = value; NotifyPropertyChanged(); }
        }


       
       
        public OccupancyTypesElement SelectedOccTypeGroup
        {
            get { return _SelectedOccTypeGroup; }
            set { _SelectedOccTypeGroup = value; LoadDamageCategoriesList(); _OcctypeTabsSelectedDictionary = _SelectedOccTypeGroup.OccTypesSelectedTabsDictionary; NotifyPropertyChanged(); }
        }
            public List<OccupancyTypesElement> OccTypeGroups
        {
            get { return _OccTypeGroups; }
            set { _OccTypeGroups = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
            public OccupancyTypesEditorVM():base()
        {
           // _OcctypeTabsSelectedDictionary = _SelectedOccTypeGroup.OccTypesSelectedTabsDictionary;//new Dictionary<string, bool[]>();
            DepthDamage.DepthDamageCurveData ddcd = new DepthDamage.DepthDamageCurveData(); //this call will load the default DD curves dictionary
            DepthDamageCurveDictionary = DepthDamage.DepthDamageCurveData.CurveDictionary;
            LoadDepthDamageCurveNames();
            OccTypeGroups = OccupancyTypesOwnerElement.ListOfOccupancyTypesGroups;

            //ListOfDataTables = new List<DataTable>();
            //foreach(OccupancyTypesElement ote in OccTypeGroups)
            //{
            //    ListOfDataTables.Add(CreateDataTable(ote));
            //}

        }

        #endregion
        #region Voids

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
            foreach(Consequences_Assist.ComputableObjects.OccupancyType ot in ote.ListOfOccupancyTypes)
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
        public void UpdateKeyInTabsDictionary(string oldName, string newName)
        {
           if( _OcctypeTabsSelectedDictionary.ContainsKey(oldName) == true)
            {
                bool[] value = _OcctypeTabsSelectedDictionary[oldName];
                //delete old one
                _OcctypeTabsSelectedDictionary.Remove(oldName);
                _OcctypeTabsSelectedDictionary.Add(newName, value);
            }
        }

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
            CreateNewDamCatVM vm = new CreateNewDamCatVM();
            Navigate(vm, true, true, "Enter New Damage Category Name");
            if(vm.WasCanceled == false)
            {
                if(vm.HasError == false)
                {
                    //store the new damage category
                    _SelectedOccType.DamageCategory = new Consequences_Assist.ComputableObjects.DamageCategory(vm.Name);

                    SetDamageCategory();
                    LoadDamageCategoriesList();
                    

                }
            }
        }
        public void LaunchNewOccTypeWindow()
        {
            CreateNewDamCatVM vm = new CreateNewDamCatVM();
            Navigate(vm, true, true, "Enter New Occupancy Type Name");
            if (vm.WasCanceled == false)
            {
                if (vm.HasError == false)
                {
                    //create the new occupancy type
                    Consequences_Assist.ComputableObjects.OccupancyType newOT = new Consequences_Assist.ComputableObjects.OccupancyType(vm.Name, SelectedDamageCategory);

                    //load all the values you can
                    newOT.StructureDepthDamageName = SelectedStructureDepthDamage;
                    newOT.ContentDepthDamageName = SelectedContentDepthDamage;
                    newOT.VehicleDepthDamageName = SelectedVehicleDepthDamage;
                    newOT.OtherDepthDamageName = SelectedOtherDepthDamage;

                    newOT.StructureValueUncertainty = new Statistics.None();
                    newOT.ContentValueUncertainty = new Statistics.None();
                    newOT.VehicleValueUncertainty = new Statistics.None();
                    newOT.OtherValueUncertainty = new Statistics.None();

                    newOT.FoundationHeightUncertainty = new Statistics.None();

                    _OcctypeTabsSelectedDictionary.Add(newOT.Name, new bool[] { true, true, true, false });

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
            CreateNewDamCatVM vm = new CreateNewDamCatVM(SelectedOccType.Name + "_Copy");
            Navigate(vm, true, true, "Name of New Occupancy Type");
            if (vm.WasCanceled == false)
            {
                if (vm.HasError == false)
                {
                    //create the new occupancy type
                    Consequences_Assist.ComputableObjects.OccupancyType newOT = new Consequences_Assist.ComputableObjects.OccupancyType(vm.Name, SelectedDamageCategory);

                    //load all the values you can
                    newOT.StructureDepthDamageName = SelectedStructureDepthDamage;
                    newOT.ContentDepthDamageName = SelectedContentDepthDamage;
                    newOT.VehicleDepthDamageName = SelectedVehicleDepthDamage;
                    newOT.OtherDepthDamageName = SelectedOtherDepthDamage;

                    newOT.StructureValueUncertainty = SelectedOccType.StructureValueUncertainty;
                    newOT.ContentValueUncertainty = SelectedOccType.ContentValueUncertainty;
                    newOT.VehicleValueUncertainty = SelectedOccType.VehicleValueUncertainty;
                    newOT.OtherValueUncertainty = SelectedOccType.OtherValueUncertainty;
                    newOT.FoundationHeightUncertainty = SelectedOccType.FoundationHeightUncertainty;

                    bool[] values = _OcctypeTabsSelectedDictionary[SelectedOccType.Name];
                    _OcctypeTabsSelectedDictionary.Add(newOT.Name, values);


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
            //set the selected occtype to be the one before, unless at 0
            if(selectedIndex>0)
            {
                Consequences_Assist.ComputableObjects.OccupancyType ot = SelectedOccTypeGroup.ListOfOccupancyTypes[selectedIndex - 1];
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
                    SetBindingsToNull();
                }
            }

        }

        private void SetBindingsToNull()
        {
             SelectedStructureDepthDamage = null;
            SelectedContentDepthDamage = null;
           SelectedVehicleDepthDamage = null;
           SelectedOtherDepthDamage = null;

            //SelectedDamageCategory = null;

            SelectedOccType = null;         
            
        }

        private void SetDamageCategory()
        {
            if (_SelectedOccType != null)
            {
                SelectedDamageCategory = _SelectedOccType.DamageCategoryName;
            }
           
        }
        private void LoadDamageCategoriesList()
        {
            if(_SelectedOccTypeGroup == null) { return; }
            List<string> uniqueDamCats = new List<string>();
            foreach(Consequences_Assist.ComputableObjects.OccupancyType ot in _SelectedOccTypeGroup.ListOfOccupancyTypes)
            {
                if(uniqueDamCats.Contains(ot.DamageCategoryName))
                { }
                else
                {
                    uniqueDamCats.Add(ot.DamageCategoryName);
                }
            }
            DamageCategoriesList = uniqueDamCats;
        }
        private void LoadDepthDamageCurveNames()
        {
            List<string> structureDDnames = new List<string>();
            List<string> contentDDnames = new List<string>();
            List<string> vehicleDDnames = new List<string>();
            List<string> otherDDnames = new List<string>();


            foreach (string key in _DepthDamageCurveDictionary.Keys.ToList())
            {
                switch(_DepthDamageCurveDictionary[key].DamageType)
                {
                    case DepthDamage.DepthDamageCurve.DamageTypeEnum.Structural:
                        {
                            structureDDnames.Add(key);
                            break;
                        }
                    case DepthDamage.DepthDamageCurve.DamageTypeEnum.Content:
                        {
                            contentDDnames.Add(key);
                            break;
                        }
                    case DepthDamage.DepthDamageCurve.DamageTypeEnum.Vehicle:
                        {
                            vehicleDDnames.Add(key);
                            break;
                        }
                    case DepthDamage.DepthDamageCurve.DamageTypeEnum.Other:
                        {
                            otherDDnames.Add(key);
                            break;
                        }
                }
            }

            StructureDepthDamageStringNames = structureDDnames;
            ContentDepthDamageStringNames = contentDDnames;
            VehicleDepthDamageStringNames = vehicleDDnames;
            OtherDepthDamageStringNames = otherDDnames;

        }
        private void SetDepthDamageCurves()
        {
            if (SelectedOccType == null) { return; }
            //if(_SelectedOccType.StructureDepthDamageName == null) { _SelectedOccType.StructureDepthDamageName = ""; }
            //if (_SelectedOccType.ContentDepthDamageName == null) { _SelectedOccType.ContentDepthDamageName = ""; }
            //if (_SelectedOccType.OtherDepthDamageName == null) { _SelectedOccType.OtherDepthDamageName = ""; }
            //if (_SelectedOccType.VehicleDepthDamageName == null) { _SelectedOccType.VehicleDepthDamageName = ""; }

            StructureDepthDamageCurve = _SelectedOccType.GetStructurePercentDD;
            ContentDepthDamageCurve = _SelectedOccType.GetContentPercentDD;
            VehicleDepthDamageCurve = _SelectedOccType.GetVehiclePercentDD;
            OtherDepthDamageCurve = _SelectedOccType.GetOtherPercentDD;

            //SelectedStructureDepthDamage = _SelectedOccType.GetStructurePercentDD;
            //SelectedContentDepthDamage = _SelectedOccType.ContentDepthDamageName;
            //SelectedVehicleDepthDamage = _SelectedOccType.VehicleDepthDamageName;
            //SelectedOtherDepthDamage = _SelectedOccType.OtherDepthDamageName;


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

        public void LaunchDepthDamageEditor()
        {
            DepthDamage.DepthDamageCurveEditorVM vm = new DepthDamage.DepthDamageCurveEditorVM();
            Navigate(vm, true, true, "Depth Damage Curve Editor");
            if (vm.WasCanceled == false)
            {
                if (vm.HasError == false)
                {
                    //this is called if the user clicks "OK" button on the form

                    //store the new dd curves in the dd curves dictionary
                    Dictionary<string, DepthDamage. DepthDamageCurve> tempDictionary = new Dictionary<string, DepthDamage.DepthDamageCurve>();

                    foreach (DepthDamage.DepthDamageCurveEditorControlVM row in vm.ListOfDepthDamageVMs)
                    {
                        DepthDamage.DepthDamageCurve tempCurve = new DepthDamage.DepthDamageCurve(row.Name, row.Description, row.Curve, row.DamageType);
                        tempDictionary.Add(row.Name, tempCurve);
                    }
                    //update the newly edited curves at the source
                    DepthDamage.DepthDamageCurveData.CurveDictionary = tempDictionary;
                    //bring the curves back into my own dictionary
                    DepthDamageCurveDictionary = DepthDamage.DepthDamageCurveData.CurveDictionary;
                    //in case new dd curves were added or removed, update the list of options
                    LoadDepthDamageCurveNames();
                }
            }
        }
        #endregion
        #region Functions
        
        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
           
        }
    }
}
