using Functions;
using HEC.Plotting.Core;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Model;
using SciChart.Charting.Numerics.CoordinateCalculators;
using SciChart.Core.Extensions;
using SciChart.Core.Utility.Mouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FdaViewModel.Conditions
{
    public class FdaCrosshairChartModifier: ChartModifierBase
    {
        private readonly CrosshairData _crosshairData;
        private readonly bool _xLinePositive;
        private readonly bool _yLinePositive;
        private readonly Line _xLine = new Line() { Stroke = Brushes.Black, StrokeThickness = 1.0, };
        private readonly Line _xAxisLine = new Line() { Stroke = Brushes.Black, StrokeThickness = 1.0, };
        private readonly Line _yLine = new Line() { Stroke = Brushes.Black, StrokeThickness = 1.0, };
        private readonly Line _yAxisLine = new Line() { Stroke = Brushes.Black, StrokeThickness = 1.0, };
        private bool _frozen;

        public FdaCrosshairChartModifier(bool xLinePositive, bool yLinePositive, CrosshairData data)
        {
            _crosshairData = data;
            _xLinePositive = xLinePositive;
            _yLinePositive = yLinePositive;

            _crosshairData.PreviousUpdated += PreviousDataUpdated;
            _crosshairData.NextUpdated += NextDataUpdated;
        }

        private double ComputeYFromX(double x)
        {
            IOrdinate ord = IOrdinateFactory.Factory(x);
            double retval = double.NaN;
            try
            {
                IFunction sampledFunc = Sampler.Sample(_crosshairData.Function, .5);
                retval = sampledFunc.F(ord).Value();
            }
            catch(Exception e)
            {
                //an exception gets thrown if it is out of range

            }
            return retval;
            //var renderableSeries = ParentSurface.RenderableSeries[0];
            //var xAxis = ParentSurface.XAxes[0];
            //var xCoord = xAxis.GetCurrentCoordinateCalculator().GetCoordinate(x.ToDouble());
            //var hitTest = renderableSeries.HitTestProvider.HitTest(new Point(xCoord, 0), 10000, true);

            //IComparable output = double.NaN;
            //if (hitTest.IsHit)
            //{
            //    output = hitTest.YValue;
            //}

            //return output;
        }

        private double ComputeXFromY(double y)
        {
            IOrdinate ord = IOrdinateFactory.Factory(y);
            double retval = double.NaN;
            try
            {
                //if the function is distributed then we will throw an exception
                //when doing an inverseF(x), so we need to sample the function first.
                IFunction sampledFunc = Sampler.Sample(_crosshairData.Function, .5);
                retval = sampledFunc.InverseF(ord).Value();
                //retval = _crosshairData.Function.InverseF(ord).Value();
            }
            catch(Exception e)
            {
                //an exception gets thrown if it is out of range
            }
            return retval;

            //var renderableSeries = ParentSurface.RenderableSeries[0];
            //var yAxis = ParentSurface.YAxes[0];
            //var dataSeries = renderableSeries.DataSeries;
            //var yValues = dataSeries.YValues.OfType<IComparable>().ToList();
            //var upperIndex = -1;

            //for (var i = 0; i < yValues.Count; i++)
            //{
            //    if(y.CompareTo(yValues[i]) == 0)
            //    {
            //        upperIndex = i;
            //        break;
            //    }
            //    else if (y.CompareTo(yValues[i]) < 0)
            //    {
            //        upperIndex = i;
            //        break;
            //    }
            //}

            ////If it's 0 or -1, we're out of bounds
            //IComparable output = double.NaN;
            //if (upperIndex > 0)
            //{
            //    var xAxis = ParentSurface.XAxes[0];
            //    var xValues = dataSeries.XValues.OfType<IComparable>().ToList();
            //    var yCoord = yAxis.GetCurrentCoordinateCalculator().GetCoordinate(y.ToDouble());
            //    var xCoord = xAxis.GetCurrentCoordinateCalculator().GetCoordinate(xValues[upperIndex].ToDouble());
            //    var hitTest = renderableSeries.HitTestProvider.HitTest(new Point(xCoord, yCoord), 10000, true);

            //    if (hitTest.IsHit)
            //    {
            //        output = hitTest.XValue;
            //    }
            //}

            //return output;
        }

        private void UpdateFromData(SharedAxisCrosshairData data, Action<IComparable, IComparable> updateAction)
        {
            var crosshairData = data.OtherCrosshairData;
            //the other axis will be this chart's axis
            var axis = data.OtherAxis;
            double x = double.NaN;
            double y = double.NaN;
            switch (axis)
            {
                case Axis.X:
                    x = (double)crosshairData.XValue;
                    y = ComputeYFromX(x);
                    if(y == double.NaN)
                    {
                        return;
                    }
                    break;
                case Axis.Y:
                    y = (double)crosshairData.YValue;
                    x = ComputeXFromY(y);
                    if (x == double.NaN)
                    {
                        return;
                    }
                    break;
            }
            if(x!= double.NaN && y != double.NaN)
            {
                updateAction.Invoke(x, y);
            }
            
        }

        /// <summary>
        /// The next chart is calling into this chart.
        /// </summary>
        private void NextDataUpdated()
        {
            UpdateFromData(_crosshairData.Next, _crosshairData.UpdatePreviousValues);
        }

        private void PreviousDataUpdated()
        {
            UpdateFromData(_crosshairData.Previous, _crosshairData.UpdateNextValues);
        }

        public override void OnModifierMouseDown(ModifierMouseArgs e)
        {
            base.OnModifierMouseDown(e);
            if (e.MouseButtons == MouseButtons.Left)
            {
                _frozen = !_frozen;
                if (_frozen)
                {
                    _xLine.Stroke = Brushes.Cyan;
                    _yLine.Stroke = Brushes.Cyan;
                }
                else
                {
                    _xLine.Stroke = Brushes.Black;
                    _yLine.Stroke = Brushes.Black;
                }
            }
        }

        public override void OnModifierMouseMove(ModifierMouseArgs e)
        {
            base.OnModifierMouseMove(e);

            if (_frozen)
            {
                return;
            }


            if (!ModifierSurface.Children.Contains(_xLine) || !ModifierSurface.Children.Contains(_yLine))
            {
                ModifierSurface.Children.Add(_xLine);
                ModifierSurface.Children.Add(_yLine);
            }

            var xAxes = ParentSurface.XAxes;
            var yAxes = ParentSurface.YAxes;
            AddAxisLines(_xAxisLine, xAxes);
            AddAxisLines(_yAxisLine, yAxes);


            //Master is always true first.
            if (e.IsMaster)
            {
                OnMasterMouseMove(e);
            }
            else
            {
                OnSlaveMouseMove(e);
            }
        }

        private void AddAxisLines(Line axisLine, AxisCollection axes)
        {
            if (axes.Count > 0)
            {
                var children = axes[0].ModifierAxisCanvas.Children;
                if (!children.Contains(axisLine))
                {
                    children.Add(axisLine);
                }
            }
        }

        private void OnSlaveMouseMove(ModifierMouseArgs e)
        {
            //Assumes one axis and series
            var xAxis = ParentSurface.XAxes[0];
            var yAxis = ParentSurface.YAxes[0];

            var xCoordCalc = xAxis.GetCurrentCoordinateCalculator();
            var yCoordCalc = yAxis.GetCurrentCoordinateCalculator();

            var xComp = _crosshairData.XValue;
            var yComp = _crosshairData.YValue;

            if (xComp != null && yComp != null)
            {
                var x = xComp.ToDouble();
                var y = yComp.ToDouble();
                if (!double.IsNaN(x) && !double.IsNaN(y))
                {
                    var coordinate = new Point(xCoordCalc.GetCoordinate(x), yCoordCalc.GetCoordinate(y));

                    UpdateLineDataForPoint(xCoordCalc, yCoordCalc, coordinate);
                }
            }
        }

        private void UpdateLineDataForPoint(ICoordinateCalculator<double> xCoordCalc, ICoordinateCalculator<double> yCoordCalc,
            Point coordinate)
        {
            var xVisMin = xCoordCalc.VisibleMin;
            var xVisMax = xCoordCalc.VisibleMax;

            var yVisMin = yCoordCalc.VisibleMin;
            var yVisMax = yCoordCalc.VisibleMax;

            _xLine.X1 = coordinate.X;
            _xLine.X2 = coordinate.X;

            _xLine.Y1 = (_xLinePositive ? coordinate.Y  : yCoordCalc.GetCoordinate(yVisMin));
            _xLine.Y2 = (_xLinePositive ? yCoordCalc.GetCoordinate(yVisMax) : coordinate.Y) ;

            _yLine.Y1 = coordinate.Y;
            _yLine.Y2 = coordinate.Y;
            _yLine.X1 = _yLinePositive ? coordinate.X : xCoordCalc.GetCoordinate(xVisMin);
            _yLine.X2 = _yLinePositive ? xCoordCalc.GetCoordinate(xVisMax) : coordinate.X;
        }

        private void OnMasterMouseMove(ModifierMouseArgs e)
        {
            var mousePoint = GetPointRelativeTo(e.MousePoint, ModifierSurface);
            if (ParentSurface.XAxes.Count == 0 || ParentSurface.YAxes.Count == 0)
            {
                return;
            }

            //Assumes one axis
            var xAxis = ParentSurface.XAxes[0];
            var yAxis = ParentSurface.YAxes[0];
            //we might have multiple series depending on how many probabilities are included.
            //todo: i think it would be best if we could put the trackers on the series that is 
            //closest to the mouse.
            int numSeries = ParentSurface.RenderableSeries.Count;
            double middleNum = numSeries / 2;
            int middleSeries = (int)Math.Round(middleNum);
            var renderableSeries = ParentSurface.RenderableSeries[middleSeries];

            var xCoordCalc = xAxis.GetCurrentCoordinateCalculator();
            var yCoordCalc = yAxis.GetCurrentCoordinateCalculator();

            var hitTest = renderableSeries.HitTestProvider.HitTest(new Point(mousePoint.X, 0), 10000, true);
            if (hitTest.IsHit)
            {
                var coordinate = new Point(xCoordCalc.GetCoordinate(hitTest.XValue.ToDouble()),
                    yCoordCalc.GetCoordinate(hitTest.YValue.ToDouble()));

                UpdateLineDataForPoint(xCoordCalc, yCoordCalc, coordinate);

                _crosshairData.UpdateValuesInSharedPlots(hitTest.XValue, hitTest.YValue);
            }
        }
    }
}
