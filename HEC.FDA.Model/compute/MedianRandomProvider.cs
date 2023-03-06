using HEC.FDA.Model.interfaces;

namespace HEC.FDA.Model.compute
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


        public double[] NextRandomSequence(long size)
        {
            double[] randomNumbers = new double[size];//needs to be initialized with a set of random nubmers between 0 and 1;
            for (int i = 0; i < size; i++)
            {
                randomNumbers[i] = (i + .5) / size;
            }
            return randomNumbers;
        }
    }
}
