using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Validation;

namespace Statistics
{
    internal class SummaryStatistics : IValidate<SummaryStatistics>
    {
        public bool IsValid { get; }
        public IEnumerable<string> Errors { get; }
        public double Mean { get; }
        public double Variance { get; }
        public double StandardDeviation { get; }
        public double Skewness { get; }
        public double Minimum { get; }
        public double Maximum { get; }
        public int SampleSize { get; }

        public SummaryStatistics(IEnumerable<double> sample)
        {
            if (sample.Any())
            {
                var stats = new MathNet.Numerics.Statistics.DescriptiveStatistics(sample);
                Mean = stats.Mean;
                Variance = stats.Variance;
                StandardDeviation = stats.StandardDeviation;
                Skewness = stats.Skewness;
                Minimum = stats.Minimum;
                Maximum = stats.Maximum;
                SampleSize = stats.Count.CastToInt();
                IsValid = Validate(new Validation.SummaryStatisticsValidator(), out IEnumerable<string> errors);
                Errors = errors;
            }
            else new SummaryStatistics();
        }

        public SummaryStatistics()
        {
            Mean = double.NaN;
            Variance = double.NaN;
            StandardDeviation = double.NaN;
            Skewness = double.NaN;
            Minimum = double.NaN;
            Maximum = double.NaN;
            SampleSize = 0;
            IsValid = Validate(new Validation.SummaryStatisticsValidator(), out IEnumerable<string> errors);
            Errors = errors;
        }
        
        public bool Validate(IValidator<SummaryStatistics> validator, out IEnumerable<string> errors)
        {
            return validator.IsValid(this, out errors);
        }
    }
}
