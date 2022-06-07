namespace structures
{
    public class DeterministicStructure
    {
        private int _fdid;
        private double _foundationHeight;
        private double _StructureValue;
        private double _ContentValue;
        private double _OtherValue;
        private DeterministicOccupancyType _occtype;
        public string DamageCatagory
        {
            get
            {
                return _occtype.DamCatName;
            }
        }


        public DeterministicStructure(int name, double structValueSample, double foundHeightSample)
        {
            _fdid = name;
            _StructureValue = structValueSample;
            _foundationHeight = foundHeightSample;
        }

        public StructureDamageResult ComputeDamage(float depth)
        {
            //TODO: fix structure damage result return
            double depthabovefoundHeight = depth - _foundationHeight;
            double structDamagepercent = _occtype.StructureDamageFunction.f(depthabovefoundHeight);
            double structDamage = structDamagepercent * _StructureValue;

            double contentDamage = 9999;

            double otherDamage = 9999;

            return new StructureDamageResult(structDamage, contentDamage, otherDamage);
        }
    }
}