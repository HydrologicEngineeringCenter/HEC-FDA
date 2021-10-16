using System;
using System.Collections.Generic;
using Statistics;
namespace ead{
    public class Simulation{
        private IDistribution _frequency_flow;
        private paireddata.UncertainPairedData _inflow_outflow;
        private paireddata.UncertainPairedData _flow_stage;
        private paireddata.UncertainPairedData _channelstage_floodplainstage;
        private paireddata.UncertainPairedData _levee_curve;
        private List<paireddata.UncertainPairedData> _damage_category_stage_damage;
        private metrics.IContainResults _results;
        
        public metrics.IContainResults Compute(Int64 seed, Int64 iterations){
            metrics.IContainResults results = new metrics.Results();
            //results.AEPThreshold = 100.0;//stage or flow or damage threshold
            for(int i = 0; i < iterations; i ++){
                paireddata.IPairedData ff = BootstrapToPairedData(_frequency_flow, 1000);//ordinates defines the number of values in the frequency curve, more would be a better approximation.
                //check if flow transform exists, and use it here
                paireddata.IPairedData _flow_stage_sample = _flow_stage.SamplePairedData(.5);
                paireddata.IPairedData stage_frequency = ff.compose(_flow_stage_sample);
                //compute aep metrics here
                //results.AddAEPEstimate
                //interior exterior
                //levees
                double totalEAD = 0.0;
                foreach(paireddata.UncertainPairedData pd in _damage_category_stage_damage){
                    paireddata.IPairedData _stage_damage_sample = pd.SamplePairedData(.5);
                    paireddata.IPairedData damage_frequency = stage_frequency.compose(_stage_damage_sample);
                    double eadEstimate = damage_frequency.integrate();
                    totalEAD += eadEstimate;
                    results.AddEADEstimate(eadEstimate, pd.Category);
                }
                results.AddEADEstimate(totalEAD, "Total");
            }
            return results;
        }
        private paireddata.IPairedData BootstrapToPairedData(IDistribution dist, Int64 ordinates){
            double[] randyPacket = new double[dist.SampleSize];//needs to be initialized with a set of random nubmers between 0 and 1;
            IDistribution bootstrap = dist.Sample(randyPacket);
            double[] x = new double[ordinates];
            double[] y = new double[ordinates];
            for(int i=0;i<ordinates; i++){
                double prob = ((double)i)/((double)ordinates);
                x[i] = prob;
                y[i] = bootstrap.InverseCDF(prob);

            }
            return new paireddata.PairedData(x, y);
        }
        
    }
}