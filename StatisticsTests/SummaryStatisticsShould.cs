using Statistics;
using System;
using Xunit;

namespace StatisticsTests
{
    public class SummaryStatisticsShould
    {
        //https://www.itl.nist.gov/div898/strd/univ/data/Mavro.dat
        [Fact]
        public void SampleStatistics_Pass_Mavro_Dataset()
        {
            double[] data = {   2.00180,   2.00170, 2.00180, 2.00190, 2.00180, 2.00170, 2.00150, 2.00140,  2.00150, 2.00150,
                2.00170, 2.00180, 2.00180, 2.00190, 2.00190, 2.00210, 2.00200, 2.00160, 2.00140, 2.00130, 2.00130, 2.00150,
                2.00150, 2.00160, 2.00150, 2.00140, 2.00130, 2.00140, 2.00150, 2.00140, 2.00150, 2.00160, 2.00150, 2.00160,
                2.00190, 2.00200, 2.00200, 2.00210, 2.00220, 2.00230, 2.00240, 2.00250, 2.00270, 2.00260, 2.00260, 2.00260,
                2.00270, 2.00260, 2.00250, 2.00240};
            SampleStatistics s = new SampleStatistics(data);
            Assert.Equal(2.00185600000000, s.Mean, 4);
            Assert.Equal(0.000429123454003053, s.StandardDeviation, 4);
            Assert.True(Math.Abs(0.64492948110838 - s.Skewness) < .03);//from excel
            //Assert.True(Math.Abs(-0.820523796773878 - s.Kurtosis) < .2);//from excel
            Assert.Equal(2.0018, s.Median, 2);

        }
    }
}
