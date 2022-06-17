using HEC.FDA.ViewModel.ImpactAreaScenario.Results;
using HEC.Plotting.SciChart2D.Charts;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.ImpactAreaScenario.Results
{
    /// <summary>
    /// Interaction logic for PerformanceAEP.xaml
    /// </summary>
    public partial class PerformanceAEP : UserControl
    {
        public PerformanceAEP()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //link the plot with its chart view model
            PerformanceAEPVM vm = DataContext as PerformanceAEPVM;

            if (vm != null)
            {
                vm.ChartViewModel = new Plotting.SciChart2D.ViewModel.SciChart2DChartViewModel(vm.ChartViewModel);
                Chart2D _chart = new Chart2D(vm.ChartViewModel);
                //add the chart to the UI
                main_grd.Children.Add(_chart);
                Grid.SetRow(_chart, 0);
                Grid.SetColumn(_chart, 1);
            }
        }

    }
}
