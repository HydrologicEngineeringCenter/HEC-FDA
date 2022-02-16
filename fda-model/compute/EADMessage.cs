using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compute
{
    public class EADMessage : HEC.MVVMFramework.Base.Interfaces.IMessage
    {
        private double _eadEstimate;
        public string Message
        {
            get
            {
                return "EAD was estimated to be " + _eadEstimate;
            }
        }
        public double EADEstimate
        {
            get
            {
                return _eadEstimate;
            }
        }
        public EADMessage(double eadEstimate)
        {
            _eadEstimate = eadEstimate;
        }
    }
}
