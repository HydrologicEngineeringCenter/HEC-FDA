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


        //TODO: the deterministic occupancy type must be unique to a structure according to the FDA algorithm 
        public SampledStructureParameters(string name, string damcat, IPairedData structDamagePairedData, IPairedData contentDamagePairedData, IPairedData vehicleDamagePairedData, IPairedData otherDamagePairedData, double foundationHeightError, double structureValueError, double contentValueError, double vehicleValueError, double otherValueError, double contentToStructureValueRatio, double otherToStructureValueRatio)
        {
            Name = name;
            DamageCatagory = damcat;
            StructDamagePairedData = structDamagePairedData;
            ContentDamagePairedData = contentDamagePairedData;
            VehicleDamagePairedData = vehicleDamagePairedData;
            OtherDamagePairedData = otherDamagePairedData;
            FoundationHeightSampled = foundationHeightError;
            StructureValueSampled = structureValueError;
            ContentValueSampled = contentValueError;
            VehicleValueSampled = vehicleValueError;
            OtherValueSampled = otherValueError;

        }
    }
}

