using ViewModel.AggregatedStageDamage;
using FunctionsView.ViewModel;
using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.ViewModel;
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

namespace View.AggregatedStageDamage
{
    /// <summary>
    /// Interaction logic for CalculatedStageDamageControl.xaml
    /// </summary>
    public partial class CalculatedStageDamageControl : UserControl
    {
        private Chart2D _lastChart;
        private Chart2D _Chart;


        public CalculatedStageDamageControl()
        {
            InitializeComponent();
            
            
        }

        private void linkChartViewModel()
        {
            CalculatedStageDamageVM vm = (CalculatedStageDamageVM)DataContext;
            int rowIndex = vm.SelectedRowIndex;
            if (rowIndex >= 0)
            {
                CoordinatesFunctionEditorVM editorVM = vm.Rows[rowIndex].EditorVM;

                //SciChart2DChartViewModel sciChart2DChartViewModel = new SciChart2DChartViewModel(editorVM.CoordinatesChartViewModel);
                _Chart = new Chart2D(editorVM.CoordinatesChartViewModel);
                //editorVM.CoordinatesChartViewModel = sciChart2DChartViewModel;

                if (_lastChart != null)
                {
                    editorGrid.Children.Remove(_lastChart);
                }
                _lastChart = _Chart;
                editorGrid.Children.Add(_Chart);
                Grid.SetColumn(_Chart, 2);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CalculatedStageDamageVM vm = (CalculatedStageDamageVM)this.DataContext;
            vm.SelectedRowChanged += Vm_SelectedRowChanged;
            //make sure the first row is selected
            lst.SelectedIndex = lst.Items.Count - 1;
            linkChartViewModel();
        }

        private void Vm_SelectedRowChanged(object sender, EventArgs e)
        {
            linkChartViewModel();
        }

        private void calculate_btn_Click(object sender, RoutedEventArgs e)
        {
            CalculatedStageDamageVM vm = (CalculatedStageDamageVM)this.DataContext;
            vm.CalculateCurves();
            editorGrid.Visibility = Visibility.Visible;
            if (lst.Items.Count > 0)
            {
                lst.SelectedIndex = 0;
            }
        }
    }
}
