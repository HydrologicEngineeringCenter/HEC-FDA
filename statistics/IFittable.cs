using System;
using System.Collections;
namespace statistics
{
    public interface IFittable
    {
         IDistributedVariable fit(IList<double> sample);
    }
}