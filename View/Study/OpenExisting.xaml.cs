using HEC.FDA.ViewModel.Study;
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
            ExistingStudyVM vm = (ExistingStudyVM)this.DataContext;
            vm.Path = fullpath;
            txtbox.Text = fullpath;
        }
    }
}
