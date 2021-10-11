using HEC.Plotting.Core;
using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.Controller;
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
using ViewModel.ImpactAreaScenario.Editor;

namespace View.ImpactAreaScenario.Editor
{
    /// <summary>
    /// Interaction logic for IASEditor.xaml
    /// </summary>
    public partial class IASEditor : UserControl
    {
        private Chart2DController _controller;
        private Chart2D _flowFreqChart;
        private Chart2D _ratingChart;
        private Chart2D _stageDamageChart;
        private Chart2D _damageFreqChart;

        private bool _plotsHaveBeenAdded;

        public IASEditor()
        {
            InitializeComponent();
        }

        private void addThresholdBtn_Click(object sender, RoutedEventArgs e)
        {
            IASEditorVM vm = (IASEditorVM)this.DataContext;
            vm.AddThresholds();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            IASEditorVM vm = (IASEditorVM)this.DataContext;

            //flow frequency
            vm.FlowFreqChartVM = new HEC.Plotting.SciChart2D.ViewModel.SciChart2DChartViewModel(vm.FlowFreqChartVM);
            _flowFreqChart = new Chart2D(vm.FlowFreqChartVM);

            //rating
            vm.RatingChartVM = new HEC.Plotting.SciChart2D.ViewModel.SciChart2DChartViewModel(vm.RatingChartVM);
            _ratingChart = new Chart2D(vm.RatingChartVM);

            //stage damage
            vm.StageDamageChartVM = new HEC.Plotting.SciChart2D.ViewModel.SciChart2DChartViewModel(vm.StageDamageChartVM);
            _stageDamageChart = new Chart2D(vm.StageDamageChartVM);

            //damage freq
            vm.DamageFreqChartVM = new HEC.Plotting.SciChart2D.ViewModel.SciChart2DChartViewModel(vm.DamageFreqChartVM);
            _damageFreqChart = new Chart2D(vm.DamageFreqChartVM);

        }

        private void plotBtn_Click(object sender, RoutedEventArgs e)
        {
            IASEditorVM vm = (IASEditorVM)this.DataContext;

            if (!_plotsHaveBeenAdded)
            {
                //flow frequency                
                PlotsGrid.Children.Add(_flowFreqChart);
                Grid.SetRow(_flowFreqChart, 0);
                Grid.SetColumn(_flowFreqChart, 1);

                //rating
                PlotsGrid.Children.Add(_ratingChart);
                Grid.SetRow(_ratingChart, 0);
                Grid.SetColumn(_ratingChart, 0);

                //stage damage
                PlotsGrid.Children.Add(_stageDamageChart);
                Grid.SetRow(_stageDamageChart, 1);
                Grid.SetColumn(_stageDamageChart, 0);

                //damage freq
                PlotsGrid.Children.Add(_damageFreqChart);
                Grid.SetRow(_damageFreqChart, 1);
                Grid.SetColumn(_damageFreqChart, 1);

                _plotsHaveBeenAdded = true;
            }


            //todo: link the charts everytime or only once?
            LinkTheCharts();
            vm.Plot();
        }

        private Chart2D[] GetChartsThatAreShowing()
        {
            List<Chart2D> charts = new List<Chart2D>();
            charts.Add(_flowFreqChart);
            charts.Add(_ratingChart);
            charts.Add(_stageDamageChart);
            charts.Add(_damageFreqChart);
            return charts.ToArray();
        }

        private void LinkTheCharts()
        {
            Guid guid = Guid.NewGuid();
            Chart2D[] charts = GetChartsThatAreShowing();
            var provider = new Chart2DProvider(GetChartsThatAreShowing);
            _controller = new Chart2DController(provider);
            foreach (Chart2D chart in charts)
            {
                _controller.RegisterChart(chart);
            }
            _controller.StateController.ContextMenuEnabled = true;
            _controller.IsBobberEnabled = true;
            _controller.IsPanningEnabled = false;
            _controller.IsSelectionEnabled = false;
            _controller.IsRectangleZoomEnabled = false;
            _controller.LegendVisibility = Visibility.Collapsed;

            //bind the chart axes together
            _controller.BindChart(ShareableAxis.Y, _flowFreqChart, _ratingChart);
            _controller.BindChart(ShareableAxis.X, _ratingChart, _stageDamageChart);
            _controller.BindChart(ShareableAxis.Y, _stageDamageChart, _damageFreqChart);

            //todo: not sure what purpose the mouse group has
            //Set up the mouse event group - this keeps the mouse events all in sync with the others.
            //_flowFreqChart.SetVerticalMouseEventGroup(guid.ToString());
            //_ratingChart.SetVerticalMouseEventGroup(guid.ToString());
            //_stageDamageChart.SetVerticalMouseEventGroup(guid.ToString());
            //_damageFreqChart.SetVerticalMouseEventGroup(guid.ToString());

            // look at SetMinMaxAxisValues in individual linked plot control

        }


    }
}
