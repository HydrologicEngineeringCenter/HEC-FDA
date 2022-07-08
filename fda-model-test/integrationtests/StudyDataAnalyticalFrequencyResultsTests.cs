using System;
using System.Collections.Generic;
using Xunit;
using compute;
using paireddata;
using Statistics;
using Statistics.Distributions;

namespace fda_model_test
{
    [Trait("Category","Integration")]
    public class StudyDataAnalyticalFrequencyResultsTests
    {

        /// <summary>
        /// The expected values in this test class were computed using HEC-FDA Version 1.4.3
        /// Let's plan on commenting out the tests below that take a long time to run, but not delete so that we may check from time to time
        /// </summary>
        /// 

        static IDistribution LP3Distribution = IDistributionFactory.FactoryLogPearsonIII(3.537, .438, .075, 125);
        static double[] RatingCurveFlows = { 0, 1500, 2120, 3140, 4210, 5070, 6240, 7050, 9680 };

        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static int impactAreaID = 0;
        static string damCat = "residential";
        static string assetCat = "structure";
        static CurveMetaData metaData = new CurveMetaData(xLabel, yLabel, name, damCat, assetCat);


        static IDistribution[] StageDistributions =
        {
            new Normal(458,0.00001),
            new Normal(468.33,.312),
            new Normal(469.97,.362),
            new Normal(471.95,.422),
            new Normal(473.06,.456),
            new Normal(473.66,.474),
            new Normal(474.53,.5),
            new Normal(475.11,.5),
            new Normal(477.4,.5)
                //note that the rating curve domain lies within the stage-damage domain
        };
        static double[] StageDamageStages = {460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479 };
        static IDistribution[] DamageDistrbutions =
        {
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0.00001),
            new Normal(.04,.16),
            new Normal(.66,1.02),
            new Normal(2.83,2.47),
            new Normal(7.48,3.55),
            new Normal(17.82,7.38),
            new Normal(39.87,12.35),
            new Normal(76.91,13.53),
            new Normal(124.82,13.87),
            new Normal(173.73,13.12),
        };

        static double[] FragilityStages = { 470, 471, 472, 473, 474, 475 };
        static IDistribution[] FragilityProbabilities =
        {
            new Deterministic(0),
            new Deterministic(.1),
            new Deterministic(.2),
            new Deterministic(.5),
            new Deterministic(.75),
            new Deterministic(1)
        };

