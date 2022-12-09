using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.interfaces;

namespace HEC.FDA.Model.structures
{ //TODO: add messaging and validation 
    public class OccupancyType
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
        private ValueUncertainty _structureValueError;
        private ValueUncertainty _contentValueError;
        private ValueUncertainty _vehicleValueError;
        private ValueUncertainty _otherValueError;

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

        #endregion
        #region Constructor
        public OccupancyType()
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
        }
        #endregion
        #region Methods
        public SampledStructureParameters Sample(IProvideRandomNumbers randomNumbers, double structureValue, double firstFloorElevation, double contentValue = -999, double otherValue = -999, double vehicleValue = -999, bool computeIsDeterministic = false)
        {
            //damage functions
            IPairedData structDamagePairedData = _structureDepthPercentDamageFunction.SamplePairedData(randomNumbers.NextRandom(), computeIsDeterministic);
            //This hack is here because we need to create these functions before assigning their value;
            //This hack feels less hacky than having an empty paired data constructor with the same junk 
            IPairedData contentDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });
            IPairedData vehicleDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });
            IPairedData otherDamagePairedData = new PairedData(new double[] { 0 }, new double[] { 0 });

            //parameters
            double firstFloorElevationSampled = _firstFloorElevationError.Sample(firstFloorElevation, randomNumbers.NextRandom(), computeIsDeterministic);
            double structureValueSampled = _structureValueError.Sample(structureValue, randomNumbers.NextRandom(), computeIsDeterministic);
            double contentValueSampled = 0;
            if (_computeContentDamage)
            {
                if (_useContentToStructureValueRatio)
                {
                    contentValueSampled = structureValueSampled * _contentToStructureValueRatio.Sample(randomNumbers.NextRandom(), computeIsDeterministic);
                }
                else
                {
                    contentValueSampled = _contentValueError.Sample(contentValue, randomNumbers.NextRandom(), computeIsDeterministic);
                }
                contentDamagePairedData = _contentDepthPercentDamageFunction.SamplePairedData(randomNumbers.NextRandom(), computeIsDeterministic);
            }
            else
            {
                contentValueSampled = contentValue;
            }
            double otherValueSampled = 0;
            if (_computeOtherDamage)
            {
                if (_useOtherToStructureValueRatio)
                {
                    otherValueSampled = structureValueSampled * _otherToStructureValueRatio.Sample(randomNumbers.NextRandom(), computeIsDeterministic);
                }
                else
                {
                    otherValueSampled = _otherValueError.Sample(otherValue, randomNumbers.NextRandom(), computeIsDeterministic);
                }
                otherDamagePairedData = _OtherDepthPercentDamageFunction.SamplePairedData(randomNumbers.NextRandom(), computeIsDeterministic);
            }
            else
            {
                otherValueSampled = otherValue;
            }
            double vehicleValueSampled = 0;
            if (_computeVehicleDamage)
            {
                vehicleValueSampled = _vehicleValueError.Sample(vehicleValue, randomNumbers.NextRandom(), computeIsDeterministic);
                vehicleDamagePairedData = _vehicleDepthPercentDamageFunction.SamplePairedData(randomNumbers.NextRandom(), computeIsDeterministic);
            }
            else
            {
                vehicleValueSampled = vehicleValue;
            }

            return new SampledStructureParameters(_OccupancyTypeName, _OccupancyTypeDamageCategory, structDamagePairedData, firstFloorElevationSampled, structureValueSampled, _computeContentDamage, _computeVehicleDamage, _computeOtherDamage, contentDamagePairedData, contentValueSampled, vehicleDamagePairedData, vehicleValueSampled, otherDamagePairedData, otherValueSampled);
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
                //validate
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
                _occupancyType._computeStructureDamage = true;
                return new OccupancyTypeBuilder(_occupancyType);
            }

            public OccupancyTypeBuilder withContentDepthPercentDamage(UncertainPairedData contentDepthPercentDamage)
            {
                _occupancyType._contentDepthPercentDamageFunction = contentDepthPercentDamage;
                _occupancyType._computeContentDamage = true;
                return new OccupancyTypeBuilder(_occupancyType);
            }

            public OccupancyTypeBuilder withVehicleDepthPercentDamage(UncertainPairedData vehicleDepthPercentDamage)
            {
                _occupancyType._vehicleDepthPercentDamageFunction = vehicleDepthPercentDamage;
                _occupancyType._computeVehicleDamage = true;
                return new OccupancyTypeBuilder(_occupancyType);
            }

            public OccupancyTypeBuilder withOtherDepthPercentDamage(UncertainPairedData otherDepthPercentDamage)
            {
                _occupancyType._OtherDepthPercentDamageFunction = otherDepthPercentDamage;
                _occupancyType._computeOtherDamage = true;
                return new OccupancyTypeBuilder(_occupancyType);
            }

            public OccupancyTypeBuilder withStructureValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _occupancyType._structureValueError = valueUncertainty;
                return new OccupancyTypeBuilder(_occupancyType);
            }

            public OccupancyTypeBuilder withContentValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _occupancyType._contentValueError = valueUncertainty;
                return new OccupancyTypeBuilder(_occupancyType);
            }
            public OccupancyTypeBuilder withVehicleValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _occupancyType._vehicleValueError = valueUncertainty;
                return new OccupancyTypeBuilder(_occupancyType);
            }
            public OccupancyTypeBuilder withOtherValueUncertainty(ValueUncertainty valueUncertainty)
            {
                _occupancyType._otherValueError = valueUncertainty;
                return new OccupancyTypeBuilder(_occupancyType);
            }
            public OccupancyTypeBuilder withContentToStructureValueRatio(ValueRatioWithUncertainty valueRatioWithUncertainty)
            {
                _occupancyType._contentToStructureValueRatio = valueRatioWithUncertainty;
                _occupancyType._useContentToStructureValueRatio = true;
                return new OccupancyTypeBuilder(_occupancyType);
            }
            public OccupancyTypeBuilder withOtherToStructureValueRatio(ValueRatioWithUncertainty valueRatioWithUncertainty)
            {
                _occupancyType._otherToStructureValueRatio = valueRatioWithUncertainty;
                _occupancyType._useOtherToStructureValueRatio = true;
                return new OccupancyTypeBuilder(_occupancyType);
            }
            public OccupancyTypeBuilder withFirstFloorElevationUncertainty(FirstFloorElevationUncertainty firstFloorElevationUncertainty)
            {
                _occupancyType._firstFloorElevationError = firstFloorElevationUncertainty;
                return new OccupancyTypeBuilder(_occupancyType);
            }
        }
    }
}