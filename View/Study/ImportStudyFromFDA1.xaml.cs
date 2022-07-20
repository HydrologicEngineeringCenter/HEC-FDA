using System.Windows;
using System.Windows.Controls;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;

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

        //private void TextBoxFolderBrowser_SelectionMade(string fullpath)
        //{
        //    if(DataContext is ImportStudyFromFDA1VM vm)
        //    {
        //        vm.FolderPath = fullpath;
        //    }
        //}

        private void TextBoxFileBrowser_SelectionMade(string fullpath, string filename)
        {
            if (DataContext is ImportStudyFromFDA1VM vm)
            {
                vm.Path = fullpath;
                txtbox.Text = fullpath;
            }
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
