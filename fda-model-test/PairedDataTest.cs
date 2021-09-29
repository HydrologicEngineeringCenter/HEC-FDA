using NUnit.Framework;
using paireddata;

namespace fda_model_test
{
    public class PairedDataTest
    {
        static double[] countByOnes = { 1, 2, 3, 4, 5 };
        PairedData pairedCountbyOnes = new PairedData(countByOnes, countByOnes);

        [Test]
        public void FInterpolatesCorrectBetween()
        {
            double expected = 1.5; 
            double actual = pairedCountbyOnes.f(1.5);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void FReturnsMaxIfaAboveBounds()
        {
            double expected = 5;
            double actual = pairedCountbyOnes.f(6);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void FReturnsMinIfBelowBounds()
        {
            double expected = 1;
            double actual = pairedCountbyOnes.f(0);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void FReturnsExactIfExact()
        {
            double expected = 4;
            double actual = pairedCountbyOnes.f(4);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void FInverseInterpolatesCorrectBetween()
        {
            double expected = 1.5;
            double actual = pairedCountbyOnes.f_inverse(1.5);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void FInverseReturnsMaxIfaAboveBounds()
        {
            double expected = 5;
            double actual = pairedCountbyOnes.f_inverse(6);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void FInverseReturnsMinIfBelowBounds()
        {
            double expected = 1;
            double actual = pairedCountbyOnes.f_inverse(0);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void FInverseReturnsExactIfExact()
        {
            double expected = 2;
            double actual = pairedCountbyOnes.f_inverse(2);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ComposeComposes()
        {
            double[] countByTens = { 10, 20, 30, 40, 50 };
            PairedData PairedByOnesAndTens = new PairedData(countByTens,countByOnes);

            IPairedData expected = PairedByOnesAndTens;
            IPairedData actual = pairedCountbyOnes.compose(PairedByOnesAndTens);
            Assert.AreEqual(expected.Xvals,actual.Xvals);
            Assert.AreEqual(expected.Yvals, actual.Yvals);
        }
        [Test]
        public void IntegratesWithEntireProbabilitySpaceDefined()
        {
            double[] probs = { 1,  .5,  0 };
            double[] vals = { 0, 1000, 11000 };
            PairedData paired = new PairedData(probs, vals);

            double expected = 3250;
            double actual = paired.integrate();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void IntegratesWithEntirProbabilitySpaceIncomplete()
        {
            //should extrapolate the last point across the remaining probability space. 
            double[] probs = { 1, .5 };
            double[] vals = { 0, 1000 };
            PairedData paired = new PairedData(probs, vals);

            double expected = 750;
            double actual = paired.integrate();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void MultiplyMultiplies()
        {
            double[] multiplierXs = { 2, 3, 4 };
            double[] multiplierYs = { .5, .5, .5 };
            double[] expectedXs = { 1, 1.999, 2, 3, 4, 4.001, 5 };
            double[] expectedYs = {0, 0, 1, 1.5, 2, 4.001, 5};
            PairedData multiplier = new PairedData(multiplierXs, multiplierYs);

            PairedData expected = new PairedData(expectedXs, expectedYs);
            PairedData actual = (PairedData)pairedCountbyOnes.multiply(multiplier); 
            Assert.AreEqual(expected.Xvals, actual.Xvals);
            Assert.AreEqual(expected.Yvals, actual.Yvals);
        }

    }
}