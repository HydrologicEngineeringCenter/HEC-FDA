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
        OccTypeItem ContentItem { get; set; }
        OccTypeItem VehicleItem { get; set; }
        OccTypeItem OtherItem { get; set; }
        ContinuousDistribution FoundationHeightUncertainty { get; set; }

        //These booleans determine if the content/vehicle/other curves are a ratio of structure value or not
        bool IsContentRatio { get; set; }
        bool IsVehicleRatio { get; set; }
        bool IsOtherRatio { get; set; }

        double ContentToStructureValue { get; set; }
        double OtherToStructureValue { get; set; }

        ContinuousDistribution ContentToStructureValueUncertainty { get; set; }
        ContinuousDistribution OtherToStructureValueUncertainty { get; set; }
    }
}
