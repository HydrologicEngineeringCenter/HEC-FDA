using HEC.Plotting.SciChart2D.Charts;
using System.Windows;
using System.Windows.Controls;
using HEC.FDA.ViewModel.Alternatives.Results;
using HEC.Plotting.SciChart2D.ViewModel;

namespace HEC.FDA.View.Alternatives.Results
{
    /// <summary>
    /// Interaction logic for EADDamageWithUncertainty.xaml
    /// </summary>
    public partial class EADDamageWithUncertainty : UserControl
    {
        private Chart2D _chart;
        public EADDamageWithUncertainty()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //link the plot with its chart view model
            DamageWithUncertaintyVM vm = (DamageWithUncertaintyVM)this.DataContext;

            frequency_textblock.Text = vm.ProbabilityExceedsValueLabel;

            //because this UI gets loaded every time the user switches and comes back to this, we were getting
            //an exception. We need to create a new chart view model every time it gets loaded and set it in the vm.
            vm.ChartViewModel = new SciChart2DChartViewModel(vm.ChartViewModel);
            _chart = new Chart2D(vm.ChartViewModel);
            _chart.EnableBobber(false);

            //add the chart to the UI
            main_grd.Children.Add(_chart);
            Grid.SetRow(_chart, 0);
            Grid.SetRowSpan(_chart, 2);
            Grid.SetColumn(_chart, 1);

            vm.PlotHistogram();
            if(!vm.HistogramVisible)
            {
                _chart.Visibility = Visibility.Collapsed;
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            //The alternative has three different DamageWithUncertaintyVM. The content control caches this view
            //and was not updating the histogram plot because the chart that we added above was not switching
            //its data context to the new vm. We do that here.
            if (e.NewValue is DamageWithUncertaintyVM vm)
            {
                if (_chart != null)
                {
                    _chart.DataContext = vm.ChartViewModel;
                    if (!vm.HistogramVisible)
                    {
                        _chart.Visibility = Visibility.Collapsed;
                    }
                }
                frequency_textblock.Text = vm.ProbabilityExceedsValueLabel;
            }          
        }
    }
}
