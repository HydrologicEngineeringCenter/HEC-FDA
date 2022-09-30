using Xunit;
using Statistics;
using System.Xml.Linq;
namespace StatisticsTests.Convergence
{
    public class ConvergenceCriteriaTests
    {
        [Theory]
        [InlineData(10001, 100, 1.96, .01)]
        public void SerializationShouldReadTheSameObjectItWrites(int maxIterations, int minIterations, double zAlpha, double tolerance)
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations, maxIterations, zAlpha, tolerance);
            XElement xElement = convergenceCriteria.WriteToXML();
            ConvergenceCriteria convergenceCriteriaFromXML = ConvergenceCriteria.ReadFromXML(xElement);
            bool success = convergenceCriteria.Equals(convergenceCriteriaFromXML);
            Assert.True(success);
        }
    }
}
