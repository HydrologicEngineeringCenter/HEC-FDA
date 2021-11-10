using HEC.Plotting.SciChart2D.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
