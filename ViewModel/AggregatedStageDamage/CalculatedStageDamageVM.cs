using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.stageDamage;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class CalculatedStageDamageVM : BaseViewModel
    {
        private HydraulicElement _SelectedWaterSurfaceElevation;
        private InventoryElement _SelectedStructureInventoryElement;
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

        public CalculatedStageDamageVM()
        {
            Rows = new ObservableCollection<CalculatedStageDamageRowItem>();
            LoadStructureInventories();
            LoadDepthGrids();
            LoadNewImpactAreaFrequencyRows();
        }
        
        public CalculatedStageDamageVM(int wseId, int inventoryID, List<StageDamageCurve> curves, List<ImpactAreaFrequencyFunctionRowItem> impAreaFrequencyRows)
        {
            Rows = new ObservableCollection<CalculatedStageDamageRowItem>();
            LoadStructureInventories();
            SelectInventory(inventoryID);
            LoadDepthGrids();
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
                List<StageDischargeElement> ratingCurveElements = StudyCache.GetChildElementsOfType<StageDischargeElement>();

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
            for(int i = 0; i < curves.Count; i++)
            {
                //used cloned curve so that you do not modify the original data
                StageDamageCurve curve = new StageDamageCurve(curves[i].WriteToXML());
                CalculatedStageDamageRowItem newRow = new CalculatedStageDamageRowItem(i + 1, curve.ImpArea, curve.DamCat, curve.ComputeComponent, curve.AssetCategory, curve.ConstructionType);
                Rows.Add(newRow);
            }
            if (Rows.Count > 0)
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
            //we know that we have an impact area. We only allow one, so it will be the first one.
            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            ImpactAreaElement impactAreaElement = impactAreaElements[0];

            StageDamageConfiguration config = new StageDamageConfiguration(impactAreaElement, SelectedWaterSurfaceElevation, SelectedStructures,
                ImpactAreaFrequencyRows);

            FdaValidationResult vr = config.Validate();
            if (vr.IsValid)
            {
                Rows.Clear();           
                List<UncertainPairedData> stageDamageFunctions = ComputeStageDamageFunctions(config);
                LoadComputedCurveRows(stageDamageFunctions);

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

        private void LoadComputedCurveRows(List<UncertainPairedData> computedCurves)
        {
            for(int i =0;i<computedCurves.Count;i++)
            {
                UncertainPairedData upd = computedCurves[i];
                ComputeComponentVM computeComponent = new ComputeComponentVM(StringConstants.STAGE_DAMAGE, StringConstants.STAGE, StringConstants.DAMAGE);              
                computeComponent.SetPairedData(upd);
                //get the impact area from the id
                int impactAreaID = upd.ImpactAreaID;
                ImpactAreaRowItem impactAreaRowItem = StudyCache.GetChildElementsOfType<ImpactAreaElement>()[0].GetImpactAreaRow(impactAreaID);
                
                Rows.Add(new CalculatedStageDamageRowItem(i+1, impactAreaRowItem, upd.DamageCategory, computeComponent,upd.AssetCategory, StageDamageConstructionType.COMPUTED));
            }

        }

        /// <summary>
        /// Runs the stage damage compute
        /// </summary>
        /// <param name="config"></param>
        /// <returns>The list of UPD curves created during the compute</returns>
        private List<UncertainPairedData> ComputeStageDamageFunctions(StageDamageConfiguration config)
        {
            ScenarioStageDamage scenarioStageDamage = new ScenarioStageDamage(config.CreateStageDamages());

            int seed = 1234;
            Model.compute.RandomProvider randomProvider = new Model.compute.RandomProvider(seed);
            Study.StudyPropertiesElement propElem = StudyCache.GetStudyPropertiesElement();
            Statistics.ConvergenceCriteria convergenceCriteria = propElem.GetStudyConvergenceCriteria();
            //these are the rows in the computed table
            List<UncertainPairedData> stageDamageFunctions = scenarioStageDamage.Compute(randomProvider, convergenceCriteria);
            return stageDamageFunctions;
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
