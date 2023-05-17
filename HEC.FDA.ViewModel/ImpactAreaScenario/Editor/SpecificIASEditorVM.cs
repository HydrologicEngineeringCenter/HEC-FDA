using HEC.CS.Collections;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using Statistics;
using HEC.MVVMFramework.Base.Events;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.compute;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class SpecificIASEditorVM : BaseViewModel
    {
        private ThresholdsVM _additionalThresholdsVM;
        private StageDamageCurve _selectedDamageCurve;
        private string _selectedAssetCategory;
        private ChildElementComboItem _selectedFrequencyRelationship;
        private bool _ratingRequired;
        private bool _showWarnings;
        private ChildElementComboItem _selectedInflowOutflowElement;
        private ChildElementComboItem _selectedRatingCurveElement;
        private ChildElementComboItem _selectedLeveeElement;
        private ChildElementComboItem _selectedExteriorInteriorElement;
        private bool _showEAD;
        private double _EAD;
        private PairedData _DamageFrequencyCurve = null;

        private Func<ChildElementComboItem> _SelectedStageDamage;

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

        public int Year { get; set; } = DateTime.Now.Year;
        public CustomObservableCollection<StageDamageCurve> DamageCategories { get; } = new CustomObservableCollection<StageDamageCurve>();
        public CustomObservableCollection<string> AssetCategories { get; } = new CustomObservableCollection<string>();
        public List<ThresholdRowItem> Thresholds { get; } = new List<ThresholdRowItem>();
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

        public StageDamageCurve SelectedDamageCurve
        {
            get { return _selectedDamageCurve; }
            set { _selectedDamageCurve = value; NotifyPropertyChanged(); }
        }
        public ChildElementComboItem SelectedFrequencyElement
        {
            get { return _selectedFrequencyRelationship; }
            set { _selectedFrequencyRelationship = value; UpdateRatingRequired(); }
        }
        public ChildElementComboItem SelectedInflowOutflowElement
        {
            get { return _selectedInflowOutflowElement; }
            set { _selectedInflowOutflowElement = value; NotifyPropertyChanged(); }
        }
        public ChildElementComboItem SelectedRatingCurveElement
        {
            get { return _selectedRatingCurveElement; }
            set { _selectedRatingCurveElement = value; NotifyPropertyChanged(); }
        }
        public ChildElementComboItem SelectedLeveeFeatureElement
        {
            get { return _selectedLeveeElement; }
            set { _selectedLeveeElement = value; NotifyPropertyChanged(); }
        }
        public ChildElementComboItem SelectedExteriorInteriorElement
        {
            get { return _selectedExteriorInteriorElement; }
            set { _selectedExteriorInteriorElement = value; NotifyPropertyChanged(); }
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
            Initialize();
            CurrentImpactArea = rowItem;
            _SelectedStageDamage = getSelectedStageDamage;
        }

        public SpecificIASEditorVM(SpecificIAS elem, ImpactAreaRowItem rowItem, Func<ChildElementComboItem> getSelectedStageDamage)
        {
            Initialize();
            CurrentImpactArea = rowItem;
            FillForm(elem);
            _SelectedStageDamage = getSelectedStageDamage;
        }

        private void Initialize()
        {
            _additionalThresholdsVM = new ThresholdsVM();

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
            FillThresholds(elem);
            //all the available elements have been loaded into this editor. We now want to select
            //the correct element for each dropdown. 
            //all the available elements have been loaded into this editor. We now want to select
            //the correct element for each dropdown. If we can't find the correct element then the selected elem 
            //will be null.
            SelectedFrequencyElement = FrequencyElements.FirstOrDefault(freq => freq.ChildElement != null && freq.ChildElement.ID == elem.FlowFreqID);
            SelectedInflowOutflowElement = InflowOutflowElements.FirstOrDefault(inf => inf.ChildElement != null && inf.ChildElement.ID == elem.InflowOutflowID);
            SelectedRatingCurveElement = RatingCurveElements.FirstOrDefault(rat => rat.ChildElement != null && rat.ChildElement.ID == elem.RatingID);
            SelectedLeveeFeatureElement = LeveeFeatureElements.FirstOrDefault(levee => levee.ChildElement != null && levee.ChildElement.ID == elem.LeveeFailureID);
            SelectedExteriorInteriorElement = ExteriorInteriorElements.FirstOrDefault(ext => ext.ChildElement != null && ext.ChildElement.ID == elem.ExtIntStageID);

            //i don't want a selected value to ever be null. Even if there are no elements we should select the blank row option.
            //so if it is null, i will set it to the first option which is empty.
            if (SelectedFrequencyElement == null)
            {
                SelectedFrequencyElement = FrequencyElements[0];
            }
            if (SelectedInflowOutflowElement == null)
            {
                SelectedInflowOutflowElement = InflowOutflowElements[0];
            }
            if (SelectedRatingCurveElement == null)
            {
                SelectedRatingCurveElement = RatingCurveElements[0];
            }
            if (SelectedLeveeFeatureElement == null)
            {
                SelectedLeveeFeatureElement = LeveeFeatureElements[0];
            }
            if (SelectedExteriorInteriorElement == null)
            {
                SelectedExteriorInteriorElement = ExteriorInteriorElements[0];
            }
            
        }

        private void FillThresholds(SpecificIAS elem)
        {
            _additionalThresholdsVM.AddRows(elem.Thresholds);
            Thresholds.AddRange(elem.Thresholds);
        }

        private void LoadElements()
        {
            //what happens if there are no elements for a combo?
            //I will always add an empty ChildElementComboItem and then select it by default.
            //this means that when asking for the selected combo item, it should never be null.
            List<ChildElement> childElems = new List<ChildElement>();

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

        private ObservableCollection<ChildElementComboItem> CreateComboItems(List<ChildElement> elems)
        {
            ObservableCollection<ChildElementComboItem> items = new ObservableCollection<ChildElementComboItem>();
            items.Add(new ChildElementComboItem(null));
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

                DamageCategories.Clear();
                DamageCategories.AddRange(stageDamageCurves);
                if (DamageCategories.Count > 0)
                {
                    SelectedDamageCurve = DamageCategories[0];
                }
            }
            else
            {
                //the user selected the blank row. Clear the damage category combo
                DamageCategories.Clear();
            }
        }

        private void LoadAssetCategories(List<StageDamageCurve> stageDamageCurves)
        {
            List<string> assetCategories = new List<string>();
            foreach(StageDamageCurve curve in stageDamageCurves)
            {
                assetCategories.Add(curve.AssetCategory);
            }
            AssetCategories.AddRange(assetCategories.Distinct());
            if(AssetCategories.Count > 0)
            {
                SelectedAssetCategory = AssetCategories[0];
            }
        }

        private List<StageDamageCurve> GetStageDamageCurves()
        {
            ChildElementComboItem selectedStageDamage = _SelectedStageDamage();
            List<StageDamageCurve> stageDamageCurves = new List<StageDamageCurve>();
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
            FdaValidationResult vr = new FdaValidationResult();
            if(SelectedFrequencyElement.ChildElement == null)
            {
                vr.AddErrorMessage("A Frequency Relationship is required.");
            }
            return vr;
        }

        private FdaValidationResult GetRatingCurveValidationResult()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (_ratingRequired && SelectedRatingCurveElement.ChildElement == null)
            {
                vr.AddErrorMessage("A stage-discharge function is required if the frequency function reflects discharge");
            }
            return vr;
        }
        private FdaValidationResult GetStageDamageValidationResult()
        {
            ChildElementComboItem selectedStageDamage = _SelectedStageDamage();
            FdaValidationResult vr = new FdaValidationResult();
            if (selectedStageDamage.ChildElement == null)
            {
                vr.AddErrorMessage("A Stage Damage is required. ");
            }
            return vr;
        }

        private FdaValidationResult GetDamageCurveSelectedValidationResult()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (SelectedDamageCurve == null)
            {
                vr.AddErrorMessage("A damage category selection is required.");
            }
            return vr;
        }

        #endregion

        public FdaValidationResult GetPlotValidationResults()
        {
            FdaValidationResult vr = new FdaValidationResult();

            vr.AddErrorMessage(GetFrequencyRelationshipValidationResult().ErrorMessage);
            vr.AddErrorMessage(GetRatingCurveValidationResult().ErrorMessage);
            vr.AddErrorMessage(GetStageDamageValidationResult().ErrorMessage);
            vr.AddErrorMessage(GetDamageCurveSelectedValidationResult().ErrorMessage);

            if (!vr.IsValid)
            {
                vr.InsertMessage(0, "Errors in Impact Area: " + CurrentImpactArea.Name);
            }
            return vr;
        }

        /// <summary>
        /// This method checks to see if this specific IAS is valid for both saving and plotting.
        /// </summary>
        /// <returns></returns>
        public FdaValidationResult GetEditorValidationResult()
        {
            FdaValidationResult vr = new FdaValidationResult();

            vr.AddErrorMessage(GetFrequencyRelationshipValidationResult().ErrorMessage);
            vr.AddErrorMessage(GetRatingCurveValidationResult().ErrorMessage);
            vr.AddErrorMessage(GetStageDamageValidationResult().ErrorMessage);

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

            SimulationCreator sc = new SimulationCreator(freqElem, inOutElem, ratElem, extIntElem, leveeElem,
                stageDamageElem, CurrentImpactArea.ID);

            foreach (ThresholdRowItem thresholdRow in Thresholds)
            {
                Threshold threshold = thresholdRow.GetThreshold();
                sc.WithAdditionalThreshold(threshold);
            }

            FdaValidationResult configurationValidationResult = sc.IsConfigurationValid();
            if(configurationValidationResult.IsValid)
            {
                ImpactAreaScenarioSimulation simulation = sc.BuildSimulation();
                simulation.MessageReport += MyMessageHandler;
                MedianRandomProvider randomProvider = new MedianRandomProvider();
                ConvergenceCriteria cc = StudyCache.GetStudyPropertiesElement().GetStudyConvergenceCriteria();
                try
                {
                    ImpactAreaScenarioResults result = simulation.PreviewCompute();
                    EAD = result.ConsequenceResults.MeanDamage(_selectedDamageCurve.DamCat, _selectedAssetCategory, CurrentImpactArea.ID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed Compute", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(configurationValidationResult.ErrorMessage, "Invalid Setup", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
    
        }

        public void MyMessageHandler(object sender, MessageEventArgs e)
        {
            //The following 3 messages are coming into here.
            //default
            //Ead message
            //total

            if (e.Message is FrequencyDamageMessage damageMessage)
            {
                //todo: not sure that this is correct. Maybe we want the "total" one, but in the current case the "total" has no values?
                if(e.Message.Message.Equals("FrequencyDamage"))
                {
                    _DamageFrequencyCurve = damageMessage.FrequencyDamage;
                }
            }
        }

        #region PlotCurves
        private IPairedDataProducer getFrequencyRelationshipFunction()
        {
            IPairedDataProducer retval = null;
            if (SelectedFrequencyElement != null && SelectedFrequencyElement.ChildElement != null)
            {
                if(SelectedFrequencyElement.ChildElement is FrequencyElement elem)
                {
                    if (elem.IsAnalytical)
                    {
                        retval = elem.LPIIIasUPD;
                    }
                    else
                    {
                        retval = elem.GraphicalUncertainPairedData;
                    }
                }
            }
            return retval;
        }

        private UncertainPairedData getRatingCurveFunction()
        {
            UncertainPairedData retval = null;
            if (SelectedRatingCurveElement != null && SelectedRatingCurveElement.ChildElement != null)
            {
                CurveChildElement elem = (CurveChildElement)SelectedRatingCurveElement.ChildElement;
                retval = elem.CurveComponentVM.SelectedItemToPairedData();
            }

            return retval;
        }

        private UncertainPairedData getStageDamageFunction()
        {      
            UncertainPairedData retval = null;
            if (SelectedDamageCurve != null)
            {
                retval = SelectedDamageCurve.ComputeComponent.SelectedItemToPairedData();
            }
            return retval;
        }

        private UncertainPairedData getDamageFrequencyFunction()
        {
            UncertainPairedData curve = null;
            if (_DamageFrequencyCurve != null)
            {
                double[] xs = _DamageFrequencyCurve.Xvals;
                double[] ys = _DamageFrequencyCurve.Yvals;
                IDistribution[] yDists = new IDistribution[ys.Length];
                for (int i = 0; i < ys.Length; i++)
                {
                    yDists[i] = new Deterministic(ys[i]);
                }

                curve = new UncertainPairedData(xs, yDists, "Stage", "Damage", "Stage-Damage", "");
            }
            return curve;
        }

        public void Plot()
        {
            FdaValidationResult validationResult = GetPlotValidationResults();
            if (validationResult.IsValid)
            {
                MessageRows.Clear();
                OverlappingRangeHelper.CheckForOverlappingRanges(SelectedFrequencyElement, SelectedInflowOutflowElement, SelectedRatingCurveElement,
                    SelectedExteriorInteriorElement, (AggregatedStageDamageElement)_SelectedStageDamage.Invoke().ChildElement, SelectedDamageCurve, MessageRows);

                PreviewCompute();

                //get the current curves and set that data on the chart controls
                //this update call will set the current crosshair data on each one
                PlotControlVM.FrequencyRelationshipControl.UpdatePlotData(getFrequencyRelationshipFunction());
                PlotControlVM.RatingRelationshipControl.UpdatePlotData(getRatingCurveFunction());
                PlotControlVM.StageDamageControl.UpdatePlotData(getStageDamageFunction());

                UncertainPairedData damageFrequencyCurve = getDamageFrequencyFunction();
                if (damageFrequencyCurve != null)
                {
                    PlotControlVM.DamageFrequencyControl.UpdatePlotData(getDamageFrequencyFunction());

                    PlotControlVM.Plot();
                    ShowWarnings = true;
                    ShowEAD = true;
                }
                else
                {
                    MessageBox.Show("The compute failed to create a damage frequency curve", "No Damage-Frequency", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show(validationResult.ErrorMessage.ToString(), "Insufficient Data", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        #endregion

        /// <summary>
        /// This method is used to convert the values in the editor into an SpecificIAS object. This object gets passed to the
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
            int stageDamID = GetComboElementID(selectedStageDamage);

            List<ThresholdRowItem> thresholdRowItems = _additionalThresholdsVM.GetThresholds();

            SpecificIAS elementToSave = new SpecificIAS(CurrentImpactArea.ID,
            flowFreqID, inflowOutID,
            ratingID, extIntID, latStructID, stageDamID, thresholdRowItems);
            return elementToSave;
        }

        private int GetComboElementID(ChildElementComboItem comboItem)
        {
            return (comboItem != null && comboItem.ChildElement != null) ? comboItem.ChildElement.ID : -1;
        }

        public void AddThresholds()
        {
            string header = "System Performance Thresholds";
            DynamicTabVM tab = new DynamicTabVM(header, _additionalThresholdsVM, "additionalThresholds",false,false);
            Navigate(tab, true, true);
            Thresholds.Clear();
            Thresholds.AddRange(_additionalThresholdsVM.GetThresholds());
        }
    }
}
