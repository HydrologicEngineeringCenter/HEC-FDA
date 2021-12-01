using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Inventory
{
    public class StructureMissingDataRowItem
    {

        public string ID { get; set; }
        public bool IsMissingOcctype { get; set; }
        public bool IsMissingFoundationHt { get; set; }
        public bool IsMissingGroundElevation { get; set; }
        public bool IsMissingStructureValue { get; set; }
        public bool IsMissingTerrainElevation { get; set; }


        public StructureMissingDataRowItem(string id)
        {
            ID = id;
        }

        public void AddMissingAttribute(string attribute)
        {
            if(!MissingAttributes.Contains(attribute))
            {
                MissingAttributes.Add(attribute);
            }
        }


    }
}
