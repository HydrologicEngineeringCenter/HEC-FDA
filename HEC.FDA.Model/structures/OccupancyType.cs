using System;
using HEC.FDA.Model.paireddata;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Model.Messaging;

namespace HEC.FDA.Model.structures
{ //TODO: add messaging and validation 
    public class OccupancyType : PropertyValidationHelper, IDontImplementValidationButMyPropertiesDo
    {
        #region Fields
        //seeds
        private const int DEPTH_DAMAGE_SEED = 1234;
        private const int FIRST_FLOOR_SEED = 2345;
        private const int STRUCTURE_VALUE_SEED = 3456;
        private const int CONTENT_VALUE_SEED = 4567;
        private const int OTHER_VALUE_SEED = 5678;
        private const int VEHICLE_VALUE_SEED = 6789;

        //fundamental traits
        private string _OccupancyTypeName;
        private string _OccupancyTypeDamageCategory;

        //damage functions
        private UncertainPairedData _StructureDepthPercentDamageFunction;
        private UncertainPairedData _ContentDepthPercentDamageFunction;
        private UncertainPairedData _VehicleDepthPercentDamageFunction;
        private UncertainPairedData _OtherDepthPercentDamageFunction;

        private FirstFloorElevationUncertainty _FirstFloorElevationError;
        private bool _FirstFloorElevationIsLogNormal = false;
        private ValueUncertainty _StructureValueError;
        private bool _StructureValueIsLogNormal = false;
        private ValueUncertainty _ContentValueError;
        private bool _ContentValueIsLogNormal = false;
        private ValueUncertainty _VehicleValueError;
        private bool _VehicleValueIsLogNormal = false;
        private ValueUncertainty _OtherValueError;
        private bool _OtherValueIsLogNormal = false;
        private ValueRatioWithUncertainty _ContentToStructureValueRatio;
        private ValueRatioWithUncertainty _OtherToStructureValueRatio;

        public event MessageReportedEventHandler MessageReport;
        #endregion

        #region Properties 
        public string DamageCategory
        {
            get { return _OccupancyTypeDamageCategory; }
        }
        public string Name
        {
            get { return _OccupancyTypeName; }
        }
        public bool UseContentToStructureValueRatio { get; set; }
        public bool UseOtherToStructureValueRatio { get; set; }
        public bool ComputeContentDamage { get; set; }
        public bool ComputeVehicleDamage { get; set; }
        public bool ComputeOtherDamage { get; set; }
        

        #endregion
        #region Constructor
        /// <summary>
        /// Use the static builder to create occupancy types
        /// </summary>
        private OccupancyType()
        {
            _StructureDepthPercentDamageFunction = new UncertainPairedData();
            _ContentDepthPercentDamageFunction = new UncertainPairedData();
            _VehicleDepthPercentDamageFunction = new UncertainPairedData();
            _OtherDepthPercentDamageFunction = new UncertainPairedData();
            _FirstFloorElevationError = new FirstFloorElevationUncertainty();
            _StructureValueError = new ValueUncertainty();
            _ContentValueError = new ValueUncertainty();
            _VehicleValueError = new ValueUncertainty();
            _OtherValueError = new ValueUncertainty();
            _ContentToStructureValueRatio = new ValueRatioWithUncertainty();
            _OtherToStructureValueRatio = new ValueRatioWithUncertainty();

        }
        #endregion
        #region Methods
        public void GenerateRandomNumbers(long size)
        {
            _StructureDepthPercentDamageFunction.GenerateRandomNumbers(DEPTH_DAMAGE_SEED, size);
            _StructureValueError.GenerateRandomNumbers(STRUCTURE_VALUE_SEED, size);
            _FirstFloorElevationError.GenerateRandomNumbers(FIRST_FLOOR_SEED, size);
            if (ComputeContentDamage)
            {
                _ContentDepthPercentDamageFunction.GenerateRandomNumbers(DEPTH_DAMAGE_SEED, size);
                if (UseContentToStructureValueRatio)
                {
                    _ContentToStructureValueRatio.GenerateRandomNumbers(CONTENT_VALUE_SEED, size);
                } else
                {
                    _ContentValueError.GenerateRandomNumbers(CONTENT_VALUE_SEED, size);
                }
            }
            if (ComputeOtherDamage)
            {
                _OtherDepthPercentDamageFunction.GenerateRandomNumbers(DEPTH_DAMAGE_SEED, size);
                if (UseOtherToStructureValueRatio)
                {
                    _OtherToStructureValueRatio.GenerateRandomNumbers(OTHER_VALUE_SEED, size);
                } else
                {
                    _OtherValueError.GenerateRandomNumbers(OTHER_VALUE_SEED,size);
                }
            }
            if (ComputeVehicleDamage)
            {
                _VehicleDepthPercentDamageFunction.GenerateRandomNumbers(DEPTH_DAMAGE_SEED, size);
                _VehicleValueError.GenerateRandomNumbers(VEHICLE_VALUE_SEED, size);
            }
        }

