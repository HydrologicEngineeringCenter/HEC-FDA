using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    public interface ISampler
    {
        IFunction Sample(ICoordinatesFunction coordinatesFunction, double probability);
        bool CanSample(ICoordinatesFunction coordinatesFunction);
    }
}
