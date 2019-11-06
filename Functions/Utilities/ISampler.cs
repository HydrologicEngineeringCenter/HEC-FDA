using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Utilities
{
    public interface ISampler
    {
        IFunction Sample(ICoordinatesFunction<IOrdinate, IOrdinate> coordinatesFunction);
        bool CanSample(ICoordinatesFunction<IOrdinate, IOrdinate> coordinatesFunction);
    }
}
