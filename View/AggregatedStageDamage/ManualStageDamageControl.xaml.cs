using FunctionsView.ViewModel;
using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using ViewModel.AggregatedStageDamage;

namespace View.AggregatedStageDamage
{
    /// <summary>
    /// Interaction logic for ManualStageDamageControl.xaml
    /// </summary>
    public partial class ManualStageDamageControl : UserControl
    {
        private Chart2D _lastChart;
        public ManualStageDamageControl()
        {
            InitializeComponent();
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ManualStageDamageVM vm)
            {
                vm.Add();
                linkChartViewModel();
            }
        }

        private void copy_btn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ManualStageDamageVM vm)
            {
                vm.Copy();
                linkChartViewModel();
            }
        }

        private void remove_btn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ManualStageDamageVM vm)
            {
                vm.Remove();
            }
        }

        private void linkChartViewModel()
        {
            if (DataContext is ManualStageDamageVM vm)
            {
                int rowIndex = vm.SelectedRowIndex;
                if (rowIndex >= 0)
                {
                    CoordinatesFunctionEditorVM editorVM = vm.Rows[rowIndex].EditorVM;

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
            if (DataContext is ManualStageDamageVM vm)
            {
                vm.SelectedRowChanged += Vm_SelectedRowChanged;
                linkChartViewModel();
            }
        }

        private void Vm_SelectedRowChanged(object sender, EventArgs e)
        {
            linkChartViewModel();
        }
    }
}
