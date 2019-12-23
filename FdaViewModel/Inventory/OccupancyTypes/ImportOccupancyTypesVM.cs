using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using FdaViewModel.Utilities;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    //[Author(q0heccdm, 7 / 11 / 2017 1:47:59 PM)]
    public class ImportOccupancyTypesVM : Editors.BaseEditorVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/11/2017 1:47:59 PM
        #endregion
        #region Fields
        //private ObservableCollection<string> _AvailablePaths;
        private string _SelectedPath;
        private BaseViewModel _CurrentView;
        #endregion
        #region Properties

        public BaseViewModel CurrentView
        {
            get { return _CurrentView; }
            set { _CurrentView = value; NotifyPropertyChanged(); }
        }

        //public ObservableCollection<string> AvailablePaths
        //{
        //    get { return _AvailablePaths; }
        //    set { _AvailablePaths = value; NotifyPropertyChanged(); }
        //}
        public string SelectedPath
        {
            get { return _SelectedPath; }
            set { _SelectedPath = value; NotifyPropertyChanged(); }
        }
        
        public List<OccupancyTypesGroupRowItemVM> ListOfRowVMs { get; set; }

        //public List<Consequences_Assist.ComputableObjects.OccupancyType> ListOfOccupancyTypes { get; set; }
        public List<OccupancyTypesElement> ListOfOccTypeElements { get; set; }


        public string OccupancyTypesGroupName { get; set; }
        #endregion
        #region Constructors
        public ImportOccupancyTypesVM() : base(null)
        {
           

            ListOfOccTypeElements = new List<OccupancyTypesElement>();
            ListOfRowVMs = new List<OccupancyTypesGroupRowItemVM>();
        }
        #endregion
        #region Voids
        public void AddGroupsToOwnedElement()
        {
            foreach(OccupancyTypesGroupRowItemVM row in ListOfRowVMs)
            {
                //add the group


                //OccupancyTypesElement ote = new OccupancyTypesElement(row.Name, row.ListOfOccTypes);
                //OccupancyTypesOwnedElement.ListOfOccupancyTypesGroups.Add(ote);

                //add the depth damage curves
                //foreach (DepthDamage.DepthDamageCurve ddc in row.ListOfDepthDamageCurves)
                //{
                //    string ddCurveName = ddc.Name + " from " + row.Name;
                //    if (!DepthDamage.DepthDamageCurveData.CurveDictionary.ContainsKey(ddCurveName))
                //    {
                //        DepthDamage.DepthDamageCurveData.CurveDictionary.Add(ddCurveName, ddc);

                //    }
                //}

            }
        }
        #endregion
        #region Functions
        private bool IsValid(ref string errorMessage)
        {
            if (!System.IO.File.Exists(SelectedPath))
            {
                errorMessage = "File not found.";
                return false;
            }
            //if (OccupancyTypesGroupName == "" || OccupancyTypesGroupName == null)
            //{
            //    errorMessage = "Name cannot be blank.";
            //    return false;
            //}
            //foreach(OccupancyTypesElement ele in OccupancyTypesOwnedElement.ListOfOccupancyTypesGroups)
            //{
            //    if(ele.OccTypesGroupName == OccupancyTypesGroupName)
            //    {
            //        errorMessage = "The name " + OccupancyTypesGroupName + " already exists.";
            //        return false;
            //    }
            //}
            
            return true;
        }
        public bool Import()
        {
            List<Consequences_Assist.ComputableObjects.OccupancyType> ListOfOccupancyTypes = new List<Consequences_Assist.ComputableObjects.OccupancyType>();

            string errorMessage = "";
            if (IsValid(ref errorMessage) == true)
            {
                try
                {
                    Consequences_Assist.ComputableObjects.OccupancyTypes ot = new Consequences_Assist.ComputableObjects.OccupancyTypes(SelectedPath);
                    //ot.LoadFromFile(SelectedPath);
                    ListOfOccupancyTypes = ot.OccupancyTypes;



                }
                catch (Exception e)
                {
                    Utilities.CustomMessageBoxVM cmb = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.OK, e.Message);
                    string header = "Error";
                    DynamicTabVM tab = new DynamicTabVM(header, cmb, "ErrorMessage");
                    Navigate(tab);
                    return false;
                }
                //I added a rule for this, so this shouldn't be possible
                //if (ListOfOccupancyTypes.Count == 0)
                //{
                //    errorMessage = "No occupancy types were detected in the selected file.";
                //    Utilities.CustomMessageBoxVM cmb = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.OK, errorMessage);
                //    Navigate(cmb);
                //    return false;
                //}


                List<DepthDamage.DepthDamageCurve> listOfDDCurves = new List<DepthDamage.DepthDamageCurve>();

                for (int j = 0; j < ListOfOccupancyTypes.Count(); j++)
                {

                    string newStructuralDDCurveName = ListOfOccupancyTypes[j].Name + " Structure Depth Damage Curve"  ;
                    string newContentDDCurveName = ListOfOccupancyTypes[j].Name + " Content Depth Damage Curve" ;
                    string newVehicleDDCurveName = ListOfOccupancyTypes[j].Name + " Vehicle Depth Damage Curve" ;
                    string newOtherDDCurveName = ListOfOccupancyTypes[j].Name + " Other Depth Damage Curve" ;




                    DepthDamage.DepthDamageCurve newStructuralDDCurve = new DepthDamage.DepthDamageCurve(newStructuralDDCurveName, "Structural depth damage curve for " + ListOfOccupancyTypes[j].Name, ListOfOccupancyTypes[j].GetStructurePercentDD, DepthDamage.DepthDamageCurve.DamageTypeEnum.Structural);
                    DepthDamage.DepthDamageCurve newContentDDCurve = new DepthDamage.DepthDamageCurve(newContentDDCurveName, "Content depth damage curve for " + ListOfOccupancyTypes[j].Name, ListOfOccupancyTypes[j].GetContentPercentDD, DepthDamage.DepthDamageCurve.DamageTypeEnum.Content);
                    DepthDamage.DepthDamageCurve newVehicleDDCurve = new DepthDamage.DepthDamageCurve(newVehicleDDCurveName, "Vehicle depth damage curve for " + ListOfOccupancyTypes[j].Name, ListOfOccupancyTypes[j].GetVehiclePercentDD, DepthDamage.DepthDamageCurve.DamageTypeEnum.Vehicle);
                    DepthDamage.DepthDamageCurve newOtherDDCurve = new DepthDamage.DepthDamageCurve(newOtherDDCurveName, "Other depth damage curve for " + ListOfOccupancyTypes[j].Name, ListOfOccupancyTypes[j].GetOtherPercentDD, DepthDamage.DepthDamageCurve.DamageTypeEnum.Other);


                    ListOfOccupancyTypes[j].StructureDepthDamageName = newStructuralDDCurveName;
                    ListOfOccupancyTypes[j].ContentDepthDamageName = newContentDDCurveName;
                    ListOfOccupancyTypes[j].VehicleDepthDamageName = newVehicleDDCurveName;
                    ListOfOccupancyTypes[j].OtherDepthDamageName = newOtherDDCurveName;

                    //add the four dd curves to the list of curves for this group

                    listOfDDCurves.Add(newStructuralDDCurve);
                    listOfDDCurves.Add(newContentDDCurve);
                    listOfDDCurves.Add(newVehicleDDCurve);
                    listOfDDCurves.Add(newOtherDDCurve);


                    //if (!DepthDamage.DepthDamageCurveData.CurveDictionary.ContainsKey(newStructuralDDCurveName))
                    //{
                    //    DepthDamage.DepthDamageCurveData.CurveDictionary.Add(newStructuralDDCurveName, newStructuralDDCurve);

                    //}
                    //if (!DepthDamage.DepthDamageCurveData.CurveDictionary.ContainsKey(newContentDDCurveName))
                    //{
                    //    DepthDamage.DepthDamageCurveData.CurveDictionary.Add(newContentDDCurveName, newContentDDCurve);
                    //}
                    //if (!DepthDamage.DepthDamageCurveData.CurveDictionary.ContainsKey(newVehicleDDCurveName))
                    //{
                    //    DepthDamage.DepthDamageCurveData.CurveDictionary.Add(newVehicleDDCurveName, newVehicleDDCurve);
                    //}
                    //if (!DepthDamage.DepthDamageCurveData.CurveDictionary.ContainsKey(newOtherDDCurveName))
                    //{
                    //    DepthDamage.DepthDamageCurveData.CurveDictionary.Add(newOtherDDCurveName, newOtherDDCurve);
                    //}




                }




                OccupancyTypesGroupRowItemVM rowVM = new OccupancyTypesGroupRowItemVM(SelectedPath, System.IO.Path.GetFileNameWithoutExtension(SelectedPath),ListOfOccupancyTypes,listOfDDCurves);
                ListOfRowVMs.Add(rowVM);
                //OccupancyTypesElement ote = new OccupancyTypesElement(OccupancyTypesGroupName, ListOfOccupancyTypes);
                //ListOfOccTypeElements.Add(ote);
                //OccupancyTypesOwnedElement.ListOfOccupancyTypesGroups.Add(ote);
                SelectedPath = "";
                return true;
            }
            else
            {
                Utilities.CustomMessageBoxVM cmb = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.OK, errorMessage);
                string header = "Error";
                DynamicTabVM tab = new DynamicTabVM(header, cmb, "ErrorMessage");
                Navigate(tab);
                return false;
            }

        }
        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
            AddRule(nameof(SelectedPath), () =>
            {
                if(ListOfRowVMs.Count==0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
               

            }, "No occupancy type groups have been added for import.", true);
        }

        public override bool RunSpecialValidation()
        {
            //test that no new names match existing names, and that no row has multiple occtypes with the same name
            List<string> uniqueNameList = new List<string>();

            List<OccupancyTypesElement> existingElements = StudyCache.GetChildElementsOfType<OccupancyTypesElement>();

            foreach (OccupancyTypesGroupRowItemVM row in ListOfRowVMs)
            {
                if(uniqueNameList.Contains(row.Name))
                {
                    System.Windows.MessageBox.Show("Multiple rows have the same name of '" + row.Name + "'.", "Error", System.Windows.MessageBoxButton.OK);
                    return false;
                }
                else
                {
                    uniqueNameList.Add(row.Name);
                }

                if(IsOccTypeElementNameUnique(row.Name, existingElements) == false) { return false; }
                //create a dummy tabs checked dictionary
                Dictionary<string, bool[]> _OcctypeTabsSelectedDictionary = new Dictionary<string, bool[]>();

                foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in row.ListOfOccTypes)
                {
                    if (_OcctypeTabsSelectedDictionary.ContainsKey(ot.Name))
                    {
                        System.Windows.MessageBox.Show("Multiple occupancy types found with the same name '" + ot.Name + "' in " + row.Name , "Error", System.Windows.MessageBoxButton.OK);
                        return false;
                    }
                    else
                    {
                        bool[] tabsCheckedArray = new bool[] { true, true, true, false };
                        _OcctypeTabsSelectedDictionary.Add(ot.Name, tabsCheckedArray);
                    }
                }
            }
            return true;
        }

        private bool IsOccTypeElementNameUnique(string name, List<OccupancyTypesElement> existingElements)
        {
            foreach (OccupancyTypesElement elem in existingElements)
            {
                if (elem.Name.Equals(name))
                {
                    System.Windows.MessageBox.Show("The name " + name + " already exists. All names must be unique.", "Error", System.Windows.MessageBoxButton.OK);
                    return false;
                }
            }
            return true;
        }

        public override void Save()
        {
            List<OccupancyTypesElement> elementsToSave = new List<OccupancyTypesElement>();
            foreach (OccupancyTypesGroupRowItemVM row in ListOfRowVMs)
            {
                //create a dummy tabs checked dictionary
                Dictionary<string, bool[]> _OcctypeTabsSelectedDictionary = new Dictionary<string, bool[]>();

                foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in row.ListOfOccTypes)
                {
                    bool[] tabsCheckedArray = new bool[] { true, true, true, false };
                    //if(_OcctypeTabsSelectedDictionary.ContainsKey(ot.Name))
                    //{
                    //    System.Windows.MessageBox.Show("Multiple occupancy types found with the same name: " + ot.Name, "Error", System.Windows.MessageBoxButton.OK);
                    //    return;
                    //}
                    _OcctypeTabsSelectedDictionary.Add(ot.Name, tabsCheckedArray);

                }

                OccupancyTypesElement elem = new OccupancyTypesElement(row.Name, row.ListOfOccTypes, _OcctypeTabsSelectedDictionary);
                //OccupancyTypesOwnerElement.ListOfOccupancyTypesGroups.Add(elem);
                elementsToSave.Add(elem);
            }
            //foreach (OccupancyTypesElement element in elementsToSave)
            //{
            List<ChildElement> tmp = elementsToSave.ToList<ChildElement>();
            Saving.PersistenceFactory.GetOccTypeManager().SaveNewElements(tmp);
            //}

            //object[] args = new object[] { elementsToSave, Actions };
            //OccupancyTypesOwnerElement.SaveFilesOnBackgroundThread(this, new DoWorkEventArgs(args));
        }

       


    }
}
