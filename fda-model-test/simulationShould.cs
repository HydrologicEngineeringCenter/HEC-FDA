
using ead;
using paireddata;
using Statistics;

namespace fda_model_test
{
    class simulationShould
    {
        //These were previously used in pairedDataTest but were moved here to be used for ead compute testing. 
        static double[] Probabilities = { .999999, 0.0000001 };
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 200000 };;
        //static double[] ProbabilitiesOfFailure = { .001, .01, .1, .5, 1 };
        //static double[] ElevationsOfFailure = { 600, 610, 650, 700, 750 };
        [Theory]
        [InlineData(150000)]
        public void ProducePairedData(double expected)
        {
            
            Statistics.IDistribution flow_frequency = IDistributionFactory.FactoryUniform(0, 100000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000*i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000*i, 10);
            }
            PairedData stage_damage = new PairedData(Stages, damages, "residential");
            ead.Simulation s = new ead.Simulation(flow_frequency,flow_stage,stage_damage);
            
            metrics.IContainResults r = ead.Compute(0,1);

            Assert.Equal(expectedSlope, r.MeanEAD("residential"));
        }

        
    }
}
