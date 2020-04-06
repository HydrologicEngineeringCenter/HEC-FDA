using Statistics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Utilities;
using Xunit;

namespace StatisticsTests.Distributions
{
    [ExcludeFromCodeCoverage]
    public class TriangularTests
    {
        [Fact]
        public void Triangular_EqualMinMax_ReturnsObjWithMessage()
        {
            IDistribution t = new Statistics.Distributions.Triangular(0, 0, 0);
            Assert.True(t.Messages.Max() == IMessageLevels.Message);
        }
        [Fact]
        public void Triangular_MinGreaterThanMax_Throws_InvalidConstructorArgumentsException()
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => new Statistics.Distributions.Triangular(1, 0, 0));
        }
    }
}
