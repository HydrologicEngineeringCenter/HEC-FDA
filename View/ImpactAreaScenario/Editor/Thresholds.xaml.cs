using System.Windows;
using System.Windows.Controls;
using ViewModel.ImpactAreaScenario.Editor;
using ViewModel.Utilities;

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
            if (DataContext is ThresholdsVM vm)
            {
                vm.AddRow();
            }
        }

        private void Copy_btn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ThresholdsVM vm)
            {
                vm.Copy();
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
                FdaValidationResult result = vm.IsValid();
                if (result.IsValid)
                {
                    var myWindow = Window.GetWindow(this);
                    myWindow.Close();
                }
                else
                {
                    MessageBox.Show(result.ErrorMessage.ToString(), "Duplicate Rows", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
    }
}
