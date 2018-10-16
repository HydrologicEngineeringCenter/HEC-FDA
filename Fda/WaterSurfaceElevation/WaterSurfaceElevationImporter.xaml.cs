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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Fda.WaterSurfaceElevation
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

            

            FdaViewModel.WaterSurfaceElevation.WaterSurfaceElevationImporterVM vm = (FdaViewModel.WaterSurfaceElevation.WaterSurfaceElevationImporterVM)this.DataContext;
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


            //lst_ListOfRows.Items.Clear();
            lst_ListOfRows.ItemsSource = vm.ListOfRows;

            

        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.WaterSurfaceElevation.WaterSurfaceElevationImporterVM vm = (FdaViewModel.WaterSurfaceElevation.WaterSurfaceElevationImporterVM)this.DataContext;
            if( vm.OKButtonClicked() == true) //this runs some validation and returns true if everything was good
            {
                var window = Window.GetWindow(this);
                vm.WasCanceled = false;
                window.Close();
            }
        }
    }
}
