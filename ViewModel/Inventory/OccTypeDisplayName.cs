using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Inventory.OccupancyTypes;

namespace ViewModel.Inventory
{
    public class OccTypeDisplayName
    {

        public string DisplayName { get; }
        public IOccupancyType OccType { get; }
        public OccTypeDisplayName(string groupName, IOccupancyType occType)
        {
            OccType = occType;
            DisplayName = groupName + " => " + occType.Name;
        }

    }
}
