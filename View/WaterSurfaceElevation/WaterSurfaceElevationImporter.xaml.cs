using HEC.FDA.ViewModel.WaterSurfaceElevation;
using System.Collections.Generic;
using System.Linq;
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
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            WaterSurfaceElevationImporterVM vm = (WaterSurfaceElevationImporterVM)this.DataContext;
            if(vm.IsEditor == true)
            {
                //get rid of the folder selection because we no longer know where the user pulled the data from
                main_grid.RowDefinitions[3].Height = new GridLength(0);
            }
        }

    }
}
