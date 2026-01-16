using HEC.CS.Collections;
using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class SpecificIASEditorVM : BaseViewModel
    {
        private string _selectedAssetCategory;
        private string _selectedDamageCategory;
        private ChildElementComboItem _selectedFrequencyRelationship;
        private bool _ratingRequired;
        private bool _DefaultStageRequired = false;
        private bool _showWarnings;
        private ChildElementComboItem _selectedInflowOutflowElement;
        private ChildElementComboItem _selectedRatingCurveElement;
        private ChildElementComboItem _selectedLeveeElement;
        private ChildElementComboItem _selectedExteriorInteriorElement;
        private bool _showEAD;
        private double _EAD;
        private PairedData _DamageFrequencyCurve = null;

        private readonly Func<ChildElementComboItem> _SelectedStageDamage;
        private bool _IsSufficientForCompute;
        private string _IsSufficientForComputeTooltip;
        private bool _calculateDefualtThresholdChecked = true;
        private double _DefaultStage;
        private bool _ScenarioReflectsEnabled;
        private bool _HasFailureStageDamage;
        private bool _HasNonFailureStageDamage;
        private bool _HasFailureStageLifeLoss;
        private bool _HasNonFailureStageLifeLoss;

        public ChildElementComboItem SelectedNonFailureStageDamage { get; set; }
        public ChildElementComboItem SelectedFailureStageLifeLoss { get; set; }
        public ChildElementComboItem SelectedNonFailureStageLifeLoss { get; set; }

        public bool HasFailureStageDamage
        {
            get { return _HasFailureStageDamage; }
            set { _HasFailureStageDamage = value; NotifyPropertyChanged(); }
        }
        public bool HasNonFailureStageDamage
        {
            get { return _HasNonFailureStageDamage; }
            set { _HasNonFailureStageDamage = value; NotifyPropertyChanged(); }
        }
        public bool HasFailureStageLifeLoss
        {
            get { return _HasFailureStageLifeLoss; }
            set { _HasFailureStageLifeLoss = value; NotifyPropertyChanged(); }
        }
        public bool HasNonFailureStageLifeLoss
        {
            get { return _HasNonFailureStageLifeLoss; }
            set { _HasNonFailureStageLifeLoss = value; NotifyPropertyChanged(); }
        }
        public bool ScenarioReflectsEnabled
        {
            get { return _ScenarioReflectsEnabled; }
            set { _ScenarioReflectsEnabled = value; NotifyPropertyChanged(); }
        }
        public double DefaultStage
        {
            get { return _DefaultStage; }
            set { _DefaultStage = value; NotifyPropertyChanged(); }
        }
        public bool CalculateDefaultThresholdChecked
        {
            get { return _calculateDefualtThresholdChecked; }
            set { _calculateDefualtThresholdChecked = value; UpdateDefaultStageRequired(); NotifyPropertyChanged(); }
        }

        public string IsSufficientForComputeTooltip
        {
            get { return _IsSufficientForComputeTooltip; }
            set { _IsSufficientForComputeTooltip = value; NotifyPropertyChanged(); }
        }

        public bool IsSufficientForCompute
        {
            get { return _IsSufficientForCompute; }
            set { _IsSufficientForCompute = value; NotifyPropertyChanged(); }
        }
        public ImpactAreaRowItem CurrentImpactArea { get; }
        public double EAD
        {
            get { return _EAD; }
            set { _EAD = value; NotifyPropertyChanged(); }
        }
        public bool ShowEAD
        {
            get { return _showEAD; }
            set { _showEAD = value; NotifyPropertyChanged(); }
        }
        public IASPlotControlVM PlotControlVM { get; } = new IASPlotControlVM();

        public bool ShowWarnings
        {
            get { return _showWarnings; }
            set { _showWarnings = value; NotifyPropertyChanged(); }
        }
        public bool RatingRequired
        {
            get { return _ratingRequired; }
            set { _ratingRequired = value; NotifyPropertyChanged(); }
        }

        public bool DefaultStageRequired
        {
            get { return _DefaultStageRequired; }
            set { _DefaultStageRequired = value; NotifyPropertyChanged(); }
        }


        public int Year { get; set; } = DateTime.Now.Year;
        public CustomObservableCollection<StageDamageCurve> StageDamageCurves { get; } = new CustomObservableCollection<StageDamageCurve>();
        public CustomObservableCollection<string> DamCats { get; } = new CustomObservableCollection<string>();
        public CustomObservableCollection<string> AssetCategories { get; } = new CustomObservableCollection<string>();
        public List<ThresholdRowItem> Thresholds { get; set; } = new List<ThresholdRowItem>();
        public CustomObservableCollection<ChildElementComboItem> FrequencyElements { get; } = new CustomObservableCollection<ChildElementComboItem>();
        public CustomObservableCollection<ChildElementComboItem> InflowOutflowElements { get; } = new CustomObservableCollection<ChildElementComboItem>();
        public CustomObservableCollection<ChildElementComboItem> RatingCurveElements { get; } = new CustomObservableCollection<ChildElementComboItem>();
        public CustomObservableCollection<ChildElementComboItem> LeveeFeatureElements { get; } = new CustomObservableCollection<ChildElementComboItem>();
        public CustomObservableCollection<ChildElementComboItem> ExteriorInteriorElements { get; } = new CustomObservableCollection<ChildElementComboItem>();

        public string SelectedAssetCategory
        {
            get { return _selectedAssetCategory; }
            set { _selectedAssetCategory = value; NotifyPropertyChanged(); }
        }

        public string SelectedDamageCategory
        {
            get { return _selectedDamageCategory; }
            set { _selectedDamageCategory = value; NotifyPropertyChanged(); }
        }

        public StageDamageCurve SelectedDamageCurve
        {
            get { return GetSelectedDamageCurve(); }
        }

        //the dam cats and asset cats come from the stage damage curves
        //not possible to not find correct function 
        private StageDamageCurve GetSelectedDamageCurve()
        {
            StageDamageCurve returnCurve = null;
            List<StageDamageCurve> stageDamageCurves = StageDamageCurves.ToList();
            foreach (StageDamageCurve stageDamageCurve in stageDamageCurves)
            {
                if (stageDamageCurve.AssetCategory == SelectedAssetCategory && stageDamageCurve.DamCat == SelectedDamageCategory)
                {
                    returnCurve = stageDamageCurve;
                    break;
                }
            }
            return returnCurve;
        }

        public ChildElementComboItem SelectedFrequencyElement
        {
            get { return _selectedFrequencyRelationship; }
            set { _selectedFrequencyRelationship = value; UpdateRatingRequired(); UpdateSufficientToCompute(); }
        }
        public ChildElementComboItem SelectedInflowOutflowElement
        {
            get { return _selectedInflowOutflowElement; }
            set { _selectedInflowOutflowElement = value; NotifyPropertyChanged(); UpdateSufficientToCompute(); }
        }
        public ChildElementComboItem SelectedRatingCurveElement
        {
            get { return _selectedRatingCurveElement; }
            set { _selectedRatingCurveElement = value; NotifyPropertyChanged(); UpdateSufficientToCompute(); }
        }
        public ChildElementComboItem SelectedLeveeFeatureElement
        {
            get { return _selectedLeveeElement; }
            set { _selectedLeveeElement = value; NotifyPropertyChanged(); UpdateThresholdStageValue(); UpdateDefaultStageRequired(); UpdateSufficientToCompute(); }
        }
        public ChildElementComboItem SelectedExteriorInteriorElement
        {
            get { return _selectedExteriorInteriorElement; }
            set { _selectedExteriorInteriorElement = value; NotifyPropertyChanged(); UpdateSufficientToCompute(); }
        }

        /// <summary>
        /// The rows that show up in the "Warnings" expander after hitting the plot button.
        /// </summary>
        public ObservableCollection<RecommendationRowItem> MessageRows { get; } = new ObservableCollection<RecommendationRowItem>();

        /// <summary>
        /// This is the create new ctor
        /// </summary>
        public SpecificIASEditorVM(ImpactAreaRowItem rowItem, Func<ChildElementComboItem> getSelectedStageDamage)
        {
            _SelectedStageDamage = getSelectedStageDamage;
            CurrentImpactArea = rowItem;
            Initialize();
        }

        public SpecificIASEditorVM(SpecificIAS elem, ImpactAreaRowItem rowItem, Func<ChildElementComboItem> getSelectedStageDamage)
        {
            _SelectedStageDamage = getSelectedStageDamage;
            CurrentImpactArea = rowItem; //Maybe set default threshold here
            DefaultStage = elem.DefaultStage;
            Initialize();
            FillForm(elem);
        }

        private void Initialize()
        {
            LoadElements();

            StudyCache.FlowFrequencyAdded += AddFlowFreqElement;
            StudyCache.FlowFrequencyRemoved += RemoveFlowFreqElement;
            StudyCache.FlowFrequencyUpdated += UpdateFlowFreqElement;

            StudyCache.InflowOutflowAdded += AddInOutElement;
            StudyCache.InflowOutflowRemoved += RemoveInOutElement;
            StudyCache.InflowOutflowUpdated += UpdateInOutElement;

            StudyCache.RatingAdded += AddRatingElement;
            StudyCache.RatingRemoved += RemoveRatingElement;
            StudyCache.RatingUpdated += UpdateRatingElement;

            StudyCache.LeveeAdded += AddLeveeElement;
            StudyCache.LeveeRemoved += RemoveLeveeElement;
            StudyCache.LeveeUpdated += UpdateLeveeElement;

            StudyCache.ExteriorInteriorAdded += AddExtIntElement;
            StudyCache.ExteriorInteriorRemoved += RemoveExtIntElement;
            StudyCache.ExteriorInteriorUpdated += UpdateExtIntElement;
        }

        #region Live Update Event Methods

        private void AddExtIntElement(object sender, ElementAddedEventArgs e)
        {
            ExteriorInteriorElements.Add(new ChildElementComboItem(e.Element));
        }
        private void RemoveExtIntElement(object sender, ElementAddedEventArgs e)
        {
            RemoveElement(e.Element.ID, ExteriorInteriorElements);
            SelectedExteriorInteriorElement = ExteriorInteriorElements[0];
        }
        private void UpdateExtIntElement(object sender, ElementUpdatedEventArgs e)
        {
            UpdateElement(ExteriorInteriorElements, SelectedExteriorInteriorElement, e.NewElement);
        }
        private void AddLeveeElement(object sender, ElementAddedEventArgs e)
        {
            LeveeFeatureElements.Add(new ChildElementComboItem(e.Element));
        }
        private void RemoveLeveeElement(object sender, ElementAddedEventArgs e)
        {
            RemoveElement(e.Element.ID, LeveeFeatureElements);
            SelectedLeveeFeatureElement = LeveeFeatureElements[0];
        }
        private void UpdateLeveeElement(object sender, ElementUpdatedEventArgs e)
        {
            UpdateElement(LeveeFeatureElements, SelectedLeveeFeatureElement, e.NewElement);
        }

        private void AddInOutElement(object sender, ElementAddedEventArgs e)
        {
            InflowOutflowElements.Add(new ChildElementComboItem(e.Element));
            SelectedInflowOutflowElement = InflowOutflowElements[0];
        }
        private void RemoveInOutElement(object sender, ElementAddedEventArgs e)
        {
            RemoveElement(e.Element.ID, InflowOutflowElements);
        }
        private void UpdateInOutElement(object sender, ElementUpdatedEventArgs e)
        {
            UpdateElement(InflowOutflowElements, SelectedInflowOutflowElement, e.NewElement);
        }
        private void AddFlowFreqElement(object sender, ElementAddedEventArgs e)
        {
            FrequencyElements.Add(new ChildElementComboItem(e.Element));
        }
        private void RemoveFlowFreqElement(object sender, ElementAddedEventArgs e)
        {
            RemoveElement(e.Element.ID, FrequencyElements);
            SelectedFrequencyElement = FrequencyElements[0];
        }
        private void UpdateFlowFreqElement(object sender, ElementUpdatedEventArgs e)
        {
            UpdateElement(FrequencyElements, SelectedFrequencyElement, e.NewElement);
            //check for the situation where the selected frequency doesn't require a rating, but the new updated one does
            UpdateRatingRequired();
        }
        private void AddRatingElement(object sender, ElementAddedEventArgs e)
        {
            RatingCurveElements.Add(new ChildElementComboItem(e.Element));
        }
        private void RemoveRatingElement(object sender, ElementAddedEventArgs e)
        {
            RemoveElement(e.Element.ID, RatingCurveElements);
            SelectedRatingCurveElement = RatingCurveElements[0];
        }
        private void UpdateRatingElement(object sender, ElementUpdatedEventArgs e)
        {
            UpdateElement(RatingCurveElements, SelectedRatingCurveElement, e.NewElement);
        }
        public static void RemoveElement(int idToRemove, ObservableCollection<ChildElementComboItem> collection)
        {
            collection.Remove(collection.Where(elem => elem.ChildElement != null && elem.ID == idToRemove).Single());
        }
        public static void UpdateElement(ObservableCollection<ChildElementComboItem> collection, ChildElementComboItem selectedItem,
              ChildElement newElement)
        {
            int idToUpdate = newElement.ID;
            ChildElementComboItem itemToUpdate = collection.Where(elem => elem.ChildElement != null && elem.ID == idToUpdate).SingleOrDefault();
            if (itemToUpdate != null)
            {
                itemToUpdate.ChildElement = newElement;
            }
        }

        #endregion

        private void FillForm(SpecificIAS elem)
        {
            CalculateDefaultThresholdChecked = elem.CalculateDefaultThreshold;
            DefaultStage = elem.DefaultStage;
            Thresholds.AddRange(elem.Thresholds);
            //all the available elements have been loaded into this editor. We now want to select
            //the correct element for each dropdown. 
            //all the available elements have been loaded into this editor. We now want to select
            //the correct element for each dropdown. If we can't find the correct element then the selected elem 
            //will be null.
            SelectedFrequencyElement = FrequencyElements.FirstOrDefault(freq => freq.ChildElement != null && freq.ChildElement.ID == elem.FlowFreqID);
            SelectedInflowOutflowElement = InflowOutflowElements.FirstOrDefault(inf => inf.ChildElement != null && inf.ChildElement.ID == elem.InflowOutflowID);
            SelectedRatingCurveElement = RatingCurveElements.FirstOrDefault(rat => rat.ChildElement != null && rat.ChildElement.ID == elem.RatingID);
            SelectedLeveeFeatureElement = LeveeFeatureElements.FirstOrDefault(levee => levee.ChildElement != null && levee.ChildElement.ID == elem.LeveeFailureID); //at this point we're gonna update the threshold. Do we wanna do that? 
            SelectedExteriorInteriorElement = ExteriorInteriorElements.FirstOrDefault(ext => ext.ChildElement != null && ext.ChildElement.ID == elem.ExtIntStageID);

            //i don't want a selected value to ever be null. Even if there are no elements we should select the blank row option.
            //so if it is null, i will set it to the first option which is empty.
            SelectedFrequencyElement ??= FrequencyElements[0];
            SelectedInflowOutflowElement ??= InflowOutflowElements[0];
            SelectedRatingCurveElement ??= RatingCurveElements[0];
            SelectedLeveeFeatureElement ??= LeveeFeatureElements[0];
            SelectedExteriorInteriorElement ??= ExteriorInteriorElements[0];

        }

        private void LoadElements()
        {
            //what happens if there are no elements for a combo?
            //I will always add an empty ChildElementComboItem and then select it by default.
            //this means that when asking for the selected combo item, it should never be null.
            List<ChildElement> childElems = new();

            List<FrequencyElement> analyticalFrequencyElements = StudyCache.GetChildElementsOfType<FrequencyElement>();
            childElems.Clear();
            childElems.AddRange(analyticalFrequencyElements);
            FrequencyElements.AddRange(CreateComboItems(childElems));
            SelectedFrequencyElement = FrequencyElements.First();

            List<InflowOutflowElement> inflowOutflowElements = StudyCache.GetChildElementsOfType<InflowOutflowElement>();
            childElems.Clear();
            childElems.AddRange(inflowOutflowElements);
            InflowOutflowElements.AddRange(CreateComboItems(childElems));
            SelectedInflowOutflowElement = InflowOutflowElements.First();

            List<StageDischargeElement> ratingCurveElements = StudyCache.GetChildElementsOfType<StageDischargeElement>();
            childElems.Clear();
            childElems.AddRange(ratingCurveElements);
            RatingCurveElements.AddRange(CreateComboItems(childElems));
            SelectedRatingCurveElement = RatingCurveElements.First();

            List<LateralStructureElement> leveeFeatureElements = StudyCache.GetChildElementsOfType<LateralStructureElement>();
            childElems.Clear();
            childElems.AddRange(leveeFeatureElements);
            LeveeFeatureElements.AddRange(CreateComboItems(childElems));
            SelectedLeveeFeatureElement = LeveeFeatureElements.First();

            List<ExteriorInteriorElement> exteriorInteriorElements = StudyCache.GetChildElementsOfType<ExteriorInteriorElement>();
            childElems.Clear();
            childElems.AddRange(exteriorInteriorElements);
            ExteriorInteriorElements.AddRange(CreateComboItems(childElems));
            SelectedExteriorInteriorElement = ExteriorInteriorElements.First();
        }

        private static ObservableCollection<ChildElementComboItem> CreateComboItems(List<ChildElement> elems)
        {
            ObservableCollection<ChildElementComboItem> items = new()
            {
                new ChildElementComboItem(null)
            };
            foreach (ChildElement elem in elems)
            {
                items.Add(new ChildElementComboItem(elem));
            }
            return items;
        }

        private void UpdateRatingRequired()
        {
            if (SelectedFrequencyElement != null && SelectedFrequencyElement.ChildElement is FrequencyElement elem)
            {
                RatingRequired = false;
                if (elem.IsAnalytical || elem.GraphicalUsesFlow)
                {
                    RatingRequired = true;
                }
            }
        }

        /// <summary>
        /// When the stage damage selection is changed we need to update the dam cats that go into
        /// the combo next to the plot button.
        /// </summary>
        public void StageDamageSelectionChanged(ChildElementComboItem selectedStageDamage)
        {
            if (selectedStageDamage != null && selectedStageDamage.ChildElement != null)
            {
                List<StageDamageCurve> stageDamageCurves = GetStageDamageCurves();

                LoadAssetCategories(stageDamageCurves);
                LoadDamageCategories(stageDamageCurves);

                StageDamageCurves.Clear();
                StageDamageCurves.AddRange(stageDamageCurves);
            }
            else
            {
                //the user selected the blank row. Clear the damage category combo
                StageDamageCurves.Clear();
            }
        }

        private void LoadDamageCategories(List<StageDamageCurve> stageDamageCurves)
        {
            List<string> damageCategories = new();
            foreach (StageDamageCurve curve in stageDamageCurves)
            {
                damageCategories.Add(curve.DamCat);
            }
            DamCats.Clear();
            DamCats.AddRange(damageCategories.Distinct());
            if (DamCats.Count > 0)
            {
                SelectedDamageCategory = DamCats[0];
            }
        }

        private void LoadAssetCategories(List<StageDamageCurve> stageDamageCurves)
        {
            List<string> assetCategories = new();
            foreach (StageDamageCurve curve in stageDamageCurves)
            {
                assetCategories.Add(curve.AssetCategory);
            }
            AssetCategories.Clear();
            AssetCategories.AddRange(assetCategories.Distinct());
            if (AssetCategories.Count > 0)
            {
                SelectedAssetCategory = AssetCategories[0];
            }
        }

        private List<StageDamageCurve> GetStageDamageCurves()
        {
            ChildElementComboItem selectedStageDamage = _SelectedStageDamage();
            List<StageDamageCurve> stageDamageCurves = new();
            if (selectedStageDamage != null && selectedStageDamage.ChildElement != null)
            {
                AggregatedStageDamageElement elem = (AggregatedStageDamageElement)selectedStageDamage.ChildElement;
                stageDamageCurves = elem.Curves.Where(curve => curve.ImpArea.ID == CurrentImpactArea.ID).ToList();
            }
            return stageDamageCurves;
        }

        #region validation

        private FdaValidationResult GetFrequencyRelationshipValidationResult()
        {
            FdaValidationResult vr = new();
            if (SelectedFrequencyElement == null || SelectedFrequencyElement.ChildElement == null)
            {
                vr.AddErrorMessage("A Frequency Relationship is required.");
            }
            return vr;
        }

        private FdaValidationResult GetRatingCurveValidationResult()
        {
            FdaValidationResult vr = new();
            if (_ratingRequired && (SelectedRatingCurveElement == null || SelectedRatingCurveElement.ChildElement == null))
            {
                vr.AddErrorMessage("A stage-discharge function is required if the frequency function reflects discharge");
            }
            return vr;
        }

        #endregion

        public void UpdateSufficientToCompute()
        {
            FdaValidationResult result = GetValidationResults();
            IsSufficientForCompute = result.IsValid;
            if (IsSufficientForCompute)
            {
                IsSufficientForComputeTooltip = "Can compute";
            }
            else
            {
                IsSufficientForComputeTooltip = result.ErrorMessage;
            }
        }

        public FdaValidationResult GetValidationResults()
        {
            FdaValidationResult vr = new();

            vr.AddErrorMessage(GetFrequencyRelationshipValidationResult().ErrorMessage);
            vr.AddErrorMessage(GetRatingCurveValidationResult().ErrorMessage);

            if (!vr.IsValid)
            {
                vr.InsertMessage(0, "Errors in Impact Area: " + CurrentImpactArea.Name);
            }
            return vr;
        }

        private void PreviewCompute()
        {
            ChildElementComboItem selectedStageDamage = _SelectedStageDamage();
            FrequencyElement freqElem = SelectedFrequencyElement.ChildElement as FrequencyElement;
            InflowOutflowElement inOutElem = SelectedInflowOutflowElement.ChildElement as InflowOutflowElement;
            StageDischargeElement ratElem = SelectedRatingCurveElement.ChildElement as StageDischargeElement;
            ExteriorInteriorElement extIntElem = SelectedExteriorInteriorElement.ChildElement as ExteriorInteriorElement;
            LateralStructureElement leveeElem = SelectedLeveeFeatureElement.ChildElement as LateralStructureElement;
            AggregatedStageDamageElement stageDamageElem = selectedStageDamage.ChildElement as AggregatedStageDamageElement;
            AggregatedStageDamageElement nonFailureStageDamageElem = null;
            if (_HasNonFailureStageDamage)
            {
                nonFailureStageDamageElem = SelectedNonFailureStageDamage.ChildElement as AggregatedStageDamageElement;
            }
            SimulationCreator sc = new(freqElem, inOutElem, ratElem, extIntElem, leveeElem, CurrentImpactArea.ID,
                HasFailureStageDamage, stageDamageElem, _HasNonFailureStageDamage, nonFailureStageDamageElem,
                HasFailureStageLifeLoss, null, HasNonFailureStageDamage, null);

            foreach (ThresholdRowItem thresholdRow in Thresholds)
            {
                Threshold threshold = thresholdRow.GetThreshold();
                sc.WithAdditionalThreshold(threshold);
            }

            //might need to be able to handle no stage damage here elegantly 

            ImpactAreaScenarioSimulation simulation = sc.BuildSimulation();
            ImpactAreaScenarioResults result = simulation.PreviewCompute();
            if (result == null)
            {
                MessageBox.Show("Preview Compute returned null result", "Failed Compute", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                EAD = result.ConsequenceResults.SampleMeanDamage(_selectedDamageCategory, _selectedAssetCategory, CurrentImpactArea.ID);
                _DamageFrequencyCurve = result.GetDamageFrequency(_selectedDamageCategory, _selectedAssetCategory);
            }


        }

        #region PlotCurves
        private PairedData GetFrequencyRelationshipFunction()
        {
            // Return null if no frequency element is selected
            if (SelectedFrequencyElement?.ChildElement is not FrequencyElement elem)
            {
                return new PairedData([], []); ;
            }

            // Handle analytical frequency elements
            //The Y label used in the Scenario editor plot comes from the metadata. We don't do a consistent
            //job using metadata throughout the compute, so i'm setting it manually here. Same for Graphical below. 
            if (elem.IsAnalytical)
            {
                UncertainPairedData upd = elem.LPIIIasUPD;
                upd.CurveMetaData.YLabel = StringConstants.DISCHARGE;
                PairedData apd = upd.SamplePairedData(-1, true);
                return apd;
            }

            // Handle graphical frequency elements. Graphical Frequencies sample always comes out in non-exceedence, so we have to convert here. 
            elem.GraphicalUncertainPairedData.CurveMetaData.YLabel = elem.GraphicalUsesFlow
                ? StringConstants.DISCHARGE
                : StringConstants.STAGE;

            PairedData nonExceedencePd = elem.GraphicalUncertainPairedData.SamplePairedData(-1, true);
            PairedData exceedencePd = new([.. nonExceedencePd.Xvals.Select(x => 1 - x)], nonExceedencePd.Yvals, nonExceedencePd.MetaData);
            exceedencePd.SortToIncreasingXVals();
            return exceedencePd;
        }

        private PairedData GetRatingCurveFunction()
        {
            // If we don't have a rating curve selected, return an empty paired data object: which means the chart remains with no function in it.
            if (SelectedRatingCurveElement == null || SelectedRatingCurveElement.ChildElement == null)
            {
                return new PairedData([], []);
            }

            // Retrieve the selected rating curve element and convert it to PairedData.
            CurveChildElement elem = (CurveChildElement)SelectedRatingCurveElement.ChildElement;
            return elem.CurveComponentVM.SelectedItemToPairedData().SamplePairedData(-1, true);
        }

        private PairedData GetStageDamageFunction()
        {
            // If no damage curve is selected, return an empty paired data object.
            if (SelectedDamageCurve == null)
            {
                return new PairedData([], []);
            }

            // Retrieve the selected stage damage curve and convert it to PairedData.
            UncertainPairedData retval = SelectedDamageCurve.ComputeComponent.SelectedItemToPairedData();
            return retval.SamplePairedData(-1, true);
        }

        private PairedData GetDamageFrequencyFunction()
        {
            // If no damage frequency curve is available, return null.
            if (_DamageFrequencyCurve == null)
            {
                return new PairedData([], []);
            }

            // Convert the damage frequency curve from non-exceedance to exceedance probabilities.
            double[] xs = _DamageFrequencyCurve.Xvals.Select(x => 1 - x).ToArray();
            double[] ys = _DamageFrequencyCurve.Yvals.ToArray();

            // Create metadata for the damage frequency curve.
            CurveMetaData curveMetaData = new CurveMetaData("Stage", "Damage", "Stage-Damage", "");

            // Create and return the paired data object.
            PairedData pd = new PairedData(xs, ys, curveMetaData);
            pd.SortToIncreasingXVals();
            return pd;
        }

        public void Plot()
        {
            FdaValidationResult validationResult = GetValidationResults();
            if (!validationResult.IsValid)
            {
                MessageBox.Show(validationResult.ErrorMessage.ToString(), "Insufficient Data", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            MessageRows.Clear();
            OverlappingRangeHelper.CheckForOverlappingRanges(SelectedFrequencyElement, SelectedInflowOutflowElement, SelectedRatingCurveElement,
                SelectedExteriorInteriorElement, (AggregatedStageDamageElement)_SelectedStageDamage.Invoke().ChildElement, SelectedDamageCurve, MessageRows);

            PreviewCompute();
            //get the current curves and set that data on the chart controls
            //this update call will set the current crosshair data on each one
            PlotControlVM.FrequencyRelationshipControl.Function = GetFrequencyRelationshipFunction();
            PlotControlVM.RatingRelationshipControl.Function = GetRatingCurveFunction();
            PlotControlVM.StageDamageControl.Function = GetStageDamageFunction();
            PairedData damageFrequencyCurve = GetDamageFrequencyFunction();

            if (damageFrequencyCurve == null)
            {
                MessageBox.Show("The compute failed to create a damage frequency curve", "No Damage-Frequency", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            PlotControlVM.DamageFrequencyControl.Function = damageFrequencyCurve;
            PlotControlVM.Plot();
            ShowWarnings = true;
            ShowEAD = true;

        }

        #endregion

        /// <summary>
        /// This method is used to convert the values in the editor into a SpecificIAS object. This object gets passed to the
        /// saving manager to save. Before calling this method make sure to check if it is valid.
        /// </summary>
        /// <returns></returns>
        public SpecificIAS CreateSpecificIAS()
        {
            ChildElementComboItem selectedStageDamage = _SelectedStageDamage();

            int flowFreqID = GetComboElementID(SelectedFrequencyElement);
            int inflowOutID = GetComboElementID(SelectedInflowOutflowElement);
            int ratingID = GetComboElementID(SelectedRatingCurveElement);
            int extIntID = GetComboElementID(SelectedExteriorInteriorElement);
            int latStructID = GetComboElementID(SelectedLeveeFeatureElement);
            int failureStageDamID = GetComboElementID(selectedStageDamage);
            int nonFailureStageDamID = GetComboElementID(SelectedNonFailureStageDamage);
            int failureStageLifeLossID = GetComboElementID(SelectedFailureStageLifeLoss);
            int nonFailureStageLifeLossID = GetComboElementID(SelectedNonFailureStageLifeLoss);

            List<ThresholdRowItem> thresholdRowItems = Thresholds;

            ConsequencesConfig config = new()
            {
                ConfigHasFailureStageDamage = HasFailureStageDamage,
                ConfigFailureStageDamageID = failureStageDamID,
                ConfigHasNonFailureStageDamage = HasNonFailureStageDamage,
                ConfigNonFailureStageDamageID = nonFailureStageDamID,
                ConfigHasFailureStageLifeLoss = HasFailureStageLifeLoss,
                ConfigFailureStageLifeLossID = failureStageLifeLossID,
                ConfigHasNonFailureStageLifeLoss = HasNonFailureStageLifeLoss,
                ConfigNonFailureStageLifeLossID = nonFailureStageLifeLossID
            };

            SpecificIAS elementToSave = new(CurrentImpactArea.ID, flowFreqID, inflowOutID, ratingID, extIntID,
                latStructID, thresholdRowItems, CalculateDefaultThresholdChecked, DefaultStage, config);
            return elementToSave;
        }

        private static int GetComboElementID(ChildElementComboItem comboItem)
        {
            return (comboItem != null && comboItem.ChildElement != null) ? comboItem.ChildElement.ID : -1;
        }

        private List<ThresholdRowItem> CloneCurrentThresholdsList()
        {
            List<ThresholdRowItem> currentThresholds = new();
            int i = 1;
            foreach (ThresholdRowItem thresh in Thresholds)
            {
                currentThresholds.Add(new ThresholdRowItem(thresh.ToXML(), i));
                i++;
            }
            return currentThresholds;
        }

        public void AddThresholds()
        {
            string header = "System Performance Thresholds";

            ThresholdsVM vm = new(CloneCurrentThresholdsList());
            DynamicTabVM tab = new(header, vm, "additionalThresholds", false, false);
            Navigate(tab, true, true);
            if (vm.IsThresholdsValid && vm.WasCanceled == false)
            {
                Thresholds = vm.Rows.ToList();
            }
        }

        public bool HasLeveeSelected()
        {
            return SelectedLeveeFeatureElement != null && SelectedLeveeFeatureElement.ChildElement != null;
        }
        public void UpdateDefaultStageRequired()
        {
            DefaultStageRequired = !CalculateDefaultThresholdChecked && !HasLeveeSelected();
        }
        private void UpdateThresholdStageValue()
        {
            if (HasLeveeSelected())
            {
                DefaultStage = ((LateralStructureElement)SelectedLeveeFeatureElement.ChildElement).Elevation;
                //disable the checkbox
                CalculateDefaultThresholdChecked = false;
                ScenarioReflectsEnabled = false;
            }
            else
            {
                //enable the checkbox
                ScenarioReflectsEnabled = true;
            }
        }
    }
}
