using Xunit;
using paireddata;
using System.Collections.Generic;
using Statistics;
namespace fda_model_test
{
    public class UncertainPairedDataShould
    {
        static double[] countByOnes = { 1, 2, 3, 4, 5 };

        [Theory]
        [InlineData(1.0, 2.0, .5, 1.5)]
        [InlineData(1.0, 3.0, .5, 2)]
        public void ProducePairedData(double minSlope, double maxSlope, double probability, double expectedSlope)
        {
            //Samples below should give min, above should give max
            IDistribution[] yvals = new IDistribution[countByOnes.Length]
            for(int i = 0; i < countByOnes.Length; i ++)
            {
                yvals[i] = IDistributionFactory.FactoryUniform(countByOnes[i] * minSlope, countByOnes[i] * maxSlope, 10);
            }
            UncertainPairedData upd = new UncertainPairedData(countByOnes, yvals);
            IPairedData pd upd.Sample(probability);
            double actual = pd.Yvals[0] / pd.Xvals[0];
            Assert.Equal(expected, actual);
        }
    }