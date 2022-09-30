using Xunit;
using paireddata;
namespace fda_model_test.unittests
{
    [Trait("Category", "Unit")]
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
        [InlineData(7,3.5)]
        [InlineData(3.5, 1.75)]
        public void ReturnYGivenX(double expected, double sample)
        {
            //Samples below should give min, above should give max
            double actual = pairedMultiplyByTwo.f(sample);
            Assert.Equal(expected, actual);
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
            PairedData PairedByOnesAndTens = new PairedData(countByTens,countByOnes);

            IPairedData expected = PairedByOnesAndTens;
            IPairedData actual = pairedCountbyOnes.compose(PairedByOnesAndTens);
            Assert.Equal(expected.Xvals,actual.Xvals);
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

        [Fact]
        public void Multiply()
        {
            double[] multiplierXs = { 2, 3, 4 };
            double[] multiplierYs = { .5, .5, .5 };
            double[] expectedXs = { 1, 1.999, 2, 3, 4, 4.001, 5 };
            double[] expectedYs = {0, 0, 1, 1.5, 2, 4.001, 5};
            PairedData multiplier = new PairedData(multiplierXs, multiplierYs);

            PairedData expected = new PairedData(expectedXs, expectedYs);
            PairedData actual = (PairedData)pairedCountbyOnes.multiply(multiplier); 
            Assert.Equal(expected.Xvals, actual.Xvals);
            Assert.Equal(expected.Yvals, actual.Yvals);
        }

    }
}