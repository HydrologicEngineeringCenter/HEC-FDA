using System;

namespace HEC.FDA.Model.compute
{
    public class ComputeCompleteMessage : MVVMFramework.Base.Interfaces.IMessage
    {
        private long _iterations;
        private int _impactAreaID;
        public string Message
        {
            get
            {
                return $"The compute for the impact area with ID {_impactAreaID} has finished after {_iterations} iterations" + Environment.NewLine;
            }
        }
        public long Iterations
        {
            get
            {
                return _iterations;
            }
        }
        public ComputeCompleteMessage(long iterations, int impactAreaID)
        {
            _iterations = iterations;
            _impactAreaID = impactAreaID;
        }

        public override string ToString()
        {
            return  Message;
        }
    }
}
