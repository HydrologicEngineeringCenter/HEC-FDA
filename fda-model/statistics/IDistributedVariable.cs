using System;
namespace statistics
{
    public interface IDistributedVariable
    {
        double inv_cdf(double probability);
    }
}