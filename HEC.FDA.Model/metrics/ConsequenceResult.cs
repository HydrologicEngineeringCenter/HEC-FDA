namespace HEC.FDA.Model.metrics
{ //TODO: I THINK SOME OR ALL OF THIS CLASS SHOULD BE INTERNAL 
    public class ConsequenceResult
    {
        #region Fields
        #endregion

        #region Properties
        public string DamageCategory { get; }
        public double OtherDamage { get; private set; } = 0;
        public double StructureDamage { get; private set; } = 0;
        public double ContentDamage { get; private set; } = 0;
        public double VehicleDamage { get; private set; } = 0;
        public bool IsNull { get; }
        #endregion

        #region Constructors

        public ConsequenceResult()
        {
            DamageCategory = "unassigned";
            IsNull = true;

        }

        public ConsequenceResult(string damageCategory)
        {
            DamageCategory = damageCategory;
            IsNull = false;
        }
        #endregion

        #region Methods
        public void IncrementConsequence(double structureDamage, double contentDamage = 0, double vehicleDamage = 0, double otherDamage = 0)
        {
            StructureDamage += structureDamage;
            ContentDamage += contentDamage;
            OtherDamage += otherDamage;
            VehicleDamage += vehicleDamage;
        }

        internal bool Equals(ConsequenceResult damageResult)
        {
            bool structureDamageMatches = StructureDamage.Equals(damageResult.StructureDamage);
            if (!structureDamageMatches)
            {
                return false;
            }
            bool contentDamageMatches = ContentDamage.Equals(damageResult.ContentDamage);
            if (!contentDamageMatches)
            {
                return false;
            }
            bool otherDamageMatches = OtherDamage.Equals(damageResult.OtherDamage);
            if (!otherDamageMatches)
            {
                return false;
            }
            bool vehicleDamageMatches = VehicleDamage.Equals(damageResult.VehicleDamage);
            if (!vehicleDamageMatches)
            {
                return false;
            }
            bool damageCategoriesMatch = DamageCategory.Equals(damageResult.DamageCategory);
            if (!damageCategoriesMatch)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
