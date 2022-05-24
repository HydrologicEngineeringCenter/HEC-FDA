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
        static double[] stageFrequencyStages = new double[] { 370, 373, 378, 381, 384, 385, 386, 387 };
       
        static double[] stageDamageStages = new double[] { 369.5, 370, 370.5, 371, 371.5, 372, 372.5, 373, 373.5, 374, 374.5, 375, 375.5, 376, 376.5, 377, 377.5, 378, 378.5, 379, 379.5, 380, 380.5, 381, 381.5, 382, 382.5, 383, 383.5, 384, 384.5, 385, 385.5, 386, 386.5, 387, 387.5, 388, 388.5, 389, 389.5, 390, 390.5, 391, 391.5, 392, 392.5, 393, 393.5, 394, 394.5, 395 };
        static IDistribution[] stageDamageDamageDistributions = new IDistribution[]
        {
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,.03),
            new Normal(.08,.37),
            new Normal(.41,.84),
            new Normal(.92,1.2),
            new Normal(1.61,1.43),
            new Normal(2.19,1.53),
            new Normal(2.89,1.68),
            new Normal(3.93,1.96),
            new Normal(5.41,2.32),
            new Normal(7.34,2.72),
            new Normal(9.66,3.12),
            new Normal(12.31,3.57),
            new Normal(15.29,4.04),
            new Normal(18.56,4.57),
            new Normal(22.11,5.13),
            new Normal(25.91,5.74),
            new Normal(29.93,6.37),
            new Normal(34.13,7.04),
            new Normal(38.47,7.73),
            new Normal(42.93,8.47),
            new Normal(47.5,9.23),
            new Normal(52.14,10.01),
            new Normal(56.83,10.81),
            new Normal(61.54,11.62),
            new Normal(66.25,12.44),
            new Normal(70.91,13.25),
            new Normal(75.5,14.06),
            new Normal(79.99,14.86),
            new Normal(84.36,15.64),
            new Normal(88.59,16.39),
            new Normal(92.63,17.12),
            new Normal(96.47,17.8),
            new Normal(100.06,18.45),
            new Normal(103.38,19.04),
            new Normal(106.41,19.58),
            new Normal(109.12,20.06),
            new Normal(111.5,20.5),
            new Normal(113.51,20.86),
            new Normal(115.11,21.16),
            new Normal(116.29,21.37),
            new Normal(117.06,21.52),
            new Normal(117.46,21.6),
            new Normal(117.59,21.63),
            new Normal(117.61,21.64),
            new Normal(117.61,21.64),
            new Normal(117.61,21.64),
            new Normal(117.61,21.64),
            new Normal(117.61,21.64)
        };
        static string assetcategory = "structure";
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string category = "residential";
        static CurveTypesEnum curveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
        static CurveMetaData curveMetaData = new CurveMetaData(xLabel, yLabel, name, category, curveType, assetcategory);
        
        [Theory]
        [InlineData(1234, 5.88)]
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
