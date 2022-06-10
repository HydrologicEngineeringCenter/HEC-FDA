using paireddata;

namespace structures
{
    public class DeterministicOccupancyType
    {
        public string Name { get; }
        public string DamageCatagory { get; }
        public IPairedData StructDamagePairedData { get; }
        public IPairedData ContentDamagePairedData { get; }
        public IPairedData VehicleDamagePairedData { get; }
        public IPairedData OtherDamagePairedData { get; }
        public double FoundationHeightError { get; }
        public double StructureValueError { get; }
        public double ContentValueError { get; }
        public double VehicleValueError { get; }
        public double OtherValueError { get; }
        public double ContentToStructureValueRatio { get; }
        public double OtherToStructureValueRatio { get; }


        public DeterministicOccupancyType(string name, string damcat, IPairedData structDamagePairedData, IPairedData contentDamagePairedData, IPairedData vehicleDamagePairedData, IPairedData otherDamagePairedData, double foundationHeightError, double structureValueError, double contentValueError, double vehicleValueError, double otherValueError, double contentToStructureValueRatio, double otherToStructureValueRatio)
        {
            Name = name;
            DamageCatagory = damcat;
            StructDamagePairedData = structDamagePairedData;
            ContentDamagePairedData = contentDamagePairedData;
            VehicleDamagePairedData = vehicleDamagePairedData;
            OtherDamagePairedData = otherDamagePairedData;
            FoundationHeightError = foundationHeightError;
            StructureValueError = structureValueError;
            ContentValueError = contentValueError;
            VehicleValueError = vehicleValueError;
            OtherValueError = otherValueError;
            ContentToStructureValueRatio = contentToStructureValueRatio;
            OtherToStructureValueRatio = otherToStructureValueRatio;

        }
    }
}