using System;
namespace statistics
{
    public interface IBootstrap: IDistributedVariable, IFittable
    {
        IBootstrap Bootstrap(Int64 seed);
        paireddata.IPairedData Bootstrap_to_PairedData(Int64 seed, Int64 ordinates);
    }
}