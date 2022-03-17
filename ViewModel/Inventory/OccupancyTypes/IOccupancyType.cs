using Statistics;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public interface IOccupancyType
    {
        int GroupID { get; set; }
        int ID { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string DamageCategory { get; set; }

        OccTypeItem StructureItem { get; set; }
        OccTypeItemWithRatio ContentItem { get; set; }
        OccTypeItem VehicleItem { get; set; }
        OccTypeItemWithRatio OtherItem { get; set; }
        ContinuousDistribution FoundationHeightUncertainty { get; set; }
        
    }
}
