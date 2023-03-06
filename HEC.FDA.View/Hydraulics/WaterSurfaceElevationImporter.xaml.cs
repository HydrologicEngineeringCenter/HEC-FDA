using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Hydraulics
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is GriddedImporterVM vm)
            {
                vm.HasChanges = false;
            }
        }
    }
}
