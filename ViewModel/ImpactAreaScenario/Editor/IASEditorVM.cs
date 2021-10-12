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
        private Chart2DController _controller;
        private AdditionalThresholdsVM _additionalThresholdsVM;
        private AggregatedStageDamageElement _selectedStageDamageElement;
        private List<string> _damageCategories;
        private string _selectedDamageCategory;
        private RatingCurveElement _selectedRatingCurveElement;


        //public SciChart2DChartViewModel FlowFreqChartVM { get; set; } = new SciChart2DChartViewModel("Flow Frequency");
        public ChartControlBase FrequencyRelationshipControl { get; set; }
        public ChartControlBase RatingRelationshipControl { get; set; }
        public ChartControlBase StageDamageControl { get; set; }
        public ChartControlBase DamageFrequencyControl { get; set; }



        //public SciChart2DChartViewModel RatingChartVM { get; set; } = new SciChart2DChartViewModel("Rating Curve");
        //public SciChart2DChartViewModel StageDamageChartVM { get; set; } = new SciChart2DChartViewModel("Stage Damage");
        //public SciChart2DChartViewModel DamageFreqChartVM { get; set; } = new SciChart2DChartViewModel("Damage Frequency");
        public int Year { get; set; }


        public List<string> DamageCategories
        {
            get { return _damageCategories; }
            set { _damageCategories = value; NotifyPropertyChanged(); }
        }
        public List<AdditionalThresholdRowItem> Thresholds { get; set; }
        public List<ImpactAreaElement> ImpactAreaElements { get; set; }
        public List<AnalyticalFrequencyElement> FrequencyElements { get; set; }
        public List<InflowOutflowElement> InflowOutflowElements { get; set; }
        public List<RatingCurveElement> RatingCurveElements { get; set; }
        
        public List<LeveeFeatureElement> LeveeFeatureElements { get; set; }
        public List<ExteriorInteriorElement> ExteriorInteriorElements { get; set; }
        public List<AggregatedStageDamageElement> StageDamageElements { get; set; }

        public string SelectedDamageCategory
        {
            get { return _selectedDamageCategory; }
            set { _selectedDamageCategory = value; NotifyPropertyChanged(); }
        }
        public ImpactAreaElement SelectedImpactAreaElement { get; set; }
        public AnalyticalFrequencyElement SelectedFrequencyElement { get; set; }
        public InflowOutflowElement SelectedInflowOutflowElement { get; set; }
        public RatingCurveElement SelectedRatingCurveElement
        {
            get { return _selectedRatingCurveElement; }
            set { _selectedRatingCurveElement = value; NotifyPropertyChanged(); }
        }
        public LeveeFeatureElement SelectedLeveeFeatureElement { get; set; }
        public ExteriorInteriorElement SelectedExteriorInteriorElement { get; set; }
        public AggregatedStageDamageElement SelectedStageDamageElement
        {
            get { return _selectedStageDamageElement; }
            set { _selectedStageDamageElement = value;  StageDamageSelectionChanged(); }
        }

        /// <summary>
        /// This is the create new ctor
        /// </summary>
        public IASEditorVM():base(null)
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
        }

        //todo: this ctor probably needs some work
        public IASEditorVM(IASElement elem) : base(elem, null)
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
            FillForm(elem);
        }

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
            SelectedImpactAreaElement = ImpactAreaElements.FirstOrDefault(imp => imp.GetElementID() == elem.ImpactAreaID);
            SelectedFrequencyElement = FrequencyElements.FirstOrDefault(freq => freq.GetElementID() == elem.FlowFreqID);
            SelectedInflowOutflowElement = InflowOutflowElements.FirstOrDefault(inf => inf.GetElementID() == elem.InflowOutflowID);
            SelectedRatingCurveElement = RatingCurveElements.FirstOrDefault(rat => rat.GetElementID() == elem.RatingID);
            SelectedLeveeFeatureElement = LeveeFeatureElements.FirstOrDefault(levee => levee.GetElementID() == elem.LeveeFailureID);
            SelectedExteriorInteriorElement = ExteriorInteriorElements.FirstOrDefault(ext => ext.GetElementID() == elem.ExtIntStageID);
            SelectedStageDamageElement = StageDamageElements.FirstOrDefault(stage => stage.GetElementID() == elem.StageDamageID);

            //todo: plot something?
            

        }
        private void FillThresholds(IASElement elem)
        {
            //todo: maybe add a different ctor or a fill method to load the rows?
            _additionalThresholdsVM.Rows = new ObservableCollection<AdditionalThresholdRowItem>();
            AdditionalThresholdRowItem row = new AdditionalThresholdRowItem(1, elem.ThresholdType, elem.ThresholdValue);
            _additionalThresholdsVM.Rows.Add(row);
        }

        private void LoadElements()
        {
            ImpactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            FrequencyElements = StudyCache.GetChildElementsOfType<AnalyticalFrequencyElement>();
            //FrequencyElements.Insert(0, null);
            InflowOutflowElements = StudyCache.GetChildElementsOfType<InflowOutflowElement>();
            RatingCurveElements = StudyCache.GetChildElementsOfType<RatingCurveElement>();
            LeveeFeatureElements = StudyCache.GetChildElementsOfType<LeveeFeatureElement>();
            ExteriorInteriorElements = StudyCache.GetChildElementsOfType<ExteriorInteriorElement>(); 
            StageDamageElements = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();

        }

        public void AddThresholds()
        {
            //AdditionalThresholdsVM vm = new AdditionalThresholdsVM();
            string header = "Annual Exceedance Probabilities Thresholds";
            DynamicTabVM tab = new DynamicTabVM(header, _additionalThresholdsVM, "additionalThresholds");
            Navigate(tab,true, true);
            Thresholds = _additionalThresholdsVM.GetThresholds();
            

        }


        private void StageDamageSelectionChanged()
        {
            List<string> damCats = new List<string>();
            damCats.Add("testing");
            DamageCategories = damCats;
            SelectedDamageCategory = damCats[0];
        }





        #region PlotCurves
        private bool CanPlot()
        {
            //todo: just for testing
            return true;


            StringBuilder errorMsg = new StringBuilder();
            if (SelectedFrequencyElement == null)
            {
                errorMsg.AppendLine("A Flow Frequency is required.");
            }
            if (SelectedRatingCurveElement == null)
            {
                errorMsg.AppendLine("A Rating Curve is required.");
            }
            if (SelectedStageDamageElement == null)
            {
                errorMsg.AppendLine("A Stage Damage is required.");
            }

            //todo: actually run the compute and see if it was successful?


            string msg = errorMsg.ToString();

            if (msg.Length > 0)
            {
                MessageBox.Show(msg, "Insufficient Data To Plot", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
            if (CanPlot())
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

                //CrosshairData stageDamageCrosshairData = StageDamageControl.currentCrosshairData;
                //ratingCrosshairData.Next = new SharedAxisCrosshairData(stageDamageCrosshairData, Axis.X, Axis.X);
                //stageDamageCrosshairData.Previous = new SharedAxisCrosshairData(ratingCrosshairData, Axis.X, Axis.X);

                //CrosshairData damageFreqCrosshairData = DamageFrequencyControl.currentCrosshairData;
                //stageDamageCrosshairData.Next = new SharedAxisCrosshairData(damageFreqCrosshairData, Axis.Y, Axis.Y);
                //damageFreqCrosshairData.Previous = new SharedAxisCrosshairData(stageDamageCrosshairData, Axis.Y, Axis.Y);

                FrequencyRelationshipControl.Plot();
                RatingRelationshipControl.Plot();
                StageDamageControl.Plot();
                DamageFrequencyControl.Plot();

   
            }
        }

        #endregion


        private Boolean ValidateIAS()
        {
            //todo: don't i have some name validator already?
            if (Description == null) { Description = ""; }
            return true;
        }

        public override void Save()
        {
            bool isValid = ValidateIAS();

            if (isValid)
            {
                Thresholds = _additionalThresholdsVM.GetThresholds();

                int impAreaID = SelectedImpactAreaElement != null ? SelectedImpactAreaElement.GetElementID() : -1;
                int flowFreqID = SelectedFrequencyElement != null ? SelectedFrequencyElement.GetElementID() : -1;
                int inflowOutID = SelectedInflowOutflowElement != null ? SelectedInflowOutflowElement.GetElementID() : -1;
                int ratingID = SelectedRatingCurveElement != null ? SelectedRatingCurveElement.GetElementID() : -1;
                int extIntID = SelectedExteriorInteriorElement != null ? SelectedExteriorInteriorElement.GetElementID() : -1;
                int latStructID = SelectedLeveeFeatureElement != null ? SelectedLeveeFeatureElement.GetElementID() : -1;
                int stageDamID = SelectedStageDamageElement != null ? SelectedStageDamageElement.GetElementID() : -1;

                List<AdditionalThresholdRowItem> thresholdRowItems = _additionalThresholdsVM.GetThresholds();

                IASElement elementToSave = new IASElement(Name, Description, Year, impAreaID,
                flowFreqID, inflowOutID,
                ratingID, extIntID, latStructID, stageDamID, thresholdRowItems[0].ThresholdType, thresholdRowItems[0].ThresholdValue);
                CurrentElement = elementToSave;

                Saving.PersistenceFactory.GetIASManager().SaveNew(elementToSave);
            }
        }


    }
}
