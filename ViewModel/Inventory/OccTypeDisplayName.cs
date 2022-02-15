using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;

namespace HEC.FDA.ViewModel.Inventory
{
    public class OccTypeDisplayName
    {
        public string DisplayName { get; }
        public IOccupancyType OccType { get; }
        public String GroupName { get; }
        public OccTypeDisplayName(string groupName, IOccupancyType occType)
        {
            GroupName = groupName;
            OccType = occType;
            DisplayName = groupName + " | " + occType.Name;
        }

    }
}
