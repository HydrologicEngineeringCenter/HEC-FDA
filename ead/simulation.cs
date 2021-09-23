using System;
namespace ead{
    public class Simulation{
        private statistics.IBootstrap _frequency_flow;
        private paireddata.UncertainPairedData _inflow_outflow;
        private paireddata.UncertainPairedData _flow_stage;
        private paireddata.UncertainPairedData _channelstage_floodplainstage;
        private paireddata.UncertainPairedData _levee_curve;
        private paireddata.UncertainPairedData _stage_damage;
        
        public double Compute(Int64 seed, Int64 iterations){
            double meanEad = 0.0;
            for(int i = 0; i < iterations; i ++){
                
                paireddata.IPairedData ff = _frequency_flow.Bootstrap_to_PairedData(seed,50,1000);
                //check if flow transform exists, and use it here
                paireddata.IPairedData _flow_stage_sample = _flow_stage.SamplePairedData(.5);
                paireddata.IPairedData stage_frequency = ff.compose(_flow_stage_sample);
                //compute aep metrics here
                //interior exterior
                //levees
                paireddata.IPairedData _stage_damage_sample = _stage_damage.SamplePairedData(.5);
                paireddata.IPairedData damage_frequency = stage_frequency.compose(_stage_damage_sample);
                double eadEstimate = damage_frequency.integrate();
                meanEad = meanEad +((eadEstimate - meanEad)/i);//probably need to cast i to avoid int division
            }
            return meanEad;
        }
        
    }
}