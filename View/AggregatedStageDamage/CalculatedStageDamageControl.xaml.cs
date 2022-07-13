using HEC.FDA.ViewModel.AggregatedStageDamage;
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

        private void calculate_btn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CalculatedStageDamageVM vm)
            {
                vm.CalculateCurves();
            }
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;
            
            // take into account vertical scrollbar
            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth - 5; 
            var col1 = 0.33;
            var col2 = 0.33;
            var col3 = 0.33;

            if (workingWidth > 0)
            {
                gView.Columns[0].Width = workingWidth * col1;
                gView.Columns[1].Width = workingWidth * col2;
                gView.Columns[2].Width = workingWidth * col3;
            }
        }

        private void ListView_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            // take into account vertical scrollbar
            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth - 15;
            var col1 = 0.1;
            var col2 = 0.3;
            var col3 = 0.3;
            var col4 = 0.3;

            if (workingWidth > 0)
            {
                gView.Columns[0].Width = workingWidth * col1;
                gView.Columns[1].Width = workingWidth * col2;
                gView.Columns[2].Width = workingWidth * col3;
                gView.Columns[3].Width = workingWidth * col4;
            }

        }
    }
}
