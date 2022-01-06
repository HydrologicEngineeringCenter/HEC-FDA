using paireddata;
using Statistics;
using HEC.FDA.ViewModel.Inventory.DamageCategory;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public interface IOccupancyType
    {
        bool IsModified { get; set; }
        int GroupID { get; set; }
        int ID { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string DamageCategory { get; set; }

        bool CalculateStructureDamage { get; set; }
        bool CalculateContentDamage { get; set; }
        bool CalculateVehicleDamage { get; set; }
        bool CalculateOtherDamage { get; set; }

        UncertainPairedData StructureDepthDamageFunction { get; set; }

        UncertainPairedData ContentDepthDamageFunction { get; set; }
        UncertainPairedData VehicleDepthDamageFunction { get; set; }
        UncertainPairedData OtherDepthDamageFunction { get; set; }

        IDistribution StructureValueUncertainty { get; set; }
        IDistribution ContentValueUncertainty { get; set; }
        IDistribution VehicleValueUncertainty { get; set; }
        IDistribution OtherValueUncertainty { get; set; }
        IDistribution FoundationHeightUncertainty { get; set; }
        
        //These booleans determine if the content/vehicle/other curves are a ratio of structure value or not
        bool IsContentRatio { get; set; }
        bool IsVehicleRatio { get; set; }
        bool IsOtherRatio { get; set; }

        ValueUncertaintyType StructureUncertaintyType { get; set; }
        ValueUncertaintyType ContentUncertaintyType { get; set; }
        ValueUncertaintyType VehicleUncertaintyType { get; set; }
        ValueUncertaintyType OtherUncertaintyType { get; set; }
        ValueUncertaintyType FoundationHtUncertaintyType { get; set; }

    }
}
