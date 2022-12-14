using HEC.FDA.Model.metrics;
using System;
using HEC.FDA.Model.structures;

namespace HEC.FDA.Model.structures
{
    public class DeterministicStructure
    {
        #region Fields
        SampledStructureParameters _sampledStructureParameters;
        int _numberOfStructures;
        int _yearInService;
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
        public DeterministicStructure(int fid, int impactAreaID, SampledStructureParameters sampledStructureParameters, double beginningDamageDepth, int numberOfStructures = 1, int yearInService = -999)
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
            _numberOfStructures = numberOfStructures;
            _yearInService = yearInService;
        }
        #endregion

        //TODO: We do not want to return a new structure damage result every time 
        #region Methods
        public ConsequenceResult ComputeDamage(float waterSurfaceElevation, double priceIndex = 1, int analysisYear = 9999)
        {
            ConsequenceResult consequenceResult = new ConsequenceResult(DamageCatagory, ImpactAreaID);

            double depthabovefoundHeight = waterSurfaceElevation - FirstFloorElevation;
            double structDamage = 0;
            double contDamage = 0;
            double vehicleDamage = 0;
            double otherDamage = 0;
            if (analysisYear > _yearInService)
            {
                //Beginning damage depth is relative to the first floor elevation and so a beginning damage depth of -1 means that damage begins 1 foot below the first floor elevation

                if (BeginningDamageDepth <= depthabovefoundHeight)
                {
                    //Structure
                    double structDamagepercent = _sampledStructureParameters.StructPercentDamagePairedData.f(depthabovefoundHeight);
                    if (structDamagepercent > 100)
                    {
                        structDamagepercent = 100;
                    }
                    if (structDamagepercent < 0)
                    {
                        structDamagepercent = 0;
                    }
                    structDamage = (structDamagepercent / 100) * StructValueSample * priceIndex * _numberOfStructures;

                    //Content
                    if (_sampledStructureParameters.ComputeContentDamage)
                    {
                        double contentDamagePercent = _sampledStructureParameters.ContentPercentDamagePairedData.f(depthabovefoundHeight);
                        if (contentDamagePercent > 100)
                        {
                            contentDamagePercent = 100;
                        }
                        contDamage = (contentDamagePercent / 100) * ContentValueSample * priceIndex * _numberOfStructures;
                    }

                    //Vehicle
                    if (_sampledStructureParameters.ComputeVehicleDamage)
                    {
                        double vehicleDamagePercent = _sampledStructureParameters.VehiclePercentDamagePairedData.f(depthabovefoundHeight);
                        if (vehicleDamagePercent > 100)
                        {
                            vehicleDamagePercent = 100;
                        }
                        vehicleDamage = (vehicleDamagePercent / 100) * VehicleValueSample * priceIndex * _numberOfStructures;
                    }

                    //Other
                    if (_sampledStructureParameters.ComputeOtherDamage)
                    {
                        double otherDamagePercent = _sampledStructureParameters.OtherPercentDamagePairedData.f(depthabovefoundHeight);
                        if (otherDamagePercent > 100)
                        {
                            otherDamagePercent = 100;
                        }
                        otherDamage = (otherDamagePercent / 100) * OtherValueSample * priceIndex * _numberOfStructures;
                    }
                }
            }
            consequenceResult.IncrementConsequence(structDamage, contDamage, vehicleDamage, otherDamage);

            return consequenceResult;
        }
        #endregion
    }
}