using System;
using System.Collections.Generic;
using System.Text;

namespace StatisticsTests.ExcelTesting
{
    // The worksheets are not 0 based they start at 1.
    public enum IDistributionTestEnum
    {
        Min = 1,
        Max = 2,
        Mode = 3,
        Mean = 4, 
        Median = 5,
        Variance = 6,
        StandardDevaiation = 7,
        Skewness = 8,
        SampleSize = 9,
        PDF = 10,
        CDF = 11,
        InverseCDF = 12,
        Print = 13,
        Equals = 14,        
        Alpha = 15,
        Beta = 16,
        Kurtosis = 17,
    }
}
