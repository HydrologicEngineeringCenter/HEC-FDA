using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Statistics
{
    internal class SampleStatistics : ISampleStatistics
    {
        private readonly MathNet.Numerics.Statistics.DescriptiveStatistics _Statistics;

        public double Mean => _Statistics.Mean;
        public double Median { get; }
        public double Variance => _Statistics.Variance;
        public double StandardDeviation => _Statistics.StandardDeviation;
        public double Skewness => _Statistics.Skewness;
        public double Kurtosis => _Statistics.Kurtosis;
        public Utilities.IRange<double> Range { get; }
        public int SampleSize => Utilities.ExtensionMethods.CastToInt(_Statistics.Count);
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }

        internal SampleStatistics(IData data)
        {
            _Statistics = new MathNet.Numerics.Statistics.DescriptiveStatistics(data.Elements);
            Median = MathNet.Numerics.Statistics.SortedArrayStatistics.Median(data.Elements.ToArray());
            Range = Utilities.IRangeFactory.Factory(_Statistics.Minimum, _Statistics.Maximum);
            State = Validate(new Validation.SummaryStatisticsValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }

        public IMessageLevels Validate(Utilities.IValidator<ISampleStatistics> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
    }
}
