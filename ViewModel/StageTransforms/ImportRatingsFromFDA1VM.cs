using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Editors;
using ViewModel.Utilities;

namespace ViewModel.StageTransforms
{
    public class ImportRatingsFromFDA1VM : ImportFromFDA1VM
    {

        public ImportRatingsFromFDA1VM(EditorActionManager manager):base(manager)
        {

        }
        public override void SaveElements()
        {
            throw new NotImplementedException();
        }

        public override FdaValidationResult Validate()
        {
            throw new NotFiniteNumberException();
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
