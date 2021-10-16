using System;
using System.Collections.Generic;
using Statistics;
namespace ead{
    public class Simulation{
        private IDistribution _frequency_flow;
        private paireddata.UncertainPairedData _inflow_outflow;
        private paireddata.UncertainPairedData _flow_stage;
        private paireddata.UncertainPairedData _frequency_stage;
        private paireddata.UncertainPairedData _channelstage_floodplainstage;
        private paireddata.UncertainPairedData _levee_curve;
        private List<paireddata.UncertainPairedData> _damage_category_stage_damage;
        private metrics.IContainResults _results;
        public Simulation(IDistribution frequency_flow, paireddata.UncertainPairedData inflow_outflow, paireddata.UncertainPairedData flow_stage, paireddata.UncertainPairedData channelstage_floodplainstage, paireddata.UncertainPairedData levee_curve, List<paireddata.UncertainPairedData> damage_curves)
        {
            _frequency_flow = frequency_flow;
            _inflow_outflow = inflow_outflow;
            _flow_stage = flow_stage;
            _frequency_stage = new paireddata.UncertainPairedData();//defaults to null
            _channelstage_floodplainstage = channelstage_floodplainstage;
            _levee_curve = levee_curve;
            _damage_category_stage_damage = damage_curves;
        }
        public Simulation(IDistribution frequency_flow, paireddata.UncertainPairedData flow_stage, paireddata.UncertainPairedData channelstage_floodplainstage, paireddata.UncertainPairedData levee_curve, List<paireddata.UncertainPairedData> damage_curves)
        {
            _frequency_flow = frequency_flow;
            _inflow_outflow = new paireddata.UncertainPairedData();//defaults to null
            _flow_stage = flow_stage;
            _frequency_stage = new paireddata.UncertainPairedData();//defaults to null
            _channelstage_floodplainstage = channelstage_floodplainstage;
            _levee_curve = levee_curve;
            _damage_category_stage_damage = damage_curves;
        }
        public Simulation(IDistribution frequency_flow, paireddata.UncertainPairedData flow_stage, paireddata.UncertainPairedData levee_curve, List<paireddata.UncertainPairedData> damage_curves)
        {
            _frequency_flow = frequency_flow;
            _inflow_outflow = new paireddata.UncertainPairedData();//defaults to null
            _flow_stage = flow_stage;
            _frequency_stage = new paireddata.UncertainPairedData();//defaults to null
            _channelstage_floodplainstage = new paireddata.UncertainPairedData();//defaults to null
            _levee_curve = levee_curve;
            _damage_category_stage_damage = damage_curves;
        }
        public Simulation(IDistribution frequency_flow, paireddata.UncertainPairedData flow_stage, List<paireddata.UncertainPairedData> damage_curves)
        {
            _frequency_flow = frequency_flow;
            _inflow_outflow = new paireddata.UncertainPairedData();//defaults to null
            _flow_stage = flow_stage;
            _frequency_stage = new paireddata.UncertainPairedData();//defaults to null
            _channelstage_floodplainstage = new paireddata.UncertainPairedData();//defaults to null
            _levee_curve = new paireddata.UncertainPairedData(); //defaults to null
            _damage_category_stage_damage = damage_curves;
        }
        public Simulation(paireddata.UncertainPairedData frequency_stage, paireddata.UncertainPairedData channelstage_floodplainstage, paireddata.UncertainPairedData levee_curve, List<paireddata.UncertainPairedData> damage_curves)
        {
            _frequency_flow = null;
            _inflow_outflow = new paireddata.UncertainPairedData();//defaults to null
            _flow_stage = new paireddata.UncertainPairedData();//defaults to null
            _frequency_stage = frequency_stage;
            _channelstage_floodplainstage = channelstage_floodplainstage;
            _levee_curve = levee_curve;
            _damage_category_stage_damage = damage_curves;
        }
        public metrics.IContainResults Compute(Int64 seed, Int64 iterations){
            metrics.IContainResults results = new metrics.Results();
            //results.AEPThreshold = 100.0;//stage or flow or damage threshold
            for(int i = 0; i < iterations; i ++){
                if (_frequency_stage.IsNull)
                {
                    //if frequency_flow is not defined throw big errors.
                    paireddata.IPairedData ff = BootstrapToPairedData(_frequency_flow, 1000);//ordinates defines the number of values in the frequency curve, more would be a better approximation.
                    //check if flow transform exists, and use it here
                    if (_inflow_outflow.IsNull)
                    {
                        if (_flow_stage.IsNull)
                        {
                            //complain loudly
                            return _results;
                        }
                        else
                        {
                            paireddata.IPairedData flow_stage_sample = _flow_stage.SamplePairedData(.5);//needs to be a random number
                            paireddata.IPairedData frequency_stage = ff.compose(flow_stage_sample);
                            ComputeFromStageFrequency(frequency_stage);
                        }
 
                    }
                    else
                    {
                        paireddata.IPairedData inflow_outflow_sample = _inflow_outflow.SamplePairedData(.5); //should be a random number
                        paireddata.IPairedData transformff = ff.compose(inflow_outflow_sample);
                            if (_flow_stage.IsNull)
                            {
                            //complain loudly
                            return _results;
                            }
                            else
                            {
                                paireddata.IPairedData flow_stage_sample = _flow_stage.SamplePairedData(.5);//needs to be a random number
                                paireddata.IPairedData frequency_stage = transformff.compose(flow_stage_sample);
                                ComputeFromStageFrequency(frequency_stage);
                            }
                    }

                }
                else
                {
                    paireddata.IPairedData frequency_stage_sample = _frequency_stage.SamplePairedData(.5);
                    ComputeFromStageFrequency(frequency_stage_sample);
                }
            }
            return results;
        }
        private void ComputeFromStageFrequency(paireddata.IPairedData frequency_stage){
            //results.AEPThreshold = 100.0;//stage or flow or damage threshold
            //compute aep metrics here
            double aep = frequency_stage.f(_results.AEPThreshold);
            _results.AddAEPEstimate(aep);
            //interior exterior
            if (_channelstage_floodplainstage.IsNull)
            {
                //levees
                if (_levee_curve.IsNull)
                {
                    double totalEAD = 0.0;
                    foreach(paireddata.UncertainPairedData pd in _damage_category_stage_damage){
                        paireddata.IPairedData _stage_damage_sample = pd.SamplePairedData(.5);//needs to be a random number
                        paireddata.IPairedData frequency_damage = frequency_stage.compose(_stage_damage_sample);
                        double eadEstimate = frequency_damage.integrate();
                        totalEAD += eadEstimate;
                        _results.AddEADEstimate(eadEstimate, pd.Category);
                    }
                    _results.AddEADEstimate(totalEAD, "Total");
                }
                else
                {
                    paireddata.IPairedData _levee_curve_sample = _levee_curve.SamplePairedData(.5); //needs to be a random number
                    paireddata.IPairedData frequency_stage_withLevee = frequency_stage.multiply(_levee_curve_sample);
                    double totalEAD = 0.0;
                    foreach(paireddata.UncertainPairedData pd in _damage_category_stage_damage){
                        paireddata.IPairedData _stage_damage_sample = pd.SamplePairedData(.5);//needs to be a random number
                        paireddata.IPairedData frequency_damage = frequency_stage_withLevee.compose(_stage_damage_sample);
                        double eadEstimate = frequency_damage.integrate();
                        totalEAD += eadEstimate;
                        _results.AddEADEstimate(eadEstimate, pd.Category);
                    }
                    _results.AddEADEstimate(totalEAD, "Total");
                }

            }
            else
            {
                paireddata.IPairedData _channelstage_floodplainstage_sample = _channelstage_floodplainstage.SamplePairedData(.5); //needs to be a random number
                paireddata.IPairedData frequency_floodplainstage = frequency_stage.compose(_channelstage_floodplainstage_sample);
                //levees
                if (_levee_curve.IsNull)
                {
                    double totalEAD = 0.0;
                    foreach(paireddata.UncertainPairedData pd in _damage_category_stage_damage){
                        paireddata.IPairedData _stage_damage_sample = pd.SamplePairedData(.5);//needs to be a random number
                        paireddata.IPairedData frequency_damage = frequency_floodplainstage.compose(_stage_damage_sample);
                        double eadEstimate = frequency_damage.integrate();
                        totalEAD += eadEstimate;
                        _results.AddEADEstimate(eadEstimate, pd.Category);
                    }
                    _results.AddEADEstimate(totalEAD, "Total");
                }
                else
                {
                    paireddata.IPairedData _levee_curve_sample = _levee_curve.SamplePairedData(.5); //needs to be a random number
                    paireddata.IPairedData frequency_floodplainstage_withLevee = frequency_floodplainstage.multiply(_levee_curve_sample);
                    double totalEAD = 0.0;
                    foreach(paireddata.UncertainPairedData pd in _damage_category_stage_damage){
                        paireddata.IPairedData _stage_damage_sample = pd.SamplePairedData(.5);//needs to be a random number
                        paireddata.IPairedData frequency_damage = frequency_floodplainstage_withLevee.compose(_stage_damage_sample);
                        double eadEstimate = frequency_damage.integrate();
                        totalEAD += eadEstimate;
                        _results.AddEADEstimate(eadEstimate, pd.Category);
                    }
                    _results.AddEADEstimate(totalEAD, "Total");
                }

            }
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