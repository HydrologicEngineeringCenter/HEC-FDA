using paireddata;

namespace structures
{
    public class SampledStructureParameters
    {
        public string Name { get; }
        public string DamageCatagory { get; }
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


        public SampledStructureParameters(string name, string damcat, IPairedData structPercentDamagePairedData, IPairedData contentPercentDamagePairedData, IPairedData vehiclePercentDamagePairedData, IPairedData otherPercentDamagePairedData, double sampledFirstFloorElevation, double sampledStructureValue, bool computeContentDamage, double sampledContentValue, bool computeVehicleDamage, double sampledVehicleValue, bool computeOtherDamage, double sampledOtherValue)
        {
            //TODO: Sampling the depth percent damage functions for each structure individually seems a bit overkill 
            Name = name;
            DamageCatagory = damcat;
            StructPercentDamagePairedData = structPercentDamagePairedData;
            ContentPercentDamagePairedData = contentPercentDamagePairedData;
            VehiclePercentDamagePairedData = vehiclePercentDamagePairedData;
            OtherPercentDamagePairedData = otherPercentDamagePairedData;
            FirstFloorElevationSampled = sampledFirstFloorElevation;
            StructureValueSampled = sampledStructureValue;
            ContentValueSampled = sampledContentValue;
            VehicleValueSampled = sampledVehicleValue;
            OtherValueSampled = sampledOtherValue;

        }
    }
}

