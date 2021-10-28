using System.Windows;
using System.Windows.Controls;
using ViewModel.ImpactAreaScenario.Editor;

namespace View.ImpactAreaScenario.Editor
{
    /// <summary>
    /// Interaction logic for AdditionalThresholds.xaml
    /// </summary>
    public partial class Thresholds : UserControl
    {
        public Thresholds()
        {
            InitializeComponent();
        }

        private void Add_btn_Click(object sender, RoutedEventArgs e)
        {
            ThresholdsVM vm = (ThresholdsVM)this.DataContext;
            vm.AddRow();
        }

        private void Copy_btn_Click(object sender, RoutedEventArgs e)
        {
            ThresholdsVM vm = (ThresholdsVM)this.DataContext;
            vm.Copy();
        }

        private void Remove_btn_Click(object sender, RoutedEventArgs e)
        {
            ThresholdsVM vm = (ThresholdsVM)this.DataContext;
            vm.Remove();
        }

        private void ok_btn_Click(object sender, RoutedEventArgs e)
        {
            var myWindow = Window.GetWindow(this);
            myWindow.Close();
        }
    }
}
