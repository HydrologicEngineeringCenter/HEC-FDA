﻿using Xunit;
using System.Collections.Generic;
using Statistics;
using System;
using Statistics.Distributions;
using Statistics.Histograms;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.metrics;

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
            int impactAreaID_1 = 1;            ConvergenceCriteria criteria = new ConvergenceCriteria();
            ConsequenceDistributionResults consequenceDistributionResults = new ConsequenceDistributionResults(criteria);
            Histogram histogram = FillHistogram(mean);

            //Impact Area 1
            ConsequenceDistributionResult residentialStructure_1 = new ConsequenceDistributionResult(residentialDamageCategory, structureAssetCategory, histogram, impactAreaID_1);
            consequenceDistributionResults.AddExistingConsequenceResultObject(residentialStructure_1);
            ConsequenceDistributionResult residentialContent_1 = new ConsequenceDistributionResult(residentialDamageCategory, contentAssetCategory, histogram, impactAreaID_1);
            consequenceDistributionResults.AddExistingConsequenceResultObject(residentialContent_1);
            ConsequenceDistributionResult commercialStructure_1 = new ConsequenceDistributionResult(commercialDamageCategory, structureAssetCategory, histogram, impactAreaID_1);
            consequenceDistributionResults.AddExistingConsequenceResultObject(commercialStructure_1);
            ConsequenceDistributionResult commercialContent_1 = new ConsequenceDistributionResult(commercialDamageCategory, contentAssetCategory, histogram, impactAreaID_1);
            consequenceDistributionResults.AddExistingConsequenceResultObject(commercialContent_1);



            List<double> stages = new List<double>();
            List<ConsequenceDistributionResults> consequenceDistributionResultsList = new List<ConsequenceDistributionResults>();
            for (int i = 0; i < 20; i++)
            {
                stages.Add(i);
                consequenceDistributionResultsList.Add(consequenceDistributionResults);
            }


            //Act
            List<UncertainPairedData> uncertainPairedData = ConsequenceDistributionResults.ToUncertainPairedData(stages, consequenceDistributionResultsList, impactAreaID_1);
            int expectedUPDs = 4;
            int expectedUPDLength = 20;
            double expectedMeanAllOver = mean + 0.5;
            double actualMeanFirstUPDMiddleStage = uncertainPairedData[0].Yvals[9].InverseCDF(0.5);
            double actualMeanLastUPDLastStage = uncertainPairedData[3].SamplePairedData(0.5).f(19);
            double relativeErrorMeanFirstUPDMiddleStage = Math.Abs(actualMeanFirstUPDMiddleStage - expectedMeanAllOver) / expectedMeanAllOver;
            double relativeErrorMeanLastUPDLastStage = Math.Abs(actualMeanLastUPDLastStage - expectedMeanAllOver) / expectedMeanAllOver;
            double tolerance = 0.5;
            //Assert
            Assert.Equal(expectedUPDs, uncertainPairedData.Count);
            Assert.Equal(expectedUPDLength, uncertainPairedData[0].Yvals.Length);
            Assert.True(relativeErrorMeanFirstUPDMiddleStage < tolerance);
            Assert.True(relativeErrorMeanLastUPDLastStage < tolerance);
        }



        private Histogram FillHistogram(double mean)
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