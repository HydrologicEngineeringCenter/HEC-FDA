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
            Chart2D chart = new Chart2D(vm.ChartViewModel);

            //add the chart to the UI
            main_grd.Children.Add(chart);
            Grid.SetRow(chart, 0);
            Grid.SetColumn(chart, 1);

            //plot the line data
            vm.PlotLineData();
        }

        //private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        //{
        //    //Add spaces between words and make multiline on "_". 
        //     string header = Regex.Replace(e.Column.Header.ToString(), "(\\B[A-Z])", " $1");
        //    string multiLineHeader = Regex.Replace(header, "_ ", Environment.NewLine);
        //    e.Column.Header = multiLineHeader;

        //    //set the last column to be star width to remove the empty final column
        //    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

        //}
    }
}
