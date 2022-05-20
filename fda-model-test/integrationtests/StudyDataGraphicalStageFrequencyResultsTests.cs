using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using metrics;
using compute;
using paireddata;
using Statistics;
using Statistics.Distributions;


namespace fda_model_test.integrationtests
{
    /// <summary>
    /// The example data in this test is based on River Des Peres study data
    /// The data is based on without project, 2025, Reach 1, Residential
    /// </summary>
    public class StudyDataGraphicalStageFrequencyResultsTests
    {
        static int impactareaid = 1;
        static int equivalentRecordLength = 25;
        static double[] exceedanceProbabilities = new double[] { .5, .2, .1, .04, .02, .01, .005, .002 };
        static double[] stageFrequencyStages = new double[] { 375, 375.5, 376, 377, 379, 380.1, 381, 381.25 };
       
        static double[] stageDamageStages = new double[] { 374, 374.5, 375, 375.5, 376, 376.5, 377, 377.5, 378, 378.5, 379, 379.5, 380, 380.5, 381, 381.5, 382, 382.5, 383, 383.5, 384, 384.5, 385, 385.5, 386, 386.5, 387, 387.5, 388, 388.5, 389, 389.5, 390, 390.5, 391, 391.5};
        static IDistribution[] stageDamageDamageDistributions = new IDistribution[]
        {
            new Normal(0,0),
            new Normal(4.61,2.14),
            new Normal(6.33, 2.52),
            new Normal(8.45, 2.91),
            new Normal(10.95, 3.34),
            new Normal(13.76, 3.8),
            new Normal(16.89, 4.3),
            new Normal(20.29, 4.84),
            new Normal(23.98, 5.43),
            new Normal(27.89, 6.05),
            new Normal(32.01, 6.7),
            new Normal(36.28, 7.38),
            new Normal(40.69, 8.1),
            new Normal(45.21, 8.85),
            new Normal(49.82, 9.61),
            new Normal(54.48, 10.4),
            new Normal(59.18, 11.21),
            new Normal(63.9, 12.03),
            new Normal(68.59, 12.84),
            new Normal(73.21, 13.66),
            new Normal(77.75, 14.46),
            new Normal(82.19, 15.25),
            new Normal(86.5, 16.02),
            new Normal(90.64, 16.76),
            new Normal(94.57, 17.46),
            new Normal(98.3, 18.13),
            new Normal(101.75, 18.75),
            new Normal(104.94, 19.31),
            new Normal(107.8, 19.83),
            new Normal(110.36, 20.29),
            new Normal(112.55, 20.68),
            new Normal(114.36, 21.02),
            new Normal(115.75, 21.27),
            new Normal(116.73, 21.46),
            new Normal(117.3, 21.57),
            new Normal(117.55, 21.62)
        };
        static string assetcategory = "structure";
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string category = "residential";
        static CurveTypesEnum curveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
        static CurveMetaData curveMetaData = new CurveMetaData(xLabel, yLabel, name, category, curveType);
        
        [Theory]
        [InlineData(1234, 8.79)]
        public void ComputeMeanEADWithIterations_Test(int seed, double expected)
        {
            GraphicalUncertainPairedData stageFrequency = new GraphicalUncertainPairedData(exceedanceProbabilities, stageFrequencyStages, equivalentRecordLength, curveMetaData, usingStagesNotFlows: true);
            UncertainPairedData stageDamage = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, curveMetaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
            stageDamageList.Add(stageDamage);
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactareaid)
                .withFrequencyStage(stageFrequency)
                .withStageDamages(stageDamageList)
                .build();
            RandomProvider randomProvider = new RandomProvider(seed);
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
            metrics.ImpactAreaScenarioResults results = simulation.Compute(randomProvider,convergenceCriteria);
            double difference = Math.Abs(expected - results.MeanExpectedAnnualConsequences(impactareaid, category, assetcategory));
            double relativeDifference = difference / expected;
            double tolerance = 0.05;
            Assert.True(relativeDifference < tolerance);
        }
        

    }
}
