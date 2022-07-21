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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is WaterSurfaceElevationImporterVM vm)
            {
                vm.HasChanges = false;
            }
        }
    }
}
