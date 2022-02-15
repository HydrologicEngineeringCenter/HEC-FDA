using HEC.Plotting.Core;
using System;
using System.Windows;
using HEC.FDA.ViewModel.ImpactAreaScenario.Editor.ChartControls;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
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
            UpdateMinMax(RatingRelationshipControl, StageDamageControl, Axis.X);
        }

        private void UpdateMinMax(ChartControlBase primary, ChartControlBase secondary, Axis axis)
        {
            
            Tuple<double, double> leftMinMax = primary.GetMinMax(axis);
            Tuple<double, double> rightMinMax = secondary.GetMinMax(axis);

            //TODO:  Once Y goes log again, we will need to adjust the min so it doesn't go <= 0, or there needs to be some guarantee outside of this that guarantees the min does not go <= 0.
            double min = Math.Min(leftMinMax.Item1, rightMinMax.Item1);
            double max = Math.Max(leftMinMax.Item2, rightMinMax.Item2);

            //They're bound, so only set the primary's min/max and it will update the secondary min/max.
            primary.SetMinMax(axis, min, max);
        }
    }
}
