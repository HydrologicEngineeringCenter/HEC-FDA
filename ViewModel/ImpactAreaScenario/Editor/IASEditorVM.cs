using Functions;
using FunctionsView.ViewModel;
using HEC.Plotting.Core;
using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.Controller;
using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModel.AggregatedStageDamage;
using ViewModel.Editors;
using ViewModel.FlowTransforms;
using ViewModel.FrequencyRelationships;
using ViewModel.GeoTech;
using ViewModel.ImpactArea;
using ViewModel.ImpactAreaScenario.Editor.ChartControls;
using ViewModel.StageTransforms;
using ViewModel.Utilities;

namespace ViewModel.ImpactAreaScenario.Editor
{
    public class IASEditorVM : BaseEditorVM
    {
        private IASElement _currentElement;
        private bool _isInEditMode;
        private Chart2DController _controller;
        private AdditionalThresholdsVM _additionalThresholdsVM;
        private ChildElementComboItem _selectedStageDamageElement;
        private List<string> _damageCategories;
        private string _selectedDamageCategory;
        private ChildElementComboItem _selectedFrequencyRelationship;
        private bool _ratingRequired;


        //public SciChart2DChartViewModel FlowFreqChartVM { get; set; } = new SciChart2DChartViewModel("Flow Frequency");
        public ChartControlBase FrequencyRelationshipControl { get; set; }
        public ChartControlBase RatingRelationshipControl { get; set; }
        public ChartControlBase StageDamageControl { get; set; }
        public ChartControlBase DamageFrequencyControl { get; set; }

        public bool RatingRequired
        {
            get { return _ratingRequired; }
            set { _ratingRequired = value; NotifyPropertyChanged(); }
        }


        //public SciChart2DChartViewModel RatingChartVM { get; set; } = new SciChart2DChartViewModel("Rating Curve");
        //public SciChart2DChartViewModel StageDamageChartVM { get; set; } = new SciChart2DChartViewModel("Stage Damage");
        //public SciChart2DChartViewModel DamageFreqChartVM { get; set; } = new SciChart2DChartViewModel("Damage Frequency");
        public int Year { get; set; } = DateTime.Now.Year;


        public List<string> DamageCategories
        {
            get { return _damageCategories; }
            set { _damageCategories = value; NotifyPropertyChanged(); }
        }
        public List<AdditionalThresholdRowItem> Thresholds { get; set; }
        public ObservableCollection<ChildElementComboItem> ImpactAreaElements { get; set; }
        public ObservableCollection<ChildElementComboItem> FrequencyElements { get; set; }
        public ObservableCollection<ChildElementComboItem> InflowOutflowElements { get; set; }
        //public List<RatingCurveElement> RatingCurveElements { get; set; }
        public ObservableCollection<ChildElementComboItem> RatingCurveElements { get; set; }

        public ObservableCollection<ChildElementComboItem> LeveeFeatureElements { get; set; }
        public ObservableCollection<ChildElementComboItem> ExteriorInteriorElements { get; set; }
        public ObservableCollection<ChildElementComboItem> StageDamageElements { get; set; }


        private ChildElementComboItem _selectedImpactAreaElement;
        private ChildElementComboItem _selectedInflowOutflowElement;
        private ChildElementComboItem _selectedRatingCurveElement;
        private ChildElementComboItem _selectedLeveeElement;
        private ChildElementComboItem _selectedExteriorInteriorElement;

        public string SelectedDamageCategory
        {
            get { return _selectedDamageCategory; }
            set { _selectedDamageCategory = value; NotifyPropertyChanged(); }
        }
        public ChildElementComboItem SelectedImpactAreaElement
        {
            get { return _selectedImpactAreaElement; }
            set { _selectedImpactAreaElement = value; NotifyPropertyChanged(); }
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
            set { _selectedStageDamageElement = value;  StageDamageSelectionChanged(); }
        }

        /// <summary>
        /// This is the create new ctor
        /// </summary>
        public IASEditorVM(EditorActionManager manager):base(manager)
        {
            Initialize();
            
        }

        //todo: this ctor probably needs some work
        public IASEditorVM(IASElement elem, EditorActionManager manager) : base(elem, manager)
        {
            _currentElement = elem;
            _isInEditMode = true;
            Initialize();
            FillForm(elem);
        }

