namespace structures
{
    public class DeterministicOccupancyType
    {
        private string name;
        private string damcat;
        private paireddata.IPairedData _StructureDamageFunction;
        private paireddata.IPairedData _ContentDamageFunction;
        private paireddata.IPairedData _OtherDamageFunction;
        private double _foundationHeightError;

        public DeterministicOccupancyType(paireddata.IPairedData structureDamagePairedData, paireddata.IPairedData contentDamagePairedData, paireddata.IPairedData otherDamagePairedData, double foundationHeightError)
        {
            _StructureDamageFunction = structureDamagePairedData;
            _ContentDamageFunction = contentDamagePairedData;
            _OtherDamageFunction = otherDamagePairedData;
            _foundationHeightError = foundationHeightError;
        }

        //other stuff.
        public paireddata.IPairedData StructureDamageFunction
        {
            get { return _StructureDamageFunction; }
        }

        public string DamCatName { get { return damcat; } }
        public string Name { get { return name; } }
        public int FoundationHeightError { get; internal set; }
    }
}