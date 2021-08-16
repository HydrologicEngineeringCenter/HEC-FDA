using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    /// <summary>
    /// Provides a public interface for internal classes that are used to sample 
    /// </summary>
    public interface ISampler
    {
        IFunction Sample(ICoordinatesFunction coordinatesFunction, double probability);
        bool CanSample(ICoordinatesFunction coordinatesFunction);
    }
}
