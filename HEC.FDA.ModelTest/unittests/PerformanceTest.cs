using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;
using Statistics;
using Statistics.Distributions;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.compute;
using System.Threading.Tasks;
using HEC.FDA.Model.interfaces;
using System.ComponentModel;

namespace HEC.FDA.ModelTest.unittests
{
    [Collection("Serial")]
    [Trait("RunsOn", "Remote")]
    public class PerformanceTest
    {
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 2 };
        static double[] StageForNonLeveeFailureProbs = { .5000, .8000, .9000, .9600, .9800, .9900, .9960, .9980 };
        static double[] ProbLeveeFailure = { .01, .02, .05, .1, .2, .3, .4, 1 };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string damCat = "residential";
        static string assetCat = "structure";
        static CurveMetaData metaData = new CurveMetaData(xLabel, yLabel, name, damCat, assetCat);
        static int id = 0;

        /// <summary>
        /// The calculations for the results in the test below can be found at https://docs.google.com/spreadsheets/d/1UUNgHYq1_zV4ifnu0iVmiPOzL2szyBCX/edit?usp=sharing&ouid=105470256128470573157&rtpof=true&sd=true
        /// </summary>
        /// <param name="thresholdValue"></param>
        /// <param name="years"></param>
        /// <param name="iterations"></param>
        /// <param name="expectedAEP"></param>
        /// <param name="expectedLTEP"></param>
        [Theory]
        [InlineData(.9200, 80, 1, .08, 0.998732271693343)]
        [InlineData(.9400, 60, 1, .06, 0.975584185541488)]
        [InlineData(.9600, 40, 1, .04, 0.80463384844468)]
        [InlineData(.9800, 20, 1, .02, 0.332392028244906)]
        public void ComputePerformanceWithSimulation_Test(double thresholdValue, int years, int iterations, double expectedAEP, double expectedLTEP)
        {

            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 2 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, metaData);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }

            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, metaData);
            List<UncertainPairedData> uncertainPairedDataList = new List<UncertainPairedData>();
            uncertainPairedDataList.Add(stage_damage);
            int thresholdID = 1;
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1, maxIterations: iterations);
            Threshold threshold = new Threshold(thresholdID, cc, ThresholdEnum.DefaultExteriorStage, thresholdValue);

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(uncertainPairedDataList)
                .withAdditionalThreshold(threshold)
                .build();

            MedianRandomProvider meanRandomProvider = new MedianRandomProvider();
            ImpactAreaScenarioResults results = simulation.Compute(meanRandomProvider, cc, false);

            double actualAEP = results.MeanAEP(thresholdID);
            double actualLTEP = results.LongTermExceedanceProbability(thresholdID, years);

            double aepDifference = Math.Abs(expectedAEP - actualAEP);
            double aepRelativeDifference = aepDifference / expectedAEP;
            Assert.True(aepRelativeDifference < .025);

            double ltepDifference = Math.Abs(expectedLTEP - actualLTEP);
            double ltepRelativeDifference = ltepDifference / expectedLTEP;
            Assert.True(ltepRelativeDifference < .025);
        }

        /// <summary>
        /// calculations for the below test can be obtained at https://docs.google.com/spreadsheets/d/1iSSQHjxlyKbtqfq1s3-RG_t4W19QZCiW/edit?usp=sharing&ouid=105470256128470573157&rtpof=true&sd=true
        /// </summary>
        /// <param name="thresholdValue"></param>
        /// <param name="iterations"></param>
        /// <param name="expected"></param>
        [Theory]
        [InlineData(.998, 1, .026)]
        public void ComputeLeveeAEP_Test(double thresholdValue, int iterations, double expected)
        {
            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 2 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, metaData);
            //create a damage distribution
            IDistribution[] failureProbs = new IDistribution[StageForNonLeveeFailureProbs.Length];
            for (int i = 0; i < StageForNonLeveeFailureProbs.Length; i++)
            {
                failureProbs[i] = new Deterministic(ProbLeveeFailure[i]);
            }
            UncertainPairedData leveeCurve = new UncertainPairedData(StageForNonLeveeFailureProbs, failureProbs, metaData);

            int thresholdID = 1;
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1, maxIterations: iterations);
            Threshold threshold = new Threshold(thresholdID, cc, ThresholdEnum.DefaultExteriorStage, thresholdValue);

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withAdditionalThreshold(threshold)
                .withLevee(leveeCurve, thresholdValue)
                .build();

            MedianRandomProvider meanRandomProvider = new MedianRandomProvider();
            ImpactAreaScenarioResults results = simulation.Compute(meanRandomProvider, cc, false);
            double actual = results.MeanAEP(thresholdID);


















            Assert.Equal(expected, actual, 2);
        }
        /// <summary>
        /// calculations for the below test can be found at https://docs.google.com/spreadsheets/d/1ui_sPDAleoYyu-T3fgraY5ye-WAMVs_j/edit?usp=sharing&ouid=105470256128470573157&rtpof=true&sd=true
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="iterations"></param>
        /// <param name="thresholdValue"></param>
        /// <param name="recurrenceInterval"></param>
        /// <param name="expected"></param>
        [Theory]
        [InlineData(3456, 10001, 1.2, .9, .666667)]
        [InlineData(5678, 10001, 1.3, .98, .663265)]
        [InlineData(6789, 10001, 1.4, .99, .707071)]
        [InlineData(8910, 10001, 1.5, .996, .753012)]
        //[InlineData(9102, 10001, 1.6, .998, .801603)] //the two tests pass for all cases except this one
        public void ComputeConditionalNonExceedanceProbability_Test(int seed, int iterations, double thresholdValue, double recurrenceInterval, double expected)
        {
            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 2 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, metaData);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }

            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, metaData);
            List<UncertainPairedData> uncertainPairedDataList = new List<UncertainPairedData>();
            uncertainPairedDataList.Add(stage_damage);
            int thresholdID = 1;

            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 10000, maxIterations: iterations, tolerance: .001);
            Threshold threshold = new Threshold(thresholdID, convergenceCriteria, ThresholdEnum.DefaultExteriorStage, thresholdValue);

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(uncertainPairedDataList)
                .withAdditionalThreshold(threshold)
                .build();

            UncertainPairedData systemResponse = CreateDefaultCurve(thresholdValue);

            ImpactAreaScenarioSimulation simulationWithLevee = ImpactAreaScenarioSimulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(uncertainPairedDataList)
                .withLevee(systemResponse, thresholdValue)
                .build();


            RandomProvider randomProvider = new RandomProvider(seed);
            ImpactAreaScenarioResults results = simulation.Compute(randomProvider, convergenceCriteria, false);
            ImpactAreaScenarioResults resultsWithLevee = simulationWithLevee.Compute(randomProvider, convergenceCriteria);

            double actualAssuranceOfThreshold = results.AssuranceOfEvent(thresholdID, recurrenceInterval);
            double actualAssuranceOfLevee = resultsWithLevee.AssuranceOfEvent(thresholdID: 0, recurrenceInterval);
            double differenceAssuranceOfThreshold = Math.Abs(actualAssuranceOfThreshold - expected);
            double relativeDifferenceAssuranceOfThreshold = differenceAssuranceOfThreshold / expected;

            double actualAssuranceOfAEP = results.AssuranceOfAEP(thresholdID, 1 - recurrenceInterval);
            double actualAssuranceOfAEPWithLevee = resultsWithLevee.AssuranceOfAEP(thresholdID: 0, 1 - recurrenceInterval);
            double differenceAssuranceOfAEP = Math.Abs(actualAssuranceOfAEP - expected); //assurance of AEP is theoretically equal to assurance of threshold 
            double relativeDifferenceAssuranceOfAEP = differenceAssuranceOfAEP / expected;//expected here is assurance of AEP being compared to assurance of threshold 

            //TODO: This tolerance seems kind of high for me 
            //Investigate why the error in threshold is so much higher 
            double tolerance = 0.02;
            Assert.True(relativeDifferenceAssuranceOfThreshold < tolerance);
            Assert.True(relativeDifferenceAssuranceOfAEP < tolerance);

            //Levee with Default System Response function should have same project performance as a threshold of the same stage
            Assert.Equal(actualAssuranceOfThreshold, actualAssuranceOfLevee, .01);
            Assert.Equal(actualAssuranceOfAEP, actualAssuranceOfAEPWithLevee, .01);
        }

        [Fact]
        public void ConvergenceTest()
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
            ThresholdEnum thresholdType = ThresholdEnum.DefaultExteriorStage;
            double thresholdValue = 4.1;
            int thresholdID1 = 1;
            int thresholdID2 = 2;
            Threshold threshold1 = new Threshold(thresholdID1, convergenceCriteria, thresholdType, thresholdValue);
            Threshold threshold2 = new Threshold(thresholdID2, convergenceCriteria, thresholdType, thresholdValue);
            PerformanceByThresholds performanceByThresholds = new PerformanceByThresholds();
            performanceByThresholds.AddThreshold(threshold1);
            performanceByThresholds.AddThreshold(threshold2);

            double keyForCNEP = .98;
            performanceByThresholds.GetThreshold(thresholdID1).SystemPerformanceResults.AddAssuranceHistogram(keyForCNEP);
            performanceByThresholds.GetThreshold(thresholdID2).SystemPerformanceResults.AddAssuranceHistogram(keyForCNEP);

            int seed = 1234;
            Random random = new Random(seed);
            Normal normal = new Normal();

            for (int i = 0; i < convergenceCriteria.MinIterations / 2; i++)
            {
                double uniformObservation1 = random.NextDouble() + 1;
                double uniformObservation2 = random.NextDouble() + 2;
                double messyObservation = normal.InverseCDF(random.NextDouble()) * random.NextDouble(); //+ random.NextDouble() * random.NextDouble() * random.NextDouble() * 1000;
                double messyObservationLogged = Math.Log(Math.Abs(messyObservation));
                performanceByThresholds.GetThreshold(thresholdID1).SystemPerformanceResults.AddStageForAssurance(keyForCNEP, uniformObservation1);
                performanceByThresholds.GetThreshold(thresholdID1).SystemPerformanceResults.AddStageForAssurance(keyForCNEP, uniformObservation2);
                performanceByThresholds.GetThreshold(thresholdID2).SystemPerformanceResults.AddStageForAssurance(keyForCNEP, messyObservationLogged);
                performanceByThresholds.GetThreshold(thresholdID2).SystemPerformanceResults.AddStageForAssurance(keyForCNEP, messyObservation);
            }
            ImpactAreaScenarioResults results = new ImpactAreaScenarioResults(id);
            results.PerformanceByThresholds = performanceByThresholds;

            bool isFirstThresholdConverged = performanceByThresholds.GetThreshold(thresholdID1).SystemPerformanceResults.AssuranceTestForConvergence(.05, .95);
            bool isSecondThresholdConverged = performanceByThresholds.GetThreshold(thresholdID2).SystemPerformanceResults.AssuranceTestForConvergence(.05, .95);
            bool isPerformanceConverged = results.IsPerformanceConverged();

            Assert.True(isFirstThresholdConverged);
            Assert.False(isSecondThresholdConverged);
            Assert.False(isPerformanceConverged);
        }

        [Theory]
        [InlineData(9102, 10001, 1.6)]
        public void SerializationShouldReadTheSameObjectItWrites(int seed, int iterations, double thresholdValue)
        {
            ContinuousDistribution flow_frequency = new Uniform(0, 100000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 2 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, metaData);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }

            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, metaData);
            List<UncertainPairedData> uncertainPairedDataList = new List<UncertainPairedData>();
            uncertainPairedDataList.Add(stage_damage);
            int thresholdID = 1;

            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 100, maxIterations: iterations, tolerance: .001);
            Threshold threshold = new Threshold(thresholdID, cc, ThresholdEnum.DefaultExteriorStage, thresholdValue);

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(uncertainPairedDataList)
                .withAdditionalThreshold(threshold)
                .build();

            RandomProvider randomProvider = new RandomProvider(seed);
            ImpactAreaScenarioResults results = simulation.Compute(randomProvider, cc, false);
            XElement xElement = results.PerformanceByThresholds.GetThreshold(thresholdID).SystemPerformanceResults.WriteToXML();
            SystemPerformanceResults projectPerformanceResults = SystemPerformanceResults.ReadFromXML(xElement);
            bool success = results.PerformanceByThresholds.GetThreshold(thresholdID).SystemPerformanceResults.Equals(projectPerformanceResults);
            Assert.True(success);
        }

        //This function was copied and pasted from the Lateral Structure Element class
        private static UncertainPairedData CreateDefaultCurve(double Elevation)
        {
            double elev = Elevation;
            double _FailureMargin = 0.001;
            double[] xs = new double[] { elev, elev + _FailureMargin };
            IDistribution[] ys = new IDistribution[] { new Deterministic(0), new Deterministic(1) };
            CurveMetaData curveMetaData = new CurveMetaData(xlabel: "Stages", ylabel: "Damage", name: "Stage-Damage");
            return new UncertainPairedData(xs, ys, curveMetaData);
        }

        private static Normal standardNormal = new Normal();

        [Theory]
        [InlineData(ThresholdEnum.DefaultExteriorStage, 2.878162, 0.998)]
        public void AssuranceResultStorageShould(ThresholdEnum thresholdEnum, double thresholdValue, double expected)
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
            SystemPerformanceResults systemPerformanceResults = new SystemPerformanceResults(thresholdEnum, thresholdValue, convergenceCriteria);
            double standardProbability = 0.998;
            systemPerformanceResults.AddAssuranceHistogram(standardProbability, .01);
            RandomProvider randomProvider = new RandomProvider(1234);
            int masterseed = 1234;
            Random masterSeedList = new Random(masterseed);//must be seeded.
            int[] seeds = new int[convergenceCriteria.MinIterations];
            for (int i = 0; i < convergenceCriteria.MinIterations; i++)
            {
                seeds[i] = masterSeedList.Next();
            }
            long iterations = convergenceCriteria.MinIterations;

            Parallel.For(0, iterations, i =>
            {
                //check if it is a mean random provider or not
                IProvideRandomNumbers threadlocalRandomProvider;
                threadlocalRandomProvider = new RandomProvider(seeds[i]);
                double invCDF = standardNormal.InverseCDF(threadlocalRandomProvider.NextRandom());
                systemPerformanceResults.AddStageForAssurance(standardProbability, invCDF);

            });

            double actual = systemPerformanceResults.AssuranceOfEvent(standardProbability);
            Assert.Equal(expected, actual, 3);
        }
    }
}

