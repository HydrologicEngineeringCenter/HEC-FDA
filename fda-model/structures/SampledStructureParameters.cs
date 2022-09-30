using paireddata;

namespace structures
{
    public class SampledStructureParameters
    {
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

         
        public SampledStructureParameters(string occupancyTypeName, string occupancyTypeDamageCategory, IPairedData structPercentDamagePairedData, double sampledFirstFloorElevation, double sampledStructureValue, bool computeContentDamage, bool computeVehicleDamage, bool computeOtherDamage, IPairedData contentPercentDamagePairedData = null, double sampledContentValue = -999, IPairedData vehiclePercentDamagePairedData = null, double sampledVehicleValue = -999, IPairedData otherPercentDamagePairedData = null, double sampledOtherValue = -999)
        {
            //TODO: Sampling the depth percent damage functions for each structure individually seems a bit overkill 
            //I am not sure that I agree with it being overkill. 
            //There is uncertainty about percent damage that reflects the interaction between the water and the structure that is a function of the structure condition 
            //implying uncertainty about percent damage is not perfectly correlated across structures 
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

        }
    }
}

