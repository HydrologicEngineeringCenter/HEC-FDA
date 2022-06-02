using HEC.FDA.ViewModel.WaterSurfaceElevation;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.WaterSurfaceElevation
{
    /// <summary>
    /// Interaction logic for WaterSurfaceElevationImporter.xaml
    /// </summary>
    public partial class WaterSurfaceElevationImporter : UserControl
    {

        public WaterSurfaceElevationImporter()
        {
            InitializeComponent();
        }

        private void TxtDirectory_SelectionMade(string fullpath)
        {
            WaterSurfaceElevationImporterVM vm = (WaterSurfaceElevationImporterVM)this.DataContext;
            vm.FileSelected(fullpath);          
            vm.SelectedPath = fullpath;
        }
    }
}
