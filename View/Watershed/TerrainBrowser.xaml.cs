using HEC.FDA.ViewModel.Watershed;
using System.Windows.Controls;

namespace HEC.FDA.View.Watershed
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
            HEC.FDA.ViewModel.Watershed.TerrainBrowserVM vm = (TerrainBrowserVM)this.DataContext;
            vm.OriginalPath = cmb_Path.SelectedPath;
        }
    }
}
