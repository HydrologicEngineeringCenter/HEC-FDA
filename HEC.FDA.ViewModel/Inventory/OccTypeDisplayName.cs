using HEC.FDA.ViewModel.Inventory.OccupancyTypes;

namespace HEC.FDA.ViewModel.Inventory
{
    public class OccTypeDisplayName
    {
        public string DisplayName { get; }
        public IOccupancyType OccType { get; }
        public int GroupID { get; }
        public OccTypeDisplayName(string groupName,int groupID, IOccupancyType occType)
        {
            GroupID = groupID;
            OccType = occType;
            DisplayName = groupName + " | " + occType.Name;
        }
    }
}
