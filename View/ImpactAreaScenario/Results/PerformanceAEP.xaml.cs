using HEC.FDA.ViewModel.ImpactAreaScenario.Results;
using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.ImpactAreaScenario.Results
{
    /// <summary>
    /// Interaction logic for PerformanceAEP.xaml
    /// </summary>
    public partial class PerformanceAEP : UserControl
    {
        private Chart2D _chart;
        public PerformanceAEP()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //link the plot with its chart view model
            if( DataContext is PerformanceAEPVM vm)
            {
                vm.ChartViewModel = new SciChart2DChartViewModel(vm.ChartViewModel);
                _chart = new Chart2D(vm.ChartViewModel);
                //add the chart to the UI
                main_grd.Children.Add(_chart);
                Grid.SetRow(_chart, 0);
                Grid.SetColumn(_chart, 1);
                Grid.SetRowSpan(_chart, 2);

                if(!vm.HistogramVisible)
                {
                    _chart.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is PerformanceAEPVM vm && _chart != null)
            {
                _chart.DataContext = vm.ChartViewModel;
                if (!vm.HistogramVisible)
                {
                    _chart.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
