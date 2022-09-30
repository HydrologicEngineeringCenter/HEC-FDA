using HEC.Plotting.Core;
using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using SciChart.Charting.Model.ChartSeries;
using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using AxisAlignment = HEC.Plotting.Core.DataModel.AxisAlignment;
using HEC.FDA.Model.paireddata;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor.ChartControls
{
    public abstract class ChartControlBase : BaseViewModel
    {
        private static readonly double[] ProbabilityTicks = new double[] { 0.999, 0.99, 0.9, 0.5, 0.1, 0.01, 0.001 };

        private string _xAxisLabel;
        private string _yAxisLabel;
        private string _seriesName;

        public UncertainPairedData Function { get; set; }
        public SciChart2DChartViewModel ChartVM { get; private set; }

        //This will probably become 3 lines
        private readonly NumericLineData _data;
        private bool _flipXY;

        public ChartControlBase(string chartModelUniqueName, string xAxisLabel, string yAxisLabel, string seriesName, bool flipXY = false, bool useProbabilityX = false,
            AxisAlignment xAxisAlignment = AxisAlignment.Bottom, AxisAlignment yAxisAlignment = AxisAlignment.Left)
        {
            _flipXY = flipXY;
            ChartVM = new SciChart2DChartViewModel(chartModelUniqueName)
            {
                LegendVisibility = Visibility.Collapsed,
                IsVisualXcelleratorEnabled = false,                     //Probability axis has difficulty rendering lines with the visual xcellerator options.
            };

            _seriesName = seriesName;
            _xAxisLabel = xAxisLabel;
            _yAxisLabel = yAxisLabel;

            _data = new NumericLineData(getXValues(), getYValues(), chartModelUniqueName, _seriesName, _xAxisLabel, _yAxisLabel, PlotType.Line)
            {
                XAxisAlignment = xAxisAlignment,
                YAxisAlignment = yAxisAlignment,
                StrokeColor = Colors.Black,
                UseProbabilityXAxis = useProbabilityX,
                FlipXAxisValues = useProbabilityX,
                CustomProbabilityTicks = ProbabilityTicks,
            };
            
            ChartVM.LineData.Add(_data);
        }

        public void UpdateYAxisLabel(string label)
        {
            _yAxisLabel = label;
            _data.YAxisName = label;
        }

        public void SetMinMax(Tuple<double, double> minMaxX, Tuple<double, double> minMaxY)
        {
            AxisCoreViewModel vmX = GetAxisViewModel(Axis.X);
            AxisCoreViewModel vmY = GetAxisViewModel(Axis.Y);
            
            UpdateVisibleRange(minMaxX, vmX);
            UpdateVisibleRange(minMaxY, vmY);
        }

        private static void UpdateVisibleRange(Tuple<double, double> minMax, AxisCoreViewModel vm)
        {
            if (vm?.VisibleRange != null)
            {
                double delta = minMax.Item2 - minMax.Item1;
                double minDeltaGrowth = vm.GrowBy.Min * delta;
                double maxDeltaGrowth = vm.GrowBy.Max * delta;
                vm.VisibleRange = vm.VisibleRange.SetMinMax(minMax.Item1 - minDeltaGrowth, minMax.Item2 + maxDeltaGrowth);
            }
        }

        private AxisCoreViewModel GetAxisViewModel(Axis axis)
        {
            //*lots* of assumptions going on in here, but it's for the betterment of society, trust me. -Ryan Miles
            AxisCoreViewModel axisVm = null;
            SciChartAxisViewModel vm = ChartVM.AxisViewModel as SciChartAxisViewModel;
            if (vm != null)
            {
                switch (axis)
                {
                    case Axis.X:
                        axisVm = vm.XAxisViewModels[0] as AxisCoreViewModel;
                        break;
                    case Axis.Y:
                        axisVm = vm.YAxisViewModels[0] as AxisCoreViewModel;
                        break;
                    default:
                        throw new NotSupportedException("2D chart only supports X and Y axes.");
                }
            }
            return axisVm;
        }
        
        public Tuple<double, double> GetMinMax(Axis axis)
        {
            double[] values;

            switch (axis)
            {
                case Axis.X:
                    values = _data.XData;
                    break;
                case Axis.Y:
                    values = _data.YData;
                    break;
                default:
                    throw new NotSupportedException("2D Chart only supports X and Y values.");
            }

            Tuple<double, double> output = new Tuple<double, double>(double.PositiveInfinity, double.NegativeInfinity);

            if (values.Length > 0)
            {
                output = new Tuple<double, double>(values.Min(), values.Max());
            }

            return output;
        }

        public void RefreshChartViewModel()
        {
            ChartVM = new SciChart2DChartViewModel(ChartVM);
        }

        public void UpdatePlotData(UncertainPairedData function)
        {
            Function = function;
        }

        public virtual void Plot()
        {
            if (!_flipXY)
            {
                _data.SetValues(getXValues(), getYValues());
            }
            else
            {
                _data.SetValues(getYValues(), getXValues());
            }
        }

        private double[] getXValues()
        {
            List<double> xVals = new List<double>();
            if (Function != null)
            {
                if (_data.FlipXAxisValues)
                {
                    foreach (double x in Function.Xvals)
                    {
                        xVals.Add(1 - x);
                    }
                }
                else
                {
                    xVals.AddRange(Function.Xvals);
                }
            }

            return xVals.ToArray();
        }
        private double[] getYValues()
        {
            List<double> yVals = new List<double>();
            if (Function != null)
            {
                IDistribution[] coordinates = Function.Yvals;
                foreach (IDistribution coord in coordinates)
                {
                    yVals.Add(coord.InverseCDF(.5));
                }
            }
            return yVals.ToArray();
        }
    }
}
