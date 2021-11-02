using System;
using System.Collections.Generic;
using Statistics;
namespace ead{
    public class Simulation {
        private IDistribution _frequency_flow;
        private paireddata.UncertainPairedData _inflow_outflow;
        private paireddata.UncertainPairedData _flow_stage;
        private paireddata.UncertainPairedData _frequency_stage;
        private paireddata.UncertainPairedData _channelstage_floodplainstage;
        private paireddata.UncertainPairedData _levee_curve;
        private List<paireddata.UncertainPairedData> _damage_category_stage_damage;
        private metrics.Results _results = new metrics.Results();

        public metrics.Thresholds PerformanceThresholds
        {
            get
            {
                return _results.Thresholds;
            }
          
        }

        public bool HasLevee
        {
            get
            {
                return !_levee_curve.IsNull;
            }
        }

        public Simulation()
        {
            _frequency_flow = null;
            _inflow_outflow = new paireddata.UncertainPairedData();//defaults to null
            _flow_stage = new paireddata.UncertainPairedData(); //defaults to null
            _frequency_stage = new paireddata.UncertainPairedData();//defaults to null
            _channelstage_floodplainstage = new paireddata.UncertainPairedData();//defaults to null
            _levee_curve = new paireddata.UncertainPairedData(); //defaults to null
            _damage_category_stage_damage = new List<paireddata.UncertainPairedData>();//defaults to empty
        }
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
            _results = new metrics.Results();
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
            _results = new metrics.Results();

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
            //TODO: is there a reason that this constructor does not have a _results object instantiated?
        }
        public metrics.Results Compute(interfaces.IProvideRandomNumbers rp, Int64 iterations){
            //results.AEPThreshold = 100.0;//stage or flow or damage threshold
            for(int i = 0; i < iterations; i ++){
                if (_frequency_stage.IsNull)
                {
                    //if frequency_flow is not defined throw big errors.
                    paireddata.IPairedData ff = BootstrapToPairedData(rp, _frequency_flow, 1000);//ordinates defines the number of values in the frequency curve, more would be a better approximation.
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
                            paireddata.IPairedData flow_stage_sample = _flow_stage.SamplePairedData(rp.NextRandom());
                            paireddata.IPairedData frequency_stage = flow_stage_sample.compose(ff);
                            ComputeFromStageFrequency(rp, frequency_stage);
                        }
 
                    }
                    else
                    {
                        paireddata.IPairedData inflow_outflow_sample = _inflow_outflow.SamplePairedData(rp.NextRandom()); //should be a random number
                        paireddata.IPairedData transformff = inflow_outflow_sample.compose(ff);
                            if (_flow_stage.IsNull)
                            {
                            //complain loudly
                            return _results;
                            }
                            else
                            {
                                paireddata.IPairedData flow_stage_sample = _flow_stage.SamplePairedData(rp.NextRandom());//needs to be a random number
                                paireddata.IPairedData frequency_stage = flow_stage_sample.compose(transformff);
                                ComputeFromStageFrequency(rp, frequency_stage);
                            }
                    }

                }
                else
                {
                    paireddata.IPairedData frequency_stage_sample = _frequency_stage.SamplePairedData(rp.NextRandom());
                    ComputeFromStageFrequency(rp, frequency_stage_sample);
                }
            }
            return _results;
        }
        private void ComputeFromStageFrequency(interfaces.IProvideRandomNumbers rp, paireddata.IPairedData frequency_stage){

            //interior exterior
            if (_channelstage_floodplainstage.IsNull)
            {
                //levees
                if (_levee_curve.IsNull)
                {
                    ComputeDamagesFromStageFrequency(rp, frequency_stage);
                }
                else
                {
                    paireddata.IPairedData levee_curve_sample = _levee_curve.SamplePairedData(rp.NextRandom()); //needs to be a random number
                    //paireddata.IPairedData frequency_stage_withLevee = frequency_stage.multiply(levee_curve_sample);
                    ComputeDamagesFromStageFrequency_WithLevee(rp, frequency_stage, levee_curve_sample);
                }
                ComputePerformance(frequency_stage);
            }
            else
            {
                paireddata.IPairedData _channelstage_floodplainstage_sample = _channelstage_floodplainstage.SamplePairedData(rp.NextRandom()); //needs to be a random number
                paireddata.IPairedData frequency_floodplainstage = _channelstage_floodplainstage_sample.compose(frequency_stage);
                //levees
                if (_levee_curve.IsNull)
                {
                    ComputeDamagesFromStageFrequency(rp, frequency_floodplainstage);
                    ComputePerformance(frequency_floodplainstage);
                }
                else
                {
                    paireddata.IPairedData levee_curve_sample = _levee_curve.SamplePairedData(rp.NextRandom()); //needs to be a random number
                    //paireddata.IPairedData frequency_floodplainstage_withLevee = frequency_floodplainstage.multiply(_levee_curve_sample);
                    ComputeDamagesFromStageFrequency_WithLevee(rp, frequency_floodplainstage, levee_curve_sample);
                    ComputePerformance(frequency_stage);//this is where we need to handle AEP differently 
                }
                
            }
        }
        private paireddata.IPairedData BootstrapToPairedData(interfaces.IProvideRandomNumbers rp, IDistribution dist, Int64 ordinates){
            double[] randyPacket = rp.NextRandomSequence(dist.SampleSize);
            IDistribution bootstrap = dist.Sample(randyPacket);
            double[] x = new double[ordinates];
            double[] y = new double[ordinates];
            for(int i=0;i<ordinates; i++){
                double val = (double) i + .5;
                //equally spaced non-exceedance (cumulative) probabilities in increasing order
                //TODO: did I break something by moving from exceedance probs to non-exceedance probs
                //in the performance compute, I believe I expect an exceedance prob 
                double prob = (val)/((double)ordinates);
                x[i] = prob;
                //y values in increasing order 
                y[i] = bootstrap.InverseCDF(prob);
        
                
            }

            return new paireddata.PairedData(x, y);
        }
        private void ComputeDamagesFromStageFrequency(interfaces.IProvideRandomNumbers rp, paireddata.IPairedData frequency_stage)
        {
            double totalEAD = 0.0;
            foreach(paireddata.UncertainPairedData pd in _damage_category_stage_damage){
                paireddata.IPairedData _stage_damage_sample = pd.SamplePairedData(rp.NextRandom());//needs to be a random number
                paireddata.IPairedData frequency_damage = _stage_damage_sample.compose(frequency_stage);
                //here we need to do something that identifies default threshold 
                double eadEstimate = frequency_damage.integrate();
                totalEAD += eadEstimate;
                _results.ExpectedAnnualDamageResults.AddEADEstimate(eadEstimate, pd.Category);
            }
            _results.ExpectedAnnualDamageResults.AddEADEstimate(totalEAD, "Total");
        }
        private void ComputeDamagesFromStageFrequency_WithLevee(interfaces.IProvideRandomNumbers rp, paireddata.IPairedData frequency_stage, paireddata.IPairedData levee)
        {
            double totalEAD = 0.0;
            foreach (paireddata.UncertainPairedData pd in _damage_category_stage_damage)
            {
                paireddata.IPairedData stage_damage_sample = pd.SamplePairedData(rp.NextRandom());//needs to be a random number
                paireddata.IPairedData stage_damage_sample_withLevee = stage_damage_sample.multiply(levee);
                paireddata.IPairedData frequency_damage = stage_damage_sample_withLevee.compose(frequency_stage);
                double eadEstimate = frequency_damage.integrate();
                totalEAD += eadEstimate;
                _results.ExpectedAnnualDamageResults.AddEADEstimate(eadEstimate, pd.Category);
            }
            _results.ExpectedAnnualDamageResults.AddEADEstimate(totalEAD, "Total");
        }

        public void ComputePerformance(paireddata.IPairedData frequency_stage)
        {
            foreach (var threshold in _results.Thresholds.ListOfThresholds)
            {
                double thresholdValue = threshold.ThresholdValue;
                double aep = 1-frequency_stage.f_inverse(thresholdValue);
                threshold.Performance.AddAEPEstimate(aep);

                double[] stageOfEvent = new double[5];
                double[] er101RequiredExceedanceProbabilities = new double[] { .1, .02, .01, .004, .002 };
                for (int i = 0; i < er101RequiredExceedanceProbabilities.Length; i++)
                {
                    //frequency_stage is non-exceedance probability 
                    stageOfEvent[i] = frequency_stage.f(1-er101RequiredExceedanceProbabilities[i]);
                    threshold.Performance.AddStageForCNEP(er101RequiredExceedanceProbabilities[i], stageOfEvent[i]);
                }
            }
        }

        public paireddata.IPairedData ComputeDamageFrequency(paireddata.IPairedData frequency_stage, paireddata.IPairedData stageDamage)
        {
                       return stageDamage.compose(frequency_stage);
        }
    }
}