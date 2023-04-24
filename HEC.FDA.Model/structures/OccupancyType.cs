using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.interfaces;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Model.Messaging;
using System.Collections.Generic;

namespace HEC.FDA.Model.structures
{ //TODO: add messaging and validation 
    public class OccupancyType: ValidationErrorLogger, IContainValidationGroups
    {
        #region Fields
        //fundamental traits
        private string _OccupancyTypeName;
        private string _OccupancyTypeDamageCategory;

        //configuration flags 
        private bool _computeStructureDamage = false;
        private bool _computeContentDamage = false;
        private bool _computeVehicleDamage = false;
        private bool _computeOtherDamage = false;

        private bool _useContentToStructureValueRatio = false;
        private bool _useOtherToStructureValueRatio = false;

        //damage functions
        private UncertainPairedData _structureDepthPercentDamageFunction;
        private UncertainPairedData _contentDepthPercentDamageFunction;
        private UncertainPairedData _vehicleDepthPercentDamageFunction;
        private UncertainPairedData _OtherDepthPercentDamageFunction;

        private FirstFloorElevationUncertainty _firstFloorElevationError;
        private bool _firstFloorElevationIsLogNormal = false;
        private ValueUncertainty _structureValueError;
        private bool _structureValueIsLogNormal = false;
        private ValueUncertainty _contentValueError;
        private bool _contentValueIsLogNormal = false;
        private ValueUncertainty _vehicleValueError;
        private bool _vehicleValueIsLogNormal = false;
        private ValueUncertainty _otherValueError;
        private bool _otherValueIsLogNormal = false;
        private ValueRatioWithUncertainty _contentToStructureValueRatio;
        private ValueRatioWithUncertainty _otherToStructureValueRatio;
        #endregion

        #region Properties 
        internal bool UseContentToStructureValueRatio
        {
            get
            {
                return _useContentToStructureValueRatio;
            }
        }
        internal bool UseOtherToStructureValueRatio
        {
            get
            {
                return _useOtherToStructureValueRatio;
            }
        }
        internal bool ComputeStructureDamage
        {
            get { return _computeStructureDamage; }
        }
        internal bool ComputeContentDamage
        {
            get { return _computeContentDamage; }
        }
        internal bool ComputeOtherDamage
        {
            get { return _computeOtherDamage; }
        }
        internal bool ComputeVehicleDamage
        {
            get { return _computeVehicleDamage; }
        }
        public string DamageCategory
        {
            get { return _OccupancyTypeDamageCategory; }
        }
        public string Name
        {
            get { return _OccupancyTypeName; }
        }

        public List<ValidationGroup> ValidationGroups { get; } = new List<ValidationGroup>();

