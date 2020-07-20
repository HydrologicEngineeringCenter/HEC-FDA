using FdaViewModel.WaterSurfaceElevation;
using Importer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace View.WaterSurfaceElevation
{
    /// <summary>
    /// Interaction logic for WaterSurfaceElevationImporterFromFDA1.xaml
    /// </summary>
    public partial class WaterSurfaceElevationImporterFromFDA1 : UserControl
    {
        public WaterSurfaceElevationImporterFromFDA1()
        {
            InitializeComponent();
        }
        private void TextBoxFileBrowser_SelectionMade(string fullpath, string filename)
        {
            WaterSurfaceElevationImporterFDA1VM vm = (WaterSurfaceElevationImporterFDA1VM)this.DataContext;
            //clear the rows. This user might be selecting a new file.
            vm.WaterSurfaceElevations = new ObservableCollection<WSERowItemVM>();
            //lst_wse.Items.Clear();
            //so we have the file, start the import and display info to user.
            AsciiImport import = new AsciiImport();
            //the importer will read the file and load the occtype property with any occtypes it found
            import.ImportAsciiData(fullpath, AsciiImport.ImportOptions.ImportWaterSurfaceProfilesOnly);
            List<WaterSurfaceElevationElement> elems = import.WaterSurfaceElevs;

            if (elems.Count > 1)
            {
                vm.AddWSEElements(elems);
            }
            else
            {
                MessageBox.Show("No water surface profiles were found in the selected file.", "No Profiles Found", MessageBoxButton.OK, MessageBoxImage.Information);
            }



        }

    }
}
