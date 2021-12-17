using Xunit;
using paireddata;
using System.Collections.Generic;
using Statistics;
using System.Xml.Linq;

namespace fda_model_test
{
    public class UncertainPairedDataShould
    {
        static double[] countByOnes = { 1, 2, 3, 4, 5 };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string description = "description";
        static int id = 1;


        [Theory]
        [InlineData(1.0, 2.0, .5, 1.5)]
        [InlineData(1.0, 3.0, .5, 2)]
        public void ProducePairedData(double minSlope, double maxSlope, double probability, double expectedSlope)
        {
            //Samples below should give min, above should give max
            IDistribution[] yvals = new IDistribution[countByOnes.Length];
            for (int i = 0; i < countByOnes.Length; i++)
            {
                yvals[i] = IDistributionFactory.FactoryUniform(countByOnes[i] * minSlope, countByOnes[i] * maxSlope, 10);
            }
            UncertainPairedData upd = new UncertainPairedData(countByOnes, yvals,xLabel,yLabel,name,description,id);
            IPairedData pd = upd.SamplePairedData(probability);
            double actual = pd.Yvals[0] / pd.Xvals[0];
            Assert.Equal(expectedSlope, actual);
        }
        [Theory]
        [InlineData(1.0, 2.0)]
        [InlineData(1.0, 3.0)]
        public void SerializeAndDeserialize(double minSlope, double maxSlope)
        {
            //Samples below should give min, above should give max
            IDistribution[] yvals = new IDistribution[countByOnes.Length];
            for (int i = 0; i < countByOnes.Length; i++)
            {
                yvals[i] = IDistributionFactory.FactoryUniform(countByOnes[i] * minSlope, countByOnes[i] * maxSlope, 10);
            }
            UncertainPairedData upd = new UncertainPairedData(countByOnes, yvals, xLabel, yLabel, name, description, id);
            XElement ele = upd.WriteToXML();
            UncertainPairedData upd2 = UncertainPairedData.ReadFromXML(ele);

            double[] minExpected = new double[upd.ys().Length];
            double[] minActual = new double[upd2.ys().Length];
            double[] maxExpected = new double[upd.ys().Length];
            double[] maxActual = new double[upd2.ys().Length];

            for (int i = 0; i<upd.ys().Length; i++)
            {
                minExpected[i] = upd.ys()[i].Min;
                maxExpected[i] = upd.ys()[i].Max;
            }
            for (int i=0; i<upd2.ys().Length; i++)
            {
                minActual[i] = upd2.ys()[i].Min;
                maxActual[i] = upd2.ys()[i].Max;
            }

            Assert.Equal(upd.xs(), upd2.xs());
            Assert.Equal(minExpected,minActual);
            Assert.Equal(maxExpected, maxActual);
        }
    }
}