using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using interfaces;
using Statistics;

namespace compute
{
    public class MedianRandomProvider : IProvideRandomNumbers
    {
        public int Seed
        {
            get
            {
                return 0;
            }
        }
        public double NextRandom()
        {
            return .5;
        }
        

        public double[] NextRandomSequence(Int64 size)
        {
            double[] randomNumbers = new double[size];//needs to be initialized with a set of random nubmers between 0 and 1;
            for (int i = 0; i < size; i++)
            {
                randomNumbers[i] = (((double)i) +.5)/ (double)size;
            }
            return randomNumbers;
        }
    }
}
