using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.stageDamage;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using SciChart.Core.Extensions;
using Utility;
using Visual.Observables;

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
        private readonly Func<string> getName;
        private bool _WriteDetailsFile = true;

        public int AnalysisYear { get; set; }

        public bool WriteDetailsFile
        {
            get { return _WriteDetailsFile; }
            set { _WriteDetailsFile = value; NotifyPropertyChanged(); }
        }

        public string CurvesEditedLabel
        {
            get { return _CurvesEditedLabel; }
            set { _CurvesEditedLabel = value; NotifyPropertyChanged(); }
        }
        public List<ImpactAreaFrequencyFunctionRowItem> ImpactAreaFrequencyRows { get; } = new List<ImpactAreaFrequencyFunctionRowItem>();
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
            set { _SelectedRow = value; NotifyPropertyChanged(); RowChanged(); }
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

        public CalculatedStageDamageVM(Func<string> getName)
        {
            AnalysisYear = DateTime.Now.Year;
            Rows = new ObservableCollection<CalculatedStageDamageRowItem>();
            LoadStructureInventories();
            LoadDepthGrids();
            LoadNewImpactAreaFrequencyRows();
            AddLiveUpdateEvents();
            this.getName = getName;
        }

        public CalculatedStageDamageVM(int wseId, int inventoryID, int analysisYear, List<StageDamageCurve> curves, List<ImpactAreaFrequencyFunctionRowItem> impAreaFrequencyRows, bool writeDetailsOut, Func<string> getName)
        {
            AnalysisYear = analysisYear;
            WriteDetailsFile = writeDetailsOut;
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
            AddLiveUpdateEvents();
            this.getName = getName;
        }

        #region Live Update Events

        private void AddLiveUpdateEvents()
        {
            StudyCache.FlowFrequencyAdded += AddFlowFreqElement;
            StudyCache.FlowFrequencyRemoved += RemoveFlowFreqElement;
            StudyCache.FlowFrequencyUpdated += UpdateFlowFreqElement;

            StudyCache.InflowOutflowAdded += AddRegUnregElement;
            StudyCache.InflowOutflowRemoved += RemoveRegUnregElement;
            StudyCache.InflowOutflowUpdated += UpdateRegUnregElement;

            StudyCache.RatingAdded += AddRatingElement;
            StudyCache.RatingRemoved += RemoveRatingElement;
            StudyCache.RatingUpdated += UpdateRatingElement;

            StudyCache.WaterSurfaceElevationAdded += AddHydraulicElement;
            StudyCache.WaterSurfaceElevationRemoved += RemoveHydraulicElement;
            StudyCache.WaterSurfaceElevationUpdated += UpdateHydraulicElement;

            StudyCache.StructureInventoryAdded += AddStructuresElement;
            StudyCache.StructureInventoryRemoved += RemoveStructuresElement;
            StudyCache.StructureInventoryUpdated += UpdateStructuresElement;

        }

        private void AddStructuresElement(object sender, ElementAddedEventArgs e)
        {
            Structures.Add((InventoryElement)e.Element);
        }

        private void RemoveStructuresElement(object sender, ElementAddedEventArgs e)
        {
            Structures.Remove(Structures.Single(s =>
            {
                if (s.ID == e.Element.ID)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }));
        }

        private void UpdateStructuresElement(object sender, ElementUpdatedEventArgs e)
        {
            int indexToUpdate = -1;

            for (int i = 0; i < Structures.Count; i++)
            {
                if (Structures[i].ID == e.NewElement.ID)
                {
                    indexToUpdate = i;
                    break;
                }
            }

            if (indexToUpdate != -1)
            {
                bool updateSelected = false;
                if (SelectedStructures == Structures[indexToUpdate])
                {
                    updateSelected = true;
                }
                Structures[indexToUpdate] = (InventoryElement)e.NewElement;
                if (updateSelected)
                {
                    SelectedStructures = Structures[indexToUpdate];
                }
            }
        }


        private void AddHydraulicElement(object sender, ElementAddedEventArgs e)
        {
            WaterSurfaceElevations.Add((HydraulicElement)e.Element);
        }

        private void RemoveHydraulicElement(object sender, ElementAddedEventArgs e)
        {
            WaterSurfaceElevations.Remove(WaterSurfaceElevations.Single(s =>
                {
                    if (s.ID == e.Element.ID)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }));
        }

        private void UpdateHydraulicElement(object sender, ElementUpdatedEventArgs e)
        {
            int indexToUpdate = -1;

            for (int i = 0; i < WaterSurfaceElevations.Count; i++)
            {
                if (WaterSurfaceElevations[i].ID == e.NewElement.ID)
                {
                    indexToUpdate = i;
                    break;
                }
            }

            if (indexToUpdate != -1)
            {
                bool updateSelected = false;
                if (SelectedWaterSurfaceElevation == WaterSurfaceElevations[indexToUpdate])
                {
                    updateSelected = true;
                }
                WaterSurfaceElevations[indexToUpdate] = (HydraulicElement)e.NewElement;
                if (updateSelected)
                {
                    SelectedWaterSurfaceElevation = WaterSurfaceElevations[indexToUpdate];
                }
            }
        }


        private void AddRatingElement(object sender, ElementAddedEventArgs e)
        {
            foreach (ImpactAreaFrequencyFunctionRowItem row in ImpactAreaFrequencyRows)
            {
                row.StageDischargeFunctions.Add(new StageDischargeElementWrapper((StageDischargeElement)e.Element));
            }
        }

        private void RemoveRatingElement(object sender, ElementAddedEventArgs e)
        {
            foreach (ImpactAreaFrequencyFunctionRowItem row in ImpactAreaFrequencyRows)
            {
                row.StageDischargeFunctions.Remove(row.StageDischargeFunctions.Single(s =>
                {
                    if (s.Element != null && s.Element.ID == e.Element.ID)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }));
            }
        }

        private void UpdateRatingElement(object sender, ElementUpdatedEventArgs e)
        {
            foreach (ImpactAreaFrequencyFunctionRowItem row in ImpactAreaFrequencyRows)
            {
                foreach (StageDischargeElementWrapper freq in row.StageDischargeFunctions)
                {
                    if (freq.Element != null && freq.Element.ID == e.NewElement.ID)
                    {
                        freq.Element = (StageDischargeElement)e.NewElement;
                    }
                }
            }
        }

        private void AddFlowFreqElement(object sender, ElementAddedEventArgs e)
        {
            foreach (ImpactAreaFrequencyFunctionRowItem row in ImpactAreaFrequencyRows)
            {
                row.FrequencyFunctions.Add(new FrequencyElementWrapper((FrequencyElement)e.Element));
            }
        }

        private void RemoveFlowFreqElement(object sender, ElementAddedEventArgs e)
        {
            foreach (ImpactAreaFrequencyFunctionRowItem row in ImpactAreaFrequencyRows)
            {
                row.FrequencyFunctions.Remove(row.FrequencyFunctions.Single(s =>
                {
                    if (s.Element != null && s.Element.ID == e.Element.ID)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }));
            }
        }

        private void UpdateFlowFreqElement(object sender, ElementUpdatedEventArgs e)
        {

            foreach (ImpactAreaFrequencyFunctionRowItem row in ImpactAreaFrequencyRows)
            {
                foreach (FrequencyElementWrapper freq in row.FrequencyFunctions)
                {
                    if (freq.Element != null && freq.Element.ID == e.NewElement.ID)
                    {
                        freq.Element = (FrequencyElement)e.NewElement;
                    }
                }
            }
        }

        private void AddRegUnregElement(object sender, ElementAddedEventArgs e)
        {
            foreach (ImpactAreaFrequencyFunctionRowItem row in ImpactAreaFrequencyRows)
            {
                row.RegulatedUnregulatedFunctions.Add(new RegulatedUnregulatedElementWrapper((InflowOutflowElement)e.Element));
            }
        }

        private void RemoveRegUnregElement(object sender, ElementAddedEventArgs e)
        {
            foreach (ImpactAreaFrequencyFunctionRowItem row in ImpactAreaFrequencyRows)
            {
                row.RegulatedUnregulatedFunctions.Remove(row.RegulatedUnregulatedFunctions.Single(s =>
                {
                    if (s.Element != null && s.Element.ID == e.Element.ID)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }));
            }
        }

        private void UpdateRegUnregElement(object sender, ElementUpdatedEventArgs e)
        {

            foreach (ImpactAreaFrequencyFunctionRowItem row in ImpactAreaFrequencyRows)
            {
                foreach (RegulatedUnregulatedElementWrapper freq in row.RegulatedUnregulatedFunctions)
                {
                    if (freq.Element != null && freq.Element.ID == e.NewElement.ID)
                    {
                        freq.Element = (InflowOutflowElement)e.NewElement;
                    }
                }
            }
        }

        #endregion

        private void LoadNewImpactAreaFrequencyRows()
        {
            List<ImpactAreaElement> impAreaElems = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impAreaElems.Count > 0)
            {
                List<ImpactAreaRowItem> impactAreaRowsCollection = impAreaElems[0].ImpactAreaRows;
                List<FrequencyElement> analyticalFrequencyElements = StudyCache.GetChildElementsOfType<FrequencyElement>();
                List<StageDischargeElement> ratingCurveElements = StudyCache.GetChildElementsOfType<StageDischargeElement>();
                List<InflowOutflowElement> inflowOutflowElements = StudyCache.GetChildElementsOfType<InflowOutflowElement>();

                foreach (ImpactAreaRowItem impactAreaRow in impactAreaRowsCollection)
                {
                    ImpactAreaFrequencyFunctionRowItem newRow = new(impactAreaRow, analyticalFrequencyElements, ratingCurveElements, inflowOutflowElements);
                    //register for "HasChanges"
                    RegisterChildViewModel(newRow);
                    ImpactAreaFrequencyRows.Add(newRow);
                }
            }
        }

        private void LoadCurves(List<StageDamageCurve> curves)
        {
            for (int i = 0; i < curves.Count; i++)
            {
                //used cloned curve so that you do not modify the original data
                StageDamageCurve curve = curves[i];
                CalculatedStageDamageRowItem newRow = new(i + 1, curve.ImpArea, curve.DamCat, curve.ComputeComponent, curve.AssetCategory, curve.ConstructionType);
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
            foreach (InventoryElement ie in Structures)
            {
                if (ie.ID == inventoryID)
                {
                    SelectedStructures = ie;
                    foundInventory = true;
                    break;
                }
            }
            if (!foundInventory)
            {
                MessageBox.Show("The previously selected inventory used in the compute of these aggregated stage-damage functions was deleted. Please select a new inventory and recompute.", "Inventory Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void SelectDepthGrid(int waterID)
        {
            bool foundHydro = false;
            foreach (HydraulicElement wat in WaterSurfaceElevations)
            {
                if (wat.ID == waterID)
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
            foreach (InventoryElement elem in inventoryElements)
            {
                Structures.Add(elem);
            }
            if (Structures.Count > 0)
            {
                SelectedStructures = Structures[0];
            }
        }

        public async Task ComputeCurvesAsync()
        {
            string name = getName();
            if (name.IsNullOrWhiteSpace())
            {
                MessageBox.Show("A name is required to compute.", "Name Required", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                await RunComputeAsync();
            }
        }

        private async Task RunComputeAsync()
        {
            //we know that we have an impact area. We only allow one, so it will be the first one.
            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            ImpactAreaElement impactAreaElement = impactAreaElements[0];

            StageDamageConfiguration config = new(impactAreaElement, SelectedWaterSurfaceElevation, SelectedStructures,
                ImpactAreaFrequencyRows, AnalysisYear);

            FdaValidationResult vr = config.ValidateConfiguration();
            if (vr.IsValid)
            {
                Rows.Clear();
                List<UncertainPairedData> stageDamageFunctions = await ComputeStageDamageFunctionsAsync(config);
                LoadComputedCurveRows(stageDamageFunctions);

                if (Rows.Count > 0)
                {
                    ShowChart = true;
                    SelectedRow = Rows[0];
                }
                else
                {
                    ShowChart = false;
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
            for (int i = 0; i < computedCurves.Count; i++)
            {
                UncertainPairedData upd = computedCurves[i];
                CurveComponentVM computeComponent = new(StringConstants.STAGE_DAMAGE, StringConstants.STAGE, StringConstants.DAMAGE, DistributionOptions.HISTOGRAM_ONLY);
                computeComponent.SetPairedData(upd);
                //get the impact area from the id
                int impactAreaID = upd.ImpactAreaID;
                ImpactAreaRowItem impactAreaRowItem = StudyCache.GetChildElementsOfType<ImpactAreaElement>()[0].GetImpactAreaRow(impactAreaID);

                Rows.Add(new CalculatedStageDamageRowItem(i + 1, impactAreaRowItem, upd.DamageCategory, computeComponent, upd.AssetCategory, StageDamageConstructionType.COMPUTED));
            }

        }

        private static FdaValidationResult DoStructuresExist(ScenarioStageDamage scenarioStageDamage)
        {
            FdaValidationResult vr = new();
            int totalStructureCount = 0;
            foreach (ImpactAreaStageDamage stageDamage in scenarioStageDamage.ImpactAreaStageDamages)
            {
                totalStructureCount += stageDamage.Inventory.Structures.Count;
            }
            if (totalStructureCount == 0)
            {
                List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
                string impactAreaName = impactAreaElements[0].Name;
                vr.AddErrorMessage("The compute will not run because this scenario has zero structures inside the impact area set '" + impactAreaName +
                    ".'");
            }
            return vr;
        }

        private static List<ImpactAreaStageDamage> GetIAStageDamagesWithZeroStructures(ScenarioStageDamage scenarioStageDamage)
        {
            List<ImpactAreaStageDamage> withZeroStructs = new();
            foreach (ImpactAreaStageDamage stageDamage in scenarioStageDamage.ImpactAreaStageDamages)
            {
                if (stageDamage.Inventory.Structures.Count == 0)
                {
                    withZeroStructs.Add(stageDamage);
                }
            }
            return withZeroStructs;
        }

        private static string GenerateNoStructuresMessage(List<ImpactAreaStageDamage> zeroStructuresImpactAreas)
        {
            FdaValidationResult vr = new();

            ImpactAreaElement impactAreaElement = StudyCache.GetChildElementsOfType<ImpactAreaElement>()[0];
            foreach (ImpactAreaStageDamage stageDamage in zeroStructuresImpactAreas)
            {
                string impactAreaName = impactAreaElement.GetImpactAreaRow(stageDamage.ImpactAreaID).Name;
                vr.AddErrorMessage("No structures detected in impact area '" + impactAreaName);
            }

            return vr.ErrorMessage;
        }

        private static bool ValidateStructureCount(ScenarioStageDamage scenarioStageDamage)
        {
            bool canCompute = true;
            //check if we have any structures to compute
            FdaValidationResult vr = DoStructuresExist(scenarioStageDamage);
            if (!vr.IsValid)
            {
                MessageBox.Show(vr.ErrorMessage, "No Structures", MessageBoxButton.OK, MessageBoxImage.Error);
                canCompute = false;
            }
            else
            {
                //TODO: seems like passing around a lot of data when all we need is impact area names 
                List<ImpactAreaStageDamage> zeroStructuresImpactAreas = GetIAStageDamagesWithZeroStructures(scenarioStageDamage);
                if (zeroStructuresImpactAreas.Count > 0)
                {
                    string zeroStructsMessage = GenerateNoStructuresMessage(zeroStructuresImpactAreas);
                    var Result = MessageBox.Show(zeroStructsMessage + Environment.NewLine +
                    Environment.NewLine + "Do you want to continue?", "Missing Structures", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (Result != MessageBoxResult.Yes)
                    {
                        canCompute = false;
                    }
                }
            }
            return canCompute;
        }

        /// <summary>
        /// Runs the stage damage compute
        /// </summary>
        /// <param name="config"></param>
        /// <returns>The list of UPD curves created during the compute</returns>
        private async Task<List<UncertainPairedData>> ComputeStageDamageFunctionsAsync(StageDamageConfiguration config)
        {
            List<UncertainPairedData> stageDamageFunctions = new();
            try
            {
                List<ImpactAreaStageDamage> impactAreaStageDamages = config.CreateStageDamages();

                ScenarioStageDamage scenarioStageDamage = new(impactAreaStageDamages);

                bool canCompute = ValidateStructureCount(scenarioStageDamage);
                if (canCompute)
                {
                    List<UncertainPairedData> quantityDamagedElementsUPD = new();
                    //these are the rows in the computed table
                    ISynchronizationContext context = new SynchronizationContext(action => Application.Current.Dispatcher.BeginInvoke(action));
                    BatchJob batchJob = new(uiThreadSyncContext: context);
                    StageDamageComputeLogVM vm = new(batchJob);
                    string header = "Compute Log: Aggregated Stage Damage";
                    DynamicTabVM tab = new(header, vm, Guid.NewGuid().ToString());
                    Navigate(tab, false, false);

                    (stageDamageFunctions, quantityDamagedElementsUPD) = await Task.Run(() => scenarioStageDamage.Compute(reporter:batchJob.Reporter));
                    List<string> errors = scenarioStageDamage.GetErrorMessages();
                    if (errors.Count > 0)
                    {
                        MessageBox.Show("There are errors in the data used to compute stage-damage. See the errors file in the structure stage damage details folder of this study directory.", "Data Errors", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("The compute has completed.", "Compute Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    WriteErrors(errors);
                    if (WriteDetailsFile)
                    {
                        WriteDetailsCsvFile(scenarioStageDamage, quantityDamagedElementsUPD);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while trying to compute stage damages:\n" + ex.Message + " " + ex.StackTrace, "Compute Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //TODO: COMMUNICATE ERROR MESSAGES

            return stageDamageFunctions;
        }

        private void WriteErrors(List<string> list)
        {
            string structureStageDamageErrorsFileName = getName() + "_StageDamageErrorMessages.log";
            string directory = Storage.Connection.Instance.GetStructureStageDamageDetailsDirectory;
            Directory.CreateDirectory(directory);
            string stageDamagePath = directory + "\\" + structureStageDamageErrorsFileName;

            using StreamWriter writer = new(File.Create(stageDamagePath));
            foreach (string error in list)
            {
                writer.WriteLine(error);
            }
        }

        private void TableDataChanged(object sender, EventArgs e)
        {
            SelectedRow.ConstructionType = StageDamageConstructionType.COMPUTED_EDITED;
            UpdateComputedCurvesModifiedLabel();
        }

        private void UpdateComputedCurvesModifiedLabel()
        {
            List<int> editedRows = new();
            foreach (CalculatedStageDamageRowItem row in Rows)
            {
                if (row.ConstructionType == StageDamageConstructionType.COMPUTED_EDITED)
                {
                    editedRows.Add(row.ID);
                }
            }
            if (editedRows.Count > 0)
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
            FdaValidationResult vr = new();
            if (AnalysisYear < 1900 || AnalysisYear > 3000)
            {
                const string Msg = "The analysis year must be greater than 1900 and less than 3000.";
                vr.AddErrorMessage(Msg);
            }
            if (SelectedWaterSurfaceElevation == null)
            {
                const string Msg = "A hydraulics data set must be selected.";
                vr.AddErrorMessage(Msg);
            }
            if (SelectedStructures == null)
            {
                const string Msg = "A structure inventory must be selected.";
                vr.AddErrorMessage(Msg);
            }
            if (Rows.Count == 0)
            {
                const string Msg = "No curves have been computed. Compute curves to save.";
                vr.AddErrorMessage(Msg);
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
            List<StageDamageCurve> curves = new();
            foreach (CalculatedStageDamageRowItem r in Rows)
            {
                //in theory this call can throw an exception, but we handle that in the validation
                //if we get here, then the curves should be constructable.
                StageDamageCurve curve = new(r.ImpactArea, r.DamageCategory, r.ComputeComponent, r.AssetCategory, r.ConstructionType);
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

        //TODO: GET STRUCTURE COUNT DETAILS WRITTEN TO FILE
        private void WriteDetailsCsvFile(ScenarioStageDamage scenarioStageDamage, List<UncertainPairedData> quantityDamagedElementsUPD)
        {
            //Its necessary for the details to be written with names for the impact areas, not FIDs. It helps users troubleshoot. I don't like this, but that's why the dictionary is passed in. to translate at the
            //lowest level before it's written
            Dictionary<int, string> iaNames = [];
            ImpactAreaElement iaEle = StudyCache.GetChildElementsOfType<ImpactAreaElement>()[0];
            List<ImpactAreaRowItem> iaRows = iaEle.ImpactAreaRows;
            for (int i = 0; i < iaRows.Count; i++)
            {
                iaNames[i] = iaRows[i].Name;
            }

            string directory = Storage.Connection.Instance.GetStructureStageDamageDetailsDirectory;
            Directory.CreateDirectory(directory);
            const string DamageElementCountFileName = "_DamagedElementCountsByStage.csv";
            const string StructureStageDamageDetailsFileName = "_StructureStageDamageDetails.csv";
            string stageDamagePath = directory + "\\" + getName() + StructureStageDamageDetailsFileName;
            string damagedElementsCountPath = directory + "\\" + getName() + DamageElementCountFileName;

            List<string> stageDamageDetails = scenarioStageDamage.ProduceStructureDetails(iaNames);
            //overwrites the existing file or creates a new one. 
            using StreamWriter detailsWriter = new(File.Create(stageDamagePath));
            foreach (string iaDetails in stageDamageDetails)
            {
                detailsWriter.WriteLine(iaDetails);
            }
            

            List<string> damagedElementCounts = UncertainPairedData.ConvertDamagedElementCountToText(quantityDamagedElementsUPD, iaNames);
            using StreamWriter damagedElesWriter = new(File.Create(damagedElementsCountPath));
            foreach (string line in damagedElementCounts)
            {
                damagedElesWriter.WriteLine(line);
            }
        }

    }
}
