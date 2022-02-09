
using Utilities;
using Xunit;

namespace FunctionsTests
{
    public class DistributionFunctionTests
    {
        [Theory]
        [InlineData(1d, 2d, 2d, 100)]
        [InlineData(1d, 3d, 3d, 100)]
        [InlineData(1d, 4d, 4d, 100)]
        [InlineData(1d, 5d, 5d, 100)]
        [InlineData(1d, 2d, -2d, 100)]
        [InlineData(1d, 3d, -3d, 100)]
        [InlineData(1d, 4d, -4d, 100)]
        [InlineData(1d, 5d, -5d, 100)]
        [InlineData(1d, 1d, 1d, 100)]
        [InlineData(2d, 2d, 2d, 100)]
        [InlineData(3d, 3d, 3d, 100)]
        [InlineData(4d, 4d, 4d, 100)]
        [InlineData(5d, 5d, 5d, 100)]
        [InlineData(6d, 6d, 6d, 100)]
        public void GoodDataLPIII_Returns_NoErrorsState(double mean, double sd, double skew, int n)
        {
            var dist = new Statistics.Distributions.LogPearson3(mean, sd, skew, n);
            var testObj = new DistributionFunction(IDistributedOrdinateFactory.Factory(dist));
            Assert.True(testObj.State < IMessageLevels.Error);
        }
    }
}
