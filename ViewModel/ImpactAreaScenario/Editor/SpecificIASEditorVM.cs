using compute;
using HEC.CS.Collections;
using paireddata;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ViewModel.AggregatedStageDamage;
using ViewModel.FlowTransforms;
using ViewModel.FrequencyRelationships;
using ViewModel.GeoTech;
using ViewModel.ImpactArea;
using ViewModel.Saving;
using ViewModel.StageTransforms;
using ViewModel.Utilities;

namespace ViewModel.ImpactAreaScenario.Editor
{
    public class SpecificIASEditorVM : BaseViewModel
    {
        private ThresholdsVM _additionalThresholdsVM;
        private ChildElementComboItem _selectedStageDamageElement;
        private StageDamageCurve _selectedDamageCurve;
        private ChildElementComboItem _selectedFrequencyRelationship;
        private bool _ratingRequired;
        private bool _showWarnings;
        private ChildElementComboItem _selectedInflowOutflowElement;
        private ChildElementComboItem _selectedRatingCurveElement;
        private ChildElementComboItem _selectedLeveeElement;
        private ChildElementComboItem _selectedExteriorInteriorElement;
        private bool _showEAD;
        private double _EAD;

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
        public List<ThresholdRowItem> Thresholds { get; } = new List<ThresholdRowItem>();
        public CustomObservableCollection<ChildElementComboItem> FrequencyElements { get; } = new CustomObservableCollection<ChildElementComboItem>();
        public CustomObservableCollection<ChildElementComboItem> InflowOutflowElements { get; } = new CustomObservableCollection<ChildElementComboItem>();
        public CustomObservableCollection<ChildElementComboItem> RatingCurveElements { get; } = new CustomObservableCollection<ChildElementComboItem>();
        public CustomObservableCollection<ChildElementComboItem> LeveeFeatureElements { get; } = new CustomObservableCollection<ChildElementComboItem>();
        public CustomObservableCollection<ChildElementComboItem> ExteriorInteriorElements { get; } = new CustomObservableCollection<ChildElementComboItem>();
        public CustomObservableCollection<ChildElementComboItem> StageDamageElements { get; } = new CustomObservableCollection<ChildElementComboItem>();

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
        public ChildElementComboItem SelectedStageDamageElement
        {
            get { return _selectedStageDamageElement; }
            set { _selectedStageDamageElement = value; StageDamageSelectionChanged(); }
        }

        /// <summary>
        /// The rows that show up in the "Warnings" expander after hitting the plot button.
        /// </summary>
        public ObservableCollection<RecommendationRowItem> MessageRows { get; set; }

        /// <summary>
        /// This is the create new ctor
        /// </summary>
        public SpecificIASEditorVM(ImpactAreaRowItem rowItem)
        {
            Initialize();
            CurrentImpactArea = rowItem;
        }

        public SpecificIASEditorVM(SpecificIAS elem, ImpactAreaRowItem rowItem)
        {
            Initialize();
            CurrentImpactArea = rowItem;
            FillForm(elem);
        }

        private void Initialize()
        {
            MessageRows = new ObservableCollection<RecommendationRowItem>();
            _additionalThresholdsVM = new ThresholdsVM();
            _additionalThresholdsVM.RequestNavigation += Navigate;

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

            StudyCache.StageDamageAdded += AddStageDamageElement;
            StudyCache.StageDamageRemoved += RemoveStageDamageElement;
            StudyCache.StageDamageUpdated += UpdateStageDamageElement;
        }


        #region Live Update Event Methods

