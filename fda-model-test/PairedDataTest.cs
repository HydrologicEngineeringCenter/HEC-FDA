using NUnit.Framework;
using paireddata;

namespace fda_model_test
{
    public class PairedDataTest
    {
        public static double[] xs = { 0, 2000, 5000, 10000, 25000, 50000 };
        private static double[] ys = { 556, 562, 565, 566, 570, 575 };
        PairedData myPairedData = new PairedData(xs, ys);

        [SetUp]
        public void Setup()
        {
 
        }

        [Test]
        public void TestGetXvals()
        { 
            Assert.AreEqual(xs,myPairedData.Xvals);
        }
        [Test]
        public void TestGetYvals()
        {
            Assert.AreEqual(ys, myPairedData.Yvals);
        }
    }
}