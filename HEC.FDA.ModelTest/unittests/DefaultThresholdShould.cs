using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using Statistics;
using Statistics.Distributions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace HEC.FDA.ModelTest.unittests
{
    [Trait("RunsOn", "Remote")]
    public class DefaultThresholdShould
    {
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 15, 30 };
        static string damCat = "residential";
        static string assetCat = "Structure";
        static int id = 1;

        private static CurveMetaData MakeMetaData(string damageCategory = "residential", string assetCategory = "Structure")
        {
            return new CurveMetaData("x", "y", "name", damageCategory, assetCategory);
        }

        private static UncertainPairedData MakeFlowStage()
        {
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 30 * i, 10);
            }
            return new UncertainPairedData(Flows, stages, MakeMetaData());
        }

        private static UncertainPairedData MakeStageDamage(string damageCategory = "residential", string assetCategory = "Structure")
        {
            IDistribution[] damages = new IDistribution[3]
            {
                new Uniform(0, 0, 10),
                new Uniform(0, 600000, 10),
                new Uniform(0, 600000, 10)
            };
            return new UncertainPairedData(Stages, damages, MakeMetaData(damageCategory, assetCategory));
        }

        /// <summary>
        /// Regression test: adding a second identical damage category should not change the default threshold stage.
        /// Before the fix, the first damage-frequency curve was double-counted, inflating the threshold.
        /// With N identical categories the threshold damage scales by N, but the stage-damage also scales by N,
        /// so the inverse lookup should return the same stage.
        /// </summary>
        [Fact]
        public void ProduceSameThresholdStageRegardlessOfDamageCategoryCount()
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            UncertainPairedData flow_stage = MakeFlowStage();

            // Single damage category
            List<UncertainPairedData> singleCategory = new List<UncertainPairedData> { MakeStageDamage() };
            ImpactAreaScenarioSimulation simSingle = ImpactAreaScenarioSimulation.Builder(id)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithStageDamages(singleCategory)
                .Build();
            ImpactAreaScenarioResults resultsSingle = simSingle.Compute(convergenceCriteria, new CancellationToken(), computeIsDeterministic: true);

            // Two damage categories (different category names so they're treated separately)
            List<UncertainPairedData> twoCategories = new List<UncertainPairedData>
            {
                MakeStageDamage("residential", "Structure"),
                MakeStageDamage("commercial", "Structure")
            };
            ImpactAreaScenarioSimulation simTwo = ImpactAreaScenarioSimulation.Builder(id)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithStageDamages(twoCategories)
                .Build();
            ImpactAreaScenarioResults resultsTwo = simTwo.Compute(convergenceCriteria, new CancellationToken(), computeIsDeterministic: true);

            Threshold defaultSingle = resultsSingle.PerformanceByThresholds.ListOfThresholds
                .First(t => t.ThresholdID == 0);
            Threshold defaultTwo = resultsTwo.PerformanceByThresholds.ListOfThresholds
                .First(t => t.ThresholdID == 0);

            // Both should produce the same threshold stage because adding identical categories
            // scales both numerator (frequency-damage) and denominator (stage-damage) equally.
            Assert.Equal(defaultSingle.ThresholdValue, defaultTwo.ThresholdValue, precision: 2);
        }

        /// <summary>
        /// Verifies that a computed default threshold exists, has the correct type, and has a reasonable stage value
        /// (between the min and max stages in the stage-damage function).
        /// </summary>
        [Fact]
        public void BeCreatedAutomaticallyWhenNoThresholdProvided()
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            UncertainPairedData flow_stage = MakeFlowStage();
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData> { MakeStageDamage() };

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.Builder(id)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithStageDamages(stageDamageList)
                .Build();

            ImpactAreaScenarioResults results = simulation.Compute(convergenceCriteria, new CancellationToken(), computeIsDeterministic: true);

            Threshold defaultThreshold = results.PerformanceByThresholds.ListOfThresholds
                .FirstOrDefault(t => t.ThresholdID == 0);

            Assert.NotNull(defaultThreshold);
            Assert.Equal(ThresholdEnum.DefaultExteriorStage, defaultThreshold.ThresholdType);
            // The threshold stage should fall within the range of the stage-damage function
            Assert.True(defaultThreshold.ThresholdValue >= Stages[0], "Default threshold stage should be >= minimum stage");
            Assert.True(defaultThreshold.ThresholdValue <= Stages[^1], "Default threshold stage should be <= maximum stage");
        }

        /// <summary>
        /// When a user provides an explicit default threshold (ID=0), the compute should use that
        /// value and not override it with a calculated one.
        /// </summary>
        [Fact]
        public void NotOverrideUserProvidedDefaultThreshold()
        {
            double userStage = 20.0;
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            UncertainPairedData flow_stage = MakeFlowStage();
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData> { MakeStageDamage() };

            Threshold userThreshold = new Threshold(0, convergenceCriteria, ThresholdEnum.AdditionalExteriorStage, userStage);
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.Builder(id)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithStageDamages(stageDamageList)
                .WithAdditionalThreshold(userThreshold)
                .Build();

            ImpactAreaScenarioResults results = simulation.Compute(convergenceCriteria, new CancellationToken(), computeIsDeterministic: true);

            Threshold defaultThreshold = results.PerformanceByThresholds.ListOfThresholds
                .First(t => t.ThresholdID == 0);

            Assert.Equal(userStage, defaultThreshold.ThresholdValue);
        }
    }
}
