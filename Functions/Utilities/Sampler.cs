using Functions.Ordinates;
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

        //todo: John, I really didn't want this to be <Constant, IOrdinate>. I wanted <IOrdinate, IOrdinate> but
        //then passing in a function that is <Constant, IOrdinate> wasn't working. I feel like it should but it wasn't
        public static IFunction Sample(ICoordinatesFunction<IOrdinate, IOrdinate> coordinatesFunction) 
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
