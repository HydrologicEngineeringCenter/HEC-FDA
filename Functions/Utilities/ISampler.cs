using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Utilities
{
    public interface ISampler
    {
        IFunction Sample(ICoordinatesFunctionBase coordinatesFunction);
        bool CanSample(ICoordinatesFunctionBase coordinatesFunction);
    }
}
