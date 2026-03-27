using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.MVVMFramework.Base.Implementations;
using Statistics;
using Statistics.Distributions;
using Statistics.Histograms;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace HEC.FDA.ModelTest.unittests.MessagingTests
{
    [Trait("RunsOn", "Remote")]
    public class ImpactAreaScenarioSimulationShould
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
        static CurveMetaData curveMetaData = new CurveMetaData(xLabel, name, damCat);

        [Theory]
        [InlineData(1234)]
        public void ReportErrorsAndWarningsMessages(int seed)
        {
            GraphicalUncertainPairedData dischargeFrequency = new GraphicalUncertainPairedData(exceedanceProbabilities, dischargeFrequencyDischarges, equivalentRecordLength, curveMetaData, usingStagesNotFlows: false);
            UncertainPairedData stageDischarge = new UncertainPairedData(stageDischargeFunctionDischarges, stageDischargeFunctionStageDistributions, curveMetaData);
            UncertainPairedData stageDamage = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, curveMetaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
            stageDamageList.Add(stageDamage);
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(dischargeFrequency)
                .WithFlowStage(stageDischarge)
                .WithStageDamages(stageDamageList)
                .Build();
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 101, maxIterations: 300);


            Listener listener = new Listener();
            MessageHub.Subscribe(listener);
            ImpactAreaScenarioResults results = simulation.Compute(convergenceCriteria);
            MessageHub.UnsubscribeAll(listener);
            Debug.WriteLine(listener.GetMessageLogAsString());

            Assert.True(listener.MessageLog.Count > 0);
        }

        [Fact]
        public void PopulateUncertainConsequenceFrequencyCurvesOnCompute()
        {
            // Arrange - use metadata with proper damage/asset categories
            CurveMetaData stageDamageMetaData = new CurveMetaData("Residential", "Structure");
            GraphicalUncertainPairedData dischargeFrequency = new GraphicalUncertainPairedData(exceedanceProbabilities, dischargeFrequencyDischarges, equivalentRecordLength, curveMetaData, usingStagesNotFlows: false);
            UncertainPairedData stageDischarge = new UncertainPairedData(stageDischargeFunctionDischarges, stageDischargeFunctionStageDistributions, curveMetaData);
            UncertainPairedData stageDamage = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, stageDamageMetaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData> { stageDamage };

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(dischargeFrequency)
                .WithFlowStage(stageDischarge)
                .WithStageDamages(stageDamageList)
                .Build();

            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 100, maxIterations: 200);

            // Act
            ImpactAreaScenarioResults results = simulation.Compute(convergenceCriteria);

            // Assert - should have created UncertainConsequenceFrequencyCurves
            Assert.NotEmpty(results.UncertainConsequenceFrequencyCurves);
            Assert.Single(results.UncertainConsequenceFrequencyCurves); // One stage-damage function = one curve
        }

        [Fact]
        public void PopulateUncertainConsequenceFrequencyCurvesWithCorrectMetadata()
        {
            // Arrange - use metadata with proper damage/asset categories
            CurveMetaData stageDamageMetaData = new CurveMetaData("Residential", "Structure");
            GraphicalUncertainPairedData dischargeFrequency = new GraphicalUncertainPairedData(exceedanceProbabilities, dischargeFrequencyDischarges, equivalentRecordLength, curveMetaData, usingStagesNotFlows: false);
            UncertainPairedData stageDischarge = new UncertainPairedData(stageDischargeFunctionDischarges, stageDischargeFunctionStageDistributions, curveMetaData);
            UncertainPairedData stageDamage = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, stageDamageMetaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData> { stageDamage };

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(dischargeFrequency)
                .WithFlowStage(stageDischarge)
                .WithStageDamages(stageDamageList)
                .Build();

            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 100, maxIterations: 200);

            // Act
            ImpactAreaScenarioResults results = simulation.Compute(convergenceCriteria);

            // Assert - curve should have correct metadata
            var uncertainCurve = results.UncertainConsequenceFrequencyCurves[0];
            Assert.Equal("Residential", uncertainCurve.DamageCategory);
            Assert.Equal("Structure", uncertainCurve.AssetCategory);
            Assert.Equal(ConsequenceType.Damage, uncertainCurve.ConsequenceType);
            Assert.Equal(RiskType.Fail, uncertainCurve.RiskType);
        }

        [Fact]
        public void PopulateUncertainConsequenceFrequencyCurvesWithHistogramData()
        {
            // Arrange - use metadata with proper damage/asset categories
            CurveMetaData stageDamageMetaData = new CurveMetaData("Residential", "Structure");
            GraphicalUncertainPairedData dischargeFrequency = new GraphicalUncertainPairedData(exceedanceProbabilities, dischargeFrequencyDischarges, equivalentRecordLength, curveMetaData, usingStagesNotFlows: false);
            UncertainPairedData stageDischarge = new UncertainPairedData(stageDischargeFunctionDischarges, stageDischargeFunctionStageDistributions, curveMetaData);
            UncertainPairedData stageDamage = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, stageDamageMetaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData> { stageDamage };

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(dischargeFrequency)
                .WithFlowStage(stageDischarge)
                .WithStageDamages(stageDamageList)
                .Build();

            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 100, maxIterations: 200);

            // Act
            ImpactAreaScenarioResults results = simulation.Compute(convergenceCriteria);

            // Assert - histograms should have been populated with data
            var uncertainCurve = results.UncertainConsequenceFrequencyCurves[0];
            Assert.NotEmpty(uncertainCurve.YHistograms);
            foreach (var histogram in uncertainCurve.YHistograms)
            {
                Assert.True(histogram.SampleSize >= convergenceCriteria.MinIterations,
                    $"Expected at least {convergenceCriteria.MinIterations} samples, but got {histogram.SampleSize}");
            }
        }

        [Fact]
        public void PopulateUncertainConsequenceFrequencyCurvesForMultipleDamageCategories()
        {
            // Arrange - use metadata with proper damage/asset categories
            CurveMetaData residentialMetaData = new CurveMetaData("Residential", "Structure");
            CurveMetaData commercialMetaData = new CurveMetaData("Commercial", "Structure");

            GraphicalUncertainPairedData dischargeFrequency = new GraphicalUncertainPairedData(exceedanceProbabilities, dischargeFrequencyDischarges, equivalentRecordLength, curveMetaData, usingStagesNotFlows: false);
            UncertainPairedData stageDischarge = new UncertainPairedData(stageDischargeFunctionDischarges, stageDischargeFunctionStageDistributions, curveMetaData);

            UncertainPairedData residentialStageDamage = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, residentialMetaData);
            UncertainPairedData commercialStageDamage = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, commercialMetaData);

            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData> { residentialStageDamage, commercialStageDamage };

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(dischargeFrequency)
                .WithFlowStage(stageDischarge)
                .WithStageDamages(stageDamageList)
                .Build();

            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 100, maxIterations: 200);

            // Act
            ImpactAreaScenarioResults results = simulation.Compute(convergenceCriteria);

            // Assert - should have two UncertainConsequenceFrequencyCurves (one per damage category)
            Assert.Equal(2, results.UncertainConsequenceFrequencyCurves.Count);

            var residentialCurve = results.UncertainConsequenceFrequencyCurves.Find(c => c.DamageCategory == "Residential");
            var commercialCurve = results.UncertainConsequenceFrequencyCurves.Find(c => c.DamageCategory == "Commercial");

            Assert.NotNull(residentialCurve);
            Assert.NotNull(commercialCurve);
        }

        [Fact]
        public void ComputeWithInteriorExteriorRelationship()
        {
            // Arrange - interior-exterior maps channel stage to floodplain stage
            // The floodplain stage is lower than the channel stage (typical interior area)
            double[] channelStages = new double[] { 458, 465, 470, 475, 480 };
            IDistribution[] floodplainStageDistributions = new IDistribution[]
            {
                new Normal(455, 0),
                new Normal(461, .2),
                new Normal(465, .3),
                new Normal(469, .4),
                new Normal(473, .5)
            };
            CurveMetaData interiorExteriorMetaData = new CurveMetaData(xLabel, yLabel, name);
            UncertainPairedData interiorExterior = new UncertainPairedData(channelStages, floodplainStageDistributions, interiorExteriorMetaData);

            CurveMetaData stageDamageMetaData = new CurveMetaData("Residential", "Structure");
            GraphicalUncertainPairedData dischargeFrequency = new GraphicalUncertainPairedData(exceedanceProbabilities, dischargeFrequencyDischarges, equivalentRecordLength, curveMetaData, usingStagesNotFlows: false);
            UncertainPairedData stageDischarge = new UncertainPairedData(stageDischargeFunctionDischarges, stageDischargeFunctionStageDistributions, curveMetaData);
            UncertainPairedData stageDamage = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, stageDamageMetaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData> { stageDamage };

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(dischargeFrequency)
                .WithFlowStage(stageDischarge)
                .WithInteriorExterior(interiorExterior)
                .WithStageDamages(stageDamageList)
                .Build();

            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 100, maxIterations: 200);

            // Act
            ImpactAreaScenarioResults results = simulation.Compute(convergenceCriteria);

            // Assert - compute should succeed and produce results
            double ead = results.ConsequenceResults.SampleMeanDamage("Residential", "Structure", impactAreaID);
            Assert.True(ead >= 0, "EAD should be non-negative");
        }

        [Fact]
        public void ProduceLowerDamagesWithInteriorExteriorThatReducesStage()
        {
            // Arrange - compute without interior-exterior first
            CurveMetaData stageDamageMetaData = new CurveMetaData("Residential", "Structure");
            GraphicalUncertainPairedData dischargeFrequency = new GraphicalUncertainPairedData(exceedanceProbabilities, dischargeFrequencyDischarges, equivalentRecordLength, curveMetaData, usingStagesNotFlows: false);
            UncertainPairedData stageDischarge = new UncertainPairedData(stageDischargeFunctionDischarges, stageDischargeFunctionStageDistributions, curveMetaData);
            UncertainPairedData stageDamage = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, stageDamageMetaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData> { stageDamage };
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 100, maxIterations: 200);

            ImpactAreaScenarioSimulation simulationWithout = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(dischargeFrequency)
                .WithFlowStage(stageDischarge)
                .WithStageDamages(stageDamageList)
                .Build();
            ImpactAreaScenarioResults resultsWithout = simulationWithout.Compute(convergenceCriteria);
            double eadWithout = resultsWithout.ConsequenceResults.SampleMeanDamage("Residential", "Structure", impactAreaID);

            // Arrange - compute with interior-exterior that lowers floodplain stage
            double[] channelStages = new double[] { 458, 465, 470, 475, 480 };
            IDistribution[] floodplainStageDistributions = new IDistribution[]
            {
                new Normal(455, 0),
                new Normal(461, .2),
                new Normal(465, .3),
                new Normal(469, .4),
                new Normal(473, .5)
            };
            CurveMetaData interiorExteriorMetaData = new CurveMetaData(xLabel, yLabel, name);
            UncertainPairedData interiorExterior = new UncertainPairedData(channelStages, floodplainStageDistributions, interiorExteriorMetaData);

            // Need fresh objects since random numbers get generated during compute
            GraphicalUncertainPairedData dischargeFrequency2 = new GraphicalUncertainPairedData(exceedanceProbabilities, dischargeFrequencyDischarges, equivalentRecordLength, curveMetaData, usingStagesNotFlows: false);
            UncertainPairedData stageDischarge2 = new UncertainPairedData(stageDischargeFunctionDischarges, stageDischargeFunctionStageDistributions, curveMetaData);
            UncertainPairedData stageDamage2 = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, stageDamageMetaData);
            List<UncertainPairedData> stageDamageList2 = new List<UncertainPairedData> { stageDamage2 };

            ImpactAreaScenarioSimulation simulationWith = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(dischargeFrequency2)
                .WithFlowStage(stageDischarge2)
                .WithInteriorExterior(interiorExterior)
                .WithStageDamages(stageDamageList2)
                .Build();
            ImpactAreaScenarioResults resultsWith = simulationWith.Compute(convergenceCriteria);
            double eadWith = resultsWith.ConsequenceResults.SampleMeanDamage("Residential", "Structure", impactAreaID);

            // Assert - interior-exterior that reduces stage should reduce damages
            Assert.True(eadWith < eadWithout, $"EAD with interior-exterior ({eadWith}) should be less than without ({eadWithout})");
        }

        [Fact]
        public void ComputeWithIdentityInteriorExteriorProducesSimilarResults()
        {
            // Arrange - identity interior-exterior (floodplain stage = channel stage) should produce similar EAD
            CurveMetaData stageDamageMetaData = new CurveMetaData("Residential", "Structure");
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 100, maxIterations: 200);

            // Without interior-exterior
            GraphicalUncertainPairedData dischargeFrequency1 = new GraphicalUncertainPairedData(exceedanceProbabilities, dischargeFrequencyDischarges, equivalentRecordLength, curveMetaData, usingStagesNotFlows: false);
            UncertainPairedData stageDischarge1 = new UncertainPairedData(stageDischargeFunctionDischarges, stageDischargeFunctionStageDistributions, curveMetaData);
            UncertainPairedData stageDamage1 = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, stageDamageMetaData);

            ImpactAreaScenarioSimulation simWithout = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(dischargeFrequency1)
                .WithFlowStage(stageDischarge1)
                .WithStageDamages(new List<UncertainPairedData> { stageDamage1 })
                .Build();
            double eadWithout = simWithout.Compute(convergenceCriteria).ConsequenceResults.SampleMeanDamage("Residential", "Structure", impactAreaID);

            // With identity interior-exterior (output = input, no uncertainty)
            double[] channelStages = new double[] { 458, 465, 470, 475, 480 };
            IDistribution[] identityDistributions = new IDistribution[]
            {
                new Normal(458, 0),
                new Normal(465, 0),
                new Normal(470, 0),
                new Normal(475, 0),
                new Normal(480, 0)
            };
            CurveMetaData interiorExteriorMetaData = new CurveMetaData(xLabel, yLabel, name);
            UncertainPairedData identityInteriorExterior = new UncertainPairedData(channelStages, identityDistributions, interiorExteriorMetaData);

            GraphicalUncertainPairedData dischargeFrequency2 = new GraphicalUncertainPairedData(exceedanceProbabilities, dischargeFrequencyDischarges, equivalentRecordLength, curveMetaData, usingStagesNotFlows: false);
            UncertainPairedData stageDischarge2 = new UncertainPairedData(stageDischargeFunctionDischarges, stageDischargeFunctionStageDistributions, curveMetaData);
            UncertainPairedData stageDamage2 = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, stageDamageMetaData);

            ImpactAreaScenarioSimulation simWith = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(dischargeFrequency2)
                .WithFlowStage(stageDischarge2)
                .WithInteriorExterior(identityInteriorExterior)
                .WithStageDamages(new List<UncertainPairedData> { stageDamage2 })
                .Build();
            double eadWith = simWith.Compute(convergenceCriteria).ConsequenceResults.SampleMeanDamage("Residential", "Structure", impactAreaID);

            // Assert - identity transform should produce similar EAD (within 10% tolerance for Monte Carlo noise)
            double tolerance = eadWithout * 0.10;
            Assert.True(System.Math.Abs(eadWith - eadWithout) < tolerance,
                $"Identity interior-exterior EAD ({eadWith}) should be close to no-transform EAD ({eadWithout}), tolerance: {tolerance}");
        }

        [Fact]
        public void ComputeWithInteriorExteriorAndLevee()
        {
            // Arrange - interior-exterior maps channel stage to a lower floodplain stage
            double[] channelStages = new double[] { 458, 465, 470, 475, 480 };
            IDistribution[] floodplainStageDistributions = new IDistribution[]
            {
                new Normal(455, 0),
                new Normal(461, 0),
                new Normal(465, 0),
                new Normal(469, 0),
                new Normal(473, 0)
            };
            CurveMetaData interiorExteriorMetaData = new CurveMetaData(xLabel, yLabel, name);
            UncertainPairedData interiorExterior = new UncertainPairedData(channelStages, floodplainStageDistributions, interiorExteriorMetaData);

            // Levee fragility defined in channel stage (exterior) - levee starts failing at channel stage 474
            double[] leveeStages = new double[] { 473, 474, 475, 476, 477 };
            IDistribution[] leveeFailureProbs = new IDistribution[]
            {
                new Deterministic(0.0),
                new Deterministic(0.1),
                new Deterministic(0.5),
                new Deterministic(0.9),
                new Deterministic(1.0)
            };
            UncertainPairedData levee = new UncertainPairedData(leveeStages, leveeFailureProbs, new CurveMetaData(xLabel, yLabel, name));
            double topOfLevee = 477;

            CurveMetaData stageDamageMetaData = new CurveMetaData("Residential", "Structure");
            GraphicalUncertainPairedData dischargeFrequency = new GraphicalUncertainPairedData(exceedanceProbabilities, dischargeFrequencyDischarges, equivalentRecordLength, curveMetaData, usingStagesNotFlows: false);
            UncertainPairedData stageDischarge = new UncertainPairedData(stageDischargeFunctionDischarges, stageDischargeFunctionStageDistributions, curveMetaData);
            UncertainPairedData stageDamage = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, stageDamageMetaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData> { stageDamage };

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(dischargeFrequency)
                .WithFlowStage(stageDischarge)
                .WithInteriorExterior(interiorExterior)
                .WithLevee(levee, topOfLevee)
                .WithStageDamages(stageDamageList)
                .Build();

            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 100, maxIterations: 200);

            // Act
            ImpactAreaScenarioResults results = simulation.Compute(convergenceCriteria);

            // Assert - compute should succeed and produce non-negative EAD
            double ead = results.ConsequenceResults.SampleMeanDamage("Residential", "Structure", impactAreaID);
            Assert.True(ead >= 0, $"EAD should be non-negative, got {ead}");
        }

        [Fact]
        public void EvaluateSystemResponseAgainstChannelStageNotFloodplainStage()
        {
            // This test verifies that system response is evaluated against channel (exterior) stage.
            // We set up a levee that fails at channel stage 474+ and an interior-exterior that maps
            // channel stage to much lower floodplain stage. If the system response were incorrectly
            // evaluated against floodplain stage, the levee would never fail (floodplain stages < 474),
            // resulting in near-zero EAD. With the correct behavior, the levee fails based on channel stage.
            double[] channelStages = new double[] { 458, 465, 470, 475, 480 };
            IDistribution[] floodplainStageDistributions = new IDistribution[]
            {
                new Normal(450, 0),
                new Normal(453, 0),
                new Normal(456, 0),
                new Normal(459, 0),
                new Normal(462, 0)
            };
            CurveMetaData interiorExteriorMetaData = new CurveMetaData(xLabel, yLabel, name);
            UncertainPairedData interiorExterior = new UncertainPairedData(channelStages, floodplainStageDistributions, interiorExteriorMetaData);

            // Levee fragility at channel stage 474 - only fails at high channel stages
            double[] leveeStages = new double[] { 473, 474, 475 };
            IDistribution[] leveeFailureProbs = new IDistribution[]
            {
                new Deterministic(0.0),
                new Deterministic(0.5),
                new Deterministic(1.0)
            };
            UncertainPairedData levee = new UncertainPairedData(leveeStages, leveeFailureProbs, new CurveMetaData(xLabel, yLabel, name));

            // Stage-damage defined at floodplain stages (450-470 range)
            double[] floodplainDamageStages = new double[] { 450, 452, 454, 456, 458, 460, 462, 464 };
            IDistribution[] floodplainDamageDistributions = new IDistribution[]
            {
                new Normal(0, 0),
                new Normal(0, 0),
                new Normal(5, 0),
                new Normal(20, 0),
                new Normal(50, 0),
                new Normal(100, 0),
                new Normal(150, 0),
                new Normal(200, 0)
            };
            CurveMetaData stageDamageMetaData = new CurveMetaData("Residential", "Structure");
            UncertainPairedData stageDamage = new UncertainPairedData(floodplainDamageStages, floodplainDamageDistributions, stageDamageMetaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData> { stageDamage };

            GraphicalUncertainPairedData dischargeFrequency = new GraphicalUncertainPairedData(exceedanceProbabilities, dischargeFrequencyDischarges, equivalentRecordLength, curveMetaData, usingStagesNotFlows: false);
            UncertainPairedData stageDischarge = new UncertainPairedData(stageDischargeFunctionDischarges, stageDischargeFunctionStageDistributions, curveMetaData);

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(dischargeFrequency)
                .WithFlowStage(stageDischarge)
                .WithInteriorExterior(interiorExterior)
                .WithLevee(levee, 475)
                .WithStageDamages(stageDamageList)
                .Build();

            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 100, maxIterations: 200);

            // Act
            ImpactAreaScenarioResults results = simulation.Compute(convergenceCriteria);

            // Assert - EAD should be > 0 because the levee fails at channel stage 474+
            // (which does occur in the frequency curve). If system response were incorrectly
            // evaluated against floodplain stage (max ~462), the levee would never fail.
            double ead = results.ConsequenceResults.SampleMeanDamage("Residential", "Structure", impactAreaID);
            Assert.True(ead > 0, $"EAD should be positive (levee should fail at channel stage 474+), got {ead}");
        }

        [Fact]
        public void PopulateUncertainConsequenceFrequencyCurvesWithNonZeroMean()
        {
            // Arrange - use metadata with proper damage/asset categories
            CurveMetaData stageDamageMetaData = new CurveMetaData("Residential", "Structure");
            GraphicalUncertainPairedData dischargeFrequency = new GraphicalUncertainPairedData(exceedanceProbabilities, dischargeFrequencyDischarges, equivalentRecordLength, curveMetaData, usingStagesNotFlows: false);
            UncertainPairedData stageDischarge = new UncertainPairedData(stageDischargeFunctionDischarges, stageDischargeFunctionStageDistributions, curveMetaData);
            UncertainPairedData stageDamage = new UncertainPairedData(stageDamageStages, stageDamageDamageDistributions, stageDamageMetaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData> { stageDamage };

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(dischargeFrequency)
                .WithFlowStage(stageDischarge)
                .WithStageDamages(stageDamageList)
                .Build();

            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 100, maxIterations: 200);

            // Act
            ImpactAreaScenarioResults results = simulation.Compute(convergenceCriteria);

            // Assert - the mean curve should have some non-zero values
            var uncertainCurve = results.UncertainConsequenceFrequencyCurves[0];
            UncertainPairedData upd = uncertainCurve.GetUncertainPairedData();

            // At least some y-values should be non-zero (we have damages at higher stages)
            bool hasNonZeroValues = false;
            foreach (var yDist in upd.Yvals)
            {
                double mean = ((IHistogram)yDist).SampleMean;
                if (mean > 0)
                {
                    hasNonZeroValues = true;
                    break;
                }
            }
            Assert.True(hasNonZeroValues, "Mean curve should have at least some non-zero damage values");
        }
    }
}
