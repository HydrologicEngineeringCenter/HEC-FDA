using System;
using System.Collections.Generic;
using System.IO;
using Statistics;
using paireddata;
using metrics;
using System.Linq;
using Base.Events;

namespace compute{
    public class Simulation: Base.Interfaces.IReportMessage, Base.Interfaces.IProgressReport {
        private const double THRESHOLD_DAMAGE_PERCENT = 0.05;
        private const double THRESHOLD_DAMAGE_RECURRENCE_INTERVAL = 0.01;
        private const int DEFAULT_THRESHOLD_ID = 0;
        private IDistribution _frequency_flow;
        private UncertainPairedData _inflow_outflow;
        private UncertainPairedData _flow_stage;
        private UncertainPairedData _frequency_stage;
        private UncertainPairedData _channelstage_floodplainstage;
        private UncertainPairedData _levee_curve;
        private double _topOfLeveeElevation;
        private List<UncertainPairedData> _damage_category_stage_damage;
        private Results _results = new Results();

        public event MessageReportedEventHandler MessageReport;
        public event ProgressReportedEventHandler ProgressReport;

        public bool HasLevee
        {
            get
            {
                return !_levee_curve.IsNull;
            }
        }

        internal Simulation()
        {
            _frequency_flow = null;
            _inflow_outflow = new UncertainPairedData();//defaults to null
            _flow_stage = new UncertainPairedData(); //defaults to null
            _frequency_stage = new UncertainPairedData();//defaults to null
            _channelstage_floodplainstage = new UncertainPairedData();//defaults to null
            _levee_curve = new UncertainPairedData(); //defaults to null
            _damage_category_stage_damage = new List<UncertainPairedData>();//defaults to empty
            _results = new Results();
        }
        /// <summary>
        /// A simulation must be built with a stage damage function for compute default threshold to be true.
        /// </summary>
        /// <param name="rp"></param>
        /// <param name="iterations"></param>
        /// <param name="computeDefaultThreshold"></param>
        /// <returns></returns>
        public Results Compute(interfaces.IProvideRandomNumbers rp, Int64 iterations, bool computeDefaultThreshold = true){
            if (computeDefaultThreshold == true)
            {//I am not sure if there is a better way to add the default threshold
                _results.PerformanceByThresholds.AddThreshold(ComputeDefaultThreshold());
            }
            Int64 progressChunks = 1;
            if (iterations > 100)
            {
                progressChunks = iterations / 100;
            }
            
            for (int i = 0; i < iterations; i ++){
                if (_frequency_stage.IsNull)
                {
                    //if frequency_flow is not defined throw big errors.
                    IPairedData ff = BootstrapToPairedData(rp, _frequency_flow, 1000);//ordinates defines the number of values in the frequency curve, more would be a better approximation.
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
                            IPairedData flow_stage_sample = _flow_stage.SamplePairedData(rp.NextRandom());
                            IPairedData frequency_stage = flow_stage_sample.compose(ff);
                            ComputeFromStageFrequency(rp, frequency_stage);
                        }
 
                    }
                    else
                    {
                        IPairedData inflow_outflow_sample = _inflow_outflow.SamplePairedData(rp.NextRandom()); //should be a random number
                        IPairedData transformff = inflow_outflow_sample.compose(ff);
                            if (_flow_stage.IsNull)
                            {
                            //complain loudly
                            return _results;
                            }
                            else
                            {
                                IPairedData flow_stage_sample = _flow_stage.SamplePairedData(rp.NextRandom());//needs to be a random number
                                IPairedData frequency_stage = flow_stage_sample.compose(transformff);
                                ComputeFromStageFrequency(rp, frequency_stage);
                            }
                    }

                }
                else
                {
                    IPairedData frequency_stage_sample = _frequency_stage.SamplePairedData(rp.NextRandom());
                    ComputeFromStageFrequency(rp, frequency_stage_sample);
                }
                if (i % progressChunks == 0)
                {
                    ReportProgress(this, new ProgressReportEventArgs((int)(i / progressChunks)));
                }
                
            }
            return _results;
        }
        private void ComputeFromStageFrequency(interfaces.IProvideRandomNumbers rp, IPairedData frequency_stage){

            //interior exterior
            if (_channelstage_floodplainstage.IsNull)
            {
                //levees
                if (_levee_curve.IsNull)
                {
                    ComputeDamagesFromStageFrequency(rp, frequency_stage);
                    ComputePerformance(frequency_stage);
                }
                else
                {
                    IPairedData levee_curve_sample = _levee_curve.SamplePairedData(rp.NextRandom()); //needs to be a random number
                    //IPairedData frequency_stage_withLevee = frequency_stage.multiply(levee_curve_sample);
                    ComputeDamagesFromStageFrequency_WithLevee(rp, frequency_stage, levee_curve_sample);
                    ComputeLeveePerformance(frequency_stage, levee_curve_sample);
                }
                
            }
            else
            {
                IPairedData _channelstage_floodplainstage_sample = _channelstage_floodplainstage.SamplePairedData(rp.NextRandom()); //needs to be a random number
                IPairedData frequency_floodplainstage = _channelstage_floodplainstage_sample.compose(frequency_stage);
                //levees
                if (_levee_curve.IsNull)
                {
                    ComputeDamagesFromStageFrequency(rp, frequency_floodplainstage);
                    ComputePerformance(frequency_floodplainstage);
                }
                else
                {
                    IPairedData levee_curve_sample = _levee_curve.SamplePairedData(rp.NextRandom()); //needs to be a random number
                    //IPairedData frequency_floodplainstage_withLevee = frequency_floodplainstage.multiply(_levee_curve_sample);
                    ComputeDamagesFromStageFrequency_WithLevee(rp, frequency_floodplainstage, levee_curve_sample);                  
                    ComputeLeveePerformance(frequency_stage,levee_curve_sample);
                }
                
            }
        }
        private IPairedData BootstrapToPairedData(interfaces.IProvideRandomNumbers rp, IDistribution dist, Int64 ordinates){
            double[] randyPacket = rp.NextRandomSequence(dist.SampleSize);
            IDistribution bootstrap = dist.Sample(randyPacket);
            double[] x = new double[ordinates];
            double[] y = new double[ordinates];
            for(int i=0;i<ordinates; i++){
                double val = (double) i + .5;
                //equally spaced non-exceedance (cumulative) probabilities in increasing order
                double prob = (val)/((double)ordinates);
                x[i] = prob;

                //y values in increasing order 
                y[i] = bootstrap.InverseCDF(prob);
                
            }

            return new PairedData(x, y);
        }
        private void ComputeDamagesFromStageFrequency(interfaces.IProvideRandomNumbers rp, IPairedData frequency_stage)
        {
            double totalEAD = 0.0;
            foreach(UncertainPairedData pd in _damage_category_stage_damage){
                IPairedData _stage_damage_sample = pd.SamplePairedData(rp.NextRandom());//needs to be a random number
                IPairedData frequency_damage = _stage_damage_sample.compose(frequency_stage);
                double eadEstimate = frequency_damage.integrate();
                totalEAD += eadEstimate;
                _results.ExpectedAnnualDamageResults.AddEADEstimate(eadEstimate, pd.Category);
            }
            _results.ExpectedAnnualDamageResults.AddEADEstimate(totalEAD, "Total");
        }
        private void ComputeDamagesFromStageFrequency_WithLevee(interfaces.IProvideRandomNumbers rp, IPairedData frequency_stage, IPairedData levee)
        {
            double totalEAD = 0.0;
            foreach (UncertainPairedData pd in _damage_category_stage_damage)
            {
                IPairedData stage_damage_sample = pd.SamplePairedData(rp.NextRandom());//needs to be a random number
                IPairedData stage_damage_sample_withLevee = stage_damage_sample.multiply(levee);
                IPairedData frequency_damage = stage_damage_sample_withLevee.compose(frequency_stage);
                double eadEstimate = frequency_damage.integrate();
                totalEAD += eadEstimate;
                _results.ExpectedAnnualDamageResults.AddEADEstimate(eadEstimate, pd.Category);
            }
            _results.ExpectedAnnualDamageResults.AddEADEstimate(totalEAD, "Total");
            ReportMessage(this, new MessageEventArgs(new EADMessage(totalEAD)));
        }
        //TODO: Review access modifiers. I think most if not all of the performance methods should be private.
        public void ComputePerformance(IPairedData frequency_stage)
        {

            foreach (var thresholdEntry in _results.PerformanceByThresholds.ThresholdsDictionary)
            {
                double thresholdValue = thresholdEntry.Value.ThresholdValue;
                double aep = 1-frequency_stage.f_inverse(thresholdValue);
                thresholdEntry.Value.ProjectPerformanceResults.AddAEPEstimate(aep);
                ComputeConditionalNonExceedanceProbability(frequency_stage, thresholdEntry.Value);
            }
        }
        //this method assumes that the levee fragility function spans the entire probability domain 
        public void ComputeLeveePerformance(IPairedData frequency_stage, IPairedData levee_curve_sample)
        {
            IPairedData levee_frequency_stage = levee_curve_sample.compose(frequency_stage);
            double aep = 0;
            //extrapolate below
            if (levee_frequency_stage.Xvals[0] != 0)
            {
                double initialProbOfStageInRange = levee_frequency_stage.Xvals[0] - 0;
                double initialProbFailure = (levee_frequency_stage.Yvals[0] + 0) / 2;
                aep += initialProbOfStageInRange * initialProbFailure;
            }
            //within function range
            for (int i = 1; i < levee_frequency_stage.Xvals.Length; i++)
            {
                double probabilityOfStageInRange = levee_frequency_stage.Xvals[i] - levee_frequency_stage.Xvals[i - 1];
                double averageProbFailure = (levee_frequency_stage.Yvals[i] + levee_frequency_stage.Yvals[i - 1]) / 2;
                aep += probabilityOfStageInRange * averageProbFailure;
            }
            //extrapolate above
            double finalProbOfStageInRange = 1 - levee_frequency_stage.Xvals[levee_frequency_stage.Xvals.Length - 1];
            double finalAvgProbFailure = levee_frequency_stage.Yvals[levee_frequency_stage.Yvals.Length - 1];
            aep += finalProbOfStageInRange * finalAvgProbFailure;
            foreach (var thresholdEntry in _results.PerformanceByThresholds.ThresholdsDictionary)
            {
                thresholdEntry.Value.ProjectPerformanceResults.AddAEPEstimate(aep);
                ComputeConditionalNonExceedanceProbability(frequency_stage, thresholdEntry.Value);
            }
            
        }

        public void ComputeConditionalNonExceedanceProbability(IPairedData frequency_stage, Threshold threshold)
        {
            double[] stageOfEvent = new double[5];
            double[] er101RequiredNonExceedanceProbabilities = new double[] { .9, .98, .99, .996, .998 };
            for (int i = 0; i < er101RequiredNonExceedanceProbabilities.Length; i++)
            {
                stageOfEvent[i] = frequency_stage.f(er101RequiredNonExceedanceProbabilities[i]);
                threshold.ProjectPerformanceResults.AddStageForCNEP(er101RequiredNonExceedanceProbabilities[i], stageOfEvent[i]);
            }
        }



        public IPairedData ComputeDamageFrequency(IDistribution flowFrequencyDistribution, UncertainPairedData flowStageUncertain, UncertainPairedData stageDamageUncertain)
        {
            MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            IPairedData frequencyFlow = BootstrapToPairedData(meanRandomProvider, flowFrequencyDistribution, 1000);
            IPairedData ratingCurve = flowStageUncertain.SamplePairedData(meanRandomProvider.NextRandom());
            IPairedData frequencyStage = ratingCurve.compose(frequencyFlow);
            IPairedData stageDamage = stageDamageUncertain.SamplePairedData(meanRandomProvider.NextRandom());
            return stageDamage.compose(frequencyStage);
        }

        private Threshold ComputeDefaultThreshold()
        {
            MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            IPairedData frequencyStage = new PairedData(null,null);
            IPairedData frequencyDamage = new PairedData(null, null, "Total");
            IPairedData totalStageDamage = ComputeTotalStageDamage(_damage_category_stage_damage);
            if (_levee_curve.IsNull)
            {

                if (_frequency_stage.IsNull)
                {
                    IPairedData frequencyFlow = BootstrapToPairedData(meanRandomProvider, _frequency_flow, 1000);
                    if (_inflow_outflow.IsNull)
                    {
                        if (_flow_stage.IsNull)
                        {
                            throw new Exception("A rating curve must accompany a flow-frequency function");
                        }
                        else
                        {
                            IPairedData flowStageSample = _flow_stage.SamplePairedData(meanRandomProvider.NextRandom());
                            frequencyStage = flowStageSample.compose(frequencyFlow);
                        }
                    }
                    else
                    {
                        IPairedData inflowOutflowSample = _inflow_outflow.SamplePairedData(meanRandomProvider.NextRandom());
                        IPairedData transformFlowFrequency = inflowOutflowSample.compose(frequencyFlow);
                        if (_flow_stage.IsNull)
                        {
                            throw new Exception("A rating curve must accompany a flow-frequency function");
                        }
                        else
                        {
                            IPairedData flowStageSample = _flow_stage.SamplePairedData(meanRandomProvider.NextRandom());
                            frequencyStage = flowStageSample.compose(transformFlowFrequency);
                        }
                    }

                }
                else
                {
                    frequencyStage = _frequency_stage.SamplePairedData(meanRandomProvider.NextRandom());
                }

                frequencyDamage = totalStageDamage.compose(frequencyStage);
                double thresholdDamage = THRESHOLD_DAMAGE_PERCENT * frequencyDamage.f(THRESHOLD_DAMAGE_RECURRENCE_INTERVAL);
                double thresholdStage = totalStageDamage.f_inverse(thresholdDamage);
                return new Threshold(DEFAULT_THRESHOLD_ID, ThresholdEnum.InteriorStage, thresholdStage);
            }
            else
            {
                double topOfLevee = FindTopOfLevee(_levee_curve);
                return new Threshold(DEFAULT_THRESHOLD_ID, ThresholdEnum.ExteriorStage, topOfLevee);
            }
        }

        internal double FindTopOfLevee(UncertainPairedData uncertainPairedData)
        {
            MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            List<double> stageList = new List<double>();
            IPairedData leveePairedData = uncertainPairedData.SamplePairedData(meanRandomProvider.NextRandom());
            for (int i=0; i<leveePairedData.Xvals.Length; i++)
            {
                if (leveePairedData.Yvals[i] == 1)
                {
                    stageList.Add(leveePairedData.Xvals[i]);
                }
            }
            if (stageList.Count == 0)
            {
                throw new ArgumentNullException("The levee curve is invalid. The top of levee must have probability = 1");
            }
            return stageList.Min();
        }

        internal PairedData ComputeTotalStageDamage(List<UncertainPairedData> listOfUncertainPairedData)
        {
            PairedData totalStageDamage = new PairedData(null, null, "Total");
            MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            foreach (UncertainPairedData uncertainPairedData in listOfUncertainPairedData)
            {
                IPairedData stageDamageSample = uncertainPairedData.SamplePairedData(meanRandomProvider.NextRandom());
                totalStageDamage = totalStageDamage.SumYsForGivenX(stageDamageSample);
            }
            return totalStageDamage;
        }
        
        public IPairedData ComputeDamageFrequency(IPairedData frequency_stage, IPairedData stageDamage)
        {
            return stageDamage.compose(frequency_stage);
        }
        public static SimulationBuilder builder()
        {
            return new SimulationBuilder(new Simulation());
        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender,e);
        }

        public void ReportProgress(object sender, ProgressReportEventArgs e)
        {
            ProgressReport?.Invoke(sender,e);
        }

        public class SimulationBuilder
        {
            private Simulation _sim;
            internal SimulationBuilder(Simulation sim)
            {
                _sim = sim;
            }
            public Simulation build()
            {

                //probably do validation here.
                return _sim;
            }
            public SimulationBuilder withFlowFrequency(Statistics.IDistribution dist)
            {
                _sim._frequency_flow = dist;
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withInflowOutflow(UncertainPairedData upd)
            {
                _sim._inflow_outflow = upd;
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withFlowStage(UncertainPairedData upd)
            {
                _sim._flow_stage = upd;
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withFrequencyStage(UncertainPairedData upd)
            {
                _sim._frequency_stage = upd;
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withInteriorExterior(UncertainPairedData upd)
            {
                _sim._channelstage_floodplainstage = upd;
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withLevee(UncertainPairedData upd, double topOfLeveeElevation)
            {
                _sim._levee_curve = upd;
                _sim._topOfLeveeElevation = topOfLeveeElevation;
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withStageDamages(List<UncertainPairedData> upd)
            {
                _sim._damage_category_stage_damage = upd;
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withPerformanceMetrics(Results mr)
            {
                _sim._results = mr;
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withAdditionalThreshold(Threshold threshold)
            {
                _sim._results.PerformanceByThresholds.AddThreshold(threshold);
                return new SimulationBuilder(_sim);
            }
        }
    }

}