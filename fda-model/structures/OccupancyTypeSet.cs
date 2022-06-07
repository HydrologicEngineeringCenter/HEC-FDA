
using System;
using System.Collections.Generic;

namespace structures
{
    public class OccupancyTypeSet
    {
        private IList<OccupancyType> _occtypes;
        public List<DeterministicOccupancyType> Sample(int seed)
        {
            Random random = new Random(seed);
            List<DeterministicOccupancyType> sample = new List<DeterministicOccupancyType>();
            foreach (OccupancyType oc in _occtypes)
            {
                sample.Add(oc.Sample(random.Next()));
            }
            return sample;
        }
    }
}