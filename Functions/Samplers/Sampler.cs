using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    public static class Sampler
    {
        private static readonly List<ISampler> _Samplers = new List<ISampler>();

        public static void RegisterSampler(ISampler sampler)
        {
            _Samplers.Add(sampler);
        }

        public static IFunction Sample(this ICoordinatesFunction coordinatesFunction, double probability) 
        {
            foreach(ISampler sampler in _Samplers)
            {
                if(sampler.CanSample(coordinatesFunction))
                {
                    return sampler.Sample(coordinatesFunction, probability);
                }
            }
            throw new ArgumentException("Could not find a registered Sampler that could sample this coordinates function.");
        }



    }
}
