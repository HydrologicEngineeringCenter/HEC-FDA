using System;
using System.Collections.Generic;
using System.Text;
using Functions;

namespace Model
{
    public interface ILateralStructure
    {
        IElevation TopElevation { get; }
        IFdaFunction FailureFunction { get; }
    }
}
