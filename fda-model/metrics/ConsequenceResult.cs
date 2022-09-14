using System;
using System.Xml.Linq;
namespace metrics
{ //TODO: I THINK SOME OR ALL OF THIS CLASS SHOULD BE INTERNAL 
    public class ConsequenceResult
    {
        #region Fields
        private string _damageCategory;
        private string _assetCategory;
        private int _regionID;
        private double _otherDamage = 0;
        private double _structureDamage = 0;
        private double _contentDamage = 0;
        private double _vehicleDamage = 0;
        private bool _isNull;
        #endregion

        #region Properties
        public string DamageCategory
        {
            get
            {
                return _damageCategory;
            }
        }
        public string AssetCategory
        {
            get
            {
                return _assetCategory;
            }
        }
        public int RegionID
        {
            get
            {
                return _regionID;
            }
        }
        public double OtherDamage
        {
            get { return _otherDamage; }
        }
        public double StructureDamage
        {
            get { return _structureDamage; }
        }
        public double ContentDamage 
        { 
            get { return _contentDamage; } 
        }
        public double VehicleDamage
        {
            get { return _vehicleDamage; }
        }
        public bool IsNull
        {
            get
            {
                return _isNull;
            }
        }
        #endregion

        #region Constructors

        public ConsequenceResult()
        {
            _damageCategory = "unassigned";
            _assetCategory = "unassigned";
            _regionID = 0;
            _isNull = true;
            
        }

        internal ConsequenceResult(string damageCategory, string assetCategory, int impactAreaID)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _regionID = impactAreaID;
            _isNull = false;
        }        
        #endregion

        #region Methods
        internal void IncrementConsequence(double structureDamage, double contentDamage = 0, double vehicleDamage = 0, double otherDamage = 0)
        {
            _structureDamage += structureDamage;
            _contentDamage += contentDamage;
            _otherDamage += otherDamage;
            _vehicleDamage += vehicleDamage;
        }

        internal bool Equals(ConsequenceResult damageResult)
        {
            bool structureDamageMatches = _structureDamage.Equals(damageResult.StructureDamage);
            if (!structureDamageMatches)
            {
                return false;
            }
            bool contentDamageMatches = _contentDamage.Equals(damageResult.ContentDamage);
            if (!contentDamageMatches)
            {
                return false;
            }
            bool otherDamageMatches = _otherDamage.Equals(damageResult.OtherDamage);
            if (!otherDamageMatches)
            {
                return false;
            }
            bool vehicleDamageMatches = _vehicleDamage.Equals(damageResult.VehicleDamage);
            if (!vehicleDamageMatches)
            {
                return false;
            }
            bool damageCategoriesMatch = _damageCategory.Equals(damageResult.DamageCategory);
            if (!damageCategoriesMatch)
            {
                return false;
            }
            bool assetCategoriesMatch = _assetCategory.Equals(damageResult.AssetCategory);
            if (!assetCategoriesMatch)
            {
                return false;
            }
            bool regionIDMatch = _regionID.Equals(damageResult.RegionID);
            if (!regionIDMatch)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
