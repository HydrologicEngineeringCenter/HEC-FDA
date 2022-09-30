using HEC.FDA.Model.metrics;

namespace HEC.FDA.Model.structures
{
    public class DeterministicStructure
    {
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
        public SampledStructureParameters SampledStructureParameters { get { return _sampledStructureParameters; } }
        #endregion

        #region Constructor 
        public DeterministicStructure(int fid, int impactAreaID, SampledStructureParameters sampledStructureParameters)
        {
            Fid = fid;
            ImpactAreaID = impactAreaID;
            DamageCatagory = sampledStructureParameters.OccupancyTypeDamageCategory;
            FirstFloorElevation = sampledStructureParameters.FirstFloorElevationSampled;
            StructValueSample = sampledStructureParameters.StructureValueSampled;
            ContentValueSample = sampledStructureParameters.ContentValueSampled;
            VehicleValueSample = sampledStructureParameters.VehicleValueSampled;
            OtherValueSample = sampledStructureParameters.OtherValueSampled;
            _sampledStructureParameters = sampledStructureParameters;
        }
        #endregion

        //TODO: We do not want to return a new structure damage result every time 
        #region Methods
        public ConsequenceResult ComputeDamage(float waterSurfaceElevation)
        {
            double depthabovefoundHeight = waterSurfaceElevation - FirstFloorElevation;

            //Structure
            double structDamagepercent = _sampledStructureParameters.StructPercentDamagePairedData.f(depthabovefoundHeight);
            double structDamage = structDamagepercent * StructValueSample;

            //Content
            double contDamage = 0;
            if (_sampledStructureParameters.ComputeContentDamage)
            {
                double contentDamagePercent = _sampledStructureParameters.ContentPercentDamagePairedData.f(depthabovefoundHeight);
                contDamage = contentDamagePercent * ContentValueSample;
            }

            //Vehicle
            double vehicleDamage = 0;
            if (_sampledStructureParameters.ComputeVehicleDamage)
            {
                double vehicleDamagePercent = _sampledStructureParameters.VehiclePercentDamagePairedData.f(depthabovefoundHeight);
                vehicleDamage = vehicleDamagePercent * VehicleValueSample;
            }

            //Other
            double otherDamage = 0;
            if (_sampledStructureParameters.ComputeOtherDamage)
            {
                double otherDamagePercent = _sampledStructureParameters.OtherPercentDamagePairedData.f(depthabovefoundHeight);
                otherDamage = otherDamagePercent * OtherValueSample;
            }

            ConsequenceResult consequenceResult = new ConsequenceResult(DamageCatagory, ImpactAreaID);
            consequenceResult.IncrementConsequence(structDamage, contDamage, vehicleDamage, otherDamage);
            return consequenceResult;
        }
        #endregion
    }
}