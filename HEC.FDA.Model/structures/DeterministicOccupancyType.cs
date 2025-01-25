using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.paireddata.Interfaces;
using Statistics;

namespace HEC.FDA.Model.structures
{
    public class DeterministicOccupancyType
    {
        #region Properties 
        public bool ComputeContentDamage { get; }
        public bool ComputeVehicleDamage { get; }
        public bool ComputeOtherDamage { get; }
        public bool UseCSVR { get; }
        public bool UseOSVR { get; }
        public string OccupancyTypeName { get; }
        public string OccupancyTypeDamageCategory { get; }
        public IPairedData StructPercentDamagePairedData { get; }
        public IPairedData ContentPercentDamagePairedData { get; }
        public IPairedData VehiclePercentDamagePairedData { get; }
        public IPairedData OtherPercentDamagePairedData { get; }
        public double FirstFloorElevationOffset { get; }
        public double StructureValueOffset { get; }
        public double ContentValueOffset { get; }
        public double VehicleValueOffset { get; }
        public double OtherValueOffset { get; }
        public double ContentToStructureValueRatio { get; }
        public double OtherToStructureValueRatio { get; }
        public bool IsStructureValueLogNormal { get; }
        public bool IsContentValueLogNormal { get; }
        public bool IsOtherValueLogNormal { get; }
        public bool IsVehicleValueLogNormal { get; }
        public bool IsFirstFloorElevationLogNormal { get; }
        public bool IsNull { get; }
        #endregion

        #region Constructor 
        public DeterministicOccupancyType(string occupancyTypeName, string occupancyTypeDamageCategory, IPairedData structPercentDamagePairedData, double sampledFirstFloorElevation, double sampledStructureValue, bool computeContentDamage, bool computeVehicleDamage, bool computeOtherDamage, IPairedData contentPercentDamagePairedData, double sampledContentValue, bool useCSVR, double csvr, IPairedData vehiclePercentDamagePairedData, double sampledVehicleValue, IPairedData otherPercentDamagePairedData, double sampledOtherValue, bool useOSVR, double osvr, bool isStructureValueLogNormal = false, bool isContentValueLogNormal = false, bool isOtherValueLogNormal = false, bool isVehicleValueLogNormal = false, bool isFirstFloorElevationLogNormal = false)
        {
            OccupancyTypeName = occupancyTypeName;
            OccupancyTypeDamageCategory = occupancyTypeDamageCategory;
            StructPercentDamagePairedData = structPercentDamagePairedData;
            ContentPercentDamagePairedData = contentPercentDamagePairedData;
            VehiclePercentDamagePairedData = vehiclePercentDamagePairedData;
            OtherPercentDamagePairedData = otherPercentDamagePairedData;
            FirstFloorElevationOffset = sampledFirstFloorElevation;
            StructureValueOffset = sampledStructureValue;
            ContentValueOffset = sampledContentValue;
            VehicleValueOffset = sampledVehicleValue;
            OtherValueOffset = sampledOtherValue;
            ComputeContentDamage = computeContentDamage;
            ComputeVehicleDamage = computeVehicleDamage;
            ComputeOtherDamage = computeOtherDamage;
            UseCSVR = useCSVR;
            UseOSVR = useOSVR;
            ContentToStructureValueRatio = csvr;
            OtherToStructureValueRatio = osvr;
            IsStructureValueLogNormal = isStructureValueLogNormal;
            IsContentValueLogNormal = isContentValueLogNormal;
            IsOtherValueLogNormal = isOtherValueLogNormal;
            IsVehicleValueLogNormal = isVehicleValueLogNormal;
            IsFirstFloorElevationLogNormal = isFirstFloorElevationLogNormal;
            IsNull = false;

        }

        public DeterministicOccupancyType()
        {
            OccupancyTypeName = "This Object Is Null";
            OccupancyTypeDamageCategory = "This Object Is Null";
            StructPercentDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 }); 
            ContentPercentDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 }); 
            VehiclePercentDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 }); 
            OtherPercentDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 }); 
            FirstFloorElevationOffset = 0;
            StructureValueOffset = 0;
            ContentValueOffset = 0;
            VehicleValueOffset = 0;
            OtherValueOffset = 0;
            ComputeContentDamage = false;
            ComputeVehicleDamage = false;
            ComputeOtherDamage = false;
            UseCSVR = false;
            UseOSVR = false;
            ContentToStructureValueRatio = 0;
            OtherToStructureValueRatio = 0;
            IsStructureValueLogNormal = false;
            IsContentValueLogNormal = false;
            IsOtherValueLogNormal = false;
            IsVehicleValueLogNormal = false;
            IsFirstFloorElevationLogNormal = false;
            IsNull = true;
        }
        #endregion
    }
}

