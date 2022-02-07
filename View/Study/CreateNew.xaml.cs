using System.Windows.Controls;

namespace View.Study
{
    /// <summary>
    /// Interaction logic for CreateNew.xaml
    /// </summary>
    public partial class CreateNew : UserControl
    {
        public CreateNew()
        {
            InitializeComponent();
        }

        private void TextBoxFolderBrowser_SelectionMade(string fullpath)
        {
            ViewModel.Study.NewStudyVM vm = (ViewModel.Study.NewStudyVM)this.DataContext;
            vm.Path = fullpath;
        }

    }
}
