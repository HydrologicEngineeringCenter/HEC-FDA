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

        //todo: John, I really didn't want this to be <Constant, IOrdinate>. I wanted <IOrdinate, IOrdinate> but
        //then passing in a function that is <Constant, IOrdinate> wasn't working. I feel like it should but it wasn't
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
