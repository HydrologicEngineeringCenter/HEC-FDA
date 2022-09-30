using System.Collections.Generic;
using Xunit;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Statistics.Distributions;
using HEC.FDA.Statistics.Convergence;

namespace HEC.FDA.ModelTest.integrationtests
{
    public class DesPeresStudyDataTests
    {
        static int impactAreaID = 1;
        static int erl = 50;
        static double[] exceedanceProabilities = new double[] { .5, .2, .1, .04, .02, .01, .005, .002 };
        static double[] stagesForFrequency = new double[] { .001, .002, .003, .004, .005, .006, .007, .553 };
        static CurveMetaData metaDataDefault = new CurveMetaData("x", "y", "res", "struct");
        static GraphicalUncertainPairedData graphicalUncertain = new GraphicalUncertainPairedData(exceedanceProabilities, stagesForFrequency, erl, metaDataDefault);
        static double[] stagesForDamage = new double[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2, 2.1 };

        static IDistribution[] zeroDamageDistributions = new IDistribution[]
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
                    new Normal(0,0)
        };
        static UncertainPairedData zeroStageDamage = new UncertainPairedData(stagesForDamage, zeroDamageDistributions, metaDataDefault);

        static List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
        static int seed = 1234;
        static RandomProvider randomProvider = new RandomProvider(seed);
        [Fact]
        public void ComputeShould()
        {
            stageDamageList.Add(zeroStageDamage);
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID)
                .withFrequencyStage(graphicalUncertain)
                .withStageDamages(stageDamageList)
                .build();

            ImpactAreaScenarioResults impactAreaScenarioResults = simulation.Compute(randomProvider, new ConvergenceCriteria());

            Assert.True(impactAreaScenarioResults.ConsequenceResults.GetConsequenceResultsHistogram("res", "struct", impactAreaID).HistogramIsZeroValued);

        }
    }
}
