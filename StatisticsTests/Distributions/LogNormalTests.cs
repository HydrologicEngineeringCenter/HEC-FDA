using Xunit;

namespace StatisticsTests.Distributions
{
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
            Statistics.Distributions.LogNormal dist = new Statistics.Distributions.LogNormal(mean, sd, n);
            dist.Validate();
            Assert.True(dist.HasErrors);
        }
        [Theory]
        [InlineData(0d, 1d, 1)]
        [InlineData(1d, 2d, 1)]
        public void GoodValidation(double mean, double sd, int n)
        {
            Statistics.Distributions.Normal dist = new Statistics.Distributions.Normal(mean, sd, n);
            dist.Validate();
            Assert.False(dist.HasErrors);
        }
    }
}
