using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Inventory
{
    public class StructureMissingElevationEditorVM : BaseViewModel
    {

        public List<StructureElevationRowItem> Rows { get; } = new List<StructureElevationRowItem>();


        public StructureMissingElevationEditorVM(List<string> structNames)
        {

            foreach(string name in structNames)
            {
                Rows.Add(new StructureElevationRowItem(name));
            }

        }

    }
}
