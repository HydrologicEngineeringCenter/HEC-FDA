using Xunit;
using HEC.FDA.Model.paireddata;

namespace HEC.FDA.ModelTest.unittests
{
    [Trait("RunsOn", "Remote")]
    public class PairedDataShould
    {
        static double[] countByOnes = { 1, 2, 3, 4, 5 };
        static double[] countByTwos = { 2, 4, 6, 8, 10 };
        PairedData pairedCountbyOnes = new PairedData(countByOnes, countByOnes);
        PairedData pairedMultiplyByTwo = new PairedData(countByOnes, countByTwos);

        [Theory]
        [InlineData(3, 1.5)]
        [InlineData(10, 6)]
        [InlineData(2, 0)]
        [InlineData(8, 4)]
        [InlineData(7, 3.5)]
        [InlineData(3.5, 1.75)]
        public void ReturnYGivenX(double expected, double sample)
        {
            //Samples below should give min, above should give max
            double actual = pairedMultiplyByTwo.f(sample);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(new double[] { -0.5, 0, 0.5, 1, 1.5, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, new double[] { 0, 2.944486373, 17.19557086, 18.44500185, 18.92034484, 26.9, 27.08615612, 41.01200721, 41.19949496, 41.29949496, 41.39949496, 44.03929351, 65.05021368, 71.41458826, 76.54765878, 78.29899332, 81.06318814, 81.19416183, 81.32520567 }, 0.950639128685, 18.32)]
        [InlineData(new double[] { -1.1, -1, -0.5, 0, 0.5, 1, 1.5, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new double[] { 0, 0.55192593, 0.651668523, 1.394744112, 7.678889007, 10.44977477, 13.97495583, 17.84208464, 22.68345023, 28.63414981, 32.20048078, 35.96039015, 39.6759527, 43.77432355, 46.39626234, 47.87619471 }, 0.458824670000012, 7.16)]
        public void ReturnYGivenXExtensiveCases(double[] xvals, double[] yvals, double xGiven, double yExpected)
        {
            PairedData pairedData = new PairedData(xvals, yvals);
            double yActual = pairedData.f(xGiven);
            Assert.Equal(yExpected, yActual, .01);
        }

        [Theory]
        [InlineData(3, 1.5)]
        [InlineData(10, 5)]
        [InlineData(2, 1)]
        [InlineData(8, 4)]
        [InlineData(7, 3.5)]
        [InlineData(3.5, 1.75)]
        public void ReturnXGivenY(double sample, double expected)
        {
            //Samples below should give min, above should give max
            double actual = pairedMultiplyByTwo.f_inverse(sample);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Compose()
        {
            double[] countByTens = { 10, 20, 30, 40, 50 };
            PairedData PairedByOnesAndTens = new PairedData(countByTens, countByOnes);

            IPairedData expected = PairedByOnesAndTens;
            IPairedData actual = pairedCountbyOnes.compose(PairedByOnesAndTens);
            Assert.Equal(expected.Xvals, actual.Xvals);
            Assert.Equal(expected.Yvals, actual.Yvals);
        }

        [Fact]
        public void SumYsForGivenX_Test()
        {
            double[] inputX = new double[6] { 1, 2, 3, 4.5, 5, 6 };
            double[] inputY = new double[6] { 10, 20, 30, 45, 50, 60 };
            PairedData inputPairedData = new PairedData(inputX, inputY);

            double[] subjectX = new double[5] { 2, 3, 4, 5, 6 };
            double[] subjectY = new double[5] { 20, 30, 40, 50, 60 };
            PairedData subjectPairedData = new PairedData(subjectX, subjectY);

            double[] expectedX = new double[6] { 1, 2, 3, 4.5, 5, 6 };
            double[] expectedY = new double[6] { 30, 40, 60, 90, 100, 120 };
            PairedData expected = new PairedData(expectedX, expectedY);

            IPairedData actual = subjectPairedData.SumYsForGivenX(inputPairedData);

            Assert.Equal(expected.Xvals, actual.Xvals);
            Assert.Equal(expected.Yvals, actual.Yvals);
        }

        [Theory]
        [InlineData(new double[] { 0, .5, 1 }, new double[] { 0, 1000, 11000 }, 3250)]
        [InlineData(new double[] { 0, .5 }, new double[] { 0, 1000 }, 750)]
        public void Integrate(double[] probs, double[] vals, double expected)
        {
            //integrate should extrapolate last value out to probability=1 if probabilty space not defined to 1.
            PairedData paired = new PairedData(probs, vals);

            double actual = paired.integrate();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(new double[] { 1, 2, 3, 4, 5 }, new double[] {0.1, 0.2, 0.5, 0.9, 1}, new double[] { 1, 2, 3, 4, 5 }, new double[] {0.1, 0.4, 1.5, 3.6, 5})] //identical x vals 
        [InlineData(new double[] { 2, 3, 4, 5, 6 }, new double[] {0.1, 0.2, 0.3, 0.8, 1}, new double[] { 1, 2, 3, 4, 5, 6 }, new double[] {0.1, 0.2, 0.6, 1.2, 4, 5})] //same quantity xvals but different range
        [InlineData(new double[] { 2, 3, 4 }, new double[] { .5, .5, .5 }, new double[] { 1, 2, 3, 4, 5 }, new double[] { 0.5, 1, 1.5, 2, 2.5 })] //multiplier has less x vals
        [InlineData(new double[] { 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5}, new double[] {.1, .2, .3, .4, .5, .6, .7, .8, .9, .95, 1}, new double[] { 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5 }, new double[] {.1, .2, .45, .8, 1.25, 1.8, 2.45, 3.2, 4.05, 4.75, 5})] //multiplier has more x vals and greater range
        public void Multiply(double[] multiplierXs, double[] multiplierYs, double[] expectedXs, double[] expectedYs)
        {
            PairedData multiplier = new PairedData(multiplierXs, multiplierYs);

            PairedData actual = (PairedData)pairedCountbyOnes.multiply(multiplier);
            for (int i = 0; i < expectedXs.Length; i++)
            {
                double expectedY = expectedYs[i];
                double actualY = actual.f(expectedXs[i]);
                Assert.Equal(expectedY, actualY, 1);
            }

        }

        [Theory]
        [InlineData(new double[] { 0.01, 0.5, 0.99 }, new double[]  { 0.99, 1.5, 1.2 }, false )]
        [InlineData(new double[] { 0.01, 0.5, 0.99 }, new double[] { 0.99, 1.2, 1.5 }, true)]
        public void InvalidArrayIsNotValid(double[] xs, double[] ys, bool expected)
        {
            CurveMetaData curveMetaData = new CurveMetaData(damageCategory: "none");
            PairedData pairedDataToTest = new PairedData(xs, ys, curveMetaData);
            bool isValid = pairedDataToTest.IsValidPerMetadata;
            Assert.Equal(expected, isValid);
        }
    }
}