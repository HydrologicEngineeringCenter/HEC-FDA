using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model
{
    public class SampleAssetElevation : ISingleSample<IElevation[], double>, ISample<IElevation[], IElevation>
    {
        public int Iterations { get; }
        internal SampleAssetElevation(int iterations = 100)
        {
            if (iterations < 1) throw new InvalidConstructorArgumentsException($"The asset elevation sampler is invalid because less than one iteration was requested.");
        }

        public int InputRandomNumberCount(IElevation[] input)
        {
            int count = 0;
            foreach (var elevation in input) if (!elevation.IsConstant) count++;
            return count;
        }
        public double SingleSample(IElevation[] inputElevations, double[] packet)
        {
            double sum = 0;
            int i = 0, N = packet.Length;
            foreach (var elevation in inputElevations)
            {
                if (elevation.IsConstant) sum += elevation.Height.Value();
                else
                {
                    if (i == N) throw new ArgumentOutOfRangeException($"{N} random numbers were provided to sample the asset elevation but {InputRandomNumberCount(inputElevations)} are required to perform the operation.");
                    else sum += elevation.Height.Value(packet[i]);
                    i++;
                }
            }
            return sum;
        }
        public IElevation Sample(IElevation[] inputs, List<double[]> packets)
        {
            double[] values = new double[packets.Count];
            for (int i  = 0; i < packets.Count; i++)
            {
                values[i] = SampleMethod(inputs, packets[i]);
            }
            return IElevationFactory.Factory(Functions.IOrdinateFactory.Factory(Statistics.IHistogramFactory.Factory(Statistics.IDataFactory.Factory(values), 100)), IElevationEnum.Asset);
        }
    }
}
