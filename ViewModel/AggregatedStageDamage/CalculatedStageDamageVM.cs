using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class CalculatedStageDamageVM : BaseViewModel
    {
        private HydraulicElement _SelectedWaterSurfaceElevation;
        private InventoryElement _SelectedStructureInventoryElement;
        private IndexPointsElement _SelectedIndexPointsElement;
        private CalculatedStageDamageRowItem _SelectedRow;
        private bool _ShowChart;
        private TableWithPlotVM _TableWithPlot;
        private string _CurvesEditedLabel;

        public string CurvesEditedLabel
        {
            get { return _CurvesEditedLabel; }
            set { _CurvesEditedLabel = value; NotifyPropertyChanged(); }
        }
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

        public ObservableCollection<HydraulicElement> WaterSurfaceElevations { get; } = new ObservableCollection<HydraulicElement>();

        public HydraulicElement SelectedWaterSurfaceElevation
        {
            get { return _SelectedWaterSurfaceElevation; }
            set { _SelectedWaterSurfaceElevation = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<IndexPointsElement> IndexPoints { get; } = new ObservableCollection<IndexPointsElement>();
        public IndexPointsElement SelectedIndexPoints
        {
            get { return _SelectedIndexPointsElement; }
            set { _SelectedIndexPointsElement = value; NotifyPropertyChanged(); }
        }

        public CalculatedStageDamageVM()
        {
            Rows = new ObservableCollection<CalculatedStageDamageRowItem>();
            LoadStructureInventories();
            LoadDepthGrids();
            LoadNewImpactAreaFrequencyRows();
            LoadIndexPoints();
        }
        
        public CalculatedStageDamageVM(int wseId, int inventoryID, List<StageDamageCurve> curves, List<ImpactAreaFrequencyFunctionRowItem> impAreaFrequencyRows)
        {
            Rows = new ObservableCollection<CalculatedStageDamageRowItem>();
            LoadStructureInventories();
            SelectInventory(inventoryID);
            LoadDepthGrids();
            LoadIndexPoints();
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
            UpdateComputedCurvesModifiedLabel();
        }       

        private void LoadNewImpactAreaFrequencyRows()
        {
            List<ImpactAreaElement> impAreaElems = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impAreaElems.Count > 0)
            {
                List<ImpactAreaRowItem> impactAreaRowsCollection = impAreaElems[0].ImpactAreaRows;
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
                CalculatedStageDamageRowItem newRow = new CalculatedStageDamageRowItem(i, curve.ImpArea, curve.DamCat, curve.ComputeComponent, curve.AssetCategory, curve.ConstructionType);
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
            foreach (HydraulicElement wat in WaterSurfaceElevations)
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
        private void LoadDepthGrids()
        {
            List<HydraulicElement> WSEElements = StudyCache.GetChildElementsOfType<HydraulicElement>();
            foreach (HydraulicElement elem in WSEElements)
            {
                WaterSurfaceElevations.Add(elem);
            }
            if (WaterSurfaceElevations.Count > 0)
            {
                SelectedWaterSurfaceElevation = WaterSurfaceElevations[0];
            }
        }

        private void LoadIndexPoints()
        {
            List<IndexPointsElement> indexPoints = StudyCache.GetChildElementsOfType<IndexPointsElement>();
            foreach (IndexPointsElement elem in indexPoints)
            {
                IndexPoints.Add(elem);
            }
            if (IndexPoints.Count > 0)
            {
                SelectedIndexPoints = IndexPoints[0];
            }
        }

        private void LoadStructureInventories()
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

        
       

        public void ComputeCurves()
        {
            //todo: this is creating dummy curves for now.
            //todo: make this a seperate class. StageDamageConfiguration



            //structures: need shapefile and mapping of occtype, the set of occtypes
            //imp area frequency rows should get dumbed down to just impact area name to UPD curves
            //we know that we have an impact area. We only allow one, so it will be the first one.
            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            ImpactAreaElement impactAreaElement = impactAreaElements[0];

            StageDamageConfiguration config = new StageDamageConfiguration(impactAreaElement, SelectedWaterSurfaceElevation, SelectedStructures,
                SelectedIndexPoints, ImpactAreaFrequencyRows);

            FdaValidationResult vr = config.Validate();
            if (vr.IsValid)
            {
                Rows.Clear();

                //todo: Make a call to the model and pass in the config object.
                //modelComputeObject obj =  config.CreateModelComputeObject();
                //someResultsObject = obj.compute();
                //then i will translate the results into my VM row items to be displayed in the UI


                //todo delete these dummy rows once we have the actual compute in place.
                for (int i = 1; i < 11; i++)
                {
                    ComputeComponentVM curve = new ComputeComponentVM(StringConstants.STAGE_DAMAGE, StringConstants.STAGE, StringConstants.DAMAGE);
                    Rows.Add(new CalculatedStageDamageRowItem(i, impactAreaElements[0].ImpactAreaRows[0], "testDamCat" + i, curve, "Total", StageDamageConstructionType.COMPUTED));
                }
                //end dummy rows
                if (Rows.Count > 0)
                {                  
                    ShowChart = true;
                    SelectedRow = Rows[0];                   
                }
                UpdateComputedCurvesModifiedLabel();
            }
            else
            {
                MessageBox.Show(vr.ErrorMessage, "Unable to Compute", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void TableDataChanged(object sender, EventArgs e)
        {
            SelectedRow.ConstructionType = StageDamageConstructionType.COMPUTED_EDITED;
            UpdateComputedCurvesModifiedLabel();
        }

        private void UpdateComputedCurvesModifiedLabel()
        {
            List<int> editedRows = new List<int>();
            foreach(CalculatedStageDamageRowItem row in Rows)
            {
                if(row.ConstructionType == StageDamageConstructionType.COMPUTED_EDITED)
                {
                    editedRows.Add(row.ID);
                }
            }
            if(editedRows.Count>0)
            {
                CurvesEditedLabel = "User modified curves: " + string.Join(", ", editedRows);
            }
            else
            {
                CurvesEditedLabel = "";
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
            if (SelectedIndexPoints == null)
            {
                vr.AddErrorMessage("Index Points must be selected.");
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
                StageDamageCurve curve = new StageDamageCurve(r.ImpactArea, r.DamageCategory, r.ComputeComponent, r.AssetCategory, r.ConstructionType); 
                curves.Add(curve);
            }
            return curves;
        }

        private void RowChanged()
        {
            if (TableWithPlot != null)
            {
                //remove previous event
                TableWithPlot.WasModified -= TableDataChanged;
            }

            if (SelectedRow != null)
            {
                TableWithPlot = new TableWithPlotVM(SelectedRow.ComputeComponent);
                //add the event
                TableWithPlot.WasModified += TableDataChanged;
            }
        }

        

    }
}
