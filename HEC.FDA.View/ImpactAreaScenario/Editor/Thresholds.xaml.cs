using System.Windows;
using System.Windows.Controls;
using HEC.FDA.ViewModel.ImpactAreaScenario.Editor;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.View.ImpactAreaScenario.Editor
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
            if (DataContext is ThresholdsVM vm)
            {
                vm.AddRow();
            }
        }

        private void Remove_btn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ThresholdsVM vm)
            {
                vm.Remove();
            }
        }

        private void ok_btn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ThresholdsVM vm)
            {
                vm.OkClicked();

                if (vm.IsThresholdsValid)
                {
                    var myWindow = Window.GetWindow(this);
                    myWindow.Close();
                }
            }
        }
    }
}
