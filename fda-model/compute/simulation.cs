using System;
using System.Collections.Generic;
using System.IO;
using Statistics;
using Statistics.Distributions;
using paireddata;
using metrics;
using System.Linq;
using Base.Events;
using System.Threading.Tasks;
using System.Threading;

namespace compute
{
    public class Simulation : Base.Implementations.Validation, Base.Interfaces.IReportMessage, Base.Interfaces.IProgressReport
    {
        private const double THRESHOLD_DAMAGE_PERCENT = 0.05;
        private const double THRESHOLD_DAMAGE_RECURRENCE_INTERVAL = 0.01;
        private const int DEFAULT_THRESHOLD_ID = 0;
        private Statistics.ContinuousDistribution _frequency_flow;
        private GraphicalUncertainPairedData _frequency_flow_graphical;
        private UncertainPairedData _inflow_outflow;
        private UncertainPairedData _flow_stage;
        private GraphicalUncertainPairedData _frequency_stage;
        private UncertainPairedData _channelstage_floodplainstage;
        private UncertainPairedData _levee_curve;
        private double _topOfLeveeElevation;
        private List<UncertainPairedData> _damage_category_stage_damage;
        private Results _results = new Results();
        private bool _leveeIsValid = false;

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
            _frequency_flow_graphical = new GraphicalUncertainPairedData(); //can we have both of these?
            _inflow_outflow = new UncertainPairedData();//defaults to null
            _flow_stage = new UncertainPairedData(); //defaults to null
            _frequency_stage = new GraphicalUncertainPairedData();//defaults to null
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
        public Results Compute(interfaces.IProvideRandomNumbers rp, Statistics.ConvergenceCriteria convergence_criteria, bool computeDefaultThreshold = true, bool giveMeADamageFrequency = false)
        {
            //Validate();
            if (HasErrors)
            {
                if (ErrorLevel >= Base.Enumerations.ErrorLevel.Fatal)
                {
                    ReportMessage(this, new MessageEventArgs(new Base.Implementations.Message("This simulation contains errors. The compute has been aborted.")));
                    return _results;
                }
                else
                {
                    ReportMessage(this, new MessageEventArgs(new Base.Implementations.Message("This simulation contains warnings")));
                }
                //enumerate what the errors and warnings are 
            }
            _leveeIsValid = true;
            int masterseed = 0;
            if (rp is MeanRandomProvider)
            {
                if (convergence_criteria.MaxIterations != 1)
                {
                    ReportMessage(this, new MessageEventArgs(new Base.Implementations.Message("This simulation was requested to provide a mean estimate, but asked for more than one iteration.")));
                    return _results;
                }

            }
            else
            {
                if (convergence_criteria.MinIterations < 100)
                {
                    ReportMessage(this, new MessageEventArgs(new Base.Implementations.Message("This simulation was requested to provide a random estimate, but asked for a minimum of one iteration.")));
                    return _results;
                }
                masterseed = rp.Seed;
            }
            foreach (UncertainPairedData pd in _damage_category_stage_damage)
            {
                _results.ExpectedAnnualDamageResults.AddEADKey(pd.Category, convergence_criteria);
            }
            _results.ExpectedAnnualDamageResults.AddEADKey("Total", convergence_criteria);

            if (computeDefaultThreshold == true)
            {//I am not sure if there is a better way to add the default threshold
                _results.PerformanceByThresholds.AddThreshold(ComputeDefaultThreshold(convergence_criteria));
            }
            SetStageForNonExceedanceProbability(convergence_criteria);
            Int64 progressChunks = 1;
            Int64 _completedIterations = 0;
            Int64 _ExpectedIterations = convergence_criteria.MaxIterations;
            if (_ExpectedIterations > 100)
            {
                progressChunks = _ExpectedIterations / 100;
            }
            Random masterSeedList = new Random(masterseed);//must be seeded.
            int[] seeds = new int[convergence_criteria.MaxIterations];
            for (int i = 0; i < convergence_criteria.MaxIterations; i++)
            {
                seeds[i] = masterSeedList.Next();
            }
            Int64 iterations = convergence_criteria.MinIterations;
            //_leveeIsValid = LeveeIsValid();///this should be integrated into more formal validation routines above.

            while (!_results.IsConverged())
            {
                Parallel.For(0, iterations, i =>
                {
                    //check if it is a mean random provider or not
                    interfaces.IProvideRandomNumbers threadlocalRandomProvider;
                    if (rp is MeanRandomProvider)
                    {
                        threadlocalRandomProvider = new MeanRandomProvider();
                    }
                    else
                    {
                        threadlocalRandomProvider = new RandomProvider(seeds[i]);
                    }
                    if (_frequency_stage.IsNull)
                    {
                        IPairedData frequencyFlow;
                        if (_frequency_flow_graphical.IsNull)
                        {
                            frequencyFlow = BootstrapToPairedData(threadlocalRandomProvider, _frequency_flow, 200);//ordinates defines the number of values in the frequency curve, more would be a better approximation.

                        }
                        else
                        {
                            frequencyFlow = _frequency_flow_graphical.SamplePairedData(threadlocalRandomProvider.NextRandom());
                        }
                        //if frequency_flow is not defined throw big errors.
                        //check if flow transform exists, and use it here
                        if (_inflow_outflow.IsNull)
                        {
                            if (_flow_stage.IsNull)
                            {
                                //complain loudly
                                ReportMessage(this, new MessageEventArgs(new Base.Implementations.Message("Flow stage is Null!!!")));
                                return; //_results;
                            }
                            else
                            {
                                IPairedData flow_stage_sample = _flow_stage.SamplePairedData(threadlocalRandomProvider.NextRandom());
                                IPairedData frequency_stage = flow_stage_sample.compose(frequencyFlow);
                                ComputeFromStageFrequency(threadlocalRandomProvider, frequency_stage, giveMeADamageFrequency, i);
                            }

                        }
                        else
                        {
                            IPairedData inflow_outflow_sample = _inflow_outflow.SamplePairedData(threadlocalRandomProvider.NextRandom()); //should be a random number
                            IPairedData transformff = inflow_outflow_sample.compose(frequencyFlow);
                            if (_flow_stage.IsNull)
                            {
                                //complain loudly
                                ReportMessage(this, new MessageEventArgs(new Base.Implementations.Message("Flow stage is Null!!!")));
                                return;// _results;
                            }
                            else
                            {
                                IPairedData flow_stage_sample = _flow_stage.SamplePairedData(threadlocalRandomProvider.NextRandom());//needs to be a random number
                                IPairedData frequency_stage = flow_stage_sample.compose(transformff);
                                ComputeFromStageFrequency(threadlocalRandomProvider, frequency_stage, giveMeADamageFrequency, i);
                            }
                        }

                    }
                    else
                    {
                        IPairedData frequency_stage_sample = _frequency_stage.SamplePairedData(threadlocalRandomProvider.NextRandom());
                        ComputeFromStageFrequency(threadlocalRandomProvider, frequency_stage_sample, giveMeADamageFrequency, i);
                    }
                    Interlocked.Increment(ref _completedIterations);
                    if (_completedIterations % progressChunks == 0)//need an atomic integer count here.
                    {
                        double percentcomplete = ((double)_completedIterations) / ((double)_ExpectedIterations) * 100;
                        ReportProgress(this, new ProgressReportEventArgs((int)percentcomplete));
                    }

                });
                if (!_results.TestResultsForConvergence(.95, .05))
                {
                    iterations = _results.RemainingIterations(.95, .05);
                    _ExpectedIterations = _completedIterations + iterations;
                    progressChunks = _ExpectedIterations / 100;
                }
                else
                {
                    iterations = 0;
                    break;
                }

            }

            return _results;
        }
        private void ComputeFromStageFrequency(interfaces.IProvideRandomNumbers rp, IPairedData frequency_stage, bool giveMeADamageFrequency, Int64 iteration)
        {

            //interior exterior
            if (_channelstage_floodplainstage.IsNull)
            {
                //levees
                if (_levee_curve.IsNull)
                {
                    ComputeDamagesFromStageFrequency(rp, frequency_stage, giveMeADamageFrequency, iteration);
                    ComputePerformance(frequency_stage, iteration);
                }
                else
                {
                    if (_leveeIsValid)
                    {
                        IPairedData levee_curve_sample = _levee_curve.SamplePairedData(rp.NextRandom()); //needs to be a random number
                        //IPairedData frequency_stage_withLevee = frequency_stage.multiply(levee_curve_sample);
                        ComputeDamagesFromStageFrequency_WithLevee(rp, frequency_stage, levee_curve_sample, giveMeADamageFrequency, iteration);
                        ComputeLeveePerformance(frequency_stage, levee_curve_sample, iteration);
                    }

                }

            }
            else
            {
                IPairedData _channelstage_floodplainstage_sample = _channelstage_floodplainstage.SamplePairedData(rp.NextRandom()); //needs to be a random number
                IPairedData frequency_floodplainstage = _channelstage_floodplainstage_sample.compose(frequency_stage);
                //levees
                if (_levee_curve.IsNull)
                {
                    ComputeDamagesFromStageFrequency(rp, frequency_floodplainstage, giveMeADamageFrequency, iteration);
                    ComputePerformance(frequency_floodplainstage, iteration);
                }
                else
                {
                    if (_leveeIsValid)
                    {
                        IPairedData levee_curve_sample = _levee_curve.SamplePairedData(rp.NextRandom()); //needs to be a random number
                        //IPairedData frequency_floodplainstage_withLevee = frequency_floodplainstage.multiply(_levee_curve_sample);
                        ComputeDamagesFromStageFrequency_WithLevee(rp, frequency_floodplainstage, levee_curve_sample, giveMeADamageFrequency, iteration);
                        ComputeLeveePerformance(frequency_stage, levee_curve_sample, iteration);
                    }

                }

            }
        }
        private IPairedData BootstrapToPairedData(interfaces.IProvideRandomNumbers rp, Statistics.ContinuousDistribution dist, Int64 ordinates)
        {

            double[] samples = rp.NextRandomSequence(dist.SampleSize);
            IDistribution bootstrap = dist.Sample(samples);
            //for (int i = 0; i < dist.SampleSize; i++) samples[i] = Math.Log10(dist.InverseCDF(samples[i]));
            //ISampleStatistics ss = new SampleStatistics(samples);
            double[] x = new double[ordinates];
            double[] y = new double[ordinates];
            //double skewdividedbysix = ss.Skewness / 6.0;
            //double twodividedbyskew = 2.0 / ss.Skewness;
            //double sd = ss.StandardDeviation;
            for (int i = 0; i < ordinates; i++)
            {
                double val = (double)i + .5;
                //equally spaced non-exceedance (cumulative) probabilities in increasing order
                double prob = (val) / ((double)ordinates);
                x[i] = prob;

                //y values in increasing order 
                y[i] = bootstrap.InverseCDF(prob);
                //y[i] =LogPearson3.FastInverseCDF(ss.Mean, sd , ss.Skewness, skewdividedbysix, twodividedbyskew, prob);

            }

            return new PairedData(x, y);

        }
        private void ComputeDamagesFromStageFrequency(interfaces.IProvideRandomNumbers rp, IPairedData frequency_stage, bool giveMeADamageFrequency, Int64 iteration)
        {
            double totalEAD = 0.0;
            CurveMetaData metadata = new CurveMetaData("Total");
            PairedData totalDamageFrequency = new PairedData(null, null, metadata);

            foreach (UncertainPairedData pairedData in _damage_category_stage_damage)
            {
                IPairedData _stage_damage_sample = pairedData.SamplePairedData(rp.NextRandom());//needs to be a random number
                IPairedData frequency_damage = _stage_damage_sample.compose(frequency_stage);
                double eadEstimate = frequency_damage.integrate();
                totalEAD += eadEstimate;
                _results.ExpectedAnnualDamageResults.AddEADEstimate(eadEstimate, pairedData.Category, iteration);

                if (giveMeADamageFrequency)
                {
                    ReportMessage(this, new MessageEventArgs(new FrequencyDamageMessage((PairedData)frequency_damage, frequency_damage.Category)));
                }
            }
            _results.ExpectedAnnualDamageResults.AddEADEstimate(totalEAD, "Total", iteration);
            ReportMessage(this, new MessageEventArgs(new EADMessage(totalEAD)));
            if (giveMeADamageFrequency)
            {
                ReportMessage(this, new MessageEventArgs(new FrequencyDamageMessage(totalDamageFrequency, totalDamageFrequency.Category)));

            }
        }
        private void ComputeDamagesFromStageFrequency_WithLevee(interfaces.IProvideRandomNumbers rp, IPairedData frequency_stage, IPairedData levee, bool giveMeADamageFrequency, Int64 iteration)
        {
            double totalEAD = 0.0;
            CurveMetaData metadata = new CurveMetaData("Total");
            PairedData totalDamageFrequency = new PairedData(null, null, metadata);
            foreach (UncertainPairedData pd in _damage_category_stage_damage)
            {
                IPairedData stage_damage_sample = pd.SamplePairedData(rp.NextRandom());//needs to be a random number
                IPairedData stage_damage_sample_withLevee = stage_damage_sample.multiply(levee);
                IPairedData frequency_damage = stage_damage_sample_withLevee.compose(frequency_stage);
                double eadEstimate = frequency_damage.integrate();
                totalEAD += eadEstimate;
                _results.ExpectedAnnualDamageResults.AddEADEstimate(eadEstimate, pd.Category, iteration);
                if (giveMeADamageFrequency)
                {
                    ComputeTotalDamageFrequency(totalDamageFrequency, (PairedData)frequency_damage);
                    ReportMessage(this, new MessageEventArgs(new FrequencyDamageMessage((PairedData)frequency_damage, frequency_damage.Category)));
                }

            }
            _results.ExpectedAnnualDamageResults.AddEADEstimate(totalEAD, "Total", iteration);
            ReportMessage(this, new MessageEventArgs(new EADMessage(totalEAD)));
            if (giveMeADamageFrequency)
            {
                ReportMessage(this, new MessageEventArgs(new FrequencyDamageMessage(totalDamageFrequency, totalDamageFrequency.Category)));

            }
        }
        //TODO: Review access modifiers. I think most if not all of the performance methods should be private.
        public void ComputePerformance(IPairedData frequency_stage, Int64 iteration)
        {

            foreach (var thresholdEntry in _results.PerformanceByThresholds.ThresholdsDictionary)
            {
                double thresholdValue = thresholdEntry.Value.ThresholdValue;
                double aep = 1 - frequency_stage.f_inverse(thresholdValue);
                thresholdEntry.Value.ProjectPerformanceResults.AddAEPEstimate(aep, iteration);
                GetStageForNonExceedanceProbability(frequency_stage, thresholdEntry.Value, iteration);
            }
        }
        //this method assumes that the levee fragility function spans the entire probability domain 
        public void ComputeLeveePerformance(IPairedData frequency_stage, IPairedData levee_curve_sample, Int64 iteration)
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
                thresholdEntry.Value.ProjectPerformanceResults.AddAEPEstimate(aep, iteration);
                GetStageForNonExceedanceProbability(frequency_stage, thresholdEntry.Value, iteration);
            }

        }

        public void GetStageForNonExceedanceProbability(IPairedData frequency_stage, Threshold threshold, Int64 iteration)
        {
            double[] stageOfEvent = new double[5];
            double[] er101RequiredNonExceedanceProbabilities = new double[] { .9, .98, .99, .996, .998 };
            for (int i = 0; i < er101RequiredNonExceedanceProbabilities.Length; i++)
            {
                stageOfEvent[i] = frequency_stage.f(er101RequiredNonExceedanceProbabilities[i]);
                threshold.ProjectPerformanceResults.AddStageForCNEP(er101RequiredNonExceedanceProbabilities[i], stageOfEvent[i], iteration);
            }
        }
        public void SetStageForNonExceedanceProbability(ConvergenceCriteria c)
        {
            double[] stageOfEvent = new double[5];
            double[] er101RequiredNonExceedanceProbabilities = new double[] { .9, .98, .99, .996, .998 };
            foreach (var thresholdEntry in _results.PerformanceByThresholds.ThresholdsDictionary)
            {
                for (int i = 0; i < er101RequiredNonExceedanceProbabilities.Length; i++)
                {
                    thresholdEntry.Value.ProjectPerformanceResults.AddConditionalNonExceedenceProbabilityKey(er101RequiredNonExceedanceProbabilities[i], c);
                }
            }
        }




        private Threshold ComputeDefaultThreshold(ConvergenceCriteria c)
        {
            MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            IPairedData frequencyStage = new PairedData(null, null);
            CurveMetaData metadata = new CurveMetaData("Total");
            IPairedData frequencyDamage = new PairedData(null, null, metadata);
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
                return new Threshold(DEFAULT_THRESHOLD_ID, c, ThresholdEnum.InteriorStage, thresholdStage);
            }
            else
            {
                return new Threshold(DEFAULT_THRESHOLD_ID, _levee_curve, c, ThresholdEnum.ExteriorStage, _topOfLeveeElevation);
            }
        }

        internal PairedData ComputeTotalStageDamage(List<UncertainPairedData> listOfUncertainPairedData)
        {
            CurveMetaData metadata = new CurveMetaData("Total");
            PairedData totalStageDamage = new PairedData(null, null, metadata);
            MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            foreach (UncertainPairedData uncertainPairedData in listOfUncertainPairedData)
            {
                IPairedData stageDamageSample = uncertainPairedData.SamplePairedData(meanRandomProvider.NextRandom());
                totalStageDamage = totalStageDamage.SumYsForGivenX(stageDamageSample);
            }
            return totalStageDamage;
        }

        internal PairedData ComputeTotalDamageFrequency(PairedData pairedDataTotal, PairedData pairedDataToBeAddedToTotal)
        {
            pairedDataTotal = pairedDataTotal.SumYsForGivenX(pairedDataToBeAddedToTotal);
            return pairedDataTotal;
        }

        public Results PreviewCompute()
        {

            MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            Results results = this.Compute(meanRandomProvider, cc, false, true);
            return results;
        }
        public static SimulationBuilder builder()
        {
            return new SimulationBuilder(new Simulation());
        }

        private bool LeveeIsValid()
        {
            if (_levee_curve.IsNull) return false;
            if (_levee_curve.Yvals.Last().Type != IDistributionEnum.Deterministic)
            {
                ReportMessage(this, new MessageEventArgs(new Base.Implementations.Message("There must exist a stage in the fragilty curve with a certain probability of failure specified as a deterministic distribution")));
                return false;
            }
            else if (_levee_curve.Yvals.Last().InverseCDF(0.5) != 1) //we should be given a deterministic distribution at the end where prob(failure) = 1
            { //the determinstic distribution could be normal with zero standard deviation, triangular or uniform with min and max = 1, doesn't matter
              //distributions where the user specifies zero variability should be passed to the model as a deterministic distribution 
              //this has been communicated 
                ReportMessage(this, new MessageEventArgs(new Base.Implementations.Message("The fragility curve must have stage at which the probability of failure of the levee is 1")));
                return false;
            }
            else
            {   //right here or somewhere we need to do validation to handle a top of levee elevation above all stages 
                //how would that play in with a fragility function?
                //
                TopOfLeveehasCertainFailure();
                return true;
            }
        }

        private void TopOfLeveehasCertainFailure()
        {
            int idx = Array.BinarySearch(_levee_curve.Xvals, _topOfLeveeElevation);
            if (idx > 0)
            {
                if (_levee_curve.Yvals[idx].InverseCDF(0.5) != 1)
                {//top of levee elevation has some probability other than 1
                    ReportMessage(this, new MessageEventArgs(new Base.Implementations.Message($"The top of levee elevation of {_topOfLeveeElevation} in the fragility function does not have a certain probability of failure")));
                }
            }
            else
            {   //top of levee elevation is not included in the fragility curve
                ReportMessage(this, new MessageEventArgs(new Base.Implementations.Message($"The top of levee elevation of {_topOfLeveeElevation} in the fragility function does not have a certain probability of failure")));
            }
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }

        public void ReportProgress(object sender, ProgressReportEventArgs e)
        {
            ProgressReport?.Invoke(sender, e);
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
                _sim.Validate();

                //add validation here to test ranges and domains.
                return _sim;
            }
            public SimulationBuilder withFlowFrequency(Statistics.ContinuousDistribution dist)
            {
                _sim._frequency_flow = dist;
                _sim.AddSinglePropertyRule("flow frequency", new Base.Implementations.Rule(() => { _sim._frequency_flow.Validate(); return !_sim._frequency_flow.HasErrors; }, _sim._frequency_flow.GetErrors().ToString()));
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withFlowFrequency(GraphicalUncertainPairedData gupd)
            {
                _sim._frequency_flow_graphical = gupd;
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withInflowOutflow(UncertainPairedData upd)
            {
                _sim._inflow_outflow = upd;
                _sim.AddSinglePropertyRule("inflow outflow", new Base.Implementations.Rule(() => { _sim._inflow_outflow.Validate(); return !_sim._inflow_outflow.HasErrors; }, _sim._inflow_outflow.GetErrors().ToString()));

                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withFlowStage(UncertainPairedData upd)
            {
                _sim._flow_stage = upd;
                _sim.AddSinglePropertyRule("flow stage", new Base.Implementations.Rule(() => { _sim._flow_stage.Validate(); return !_sim._flow_stage.HasErrors; }, _sim._flow_stage.GetErrors().ToString()));

                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withFrequencyStage(GraphicalUncertainPairedData gupd)
            {
                _sim._frequency_stage = gupd;
                _sim.AddSinglePropertyRule("frequency_stage", new Base.Implementations.Rule(() => { _sim._frequency_stage.Validate(); return !_sim._frequency_stage.HasErrors; }, _sim._frequency_stage.GetErrors().ToString()));
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withInteriorExterior(UncertainPairedData upd)
            {
                _sim._channelstage_floodplainstage = upd;
                _sim.AddSinglePropertyRule("channelstage_floodplainstage", new Base.Implementations.Rule(() => { _sim._channelstage_floodplainstage.Validate(); return !_sim._channelstage_floodplainstage.HasErrors; }, _sim._channelstage_floodplainstage.GetErrors().ToString()));
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withLevee(UncertainPairedData upd, double topOfLeveeElevation)
            {
                _sim.AddSinglePropertyRule("levee", new Base.Implementations.Rule(() => _sim.LeveeIsValid(), "Levee is invalid."));
                _sim._levee_curve = upd;
                _sim._topOfLeveeElevation = topOfLeveeElevation;
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withStageDamages(List<UncertainPairedData> upd)
            {
                _sim._damage_category_stage_damage = upd;
                foreach (UncertainPairedData uncertain in _sim._damage_category_stage_damage)
                {
                    _sim.AddSinglePropertyRule(uncertain.Category + " stage damages", new Base.Implementations.Rule(() => { uncertain.Validate(); return !uncertain.HasErrors; }, uncertain.GetErrors().ToString()));
                }
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