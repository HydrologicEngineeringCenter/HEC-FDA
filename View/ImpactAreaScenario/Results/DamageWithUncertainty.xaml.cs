using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.ViewModel;
using System.Windows;
using System.Windows.Controls;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results;

namespace HEC.FDA.View.ImpactAreaScenario.Results
{

    /// <summary>
    /// Interaction logic for DamageWithUncertainty.xaml
    /// </summary>
    public partial class DamageWithUncertainty : UserControl
    {
        private Chart2D _chart;

        public DamageWithUncertainty()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //link the plot with its chart view model
            DamageWithUncertaintyVM vm = DataContext as DamageWithUncertaintyVM;

            if (vm != null)
            {
                vm.ChartViewModel = new SciChart2DChartViewModel(vm.ChartViewModel);
                _chart = new Chart2D(vm.ChartViewModel);
                //add the chart to the UI
                main_grd.Children.Add(_chart);
                Grid.SetRow(_chart, 0);
                Grid.SetRowSpan(_chart, 2);
                Grid.SetColumn(_chart, 1);
                if (!vm.HistogramVisible)
                {
                    _chart.Visibility = Visibility.Collapsed;
                }
            }

        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is DamageWithUncertaintyVM vm && _chart != null)
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
