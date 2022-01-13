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
using ViewModel.Utilities;

namespace View.Utilities
{
    /// <summary>
    /// Interaction logic for ImportFromFDA1.xaml
    /// </summary>
    public partial class ImportFromFDA1 : UserControl
    {
        public ImportFromFDA1()
        {
            InitializeComponent();
        }

        private void TextBoxFileBrowser_SelectionMade(string fullpath, string filename)
        {
            if(DataContext is ImportFromFDA1VM vm)
            {
                vm.Path = fullpath;
            }
            //grd_inventory.Visibility = Visibility.Visible;
            //ImportStructuresFromFDA1VM vm = (ImportStructuresFromFDA1VM)this.DataContext;
            ////clear the rows. This user might be selecting a new file.
            //vm.Inventory = null;
            ////so we have the file, start the import and display info to user.
            //AsciiImport import = new AsciiImport();
            ////the importer will read the file and load the occtype property with any occtypes it found
            //import.ImportAsciiData(fullpath, AsciiImport.ImportOptions.ImportStructuresOnly);

            //DataTable structureTable = import.StructuresForFDA2;

            //if (structureTable.Rows.Count > 0)
            //{
            //    vm.SetInventory(structureTable, filename);
            //}
            //else
            //{
            //    grd_inventory.Visibility = Visibility.Hidden;
            //    MessageBox.Show("No structures were found in the selected file.", "No structures Found", MessageBoxButton.OK, MessageBoxImage.Information);
            //}



        }
    }
}
