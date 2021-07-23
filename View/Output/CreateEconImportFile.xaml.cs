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

namespace Fda.Output
{
    /// <summary>
    /// Interaction logic for CreateEconImportFile.xaml
    /// </summary>
    public partial class CreateEconImportFile : UserControl
    {
        public CreateEconImportFile()
        {
            InitializeComponent();
        }
        public CreateEconImportFile(ViewModel.Output.CreateEconImportFileVM vm)
        {
            InitializeComponent();
            Resources["vm"] = vm;
        }

    }
}
