using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Utilities
{
    public static class Sampler
    {
        static private List<ISampler> _samplers = new List<ISampler>();

        public static void RegisterSampler(ISampler sampler)
        {
            _samplers.Add(sampler);
        }

        public static IFunction Sample(ICoordinatesFunctionBase coordinatesFunction)
        {
            foreach(ISampler sampler in _samplers)
            {
                if(sampler.CanSample(coordinatesFunction))
                {
                    return sampler.Sample(coordinatesFunction);
                }
            }
            throw new ArgumentException("Could not find a registered Sampler that could sample thid coordinates function.");
        }


    }
}
