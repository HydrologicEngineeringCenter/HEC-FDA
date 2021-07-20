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
using FdaViewModel.AggregatedStageDamage;
using FunctionsView.ViewModel;
using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.ViewModel;

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
            ManualStageDamageVM vm = (ManualStageDamageVM)this.DataContext;
            vm.Add();
            lst.SelectedIndex = lst.Items.Count - 1;
            linkChartViewModel();
        }

        private void copy_btn_Click(object sender, RoutedEventArgs e)
        {
            ManualStageDamageVM vm = (ManualStageDamageVM)this.DataContext;
            vm.Copy();
            lst.SelectedIndex = lst.Items.Count - 1;
            linkChartViewModel();
        }

        private void remove_btn_Click(object sender, RoutedEventArgs e)
        {
            ManualStageDamageVM vm = (ManualStageDamageVM)this.DataContext;
            vm.Remove();
        }

        private void linkChartViewModel()
        {
            ManualStageDamageVM vm = (ManualStageDamageVM)this.DataContext;
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ManualStageDamageVM vm = (ManualStageDamageVM)this.DataContext;
            vm.SelectedRowChanged += Vm_SelectedRowChanged;
            //make sure the first row is selected
            lst.SelectedIndex = lst.Items.Count - 1;
            linkChartViewModel();

        }

        private void Vm_SelectedRowChanged(object sender, EventArgs e)
        {
            linkChartViewModel();
        }

        //private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var context = DataContext as ManualStageDamageVM;
        //    context.
        //}
    }
}
