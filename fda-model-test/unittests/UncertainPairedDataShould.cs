using Xunit;
using System.Collections.Generic;
using System.Xml.Linq;
using System;
using System.Threading.Tasks;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Statistics.Convergence;
using HEC.FDA.Statistics.Histograms;
using HEC.FDA.Statistics.Distributions;

namespace HEC.FDA.ModelTest.unittests
{
    [Trait("Category", "Unit")]
    public class UncertainPairedDataShould
    {
        static double[] countByOnes = { 1, 2, 3, 4, 5 };
        static double[] probabilities = { .9, .8, .7, .6, .5 };
        static double[] increasingprobabilities = { .5, .6, .7, .8, .9 };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static int id = 1;
        static CurveMetaData curveMetaData = new CurveMetaData(xLabel, yLabel, name);

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
            UncertainPairedData upd = new UncertainPairedData(countByOnes, yvals, curveMetaData);
            IPairedData pd = upd.SamplePairedData(probability);
            double actual = pd.Yvals[0] / pd.Xvals[0];
            Assert.Equal(expectedSlope, actual);
        }

        [Theory]
        [InlineData(1.0, 2.0, 2175)]
        [InlineData(1.0, 3.0, 2900)]
        public void ProducePairedDataInSequence_Test(double minSlope, double maxSlope, double expectedCumulativeArea)
        {
            //Samples below should give min, above should give max
            IDistribution[] yvals = new IDistribution[countByOnes.Length];
            for (int i = 0; i < countByOnes.Length; i++)
            {
                yvals[i] = new Uniform(countByOnes[i] * minSlope, countByOnes[i] * maxSlope, 10);
            }
            UncertainPairedData upd = new UncertainPairedData(increasingprobabilities, yvals, curveMetaData);
            int arraySize = 1000;
            double[] arrayOfProbabilities = new double[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                arrayOfProbabilities[i] = (i + 0.5) / arraySize;
            }
            double cumulativeArea = 0;
            for (int i = 0; i < arraySize; i++)
            {

                IPairedData pairedData = upd.SamplePairedData(arrayOfProbabilities[i]);
                double area = pairedData.integrate();
                cumulativeArea += area;
            }
            Assert.Equal(expectedCumulativeArea, cumulativeArea, 0);
        }
        [Theory]
        [InlineData(1.0, 2.0, 2175)]
        [InlineData(1.0, 3.0, 2900)]
        public void ProducePairedDataInParallel_Test(double minSlope, double maxSlope, double expectedCumulativeArea)
        {
            //Samples below should give min, above should give max
            IDistribution[] yvals = new IDistribution[countByOnes.Length];
            for (int i = 0; i < countByOnes.Length; i++)
            {
                yvals[i] = new Uniform(countByOnes[i] * minSlope, countByOnes[i] * maxSlope, 10);
            }
            UncertainPairedData upd = new UncertainPairedData(increasingprobabilities, yvals, curveMetaData);
            int arraySize = 1000;
            double[] arrayOfProbabilities = new double[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                arrayOfProbabilities[i] = (i + 0.5) / arraySize;
            }
            object areaLock = new object();
            double cumulativeArea = 0;
            Parallel.For(0, arraySize, i =>
           {
               IPairedData pairedData = upd.SamplePairedData(arrayOfProbabilities[i]);
               double area = pairedData.integrate();
               lock (areaLock)
               {
                   cumulativeArea += area;
               }

           });
            Assert.Equal(expectedCumulativeArea, cumulativeArea, 0);
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
                yvalsTriangular[i] = new Triangular(countByOnes[i] * minSlope, countByOnes[i] * minSlope * 1.5, countByOnes[i] * maxSlope, 10);
            }

            IDistribution[] yvalsNormal = new IDistribution[countByOnes.Length];
            for (int i = 0; i < countByOnes.Length; i++)
            {
                yvalsNormal[i] = new Normal(countByOnes[i] * minSlope, countByOnes[i] * .5, 10);
            }

            IEnumerable<IDistribution[]> list = new List<IDistribution[]>() { yvalsDeterministic, yvalsNormal, yvalsTriangular, yvalsUniform };

            foreach (IDistribution[] yvals in list)
            {
                UncertainPairedData upd = new UncertainPairedData(countByOnes, yvals, curveMetaData);
                XElement ele = upd.WriteToXML();
                UncertainPairedData upd2 = UncertainPairedData.ReadFromXML(ele);

                double[] minExpected = new double[upd.Yvals.Length];
                double[] minActual = new double[upd2.Yvals.Length];
                double[] maxExpected = new double[upd.Yvals.Length];
                double[] maxActual = new double[upd2.Yvals.Length];

                for (int i = 0; i < upd.Yvals.Length; i++)
                {
                    minExpected[i] = upd.Yvals[i].InverseCDF(0.0);
                    maxExpected[i] = upd.Yvals[i].InverseCDF(1.0);
                }
                for (int i = 0; i < upd2.Yvals.Length; i++)
                {
                    minActual[i] = upd2.Yvals[i].InverseCDF(0.0);
                    maxActual[i] = upd2.Yvals[i].InverseCDF(1.0);
                }

                Assert.Equal(upd.Xvals, upd2.Xvals);
                Assert.Equal(minExpected, minActual);
                Assert.Equal(maxExpected, maxActual);

            }
        }
        [Fact]
        public void HistogramUncertaintyShouldSampleCorrectly()
        {
            //Arrange
            int computePoints = 10;
            Histogram[] histograms = new Histogram[computePoints];
            double[] stages = new double[computePoints];
            double[] expectedMeans = new double[computePoints];
            for (int i = 0; i < computePoints; i++)
            {
                stages[i] = i * 10;
                histograms[i] = FillHistogram(i);
                expectedMeans[i] = i + .5;
            }
            UncertainPairedData uncertainPairedData = new UncertainPairedData(stages, histograms, curveMetaData);
            double medianProbability = 0.5;

            //Act
            IPairedData pairedData = uncertainPairedData.SamplePairedData(medianProbability);
            double tolerance = 0.05;

            //Assert
            for (int i = 0; i < computePoints; i++)
            {
                double mean = pairedData.Yvals[i];
                double expectedMean = expectedMeans[i];
                double difference = Math.Abs(mean - expectedMean);
                double relativeDifference = difference / expectedMean;
                Assert.True(relativeDifference < tolerance);
            }

        }

        private Histogram FillHistogram(int mean)
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
            int seed = 1234;
            int iterations = 1000;
            List<double> data = new List<double>();
            Normal normal = new Normal(mean + .5, mean / 2);
            Random random = new Random(seed);
            for (int i = 0; i < iterations; i++)
            {
                double sampledValue = normal.InverseCDF(random.NextDouble());
                data.Add(sampledValue);
            }
            Histogram histogram = new Histogram(data, convergenceCriteria);
            return histogram;
        }

    }
}