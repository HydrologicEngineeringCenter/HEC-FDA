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

        public List<StructureElevationRowItem> Rows { get; } = new List<StructureElevationRowItem>();


        public StructureMissingElevationEditorVM(List<string> structNames):base(null)
        {

            foreach(string name in structNames)
            {
                Rows.Add(new StructureElevationRowItem(name));
            }

            
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
