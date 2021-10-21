using HEC.Plotting.Core;
using HEC.Plotting.Core.Charts;
using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.Controller;
using HEC.Plotting.SciChart2D.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ViewModel.ImpactAreaScenario.Editor;

namespace View.ImpactAreaScenario.Editor
{
    /// <summary>
    /// Interaction logic for IASPlotControl.xaml
    /// </summary>
    public partial class IASPlotControl : UserControl
    {
        private Chart2DController _controller;
        private Chart2D _flowFreqChart;
        private Chart2D _ratingChart;
        private Chart2D _stageDamageChart;
        private Chart2D _damageFreqChart;

        private IASPlotControlVM VM => DataContext as IASPlotControlVM;

        public IASPlotControl()
        {
            InitializeComponent();
        }

        private void InitializeCharts()
        {
            if (_controller == null)
            {
                ConstructCharts();
            }
            else
            {
                UpdateCharts();
            }
        }

        private void UpdateCharts()
        {
            IASPlotControlVM vm = VM;
            if(vm != null)
            {
                BindCharts();
            }
        }

        private void ConstructCharts()
        {
            _flowFreqChart = new Chart2D(VM.FrequencyRelationshipControl.ChartVM);
            _ratingChart = new Chart2D(VM.RatingRelationshipControl.ChartVM);
            _stageDamageChart = new Chart2D(VM.StageDamageControl.ChartVM);
            _damageFreqChart = new Chart2D(VM.DamageFrequencyControl.ChartVM);

            Chart2D[] charts = GetChartsThatAreShowing();
            var provider = new Chart2DProvider(GetChartsThatAreShowing);
            _controller = new Chart2DController(provider);
            BindCharts();
        }

        private void BindCharts()
        {
            _controller.IsBobberEnabled = false;
            _controller.IsSelectionEnabled = false;
            _controller.LegendVisibility = Visibility.Collapsed;

            //Update the controller, then register the charts, this doesn't do binding, but it keeps the options consistent.
            //Register chart updates the plot context, which is the only place to update the right click menu.
            //This updates the vertical chart group, we want to split that up.
            _controller.RegisterChart(_flowFreqChart, _ratingChart, _stageDamageChart, _damageFreqChart);

            //This fixes an issue where there's a thick bar in between the charts.
            //Guid guid = new Guid();
            //_flowFreqChart.SetVerticalChartGroup(guid.ToString());
            //_damageFreqChart.SetVerticalChartGroup(guid.ToString());

            _controller.BindChart(Visibility.Visible, ChartDirectionality.Vertical, ShareableAxis.X, _flowFreqChart, _damageFreqChart);
            _controller.BindChart(Visibility.Visible, ChartDirectionality.Vertical, ShareableAxis.X, _ratingChart, _stageDamageChart);

            _controller.BindChart(Visibility.Visible, ChartDirectionality.Horizontal, ShareableAxis.Y, _ratingChart, _flowFreqChart);
            _controller.BindChart(Visibility.Visible, ChartDirectionality.Horizontal, ShareableAxis.Y, _stageDamageChart, _damageFreqChart);
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            IASPlotControlVM vm = VM;
            if (_controller != null && vm != null)
            {
                _flowFreqChart.DataContext = vm.FrequencyRelationshipControl.ChartVM;
                _ratingChart.DataContext = vm.RatingRelationshipControl.ChartVM;
                _stageDamageChart.DataContext = vm.StageDamageControl.ChartVM;
                _damageFreqChart.DataContext = vm.DamageFrequencyControl.ChartVM;

                BindCharts();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            //Initialize the charts and make sure they're binding and stuff.
            InitializeCharts();

            //Flow freq
            AddChart(_flowFreqChart, 0, 1);

            //rating
            AddChart(_ratingChart, 0, 0);

            //stage damage
            AddChart(_stageDamageChart, 1, 0);

            //damage freq
            AddChart(_damageFreqChart, 1, 1);
        }

        private void AddChart(Control control, int row, int column)
        {
            if(!PlotsGrid.Children.Contains(control))
            {
                PlotsGrid.Children.Add(control);
                Grid.SetRow(control, row);
                Grid.SetColumn(control, column);
            }
        }

        private Chart2D[] GetChartsThatAreShowing()
        {
            return new []
            {
                _flowFreqChart,
                _ratingChart,
                _stageDamageChart,
                _damageFreqChart
            };
        }

        public void Plot()
        {
            //Values have been updated, and are ready to show.
            _flowFreqChart.ZoomToExtents(TimeSpan.Zero);
            _ratingChart.ZoomToExtents(TimeSpan.Zero);
            _stageDamageChart.ZoomToExtents(TimeSpan.Zero);
            _damageFreqChart.ZoomToExtents(TimeSpan.Zero);
        }
    }
}