        private void Initialize()
        {
            FrequencyRelationshipControl = new FrequencyRelationshipControl();
            RatingRelationshipControl = new RatingRelationshipControl();
            StageDamageControl = new StageDamageControl();
            DamageFrequencyControl = new DamageFrequencyControl();

            //hook up the navigate event for the additional thresholds dialog
            _additionalThresholdsVM = new AdditionalThresholdsVM();
            _additionalThresholdsVM.RequestNavigation += Navigate;

            Thresholds = new List<AdditionalThresholdRowItem>();

            LoadElements();

            StudyCache.ImpactAreaAdded += AddImpactAreaElement;
            StudyCache.ImpactAreaRemoved += RemoveImpactAreaElement;
            StudyCache.ImpactAreaUpdated += UpdateImpactAreaElement;

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

        private void AddStageDamageElement(object sender, Saving.ElementAddedEventArgs e)
        {
            StageDamageElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveStageDamageElement(object sender, Saving.ElementAddedEventArgs e)
        {
            removeElement(((ChildElement)e.Element).GetElementID(), StageDamageElements);
        }
        private void UpdateStageDamageElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            updateElement(StageDamageElements, SelectedStageDamageElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }

        private void AddExtIntElement(object sender, Saving.ElementAddedEventArgs e)
        {
            ExteriorInteriorElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveExtIntElement(object sender, Saving.ElementAddedEventArgs e)
        {
            removeElement(((ChildElement)e.Element).GetElementID(), ExteriorInteriorElements);
        }
        private void UpdateExtIntElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            updateElement(ExteriorInteriorElements, SelectedExteriorInteriorElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }

        private void AddLeveeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            LeveeFeatureElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveLeveeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            removeElement(((ChildElement)e.Element).GetElementID(), LeveeFeatureElements);
        }
        private void UpdateLeveeElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            updateElement(LeveeFeatureElements, SelectedLeveeFeatureElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }

        private void AddInOutElement(object sender, Saving.ElementAddedEventArgs e)
        {
            InflowOutflowElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveInOutElement(object sender, Saving.ElementAddedEventArgs e)
        {
            removeElement(((ChildElement)e.Element).GetElementID(), InflowOutflowElements);
        }
        private void UpdateInOutElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            updateElement(InflowOutflowElements, SelectedInflowOutflowElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }


        private void AddFlowFreqElement(object sender, Saving.ElementAddedEventArgs e)
        {
            FrequencyElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveFlowFreqElement(object sender, Saving.ElementAddedEventArgs e)
        {
            removeElement(((ChildElement)e.Element).GetElementID(), FrequencyElements);
        }
        private void UpdateFlowFreqElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            updateElement(FrequencyElements, SelectedFrequencyElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }

        private void AddImpactAreaElement(object sender, Saving.ElementAddedEventArgs e)
        {
            ImpactAreaElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveImpactAreaElement(object sender, Saving.ElementAddedEventArgs e)
        {
            removeElement(((ChildElement)e.Element).GetElementID(), ImpactAreaElements);
        }
        private void UpdateImpactAreaElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            updateElement(ImpactAreaElements, SelectedImpactAreaElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }

        private void AddRatingElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RatingCurveElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveRatingElement(object sender, Saving.ElementAddedEventArgs e)
        {
            removeElement(((ChildElement)e.Element).GetElementID(), RatingCurveElements);
        }
        private void UpdateRatingElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            updateElement(RatingCurveElements, SelectedRatingCurveElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }

        private void removeElement(int idToRemove, ObservableCollection<ChildElementComboItem> collection )
        {
            collection.Remove(collection.Where(elem => elem.ChildElement != null && elem.ChildElement.GetElementID() == idToRemove).Single());
        }

        private void updateElement(ObservableCollection<ChildElementComboItem> collection, ChildElementComboItem selectedItem,
             ChildElement oldElement, ChildElement newElement)
        {
            int idToUpdate = oldElement.GetElementID();

            ChildElementComboItem itemToUpdate = collection.Where(elem => elem.ChildElement != null && elem.ChildElement.GetElementID() == idToUpdate).SingleOrDefault();
            if (itemToUpdate != null)
            {
                int index = collection.IndexOf(itemToUpdate);

                //this was an attempt to update the selected item if that is the one we are swapping out. For some reason
                //this doesn't work. I was trying to find a way to pass the property into this method and was unsuccessful.
                //bool needToUpdateSelected = selectedItem.ChildElement != null && selectedItem.ChildElement.GetElementID() == idToUpdate;

                //if (index != -1)
                //{
                //    collection.RemoveAt(index);
                //    collection.Insert(index, new ChildElementComboItem(newElement));
                //    if (needToUpdateSelected)
                //    {
                //        propToUpdate (collection[index]);
                //    }
                //}
            }
        }

        #endregion

        private void FillForm(IASElement elem)
        {
            Name = elem.Name;
            Description = elem.Description;
            Year = elem.AnalysisYear;
            //SelectedImpactAreaElement = elem.ImpactAreaID;
            FillThresholds(elem);

            //all the available elements have been loaded into this editor. We now want to select
            //the correct element for each dropdown. If we can't find the correct element then the selected elem 
            //will be null.
            SelectedImpactAreaElement = ImpactAreaElements.FirstOrDefault(imp => imp.ChildElement != null && imp.ChildElement.GetElementID() == elem.ImpactAreaID);
            SelectedFrequencyElement = FrequencyElements.FirstOrDefault(freq => freq.ChildElement != null && freq.ChildElement.GetElementID() == elem.FlowFreqID);
            SelectedInflowOutflowElement = InflowOutflowElements.FirstOrDefault(inf => inf.ChildElement != null && inf.ChildElement.GetElementID() == elem.InflowOutflowID);
            SelectedRatingCurveElement = RatingCurveElements.FirstOrDefault(rat => rat.ChildElement != null && rat.ChildElement.GetElementID() == elem.RatingID);
            SelectedLeveeFeatureElement = LeveeFeatureElements.FirstOrDefault(levee => levee.ChildElement != null && levee.ChildElement.GetElementID() == elem.LeveeFailureID);
            SelectedExteriorInteriorElement = ExteriorInteriorElements.FirstOrDefault(ext => ext.ChildElement != null && ext.ChildElement.GetElementID() == elem.ExtIntStageID);
            SelectedStageDamageElement = StageDamageElements.FirstOrDefault(stage => stage.ChildElement != null && stage.ChildElement.GetElementID() == elem.StageDamageID);

            //todo: plot something?
            

        }
        private void FillThresholds(IASElement elem)
        {
            //todo: maybe add a different ctor or a fill method to load the rows?
            _additionalThresholdsVM.Rows = new ObservableCollection<AdditionalThresholdRowItem>();
            foreach (AdditionalThresholdRowItem row in elem.Thresholds)
            {
                _additionalThresholdsVM.Rows.Add(row);
            }
        }

        private void LoadElements()
        {
            //what happens if there are no elements for a combo?
            //I will always add an empty ChildElementComboItem and then select it by default.
            //this means that when asking for the selected combo item, it should never be null.
            List<ChildElement> childElems = new List<ChildElement>();

            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            childElems.AddRange(impactAreaElements);
            ImpactAreaElements = CreateComboItems(childElems);
            SelectedImpactAreaElement = ImpactAreaElements.First();

            List<AnalyticalFrequencyElement> analyticalFrequencyElements = StudyCache.GetChildElementsOfType<AnalyticalFrequencyElement>();
            childElems.Clear();
            childElems.AddRange(analyticalFrequencyElements);
            FrequencyElements = CreateComboItems(childElems);
            SelectedFrequencyElement = FrequencyElements.First();

            List<InflowOutflowElement> inflowOutflowElements = StudyCache.GetChildElementsOfType<InflowOutflowElement>();
            childElems.Clear();
            childElems.AddRange(inflowOutflowElements);
            InflowOutflowElements = CreateComboItems(childElems);
            SelectedInflowOutflowElement = InflowOutflowElements.First();

            List<RatingCurveElement> ratingCurveElements = StudyCache.GetChildElementsOfType<RatingCurveElement>();
            childElems.Clear();
            childElems.AddRange(ratingCurveElements);
            RatingCurveElements = CreateComboItems(childElems);
            SelectedRatingCurveElement = RatingCurveElements.First();

            List<LeveeFeatureElement> leveeFeatureElements = StudyCache.GetChildElementsOfType<LeveeFeatureElement>();
            childElems.Clear();
            childElems.AddRange(leveeFeatureElements);
            LeveeFeatureElements = CreateComboItems(childElems);
            SelectedLeveeFeatureElement = LeveeFeatureElements.First();

            List<ExteriorInteriorElement> exteriorInteriorElements = StudyCache.GetChildElementsOfType<ExteriorInteriorElement>();
            childElems.Clear();
            childElems.AddRange(exteriorInteriorElements);
            ExteriorInteriorElements = CreateComboItems(childElems);
            SelectedExteriorInteriorElement = ExteriorInteriorElements.First();

            List<AggregatedStageDamageElement> aggregatedStageDamageElements = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            childElems.Clear();
            childElems.AddRange(aggregatedStageDamageElements);
            StageDamageElements = CreateComboItems(childElems);
            SelectedStageDamageElement = StageDamageElements.First();
        }

        private ObservableCollection<ChildElementComboItem> CreateComboItems(List<ChildElement> elems)
        {
            ObservableCollection<ChildElementComboItem> items = new ObservableCollection<ChildElementComboItem>();
            items.Add(new ChildElementComboItem(null));
            foreach(ChildElement elem in elems)
            {
                items.Add(new ChildElementComboItem(elem));
            }

            return items;
        }

        public void AddThresholds()
        {
            //AdditionalThresholdsVM vm = new AdditionalThresholdsVM();
            string header = "Annual Exceedance Probabilities Thresholds";
            DynamicTabVM tab = new DynamicTabVM(header, _additionalThresholdsVM, "additionalThresholds");
            Navigate(tab,true, true);
            Thresholds = _additionalThresholdsVM.GetThresholds();
            

        }

        private void UpdateRatingRequired()
        {
            if (SelectedFrequencyElement != null && SelectedFrequencyElement.ChildElement != null )
            {
                //todo: we need to check to see if this is a flow freq.
                //if the flow freq is of type flow freq, then the rating curve is required.
                //this is task 5 in the clean document.
                RatingRequired = true;
            }
        }
        private void StageDamageSelectionChanged()
        {
            if (SelectedStageDamageElement != null && SelectedStageDamageElement.ChildElement != null)
            {
                List<string> damCats = new List<string>();
                damCats.Add("todo: add dam cats");
                DamageCategories = damCats;
                SelectedDamageCategory = damCats[0];
            }
        }

        #region validation

        private ValidationResult IsFrequencyRelationshipValid()
        {
            ValidationResult vr = new ValidationResult();
            if(SelectedFrequencyElement.ChildElement == null)
            {
                vr.IsValid = false;
                vr.ErrorMessage = "A Frequency Relationship is required.";
            }

            return vr;
        }

        private ValidationResult IsRatingCurveValid()
        {
            ValidationResult vr = new ValidationResult();
            if (_ratingRequired && SelectedRatingCurveElement.ChildElement == null)
            {
                vr.IsValid = false;
                vr.ErrorMessage = "A Rating Curve is required when using a flow-frequency relationship.";
            }

            return vr;
        }
        private ValidationResult IsStageDamageValid()
        {
            ValidationResult vr = new ValidationResult();
            if (SelectedStageDamageElement.ChildElement == null)
            {
                vr.IsValid = false;
                vr.ErrorMessage = "A Stage Damage is required.";
            }

            return vr;
        }

        private ValidationResult IsYearValid()
        {
            ValidationResult vr = new ValidationResult();
            if (Year < 1900 && Year > 3000)
            {
                vr.IsValid = false;
                vr.ErrorMessage = "A year is required and must be greater than 1900 and less than 3000";
            }

            return vr;
        }

        private ValidationResult IsThresholdsValid()
        {
            ValidationResult vr = new ValidationResult();
            if (Thresholds.Count == 0)
            {
                vr.IsValid = false;
                vr.ErrorMessage = "At least one threshold is required.";
            }

            return vr;
        }
        #endregion



        #region PlotCurves
        public bool CanPlot()
        {


            StringBuilder errorMsg = new StringBuilder();
            ValidationResult vr = IsFrequencyRelationshipValid();
            if (!vr.IsValid)
            {
                errorMsg.AppendLine(vr.ErrorMessage);
            }

            vr = IsRatingCurveValid();
            if (!vr.IsValid)
            {
                errorMsg.AppendLine(vr.ErrorMessage);
            }

            vr = IsStageDamageValid();
            if (!vr.IsValid)
            {
                errorMsg.AppendLine(vr.ErrorMessage);
            }

            vr = IsYearValid();
            if(!vr.IsValid)
            {
                errorMsg.AppendLine(vr.ErrorMessage);
            }

            vr = IsThresholdsValid();
            if (!vr.IsValid)
            {
                errorMsg.AppendLine(vr.ErrorMessage);
            }

            //todo: actually run the compute and see if it was successful?


            string msg = errorMsg.ToString();

            if (msg.Length > 0)
            {
                MessageBox.Show(msg, "Insufficient Data", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else
            {
                //if no msg's then we can plot
                return true;
            }

        }
        private IFdaFunction getFrequencyRelationshipFunction()
        {
            //todo: this will just be getting the selected curve

            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                xValues.Add(i / 10.0);
                yValues.Add(i * 9);
            }
            ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.OutflowFrequency, coordinatesFunction);
            return fdaFunction;
        }

        private IFdaFunction getRatingCurveFunction()
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                xValues.Add(i);
                yValues.Add(i * 11);
            }

            ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.Rating, coordinatesFunction);
            return fdaFunction;
        }

        private IFdaFunction getStageDamageFunction()
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                xValues.Add(i + 2);
                yValues.Add(i * 90);
            }

            ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.InteriorStageDamage, coordinatesFunction);
            return fdaFunction;
        }

