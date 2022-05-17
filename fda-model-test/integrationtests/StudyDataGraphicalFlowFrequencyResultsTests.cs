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
    /// The example data in this test is based on Bear Creek Workshop 4 FDA study data
    /// The data is based on PYSR Without, 2024, S Fork Bear, SF-8, Residential
    /// see: https://drive.google.com/file/d/12WJL6ambACQLfqGUwbg7tv_wMxLn-a6t/view?usp=sharing
    /// </summary>
    public class StudyDataGraphicalFlowFrequencyResultsTests
    {
        static int equivalentRecordLength = 48;
        static double[] exceedanceProbabilities = new double[] { .999, .5, .2, .1, .04, .02, .01, .004, .002 };
        static double[] dischargeFrequencyDischarges = new double[] { 900, 1500, 2120, 3140, 4210, 5070, 6240, 7050, 9680 };
        static double[] stageDischargeFunctionDischarges = new double[] { 0, 1500, 2120, 3140, 4210, 5070, 6240, 7050, 9680 };
        static IDistribution[] stageDischargeFunctionStageDistributions = new IDistribution[]
        {
            new Normal(458,0),
            new Normal(468.33, .312),
            new Normal(469.97, .362),
            new Normal(471.95, .422),
            new Normal(473.06, .456),
            new Normal(473.66,.474),
            new Normal(474.53, .5),
            new Normal(475.11, .5),
            new Normal(477.4, .5)
        }; ///observe the large non-overlapping portion of stage-damage vs stage-discharge
        static double[] stageDamageStages = new double[] { 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482 };
        static IDistribution[] stageDamageDamageDistributions = new IDistribution[]
        {
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(.04,.16),
            new Normal(.66,1.02),
            new Normal(2.83,2.47),
            new Normal(7.48,3.55),
            new Normal(17.82,7.38),
            new Normal(39.87,12.35),
            new Normal(76.91,13.53),
            new Normal(124.82,13.87),
            new Normal(173.73,13.12),
            new Normal(218.32,12.03),
            new Normal(257.83,11.1),
            new Normal(292.52,10.31)
        };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string damCat = "residential";
        static string assetCat = "content";
        static int impactAreaID = 0;
        static CurveTypesEnum curveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
        static CurveMetaData curveMetaData = new CurveMetaData(xLabel, yLabel, name, damCat, curveType,assetCat);
        
        [Theory]
        [InlineData(1234, 0.96)]
        public void ComputeMeanEADWithIterations_Test(int seed, double expected)
        {
            GraphicalUncertainPairedData dischargeFrequency = new GraphicalUncertainPairedData(exceedanceProbabilities, dischargeFrequencyDischarges, equivalentRecordLength, curveMetaData, usingStagesNotFlows: false);
            UncertainPairedData stageDischarge = new UncertainPairedData(stageDischargeFunctionDischarges, stageDischargeFunctionStageDistributions, curveMetaData);
            UncertainPairedData stageDamage = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, curveMetaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
            stageDamageList.Add(stageDamage);
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID)
                .withFlowFrequency(dischargeFrequency)
                .withFlowStage(stageDischarge)
                .withStageDamages(stageDamageList)
                .build();
            RandomProvider randomProvider = new RandomProvider(seed);
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
            metrics.ImpactAreaScenarioResults results = simulation.Compute(randomProvider,convergenceCriteria);
            double difference = Math.Abs(expected - results.ConsequenceResults.MeanDamage(damCat,assetCat,impactAreaID));
            double relativeDifference = difference / expected;
            double tolerance = 0.05;
            Assert.True(relativeDifference < tolerance);
        }
        

    }
}
