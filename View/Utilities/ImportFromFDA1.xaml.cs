using System.Windows;
using System.Windows.Controls;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.View.Utilities
{
    /// <summary>
    /// Interaction logic for ImportFromFDA1.xaml
    /// </summary>
    public partial class ImportFromFDA1 : UserControl
    {
        public ImportFromFDA1()
        {
            InitializeComponent();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ImportFromFDA1VM vm)
            {
                vm.Import();
                ImportBtn.IsEnabled = false;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Close();
        }
    }
}
