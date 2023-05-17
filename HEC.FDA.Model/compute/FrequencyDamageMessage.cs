using HEC.FDA.Model.paireddata;

namespace HEC.FDA.Model.compute
{
    public class FrequencyDamageMessage : MVVMFramework.Base.Interfaces.IMessage
    {
        private PairedData _frequencyDamage;
        private string _damageCategory;
        private string _assetCategory;

        public string Message
        {
            get
            {
                return $"FrequencyDamage";
            }
        }
        public PairedData FrequencyDamage
        {
            get
            {
                return _frequencyDamage;
            }
        }
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
        public FrequencyDamageMessage(PairedData frequencyDamage, string damageCategory, string assetCategory)
        {
            _frequencyDamage = frequencyDamage;
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
        }
    }
}

