using paireddata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.WaterSurfaceElevation;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class CalculatedStageDamageVM : BaseViewModel
    {
        public event EventHandler SelectedRowChanged;

        private ObservableCollection<WaterSurfaceElevationElement> _WaterSurfaceElevations;
        private WaterSurfaceElevationElement _SelectedWaterSurfaceElevation;

        private ObservableCollection<InventoryElement> _StructureInventoryElements;
        private InventoryElement _SelectedStructureInventoryElement;
        private CalculatedStageDamageRowItem _SelectedRow;
        private bool _ShowChart;

        public ObservableCollection<CalculatedStageDamageRowItem> Rows { get; set; }

        public bool ShowChart
        {
            get { return _ShowChart; }
            set { _ShowChart = value; NotifyPropertyChanged(); }
        }
        public CalculatedStageDamageRowItem SelectedRow
        {
            get { return _SelectedRow; }
            set { _SelectedRow = value; NotifyPropertyChanged(); SelectedRowChanged?.Invoke(this, new EventArgs()); }
        }

        public ObservableCollection<InventoryElement> Structures
        {
            get { return _StructureInventoryElements; }
            set { _StructureInventoryElements = value; NotifyPropertyChanged(); }
        }

        public InventoryElement SelectedStructures
        {
            get { return _SelectedStructureInventoryElement; }
            set { _SelectedStructureInventoryElement = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<WaterSurfaceElevationElement> WaterSurfaceElevations
        {
            get { return _WaterSurfaceElevations; }
            set { _WaterSurfaceElevations = value; NotifyPropertyChanged(); }
        }

        public WaterSurfaceElevationElement SelectedWaterSurfaceElevation
        {
            get { return _SelectedWaterSurfaceElevation; }
            set { _SelectedWaterSurfaceElevation = value; NotifyPropertyChanged(); }
        }

        public int SelectedRowIndex {get;set;}

        public CalculatedStageDamageVM()
        {
            Rows = new ObservableCollection<CalculatedStageDamageRowItem>();
            loadStructureInventories();
            loadDepthGrids();
        }
        
        public CalculatedStageDamageVM(int wseId, int inventoryID, List<StageDamageCurve> curves)
        {
            Rows = new ObservableCollection<CalculatedStageDamageRowItem>();
            loadStructureInventories();
            SelectInventory(inventoryID);
            loadDepthGrids();
            SelectDepthGrid(wseId);
            LoadCurves(curves);
        }

        private void LoadCurves(List<StageDamageCurve> curves)
        {
            int i = 1;
            foreach (StageDamageCurve curve in curves)
            {
                CalculatedStageDamageRowItem newRow = new CalculatedStageDamageRowItem(i, curve.ImpArea, curve.DamCat, curve.Function);
                Rows.Add(newRow);
                i++;
            }
            if(Rows.Count>0)
            {
                ShowChart = true;
                SelectedRowIndex = 0;
            }
        }

        private void SelectInventory(int inventoryID)
        {
            foreach(InventoryElement ie in Structures)
            {
                if(ie.GetElementID() == inventoryID)
                {
                    SelectedStructures = ie;
                    break;
                }
            }
        }
        private void SelectDepthGrid(int waterID)
        {
            foreach(WaterSurfaceElevationElement wat in WaterSurfaceElevations)
            {
                if(wat.GetElementID() == waterID)
                {
                    SelectedWaterSurfaceElevation = wat;
                    break;
                }
            }
        }
        private void loadDepthGrids()
        {
            WaterSurfaceElevations = new ObservableCollection<WaterSurfaceElevationElement>();
            List<WaterSurfaceElevationElement> WSEElements = StudyCache.GetChildElementsOfType<WaterSurfaceElevationElement>();
            foreach (WaterSurfaceElevationElement elem in WSEElements)
            {
                WaterSurfaceElevations.Add(elem);
            }
            if (WaterSurfaceElevations.Count > 0)
            {
                SelectedWaterSurfaceElevation = WaterSurfaceElevations[0];
            }
        }

        private void loadStructureInventories()
        {
            Structures = new ObservableCollection<InventoryElement>();
            List<InventoryElement> inventoryElements = StudyCache.GetChildElementsOfType<InventoryElement>();
            foreach(InventoryElement elem in inventoryElements)
            {
                Structures.Add(elem);
            }
            if (Structures.Count > 0)
            {
                SelectedStructures = Structures[0];
            }
        }

        private FdaValidationResult ValidateForCompute()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if(SelectedWaterSurfaceElevation == null || SelectedStructures == null)
            {
                vr.AddErrorMessage("A water surface elevation and a structure inventory selection is required to compute.");
            }
            return vr;
        }

        public void CalculateCurves()
        {
            //todo Richard will implament this method. I am not sure what all you need. You can grab elements from the study cache
            //just like i did in the line below to get the impact area elements. You will need to create "CalculatedStageDamageRowItem"s.
            //These objects basically hold an impact area, damcat, coordinates function. The coordinates function gets created by using
            //the ICoordinatesFunctionsFactory. To get the curves to show up in the UI you just add them to the "Rows" property.

            FdaValidationResult vr = ValidateForCompute();
            if (vr.IsValid)
            {
                //we know that we have an impact area, we need to verify that we have a structure inventory
                List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();

                //there should only ever be one impact area. To get to this point we know we have an impact area.
                ImpactAreaElement impactAreaElement = impactAreaElements[0];

                InventoryElement inventoryElement = SelectedStructures;
                WaterSurfaceElevationElement waterSurfaceElevationElement = SelectedWaterSurfaceElevation;

                //todo delete these dummy rows once we have the actual compute in place.
                for (int i = 1; i < 11; i++)
                {
                    UncertainPairedData uncertainPairedData = UncertainPairedDataFactory.CreateDefaultNormalData("Stage", "Damage", "testName");

                    Rows.Add(new CalculatedStageDamageRowItem(i, impactAreaElements[0].ImpactAreaRows[0], "testDamCat" + i, uncertainPairedData));
                }
                //end dummy rows
                ShowChart = true;
                if (Rows.Count > 0)
                {
                    SelectedRowIndex = 0;
                }
            }
            else
            {
                MessageBox.Show(vr.ErrorMessage, "Unable to Compute", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public FdaValidationResult ValidateForm()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (SelectedWaterSurfaceElevation == null)
            {
                vr.AddErrorMessage("A hydraulics data set must be selected.");
            }
            if (SelectedStructures == null)
            {
                vr.AddErrorMessage("A structure inventory must be selected.");
            }
            if (Rows.Count == 0)
            {
                vr.AddErrorMessage("No curves have been computed. Compute curves to save.");
            }
            return vr;
        }

        /// <summary>
        /// This should only be called after validation has been called. Creating a coordinates funtion from the table
        /// of values can throw an exception. 
        /// </summary>
        /// <returns></returns>
        public List<StageDamageCurve> GetStageDamageCurves()
        {
            List<StageDamageCurve> curves = new List<StageDamageCurve>();
            foreach (CalculatedStageDamageRowItem r in Rows)
            {
                //in theory this call can throw an exception, but we handle that in the validation
                //if we get here, then the curves should be constructable.
                StageDamageCurve curve = new StageDamageCurve(r.ImpactArea, r.DamageCategory, null); //r.EditorVM.CreateFunctionFromTables());
                curves.Add(curve);
            }

            return curves;
        }


    }
}
