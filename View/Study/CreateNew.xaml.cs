using HEC.Plotting.SciChart2D.Charts;
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
