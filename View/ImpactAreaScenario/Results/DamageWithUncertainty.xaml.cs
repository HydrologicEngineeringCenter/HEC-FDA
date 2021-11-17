using HEC.Plotting.SciChart2D.Charts;
using System.Windows;
using System.Windows.Controls;
using ViewModel.ImpactAreaScenario.Results;

namespace View.ImpactAreaScenario.Results
{
    /// <summary>
    /// Interaction logic for DamageWithUncertainty.xaml
    /// </summary>
    public partial class DamageWithUncertainty : UserControl
    {
        private Chart2D _Chart;
        public DamageWithUncertainty()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //link the plot with its chart view model
            DamageWithUncertaintyVM vm = DataContext as DamageWithUncertaintyVM;

            if (_Chart == null && vm != null)
            {
                _Chart = new Chart2D(vm.ChartViewModel);
                //add the chart to the UI
                main_grd.Children.Add(_Chart);
                Grid.SetRow(_Chart, 0);
                Grid.SetRowSpan(_Chart, 2);
                Grid.SetColumn(_Chart, 1);
            }
            //plot the histogram
            vm?.PlotHistogram();

        }

    }
}
