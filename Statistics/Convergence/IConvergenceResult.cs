using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    public interface IConvergenceResult
    {
        bool Passed { get; }
        Utilities.IMessage TestMessage { get; }
    }
}
