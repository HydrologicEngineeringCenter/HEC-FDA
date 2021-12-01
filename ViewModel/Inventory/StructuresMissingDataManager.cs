using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Inventory
{
    public class StructuresMissingDataManager
    {

        private Dictionary<string, StructureMissingDataRowItem> _MissingDataRows = new Dictionary<string, StructureMissingDataRowItem>();

        public StructuresMissingDataManager()
        {

        }

        public void AddStructureWithMissingData(string id, string missingAttribute)
        {
            if(_MissingDataRows.ContainsKey(id))
            {
                _MissingDataRows[id].AddMissingAttribute(missingAttribute);
            }
            else
            {
                _MissingDataRows.Add(id, new StructureMissingDataRowItem(id, missingAttribute));
            }
        }

        //public List<StructureMissingDataRowItem> GetRows()
        //{

        //}

    }
}
