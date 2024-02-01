using HEC.FDA.ViewModel.AggregatedStageDamage;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.AggregatedStageDamage
{
    /// <summary>
    /// Interaction logic for CalculatedStageDamageControl.xaml
    /// </summary>
    public partial class CalculatedStageDamageControl : UserControl
    {

        public CalculatedStageDamageControl()
        {
            InitializeComponent();
        }

        private async void calculate_btn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CalculatedStageDamageVM vm)
            {
                computeButton.IsEnabled = false;
                await vm.ComputeCurvesAsync();
                computeButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// This is to stretch the listview columns to fill the space as the user stretches the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImpactAreaFrequencyListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double columnPercent = .25;

            ListView listView = sender as ListView;
            List<double> columnPercents = new List<double>() { columnPercent, columnPercent, columnPercent, columnPercent };
            StretchListView(listView, columnPercents);
        }

        /// <summary>
        /// This is to stretch the listview columns to fill the space as the user stretches the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComputedCurves_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            List<double> columnPercents = new List<double>() { .1, .3, .3, .3 };
            StretchListView(listView, columnPercents);
        }


        private void StretchListView(ListView listView, List<double> columnPercent)
        {
            GridView gView = listView.View as GridView;
            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth - 15;

            if (workingWidth > 0)
            {
                for(int i = 0;i<columnPercent.Count;i++)
                {
                    gView.Columns[i].Width = workingWidth * columnPercent[i];
                }
            }
        }

    }
}
