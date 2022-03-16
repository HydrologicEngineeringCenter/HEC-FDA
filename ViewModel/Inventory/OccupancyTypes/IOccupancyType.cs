using paireddata;
using Statistics;
using HEC.FDA.ViewModel.Inventory.DamageCategory;
using HEC.FDA.ViewModel.TableWithPlot;

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


        //bool CalculateStructureDamage { get; set; }
        //bool CalculateContentDamage { get; set; }
        //bool CalculateVehicleDamage { get; set; }
        //bool CalculateOtherDamage { get; set; }

        //ComputeComponentVM StructureDepthDamageFunction { get; set; }
        //ComputeComponentVM ContentDepthDamageFunction { get; set; }
        //ComputeComponentVM VehicleDepthDamageFunction { get; set; }
        //ComputeComponentVM OtherDepthDamageFunction { get; set; }

        //ContinuousDistribution StructureValueUncertainty { get; set; }
        //ContinuousDistribution ContentValueUncertainty { get; set; }
        //ContinuousDistribution VehicleValueUncertainty { get; set; }
        //ContinuousDistribution OtherValueUncertainty { get; set; }
        ContinuousDistribution FoundationHeightUncertainty { get; set; }
        //These booleans determine if the content/vehicle/other curves are a ratio of structure value or not
        bool IsContentRatio { get; set; }
        bool IsVehicleRatio { get; set; }
        bool IsOtherRatio { get; set; }

        //ValueUncertaintyType StructureUncertaintyType { get; set; }
        //ValueUncertaintyType ContentUncertaintyType { get; set; }
        //ValueUncertaintyType VehicleUncertaintyType { get; set; }
        //ValueUncertaintyType OtherUncertaintyType { get; set; }
        //ValueUncertaintyType FoundationHtUncertaintyType { get; set; }

        double ContentToStructureValue { get; set; }
        double OtherToStructureValue { get; set; }

        ContinuousDistribution ContentToStructureValueUncertainty { get; set; }
        ContinuousDistribution OtherToStructureValueUncertainty { get; set; }



    }
}
