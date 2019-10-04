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

namespace Fda.Watershed
{
    /// <summary>
    /// Interaction logic for TerrainBrowser.xaml
    /// </summary>
    public partial class TerrainBrowser : UserControl
    {
        public TerrainBrowser()
        {
            InitializeComponent();
        }

        private void cmb_Path_CmbSelectionMade(string path)
        {
            FdaViewModel.Watershed.TerrainBrowserVM vm = (FdaViewModel.Watershed.TerrainBrowserVM)this.DataContext;
            vm.OriginalPath = cmb_Path.SelectedPath;
        }
    }
}
