using Xunit;
using paireddata;
using System.Collections.Generic;
using Statistics;
using System.Xml.Linq;
using System;
using Statistics.Distributions;
using System.Runtime.Remoting;

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
                yvals[i] = new Uniform(countByOnes[i] * minSlope, countByOnes[i] * maxSlope, 10);
            }
            UncertainPairedData upd = new UncertainPairedData(countByOnes, yvals, xLabel, yLabel, name, description, id);
            IPairedData pd = upd.SamplePairedData(probability);
            double actual = pd.Yvals[0] / pd.Xvals[0];
            Assert.Equal(expectedSlope, actual);
        }
        [Theory]
        [InlineData(1.0, 2.0)]
        [InlineData(1.0, 3.0)]
        public void SerializeAndDeserializeAllDistributions(double minSlope, double maxSlope)
        {
            //Samples below should give min, above should give max
            IDistribution[] yvalsUniform = new IDistribution[countByOnes.Length];
            for (int i = 0; i < countByOnes.Length; i++)
            {
                yvalsUniform[i] = new Uniform(countByOnes[i] * minSlope, countByOnes[i] * maxSlope, 10);
            }

            IDistribution[] yvalsDeterministic = new IDistribution[countByOnes.Length];
            for (int i = 0; i < countByOnes.Length; i++)
            {
                yvalsDeterministic[i] = new Deterministic(countByOnes[i]);
            }

            IDistribution[] yvalsTriangular = new IDistribution[countByOnes.Length];
            for (int i = 0; i < countByOnes.Length; i++)
            {
                yvalsTriangular[i] = new Triangular(countByOnes[i] * minSlope,countByOnes[i] * minSlope * 1.5, countByOnes[i] * maxSlope, 10);
            }

            IDistribution[] yvalsNormal = new IDistribution[countByOnes.Length];
            for (int i = 0; i < countByOnes.Length; i++)
            {
                yvalsNormal[i] = new Normal(countByOnes[i] * minSlope, countByOnes[i] * .5, 10);
            }

            IEnumerable<IDistribution[]> list = new List<IDistribution[]>() { yvalsDeterministic, yvalsNormal, yvalsTriangular, yvalsUniform };

            foreach(IDistribution[] yvals in list)
            {
                UncertainPairedData upd = new UncertainPairedData(countByOnes, yvals, xLabel, yLabel, name, description, id);
                XElement ele = upd.WriteToXML();
                UncertainPairedData upd2 = UncertainPairedData.ReadFromXML(ele);

                double[] minExpected = new double[upd.ys().Length];
                double[] minActual = new double[upd2.ys().Length];
                double[] maxExpected = new double[upd.ys().Length];
                double[] maxActual = new double[upd2.ys().Length];

                for (int i = 0; i < upd.ys().Length; i++)
                {
                    minExpected[i] = upd.ys()[i].InverseCDF(0.0);
                    maxExpected[i] = upd.ys()[i].InverseCDF(1.0);
                }
                for (int i = 0; i < upd2.ys().Length; i++)
                {
                    minActual[i] = upd2.ys()[i].InverseCDF(0.0);
                    maxActual[i] = upd2.ys()[i].InverseCDF(1.0);
                }

                Assert.Equal(upd.xs(), upd2.xs());
                Assert.Equal(minExpected, minActual);
                Assert.Equal(maxExpected, maxActual);

            }
        }
    }
}