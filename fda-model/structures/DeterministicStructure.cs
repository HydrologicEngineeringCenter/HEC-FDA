using HEC.FDA.Model.metrics;
using System;
using HEC.FDA.Model.structures;

namespace HEC.FDA.Model.structures
{
    public class DeterministicStructure
    {
        #region Fields
        SampledStructureParameters _sampledStructureParameters;
        double _DepthAboveFoundationHeight;
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
        public double BeginningDamageDepth { get; }
        public SampledStructureParameters SampledStructureParameters { get { return _sampledStructureParameters; } }
        #endregion

        #region Constructor 
        public DeterministicStructure(int fid, int impactAreaID, SampledStructureParameters sampledStructureParameters, double beginningDamageDepth)
        {
            Fid = fid;
            ImpactAreaID = impactAreaID;
            BeginningDamageDepth = beginningDamageDepth;
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
            ConsequenceResult consequenceResult = new ConsequenceResult(DamageCatagory, ImpactAreaID);

            double depthabovefoundHeight = waterSurfaceElevation - FirstFloorElevation;

            if (BeginningDamageDepth < depthabovefoundHeight)
            {

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

                consequenceResult.IncrementConsequence(structDamage, contDamage, vehicleDamage, otherDamage);
            }
            return consequenceResult;
        }

        internal string ComputeStageAndDamageDetails(float v)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}