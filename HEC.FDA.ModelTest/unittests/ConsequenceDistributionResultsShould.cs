using Xunit;
using System.Collections.Generic;
using Statistics;
using System;
using Statistics.Distributions;
using Statistics.Histograms;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.metrics;
using System.Xml.Linq;

namespace HEC.FDA.ModelTest.unittests
{
    [Trait("RunsOn", "Remote")]
    public class ConsequenceDistributionResultsShould
    {
        [Fact]
        public void ConsequenceDistResultsToUPDShould()
        {
            //Arrange
            string residentialDamageCategory = "Residential";
            string commercialDamageCategory = "Commercial";
            string structureAssetCategory = "Structure";
            string contentAssetCategory = "Content";
            double mean = 2;
            int impactAreaID_1 = 1;           
            StudyAreaConsequencesBinned consequenceDistributionResults = new(false);
            Histogram histogram = FillHistogram(mean);

            //Impact Area 1
            AggregatedConsequencesBinned residentialStructure_1 = new(residentialDamageCategory, structureAssetCategory, histogram, impactAreaID_1);
            consequenceDistributionResults.AddExistingConsequenceResultObject(residentialStructure_1);
            AggregatedConsequencesBinned residentialContent_1 = new(residentialDamageCategory, contentAssetCategory, histogram, impactAreaID_1);
            consequenceDistributionResults.AddExistingConsequenceResultObject(residentialContent_1);
            AggregatedConsequencesBinned commercialStructure_1 = new(commercialDamageCategory, structureAssetCategory, histogram, impactAreaID_1);
            consequenceDistributionResults.AddExistingConsequenceResultObject(commercialStructure_1);
            AggregatedConsequencesBinned commercialContent_1 = new(commercialDamageCategory, contentAssetCategory, histogram, impactAreaID_1);
            consequenceDistributionResults.AddExistingConsequenceResultObject(commercialContent_1);



            List<double> stages = new();
            List<StudyAreaConsequencesBinned> consequenceDistributionResultsList = new();
            for (int i = 0; i < 20; i++)
            {
                stages.Add(i);
                consequenceDistributionResultsList.Add(consequenceDistributionResults);
            }


            //Act
            List<UncertainPairedData> damageUncertainPairedData = StudyAreaConsequencesBinned.ToUncertainPairedData(stages, consequenceDistributionResultsList, impactAreaID_1).Item1;
            int expectedUPDs = 4;
            int expectedUPDLength = 20;
            double expectedMeanAllOver = mean + 0.5;
            double actualMeanFirstUPDMiddleStage = damageUncertainPairedData[0].Yvals[9].InverseCDF(0.5);
            IPairedData pairedData = damageUncertainPairedData[3].SamplePairedData(0.5, true);
            double actualMeanLastUPDLastStage = pairedData.f(19);
            double relativeErrorMeanFirstUPDMiddleStage = Math.Abs(actualMeanFirstUPDMiddleStage - expectedMeanAllOver) / expectedMeanAllOver;
            double relativeErrorMeanLastUPDLastStage = Math.Abs(actualMeanLastUPDLastStage - expectedMeanAllOver) / expectedMeanAllOver;
            double tolerance = 0.05;
            //Assert
            Assert.Equal(expectedUPDs, damageUncertainPairedData.Count);
            Assert.Equal(expectedUPDLength, damageUncertainPairedData[0].Yvals.Length);
            Assert.True(relativeErrorMeanFirstUPDMiddleStage < tolerance);
            Assert.True(relativeErrorMeanLastUPDLastStage < tolerance);
        }



        private static Histogram FillHistogram(double mean)
        {
            ConvergenceCriteria convergenceCriteria = new();
            int seed = 1234;
            int iterations = 1000;
            List<double> data = new();
            Normal normal = new(mean + .5, mean / 2);
            Random random = new(seed);
            for (int i = 0; i < iterations; i++)
            {
                double sampledValue = normal.InverseCDF(random.NextDouble());
                data.Add(sampledValue);
            }
            Histogram histogram = new(data, convergenceCriteria);
            return histogram;
        }

        [Theory]
        [InlineData(1111, 100)]
        public void SerializationShouldReadTheSameObjectItWrites(int seed, int iterations)
        {
            StudyAreaConsequencesBinned expected = new StudyAreaConsequencesBinned();
            List<AggregatedConsequencesBinned> resultList = new();
            List<double> data = new() { 0, 1, 2, 3, 4 };
            expected.AddExistingConsequenceResultObject(new AggregatedConsequencesBinned("DamCat", "AssetCat", new Histogram(data, new ConvergenceCriteria(69, 8008)), 0));
            XElement xElement = expected.WriteToXML();

            StudyAreaConsequencesBinned actual = StudyAreaConsequencesBinned.ReadFromXML(xElement);

            Assert.True(actual.Equals(expected));
        }
    }
}
