using metrics;

namespace structures
{
    public class DeterministicStructure
    {//TODO: How are we going to handle parameters that are not being used?
        #region Fields
        SampledStructureParameters _sampledStructureParameters;
        #endregion

        #region Properties 
        public int Fid { get; }
        public int ImpactAreaID { get; }
        public string StructureDamageCategory { get; }
        public double FirstFloorElevation { get; }
        public double StructValueSample { get; }
        public double ContentValueSample { get; }
        public double VehicleValueSample { get; }
        public double OtherValueSample { get; }
        public string DamageCatagory { get; }
        #endregion

        #region Constructor 
        public DeterministicStructure(int fid, int impactAreaID, string damageCatagory, SampledStructureParameters sampledStructureParameters)
        {
            Fid = fid;
            ImpactAreaID = impactAreaID;
            DamageCatagory = damageCatagory;
            FirstFloorElevation = sampledStructureParameters.FirstFloorElevationSampled;
            StructValueSample = sampledStructureParameters.StructureValueSampled;
            ContentValueSample = sampledStructureParameters.ContentValueSampled;
            VehicleValueSample = sampledStructureParameters.VehicleValueSampled;
            OtherValueSample = sampledStructureParameters.OtherValueSampled;
        }
        #endregion

        //TODO: We do not want to return a new structure damage result every time 
        #region Methods
        public ConsequenceResult ComputeDamage(float waterSurfaceElevation)
        {
            double depthabovefoundHeight = waterSurfaceElevation - FirstFloorElevation;

            //Structure
            double structDamagepercent = _sampledStructureParameters.StructDamagePairedData.f(depthabovefoundHeight);
            double structDamage = structDamagepercent * StructValueSample;

            //Content
            double contentDamagePercent = _sampledStructureParameters.ContentDamagePairedData.f(depthabovefoundHeight);
            double contDamage = contentDamagePercent * ContentValueSample;

            //Vehicle
            double vehicleDamagePercent = _sampledStructureParameters.VehicleDamagePairedData.f(depthabovefoundHeight);
            double vehicleDamage = vehicleDamagePercent * VehicleValueSample;

            //Other
            double otherDamagePercent = _sampledStructureParameters.OtherDamagePairedData.f(depthabovefoundHeight);
            double otherDamage = otherDamagePercent * OtherValueSample;


            ConsequenceResult consequenceResult = new ConsequenceResult(DamageCatagory, ImpactAreaID);
            consequenceResult.IncrementConsequence(structDamage, contDamage, vehicleDamage, otherDamage);
            return consequenceResult;
        }
        #endregion
    }
}