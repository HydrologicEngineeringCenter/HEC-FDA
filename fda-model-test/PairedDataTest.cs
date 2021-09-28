using NUnit.Framework;
using paireddata;
using System.Collections.Generic;

namespace fda_model_test
{
    public class PairedDataTest
    {
        static List<double> Flows = new List<double> { 0, 2000, 5000, 10000, 25000, 50000 };
        static List<double> Stages = new List<double> { 556, 562, 565, 566, 570, 575 };
        PairedData myRatingCurve = new PairedData(Flows, Stages);

        static List<double> Probabilities = new List<double> { .5, .2, .1, .05, .02, .01 };
        PairedData myFlowFrequencyCurve = new PairedData(Probabilities, Flows);


        [Test]
        public void GetXvals()
        {
            Assert.AreEqual(Flows, myRatingCurve.Xvals);
        }
        [Test]
        public void GetYvals()
        {
            Assert.AreEqual(Stages, myRatingCurve.Yvals);
        }
        [Test]
        public void FInterpolatesCorrect()
        {
            double expected = 565.5; //linear interp of ys when x=7500
            double actual = myRatingCurve.f(7500);
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