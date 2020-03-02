using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    interface IOccupancyTypeGroupEditable
    {
        List<IOccupancyTypeEditable> Occtypes { get; set; }
    }
}
