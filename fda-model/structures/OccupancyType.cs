using paireddata;
using Statistics;
using System;

namespace structures
{
    public class OccupancyType
    {
        #region Fields
        //fundamental traits
        private string name;
        private string damcat;

        //damage functions
        private UncertainPairedData _structurePercentDamageFunction;
        private UncertainPairedData _contentPercentDamageFunction;
        private UncertainPairedData _vehiclePercentDamageFunction;
        private UncertainPairedData _OtherPercentDamageFunction;

        //error distributions - Assuming these are all %s
        private ContinuousDistribution _foundationHeightError;
        private ContinuousDistribution _structureValueError;
        private ContinuousDistribution _contentValueError;
        private ContinuousDistribution _vehicleValueError;
        private ContinuousDistribution _otherValueError;

        //value ratios
        private ContinuousDistribution _contentToStructureValueRatio;
        private ContinuousDistribution _otherToStructureValueRatio;
        #endregion
        #region Constructor
        public OccupancyType(string name, string damcat, UncertainPairedData structureDamageFunction, UncertainPairedData contentDamageFunction, UncertainPairedData vehicleDamageFunction, UncertainPairedData otherDamageFunction, ContinuousDistribution foundationHeightError, ContinuousDistribution structureValueError, ContinuousDistribution contentValueError, ContinuousDistribution vehicleValueError, ContinuousDistribution otherValueError, ContinuousDistribution contentToStructureValueRatio, ContinuousDistribution otherToStructureValueRatio)
        {
            this.name = name;
            this.damcat = damcat;
            _structurePercentDamageFunction = structureDamageFunction;
            _contentPercentDamageFunction = contentDamageFunction;
            _vehiclePercentDamageFunction = vehicleDamageFunction;
            _OtherPercentDamageFunction = otherDamageFunction;
            _foundationHeightError = foundationHeightError;
            _structureValueError = structureValueError;
            _contentValueError = contentValueError;
            _vehicleValueError = vehicleValueError;
            _otherValueError = otherValueError;
            _contentToStructureValueRatio = contentToStructureValueRatio;
            _otherToStructureValueRatio = otherToStructureValueRatio;
        }
        #endregion
        #region Methods
        public DeterministicOccupancyType Sample(int seed)
        {
            Random random = new Random(seed);
            //damage functions
            IPairedData structDamagePairedData = _structurePercentDamageFunction.SamplePairedData(random.NextDouble());
            IPairedData contentDamagePairedData = _contentPercentDamageFunction.SamplePairedData(random.NextDouble());
            IPairedData vehicleDamagePairedData = _vehiclePercentDamageFunction.SamplePairedData(random.NextDouble());
            IPairedData otherDamagePairedData = _OtherPercentDamageFunction.SamplePairedData(random.NextDouble());

            //errors
            double foundationHeightError = _foundationHeightError.InverseCDF(random.NextDouble());
            double structureValueError = _structureValueError.InverseCDF(random.NextDouble());
            double contentValueError = _contentValueError.InverseCDF(random.NextDouble());
            double vehicleValueError = _vehicleValueError.InverseCDF(random.NextDouble());
            double otherValueError = _otherValueError.InverseCDF(random.NextDouble());

            //ratios
            double contentToStructureValueRatio = _contentToStructureValueRatio.InverseCDF(random.NextDouble());
            double otherToStructureValueRatio = _otherToStructureValueRatio.InverseCDF(random.NextDouble());
            
            return new DeterministicOccupancyType(name, damcat, structDamagePairedData, contentDamagePairedData, vehicleDamagePairedData, otherDamagePairedData, foundationHeightError, structureValueError, contentValueError, vehicleValueError, otherValueError, contentToStructureValueRatio, otherToStructureValueRatio);
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

        }
    }
}