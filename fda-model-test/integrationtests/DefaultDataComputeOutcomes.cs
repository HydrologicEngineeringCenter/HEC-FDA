using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compute;
using metrics;
using paireddata;
using scenarios;
using Statistics;
using Statistics.Distributions;
using Xunit;

namespace fda_model_test.integrationtests
{
    public class DefaultDataComputeOutcomes
    {
        //this class draws on the default data used in the user interface
        //this data can be found in HEC-FDA/ViewModel/Utilities/DefaultCurveData.cs
        #region StudyData
        //Set up curve meta data and convergence criteria 
        private static string xLabel = "X";
        private static string yLabel = "Y";
        private static string name = "Name";
        private static CurveTypesEnum curveType = CurveTypesEnum.MonotonicallyIncreasing;
        private static CurveMetaData generalCurveMetaData = new CurveMetaData(xLabel, yLabel, name, curveType);
        private static ConvergenceCriteria defaultConvergenceCriteria = new ConvergenceCriteria();
        private static int seed = 1234;
        private static RandomProvider randomProvider = new RandomProvider(seed);
        //set up exterior-interior relationship 
        private static double[] _ExteriorInteriorXValues = new double[] { 474, 474.1, 474.3, 474.5, 478 };
        private static IDistribution[] _ExteriorInteriorYValues = new IDistribution[]
        {
            new Deterministic(472),
            new Deterministic(473),
            new Deterministic(474),
            new Deterministic(474.1),
            new Deterministic(478)
        };
        private static UncertainPairedData interiorExterior = new UncertainPairedData(_ExteriorInteriorXValues, _ExteriorInteriorYValues, generalCurveMetaData);

        //set up graphical flow-frequency relationship - uses LP3POR for ERL
        private static double[] _GraphicalXValues = new double[] { .5, .2, .1, .04, .02, .01, .004, .002 };
        private static double[] _GraphicalYValues = new double[] { 1500, 2120, 3140, 4210, 5070, 6240, 7050, 9680 };
        private static GraphicalUncertainPairedData graphicalFlowFrequency = new GraphicalUncertainPairedData(_GraphicalXValues, _GraphicalYValues, LP3POR, generalCurveMetaData, false);

        //set up graphical stage-frequency relationship - uses LP3POR for ERL 
        private static double[] _GraphicalStageFreqXValues = new double[] { .999, .5, .2, .1, .04, .02, .01, .004, .002 };
        private static double[] _GraphicalStageFreqYValues = new double[] { 458, 468.33, 469.97, 471.95, 473.06, 473.66, 474.53, 475.11, 477.4 };
        private static GraphicalUncertainPairedData graphicalStageFrequency = new GraphicalUncertainPairedData(_GraphicalStageFreqXValues, _GraphicalStageFreqYValues, LP3POR, generalCurveMetaData);

        //set up LP3 dist
        private static double LP3Mean = 3.3;
        private static double LP3StDev = .254;
        private static double LP3Skew = -.1021;
        private static int LP3POR = 48;
        private static ContinuousDistribution lp3 = new LogPearson3(LP3Mean, LP3StDev, LP3Skew, LP3POR);

        //set up regulated-unregulated transform function 
        private static double[] _RegulatedUnregulatedXValues = new double[] { 900, 1500, 2120, 3140, 4210, 5070, 6240, 7050, 9680 };
        private static IDistribution[] _RegulatedUnregulatedYValues = new IDistribution[]
        {
            new Deterministic(900),
            new Deterministic(1500),
            new Deterministic(2000),
            new Deterministic(2010),
            new Deterministic(2020),
            new Deterministic(2050),
            new Deterministic(5500),
            new Deterministic(7050),
            new Deterministic(9680)
        };
        private static UncertainPairedData regulatedUnregulated = new UncertainPairedData(_RegulatedUnregulatedXValues, _RegulatedUnregulatedYValues, generalCurveMetaData);

