using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    class OccupancyTypeGroupEditable : IOccupancyTypeGroupEditable
    {
        public List<IOccupancyTypeEditable> Occtypes { get; set; }

        public OccupancyTypeGroupEditable(List<IOccupancyTypeEditable> occtypes)
        {
            Occtypes = occtypes;
        }

    }
}
