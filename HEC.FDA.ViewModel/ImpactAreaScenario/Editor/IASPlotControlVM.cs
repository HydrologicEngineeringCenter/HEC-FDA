using HEC.Plotting.Core;
using System;
using System.Windows;
using HEC.FDA.ViewModel.ImpactAreaScenario.Editor.ChartControls;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class IASPlotControlVM : BaseViewModel
    {
        private Visibility _plotVisibility = Visibility.Hidden;

        public FrequencyRelationshipControl FrequencyRelationshipControl { get; } = new FrequencyRelationshipControl();
        public ChartControlBase RatingRelationshipControl { get; } = new RatingRelationshipControl();
        public ChartControlBase StageDamageControl { get; } = new StageDamageControl();
        public ChartControlBase DamageFrequencyControl { get; } = new DamageFrequencyControl();

        public Visibility PlotControlVisibility
        {
            get => _plotVisibility;
            set
            {
                _plotVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public void Plot()
        {
            PlotControlVisibility = Visibility.Visible;
            FrequencyRelationshipControl.UpdateYAxisLabel();
            FrequencyRelationshipControl.Plot();
            RatingRelationshipControl.Plot();
            StageDamageControl.Plot();
            DamageFrequencyControl.Plot();

            UpdateMinMax(FrequencyRelationshipControl, RatingRelationshipControl, DamageFrequencyControl);
            UpdateMinMax(RatingRelationshipControl, FrequencyRelationshipControl, StageDamageControl);
            // if we don't have a rating curve, we need to set the axes instead to match the STAGE frequency function
            if (RatingRelationshipControl.Function.Xvals.Count == 0 && RatingRelationshipControl.Function.Yvals.Count == 0)
            {
                Tuple<double, double> freqYMinMax = FrequencyRelationshipControl.GetMinMax(Axis.Y);
                Tuple<double, double> minMaxX = GetMinMax(StageDamageControl.GetMinMax(Axis.X), freqYMinMax);
                Tuple<double, double> minMaxY = GetMinMax(StageDamageControl.GetMinMax(Axis.Y), DamageFrequencyControl.GetMinMax(Axis.Y));
                StageDamageControl.SetMinMax(minMaxX, minMaxY);
                //Rating Curve and Stage Damage enforce the same X axis in the 4 plot chart. The Y here is arbitrary. The x needs to match. 
                RatingRelationshipControl.SetMinMax(minMaxX, Tuple.Create(100d, 1000d));
            }
            else
            {
                UpdateMinMax(StageDamageControl, DamageFrequencyControl, RatingRelationshipControl);
            }
            UpdateMinMax(DamageFrequencyControl, StageDamageControl, FrequencyRelationshipControl);

        }

        private void UpdateMinMax(ChartControlBase primary, ChartControlBase horizontalNeighbor, ChartControlBase verticalNeighbor)
        { 
            Tuple<double, double> minMaxX = GetMinMax(primary.GetMinMax(Axis.X), verticalNeighbor.GetMinMax(Axis.X));
            Tuple<double, double> minMaxY = GetMinMax(primary.GetMinMax(Axis.Y), horizontalNeighbor.GetMinMax(Axis.Y));
            
            primary.SetMinMax(minMaxX, minMaxY);
        }

        private Tuple<double, double> GetMinMax(Tuple<double, double> left, Tuple<double, double> right)
        {
            double item1 = Math.Min(left.Item1, right.Item1);
            double item2 = Math.Max(left.Item2, right.Item2);
            return new Tuple<double, double>(item1, item2);
        }
    }
}
