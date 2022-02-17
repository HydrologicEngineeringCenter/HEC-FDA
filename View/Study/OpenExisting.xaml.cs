using System.Windows.Controls;

namespace HEC.FDA.View.Study
{
    /// <summary>
    /// Interaction logic for CreateNew.xaml
    /// </summary>
    public partial class OpenExisting : UserControl
    {
        public OpenExisting()
        {
            InitializeComponent();
        }

        private void TextBoxFileBrowser_SelectionMade(string fullpath, string filename)
        {
            HEC.FDA.ViewModel.Study.ExistingStudyVM vm = (HEC.FDA.ViewModel.Study.ExistingStudyVM)this.DataContext;
            vm.Path = fullpath;
            txtbox.Text = fullpath;
        }

        //private void TextBoxFileBrowser_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    HEC.FDA.ViewModel.Study.ExistingStudyVM vm = (HEC.FDA.ViewModel.Study.ExistingStudyVM)this.DataContext;
        //    vm.Path = ((Consequences_Assist.Controls.TextBoxFileBrowser)sender).Path;
        //    txtbox.Text = vm.Path;
        //}

        //private void TextBoxFolderBrowser_SelectionMade(string fullpath)
        //{

        //}
    }
}
