using Functions;
using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario.Editor.ChartControls
{
    public abstract class ChartControlBase : BaseViewModel
    {
        private string _xAxisLabel;
        private string _yAxisLabel;
        private bool _crosshairXPositive;
        private bool _crosshairYPositive;
        private string _seriesName;

        public CrosshairData currentCrosshairData { get; set; }
        public CrosshairData nextCrosshairData { get; set; }
        public CrosshairData previousCrosshairData { get; set; }

        public IFdaFunction Function { get; set; }
        public SciChart2DChartViewModel ChartVM { get; set; }


        public ChartControlBase(string chartModelUniqueName, string xAxisLabel, string yAxisLabel, string seriesName, bool crosshairXPositive, bool crosshairYPositive)
        {
            ChartVM = new SciChart2DChartViewModel(chartModelUniqueName);

            _crosshairXPositive = crosshairXPositive;
            _crosshairYPositive = crosshairYPositive;
            _seriesName = seriesName;
            _xAxisLabel = xAxisLabel;
            _yAxisLabel = yAxisLabel;
        }

        public void UpdatePlotData(IFdaFunction function)
        {
            Function = function;
            currentCrosshairData = new CrosshairData(Function);
            ChartVM.ModifierGroup.ChildModifiers.Add(new FdaCrosshairChartModifier(_crosshairXPositive, _crosshairYPositive, currentCrosshairData));

        }

        //public abstract void LinkPlots();

        public virtual void Plot()
        {

            if (Function != null)
            {
                AddCrosshairs();

                List<ICoordinate> coordinates = Function.Coordinates;
                SciLineData lineData = new NumericLineData(getXValues(), getYValues(), "asdf", _seriesName, _xAxisLabel, _yAxisLabel, PlotType.Line);
                ChartVM.LineData.Set(new List<SciLineData>() { lineData });
            }
        }

        private double[] getXValues()
        {
                List<double> xVals = new List<double>();
            if (Function != null)
            {
                List<ICoordinate> coordinates = Function.Coordinates;
                foreach(ICoordinate coord in coordinates)
                {
                    xVals.Add(coord.X.Value());
                }
            }
            return xVals.ToArray();
        }
        private double[] getYValues()
        {
            List<double> yVals = new List<double>();
            if (Function != null)
            {
                List<ICoordinate> coordinates = Function.Coordinates;
                foreach (ICoordinate coord in coordinates)
                {
                    yVals.Add(coord.Y.Value());
                }
            }
            return yVals.ToArray();
        }

        public virtual void AddCrosshairs()
        {
            CrosshairData crosshairData = new CrosshairData(Function);
            ChartVM.ModifierGroup.ChildModifiers.Add(new FdaCrosshairChartModifier(_crosshairXPositive,_crosshairYPositive, crosshairData));
        }

    }
}
