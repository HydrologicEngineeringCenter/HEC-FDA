using System;

namespace structures
{
    public class OccupancyType
    {
        private string name;
        private string damcat;
        private Statistics.ContinuousDistribution _foundationHeightError;
        private paireddata.UncertainPairedData _StructureDamageFunction;
        private paireddata.UncertainPairedData _ContentDamageFunction;
        private paireddata.UncertainPairedData _OtherDamageFunction;
        //other stuff.
        public paireddata.UncertainPairedData StructureDamageFunction
        {
            get { return _StructureDamageFunction; }
        }
        public DeterministicOccupancyType Sample(int seed)
        {
            //sample parts and put them in an occtype
            Random random = new Random(seed);
            paireddata.IPairedData structDamagePairedData = StructureDamageFunction.SamplePairedData(random.NextDouble());
            paireddata.IPairedData contentDamagePairedData = StructureDamageFunction.SamplePairedData(random.NextDouble());
            paireddata.IPairedData otherDamagePairedData = StructureDamageFunction.SamplePairedData(random.NextDouble());
            double foundationHeightError = _foundationHeightError.InverseCDF(random.NextDouble());
            return new DeterministicOccupancyType(structDamagePairedData, contentDamagePairedData, otherDamagePairedData, foundationHeightError);
        }
    }
}