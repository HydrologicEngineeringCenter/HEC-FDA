using System.Windows;
using System.Windows.Controls;
using ViewModel.Study;
using ViewModel.Utilities;

namespace View.Study
{
    /// <summary>
    /// Interaction logic for ImportFromOldFda.xaml
    /// </summary>
    public partial class ImportFromOldFda : UserControl
    {
        public ImportFromOldFda()
        {
            InitializeComponent();        
        }

        private void TextBoxFolderBrowser_SelectionMade(string fullpath)
        {
            ImportFromOldFdaVM vm = (ImportFromOldFdaVM)this.DataContext;
            vm.FolderPath = fullpath;
        }

        private void TextBoxFileBrowser_SelectionMade(string fullpath, string filename)
        {
            ImportFromOldFdaVM vm = (ImportFromOldFdaVM)this.DataContext;
            vm.Path = fullpath;
            txtbox.Text = fullpath;
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