        private IFdaFunction getDamageFrequencyFunction()
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                xValues.Add(i / 9.0);
                yValues.Add(i * 110);
            }

            ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.DamageFrequency, coordinatesFunction);
            return fdaFunction;
        }

        public void Plot()
        {
            //CanPlot() is being called by the view before calling this method.
            //if (CanPlot())
            {
                //get the current curves and set that data on the chart controls
                //this update call will set the current crosshair data on each one
                FrequencyRelationshipControl.UpdatePlotData(getFrequencyRelationshipFunction());
                RatingRelationshipControl.UpdatePlotData(getRatingCurveFunction());
                StageDamageControl.UpdatePlotData(getStageDamageFunction());
                DamageFrequencyControl.UpdatePlotData(getDamageFrequencyFunction());

                //link the crosshair data to eachother
                CrosshairData freqRelationshipCrosshairData = FrequencyRelationshipControl.currentCrosshairData;
                CrosshairData ratingCrosshairData = RatingRelationshipControl.currentCrosshairData;
                freqRelationshipCrosshairData.Next = new SharedAxisCrosshairData(ratingCrosshairData, Axis.Y, Axis.Y);
                ratingCrosshairData.Previous = new SharedAxisCrosshairData(freqRelationshipCrosshairData, Axis.Y, Axis.Y);

                CrosshairData stageDamageCrosshairData = StageDamageControl.currentCrosshairData;
                ratingCrosshairData.Next = new SharedAxisCrosshairData(stageDamageCrosshairData, Axis.X, Axis.X);
                stageDamageCrosshairData.Previous = new SharedAxisCrosshairData(ratingCrosshairData, Axis.X, Axis.X);

                CrosshairData damageFreqCrosshairData = DamageFrequencyControl.currentCrosshairData;
                stageDamageCrosshairData.Next = new SharedAxisCrosshairData(damageFreqCrosshairData, Axis.Y, Axis.Y);
                damageFreqCrosshairData.Previous = new SharedAxisCrosshairData(stageDamageCrosshairData, Axis.Y, Axis.Y);

                FrequencyRelationshipControl.Plot();
                RatingRelationshipControl.Plot();
                StageDamageControl.Plot();
                DamageFrequencyControl.Plot();
            }
        }

        #endregion


        private Boolean ValidateIAS()
        {
            //todo: the rating curve is required is the frequency relationship is of type
            //flow-frequency. This will need to get added once we complete task 5 in the clean doc.
            //if (Description == null) { Description = ""; }

            //todo: is this the same as the CanPlot() or are there differences?
            return CanPlot();

        }

        public override void Save()
        {
            bool isValid = ValidateIAS();

            if (isValid)
            {
                Thresholds = _additionalThresholdsVM.GetThresholds();

                int impAreaID = SelectedImpactAreaElement.ChildElement != null ? SelectedImpactAreaElement.ChildElement. GetElementID() : -1;
                int flowFreqID = SelectedFrequencyElement.ChildElement != null ? SelectedFrequencyElement.ChildElement.GetElementID() : -1;
                int inflowOutID = SelectedInflowOutflowElement.ChildElement != null ? SelectedInflowOutflowElement.ChildElement.GetElementID() : -1;
                int ratingID = SelectedRatingCurveElement.ChildElement != null ? SelectedRatingCurveElement.ChildElement.GetElementID() : -1;
                int extIntID = SelectedExteriorInteriorElement.ChildElement != null ? SelectedExteriorInteriorElement.ChildElement.GetElementID() : -1;
                int latStructID = SelectedLeveeFeatureElement.ChildElement != null ? SelectedLeveeFeatureElement.ChildElement.GetElementID() : -1;
                int stageDamID = SelectedStageDamageElement.ChildElement != null ? SelectedStageDamageElement.ChildElement.GetElementID() : -1;

                List<AdditionalThresholdRowItem> thresholdRowItems = _additionalThresholdsVM.GetThresholds();

                IASElement elementToSave = new IASElement(Name, Description, Year, impAreaID,
                flowFreqID, inflowOutID,
                ratingID, extIntID, latStructID, stageDamID, thresholdRowItems);
                CurrentElement = elementToSave;

                if (_isInEditMode)
                {
                    Saving.PersistenceFactory.GetIASManager().SaveExisting(_currentElement, elementToSave);
                }
                else
                {
                    Saving.PersistenceFactory.GetIASManager().SaveNew(elementToSave);
                }
            }
        }


    }
}
