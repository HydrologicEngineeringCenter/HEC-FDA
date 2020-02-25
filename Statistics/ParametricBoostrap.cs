using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics
{
    //internal class ParametricBoostrap: Utilities.ISample<IDistribution, IDistribution>
    //{
    //    private readonly IDistribution _Distribution;

    //    public int Iterations => 1;
    //    public int PacketSize { get; }
        
    //    public ParametricBoostrap(IDistribution input, Random rng)
    //    {
    //        if (input.IsNull()) throw new ArgumentNullException($"The parametric bootstrap sampler could not be constructed because it or the random number generator are null.");
    //        if (!(input.State < IMessageLevels.Error)) throw new ArgumentException($"The parametric bootstrap sampler could not be constructed because the distribution to be sampled is in an error state. It contains the following messages: {input.Messages.PrintTabbedListOfMessages()}");
    //        _Distribution = input;
    //    }
    //    public ParametricBoostrap(IDistribution input, double[] packet)
    //    {
    //        if (input.IsNull()) throw new ArgumentNullException($"The parametric bootstrap sampler could not be constructed because it or the random number generator are null.");
    //        if (!(input.State < IMessageLevels.Error)) throw new ArgumentException($"The parametric bootstrap sampler could not be constructed because the distribution to be sampled is in an error state. It contains the following messages: {input.Messages.PrintTabbedListOfMessages()}");
    //    }
    //}
}
