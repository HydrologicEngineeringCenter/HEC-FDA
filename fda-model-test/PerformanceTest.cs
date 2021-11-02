using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using metrics;
using ead;
using paireddata;
using Statistics;

namespace fda_model_test
{
   
    public class PerformanceTest
    {
        //These were previously used in pairedDataTest but were moved here to be used for ead compute testing. 
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 150000 };

        [Theory]
        [InlineData(1234, 100, 0.002, 0)]
        public void ComputeEAD_Iterations(int seed, int iterations, double exceedanceProbability, double expected)
        {

            Statistics.IDistribution flow_frequency = IDistributionFactory.FactoryUniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, "residential");
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);

            Simulation s = new Simulation(flow_frequency, flow_stage, upd);
            Threshold threshold = new Threshold(0, ThresholdEnum.ExteriorStage, 150000);
            s.PerformanceThresholds.AddThreshold(threshold);
            RandomProvider rp = new RandomProvider(seed);
            metrics.Results r = s.Compute(rp, iterations);

            double actual = r.Thresholds.ListOfThresholds.Last().Performance.ConditionalNonExceedanceProbability(exceedanceProbability); 
            double difference = expected - actual;
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .01);
        }
    }

}

