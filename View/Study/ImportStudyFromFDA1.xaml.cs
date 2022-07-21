using HEC.FDA.ViewModel.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Study
{
    /// <summary>
    /// Interaction logic for ImportFromOldFda.xaml
    /// </summary>
    public partial class ImportStudyFromFDA1 : UserControl
    {
        public ImportStudyFromFDA1()
        {
            InitializeComponent();        
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ImportFromFDA1VM vm)
            {
                vm.Import();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Close();
        }

    }
}
