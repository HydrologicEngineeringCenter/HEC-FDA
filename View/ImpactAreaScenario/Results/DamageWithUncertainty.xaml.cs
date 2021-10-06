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
        
        public DamageWithUncertainty()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //link the plot with its chart view model
            DamageWithUncertaintyVM vm = (DamageWithUncertaintyVM)this.DataContext;

            //because this UI gets loaded every time the user switches and comes back to this, we were getting
            //an exception. We need to create a new chart view model every time it gets loaded and set it in the vm.
            vm.ChartViewModel = new HEC.Plotting.SciChart2D.ViewModel.SciChart2DChartViewModel(vm.ChartViewModel);
            Chart2D _chart = new Chart2D(vm.ChartViewModel);

            //add the chart to the UI
            main_grd.Children.Add(_chart);
            Grid.SetRow(_chart, 0);
            Grid.SetColumn(_chart, 1);

            //plot the line data
            vm.PlotLineData();

        }

    }
}
