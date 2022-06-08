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
        private UncertainPairedData _structureDamageFunction;
        private UncertainPairedData _contentDamageFunction;
        private UncertainPairedData _vehicleDamageFunction;
        private UncertainPairedData _OtherDamageFunction;

        //error distributions - Assuming these are all %s
        private ContinuousDistribution _foundationHeightError;
        private ContinuousDistribution _structureValueError;
        private ContinuousDistribution _contentValueError;
        private ContinuousDistribution _vehicleValueError;
        private ContinuousDistribution _otherValueError;
        #endregion
        #region Constructor
        public OccupancyType(string name, string damcat, UncertainPairedData structureDamageFunction, UncertainPairedData contentDamageFunction, UncertainPairedData vehicleDamageFunction, UncertainPairedData otherDamageFunction, ContinuousDistribution foundationHeightError, ContinuousDistribution structureValueError, ContinuousDistribution contentValueError, ContinuousDistribution vehicleValueError, ContinuousDistribution otherValueError)
        {
            this.name = name;
            this.damcat = damcat;
            _structureDamageFunction = structureDamageFunction;
            _contentDamageFunction = contentDamageFunction;
            _vehicleDamageFunction = vehicleDamageFunction;
            _OtherDamageFunction = otherDamageFunction;
            _foundationHeightError = foundationHeightError;
            _structureValueError = structureValueError;
            _contentValueError = contentValueError;
            _vehicleValueError = vehicleValueError;
            _otherValueError = otherValueError;
        }
        #endregion
        #region Methods
        public DeterministicOccupancyType Sample(int seed)
        {
            Random random = new Random(seed);
            //damage functions
            IPairedData structDamagePairedData = _structureDamageFunction.SamplePairedData(random.NextDouble());
            IPairedData contentDamagePairedData = _contentDamageFunction.SamplePairedData(random.NextDouble());
            IPairedData vehicleDamagePairedData = _vehicleDamageFunction.SamplePairedData(random.NextDouble());
            IPairedData otherDamagePairedData = _OtherDamageFunction.SamplePairedData(random.NextDouble());

            //errors
            double foundationHeightError = _foundationHeightError.InverseCDF(random.NextDouble());
            double structureValueError = _structureValueError.InverseCDF(random.NextDouble());
            double contentValueError = _contentValueError.InverseCDF(random.NextDouble());
            double vehicleValueError = _vehicleValueError.InverseCDF(random.NextDouble());
            double otherValueError = _otherValueError.InverseCDF(random.NextDouble());
            
            return new DeterministicOccupancyType(name, damcat, structDamagePairedData, contentDamagePairedData, vehicleDamagePairedData, otherDamagePairedData, foundationHeightError, structureValueError, contentValueError, vehicleValueError, otherValueError);
        }
        #endregion
    }
}