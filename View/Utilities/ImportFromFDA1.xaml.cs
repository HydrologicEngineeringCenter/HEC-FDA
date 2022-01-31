using System.Windows;
using System.Windows.Controls;
using ViewModel.Utilities;

namespace View.Utilities
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

        private void TextBoxFileBrowser_SelectionMade(string fullpath, string filename)
        {
            if(DataContext is ImportFromFDA1VM vm)
            {
                vm.Path = fullpath;
                ImportBtn.IsEnabled = true;
            }          
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
            var window = Window.GetWindow(this);
            window.Close();
        }
    }
}
