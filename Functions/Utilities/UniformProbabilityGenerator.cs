using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Functions.Utilities
{
    internal sealed class UniformProbabilityGenerator : ISampleGenerator
    {
        private readonly Random _Generator;

        public int N { get; set; }
        public int Seed { get; }

        internal UniformProbabilityGenerator(int seed)
        {
            N = 0;
            Seed = seed;
            _Generator = new Random(Seed);
        }
        internal UniformProbabilityGenerator()
        {
            N = 0;
            Seed = new Random().Next();
            _Generator = new Random(Seed);
        }

        public double[] DrawSample(int n)
        {
            double[] sample = new double[n];
            for (int i = 0; i < n; i++) sample[i] = _Generator.NextDouble();
            return sample;
        }
    }
}
