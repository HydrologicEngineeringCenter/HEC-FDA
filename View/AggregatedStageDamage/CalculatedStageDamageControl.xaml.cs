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
    }
}
