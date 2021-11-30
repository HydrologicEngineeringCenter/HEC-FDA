using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Inventory
{
    public class StructureElevationRowItem
    {

        public string Name { get; set; }
        public double? Elevation { get; set; }

        public StructureElevationRowItem(string name)
        {
            Name = name;
        }


    }
}
