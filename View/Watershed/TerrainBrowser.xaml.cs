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

        private void cmb_Path_CmbSelectionMade(string fullpath, string filename)
        {
            if (DataContext is TerrainBrowserVM vm)
            {
                vm.FileSelected(fullpath);
            }
        }
    }
}
