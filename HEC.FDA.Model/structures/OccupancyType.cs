using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.interfaces;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Model.Messaging;
using System.Collections.Generic;

namespace HEC.FDA.Model.structures
{ //TODO: add messaging and validation 
    public class OccupancyType: ValidationErrorLogger
    {
        #region Fields
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
        public DeterministicOccupancyType Sample(IProvideRandomNumbers randomNumbers, bool computeIsDeterministic = false)
        {
            if (ErrorLevel >= ErrorLevel.Major)
            {
                Message message = new($"Occupancy type {Name} has at least one major error and cannot be sampled. An arbitrary set of sampled parameters is being returned");
                ReportMessage(this, new MessageEventArgs(message));
                return new DeterministicOccupancyType();
            }
            //damage functions
            IPairedData structDamagePairedData = _StructureDepthPercentDamageFunction.SamplePairedData(randomNumbers.NextRandom(), computeIsDeterministic);
            //This hack is here because we need to create these functions before assigning their value;
            //This hack feels less hacky than having an empty paired data constructor with the same junk 
            IPairedData contentDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });
            IPairedData vehicleDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });
            IPairedData otherDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });

            //parameters
            double firstFloorElevationOffsetSampled = _FirstFloorElevationError.Sample(randomNumbers.NextRandom(), computeIsDeterministic);
            double structureValueOffsetSampled = _StructureValueError.Sample(randomNumbers.NextRandom(), computeIsDeterministic);
            double contentValueOffsetSampled = 0;
            double contentValueRatioSampled = 0;
            if (ComputeContentDamage)
            {
                if (UseContentToStructureValueRatio)
                {
                    contentValueRatioSampled = (_ContentToStructureValueRatio.Sample(randomNumbers.NextRandom(), computeIsDeterministic));
                }
                else
                {
                    contentValueOffsetSampled = _ContentValueError.Sample(randomNumbers.NextRandom(), computeIsDeterministic);
                }
                contentDamagePairedData = _ContentDepthPercentDamageFunction.SamplePairedData(randomNumbers.NextRandom(), computeIsDeterministic);
            }
            double otherValueOffsetSampled = 0;
            double otherValueRatioSampled = 0;
            if (ComputeOtherDamage)
            {
                if (UseOtherToStructureValueRatio)
                {
                    otherValueRatioSampled = (_OtherToStructureValueRatio.Sample(randomNumbers.NextRandom(), computeIsDeterministic))/100;
                }
                else
                {
                    otherValueOffsetSampled = _OtherValueError.Sample(randomNumbers.NextRandom(), computeIsDeterministic);
                }
                otherDamagePairedData = _OtherDepthPercentDamageFunction.SamplePairedData(randomNumbers.NextRandom(), computeIsDeterministic);
            }

            double vehicleValueOffsetSampled = 0;
            if (ComputeVehicleDamage)
            {
                vehicleValueOffsetSampled = _VehicleValueError.Sample(randomNumbers.NextRandom(), computeIsDeterministic);
                vehicleDamagePairedData = _VehicleDepthPercentDamageFunction.SamplePairedData(randomNumbers.NextRandom(), computeIsDeterministic);
            }
            return new DeterministicOccupancyType(_OccupancyTypeName, _OccupancyTypeDamageCategory, structDamagePairedData, firstFloorElevationOffsetSampled, structureValueOffsetSampled, ComputeContentDamage, ComputeVehicleDamage, ComputeOtherDamage, contentDamagePairedData, contentValueOffsetSampled, UseContentToStructureValueRatio, contentValueRatioSampled, vehicleDamagePairedData, vehicleValueOffsetSampled, otherDamagePairedData, otherValueOffsetSampled, UseOtherToStructureValueRatio, otherValueRatioSampled, _StructureValueIsLogNormal, _ContentValueIsLogNormal, _OtherValueIsLogNormal, _VehicleValueIsLogNormal, _FirstFloorElevationIsLogNormal);
        }


        public static OccupancyTypeBuilder Builder()
        {
            return new OccupancyTypeBuilder(new OccupancyType());
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
                _OccupancyType.Validate();
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
                _OccupancyType.AddSinglePropertyRule(nameof(structureDepthPercentDamage), new Rule(() => { structureDepthPercentDamage.Validate(); return !structureDepthPercentDamage.HasErrors; }, $"The structure depth-percent damage function named {structureDepthPercentDamage.Name} associated with occupancy type {_OccupancyType.Name} has errors: "+structureDepthPercentDamage.GetErrorMessages(), structureDepthPercentDamage.ErrorLevel)) ;
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithContentDepthPercentDamage(UncertainPairedData contentDepthPercentDamage)
            {
                _OccupancyType._ContentDepthPercentDamageFunction = contentDepthPercentDamage;
                _OccupancyType.AddSinglePropertyRule(nameof(contentDepthPercentDamage), new Rule(() => { contentDepthPercentDamage.Validate(); return !contentDepthPercentDamage.HasErrors; }, $"The content depth percent-damage function named {contentDepthPercentDamage.Name} associated with occupancy type {_OccupancyType.Name} has errors: " + contentDepthPercentDamage.GetErrorMessages(), contentDepthPercentDamage.ErrorLevel));
                _OccupancyType.ComputeContentDamage = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithVehicleDepthPercentDamage(UncertainPairedData vehicleDepthPercentDamage)
            {
                _OccupancyType._VehicleDepthPercentDamageFunction = vehicleDepthPercentDamage;
                _OccupancyType.AddSinglePropertyRule(nameof(vehicleDepthPercentDamage), new Rule(() => { vehicleDepthPercentDamage.Validate(); return !vehicleDepthPercentDamage.HasErrors; }, $"The vehicle depth percent-damage function named {vehicleDepthPercentDamage.Name} associated with occupancy type {_OccupancyType.Name}  has errors: " + vehicleDepthPercentDamage.GetErrorMessages(), vehicleDepthPercentDamage.ErrorLevel));
                _OccupancyType.ComputeVehicleDamage = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithOtherDepthPercentDamage(UncertainPairedData otherDepthPercentDamage)
            {
                _OccupancyType._OtherDepthPercentDamageFunction = otherDepthPercentDamage;
                _OccupancyType.AddSinglePropertyRule(nameof(otherDepthPercentDamage), new Rule(() => { otherDepthPercentDamage.Validate(); return !otherDepthPercentDamage.HasErrors; }, $"The vehicle depth percent-damage function named {otherDepthPercentDamage.Name} associated with occupancy type {_OccupancyType.Name} has errors: " + otherDepthPercentDamage.GetErrorMessages(), otherDepthPercentDamage.ErrorLevel));
                _OccupancyType.ComputeOtherDamage = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithStructureValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _OccupancyType._StructureValueError = valueUncertainty;
                if(valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _OccupancyType._StructureValueIsLogNormal = true;
                }
                _OccupancyType.AddSinglePropertyRule(nameof(valueUncertainty), new Rule(() => { valueUncertainty.Validate(); return !valueUncertainty.HasErrors; }, $"The uncertainty about structure value for occupancy type {_OccupancyType.Name} has errors: " + valueUncertainty.GetErrorMessages(), valueUncertainty.ErrorLevel));
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithContentValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _OccupancyType._ContentValueError = valueUncertainty;
                if (valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _OccupancyType._ContentValueIsLogNormal = true;
                }
                _OccupancyType.AddSinglePropertyRule(nameof(valueUncertainty), new Rule(() => { valueUncertainty.Validate(); return !valueUncertainty.HasErrors; }, $"The uncertainty about content value for occupancy type {_OccupancyType.Name} has errors: " + valueUncertainty.GetErrorMessages(), valueUncertainty.ErrorLevel));
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithVehicleValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _OccupancyType._VehicleValueError = valueUncertainty;
                if (valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _OccupancyType._VehicleValueIsLogNormal = true;
                }
                _OccupancyType.AddSinglePropertyRule(nameof(valueUncertainty), new Rule(() => { valueUncertainty.Validate(); return !valueUncertainty.HasErrors; }, $"The uncertainty about vehicle value for occupancy type {_OccupancyType.Name} has errors: " + valueUncertainty.GetErrorMessages(), valueUncertainty.ErrorLevel));
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithOtherValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _OccupancyType._OtherValueError = valueUncertainty;
                if (valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _OccupancyType._OtherValueIsLogNormal = true;
                }
                _OccupancyType.AddSinglePropertyRule(nameof(valueUncertainty), new Rule(() => { valueUncertainty.Validate(); return !valueUncertainty.HasErrors; }, $"The uncertainty about other value for occupancy type {_OccupancyType.Name} has errors: " + valueUncertainty.GetErrorMessages(), valueUncertainty.ErrorLevel));
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithContentToStructureValueRatio(ValueRatioWithUncertainty valueRatioWithUncertainty)
            {
                _OccupancyType._ContentToStructureValueRatio = valueRatioWithUncertainty;
                _OccupancyType.AddSinglePropertyRule(nameof(valueRatioWithUncertainty), new Rule(() => { valueRatioWithUncertainty.Validate(); return !valueRatioWithUncertainty.HasErrors; }, $"The uncertainty about the content to structure value ratio for occupancy type {_OccupancyType.Name} has errors: " + valueRatioWithUncertainty.GetErrorMessages(), valueRatioWithUncertainty.ErrorLevel));
                _OccupancyType.UseContentToStructureValueRatio = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithOtherToStructureValueRatio(ValueRatioWithUncertainty valueRatioWithUncertainty)
            {
                _OccupancyType._OtherToStructureValueRatio = valueRatioWithUncertainty;
                _OccupancyType.AddSinglePropertyRule(nameof(valueRatioWithUncertainty), new Rule(() => { valueRatioWithUncertainty.Validate(); return !valueRatioWithUncertainty.HasErrors; }, $"The uncertainty about the other to structure value ratio for occupancy type {_OccupancyType.Name} has errors: " + valueRatioWithUncertainty.GetErrorMessages(), valueRatioWithUncertainty.ErrorLevel));
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
                _OccupancyType.AddSinglePropertyRule(nameof(firstFloorElevationUncertainty), new Rule(() => { firstFloorElevationUncertainty.Validate(); return !firstFloorElevationUncertainty.HasErrors; }, $"The uncertainty about the other to structure value ratio for occupancy type {_OccupancyType.Name} has errors: " + firstFloorElevationUncertainty.GetErrorMessages(), firstFloorElevationUncertainty.ErrorLevel));

                return new OccupancyTypeBuilder(_OccupancyType);
            }
        }
    }
}