namespace structures
{
    public class DeterministicStructure
    {
        public int Fid { get; }
        public int ImpactAreaID { get; }
        public string St_damcat { get; }
        public double FoundHeightSample { get; }
        public double StructValueSample { get; }
        public double ContentValueSample { get; }
        public double VehicleValueSample { get; }
        public double OtherValueSample { get; }
        public string DamageCatagory { get; }
        public DeterministicOccupancyType OccupancyType { get; }

        public DeterministicStructure(int fid, int impactAreaID, string damageCatagory, DeterministicOccupancyType occupancyType, double foundHeightSample, double structValueSample, double contentValueSample, double vehicleValueSample, double otherValueSample)
        {
            Fid = fid;
            ImpactAreaID = impactAreaID;
            DamageCatagory = damageCatagory;
            OccupancyType = occupancyType;
            FoundHeightSample = foundHeightSample;
            StructValueSample = structValueSample;
            ContentValueSample = contentValueSample;
            VehicleValueSample = vehicleValueSample;
            OtherValueSample = otherValueSample;
        }

        public StructureDamageResult ComputeDamage(float depth)
        {
            double depthabovefoundHeight = depth - FoundHeightSample;

            //Structure
            double structDamagepercent = OccupancyType.StructDamagePairedData.f(depthabovefoundHeight);
            double structDamage = structDamagepercent * StructValueSample;

            //Content
            double contentDamagePercent = OccupancyType.ContentDamagePairedData.f(depthabovefoundHeight);
            double contDamage = contentDamagePercent * ContentValueSample;

            //Vehicle
            double vehicleDamagePercent = OccupancyType.VehicleDamagePairedData.f(depthabovefoundHeight);
            double vehicleDamage = vehicleDamagePercent * VehicleValueSample;

            //Other
            double otherDamagePercent = OccupancyType.OtherDamagePairedData.f(depthabovefoundHeight);
            double otherDamage = otherDamagePercent * OtherValueSample;

            return new StructureDamageResult(structDamage, contDamage, vehicleDamage, otherDamage);
        }
    }
}