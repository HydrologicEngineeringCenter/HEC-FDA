using System;
using System.Collections.Generic;
using System.Text;

using Functions;

namespace Model
{
    public interface IElevation
    {
        bool IsConstant { get; }
        IElevationEnum Type { get; }
        IOrdinate Height { get; }
    }
}
