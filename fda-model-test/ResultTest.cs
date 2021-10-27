using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using metrics;

namespace fda_model_test
{
    public class ResultTest
    {
        [Theory]
        [InlineData(20000000, new double[8] { .5, .2, .1, .04, .02, .01, .004, .002 }, new double[8] { .5, .2, .1, .04, .02, .01, .004, .002 })]

        public void AssuranceOfAEP_Test(int n, double[] exceedanceProb, double[] expected)
        {
            Statistics.IDistribution uniform = new Statistics.Distributions.Uniform(0, 1);
            var rand = new Random(1234);
            Results results = new Results();
            double[] aeps = new double[n];

            for (Int64 i = 0; i < n; i++)
            {
                var randProb = rand.NextDouble();
                aeps[i] = uniform.InverseCDF(randProb);
                results.AddAEPEstimate(aeps[i]);
            }
            double[] assuranceOfAEP = new double[8];
            for (int i = 0; i < exceedanceProb.Length; i++)
            {
                assuranceOfAEP[i] = results.AssuranceOfAEP(exceedanceProb[i]);
                double err = Math.Abs((expected[i] - assuranceOfAEP[i]) / expected[i]);
                double errTol = 0.5;
                Assert.True(err < errTol);
            }
        }
    }

}
    
