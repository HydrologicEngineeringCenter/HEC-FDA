using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    public interface ISummaryStatistics: Utilities.IValidate<ISummaryStatistics>
    {
        double Mean { get; }
        double Median { get; }
        double Variance { get; }
        double StandardDeviation { get; }
        double Skewness { get; }
        Utilities.IRange<double> Range { get; }
        //double Minimum { get; }
        //double Maximum { get; }
        int SampleSize { get; }
    }
}
