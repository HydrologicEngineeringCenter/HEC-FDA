using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace View.Study
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
            ViewModel.Study.ExistingStudyVM vm = (ViewModel.Study.ExistingStudyVM)this.DataContext;
            vm.Path = fullpath;
            txtbox.Text = fullpath;
        }

        //private void TextBoxFileBrowser_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    ViewModel.Study.ExistingStudyVM vm = (ViewModel.Study.ExistingStudyVM)this.DataContext;
        //    vm.Path = ((Consequences_Assist.Controls.TextBoxFileBrowser)sender).Path;
        //    txtbox.Text = vm.Path;
        //}

        //private void TextBoxFolderBrowser_SelectionMade(string fullpath)
        //{

        //}
    }
}
