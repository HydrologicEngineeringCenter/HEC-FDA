using ViewModel.Editors;
using ViewModel.Plots;
using HEC.Plotting.SciChart2D.Charts;
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
using ViewModel.Conditions;
using HEC.Plotting.SciChart2D.ViewModel;

namespace View.Plots
{
    /// <summary>
    /// Interaction logic for IndividualSciChart.xaml
    /// </summary>
    public partial class IndividualSciChart : UserControl
    {
        public IndividualSciChart()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            IIndividualLinkedPlotWrapper vm = (IIndividualLinkedPlotWrapper)this.DataContext;
            IndividualLinkedPlotVM editorVM = vm.PlotVM;
            ConditionChartViewModel viewModel = new ConditionChartViewModel(editorVM.CoordinatesChartViewModel);
            Chart2D chart = new Chart2D(viewModel);

            editorVM.CoordinatesChartViewModel = viewModel;
            //the two lines below this will allow me to override the visual range from the IndividualLinkedPlotControl.xaml.cs when linking charts together.
            //chart.GetAxes(HEC.Plotting.Core.Axis.Y)[0].AutoRange = SciChart.Charting.Visuals.Axes.AutoRange.Once;
            //chart.GetAxes(HEC.Plotting.Core.Axis.X)[0].AutoRange = SciChart.Charting.Visuals.Axes.AutoRange.Once;

            if (viewModel.AxisViewModel is SciChartAxisViewModel axisVm)
            {
                axisVm.YAxisViewModels[0].AxisTitle = editorVM.YAxisLabel;
                axisVm.XAxisViewModels[0].AxisTitle = editorVM.XAxisLabel;
            }
            //chart.ti editorVM.Title;

            //Binding myBinding = new Binding("EditorVM.CoordinatesChartViewModel");
            //myBinding.Source = this.DataContext;
            //chart.SetBinding(Chart2D.DataContextProperty, myBinding);

            Main_Grid.Children.Add(chart);
            //Grid.SetColumn(chart, 2);


            //find parent and add this plot to its selectedPlot property.
            ContentControl parentControl = IndividualLinkedPlotControl.FindParent<ContentControl>(this);
            if (parentControl != null && parentControl.GetType() == typeof(ConditionsIndividualPlotWrapper))
            {
                parentControl = IndividualLinkedPlotControl.FindParent<ContentControl>(parentControl);
            }

            if (parentControl != null && parentControl.GetType() == typeof(IndividualLinkedPlotControl))
            {
                IndividualLinkedPlotControlVM controlVM = (IndividualLinkedPlotControlVM)parentControl.DataContext;

                //this.BaseFunction = controlVM.IndividualPlotWrapperVM.PlotVM.BaseFunction;
                //((IndividualLinkedPlotControl)parentControl).LinkedPlot = this;
                ((IndividualLinkedPlotControl)parentControl).Chart = chart;
                ((IndividualLinkedPlotControl)parentControl).UpdateThePlots();
            }
        }
    }
}
