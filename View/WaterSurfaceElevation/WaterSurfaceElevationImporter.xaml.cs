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
            //clear out any already existing rows
            if (!System.IO.Directory.Exists(fullpath))
            {
                vm.ListOfRows.Clear();
                return;
            }
            //is this an old fda study?

            List<string> tifFiles = new List<string>();
            List<string> fltFiles = new List<string>();
            List<string> vrtFiles = new List<string>();

            string[] fileList = System.IO.Directory.GetFiles(fullpath);
            
            if(fileList.Count()==0)
            {
                return;
            }

            foreach(string file in fileList)
            {
                if(System.IO.Path.GetExtension(file) == ".tif") { tifFiles.Add(file); }
                if (System.IO.Path.GetExtension(file) == ".flt") { fltFiles.Add(file); }
                if (System.IO.Path.GetExtension(file) == ".vrt") { vrtFiles.Add(file); }

            }

            //clear out any already existing rows
            vm.ListOfRows.Clear();

            double prob = 0;
            foreach(string tifFile in tifFiles)
            {
                prob += .1;
                vm.AddRow(true, System.IO.Path.GetFileName(tifFile),System.IO.Path.GetFullPath(tifFile), prob);
            }
            prob = 0;
            foreach (string fltFile in fltFiles)
            {
                prob += .1;
                vm.AddRow(true, System.IO.Path.GetFileName(fltFile), System.IO.Path.GetFullPath(fltFile), prob);
            }
            prob = 0;
            foreach (string vrtFile in vrtFiles)
            {
                prob += .1;
                vm.AddRow(true, System.IO.Path.GetFileName(vrtFile), System.IO.Path.GetFullPath(vrtFile), prob);
            }

            //lst_ListOfRows.ItemsSource = vm.ListOfRows;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            HEC.FDA.ViewModel.WaterSurfaceElevation.WaterSurfaceElevationImporterVM vm = (WaterSurfaceElevationImporterVM)this.DataContext;
            if(vm.IsEditor == true)
            {
                //get rid of the folder selection because we no longer know where the user pulled the data from
                main_grid.RowDefinitions[3].Height = new GridLength(0);
            }
        }

    }
}
