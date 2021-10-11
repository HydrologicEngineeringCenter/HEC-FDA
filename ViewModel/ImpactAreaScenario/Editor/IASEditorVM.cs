using FunctionsView.ViewModel;
using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.Controller;
using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using System;
using System.Collections.Generic;
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


        public SciChart2DChartViewModel FlowFreqChartVM { get; set; } = new SciChart2DChartViewModel("Flow Frequency");
        public SciChart2DChartViewModel RatingChartVM { get; set; } = new SciChart2DChartViewModel("Rating Curve");
        public SciChart2DChartViewModel StageDamageChartVM { get; set; } = new SciChart2DChartViewModel("Stage Damage");
        public SciChart2DChartViewModel DamageFreqChartVM { get; set; } = new SciChart2DChartViewModel("Damage Frequency");
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
        public RatingCurveElement SelectedRatingCurveElement { get; set; }
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
            //hook up the navigate event for the additional thresholds dialog
            _additionalThresholdsVM = new AdditionalThresholdsVM();
            _additionalThresholdsVM.RequestNavigation += Navigate;

            Thresholds = new List<AdditionalThresholdRowItem>();

            LoadElements();
        }

        private void AddCrosshairs()
        {

            CrosshairData flowFreqCrosshairData = new CrosshairData(SelectedFrequencyElement.Curve);
            FlowFreqChartVM.ModifierGroup.ChildModifiers.Add(new FdaCrosshairChartModifier(true, true, flowFreqCrosshairData));
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

        

        private bool CanPlot()
        {
          
            StringBuilder errorMsg = new StringBuilder();
            if(SelectedFrequencyElement == null)
            {
                errorMsg.AppendLine("A Flow Frequency is required.");
            }
            if(SelectedRatingCurveElement == null)
            {
                errorMsg.AppendLine("A Rating Curve is required.");
            }
            if (SelectedStageDamageElement == null)
            {
                errorMsg.AppendLine("A Stage Damage is required.");
            }
            
            //todo: actually run the compute and see if it was successful?
            
            
            string msg = errorMsg.ToString();

            if(msg.Length>0)
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

        #region PlotCurves

        public void Plot()
        {
            if (CanPlot())
            {
                AddCrosshairs();

                PlotTestFlowFreq();
                PlotTestRating();
                PlotTestStageDamage();
                PlotTestDamageFreq();

                //PlotFlowFreq();
                //PlotRating();
                //PlotStageDamage();
            }
        }

        private void PlotTestFlowFreq()
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            for(int i = 0;i<10;i++)
            {
                xValues.Add(i/10.0);
                yValues.Add(i*9);
            }          

            SciLineData lineData = new NumericLineData(xValues.ToArray(), yValues.ToArray(), "asdf", "asdf", "adsf", "asdf", PlotType.Line);
            FlowFreqChartVM.LineData.Set(new List<SciLineData>() { lineData });
        }

        private void PlotTestRating()
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                xValues.Add(i);
                yValues.Add(i*11);
            }

            SciLineData lineData = new NumericLineData(xValues.ToArray(), yValues.ToArray(), "asdf", "asdf", "adsf", "asdf", PlotType.Line);
            RatingChartVM.LineData.Set(new List<SciLineData>() { lineData });
        }

        private void PlotTestStageDamage()
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                xValues.Add(i+2);
                yValues.Add(i*90);
            }

            SciLineData lineData = new NumericLineData(xValues.ToArray(), yValues.ToArray(), "asdf", "asdf", "adsf", "asdf", PlotType.Line);
            StageDamageChartVM.LineData.Set(new List<SciLineData>() { lineData });
        }
        private void PlotTestDamageFreq()
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                xValues.Add(i / 9.0);
                yValues.Add(i*110);
            }

            SciLineData lineData = new NumericLineData(xValues.ToArray(), yValues.ToArray(), "asdf", "asdf", "adsf", "asdf", PlotType.Line);
            DamageFreqChartVM.LineData.Set(new List<SciLineData>() { lineData });
        }

        private void PlotFlowFreq()
        {
            CoordinatesFunctionEditorChartHelper chartHelper = new CoordinatesFunctionEditorChartHelper(SelectedFrequencyElement.Curve.Function, "Frequency", "Flow");
            List<SciLineData> lineData = chartHelper.CreateLineData(false, true, true);
            foreach (SciLineData ld in lineData)
            {
                ld.XAxisAlignment = HEC.Plotting.Core.DataModel.AxisAlignment.Top;
                ld.YAxisAlignment = HEC.Plotting.Core.DataModel.AxisAlignment.Right;
            }
            DamageFreqChartVM.LineData.Set(lineData);
        }

        private void PlotRating()
        {
            CoordinatesFunctionEditorChartHelper chartHelper = new CoordinatesFunctionEditorChartHelper(SelectedRatingCurveElement.Curve.Function, "Stage", "Flow");
            List<SciLineData> lineData = chartHelper.CreateLineData(false, true, false);
            foreach (SciLineData ld in lineData)
            {
                ld.XAxisAlignment = HEC.Plotting.Core.DataModel.AxisAlignment.Top;
                ld.YAxisAlignment = HEC.Plotting.Core.DataModel.AxisAlignment.Left;
            }
            RatingChartVM.LineData.Set(lineData);
        }

        private void PlotStageDamage()
        {
            CoordinatesFunctionEditorChartHelper chartHelper = new CoordinatesFunctionEditorChartHelper(SelectedStageDamageElement.Curve.Function, "Stage", "Damage");
            List<SciLineData> lineData = chartHelper.CreateLineData(false, true, true);
            foreach (SciLineData ld in lineData)
            {
                ld.XAxisAlignment = HEC.Plotting.Core.DataModel.AxisAlignment.Bottom;
                ld.YAxisAlignment = HEC.Plotting.Core.DataModel.AxisAlignment.Left;
            }
            StageDamageChartVM.LineData.Set(lineData);
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

                //IASElement elementToSave = new IASElement(Name, Description, Year, SelectedImpactArea.GetElementID(),
                //flowFreqID, inflowOutflowID, ratingID, extIntID, leveeFailureID, stageDamageID, SelectedThresholdType, ThresholdValue);
                //CurrentElement = elementToSave;

                //Saving.PersistenceFactory.GetIASManager().SaveNew(elementToSave);
            }
        }


    }
}
