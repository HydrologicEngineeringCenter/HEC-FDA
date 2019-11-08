using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Utilities
{
    public interface ISampler
    {
        IFunction Sample(ICoordinatesFunction coordinatesFunction, double probability);
        bool CanSample(ICoordinatesFunction coordinatesFunction);
    }
}
