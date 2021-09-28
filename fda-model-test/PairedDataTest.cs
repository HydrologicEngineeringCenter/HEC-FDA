using NUnit.Framework;
using paireddata;
using System.Collections.Generic;

namespace fda_model_test
{
    public class PairedDataTest
    {
        static double[] Flows = new double[] { 10, 2000, 5000, 10000, 25000, 50000 };
        static double[] Stages = new double[] { 556, 562, 565, 566, 570, 575 };
        PairedData myRatingCurve = new PairedData(Flows, Stages);

        static double[] Probabilities = new double[] { .5, .2, .1, .05, .02, .01 };
        PairedData myFlowFrequencyCurve = new PairedData(Probabilities, Flows);


        [Test]
        public void FInterpolatesCorrectBetween()
        {
            double expected = 565.5; //linear interp of ys when x=7500
            double actual = myRatingCurve.f(7500);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void FReturnsMaxIfaAboveBounds()
        {
            double expected = 575;
            double actual = myRatingCurve.f(100000);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void FReturnsMinIfBelowBounds()
        {
            double expected = 556;
            double actual = myRatingCurve.f(0);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void FReturnsExactIfExact()
        {
            double expected = 562;
            double actual = myRatingCurve.f(2000);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ComposeComposes()
        {
            IPairedData expected = new PairedData(Probabilities, Stages); //Probability-Stage
            IPairedData actual = myRatingCurve.compose(myFlowFrequencyCurve);
            Assert.AreEqual(expected.Xvals,actual.Xvals);
            Assert.AreEqual(expected.Yvals, actual.Yvals);
            
        }
    }
}