        /// <summary>
        /// Study data for the below test can be found at https://drive.google.com/file/d/1yruy7Di0yDPHsPf8ciQMHTQ4D7YKbFSR/view?usp=sharing
        /// </summary>
        /// <param name="expected"></param>
        [Theory]
        [InlineData(20.74)]
        public void ComputeMeanEAD_Test(double expected)
        {
            Statistics.ContinuousDistribution flowFrequency = new Statistics.Distributions.LogPearson3(3.537, .438, .075, 125);
            UncertainPairedData flowStage = new UncertainPairedData(RatingCurveFlows, StageDistributions, metaData);
            UncertainPairedData stageDamage = new UncertainPairedData(StageDamageStages, DamageDistrbutions, metaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
            stageDamageList.Add(stageDamage);
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID)
                .withFlowFrequency(flowFrequency)
                .withFlowStage(flowStage)
                .withStageDamages(stageDamageList)
                .build();
            metrics.ImpactAreaScenarioResults results = simulation.PreviewCompute();
            double difference = expected - results.ConsequenceResults.MeanDamage(damCat,assetCat,impactAreaID);
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .016);
        }
        /// <summary>
        /// Study data for the below test can be found at https://drive.google.com/file/d/1yruy7Di0yDPHsPf8ciQMHTQ4D7YKbFSR/view?usp=sharing
        /// </summary>
        /// <param name="iterations"></param>
        /// <param name="seed"></param>
        /// <param name="expected"></param>
        [Theory]
        [InlineData(10000,2345,21.09)]
        public void ComputeMeanEADWithIterations_Test(int iterations, int seed, double expected)
        {
            Statistics.ContinuousDistribution flowFrequency = new Statistics.Distributions.LogPearson3(3.537, .438, .075, 125);
            UncertainPairedData flowStage = new UncertainPairedData(RatingCurveFlows, StageDistributions, metaData);
            UncertainPairedData stageDamage = new UncertainPairedData(StageDamageStages, DamageDistrbutions, metaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
            stageDamageList.Add(stageDamage);
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID)
                .withFlowFrequency(flowFrequency)
                .withFlowStage(flowStage)
                .withStageDamages(stageDamageList)
                .build();

            compute.RandomProvider randomProvider = new RandomProvider(seed);
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 100, maxIterations: iterations);
            metrics.ImpactAreaScenarioResults results = simulation.Compute(randomProvider, cc);
            double difference = expected - results.ConsequenceResults.MeanDamage(damCat,assetCat,impactAreaID);
            double relativeDifference = Math.Abs(difference / expected);
            Assert.True(relativeDifference < .015);
        }
        /// <summary>
        /// Study data for the below test can be found at https://drive.google.com/file/d/1Wci-Kno92kb32sBwg-CeUniyHf7YVzel/view?usp=sharing
        /// </summary>
        /// <param name="iterations"></param>
        /// <param name="seed"></param>
        /// <param name="expectedEAD"></param>
        /// <param name="topOfLeveeElevation"></param>
        /// <param name="meanExpectedAEP"></param>
        [Theory]
        [InlineData(10000, 2345, 19.46, 475, .2487)]
        public void ComputeMeanEADAndPerformanceWithIterationsAndLevee_Test(int iterations, int seed, double expectedEAD, double topOfLeveeElevation, double meanExpectedAEP)
        {
            Statistics.ContinuousDistribution flowFrequency = new Statistics.Distributions.LogPearson3(3.537, .438, .075, 125);
            UncertainPairedData flowStage = new UncertainPairedData(RatingCurveFlows, StageDistributions, metaData);
            UncertainPairedData stageDamage = new UncertainPairedData(StageDamageStages, DamageDistrbutions, metaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
            stageDamageList.Add(stageDamage);

            double epsilon = 0.0001;
            double[] leveestages = new double[] { 0.0d, topOfLeveeElevation - epsilon, topOfLeveeElevation };
            IDistribution[] leveefailprobs = new IDistribution[3];
            for (int i = 0; i < 2; i++)
            {
                leveefailprobs[i] = new Statistics.Distributions.Deterministic(0); //probability at the top must be 1
            }
            leveefailprobs[2] = new Statistics.Distributions.Deterministic(1);
            UncertainPairedData leveeFragilityFunction = new UncertainPairedData(leveestages, leveefailprobs, metaData);

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID)
                .withFlowFrequency(flowFrequency)
                .withFlowStage(flowStage)
                .withStageDamages(stageDamageList)
                .withLevee(leveeFragilityFunction,topOfLeveeElevation)
                .build();
            compute.RandomProvider randomProvider = new RandomProvider(seed);
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1000, maxIterations: iterations);
            metrics.ImpactAreaScenarioResults results = simulation.Compute(randomProvider, cc);

            double differenceEAD = expectedEAD - results.ConsequenceResults.MeanDamage(damCat,assetCat,impactAreaID);
            double relativeDifferenceEAD = Math.Abs(differenceEAD / expectedEAD);
            Assert.True(relativeDifferenceEAD < .02);
            metrics.SystemPerformanceResults systemPerformanceResults = results.PerformanceByThresholds.GetThreshold(0).SystemPerformanceResults;
            double meanActualAEP = systemPerformanceResults.MeanAEP();
            Assert.Equal(meanExpectedAEP, meanActualAEP, 2);
        }
        /// <summary>
        /// study data for the below test can be found at https://drive.google.com/file/d/1_n39h-ZR0I_5CvIJFBBoOCpJ2amqQlHA/view?usp=sharing
        /// </summary>
        /// <param name="iterations"></param>
        /// <param name="seed"></param>
        /// <param name="expectedEAD"></param>
        /// <param name="topOfLeveeElevation"></param>
        /// <param name="meanExpectedAEP"></param>
        [Theory]
        [InlineData(10000, 2345, 20.63, 475, .4225)]
        public void ComputeMeanEADAndPerformanceWithIterationsAndLeveeAndFragility_Test(int iterations, int seed, double expectedEAD, double topOfLeveeElevation, double meanExpectedAEP)
        {
            Statistics.ContinuousDistribution flowFrequency = new Statistics.Distributions.LogPearson3(3.537, .438, .075, 125);
            UncertainPairedData flowStage = new UncertainPairedData(RatingCurveFlows, StageDistributions, metaData);
            UncertainPairedData stageDamage = new UncertainPairedData(StageDamageStages, DamageDistrbutions,metaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
            stageDamageList.Add(stageDamage);
            UncertainPairedData fragilityCurve = new UncertainPairedData(FragilityStages, FragilityProbabilities, xLabel, yLabel, name);
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID)
                .withFlowFrequency(flowFrequency)
                .withFlowStage(flowStage)
                .withStageDamages(stageDamageList)
                .withLevee(fragilityCurve, topOfLeveeElevation)
                .build();
            compute.RandomProvider randomProvider = new RandomProvider(seed);
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 100, maxIterations: iterations);
            metrics.ImpactAreaScenarioResults results = simulation.Compute(randomProvider, cc);

            double differenceEAD = expectedEAD - results.ConsequenceResults.MeanDamage(damCat,assetCat,impactAreaID);
            double relativeDifferenceEAD = Math.Abs(differenceEAD / expectedEAD);
            Assert.True(relativeDifferenceEAD < .01);//try assert.equal with -2
            metrics.SystemPerformanceResults systemPerformanceResults = results.PerformanceByThresholds.GetThreshold(0).SystemPerformanceResults;
            double meanActualAEP = systemPerformanceResults.MeanAEP();
            Assert.Equal(meanExpectedAEP, meanActualAEP, 2);
        }

    }
}
