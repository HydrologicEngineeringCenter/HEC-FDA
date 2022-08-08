using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alternatives;
using compute;
using metrics;
using paireddata;
using scenarios;
using Statistics;
using Statistics.Distributions;
using Statistics.Histograms;
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

        private static CurveMetaData analyticalMetaData = new CurveMetaData("frequency", "flow", "Muncie Analytical Flow Frequeny", curveType);
        private static CurveMetaData regulatedUnregulatedMetaData = new CurveMetaData("unregulated", "regulated", "Muncie Regulated Unregulated Flow Transform Function", curveType);
        private static CurveMetaData graphicalFlowMetaData = new CurveMetaData("frequency", "flow", "Muncie Graphical Flow Frequency", curveType);
        private static CurveMetaData graphicalStageMetaData = new CurveMetaData("frequency", "stage", "Muncie Graphical Stage Frequency", curveType);
        private static CurveMetaData stageDischargeMetaData = new CurveMetaData("discharge", "stage", "Muncie Stage Disharge Function", curveType);
        private static CurveMetaData interiorExteriorMetaData = new CurveMetaData("interior", "exterior", "Muncie Interior Exterior Function", curveType);
        private static CurveMetaData defaultLeveeMetaData = new CurveMetaData("stage", "probability of failure", "Muncie Default Levee No Fragility", curveType);
        private static CurveMetaData failureLeveeMetaData = new CurveMetaData("stage", "probability of failure", "Muncie Levee With Failure", curveType);
        private static CurveMetaData residentialDamageMetaData = new CurveMetaData("stage", "damage", "Muncie Residential Stage Damage", "Residential", "Total");
        private static CurveMetaData commercialDamageMetaData = new CurveMetaData("stage", "damage", "Muncie Commercial Stage Damage", "Commercial", "Total");


        private static ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
        private static int seed = 1234;
        private static RandomProvider randomProvider = new RandomProvider(seed);
        //set up exterior-interior relationship 

        private static double[] _ExteriorInteriorXValues = new double[] { 930, 931, 932, 933, 934, 935, 935.5, 936, 936.5, 937, 937.5, 938 };
        private static IDistribution[] _ExteriorInteriorYValues = new IDistribution[]
        {
            new Deterministic(928),
            new Deterministic(930),
            new Deterministic(930.1),
            new Deterministic(930.2),
            new Deterministic(930.5),
            new Deterministic(930.6),
            new Deterministic(930.8),
            new Deterministic(933),
            new Deterministic(936),
            new Deterministic(936.5),
            new Deterministic(938)
        };
        private static UncertainPairedData interiorExterior = new UncertainPairedData(_ExteriorInteriorXValues, _ExteriorInteriorYValues, interiorExteriorMetaData);

        //set up LP3 dist
        private static double LP3Mean = 3.707;
        private static double LP3StDev = .24;
        private static double LP3Skew = -.475;
        private static int LP3POR = 48;
        private static ContinuousDistribution lp3 = new LogPearson3(LP3Mean, LP3StDev, LP3Skew, LP3POR);

        //set up graphical flow-frequency relationship - uses LP3POR for ERL
        private static double[] _GraphicalXValues = new double[] { 0.99, 0.5, 0.2, 0.1, 0.04, 0.02, 0.01, 0.004, 0.002, 0.0001 };
        private static double[] _GraphicalYValues = new double[] { 1166, 5320, 8175, 9995, 12175, 13706, 15157, 16962, 18278, 23359 };
        private static GraphicalUncertainPairedData graphicalFlowFrequency = new GraphicalUncertainPairedData(_GraphicalXValues, _GraphicalYValues, LP3POR, graphicalFlowMetaData, false);

        //set up graphical stage-frequency relationship - uses LP3POR for ERL 
        private static double[] _GraphicalStageFreqXValues = new double[] { .999, .5, .2, .1, .04, .02, .01, .004, .002 };
        private static double[] _GraphicalStageFreqYValues = new double[] { 929.21, 934.5, 936.5, 937.6, 938.6, 939.7, 940.7, 941.6, 942.1 };

        private static GraphicalUncertainPairedData graphicalStageFrequency = new GraphicalUncertainPairedData(_GraphicalStageFreqXValues, _GraphicalStageFreqYValues, LP3POR, graphicalStageMetaData);
        private static GraphicalUncertainPairedData graphicalStageAsFlowsFrequency = new GraphicalUncertainPairedData(_GraphicalStageFreqXValues, _GraphicalStageFreqYValues, LP3POR, graphicalStageMetaData, false);



        //set up regulated-unregulated transform function 
        private static double[] _RegulatedUnregulatedXValues = new double[] { 400, 8000, 10000, 15000, 16000, 17000, 18000, 19000, 20000, 21000, 22000, 25000, 30000 };

        private static IDistribution[] _RegulatedUnregulatedYValues = MakeARegulatedUnregulated();

        private static IDistribution[] MakeARegulatedUnregulated()
        {
            int[] mostLikelyFlows = new int[] { 400, 7800, 9000, 9040, 9090, 9200, 11000, 18000, 20000, 21000, 22000, 25000, 30000 };
            int[] maxFlows = new int[] { 400, 7950, 9500, 9600, 9800, 10000, 15000, 19000, 20000, 21000, 22000, 25000, 30000 };
            int[] minFlows = new int[] { 400, 7000, 8000, 8500, 8700, 8800, 10000, 12000, 18000, 21000, 22000, 25000, 30000 };
            IDistribution[] triangularDistributionsOfFlows = new IDistribution[mostLikelyFlows.Length];

            for (int i = 0; i < mostLikelyFlows.Length; i++)
            {
                triangularDistributionsOfFlows[i] = new Triangular(minFlows[i], mostLikelyFlows[i], maxFlows[i]);
            }
            return triangularDistributionsOfFlows;
        }

        private static UncertainPairedData regulatedUnregulated = new UncertainPairedData(_RegulatedUnregulatedXValues, _RegulatedUnregulatedYValues, regulatedUnregulatedMetaData);

        //set up a set of stage-damage functions - all functions are the same - just different damage categories and asset categories 
        private static double[] _StageDamageXValues = new double[] { 928.5, 929, 929.5, 930, 930.5, 931, 931.5, 932, 932.5, 933, 933.5, 934, 934.5, 935, 935.5, 936, 936.5, 937, 937.5, 938, 938.5, 939, 939.5, 940, 940.5, 941, 941.5, 942, 942.5, 943, 943.5, 944, 944.5, 945, 945.5, 946, 946.5, 947, 947.5, 948 };
        private static IDistribution[] _ResidentialStageDamageYValues = ResidentialStageDamageYValues();
        private static IDistribution[] _CommercialStageDamageYValues = CommercialStageDamageYValues();

        private static IDistribution[] CommercialStageDamageYValues()
        {
            double[] meanDamage = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 128.360217, 4468.8739, 59469.291, 156145.883, 214156.12, 333123.004, 612710.635, 1176218.11, 2190460.18, 3190704.36, 4131829.42, 5117499.06, 6155497.61, 7207650.59, 9891031.66, 12025257.4, 14241762.9, 16567845.2, 18816831.2, 20963806.1, 22993727.1, 24959017.8, 26877164.3, 28611110.3, 30053487.1, 31248028.2, 32315917.4 }
            double[] standardDeviationDamage = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 562.434471, 11199.7019, 52526.3163, 64565.0729, 71871.4852, 214203.97, 548700.336, 824951.013, 1009010.59, 1048443.2, 1041513.53, 1062918.93, 1093154.12, 1133075.73, 1254636.09, 1393258.11, 1514087.28, 1615528.87, 1720088.68, 1830377.22, 1948126.3, 2072579.08, 2192471, 2289973.16, 2370007.84, 2425315.78, 2469413.8 }
            IDistribution[] normalDistributions = new IDistribution[meanDamage.Length];
            for (int i = 0; i < meanDamage.Length; i++)
            {
                normalDistributions[i] = new Normal(meanDamage[i], standardDeviationDamage[i]);
            }
            return normalDistributions;
        }

        private static IDistribution[] ResidentialStageDamageYValues()
        {
            double[] meanDamage = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 51.5928738, 8494.40654, 42705.2505, 78843.3841, 181535.627, 282786.202, 461376.05, 657568.887, 940228.124, 1291920.96, 1600444.82, 1927824.54, 2331462.36, 3132727.18, 4634893.77, 13921118, 18969940.3, 22201190.7, 25412263.2, 28414272.2, 31081561.4, 33582446.2, 36144273.3, 38704925.7, 41340625.3, 44264231.4, 47578609.5, 50860755.3 };
            double[] standardDeviationDamage = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 261.523066, 5853.03112, 10436.3847, 12648.719, 46903.951, 62562.6664, 88360.1568, 106897.986, 116380.758, 123996.743, 124688.799, 128318.428, 146003.121, 183590.983, 272085.674, 462659.916, 556548.68, 572080.545, 580749.227, 592921.698, 596412.316, 611095.533, 633548.127, 655088.607, 693078.291, 751016.162, 798362.486, 798706.801 };
            IDistribution[] normalDistributions = new IDistribution[meanDamage.Length];
            for (int i = 0; i < meanDamage.Length; i++)
            {
                normalDistributions[i] = new Normal(meanDamage[i], standardDeviationDamage[i]);
            }
            return normalDistributions;
        }

        private static UncertainPairedData residentialTotalDamage = new UncertainPairedData(_StageDamageXValues, _ResidentialStageDamageYValues, residentialDamageMetaData);
        private static UncertainPairedData commercialTotalDamage = new UncertainPairedData(_StageDamageXValues, _CommercialStageDamageYValues, commercialDamageMetaData);
        private static List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>() { residentialTotalDamage, commercialTotalDamage };

        //set up stage-discharge function 
        private static double[] _StageDischargeXValues = new double[] { 1166, 2000, 3000, 4000, 5320, 6000, 7000, 8175, 9000, 9995, 12175, 13706, 15157, 16962, 18278, 20000, 24000 };
        private static IDistribution[] _StageDischargeYValues = StageDischargeYValues();
        private static IDistribution[] StageDischargeYValues()
        {
            double[] meanStages = new double[] { 929.21, 931.4, 932.6, 933.6, 934.5, 935.2, 935.8, 936.5, 937.1, 937.6, 938.6, 939.7, 940.7, 941.6, 942.1, 943.2, 944.6 };
            double[] minStages = new double[] { 928.9, 930.7, 932, 933, 933.8, 934.5, 935, 935.4, 935.9, 936.3, 937.1, 937.9, 938.6, 939.3, 941.2, 942.3, 944.5 };
            double[] maxStages = new double[] { 929.5, 931.9, 933.3, 934.4, 935.3, 936.1, 936.8, 937.6, 938.2, 939, 940.2, 941.4, 942.8, 943.8, 944.25, 946.6, 948.5 };
            IDistribution[] stageDischarge = new IDistribution[meanStages.Length];  
            for (int i = 0; i < meanStages.Length; i++)
            {
                stageDischarge[i] = new Triangular(minStages[i], meanStages[i], maxStages[i]);
            }
            return stageDischarge;
        }
        private static UncertainPairedData stageDischarge = new UncertainPairedData(_StageDischargeXValues, _StageDischargeYValues, stageDischargeMetaData);

        //set up levee
        private static double[] _FailureXValues = new double[] { 930, 935, 936, 936.5, 936.9, 937 };
        private static IDistribution[] _FailureYValues = FailureYValues();

        private static IDistribution[] FailureYValues()
        {
            double[] failureProbabilities = new double[] { 0, 0.01, 0.1, 0.5, 0.9, 1 };
            IDistribution[] failureProbArray = new IDistribution[failureProbabilities.Length];
            for (int i = 0; i < failureProbabilities.Length; i++)
            {
                failureProbArray[i] = new Deterministic(failureProbabilities[i]);
            }
            return failureProbArray;
        }

        private static UncertainPairedData systemResponse = new UncertainPairedData(_FailureXValues, _FailureYValues, failureLeveeMetaData);
        private static double defaultLeveeElevation = 937;
        private static double[] defaultFailureStages = new double[] { 920, 936.999, 937 };
        private static IDistribution[] defaultFailureProbs = new IDistribution[] 
        { 
            new Deterministic(0), 
            new Deterministic(0), 
            new Deterministic(1) 
        };
        private static UncertainPairedData defaultSystemResponse = new UncertainPairedData(defaultFailureStages, defaultFailureProbs, defaultLeveeMetaData);

        private static int impactAreaID1 = 1;
        private static int impactAreaID2 = 2;
        private static int baseYear = 2030;
        private static int futureYear = 2050;
        #endregion

        [Theory]
        [InlineData(240.5)]
        public void WithoutAnalyticalExpandedStageDamage_ScenarioResults(double expectedMeanEAD)
        {
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID1)
                .withFlowFrequency(lp3)
                .withFlowStage(stageDischarge)
                .withStageDamages(stageDamageList)
                .build();
            List<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            impactAreaScenarioSimulations.Add(simulation);

            Scenario scenario = new Scenario(baseYear, impactAreaScenarioSimulations);
            ScenarioResults scenarioResults = scenario.Compute(randomProvider, convergenceCriteria);

            Scenario scenario2 = new Scenario(futureYear, impactAreaScenarioSimulations);
            ScenarioResults scenarioResults2 = scenario2.Compute(randomProvider, convergenceCriteria);

            AlternativeResults alternativeResults = Alternative.AnnualizationCompute(randomProvider, .025, 50, 1, scenarioResults, scenarioResults2);

            double actualMeanAAEQ = alternativeResults.MeanAAEQDamage();
            double actualMeanEAD = alternativeResults.MeanBaseYearEAD();

            double tolerance = 0.061;
            double EADRelativeDifference = Math.Abs(actualMeanEAD - expectedMeanEAD) / expectedMeanEAD;
            double AAEQRelativeDifference = Math.Abs(actualMeanAAEQ - expectedMeanEAD) / expectedMeanEAD; //EAD is constant over POA so AAEQ = EAD

            //TODO: Add these three lines to the investigation list. 
            //the results should be approximately the same but are off by 
            //about 10%

            IHistogram eadHistogram = alternativeResults.GetBaseYearEADHistogram();
            double actualMeanEADFromAnotherSource = eadHistogram.Mean;
            Assert.Equal(actualMeanEAD, actualMeanEADFromAnotherSource, 1);

            Assert.True(EADRelativeDifference < tolerance);
            Assert.True(AAEQRelativeDifference < tolerance);
        }

        [Theory]
        [InlineData(.3591, 120.23)]
        public void WithoutAnalytical_ScenarioResults(double expectedMeanAEP, double expectedMeanEAD)
        {
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID1)
                .withFlowFrequency(lp3)
                .withFlowStage(stageDischarge)
                .withStageDamages(stageDamageList)
                .build();
            List<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            impactAreaScenarioSimulations.Add(simulation);

            Scenario scenario = new Scenario(baseYear, impactAreaScenarioSimulations);
            ScenarioResults scenarioResults = scenario.Compute(randomProvider, convergenceCriteria);
            double actualMeanAEP = scenarioResults.MeanAEP(impactAreaID1);
            double actualMeanEAD = scenarioResults.MeanExpectedAnnualConsequences(impactAreaID1);

            double tolerance = 0.06;
            double AEPRelativeDifference = Math.Abs(actualMeanAEP - expectedMeanAEP) / expectedMeanAEP;
            double EADRelativeDifference = Math.Abs(actualMeanEAD - expectedMeanEAD) / expectedMeanEAD;

            Assert.True(AEPRelativeDifference < tolerance);
            Assert.True(EADRelativeDifference < tolerance);

        }

        [Theory]
        [InlineData(.0526, 65.20)]
        public void AnalyticalWithRegUnreg_ScenarioResults(double meanAEP, double meanEAD)
        {
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID1)
                .withFlowFrequency(lp3)
                .withInflowOutflow(regulatedUnregulated)
                .withFlowStage(stageDischarge)
                .withStageDamages(stageDamageList)
                .build();
            ImpactAreaScenarioSimulation simulation2 = ImpactAreaScenarioSimulation.builder(impactAreaID2)
                .withFlowFrequency(lp3)
                .withInflowOutflow(regulatedUnregulated)
                .withFlowStage(stageDischarge)
                .withStageDamages(stageDamageList)
                .build();
            List<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            impactAreaScenarioSimulations.Add(simulation);
            impactAreaScenarioSimulations.Add(simulation2);

            Scenario scenario = new Scenario(baseYear, impactAreaScenarioSimulations);
            ScenarioResults scenarioResults = scenario.Compute(randomProvider, convergenceCriteria);
            double actualMeanAEP = scenarioResults.MeanAEP(impactAreaID1);
            double actualMeanEAD = scenarioResults.MeanExpectedAnnualConsequences(impactAreaID1);

            double tolerance = 0.10;
            double AEPRelativeDifference = Math.Abs(actualMeanAEP - meanAEP) / meanAEP;
            double EADRelativeDifference = Math.Abs(actualMeanEAD - meanEAD) / meanEAD;

            Assert.True(AEPRelativeDifference < tolerance);
            Assert.True(EADRelativeDifference < tolerance);

        }

        [Theory]
        [InlineData(.036, 50.23)]
        public void AnalyticalWithLevee_ScenarioResults(double meanAEP, double meanEAD)
        {  
            //TODO: The compute ran when I passed a double[] instead of IDistribution[] into .WithLevee - WHY?
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID1)
                .withFlowFrequency(lp3)
                .withLevee(defaultSystemResponse,defaultLeveeElevation)
                .withFlowStage(stageDischarge)
                .withStageDamages(stageDamageList)
                .build();
            List<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            impactAreaScenarioSimulations.Add(simulation);

            Scenario scenario = new Scenario(baseYear, impactAreaScenarioSimulations);
            ScenarioResults scenarioResults = scenario.Compute(randomProvider, convergenceCriteria);

            double actualMeanAEP = scenarioResults.MeanAEP(impactAreaID1);
            double actualMeanEAD = scenarioResults.MeanExpectedAnnualConsequences(impactAreaID1);

            //Note the tolerance: 2.0 results are just under 14% different from the 1.4.3 results 
            //whereas without the levee, 2.0 is 6% different from 1.4.3
            //so something about the levee
            //large quantity of iterations does not change the result 
            double tolerance = 0.14;
            double AEPRelativeDifference = Math.Abs(actualMeanAEP - meanAEP) / meanAEP;
            double EADRelativeDifference = Math.Abs(actualMeanEAD - meanEAD) / meanEAD;

            Assert.True(AEPRelativeDifference < tolerance);
            Assert.True(EADRelativeDifference < tolerance);

        }

        [Theory]
        [InlineData(.036, 24.80)]
        public void AnalyticalWithLeveeAndExtInt_ScenarioResults(double meanAEP, double meanEAD)
        {//TODO: Why would AEP here be closer than AEP above?
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID1)
                .withFlowFrequency(lp3)
                .withLevee(defaultSystemResponse, defaultLeveeElevation)
                .withFlowStage(stageDischarge)
                .withInteriorExterior(interiorExterior) 
                .withStageDamages(stageDamageList)
                .build();
            List<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            impactAreaScenarioSimulations.Add(simulation);

            Scenario scenario = new Scenario(baseYear, impactAreaScenarioSimulations);
            ScenarioResults scenarioResults = scenario.Compute(randomProvider, convergenceCriteria);
            double actualMeanAEP = scenarioResults.MeanAEP(impactAreaID1);
            double actualMeanEAD = scenarioResults.MeanExpectedAnnualConsequences(impactAreaID1);

            double tolerance = 0.14;
            double AEPRelativeDifference = Math.Abs(actualMeanAEP - meanAEP) / meanAEP;
            double EADRelativeDifference = Math.Abs(actualMeanEAD - meanEAD) / meanEAD;

            Assert.True(AEPRelativeDifference < tolerance);
            Assert.True(EADRelativeDifference < tolerance);

        }

        [Theory]
        [InlineData(.1986, 88.73)]
        public void AnalyticalWithLeveeAndFragility_ScenarioResults(double meanAEP, double meanEAD)
        {
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID1)
                .withFlowFrequency(lp3)
                .withLevee(systemResponse, defaultLeveeElevation)
                .withFlowStage(stageDischarge)
                .withStageDamages(stageDamageList)
                .build();

            List<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            impactAreaScenarioSimulations.Add(simulation);

            Scenario scenario = new Scenario(baseYear, impactAreaScenarioSimulations);
            ScenarioResults scenarioResults = scenario.Compute(randomProvider, convergenceCriteria);
            double actualMeanAEP = scenarioResults.MeanAEP(impactAreaID1);
            double actualMeanEAD = scenarioResults.MeanExpectedAnnualConsequences(impactAreaID1);

            double tolerance = 0.10;
            double AEPRelativeDifference = Math.Abs(actualMeanAEP - meanAEP) / meanAEP;
            double EADRelativeDifference = Math.Abs(actualMeanEAD - meanEAD) / meanEAD;

            Assert.True(AEPRelativeDifference < tolerance);
            Assert.True(EADRelativeDifference < tolerance);

        }

        [Theory]
        [InlineData(.1522, 65.42)]
        public void WithoutGraphicalFlow_ScenarioResults(double meanAEP, double meanEAD)
        {//TODO: These results are REALLY messed up mathematically 
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID1)
                .withFlowFrequency(graphicalFlowFrequency)
                .withFlowStage(stageDischarge)
                .withStageDamages(stageDamageList)
                .build();
            List<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            impactAreaScenarioSimulations.Add(simulation);

            Scenario scenario = new Scenario(baseYear, impactAreaScenarioSimulations);
            ScenarioResults scenarioResults = scenario.Compute(randomProvider, convergenceCriteria);
            double actualMeanAEP = scenarioResults.MeanAEP(impactAreaID1);
            double actualMeanEAD = scenarioResults.MeanExpectedAnnualConsequences(impactAreaID1);

            double tolerance = 0.10;
            double AEPRelativeDifference = Math.Abs(actualMeanAEP - meanAEP) / meanAEP;
            double EADRelativeDifference = Math.Abs(actualMeanEAD - meanEAD) / meanEAD;

            Assert.True(AEPRelativeDifference < tolerance);
            Assert.True(EADRelativeDifference < tolerance);

        }

        [Theory]
        [InlineData(.1554, 45.36)]
        public void WithoutGraphicalStage_ScenarioResults(double meanAEP, double meanEAD)
        {//TODO: These results are REALLY messed up mathematically 
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID1)
                .withFrequencyStage(graphicalStageFrequency)
                .withStageDamages(stageDamageList)
                .build();
            List<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            impactAreaScenarioSimulations.Add(simulation);

            Scenario scenario = new Scenario(baseYear, impactAreaScenarioSimulations);
            ScenarioResults scenarioResults = scenario.Compute(randomProvider, convergenceCriteria);
            double actualMeanAEP = scenarioResults.MeanAEP(impactAreaID1);
            double actualMeanEAD = scenarioResults.MeanExpectedAnnualConsequences(impactAreaID1);

            double tolerance = 0.10;
            double AEPRelativeDifference = Math.Abs(actualMeanAEP - meanAEP) / meanAEP;
            double EADRelativeDifference = Math.Abs(actualMeanEAD - meanEAD) / meanEAD;

            Assert.True(AEPRelativeDifference < tolerance);
            Assert.True(EADRelativeDifference < tolerance);

        }
        [Fact]
        public void WithoutGraphicalStageAsFlows_ScenarioResults()
        {
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID1)
                .withFlowFrequency(graphicalStageAsFlowsFrequency)
                .withFlowStage(stageDischarge)
                .withStageDamages(stageDamageList)
                .build();
            List<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            impactAreaScenarioSimulations.Add(simulation);
            Scenario scenario = new Scenario(baseYear, impactAreaScenarioSimulations);
            ScenarioResults scenarioResults = scenario.Compute(randomProvider, convergenceCriteria);
            ImpactAreaScenarioResults impactAreaScenarioResults = scenarioResults.GetResults(impactAreaID1);

            bool resultsAreNull = impactAreaScenarioResults.ConsequenceResults.ConsequenceResultList.Count == 0;
            
            Assert.True(resultsAreNull);

        }
        //The expected values below are not for testing the validity of the compute 
        //rather, the values are used as part of troubleshooting unhandled exceptions
        //TODO this objective of this test is to pass in a sample size of zero and return blank results 
        //The property rule is not working like we expect 
        //until the property rule works, we need to keep a good sample size here 
        [Fact]
        public void AssuranceOfAEPDoesNotHitIndexOutOfBoundsException()
        {
            ContinuousDistribution lp3 = new LogPearson3(3.3, .254, -.1021, 0);
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID1)
                .withFlowFrequency(lp3)
                .withFlowStage(stageDischarge)
                .withStageDamages(stageDamageList)
                .withLevee(systemResponse, defaultLeveeElevation)
                .build();
            List<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            impactAreaScenarioSimulations.Add(simulation);

            Scenario scenario = new Scenario(baseYear, impactAreaScenarioSimulations);
            ScenarioResults scenarioResults = scenario.Compute(randomProvider, convergenceCriteria);

            bool resultsAreNull = scenarioResults.GetResults(impactAreaID1).ConsequenceResults.ConsequenceResultList.Count == 0;
            Assert.True(resultsAreNull);
        }
    }
}
