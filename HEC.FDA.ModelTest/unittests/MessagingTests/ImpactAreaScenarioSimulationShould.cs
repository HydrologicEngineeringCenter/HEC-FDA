﻿using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.MVVMFramework.Base.Implementations;
using Statistics;
using Statistics.Distributions;
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
    }
}
