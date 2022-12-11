using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEC.FDA.Model.utilities;

namespace HEC.FDA.Model.metrics
{
    public class ProcessedConsequenceResults
    {
        #region Fields 
        private string _assetCategory;
        private string _damageCategory;
        private List<double> _damageRealizations;
        private bool _isNull = false;
        #endregion

        #region Properties 
        public string AssetCategory { get { return _assetCategory; } }
        public string DamageCategory { get { return _damageCategory; } }
        public List<double> DamageRealizations { get { return _damageRealizations; } }
        internal bool IsNull { get { return _isNull; } }
        #endregion

        #region Constructor 
        internal ProcessedConsequenceResults(bool isNull)
        {
            _assetCategory = StringConstants.UNASSIGNED;
            _damageCategory = StringConstants.UNASSIGNED;
            _damageRealizations = new List<double>();
            _isNull = isNull;
        }
        public ProcessedConsequenceResults(double damageValue, string assetCategory, string damageCategory)
        {
            _damageRealizations = new List<double>() { damageValue };
            _assetCategory = assetCategory;
            _damageCategory = damageCategory;
        }
        #endregion

        #region Methods 
        public void AddDamageRealization(double damageValue)
        {
            _damageRealizations.Add(damageValue);
        }
        #endregion
    }
}
