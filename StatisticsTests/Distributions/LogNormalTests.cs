using Xunit;
using Statistics.Distributions;

namespace StatisticsTests.Distributions
{
    [Trait("Category", "Unit")]
    public class LogNormalTests
    {
        [Theory]
        [InlineData(0d, -1d, 1)]
        [InlineData(0d, 1d, 1)]
        [InlineData(-1d, -2d, 1)]
        [InlineData(-1d, 1d, -1)]
        [InlineData(1d, 1d, -1)]
        public void BadValidation(double mean, double sd, int n)
        {
            LogNormal dist = new LogNormal(mean, sd, n);
            dist.Validate();
            Assert.True(dist.HasErrors);
        }
        [Theory]
        [InlineData(0d, 1d, 1)]
        [InlineData(1d, 2d, 1)]
        public void GoodValidation(double mean, double sd, int n)
        {
            Normal dist = new Normal(mean, sd, n);
            dist.Validate();
            Assert.False(dist.HasErrors);
        }


        [Theory]
        [InlineData(-1.5, .25, .974, .3633)]
        [InlineData(-.5,.75,.152,.2802)]
        [InlineData(0.5,1.25,.008,.0812)]
        [InlineData(1.5,1.75,.607,7.208)]
        [InlineData(2.5,2.25,.278,3.231)]
        [InlineData(3.5,2.75,.269,6.098)]
        public void InverseCDF_Test(double mean, double standardDeviation, double probability, double expected)
        {
            LogNormal logNormal = new LogNormal(mean, standardDeviation);
            double actual = logNormal.InverseCDF(probability);
            Assert.Equal(expected, actual, 1);
        }

        [Theory]
        [InlineData(-.5, .75, .895, .349,.698)]
        [InlineData(0.5, 1.25, 1.527,.398,.476)]
        [InlineData(1.5, 1.75, 1.601,.336,.278)]
        [InlineData(2.5, 2.25, 1.799,.278,.198)]
        [InlineData(3.5, 2.75, 2.721,.264,.182)]
        public void CDF_PDF_Test(double mean, double standardDeviation, double unloggedValue, double expectedPDF, double expectedCDF)
        {
            LogNormal logNormal = new LogNormal(mean, standardDeviation);
            double actualPDF = logNormal.PDF(unloggedValue);
            double actualCDF = logNormal.CDF(unloggedValue);
            Assert.Equal(expectedPDF, actualPDF, 2);
            Assert.Equal(expectedCDF, actualCDF, 2);
        }

    }
}
