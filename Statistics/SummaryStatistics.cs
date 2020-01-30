using System;
using System.Collections.Generic;
using System.Linq;

using Utilities;

namespace Statistics
{
    internal class SummaryStatistics : ISummaryStatistics
    {
        
        public double Mean { get; }
        public double Median { get; }
        public double Variance { get; }
        public double StandardDeviation { get; }
        public double Skewness { get; }
        public IRange<double> Range { get; }
        public int SampleSize { get; }
        public bool IsValid { get; }
        public IEnumerable<IMessage> Messages { get; }

        public SummaryStatistics(IData data)
        {
            SampleSize = data.SampleSize;
            if (SampleSize > 1)
            {
                Range = data.Range;
                Mean = data.Elements.Sum() / SampleSize;
                double median = 0, deviations2 = 0, deviations3 = 0;
                int i = 0, iMid = data.SampleSize / 2, iMidMod = data.SampleSize % 2;
                foreach (var x in data.Elements)
                {
                    if (i == iMid || i == iMid + 1 && iMidMod > 0) median = x;
                    if (i == iMid + 1 && iMidMod != 0) median = (median + x) / 2;
                    deviations2 += (x - Mean) * (x - Mean);
                    deviations3 += (x - Mean) * (x - Mean) * (x - Mean);
                    // could attempt to outwit compiler by calculating deviation, deviation2 first, or having iterator for median calculations.
                }
                Variance = deviations2 / (double)(SampleSize - 1);
                StandardDeviation = Math.Sqrt(Variance);
                Skewness = (deviations3 / (double)SampleSize) / (StandardDeviation * StandardDeviation * StandardDeviation);
            }
            else
            {
                if (SampleSize == 1)
                {
                    Mean = data.Elements.Sum();
                    Median = Mean;
                    Variance = 0;
                    Skewness = 0;
                    StandardDeviation = 0;
                    IRangeFactory.Factory(Mean, Mean);
                }
                else
                {
                    Mean = double.NaN;
                    Median = double.NaN;
                    Variance = double.NaN;
                    Skewness = double.NaN;
                    StandardDeviation = double.NaN;
                    Range = IRangeFactory.Factory(double.NaN, double.NaN);
                }
            }
            IsValid = Validate(new Validation.SummaryStatisticsValidator(), out IEnumerable<IMessage> msgs);
            Messages = data.Messages.Concat(msgs);
        }       
        public bool Validate(IValidator<ISummaryStatistics> validator, out IEnumerable<IMessage> errors)
        {
            return validator.IsValid(this, out errors);
        }
    }
}
