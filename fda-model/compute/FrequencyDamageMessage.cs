using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using paireddata;

namespace compute
{
    public class FrequencyDamageMessage : HEC.MVVMFramework.Base.Interfaces.IMessage
    {
    private PairedData _frequencyDamage;
        private string _damageCategory;
        private string _assetCategory;
    
    public string Message
        {
            get
            {
                return "This is the damage-frequency function for damage and asset categories" + _damageCategory + " and " + _assetCategory;
            }
        }
    public PairedData FrequencyDamage
        {
            get
            {
                return _frequencyDamage;
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

