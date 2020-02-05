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
            //Random r = new Random(1);
            //double[] data = new double[100];
            //for (int i = 0; i < 100; i++) data[i] = r.NextDouble();
            //var data = Statistics.IDataFactory.Factory(BoxMuller());
            var testObj = Statistics.Distributions.Beta4Parameters.Fit(BoxMuller());
            //throws exception due to bounds 1 and 2 - check this
            Assert.True(false);
        }
        public double[] BoxMuller()
        {
            Random r = new Random();
            double[] norm = new double[100];
            for (int i = 0; i < norm.Length; i++)
            {
                double u1 = 1d - r.NextDouble(), u2 = 1d - r.NextDouble();
                norm[i] = Math.Sqrt(-2d * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            }
            return norm;
        }
    }
}
