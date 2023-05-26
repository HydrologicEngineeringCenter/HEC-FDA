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
        private bool _ComputeStructureDamage = false;
        private bool _ComputeContentDamage = false;
        private bool _ComputeVehicleDamage = false;
        private bool _ComputeOtherDamage = false;

        private bool _UseContentToStructureValueRatio = false;
        private bool _UseOtherToStructureValueRatio = false;

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
        internal bool UseContentToStructureValueRatio
        {
            get
            {
                return _UseContentToStructureValueRatio;
            }
        }
        internal bool UseOtherToStructureValueRatio
        {
            get
            {
                return _UseOtherToStructureValueRatio;
            }
        }
        internal bool ComputeStructureDamage
        {
            get { return _ComputeStructureDamage; }
        }
        internal bool ComputeContentDamage
        {
            get { return _ComputeContentDamage; }
        }
        internal bool ComputeOtherDamage
        {
            get { return _ComputeOtherDamage; }
        }
        internal bool ComputeVehicleDamage
        {
            get { return _ComputeVehicleDamage; }
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


            List<ValidationErrorLogger> validationObjects = new() 
            { 
                _StructureValueError,
                _ContentValueError,
                _VehicleValueError,
                _OtherValueError
            };
            List<string> validationIntroMsgs = new() 
            { 
                "The structure value uncertainty has the following errors:",
                "The structure value uncertainty has the following errors:",
                "The structure value uncertainty has the following errors:",
                "The structure value uncertainty has the following errors:",
            };
            ValidationGroup vg = new(validationObjects, validationIntroMsgs, "This occtype has the following errors:");
            ValidationGroups.Add(vg);

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
            if (_ComputeContentDamage)
            {
                if (_UseContentToStructureValueRatio)
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
            if (_ComputeOtherDamage)
            {
                if (_UseOtherToStructureValueRatio)
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
            if (_ComputeVehicleDamage)
            {
                vehicleValueOffsetSampled = _VehicleValueError.Sample(randomNumbers.NextRandom(), computeIsDeterministic);
                vehicleDamagePairedData = _VehicleDepthPercentDamageFunction.SamplePairedData(randomNumbers.NextRandom(), computeIsDeterministic);
            }
            return new DeterministicOccupancyType(_OccupancyTypeName, _OccupancyTypeDamageCategory, structDamagePairedData, firstFloorElevationOffsetSampled, structureValueOffsetSampled, _ComputeContentDamage, _ComputeVehicleDamage, _ComputeOtherDamage, contentDamagePairedData, contentValueOffsetSampled, _UseContentToStructureValueRatio, contentValueRatioSampled, vehicleDamagePairedData, vehicleValueOffsetSampled, otherDamagePairedData, otherValueOffsetSampled, _UseOtherToStructureValueRatio, otherValueRatioSampled, _StructureValueIsLogNormal, _ContentValueIsLogNormal, _OtherValueIsLogNormal, _VehicleValueIsLogNormal, _FirstFloorElevationIsLogNormal);
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
                //unregister?
                //unregister each of the objects that got built within the builder pattern 
                //reflect on an instance of this, ask for any field that is a validation data error logger thingy
                //on that thing, if not null, unregister 

                //need something like IUnregisterFromMessageHub
                //in computational method, at the very beginning, we call Validate()
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
                _OccupancyType.AddSinglePropertyRule(nameof(structureDepthPercentDamage), new Rule(() => { structureDepthPercentDamage.Validate(); return !structureDepthPercentDamage.HasErrors; }, $"The structure depth-percent damage function named {structureDepthPercentDamage.Name} associated with occupancy type {_OccupancyType.Name} has errors: "+structureDepthPercentDamage.GetErrors().ToString(), structureDepthPercentDamage.ErrorLevel)) ;
                _OccupancyType._ComputeStructureDamage = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithContentDepthPercentDamage(UncertainPairedData contentDepthPercentDamage)
            {
                _OccupancyType._ContentDepthPercentDamageFunction = contentDepthPercentDamage;
                _OccupancyType.AddSinglePropertyRule(nameof(contentDepthPercentDamage), new Rule(() => { contentDepthPercentDamage.Validate(); return !contentDepthPercentDamage.HasErrors; }, $"The content depth percent-damage function named {contentDepthPercentDamage.Name} associated with occupancy type {_OccupancyType.Name} has errors: " + contentDepthPercentDamage.GetErrors().ToString(), contentDepthPercentDamage.ErrorLevel));
                _OccupancyType._ComputeContentDamage = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithVehicleDepthPercentDamage(UncertainPairedData vehicleDepthPercentDamage)
            {
                _OccupancyType._VehicleDepthPercentDamageFunction = vehicleDepthPercentDamage;
                _OccupancyType.AddSinglePropertyRule(nameof(vehicleDepthPercentDamage), new Rule(() => { vehicleDepthPercentDamage.Validate(); return !vehicleDepthPercentDamage.HasErrors; }, $"The vehicle depth percent-damage function named {vehicleDepthPercentDamage.Name} associated with occupancy type {_OccupancyType.Name}  has errors: " + vehicleDepthPercentDamage.GetErrors().ToString(), vehicleDepthPercentDamage.ErrorLevel));
                _OccupancyType._ComputeVehicleDamage = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithOtherDepthPercentDamage(UncertainPairedData otherDepthPercentDamage)
            {
                _OccupancyType._OtherDepthPercentDamageFunction = otherDepthPercentDamage;
                _OccupancyType.AddSinglePropertyRule(nameof(otherDepthPercentDamage), new Rule(() => { otherDepthPercentDamage.Validate(); return !otherDepthPercentDamage.HasErrors; }, $"The vehicle depth percent-damage function named {otherDepthPercentDamage.Name} associated with occupancy type {_OccupancyType.Name} has errors: " + otherDepthPercentDamage.GetErrors().ToString(), otherDepthPercentDamage.ErrorLevel));
                _OccupancyType._ComputeOtherDamage = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithStructureValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _OccupancyType._StructureValueError = valueUncertainty;
                if(valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _OccupancyType._StructureValueIsLogNormal = true;
                }
                _OccupancyType.AddSinglePropertyRule(nameof(valueUncertainty), new Rule(() => { valueUncertainty.Validate(); return !valueUncertainty.HasErrors; }, $"The uncertainty about structure value for occupancy type {_OccupancyType.Name} has errors: " + valueUncertainty.GetErrors().ToString(), valueUncertainty.ErrorLevel));
                return new OccupancyTypeBuilder(_OccupancyType);
            }

            public OccupancyTypeBuilder WithContentValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _OccupancyType._ContentValueError = valueUncertainty;
                if (valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _OccupancyType._ContentValueIsLogNormal = true;
                }
                _OccupancyType.AddSinglePropertyRule(nameof(valueUncertainty), new Rule(() => { valueUncertainty.Validate(); return !valueUncertainty.HasErrors; }, $"The uncertainty about content value for occupancy type {_OccupancyType.Name} has errors: " + valueUncertainty.GetErrors().ToString(), valueUncertainty.ErrorLevel));
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithVehicleValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _OccupancyType._VehicleValueError = valueUncertainty;
                if (valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _OccupancyType._VehicleValueIsLogNormal = true;
                }
                _OccupancyType.AddSinglePropertyRule(nameof(valueUncertainty), new Rule(() => { valueUncertainty.Validate(); return !valueUncertainty.HasErrors; }, $"The uncertainty about vehicle value for occupancy type {_OccupancyType.Name} has errors: " + valueUncertainty.GetErrors().ToString(), valueUncertainty.ErrorLevel));
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithOtherValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _OccupancyType._OtherValueError = valueUncertainty;
                if (valueUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _OccupancyType._OtherValueIsLogNormal = true;
                }
                _OccupancyType.AddSinglePropertyRule(nameof(valueUncertainty), new Rule(() => { valueUncertainty.Validate(); return !valueUncertainty.HasErrors; }, $"The uncertainty about other value for occupancy type {_OccupancyType.Name} has errors: " + valueUncertainty.GetErrors().ToString(), valueUncertainty.ErrorLevel));
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithContentToStructureValueRatio(ValueRatioWithUncertainty valueRatioWithUncertainty)
            {
                _OccupancyType._ContentToStructureValueRatio = valueRatioWithUncertainty;
                _OccupancyType.AddSinglePropertyRule(nameof(valueRatioWithUncertainty), new Rule(() => { valueRatioWithUncertainty.Validate(); return !valueRatioWithUncertainty.HasErrors; }, $"The uncertainty about the content to structure value ratio for occupancy type {_OccupancyType.Name} has errors: " + valueRatioWithUncertainty.GetErrors().ToString(), valueRatioWithUncertainty.ErrorLevel));
                _OccupancyType._UseContentToStructureValueRatio = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithOtherToStructureValueRatio(ValueRatioWithUncertainty valueRatioWithUncertainty)
            {
                _OccupancyType._OtherToStructureValueRatio = valueRatioWithUncertainty;
                _OccupancyType.AddSinglePropertyRule(nameof(valueRatioWithUncertainty), new Rule(() => { valueRatioWithUncertainty.Validate(); return !valueRatioWithUncertainty.HasErrors; }, $"The uncertainty about the other to structure value ratio for occupancy type {_OccupancyType.Name} has errors: " + valueRatioWithUncertainty.GetErrors().ToString(), valueRatioWithUncertainty.ErrorLevel));
                _OccupancyType._UseOtherToStructureValueRatio = true;
                return new OccupancyTypeBuilder(_OccupancyType);
            }
            public OccupancyTypeBuilder WithFirstFloorElevationUncertainty(FirstFloorElevationUncertainty firstFloorElevationUncertainty)
            {
                _OccupancyType._FirstFloorElevationError = firstFloorElevationUncertainty;
                if (firstFloorElevationUncertainty.DistributionType == Statistics.IDistributionEnum.LogNormal)
                {
                    _OccupancyType._FirstFloorElevationIsLogNormal = true;
                }
                _OccupancyType.AddSinglePropertyRule(nameof(firstFloorElevationUncertainty), new Rule(() => { firstFloorElevationUncertainty.Validate(); return !firstFloorElevationUncertainty.HasErrors; }, $"The uncertainty about the other to structure value ratio for occupancy type {_OccupancyType.Name} has errors: " + firstFloorElevationUncertainty.GetErrors().ToString(), firstFloorElevationUncertainty.ErrorLevel));

                return new OccupancyTypeBuilder(_OccupancyType);
            }
        }
    }
}