using System;
using System.Collections.Generic;
namespace statistics
{
    public interface IFittable
    {
         IDistributedVariable fit(IList<double> sample);
    }
}