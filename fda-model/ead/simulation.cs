using System;
using System.Collections;
namespace ead{
    public class Simulation{
        private statistics.IBootstrap _frequency_flow;
        private paireddata.UncertainPairedData _inflow_outflow;
        private paireddata.UncertainPairedData _flow_stage;
        private paireddata.UncertainPairedData _channelstage_floodplainstage;
        private paireddata.UncertainPairedData _levee_curve;
        private IList<paireddata.UncertainPairedData> _damage_category_stage_damage;
        private metrics.IContainResults _results;
        
        public metrics.IContainResults Compute(Int64 seed, Int64 iterations){
            metrics.IContainResults results = new metrics.Results();
            //results.AEPThreshold = 100.0;//stage or flow or damage threshold
            for(int i = 0; i < iterations; i ++){
                
                paireddata.IPairedData ff = _frequency_flow.Bootstrap_to_PairedData(seed,50,1000);
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
        
    }
}