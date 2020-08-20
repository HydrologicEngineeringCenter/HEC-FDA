using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics.Convergence
{
    internal class ConvergenceResult: IConvergenceResult
    {
        public bool Passed { get; }
        public Utilities.IMessage TestMessage { get; }

        public ConvergenceResult(bool passed, Utilities.IMessage msg)
        {
            Passed = passed;
            TestMessage = msg;
        }
        public ConvergenceResult()
        {
            Passed = false;
            TestMessage = Utilities.IMessageFactory.Factory(Utilities.IMessageLevels.Message, "Not tested.");
        }
    }
}
