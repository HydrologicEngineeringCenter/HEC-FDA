using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compute
{
    public class ComputeCompleteMessage : HEC.MVVMFramework.Base.Interfaces.IMessage
    {
        private Int64 _iterations;
        private int _impactAreaID;
        public string Message
        {
            get
            {
                return $"The compute for the impact area with ID {_impactAreaID} has finished after {_iterations} iterations" + Environment.NewLine;
            }
        }
        public Int64 Iterations
        {
            get
            {
                return _iterations;
            }
        }
        public ComputeCompleteMessage(Int64 iterations, int impactAreaID)
        {
            _iterations = iterations;
            _impactAreaID = impactAreaID;
        }
    }
}
