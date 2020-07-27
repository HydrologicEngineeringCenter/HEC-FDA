using System;
using System.Collections.Generic;
using System.Text;
using Functions;
using Utilities;

namespace Model
{
    /// <summary>
    /// Provides a data interface for keeping track of sampled and constant parameters in complex functions, for instance <see cref="IConditionLocation.Compute()"/>
    /// </summary>
    public interface ISampleRecord
    {
        /// <summary>
        /// <see langword="true"/> if the nonexceedance property represents a randomly sampled value, <see langword="false"/> otherwise.
        /// </summary>
        bool DoSample { get; }
        /// <summary>
        /// The nonexceedance probability to be used in a sample function, for instance <see cref="ModelUtilities.Sample(IFdaFunction, double)"/>.
        /// </summary>
        double Probability { get; }
    }

    internal sealed class SampleRecord : ISampleRecord
    {
        public bool DoSample { get; }
        public double Probability { get; }

        internal SampleRecord()
        {
            DoSample = false;
            Probability = 0.50d; // default value.
        }
        internal SampleRecord(double nonexceedanceProbability)
        {
            if (!nonexceedanceProbability.IsOnRange(0d, 1d)) throw new ArgumentOutOfRangeException();
            DoSample = true;
            Probability = nonexceedanceProbability;
        }
    }
}
