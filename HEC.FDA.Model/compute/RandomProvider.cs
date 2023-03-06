using System;
using HEC.FDA.Model.interfaces;

namespace HEC.FDA.Model.compute
{
    public class RandomProvider : IProvideRandomNumbers
    {
        private Random _randomNumberGenerator;
        private int _seed;
        public int Seed
        {
            get
            {
                return _seed;
            }
        }
        public RandomProvider()
        {
            _randomNumberGenerator = new Random();
        }
        public RandomProvider(int seed)
        {
            _seed = seed;
            _randomNumberGenerator = new Random(seed);
        }
        public double NextRandom()
        {
            return _randomNumberGenerator.NextDouble();
        }
        public double[] NextRandomSequence(long size)
        {
            double[] randomNumbers = new double[size];//needs to be initialized with a set of random nubmers between 0 and 1;
            for (int i = 0; i < size; i++)
            {
                randomNumbers[i] = _randomNumberGenerator.NextDouble();
            }
            return randomNumbers;

        }
    }
}
