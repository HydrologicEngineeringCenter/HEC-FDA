using Xunit;


namespace StatisticsTests.Distributions
{
    public class DeterministicTests
    {
        [Theory]
        [InlineData(.25, 1, 1)]
        [InlineData(.5, 1.2, 1.2)]
        [InlineData(.75, -1, -1)]
        public void Deterministic_InvCDF(double p, double x, double expected)
        {
            var dist = new Statistics.Distributions.Deterministic(x);
            double actual = dist.InverseCDF(p);
            Assert.Equal(expected, actual);

        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(.5, 1.2, 1)]
        [InlineData(.75, .5, 0)]
        public void Deterministic_CDF(double x, double val, double expected)
        {
            var dist = new Statistics.Distributions.Deterministic(x);
            double actual = dist.CDF(val);
            Assert.Equal(expected, actual);

        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(.5, 1.2, 0)]
        [InlineData(.75, .5, 0)]
        public void Deterministic_PDF(double x, double val, double expected)
        {
            var dist = new Statistics.Distributions.Deterministic(x);
            double actual = dist.PDF(val);
            Assert.Equal(expected, actual);

        }
    }
}