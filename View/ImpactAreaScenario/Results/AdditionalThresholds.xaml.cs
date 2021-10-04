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
using ViewModel.ImpactAreaScenario.Results;

namespace View.ImpactAreaScenario.Results
{
    /// <summary>
    /// Interaction logic for AdditionalThresholds.xaml
    /// </summary>
    public partial class AdditionalThresholds : UserControl
    {
        public AdditionalThresholds()
        {
            InitializeComponent();
        }

        private void Add_btn_Click(object sender, RoutedEventArgs e)
        {
            AdditionalThresholdsVM vm = (AdditionalThresholdsVM)this.DataContext;
            vm.AddRow();
            this.UpdateLayout();
        }

        private void Copy_btn_Click(object sender, RoutedEventArgs e)
        {
            AdditionalThresholdsVM vm = (AdditionalThresholdsVM)this.DataContext;
            vm.Copy();
        }

        private void Remove_btn_Click(object sender, RoutedEventArgs e)
        {
            AdditionalThresholdsVM vm = (AdditionalThresholdsVM)this.DataContext;
            vm.Remove();
        }

        private void ok_btn_Click(object sender, RoutedEventArgs e)
        {
            AdditionalThresholdsVM vm = (AdditionalThresholdsVM)this.DataContext;
            vm.OkClicked();
        }
    }
}
