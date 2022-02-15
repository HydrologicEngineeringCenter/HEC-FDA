using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.View.AggregatedStageDamage
{
    /// <summary>
    /// Interaction logic for CalculatedStageDamageControl.xaml
    /// </summary>
    public partial class CalculatedStageDamageControl : UserControl
    {
        private Chart2D _lastChart;

        public CalculatedStageDamageControl()
        {
            InitializeComponent();
        }

        private void linkChartViewModel()
        {
            if(DataContext is CalculatedStageDamageVM vm)
            {
                if (vm.Rows.Count > 0)
                {
                    CoordinatesFunctionEditorVM editorVM = vm.Rows[vm.SelectedRowIndex].EditorVM;

                    SciChart2DChartViewModel sciChart2DChartViewModel = new SciChart2DChartViewModel(editorVM.CoordinatesChartViewModel);
                    Chart2D chart = new Chart2D(sciChart2DChartViewModel);
                    editorVM.CoordinatesChartViewModel = sciChart2DChartViewModel;

                    if (_lastChart != null)
                    {
                        editorGrid.Children.Remove(_lastChart);
                    }
                    _lastChart = chart;
                    editorGrid.Children.Add(chart);
                    Grid.SetColumn(chart, 2);
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CalculatedStageDamageVM vm)
            {
                vm.SelectedRowChanged += Vm_SelectedRowChanged;
                linkChartViewModel();
            }
        }

        private void Vm_SelectedRowChanged(object sender, EventArgs e)
        {
            linkChartViewModel();
        }

        private void calculate_btn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CalculatedStageDamageVM vm)
            {
                vm.CalculateCurves();
            }
        }
    }
}
