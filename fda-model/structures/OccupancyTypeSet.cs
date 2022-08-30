
using System;
using System.Collections.Generic;

namespace structures
{
    public class OccupancyTypeSet
    {
        private IList<OccupancyType> _occtypes;
        public List<SampledStructureParameters> Sample(int seed)
        {
            Random random = new Random(seed);
            List<SampledStructureParameters> sample = new List<SampledStructureParameters>();
            foreach (OccupancyType oc in _occtypes)
            {
                //TODO 
                //This approach does not work for the FDA algorithm 
                sample.Add(oc.Sample(random.Next()));
            }
            return sample;
        }
    }
}