using HEC.FDA.Model.paireddata;

namespace HEC.FDA.Model.compute
{
    public class FrequencyDamageMessage : MVVMFramework.Base.Interfaces.IMessage
    {
        public string Message { get => $"FrequencyDamage"; }
        public PairedData FrequencyDamage { get;}
        public string DamageCategory {get;}
        public string AssetCategory { get; }
        public FrequencyDamageMessage(PairedData frequencyDamage, string damageCategory, string assetCategory)
        {
            FrequencyDamage = frequencyDamage;
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
        }
    }
}

