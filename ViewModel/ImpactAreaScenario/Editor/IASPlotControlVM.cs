using HEC.Plotting.Core;
using System;
using System.Windows;
using ViewModel.ImpactAreaScenario.Editor.ChartControls;

namespace ViewModel.ImpactAreaScenario.Editor
{
    public class IASPlotControlVM : BaseViewModel
    {
        private Visibility _plotVisibility = Visibility.Hidden;

        public ChartControlBase FrequencyRelationshipControl { get; } = new FrequencyRelationshipControl();
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
            FrequencyRelationshipControl.Plot();
            RatingRelationshipControl.Plot();
            StageDamageControl.Plot();
            DamageFrequencyControl.Plot();

            UpdateMinMax(FrequencyRelationshipControl, RatingRelationshipControl, Axis.Y);
            UpdateMinMax(StageDamageControl, DamageFrequencyControl, Axis.Y);

            //UpdateMinMax(FrequencyRelationshipControl, DamageFrequencyControl, Axis.X);
            UpdateMinMax(StageDamageControl, RatingRelationshipControl, Axis.X);
        }

        private void UpdateMinMax(ChartControlBase primary, ChartControlBase secondary, Axis axis)
        {
            
            Tuple<double, double> leftMinMax = primary.GetMinMax(axis);
            Tuple<double, double> rightMinMax = secondary.GetMinMax(axis);

            double min;
            if (axis == Axis.Y)
            {
                //Y is log for all charts.  don't go lower than the min.
                min = Math.Max(Math.Min(leftMinMax.Item1, rightMinMax.Item1), 0.00001);
            }
            else
            {
                min = Math.Min(leftMinMax.Item1, rightMinMax.Item1);
            }
            
            double max = Math.Max(leftMinMax.Item2, rightMinMax.Item2);

            //They're bound, so set the primary's min/max and that should work.
            primary.SetMinMax(axis, min, max);
        }
    }
}
