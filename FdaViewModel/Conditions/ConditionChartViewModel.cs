using HEC.Plotting.Core;
using HEC.Plotting.Core.DataModel;
using HEC.Plotting.SciChart2D.Annotations;
using HEC.Plotting.SciChart2D.Charts.Sci.Modifiers;
using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using HEC.Plotting.SciChart2D.ViewModel.ChartAxis;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Visuals.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FdaViewModel.Conditions
{
    public class ConditionChartViewModel : SciChart2DChartViewModel
    {
        private RectangleAnnotationData<double, double> _xUpperLimit;
        private RectangleAnnotationData<double, double> _xLowerLimit;
        private RectangleAnnotationData<double, double> _yUpperLimit;
        private RectangleAnnotationData<double, double> _yLowerLimit;
        private RectangleAnnotationData<double, double> _test;
        private bool _rangesSet = false;

        public double XMin { get; set; }
        public double XMax { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }

        public double SharedXAxisMin { get; set; }
        public double SharedXAxisMax { get; set; }
        public double SharedYAxisMin { get; set; }
        public double SharedYAxisMax { get; set; }

        public ConditionChartViewModel(string chartId) : base(chartId)
        {
            //This is the SciChart modifier group on the view model.
            //Set up your crosshair modifier here as needed.
            ModifierGroup.ChildModifiers.Add(new BorderModifier());
        }

        protected override void LineDataRemoved(IEnumerable<ILineData> lineDataToRemove)
        {
            base.LineDataRemoved(lineDataToRemove);
            foreach (var data in lineDataToRemove)
            {
                data.PropertyChanged -= LineDataPropertyChanged;
            }

            AnnotationData.RemoveRange(new[] { _xUpperLimit, _xLowerLimit, _yLowerLimit, _yUpperLimit });
        }

        private void LineDataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "XData":
                case "YData":
                    UpdateLimits();
                    break;
            }
        }

        protected override void AxesReset(Axis axis)
        {
            UpdateLimits();
        }

        protected override void AxesAdded(IEnumerable<IAxisViewModel> newViewModels, Axis axis)
        {
            base.AxesAdded(newViewModels, axis);
            foreach (var vm in newViewModels)
            {
                vm.AutoRange = AutoRange.Always;
                vm.VisibleRangeChanged += AxisVisibleRangeChanged;
            }
        }

        private void AxisVisibleRangeChanged(object sender, VisibleRangeChangedEventArgs e)
        {
            UpdateLimits();
        }

        private void UpdateLimits()
        {
            if (_xLowerLimit != null && _xUpperLimit != null && _yLowerLimit != null && _yUpperLimit != null)
            {
                var xDataMin = double.PositiveInfinity;
                var xDataMax = double.NegativeInfinity;
                var yDataMin = double.PositiveInfinity;
                var yDataMax = double.NegativeInfinity;

                var xVisibleMin = double.PositiveInfinity;
                var xVisibleMax = double.NegativeInfinity;
                var yVisibleMin = double.PositiveInfinity;
                var yVisibleMax = double.NegativeInfinity;

                //This is a simplified way of computing these things based on the existing data, however your case is
                //bound to differ from this.
                ComputeMinMaxXyValues(ref yDataMin, ref yDataMax, ref xDataMin, ref xDataMax);
                ComputeVisibleMinMaxXyValues(ref yVisibleMin, ref yVisibleMax, ref xVisibleMin, ref xVisibleMax);

                if (double.IsInfinity(xDataMax) || double.IsInfinity(yDataMax) || double.IsInfinity(xDataMin)
                    || double.IsInfinity(yDataMin) || double.IsInfinity(xVisibleMax) || double.IsInfinity(yVisibleMax)
                    || double.IsInfinity(xVisibleMin) || double.IsInfinity(yVisibleMin))
                {
                    return;
                }

                // update the visible range with shared axis values
                //for some reason this group of code works from visual studio, 
                //but crashes if run from the exe -- this ended up being true if the shared value
                //was 0. It created some sort of infinite width. This might have to do with the log axes. 
                if (SharedXAxisMin != 0 && SharedXAxisMin < xVisibleMin)
                {
                    xVisibleMin = SharedXAxisMin;
                }
                if (SharedXAxisMax != 0 && SharedXAxisMax > xVisibleMax)
                {
                    xVisibleMax = SharedXAxisMax;
                }
                if (SharedYAxisMin != 0 && SharedYAxisMin < yVisibleMin)
                {
                    yVisibleMin = SharedYAxisMin;
                }
                if (SharedYAxisMax != 0 && SharedYAxisMax > yVisibleMax)
                {
                    yVisibleMax = SharedYAxisMax;
                }

                _xLowerLimit.X1 = xVisibleMin;
                _xLowerLimit.X2 = xDataMin;
                _xLowerLimit.Y1 = yVisibleMin;
                _xLowerLimit.Y2 = yVisibleMax;

                _xUpperLimit.X1 = xDataMax;
                _xUpperLimit.X2 = xVisibleMax;
                _xUpperLimit.Y1 = yVisibleMin;
                _xUpperLimit.Y2 = yVisibleMax;

                _yLowerLimit.X1 = xVisibleMin;
                _yLowerLimit.X2 = xVisibleMax;
                _yLowerLimit.Y2 = yVisibleMin;
                _yLowerLimit.Y1 = yDataMin;

                _yUpperLimit.X1 = xVisibleMin;
                _yUpperLimit.X2 = xVisibleMax;
                _yUpperLimit.Y1 = yDataMax;
                _yUpperLimit.Y2 = yVisibleMax;

                _xLowerLimit.Refresh();
                _xUpperLimit.Refresh();
                _yLowerLimit.Refresh();
                _yUpperLimit.Refresh();

                _test.X1 = xDataMin;
                _test.X2 = xDataMax;
                _test.Y1 = yDataMin;
                _test.Y2 = yDataMax;
            }
        }

        private void ComputeVisibleMinMaxXyValues(ref double yDataMin, ref double yDataMax, ref double xDataMin,
            ref double xDataMax)
        {
            IAxisViewModel xAxisVm = null;
            IAxisViewModel yAxisVm = null;

            var vm = AxisViewModel as SciChartAxisViewModel;
            if (vm.XAxisViewModels.Count > 0)
            {
                xAxisVm = vm.XAxisViewModels[0];
            }

            if (vm.YAxisViewModels.Count > 0)
            {
                yAxisVm = vm.YAxisViewModels[0];
            }

            foreach (var data in LineData)
            {
                IEnumerable<double> yValues = data.GetArray(Axis.Y).OfType<double>();
                IEnumerable<double> xValues = data.GetArray(Axis.X).OfType<double>();

                ComputeMinMaxValue(xAxisVm, xValues, ref xDataMin, ref xDataMax);
                ComputeMinMaxValue(yAxisVm, yValues, ref yDataMin, ref yDataMax);
            }
        }

        private void ComputeMinMaxValue(IAxisViewModel vm, IEnumerable<double> array, ref double min, ref double max)
        {
            //This needs computing, because the visible range isn't guaranteed to exist by the time we compute all of this
            //So...let's compute it like a boss.
            var realVm = vm as AxisBaseViewModel;
            var growBy = realVm?.GrowBy;

            if (growBy != null)
            {
                var growByDouble = growBy.AsDoubleRange();
                var growMin = growByDouble.Min;
                var growMax = growByDouble.Max;
                var dataMin = double.MaxValue;
                var dataMax = double.MinValue;
                double visibleMin, visibleMax;

                ComputeMinMaxValue(array, ref dataMin, ref dataMax);

                if (realVm is LogarithmicNumericAxisViewModel)
                {
                    //Grow by is a bit different for these.
                    var logAxis = realVm as LogarithmicNumericAxisViewModel;
                    var logDataMin = Math.Log(dataMin, logAxis.LogarithmicBase);
                    var logDataMax = Math.Log(dataMax, logAxis.LogarithmicBase);
                    var dataDelta = logDataMax - logDataMin;
                    var deltaMin = dataDelta * growMin;
                    var deltaMax = dataDelta * growMax;
                    visibleMin = Math.Pow(logAxis.LogarithmicBase, logDataMin - deltaMin);
                    visibleMax = Math.Pow(logAxis.LogarithmicBase, logDataMax + deltaMax);
                }
                else if (realVm is ProbabilityNumericAxisViewModel)
                {
                    visibleMax = 0.9999;
                    visibleMin = 0.0001;
                }
                else
                {
                    var dataDelta = dataMax - dataMin;
                    var deltaMin = dataDelta * growMin;
                    var deltaMax = dataDelta * growMax;
                    visibleMin = dataMin - deltaMin;
                    visibleMax = dataMax + deltaMax;
                }

                min = visibleMin;
                max = visibleMax;
            }
        }

        private void ComputeMinMaxXyValues(ref double yDataMin, ref double yDataMax, ref double xDataMin,
            ref double xDataMax)
        {
            foreach (var data in LineData)
            {
                IEnumerable<double> yValues = data.GetArray(Axis.Y).OfType<double>();
                IEnumerable<double> xValues = data.GetArray(Axis.X).OfType<double>();

                ComputeMinMaxValue(yValues, ref yDataMin, ref yDataMax);
                ComputeMinMaxValue(xValues, ref xDataMin, ref xDataMax);

            }
        }

        private void ComputeMinMaxValue(IEnumerable<double> values, ref double dataMin, ref double dataMax)
        {
            foreach (var value in values)
            {
                dataMin = Math.Min(dataMin, value);
                dataMax = Math.Max(dataMax, value);
            }
        }

        protected override void LineDataAdded(IEnumerable<ILineData> lineDataToAdd)
        {
            XMin = ((NumericLineData)lineDataToAdd.First()).MinX;
            XMax = ((NumericLineData)lineDataToAdd.Last()).MaxX;
            YMin = ((NumericLineData)lineDataToAdd.First()).MinY;
            YMax = ((NumericLineData)lineDataToAdd.Last()).MaxY;

            base.LineDataAdded(lineDataToAdd);
            string xAxis = null;
            string yAxis = null;

            foreach (var lineData in lineDataToAdd)
            {
                lineData.PropertyChanged += LineDataPropertyChanged;
                xAxis = lineData.AxisName(Axis.X);
                yAxis = lineData.AxisName(Axis.Y);
            }

            if (xAxis != null && yAxis != null)
            {
                _xLowerLimit = ConstructLimitAnnotationData(xAxis, yAxis);
                // _xLowerLimit.BackgroundBrush = new SolidColorBrush(Color.FromArgb(0xCC, 0xFF, 0x00, 0x00));
                _xUpperLimit = ConstructLimitAnnotationData(xAxis, yAxis);
                // _xUpperLimit.BackgroundBrush = new SolidColorBrush(Color.FromArgb(0xCC, 0x00, 0xFF, 0x00));
                _yLowerLimit = ConstructLimitAnnotationData(xAxis, yAxis);
                // _yLowerLimit.BackgroundBrush = new SolidColorBrush(Color.FromArgb(0xCC, 0x00, 0x00, 0xFF));
                _yUpperLimit = ConstructLimitAnnotationData(xAxis, yAxis);
                // _yUpperLimit.BackgroundBrush = new SolidColorBrush(Color.FromArgb(0xCC, 0x00, 0xFF, 0xFF));
                _test = ConstructLimitAnnotationData(xAxis, yAxis);
                // _test.BackgroundBrush = new SolidColorBrush(Color.FromArgb(0x88, 0xFF, 0x00, 0xFF));
            }

            UpdateLimits();
            // AnnotationData.AddRange(new[] {_test, _xUpperLimit, _xLowerLimit, _yLowerLimit, _yUpperLimit});
            AnnotationData.AddRange(new[] { _xUpperLimit, _xLowerLimit, _yLowerLimit, _yUpperLimit });
            // AnnotationData.AddRange(new[] {_yUpperLimit});
        }

        private RectangleAnnotationData<double, double> ConstructLimitAnnotationData(string xAxis, string yAxis)
        {
            return new RectangleAnnotationData<double, double>(ChartId, xAxis, yAxis)
            {
                BorderThickness = new Thickness(0.5),
                Margin = default(Thickness),
                BorderBrush = new SolidColorBrush(Color.FromArgb(0x77, 0x00, 0x00, 0x00)),
                BackgroundBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x00, 0x00, 0x00)),
                CornerRadius = new CornerRadius(1),
                PositionType = PositionType.Absolute,
            };
        }
    }
}
