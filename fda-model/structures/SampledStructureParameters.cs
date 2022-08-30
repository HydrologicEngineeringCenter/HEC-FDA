using paireddata;

namespace structures
{
    public class SampledStructureParameters
    {
        public string Name { get; }
        public string DamageCatagory { get; }
        public IPairedData StructDamagePairedData { get; }
        public IPairedData ContentDamagePairedData { get; }
        public IPairedData VehicleDamagePairedData { get; }
        public IPairedData OtherDamagePairedData { get; }
        public double FoundationHeightSampled { get; }
        public double StructureValueSampled { get; }
        //this will be set either using the content value or the content to structure value ratio
        public double ContentValueSampled { get; }
        public double VehicleValueSampled { get; }
        //this will be set either using the other value or the other to structure value ratio 
        public double OtherValueSampled { get; }


        public SampledStructureParameters(string name, string damcat, IPairedData structPercentDamagePairedData, IPairedData contentPercentDamagePairedData, IPairedData vehiclePercentDamagePairedData, IPairedData otherPercentDamagePairedData, double sampledFirstFloorElevation, double sampledStructureValue, double sampledContentValue, double sampledVehicleValue, double sampledOtherValue)
        {
            //TODO: Sampling the depth percent damage functions for each structure individually seems a bit overkill 
            Name = name;
            DamageCatagory = damcat;
            StructDamagePairedData = structPercentDamagePairedData;
            ContentDamagePairedData = contentPercentDamagePairedData;
            VehicleDamagePairedData = vehiclePercentDamagePairedData;
            OtherDamagePairedData = otherPercentDamagePairedData;
            FoundationHeightSampled = sampledFirstFloorElevation;
            StructureValueSampled = sampledStructureValue;
            ContentValueSampled = sampledContentValue;
            VehicleValueSampled = sampledVehicleValue;
            OtherValueSampled = sampledOtherValue;

        }
    }
}

