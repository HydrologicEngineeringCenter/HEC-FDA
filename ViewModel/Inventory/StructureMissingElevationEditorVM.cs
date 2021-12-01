using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Editors;

namespace ViewModel.Inventory
{
    public class StructureMissingElevationEditorVM : BaseEditorVM
    {

        public List<StructureMissingDataRowItem> Rows { get; } = new List<StructureMissingDataRowItem>();


        public StructureMissingElevationEditorVM(List<StructureMissingDataRowItem> rows):base(null)
        {
            Rows = rows;         
        }

        public override void AddValidationRules()
        {
            //this is here so that we don't look for a name property.
        }

        public override void Save()
        {
            //validate that there is a value for each row.

            int i = 0;
        }
    }
}
