using HEC.FDA.ViewModel.Inventory.OccupancyTypes;

namespace HEC.FDA.ViewModel.Inventory
{
    public class OccTypeDisplayName
    {
        public string DisplayName { get; }
        public OccupancyType OccType { get; }
        public int GroupID { get; }
        public OccTypeDisplayName(string groupName,int groupID, OccupancyType occType)
        {
            GroupID = groupID;
            OccType = occType;
            DisplayName = groupName + " | " + occType.Name;
        }
    }
}
