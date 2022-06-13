using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compute
{
    public class ComputeCompleteMessage : HEC.MVVMFramework.Base.Interfaces.IMessage
    {
        private double _iterations;
        public string Message
        {
            get
            {
                return "The compute has finished after " + _iterations + " iterations";
            }
        }
        public double Iterations
        {
            get
            {
                return _iterations;
            }
        }
        public ComputeCompleteMessage(int iterations)
        {
            _iterations = iterations;
        }
    }
}