        public DeterministicOccupancyType Sample(double probability)
        {
            if (ErrorLevel >= ErrorLevel.Major)
            {
                string message = $"Occupancy type {Name} has at least one major error and cannot be sampled.";
                throw new Exception(message);
            }
            //damage functions
            IPairedData structDamagePairedData = _StructureDepthPercentDamageFunction.SamplePairedData(probability);
            //This hack is here because we need to create these functions before assigning their value;
            //This hack feels less hacky than having an empty paired data constructor with the same junk 
            IPairedData contentDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });
            IPairedData vehicleDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });
            IPairedData otherDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });

            //parameters
            double firstFloorElevationOffsetSampled = _FirstFloorElevationError.Sample(probability);
            double structureValueOffsetSampled = _StructureValueError.Sample(probability);
            double contentValueOffsetSampled = 0;
            double contentValueRatioSampled = 0;
            if (ComputeContentDamage)
            {
                if (UseContentToStructureValueRatio)
                {
                    contentValueRatioSampled = _ContentToStructureValueRatio.Sample(probability);
                }
                else
                {
                    contentValueOffsetSampled = _ContentValueError.Sample(probability);
                }
                contentDamagePairedData = _ContentDepthPercentDamageFunction.SamplePairedData(probability);
            }
            double otherValueOffsetSampled = 0;
            double otherValueRatioSampled = 0;
            if (ComputeOtherDamage)
            {
                if (UseOtherToStructureValueRatio)
                {
                    otherValueRatioSampled = _OtherToStructureValueRatio.Sample(probability) / 100;
                }
                else
                {
                    otherValueOffsetSampled = _OtherValueError.Sample(probability);
                }
                otherDamagePairedData = _OtherDepthPercentDamageFunction.SamplePairedData(probability);
            }

            double vehicleValueOffsetSampled = 0;
            if (ComputeVehicleDamage)
            {
                vehicleValueOffsetSampled = _VehicleValueError.Sample(probability);
                vehicleDamagePairedData = _VehicleDepthPercentDamageFunction.SamplePairedData(probability);
            }
            return new DeterministicOccupancyType(_OccupancyTypeName, _OccupancyTypeDamageCategory, structDamagePairedData, firstFloorElevationOffsetSampled, structureValueOffsetSampled, ComputeContentDamage, ComputeVehicleDamage, ComputeOtherDamage, contentDamagePairedData, contentValueOffsetSampled, UseContentToStructureValueRatio, contentValueRatioSampled, vehicleDamagePairedData, vehicleValueOffsetSampled, otherDamagePairedData, otherValueOffsetSampled, UseOtherToStructureValueRatio, otherValueRatioSampled, _StructureValueIsLogNormal, _ContentValueIsLogNormal, _OtherValueIsLogNormal, _VehicleValueIsLogNormal, _FirstFloorElevationIsLogNormal);
        }
        /// <summary>
        /// All sampling methods include a computeIsDeterministic argument that bypasses the iteration number for the retrieval of the deterministic representation of the variable 
        /// </summary>
        /// <param name="iteration"></param> If this method is called during a full compute with uncertainty, random numbers need to have been previously generated, and the correct random number will be pulled for the given iteration number
        /// <param name="computeIsDeterministic"></param> If the method is instead called during a deterministic compute, the iteration number will be bypassed and the deterministic representation will be returned
        /// <returns></returns> a set of deterministic occupancy types are returned with all uncertain variables sampled 
        public DeterministicOccupancyType Sample(long iteration, bool computeIsDeterministic = false)
        {
            if (ErrorLevel >= ErrorLevel.Major)
            {
                string message = $"Occupancy type {Name} has at least one major error and cannot be sampled.";
                throw new Exception(message);
            }
            //damage functions
            IPairedData structDamagePairedData = _StructureDepthPercentDamageFunction.SamplePairedData(iteration, computeIsDeterministic);
            //This hack is here because we need to create these functions before assigning their value;
            //This hack feels less hacky than having an empty paired data constructor with the same junk 
            IPairedData contentDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });
            IPairedData vehicleDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });
            IPairedData otherDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });

            //parameters
            double firstFloorElevationOffsetSampled = _FirstFloorElevationError.Sample(iteration, computeIsDeterministic);
            double structureValueOffsetSampled = _StructureValueError.Sample(iteration, computeIsDeterministic);
            double contentValueOffsetSampled = 0;
            double contentValueRatioSampled = 0;
            if (ComputeContentDamage)
            {
                if (UseContentToStructureValueRatio)
                {
                    contentValueRatioSampled = (_ContentToStructureValueRatio.Sample(iteration, computeIsDeterministic));
                }
                else
                {
                    contentValueOffsetSampled = _ContentValueError.Sample(iteration, computeIsDeterministic);
                }
                contentDamagePairedData = _ContentDepthPercentDamageFunction.SamplePairedData(iteration, computeIsDeterministic);
            }
            double otherValueOffsetSampled = 0;
            double otherValueRatioSampled = 0;
            if (ComputeOtherDamage)
            {
                if (UseOtherToStructureValueRatio)
                {
                    otherValueRatioSampled = (_OtherToStructureValueRatio.Sample(iteration, computeIsDeterministic)) / 100;
                }
                else
                {
                    otherValueOffsetSampled = _OtherValueError.Sample(iteration, computeIsDeterministic);
                }
                otherDamagePairedData = _OtherDepthPercentDamageFunction.SamplePairedData(iteration, computeIsDeterministic);
            }

            double vehicleValueOffsetSampled = 0;
            if (ComputeVehicleDamage)
            {
                vehicleValueOffsetSampled = _VehicleValueError.Sample(iteration, computeIsDeterministic);
                vehicleDamagePairedData = _VehicleDepthPercentDamageFunction.SamplePairedData(iteration, computeIsDeterministic);
            }
            return new DeterministicOccupancyType(_OccupancyTypeName, _OccupancyTypeDamageCategory, structDamagePairedData, firstFloorElevationOffsetSampled, structureValueOffsetSampled, ComputeContentDamage, ComputeVehicleDamage, ComputeOtherDamage, contentDamagePairedData, contentValueOffsetSampled, UseContentToStructureValueRatio, contentValueRatioSampled, vehicleDamagePairedData, vehicleValueOffsetSampled, otherDamagePairedData, otherValueOffsetSampled, UseOtherToStructureValueRatio, otherValueRatioSampled, _StructureValueIsLogNormal, _ContentValueIsLogNormal, _OtherValueIsLogNormal, _VehicleValueIsLogNormal, _FirstFloorElevationIsLogNormal);
        }

        private void ReportMessage(OccupancyType occupancyType, MessageEventArgs messageEventArgs)
        {
            MessageHub.Register(this);
            MessageReport?.Invoke(occupancyType, messageEventArgs);
            MessageHub.Unregister(this);
        }

        public static OccupancyTypeBuilder Builder()
        {
            return new OccupancyTypeBuilder(new OccupancyType());
        }

        public string GetErrorsFromProperties()
        {
            string identifyingString = $"Occupancy Type {Name} ";
            ErrorLevel minimumLevelToCheckForErrors = ErrorLevel.Unassigned;
            string errors = "";
            errors += _StructureDepthPercentDamageFunction.GetErrorMessages(minimumLevelToCheckForErrors, identifyingString + nameof(_StructureDepthPercentDamageFunction));
            errors += _StructureValueError.GetErrorMessages(minimumLevelToCheckForErrors, identifyingString + nameof(_StructureValueError));
            errors += _FirstFloorElevationError.GetErrorMessages(minimumLevelToCheckForErrors, identifyingString + nameof(_FirstFloorElevationError));
            if (ComputeContentDamage)
            {
                errors += _ContentDepthPercentDamageFunction.GetErrorMessages(minimumLevelToCheckForErrors, identifyingString + nameof(_ContentDepthPercentDamageFunction));
                if (UseContentToStructureValueRatio)
                {
                    errors += _ContentToStructureValueRatio.GetErrorMessages(minimumLevelToCheckForErrors, identifyingString + nameof(_ContentToStructureValueRatio));
                }
                else
                {
                    errors += _ContentValueError.GetErrorMessages(minimumLevelToCheckForErrors, identifyingString + nameof(_ContentValueError));
                }
            }
            if (ComputeOtherDamage)
            {
                errors += _OtherDepthPercentDamageFunction.GetErrorMessages(minimumLevelToCheckForErrors, identifyingString + nameof(_OtherDepthPercentDamageFunction));
                if (UseContentToStructureValueRatio)
                {
                    errors += _OtherToStructureValueRatio.GetErrorMessages(minimumLevelToCheckForErrors, identifyingString + nameof(_OtherToStructureValueRatio));
                }
                else
                {
                    errors += _OtherValueError.GetErrorMessages(minimumLevelToCheckForErrors, identifyingString + nameof(_OtherValueError));
                }
            }
            if (ComputeVehicleDamage)
            {
                errors += _VehicleDepthPercentDamageFunction.GetErrorMessages(minimumLevelToCheckForErrors, identifyingString + nameof(_VehicleDepthPercentDamageFunction));
                errors += _VehicleValueError.GetErrorMessages(minimumLevelToCheckForErrors, identifyingString + nameof(_VehicleValueError));
            }
            return errors;
        }

        public void Validate()
        {
            HasErrors = false;
            ErrorLevel = ErrorLevel.Unassigned;

            ValidateProperty(_StructureDepthPercentDamageFunction);
            ValidateProperty(_FirstFloorElevationError);
            ValidateProperty(_StructureValueError);
            if (ComputeContentDamage)
            {
                ValidateProperty(_ContentDepthPercentDamageFunction);
                if (UseContentToStructureValueRatio)
                {
                    ValidateProperty(_ContentToStructureValueRatio);
                }
                else
                {
                    ValidateProperty(_ContentValueError);
                }
            }
            if (ComputeOtherDamage)
            {
                ValidateProperty(_OtherDepthPercentDamageFunction);
                if (UseOtherToStructureValueRatio)
                {
                    ValidateProperty(_OtherToStructureValueRatio);
                }
                else
                {
                    ValidateProperty(_OtherValueError);
                }
            }
            if (ComputeVehicleDamage)
            {
                ValidateProperty(_VehicleDepthPercentDamageFunction);

                ValidateProperty(_VehicleValueError);

            }
        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        public class OccupancyTypeBuilder
        {
            private readonly OccupancyType _OccupancyType;
            internal OccupancyTypeBuilder(OccupancyType occupancyType)
            {
                _OccupancyType = occupancyType;
            }
            public OccupancyType Build()
            {
                return _OccupancyType;
            }
            public OccupancyTypeBuilder WithDamageCategory(string damageCategory)
            {
                _OccupancyType._OccupancyTypeDamageCategory = damageCategory;
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithName(string name)
            {
                _OccupancyType._OccupancyTypeName = name;
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithStructureDepthPercentDamage(UncertainPairedData structureDepthPercentDamage)
            {
                _OccupancyType._StructureDepthPercentDamageFunction = structureDepthPercentDamage;
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithContentDepthPercentDamage(UncertainPairedData contentDepthPercentDamage)
            {
                _OccupancyType._ContentDepthPercentDamageFunction = contentDepthPercentDamage;
                _OccupancyType.ComputeContentDamage = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithVehicleDepthPercentDamage(UncertainPairedData vehicleDepthPercentDamage)
            {
                _OccupancyType._VehicleDepthPercentDamageFunction = vehicleDepthPercentDamage;
                _OccupancyType.ComputeVehicleDamage = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithOtherDepthPercentDamage(UncertainPairedData otherDepthPercentDamage)
            {
                _OccupancyType._OtherDepthPercentDamageFunction = otherDepthPercentDamage;
                _OccupancyType.ComputeOtherDamage = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithStructureValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _OccupancyType._StructureValueError = valueUncertainty;
                if (valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _OccupancyType._StructureValueIsLogNormal = true;
                }
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithContentValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _OccupancyType._ContentValueError = valueUncertainty;
                if (valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _OccupancyType._ContentValueIsLogNormal = true;
                }
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithVehicleValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _OccupancyType._VehicleValueError = valueUncertainty;
                if (valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _OccupancyType._VehicleValueIsLogNormal = true;
                }
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithOtherValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _OccupancyType._OtherValueError = valueUncertainty;
                if (valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _OccupancyType._OtherValueIsLogNormal = true;
                }
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithContentToStructureValueRatio(ValueRatioWithUncertainty valueRatioWithUncertainty)
            {
                _OccupancyType._ContentToStructureValueRatio = valueRatioWithUncertainty;
                _OccupancyType.UseContentToStructureValueRatio = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithOtherToStructureValueRatio(ValueRatioWithUncertainty valueRatioWithUncertainty)
            {
                _OccupancyType._OtherToStructureValueRatio = valueRatioWithUncertainty;
                _OccupancyType.UseOtherToStructureValueRatio = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithFirstFloorElevationUncertainty(FirstFloorElevationUncertainty firstFloorElevationUncertainty)
            {
                _OccupancyType._FirstFloorElevationError = firstFloorElevationUncertainty;
                if (firstFloorElevationUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _OccupancyType._FirstFloorElevationIsLogNormal = true;
                }

                return new OccupancyTypeBuilder(_OccupancyType);
            }
        }
    }
}