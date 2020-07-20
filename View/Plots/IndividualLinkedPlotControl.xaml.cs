using FdaViewModel.Plots;
using HEC.Plotting.Core;
using HEC.Plotting.Core.DataModel;
using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.Controller;
using HEC.Plotting.SciChart2D.DataModel;
using Model.Inputs.Functions.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace View.Plots
{
    /// <summary>
    /// Interaction logic for IndividualLinkedPlotControl.xaml
    /// </summary>
    public partial class IndividualLinkedPlotControl : UserControl, ILinkedPlotControl
    {
        public static readonly DependencyProperty UpdatePlotsFromVMProperty = DependencyProperty.Register("UpdatePlotsFromVM", typeof(bool), typeof(IndividualLinkedPlotControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(UpdatePlotsFromVMCallBack)));


        public event EventHandler UpdatePlots;
        public event EventHandler PopImporterIntoPlot1;
        public event EventHandler PopPlotIntoPlot1;

        public event EventHandler PopImporterIntoPlot5;
        public event EventHandler PopPlotIntoPlot5;

        public bool UpdatePlotsFromVM
        {
            get { return (bool)GetValue(UpdatePlotsFromVMProperty); }
            set { SetValue(UpdatePlotsFromVMProperty, value); }
        }
        public ILinkedPlot LinkedPlot
        {
            get;
            
            set;
        }

        public Chart2D Chart
        {
            get;
            set;
        }

        public ImpactAreaFunctionEnum FunctionType 
        { 
            get
            {
                IndividualLinkedPlotControlVM vm = (IndividualLinkedPlotControlVM)this.DataContext;
                if (vm.IndividualPlotWrapperVM != null && vm.IndividualPlotWrapperVM.PlotVM != null
                    && vm.IndividualPlotWrapperVM.PlotVM.BaseFunction != null)
                {
                    return vm.IndividualPlotWrapperVM.PlotVM.BaseFunction.Type;
                }
                else
                {
                    return ImpactAreaFunctionEnum.NotSet;
                }
            }
       
        }

        public IndividualLinkedPlotControl()
        {
            InitializeComponent();
            
        }

        public void PopTheImporterIntoPlot5()
        {
            PopImporterIntoPlot5?.Invoke(this, new EventArgs());
        }

        public void PopThePlotIntoPlot5()
        {
            PopPlotIntoPlot5?.Invoke(this, new EventArgs());
        }
        public void PopTheImporterIntoPlot1()
        {
            PopImporterIntoPlot1?.Invoke(this, new EventArgs());
        }

        public void PopThePlotIntoPlot1()
        {
            PopPlotIntoPlot1?.Invoke(this, new EventArgs());
        }

       public void UpdateThePlots()
        {
            UpdatePlots?.Invoke(this, new EventArgs());
        }


        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            if(child == null) { return null; }
            T parent = VisualTreeHelper.GetParent(child) as T;

            if (parent != null)

                return parent;

            else

                return FindParent<T>(VisualTreeHelper.GetParent(child));
        }


        private static void UpdatePlotsFromVMCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndividualLinkedPlotControl owner = d as IndividualLinkedPlotControl;
            owner.UpdateThePlots();
        }

        //public void UnBindThePlot()
        //{

        //}

        public void BindToNextPlot(ILinkedPlotControl nextControl, Chart2DController controller, Guid guid )
        {
            ImpactAreaFunctionEnum thisType = FunctionType;
            ImpactAreaFunctionEnum nextType = nextControl.FunctionType;

            switch (thisType)
            {
                case ImpactAreaFunctionEnum.InflowFrequency:
                    {
                        //if i am inflow frequency, then i can only link to inflow outflow or to rating
                        if (nextType == ImpactAreaFunctionEnum.InflowOutflow)
                        {
                            controller.BindChart(ShareableAxis.Y, Chart, ShareableAxis.X, nextControl.Chart);
                            Chart.SetVerticalMouseEventGroup(guid.ToString());
                            nextControl.Chart.SetVerticalMouseEventGroup(guid.ToString());
                            SetMinMaxAxisValues(Chart, nextControl, Axis.Y);
                        }
                        else if (nextType == ImpactAreaFunctionEnum.Rating)
                        {
                            controller.BindChart(ShareableAxis.Y, Chart, nextControl.Chart);
                            Chart.SetVerticalMouseEventGroup(guid.ToString());
                            nextControl.Chart.SetVerticalMouseEventGroup(guid.ToString());
                            SetMinMaxAxisValues(Chart, nextControl, Axis.Y);
                        }
                        break;
                    }
                case ImpactAreaFunctionEnum.InflowOutflow:
                    {
                        
                        if (nextType == ImpactAreaFunctionEnum.Rating)
                        {
                            controller.BindChart(ShareableAxis.Y, Chart, nextControl.Chart);
                            Chart.SetVerticalMouseEventGroup(guid.ToString());
                            nextControl.Chart.SetVerticalMouseEventGroup(guid.ToString());
                            SetMinMaxAxisValues(Chart, nextControl, Axis.Y);
                        }
                        break;
                    }
                case ImpactAreaFunctionEnum.Rating:
                    {
                        if (nextType == ImpactAreaFunctionEnum.ExteriorInteriorStage)
                        {
                            controller.BindChart(ShareableAxis.X, Chart, nextControl.Chart);
                            Chart.SetVerticalMouseEventGroup(guid.ToString());
                            nextControl.Chart.SetVerticalMouseEventGroup(guid.ToString());
                            SetMinMaxAxisValues(Chart, nextControl, Axis.X);
                        }
                        else if (nextType == ImpactAreaFunctionEnum.InteriorStageDamage)
                        {
                            controller.BindChart(ShareableAxis.X, Chart, nextControl.Chart);
                            Chart.SetVerticalMouseEventGroup(guid.ToString());
                            nextControl.Chart.SetVerticalMouseEventGroup(guid.ToString());
                            SetMinMaxAxisValues(Chart, nextControl, Axis.X);



                        }
                        break;
                    }
                case ImpactAreaFunctionEnum.ExteriorInteriorStage:
                    {
                        if (nextType == ImpactAreaFunctionEnum.InteriorStageDamage)
                        {
                            controller.BindChart(ShareableAxis.Y, Chart, nextControl.Chart);
                            Chart.SetVerticalMouseEventGroup(guid.ToString());
                            nextControl.Chart.SetVerticalMouseEventGroup(guid.ToString());
                            SetMinMaxAxisValues(Chart, nextControl, Axis.Y);
                        }
                        break;
                    }
                case ImpactAreaFunctionEnum.InteriorStageDamage:
                    {
                        if (nextType == ImpactAreaFunctionEnum.DamageFrequency)
                        {
                            controller.BindChart(ShareableAxis.Y, Chart, nextControl.Chart);
                            Chart.SetVerticalMouseEventGroup(guid.ToString());
                            nextControl.Chart.SetVerticalMouseEventGroup(guid.ToString());
                            SetMinMaxAxisValues(Chart, nextControl, Axis.Y);
                        }
                        break;
                    }
            }
        }


        /// <summary>
        /// Sets the min and max values between two charts based on a shared axis.
        /// </summary>
        /// <param name="chart1"></param>
        /// <param name="nextControl"></param>
        /// <param name="axis"></param>
        private void SetMinMaxAxisValues(Chart2D chart1, ILinkedPlotControl nextControl, Axis axis)
        {
            Chart2D chart2 = nextControl.Chart;
            double[] minMax = GetMinMaxValueOnAxes(chart1, chart2, axis);
            var chart1Axis = chart1.GetAxes(axis);
            var chart2Axis = chart2.GetAxes(axis);

            //i need to get the line data. I need to cast to a numericlinedata and then call useXlogAxis.
            //ILineData data = chart1.ChartViewModel2DChart.LineData[0];
            //i think all y axes are log. so lets just double the max y value to create the buffer.

            //maybe add 5% on either side of the min/max for a margin around the plot
            double marginPercent = .05;
            double min = minMax[0];
            double max = minMax[1];
            double margin = (max - min) * marginPercent;

            if(axis == Axis.Y)
            {
                max = 300000;    
            }

            chart1Axis[0].VisibleRange.SetMinMax(min-margin, max+margin);
            chart2Axis[0].VisibleRange.SetMinMax(min - margin, max + margin);

        }
      

        private double[] GetMinMaxValueOnAxes(Chart2D chart1, Chart2D chart2, Axis axis)
        {
            double[] minMaxChart1 = GetMinMaxValueOnAxes(chart1, axis);
            double[] minMaxChart2 = GetMinMaxValueOnAxes(chart2, axis);

            double min = minMaxChart1[0];
            if(minMaxChart2[0] < min)
            {
                min = minMaxChart2[0];
            }

            double max = minMaxChart1[1];
            if(minMaxChart2[1]>max)
            {
                max = minMaxChart2[1];
            }

            return new double[] { min, max };
        }

        //i was thinking that i could get the min and the max from the vm side but that wouldn't work for the next control 
        //because i would not have that vm. I guess i could try to pass those values in from the ConditionsPlotEditor.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="axis"></param>
        /// <returns>min value and then max value</returns>
        private double[] GetMinMaxValueOnAxes(Chart2D chart, Axis axis)
        {
            //var xaxes = chart.GetAxes(axis);
            ////var nextAxes = nextControl.Chart.GetAxes(Axis.X);
            //var myDataRange = xaxes[0].VisibleRange.AsDoubleRange();
            //double min = myDataRange.Min;
            //double max = myDataRange.Max;
            double[] retval = new double[2];
            NumericLineData minn = (NumericLineData)chart.ViewModel.LineData.First();

            if (axis == Axis.X)
            {
                 retval[0]= minn.MinX;
                 retval[1]= minn.MaxX;
            }
            else
            {
                retval[0] = minn.MinY;
                retval[1] = minn.MaxY;
            }
            return retval;

            //var minValue = myDataRange.Min;
            //var maxValue = myDataRange.Max;
            //if (nextDataRange.Min < minValue)
            //{
            //    minValue = nextDataRange.Min;
            //}

            //if (nextDataRange.Max > maxValue)
            //{
            //    maxValue = nextDataRange.Max;
            //}
        }


    }
}
