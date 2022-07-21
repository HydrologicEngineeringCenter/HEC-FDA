using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Hydraulics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class CalculatedStageDamageVM : BaseViewModel
    {
        private WaterSurfaceElevationElement _SelectedWaterSurfaceElevation;
        private InventoryElement _SelectedStructureInventoryElement;
        private CalculatedStageDamageRowItem _SelectedRow;
        private bool _ShowChart;
        private TableWithPlotVM _TableWithPlot;


        public List<ImpactAreaFrequencyFunctionRowItem> ImpactAreaFrequencyRows { get;} = new List<ImpactAreaFrequencyFunctionRowItem>();
        public ObservableCollection<CalculatedStageDamageRowItem> Rows { get; set; }

        public TableWithPlotVM TableWithPlot
        {
            get { return _TableWithPlot; }
            set { _TableWithPlot = value; NotifyPropertyChanged(); }
        }
        public bool ShowChart
        {
            get { return _ShowChart; }
            set { _ShowChart = value; NotifyPropertyChanged(); }
        }
        public CalculatedStageDamageRowItem SelectedRow
        {
            get { return _SelectedRow; }
            set { _SelectedRow = value; NotifyPropertyChanged(); RowChanged();}
        }

        public ObservableCollection<InventoryElement> Structures { get; } = new ObservableCollection<InventoryElement>();

        public InventoryElement SelectedStructures
        {
            get { return _SelectedStructureInventoryElement; }
            set { _SelectedStructureInventoryElement = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<WaterSurfaceElevationElement> WaterSurfaceElevations { get; } = new ObservableCollection<WaterSurfaceElevationElement>();

        public WaterSurfaceElevationElement SelectedWaterSurfaceElevation
        {
            get { return _SelectedWaterSurfaceElevation; }
            set { _SelectedWaterSurfaceElevation = value; NotifyPropertyChanged(); }
        }

        public CalculatedStageDamageVM()
        {
            Rows = new ObservableCollection<CalculatedStageDamageRowItem>();
            loadStructureInventories();
            loadDepthGrids();
            LoadNewImpactAreaFrequencyRows();
        }
        
        public CalculatedStageDamageVM(int wseId, int inventoryID, List<StageDamageCurve> curves, List<ImpactAreaFrequencyFunctionRowItem> impAreaFrequencyRows)
        {
            Rows = new ObservableCollection<CalculatedStageDamageRowItem>();
            loadStructureInventories();
            SelectInventory(inventoryID);
            loadDepthGrids();
            SelectDepthGrid(wseId);
            LoadCurves(curves);

            //clone the rows
            foreach (ImpactAreaFrequencyFunctionRowItem row in impAreaFrequencyRows)
            {
                ImpactAreaFrequencyFunctionRowItem clonedRow = row.Clone();
                //register for "HasChanges"
                RegisterChildViewModel(clonedRow);
                ImpactAreaFrequencyRows.Add(clonedRow);
            }
        }       

        private void LoadNewImpactAreaFrequencyRows()
        {
            List<ImpactAreaElement> impAreaElems = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impAreaElems.Count > 0)
            {
                ObservableCollection<ImpactAreaRowItem> impactAreaRowsCollection = impAreaElems[0].ImpactAreaRows;
                List<AnalyticalFrequencyElement> analyticalFrequencyElements = StudyCache.GetChildElementsOfType<AnalyticalFrequencyElement>();
                List<RatingCurveElement> ratingCurveElements = StudyCache.GetChildElementsOfType<RatingCurveElement>();

                foreach (ImpactAreaRowItem impactAreaRow in impactAreaRowsCollection)
                {
                    ImpactAreaFrequencyFunctionRowItem newRow = new ImpactAreaFrequencyFunctionRowItem(impactAreaRow, analyticalFrequencyElements, ratingCurveElements);
                    //register for "HasChanges"
                    RegisterChildViewModel(newRow);
                    ImpactAreaFrequencyRows.Add(newRow);
                }
            }
        }

        private void LoadCurves(List<StageDamageCurve> curves)
        {
            int i = 1;
            foreach (StageDamageCurve stageDamageCurve in curves)
            {
                //used cloned curve so that you do not modify the original data
                StageDamageCurve curve = new StageDamageCurve(stageDamageCurve.WriteToXML());
                CalculatedStageDamageRowItem newRow = new CalculatedStageDamageRowItem(i, curve.ImpArea, curve.DamCat, curve.ComputeComponent, curve.AssetCategory);
                Rows.Add(newRow);
                i++;
            }
            if(Rows.Count>0)
            {
                ShowChart = true;
                SelectedRow = Rows[0];
            }
        }

        private void SelectInventory(int inventoryID)
        {
            bool foundInventory = false;
            foreach(InventoryElement ie in Structures)
            {
                if(ie.ID == inventoryID)
                {
                    SelectedStructures = ie;
                    foundInventory = true;
                    break;
                }
            }
            if(!foundInventory)
            {
                MessageBox.Show("The previously selected inventory used in the compute of these aggregated stage-damage functions was deleted. Please select a new inventory and recompute.", "Inventory Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void SelectDepthGrid(int waterID)
        {
            bool foundHydro = false;
            foreach (WaterSurfaceElevationElement wat in WaterSurfaceElevations)
            {
                if(wat.ID == waterID)
                {
                    SelectedWaterSurfaceElevation = wat;
                    foundHydro = true;
                    break;
                }
            }
            if (!foundHydro)
            {
                MessageBox.Show("The previously selected hydraulics data set used in the compute of these aggregated stage-damage functions was deleted. Please select a new hydraulics data set and recompute.", "Hydraulogy Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void loadDepthGrids()
        {
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
                vr.AddErrorMessage("A hydraulics data set and a structure inventory selection is required to compute.");
            }
            else
            {
                vr.AddErrorMessage(ValidateImpactAreaFrequencyFunctionTable().ErrorMessage);
            }
            return vr;
        }

        private FdaValidationResult ValidateImpactAreaFrequencyFunctionTable()
        {
            FdaValidationResult vr = new FdaValidationResult();
            foreach (ImpactAreaFrequencyFunctionRowItem row in ImpactAreaFrequencyRows)
            {
                vr.AddErrorMessage(row.ValidateRow().ErrorMessage);
            }
            return vr;
        }

        public void ComputeCurves()
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
                    ComputeComponentVM curve = new ComputeComponentVM(StringConstants.STAGE_DAMAGE, StringConstants.STAGE, StringConstants.DAMAGE);
                    Rows.Add(new CalculatedStageDamageRowItem(i, impactAreaElements[0].ImpactAreaRows[0], "testDamCat" + i, curve, "Total"));
                }
                //end dummy rows
                if (Rows.Count > 0)
                {
                    TableWithPlot = new TableWithPlotVM(Rows[0].ComputeComponent);
                    ShowChart = true;
                    SelectedRow = Rows[0];
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
                StageDamageCurve curve = new StageDamageCurve(r.ImpactArea, r.DamageCategory, r.ComputeComponent, r.AssetCategory); 
                curves.Add(curve);
            }
            return curves;
        }

        private void RowChanged()
        {
            TableWithPlot = new TableWithPlotVM(SelectedRow.ComputeComponent);
        }
    }
}
