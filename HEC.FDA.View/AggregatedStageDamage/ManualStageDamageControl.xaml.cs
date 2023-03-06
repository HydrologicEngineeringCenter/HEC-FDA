using HEC.FDA.ViewModel.AggregatedStageDamage;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.AggregatedStageDamage
{
    /// <summary>
    /// Interaction logic for ManualStageDamageControl.xaml
    /// </summary>
    public partial class ManualStageDamageControl : UserControl
    {
        public ManualStageDamageControl()
        {
            InitializeComponent();
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ManualStageDamageVM vm)
            {
                vm.Add();
            }
        }

        private void copy_btn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ManualStageDamageVM vm)
            {
                vm.Copy();
            }
        }

        private void remove_btn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ManualStageDamageVM vm)
            {
                vm.Remove();
            }
        }

    }
}
