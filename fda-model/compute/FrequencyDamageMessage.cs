using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using paireddata;

namespace compute
{
    public class FrequencyDamageMessage : Base.Implementations.Message
{
        private PairedData _frequencyDamage;

    public PairedData FrequencyDamage
        {
            get
            {
                return _frequencyDamage;
            }
        }
    public FrequencyDamageMessage(PairedData frequencyDamage, string damageCategory): base(damageCategory)
    {
            _frequencyDamage = frequencyDamage;
    }
}
}

