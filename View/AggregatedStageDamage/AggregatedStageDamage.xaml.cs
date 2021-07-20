using FdaViewModel.AggregatedStageDamage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace View.AggregatedStageDamage
{
    /// <summary>
    /// Interaction logic for AggregatedStageDamage.xaml
    /// </summary>
    public partial class AggregatedStageDamage : UserControl
    {
        public AggregatedStageDamage()
        {
            InitializeComponent();
            manual_rad.IsChecked = true;
        }

        private void manual_rad_Checked(object sender, RoutedEventArgs e)
        {
            manual_control.Visibility = Visibility.Visible;
            calculated_control.Visibility = Visibility.Hidden;
        }

        private void calculated_rad_Checked(object sender, RoutedEventArgs e)
        {
            manual_control.Visibility = Visibility.Hidden;
            calculated_control.Visibility = Visibility.Visible;
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            AggregatedStageDamageEditorVM vm = (AggregatedStageDamageEditorVM)this.DataContext;
            vm.Save();
        }
    }
}
