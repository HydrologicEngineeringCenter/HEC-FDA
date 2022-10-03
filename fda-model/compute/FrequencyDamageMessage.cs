using HEC.FDA.Model.paireddata;

namespace HEC.FDA.Model.compute
{
    public class FrequencyDamageMessage : MVVMFramework.Base.Interfaces.IMessage
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