        private void AddStageDamageElement(object sender, ElementAddedEventArgs e)
        {
            StageDamageElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveStageDamageElement(object sender, ElementAddedEventArgs e)
        {
            removeElement(e.ID, StageDamageElements);
            SelectedStageDamageElement = StageDamageElements[0];
        }
        private void UpdateStageDamageElement(object sender, ElementUpdatedEventArgs e)
        {
            updateElement(StageDamageElements, SelectedStageDamageElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }

        private void AddExtIntElement(object sender, ElementAddedEventArgs e)
        {
            ExteriorInteriorElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveExtIntElement(object sender, ElementAddedEventArgs e)
        {
            removeElement(e.ID, ExteriorInteriorElements);
            SelectedExteriorInteriorElement = ExteriorInteriorElements[0];
        }
        private void UpdateExtIntElement(object sender, ElementUpdatedEventArgs e)
        {
            updateElement(ExteriorInteriorElements, SelectedExteriorInteriorElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }
        private void AddLeveeElement(object sender, ElementAddedEventArgs e)
        {
            LeveeFeatureElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveLeveeElement(object sender, ElementAddedEventArgs e)
        {
            removeElement(e.ID, LeveeFeatureElements);
            SelectedLeveeFeatureElement = LeveeFeatureElements[0];
        }
        private void UpdateLeveeElement(object sender, ElementUpdatedEventArgs e)
        {
            updateElement(LeveeFeatureElements, SelectedLeveeFeatureElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }

        private void AddInOutElement(object sender, ElementAddedEventArgs e)
        {
            InflowOutflowElements.Add(new ChildElementComboItem((ChildElement)e.Element));
            SelectedInflowOutflowElement = InflowOutflowElements[0];
        }
        private void RemoveInOutElement(object sender, ElementAddedEventArgs e)
        {
            removeElement(e.ID, InflowOutflowElements);
        }
        private void UpdateInOutElement(object sender, ElementUpdatedEventArgs e)
        {
            updateElement(InflowOutflowElements, SelectedInflowOutflowElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }
        private void AddFlowFreqElement(object sender, ElementAddedEventArgs e)
        {
            FrequencyElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveFlowFreqElement(object sender, ElementAddedEventArgs e)
        {
            removeElement(e.ID, FrequencyElements);
            SelectedFrequencyElement = FrequencyElements[0];
        }
        private void UpdateFlowFreqElement(object sender, ElementUpdatedEventArgs e)
        {
            updateElement(FrequencyElements, SelectedFrequencyElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }
        private void AddRatingElement(object sender, ElementAddedEventArgs e)
        {
            RatingCurveElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveRatingElement(object sender, ElementAddedEventArgs e)
        {
            removeElement(e.ID, RatingCurveElements);
            SelectedRatingCurveElement = RatingCurveElements[0];
        }
        private void UpdateRatingElement(object sender, ElementUpdatedEventArgs e)
        {
            updateElement(RatingCurveElements, SelectedRatingCurveElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }
        private void removeElement(int idToRemove, ObservableCollection<ChildElementComboItem> collection)
        {
            collection.Remove(collection.Where(elem => elem.ChildElement != null && elem.ID == idToRemove).Single());
        }
        private void updateElement(ObservableCollection<ChildElementComboItem> collection, ChildElementComboItem selectedItem,
             ChildElement oldElement, ChildElement newElement)
        {
            int idToUpdate = oldElement.GetElementID();

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
            SelectedFrequencyElement = FrequencyElements.FirstOrDefault(freq => freq.ChildElement != null && freq.ChildElement.GetElementID() == elem.FlowFreqID);
            SelectedInflowOutflowElement = InflowOutflowElements.FirstOrDefault(inf => inf.ChildElement != null && inf.ChildElement.GetElementID() == elem.InflowOutflowID);
            SelectedRatingCurveElement = RatingCurveElements.FirstOrDefault(rat => rat.ChildElement != null && rat.ChildElement.GetElementID() == elem.RatingID);
            SelectedLeveeFeatureElement = LeveeFeatureElements.FirstOrDefault(levee => levee.ChildElement != null && levee.ChildElement.GetElementID() == elem.LeveeFailureID);
            SelectedExteriorInteriorElement = ExteriorInteriorElements.FirstOrDefault(ext => ext.ChildElement != null && ext.ChildElement.GetElementID() == elem.ExtIntStageID);
            SelectedStageDamageElement = StageDamageElements.FirstOrDefault(stage => stage.ChildElement != null && stage.ChildElement.GetElementID() == elem.StageDamageID);

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
            if (SelectedStageDamageElement == null)
            {
                SelectedStageDamageElement = StageDamageElements[0];
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

            List<AnalyticalFrequencyElement> analyticalFrequencyElements = StudyCache.GetChildElementsOfType<AnalyticalFrequencyElement>();
            childElems.Clear();
            childElems.AddRange(analyticalFrequencyElements);
            FrequencyElements.AddRange(CreateComboItems(childElems));
            SelectedFrequencyElement = FrequencyElements.First();

            List<InflowOutflowElement> inflowOutflowElements = StudyCache.GetChildElementsOfType<InflowOutflowElement>();
            childElems.Clear();
            childElems.AddRange(inflowOutflowElements);
            InflowOutflowElements.AddRange(CreateComboItems(childElems));
            SelectedInflowOutflowElement = InflowOutflowElements.First();

            List<RatingCurveElement> ratingCurveElements = StudyCache.GetChildElementsOfType<RatingCurveElement>();
            childElems.Clear();
            childElems.AddRange(ratingCurveElements);
            RatingCurveElements.AddRange(CreateComboItems(childElems));
            SelectedRatingCurveElement = RatingCurveElements.First();

            List<LeveeFeatureElement> leveeFeatureElements = StudyCache.GetChildElementsOfType<LeveeFeatureElement>();
            childElems.Clear();
            childElems.AddRange(leveeFeatureElements);
            LeveeFeatureElements.AddRange(CreateComboItems(childElems));
            SelectedLeveeFeatureElement = LeveeFeatureElements.First();

            List<ExteriorInteriorElement> exteriorInteriorElements = StudyCache.GetChildElementsOfType<ExteriorInteriorElement>();
            childElems.Clear();
            childElems.AddRange(exteriorInteriorElements);
            ExteriorInteriorElements.AddRange(CreateComboItems(childElems));
            SelectedExteriorInteriorElement = ExteriorInteriorElements.First();

            List<AggregatedStageDamageElement> aggregatedStageDamageElements = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            childElems.Clear();
            childElems.AddRange(aggregatedStageDamageElements);
            StageDamageElements.AddRange(CreateComboItems(childElems));
            SelectedStageDamageElement = StageDamageElements.First();
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
            if (SelectedFrequencyElement != null && SelectedFrequencyElement.ChildElement != null)
            {
                //todo: we need to check to see if this is a flow freq.
                //if the flow freq is of type flow freq, then the rating curve is required.
                //this is task 5 in the clean document.
                RatingRequired = true;
            }
        }

        /// <summary>
        /// When the stage damage selection is changed we need to update the dam cats that go into
        /// the combo next to the plot button.
        /// </summary>
        private void StageDamageSelectionChanged()
        {
            if (SelectedStageDamageElement != null && SelectedStageDamageElement.ChildElement != null)
            {
                List<StageDamageCurve> stageDamageCurves = GetStageDamageCurves();

                DamageCategories.Clear();
                DamageCategories.AddRange(stageDamageCurves);
                if (DamageCategories.Count > 0)
                {
                    SelectedDamageCurve = DamageCategories[0];
                }
            }
        }

        //todo: check that these selected items aren't null?
        private List<StageDamageCurve> GetStageDamageCurves()
        {
            AggregatedStageDamageElement elem = (AggregatedStageDamageElement)SelectedStageDamageElement.ChildElement;
            List<StageDamageCurve> stageDamageCurves = elem.Curves.Where(curve => curve.ImpArea.ID == CurrentImpactArea.ID).ToList();
            return stageDamageCurves;
        }

        #region validation

        private string IsFrequencyRelationshipValid()
        {
            return SelectedFrequencyElement.ChildElement == null ? "A Frequency Relationship is required." : null;      
        }

        private string IsRatingCurveValid()
        {
            //todo: the rating curve is required if the frequency relationship is of type
            //flow-frequency. This will need to get added once we complete task 5 in the clean doc.
            string msg = null;
            if (_ratingRequired && SelectedRatingCurveElement.ChildElement == null)
            {
                msg = "A Rating Curve is required when using a flow-frequency relationship.";
            }
            return msg;
        }
        private string IsStageDamageValid()
        {
            return SelectedStageDamageElement.ChildElement == null ? "A Stage Damage is required." : null;
        }


        #endregion


        /// <summary>
        /// This method checks to see if this specific IAS is valid for both saving and plotting.
        /// </summary>
        /// <returns></returns>
        public FdaValidationResult IsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();

            vr.AddErrorMessage(IsFrequencyRelationshipValid());
            vr.AddErrorMessage(IsRatingCurveValid());
            vr.AddErrorMessage(IsStageDamageValid());

            if (!vr.IsValid)
            {
                vr.InsertNewLineMessage(0, "Errors in Impact Area: " + CurrentImpactArea.Name);
            }
            return vr;
        }

        private void PreviewCompute()
        {
            AnalyticalFrequencyElement freqElem = SelectedFrequencyElement.ChildElement as AnalyticalFrequencyElement;
            InflowOutflowElement inOutElem = SelectedInflowOutflowElement.ChildElement as InflowOutflowElement;
            RatingCurveElement ratElem = SelectedRatingCurveElement.ChildElement as RatingCurveElement;
            ExteriorInteriorElement extIntElem = SelectedExteriorInteriorElement.ChildElement as ExteriorInteriorElement;
            LeveeFeatureElement leveeElem = SelectedLeveeFeatureElement.ChildElement as LeveeFeatureElement;
            AggregatedStageDamageElement stageDamageElem = SelectedStageDamageElement.ChildElement as AggregatedStageDamageElement;

            SimulationCreator sc = new SimulationCreator(freqElem, inOutElem, ratElem, extIntElem, leveeElem,
                stageDamageElem, CurrentImpactArea.ID);

            Simulation simulation = sc.BuildSimulation();

            MeanRandomProvider mrp = new MeanRandomProvider();
            try
            {
                //metrics.Results result = simulation.Compute(mrp, 1);
                //Console.WriteLine("Mean ead: " + result.ExpectedAnnualDamageResults.MeanEAD("InteriorStageDamage"));
                //double ead = result.ExpectedAnnualDamageResults.MeanEAD("InteriorStageDamage");
                //double total = result.ExpectedAnnualDamageResults.MeanEAD("Total");
                //int i = 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private List<UncertainPairedData> GetStageDamagesAsPairedData()
        {
            List<UncertainPairedData> stageDamages = new List<UncertainPairedData>();
            List<StageDamageCurve> stageDamageCurves = GetStageDamageCurves();
            foreach (StageDamageCurve curve in stageDamageCurves)
            {
                stageDamages.Add(curve.Function);
            }
            return stageDamages;
        }


        #region PlotCurves
        private UncertainPairedData getFrequencyRelationshipFunction()
        {
            UncertainPairedData retval = null;
            if (SelectedFrequencyElement != null && SelectedFrequencyElement.ChildElement != null)
            {
                retval = SelectedFrequencyElement.ChildElement.Curve;
            }

            return retval;
        }

        private UncertainPairedData getRatingCurveFunction()
        {
            UncertainPairedData retval = null;
            if (SelectedRatingCurveElement != null && SelectedRatingCurveElement.ChildElement != null)
            {
                retval = SelectedRatingCurveElement.ChildElement.Curve;
            }

            return retval;
        }

        private UncertainPairedData getStageDamageFunction()
        {
            UncertainPairedData retval = null;
            if (SelectedDamageCurve != null)
            {
                retval = SelectedDamageCurve.Function;
            }
            return retval;
        }

        private UncertainPairedData getDamageFrequencyFunction()
        {
            //todo: this will be the result from the compute. I don't think we need this method once the compute is happening.
            double[] xs = new double[10];
            Normal[] ys = new Normal[10];
            for (int i = 0; i < 10; i++)
            {
                xs[i] = i;
                ys[i] = new Normal(i, 0);
            }
            UncertainPairedData curve = new UncertainPairedData(xs, ys, "Stage", "Damage", "Stage-Damage", "", -1);
            return curve;

        }

        public void Plot()
        {
            FdaValidationResult validationResult = IsValid();
            if (validationResult.IsValid)
            {
                MessageRows.Clear();
                OverlappingRangeHelper.CheckForOverlappingRanges(SelectedFrequencyElement, SelectedInflowOutflowElement, SelectedRatingCurveElement,
                    SelectedExteriorInteriorElement, (AggregatedStageDamageElement)SelectedStageDamageElement.ChildElement, SelectedDamageCurve, MessageRows);

                PreviewCompute();

                //get the current curves and set that data on the chart controls
                //this update call will set the current crosshair data on each one
                PlotControlVM.FrequencyRelationshipControl.UpdatePlotData(getFrequencyRelationshipFunction());
                PlotControlVM.RatingRelationshipControl.UpdatePlotData(getRatingCurveFunction());
                PlotControlVM.StageDamageControl.UpdatePlotData(getStageDamageFunction());
                PlotControlVM.DamageFrequencyControl.UpdatePlotData(getDamageFrequencyFunction());

                PlotControlVM.Plot();
                ShowWarnings = true;
                EAD = .123;
                ShowEAD = true;
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
            int flowFreqID = GetComboElementID(SelectedFrequencyElement);
            int inflowOutID = GetComboElementID(SelectedInflowOutflowElement);
            int ratingID = GetComboElementID(SelectedRatingCurveElement);
            int extIntID = GetComboElementID(SelectedExteriorInteriorElement);
            int latStructID = GetComboElementID(SelectedLeveeFeatureElement);
            int stageDamID = GetComboElementID(SelectedStageDamageElement);

            List<ThresholdRowItem> thresholdRowItems = _additionalThresholdsVM.GetThresholds();

            SpecificIAS elementToSave = new SpecificIAS(CurrentImpactArea.ID,
            flowFreqID, inflowOutID,
            ratingID, extIntID, latStructID, stageDamID, thresholdRowItems);
            return elementToSave;
        }

        private int GetComboElementID(ChildElementComboItem comboItem)
        {
            return (comboItem != null && comboItem.ChildElement != null) ? comboItem.ChildElement.GetElementID() : -1;
        }

        public void AddThresholds()
        {
            string header = "Annual Exceedance Probabilities Thresholds";
            DynamicTabVM tab = new DynamicTabVM(header, _additionalThresholdsVM, "additionalThresholds");
            Navigate(tab, true, true);
            Thresholds.Clear();
            Thresholds.AddRange(_additionalThresholdsVM.GetThresholds());
        }
    }
}
