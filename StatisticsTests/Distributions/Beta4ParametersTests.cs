using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Xunit;
 

namespace StatisticsTests.Distributions
{
    public class Beta4ParametersTests
    {
        //TODO: Standard
        [Fact]
        public void Beta4Parameter_Alpha1Beta1_ReturnsUniform()
        {
            Random r = new Random(1);
            double[] data = new double[100];
            for (int i = 0; i < 100; i++) data[i] = r.NextDouble();
            var testObj = Statistics.Distributions.Beta4Parameters.Fit(data);
            //throws exception due to bounds 1 and 2 - check this
            Assert.True(false);
        }
    }
}
