using HEC.FDA.Model.paireddata;

namespace HEC.FDA.Model.structures
{
    public class SampledStructureParameters
    {
        #region Properties 
        public bool ComputeContentDamage { get; }
        public bool ComputeVehicleDamage { get; }
        public bool ComputeOtherDamage { get; }
        public string OccupancyTypeName { get; }
        public string OccupancyTypeDamageCategory { get; }
        public IPairedData StructPercentDamagePairedData { get; }
        public IPairedData ContentPercentDamagePairedData { get; }
        public IPairedData VehiclePercentDamagePairedData { get; }
        public IPairedData OtherPercentDamagePairedData { get; }
        public double FirstFloorElevationSampled { get; }
        public double StructureValueSampled { get; }
        //this will be set either using the content value or the content to structure value ratio
        public double ContentValueSampled { get; }
        public double VehicleValueSampled { get; }
        //this will be set either using the other value or the other to structure value ratio 
        public double OtherValueSampled { get; }
        public bool IsNull { get; }
        #endregion

        #region Constructor 
        public SampledStructureParameters(string occupancyTypeName, string occupancyTypeDamageCategory, IPairedData structPercentDamagePairedData, double sampledFirstFloorElevation, double sampledStructureValue, bool computeContentDamage, bool computeVehicleDamage, bool computeOtherDamage, IPairedData contentPercentDamagePairedData = null, double sampledContentValue = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, IPairedData vehiclePercentDamagePairedData = null, double sampledVehicleValue = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, IPairedData otherPercentDamagePairedData = null, double sampledOtherValue = utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
        {
            OccupancyTypeName = occupancyTypeName;
            OccupancyTypeDamageCategory = occupancyTypeDamageCategory;
            StructPercentDamagePairedData = structPercentDamagePairedData;
            ContentPercentDamagePairedData = contentPercentDamagePairedData;
            VehiclePercentDamagePairedData = vehiclePercentDamagePairedData;
            OtherPercentDamagePairedData = otherPercentDamagePairedData;
            FirstFloorElevationSampled = sampledFirstFloorElevation;
            StructureValueSampled = sampledStructureValue;
            ContentValueSampled = sampledContentValue;
            VehicleValueSampled = sampledVehicleValue;
            OtherValueSampled = sampledOtherValue;
            ComputeContentDamage = computeContentDamage;
            ComputeVehicleDamage = computeVehicleDamage;
            ComputeOtherDamage = computeOtherDamage;
            IsNull = false;

        }

        public SampledStructureParameters()
        {
            //TODO: by not handling the properties in this constructor we are simply passing the null reference exception downstream 
            //Everything has a default or empty constructor except Paired Data
            //Another case for a Paired Data empty constructor 
            IsNull = true;
        }
        #endregion
    }
}

