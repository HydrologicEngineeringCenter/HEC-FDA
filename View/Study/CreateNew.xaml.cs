using System.Windows.Controls;

namespace HEC.FDA.View.Study
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
            HEC.FDA.ViewModel.Study.NewStudyVM vm = (HEC.FDA.ViewModel.Study.NewStudyVM)this.DataContext;
            vm.Path = fullpath;
        }

    }
}