        //set up a set of stage-damage functions - all functions are the same - just different damage categories and asset categories 
        private static double[] _StageDamageXValues = new double[] { 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482 };
        private static IDistribution[] _StageDamageYValues = new IDistribution[]
        {
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(.04, .16),
            new Normal(.66,1.02),
            new Normal(2.83,2.47),
            new Normal(7.48,3.55),
            new Normal(17.82,7.38),
            new Normal(39.87, 12.35),
            new Normal(76.91, 13.53),
            new Normal(124.82, 13.87),
            new Normal(173.73, 13.12),
            new Normal(218.32, 12.03),
            new Normal(257.83, 11.1),
            new Normal(292.52, 10.31),
            new Normal(370.12,12.3),
            new Normal(480.94,20.45),
            new Normal(890.76,45.67),
            new Normal(1287.45,62.34),
            new Normal(2376.23,134.896),
        };
        private static string residentialDamCat = "residential";
        private static string commercialDamCat = "commercial";
        private static string structureAssetCat = "structure";
        private static string contentAssetCat = "content";
        private static CurveMetaData residentialStructureMeta = new CurveMetaData(xLabel, yLabel, name, residentialDamCat, curveType, structureAssetCat);
        private static CurveMetaData residentialContentMeta = new CurveMetaData(xLabel, yLabel, name, residentialDamCat, curveType, contentAssetCat);
        private static CurveMetaData commercialStructureMeta = new CurveMetaData(xLabel, yLabel, name, commercialDamCat, curveType, structureAssetCat);
        private static CurveMetaData commercialContentMeta = new CurveMetaData(xLabel, yLabel, name, commercialDamCat, curveType, contentAssetCat);
        private static UncertainPairedData residentialStructureDamage = new UncertainPairedData(_StageDamageXValues, _StageDamageYValues, residentialStructureMeta);
        private static UncertainPairedData commercialStructureDamage = new UncertainPairedData(_StageDamageXValues, _StageDamageYValues, commercialStructureMeta);
        private static List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>() { residentialStructureDamage, commercialStructureDamage };

        //set up stage-discharge function 
        private static double[] _StageDischargeXValues = new double[] { 0, 1500, 2120, 3140, 4210, 5070, 6240, 7050, 9680 };
        private static IDistribution[] _StageDischargeYValues = new IDistribution[]
        {
            new Normal(458,0),
            new Normal(468.33,.312),
            new Normal(469.97,.362),
            new Normal(471.95,.422),
            new Normal(473.06,.456),
            new Normal(473.66,.474),
            new Normal(477.53,0.5),
            new Normal(479.11,0.5),
            new Normal(481.44, 0.5),
        };
        private static UncertainPairedData stageDischarge = new UncertainPairedData(_StageDischargeXValues, _StageDischargeYValues, generalCurveMetaData);

        //set up levee
        private static double[] _FailureXValues = new double[] { 458, 468, 470, 471, 472, 472.5, 473, 474 };
        private static IDistribution[] _FailureYValues = new IDistribution[]
        {
            new Deterministic(0),
            new Deterministic(.01),
            new Deterministic(.05),
            new Deterministic(.07),
            new Deterministic(.1),
            new Deterministic(.8),
            new Deterministic(.9),
            new Deterministic(1),
        };
        private static UncertainPairedData systemResponse = new UncertainPairedData(_FailureXValues, _FailureYValues, generalCurveMetaData);
        private static double DefaultLeveeElevation = 476;
        private static double[] defaultFailureStages = new double[] { 458, 475.999, 476 };
        private static double[] defaultFailureProbs = new double[] { 0, 0, 1 };

        private static int impactAreaID = 1;
        private static int year = 2030;
        #endregion

        [Theory]
        [InlineData(.3591, 120.23)]
        public void WithoutAnalytical_ScenarioResults(double expectedMeanAEP, double expectedMeanEAD)
        {
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID)
                .withFlowFrequency(lp3)
                .withFlowStage(stageDischarge)
                .withStageDamages(stageDamageList)
                .build();
            List<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            impactAreaScenarioSimulations.Add(simulation);

            Scenario scenario = new Scenario(year, impactAreaScenarioSimulations);
            ScenarioResults scenarioResults = scenario.Compute(randomProvider, defaultConvergenceCriteria);
            double actualMeanAEP = scenarioResults.MeanAEP(impactAreaID);
            double actualMeanEAD = scenarioResults.MeanExpectedAnnualConsequences(impactAreaID);

            double tolerance = 0.05;
            double AEPRelativeDifference = Math.Abs(actualMeanAEP - expectedMeanAEP) / expectedMeanAEP;
            double EADRelativeDifference = Math.Abs(actualMeanEAD - expectedMeanEAD) / expectedMeanEAD;

            Assert.True(AEPRelativeDifference < tolerance);
            Assert.True(EADRelativeDifference < tolerance);
            
        }
    }
}
