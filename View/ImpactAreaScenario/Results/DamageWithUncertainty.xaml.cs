using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
                Chart2D chart = new Chart2D(vm.ChartViewModel);
                //add the chart to the UI
                main_grd.Children.Add(chart);
                Grid.SetRow(chart, 0);
                Grid.SetRowSpan(chart, 2);
                Grid.SetColumn(chart, 1);
                //plot the histogram
                vm.PlotHistogram();
            }

        }

    }
}