        #endregion
        #region Constructor
        /// <summary>
        /// Use the static builder to create occupancy types
        /// </summary>
        private OccupancyType()
        {
            _structureDepthPercentDamageFunction = new UncertainPairedData();
            _contentDepthPercentDamageFunction = new UncertainPairedData();
            _vehicleDepthPercentDamageFunction = new UncertainPairedData();
            _OtherDepthPercentDamageFunction = new UncertainPairedData();
            _firstFloorElevationError = new FirstFloorElevationUncertainty();
            _structureValueError = new ValueUncertainty();
            _contentValueError = new ValueUncertainty();
            _vehicleValueError = new ValueUncertainty();
            _otherValueError = new ValueUncertainty();
            _contentToStructureValueRatio = new ValueRatioWithUncertainty();
            _otherToStructureValueRatio = new ValueRatioWithUncertainty();


            List<ValidationErrorLogger> validationObjects = new List<ValidationErrorLogger>() 
            { 
                _structureValueError,
                _contentValueError,
                _vehicleValueError,
                _otherValueError
            };
            List<string> validationIntroMsgs = new List<string>() 
            { 
                "The structure value uncertainty has the following errors:",
                "The structure value uncertainty has the following errors:",
                "The structure value uncertainty has the following errors:",
                "The structure value uncertainty has the following errors:",
            };
            ValidationGroup vg = new ValidationGroup(validationObjects, validationIntroMsgs, "This occtype has the following errors:");
            ValidationGroups.Add(vg);

        }
        #endregion
        #region Methods
        public DeterministicOccupancyType Sample(IProvideRandomNumbers randomNumbers, bool computeIsDeterministic = false)
        {
            if (ErrorLevel >= ErrorLevel.Major)
            {
                Message message = new Message($"Occupancy type {Name} has at least one major error and cannot be sampled. An arbitrary set of sampled parameters is being returned");
                ReportMessage(this, new MessageEventArgs(message));
                return new DeterministicOccupancyType();
            }
            //damage functions
            IPairedData structDamagePairedData = _structureDepthPercentDamageFunction.SamplePairedData(randomNumbers.NextRandom(), computeIsDeterministic);
            //This hack is here because we need to create these functions before assigning their value;
            //This hack feels less hacky than having an empty paired data constructor with the same junk 
            IPairedData contentDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });
            IPairedData vehicleDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });
            IPairedData otherDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });

            //parameters
            double firstFloorElevationOffsetSampled = _firstFloorElevationError.Sample(randomNumbers.NextRandom(), computeIsDeterministic);
            double structureValueOffsetSampled = _structureValueError.Sample(randomNumbers.NextRandom(), computeIsDeterministic);
            double contentValueOffsetSampled = 0;
            double contentValueRatioSampled = 0;
            if (_computeContentDamage)
            {
                if (_useContentToStructureValueRatio)
                {
                    contentValueRatioSampled = (_contentToStructureValueRatio.Sample(randomNumbers.NextRandom(), computeIsDeterministic));
                }
                else
                {
                    contentValueOffsetSampled = _contentValueError.Sample(randomNumbers.NextRandom(), computeIsDeterministic);
                }
                contentDamagePairedData = _contentDepthPercentDamageFunction.SamplePairedData(randomNumbers.NextRandom(), computeIsDeterministic);
            }
            double otherValueOffsetSampled = 0;
            double otherValueRatioSampled = 0;
            if (_computeOtherDamage)
            {
                if (_useOtherToStructureValueRatio)
                {
                    otherValueRatioSampled = (_otherToStructureValueRatio.Sample(randomNumbers.NextRandom(), computeIsDeterministic))/100;
                }
                else
                {
                    otherValueOffsetSampled = _otherValueError.Sample(randomNumbers.NextRandom(), computeIsDeterministic);
                }
                otherDamagePairedData = _OtherDepthPercentDamageFunction.SamplePairedData(randomNumbers.NextRandom(), computeIsDeterministic);
            }

            double vehicleValueOffsetSampled = 0;
            if (_computeVehicleDamage)
            {
                vehicleValueOffsetSampled = _vehicleValueError.Sample(randomNumbers.NextRandom(), computeIsDeterministic);
                vehicleDamagePairedData = _vehicleDepthPercentDamageFunction.SamplePairedData(randomNumbers.NextRandom(), computeIsDeterministic);
            }
            return new DeterministicOccupancyType(_OccupancyTypeName, _OccupancyTypeDamageCategory, structDamagePairedData, firstFloorElevationOffsetSampled, structureValueOffsetSampled, _computeContentDamage, _computeVehicleDamage, _computeOtherDamage, contentDamagePairedData, contentValueOffsetSampled, _useContentToStructureValueRatio, contentValueRatioSampled, vehicleDamagePairedData, vehicleValueOffsetSampled, otherDamagePairedData, otherValueOffsetSampled, _useOtherToStructureValueRatio, otherValueRatioSampled, _structureValueIsLogNormal, _contentValueIsLogNormal, _otherValueIsLogNormal, _vehicleValueIsLogNormal, _firstFloorElevationIsLogNormal);
        }


        public static OccupancyTypeBuilder builder()
        {
            return new OccupancyTypeBuilder(new OccupancyType());
        }
        #endregion

        public class OccupancyTypeBuilder
        {
            private OccupancyType _occupancyType;
            internal OccupancyTypeBuilder(OccupancyType occupancyType)
            {
                _occupancyType = occupancyType;
            }
            public OccupancyType build()
            {
                _occupancyType.Validate();
                return _occupancyType;
            }
            public OccupancyTypeBuilder withDamageCategory(string damageCategory)
            {
                _occupancyType._OccupancyTypeDamageCategory = damageCategory;
                return new OccupancyTypeBuilder(_occupancyType);
            }
            public OccupancyTypeBuilder withName(string name)
            {
                _occupancyType._OccupancyTypeName = name;
                return new OccupancyTypeBuilder(_occupancyType);
            }
            public OccupancyTypeBuilder withStructureDepthPercentDamage(UncertainPairedData structureDepthPercentDamage)
            {
                _occupancyType._structureDepthPercentDamageFunction = structureDepthPercentDamage;
                _occupancyType.AddSinglePropertyRule(nameof(structureDepthPercentDamage), new Rule(() => { structureDepthPercentDamage.Validate(); return !structureDepthPercentDamage.HasErrors; }, $"The structure depth-percent damage function named {structureDepthPercentDamage.Name} associated with occupancy type {_occupancyType.Name} has errors: "+structureDepthPercentDamage.GetErrors().ToString(), structureDepthPercentDamage.ErrorLevel)) ;
                _occupancyType._computeStructureDamage = true;
                return new OccupancyTypeBuilder(_occupancyType);
            }

            public OccupancyTypeBuilder withContentDepthPercentDamage(UncertainPairedData contentDepthPercentDamage)
            {
                _occupancyType._contentDepthPercentDamageFunction = contentDepthPercentDamage;
                _occupancyType.AddSinglePropertyRule(nameof(contentDepthPercentDamage), new Rule(() => { contentDepthPercentDamage.Validate(); return !contentDepthPercentDamage.HasErrors; }, $"The content depth percent-damage function named {contentDepthPercentDamage.Name} associated with occupancy type {_occupancyType.Name} has errors: " + contentDepthPercentDamage.GetErrors().ToString(), contentDepthPercentDamage.ErrorLevel));
                _occupancyType._computeContentDamage = true;
                return new OccupancyTypeBuilder(_occupancyType);
            }

            public OccupancyTypeBuilder withVehicleDepthPercentDamage(UncertainPairedData vehicleDepthPercentDamage)
            {
                _occupancyType._vehicleDepthPercentDamageFunction = vehicleDepthPercentDamage;
                _occupancyType.AddSinglePropertyRule(nameof(vehicleDepthPercentDamage), new Rule(() => { vehicleDepthPercentDamage.Validate(); return !vehicleDepthPercentDamage.HasErrors; }, $"The vehicle depth percent-damage function named {vehicleDepthPercentDamage.Name} associated with occupancy type {_occupancyType.Name}  has errors: " + vehicleDepthPercentDamage.GetErrors().ToString(), vehicleDepthPercentDamage.ErrorLevel));
                _occupancyType._computeVehicleDamage = true;
                return new OccupancyTypeBuilder(_occupancyType);
            }

            public OccupancyTypeBuilder withOtherDepthPercentDamage(UncertainPairedData otherDepthPercentDamage)
            {
                _occupancyType._OtherDepthPercentDamageFunction = otherDepthPercentDamage;
                _occupancyType.AddSinglePropertyRule(nameof(otherDepthPercentDamage), new Rule(() => { otherDepthPercentDamage.Validate(); return !otherDepthPercentDamage.HasErrors; }, $"The vehicle depth percent-damage function named {otherDepthPercentDamage.Name} associated with occupancy type {_occupancyType.Name} has errors: " + otherDepthPercentDamage.GetErrors().ToString(), otherDepthPercentDamage.ErrorLevel));
                _occupancyType._computeOtherDamage = true;
                return new OccupancyTypeBuilder(_occupancyType);
            }

            public OccupancyTypeBuilder withStructureValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _occupancyType._structureValueError = valueUncertainty;
                if(valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _occupancyType._structureValueIsLogNormal = true;
                }
                _occupancyType.AddSinglePropertyRule(nameof(valueUncertainty), new Rule(() => { valueUncertainty.Validate(); return !valueUncertainty.HasErrors; }, $"The uncertainty about structure value for occupancy type {_occupancyType.Name} has errors: " + valueUncertainty.GetErrors().ToString(), valueUncertainty.ErrorLevel));
                return new OccupancyTypeBuilder(_occupancyType);
            }

            public OccupancyTypeBuilder withContentValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _occupancyType._contentValueError = valueUncertainty;
                if (valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _occupancyType._contentValueIsLogNormal = true;
                }
                _occupancyType.AddSinglePropertyRule(nameof(valueUncertainty), new Rule(() => { valueUncertainty.Validate(); return !valueUncertainty.HasErrors; }, $"The uncertainty about content value for occupancy type {_occupancyType.Name} has errors: " + valueUncertainty.GetErrors().ToString(), valueUncertainty.ErrorLevel));
                return new OccupancyTypeBuilder(_occupancyType);
            }
            public OccupancyTypeBuilder withVehicleValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _occupancyType._vehicleValueError = valueUncertainty;
                if (valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _occupancyType._vehicleValueIsLogNormal = true;
                }
                _occupancyType.AddSinglePropertyRule(nameof(valueUncertainty), new Rule(() => { valueUncertainty.Validate(); return !valueUncertainty.HasErrors; }, $"The uncertainty about vehicle value for occupancy type {_occupancyType.Name} has errors: " + valueUncertainty.GetErrors().ToString(), valueUncertainty.ErrorLevel));
                return new OccupancyTypeBuilder(_occupancyType);
            }
            public OccupancyTypeBuilder withOtherValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _occupancyType._otherValueError = valueUncertainty;
                if (valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _occupancyType._otherValueIsLogNormal = true;
                }
                _occupancyType.AddSinglePropertyRule(nameof(valueUncertainty), new Rule(() => { valueUncertainty.Validate(); return !valueUncertainty.HasErrors; }, $"The uncertainty about other value for occupancy type {_occupancyType.Name} has errors: " + valueUncertainty.GetErrors().ToString(), valueUncertainty.ErrorLevel));
                return new OccupancyTypeBuilder(_occupancyType);
            }
            public OccupancyTypeBuilder withContentToStructureValueRatio(ValueRatioWithUncertainty valueRatioWithUncertainty)
            {
                _occupancyType._contentToStructureValueRatio = valueRatioWithUncertainty;
                _occupancyType.AddSinglePropertyRule(nameof(valueRatioWithUncertainty), new Rule(() => { valueRatioWithUncertainty.Validate(); return !valueRatioWithUncertainty.HasErrors; }, $"The uncertainty about the content to structure value ratio for occupancy type {_occupancyType.Name} has errors: " + valueRatioWithUncertainty.GetErrors().ToString(), valueRatioWithUncertainty.ErrorLevel));
                _occupancyType._useContentToStructureValueRatio = true;
                return new OccupancyTypeBuilder(_occupancyType);
            }
            public OccupancyTypeBuilder withOtherToStructureValueRatio(ValueRatioWithUncertainty valueRatioWithUncertainty)
            {
                _occupancyType._otherToStructureValueRatio = valueRatioWithUncertainty;
                _occupancyType.AddSinglePropertyRule(nameof(valueRatioWithUncertainty), new Rule(() => { valueRatioWithUncertainty.Validate(); return !valueRatioWithUncertainty.HasErrors; }, $"The uncertainty about the other to structure value ratio for occupancy type {_occupancyType.Name} has errors: " + valueRatioWithUncertainty.GetErrors().ToString(), valueRatioWithUncertainty.ErrorLevel));
                _occupancyType._useOtherToStructureValueRatio = true;
                return new OccupancyTypeBuilder(_occupancyType);
            }
            public OccupancyTypeBuilder withFirstFloorElevationUncertainty(FirstFloorElevationUncertainty firstFloorElevationUncertainty)
            {
                _occupancyType._firstFloorElevationError = firstFloorElevationUncertainty;
                if (firstFloorElevationUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _occupancyType._firstFloorElevationIsLogNormal = true;
                }
                _occupancyType.AddSinglePropertyRule(nameof(firstFloorElevationUncertainty), new Rule(() => { firstFloorElevationUncertainty.Validate(); return !firstFloorElevationUncertainty.HasErrors; }, $"The uncertainty about the other to structure value ratio for occupancy type {_occupancyType.Name} has errors: " + firstFloorElevationUncertainty.GetErrors().ToString(), firstFloorElevationUncertainty.ErrorLevel));

                return new OccupancyTypeBuilder(_occupancyType);
            }
        }
    }
}