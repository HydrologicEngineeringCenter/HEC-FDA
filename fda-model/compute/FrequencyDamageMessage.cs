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
        private int _impactAreaID;
    
    public string Message
        {
            get
            {
                return $"This is the damage-frequency function for the impact area with ID {_impactAreaID}, damage category of {_damageCategory}, and asset category of {_assetCategory}";
            }
        }
    public PairedData FrequencyDamage
        {
            get
            {
                return _frequencyDamage;
            }
        }
    public FrequencyDamageMessage(PairedData frequencyDamage, string damageCategory, string assetCategory, int impactAreaID)
    {
            _frequencyDamage = frequencyDamage;
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _impactAreaID = impactAreaID;
    }
}
}

