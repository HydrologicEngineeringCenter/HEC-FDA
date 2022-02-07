using paireddata;
using Statistics;

namespace OccupancyTypes
{
    public interface IOccupancyType
    {
        string Name { get; set; }
        string Description { get; set; }
        IDamageCategory DamageCategory { get; set; }

        bool CalculateStructureDamage { get; set; }
        bool CalcualateContentDamage { get; set; }
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

        string StructureDepthDamageName { get; set; }
        string ContentDepthDamageName { get; set; }
        string VehicleDepthDamageName { get; set; }
        string OtherDepthDamageName { get; set; }

        IOccupancyType Clone();

    }
}
