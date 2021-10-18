using Functions;
using HEC.Plotting.Core.DataModel;
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

        public CrosshairData CrosshairData { get; set; }

        public IFdaFunction Function { get; set; }
        public SciChart2DChartViewModel ChartVM { get; set; }
        protected string SeriesName { get; private set; }
        protected string YAxisLabel { get; private set; }
        protected string XAxisLabel { get; private set; }
        protected AxisAlignment XAxisAlignment { get; set; } = AxisAlignment.Default;
        protected AxisAlignment YAxisAlignment { get; set; } = AxisAlignment.Default;

        private readonly FdaCrosshairChartModifier _modifier;


        public ChartControlBase(string chartModelUniqueName, string xAxisLabel, string yAxisLabel, string seriesName, bool crosshairXPositive, bool crosshairYPositive)
        {
            ChartVM = new SciChart2DChartViewModel(chartModelUniqueName);
            _modifier = new FdaCrosshairChartModifier(crosshairXPositive, crosshairYPositive);
            ChartVM.ModifierGroup.ChildModifiers.Add(_modifier);

            SeriesName = seriesName;
            XAxisLabel = xAxisLabel;
            YAxisLabel = yAxisLabel;
        }

        public void SetFunction(IFdaFunction function)
        {
            Function = function;
            CrosshairData = new CrosshairData(Function);
            _modifier.CrosshairData = CrosshairData;
        }

        public void RefreshViewModel()
        {
            //The old ChartVM is guaranteed to be non-null.
            ChartVM.ModifierGroup.ChildModifiers.Remove(_modifier);
            ChartVM = new SciChart2DChartViewModel(ChartVM);
            ChartVM.ModifierGroup.ChildModifiers.Add(_modifier);
        }

        //public abstract void LinkPlots();

        public virtual void Plot()
        {
            if (Function != null)
            {
                SciLineData lineData = new NumericLineData(getXValues(), getYValues(), "asdf", SeriesName, XAxisLabel, YAxisLabel, PlotType.Line)
                {
                    XAxisAlignment = XAxisAlignment,
                    YAxisAlignment = YAxisAlignment,
                };
                ChartVM.LineData.Clear();
                ChartVM.LineData.Add(lineData);
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
    }
}
