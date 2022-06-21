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
    public FrequencyDamageMessage(PairedData frequencyDamage)
    {
            _frequencyDamage = frequencyDamage;
    }
}
}

