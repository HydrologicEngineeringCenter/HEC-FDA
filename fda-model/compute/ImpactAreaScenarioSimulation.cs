using System;
using System.Collections.Generic;
using Statistics;
using paireddata;
using metrics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Enumerations;
using interfaces;
using System.Xml.Linq;
using HEC.MVVMFramework.Model.Messaging;
using System.Text;

namespace compute
{
    public class ImpactAreaScenarioSimulation : Validation, IReportMessage, IProgressReport
    {
        private const double THRESHOLD_DAMAGE_PERCENT = 0.05;
        private const double THRESHOLD_DAMAGE_RECURRENCE_INTERVAL = 0.99; //this should be a non-exceedance probability 
        private const int DEFAULT_THRESHOLD_ID = 0;
        private ContinuousDistribution _frequency_discharge;
        private GraphicalUncertainPairedData _frequency_discharge_graphical;
        private UncertainPairedData _unregulated_regulated;
        private UncertainPairedData _discharge_stage;
        private GraphicalUncertainPairedData _frequency_stage;
        private UncertainPairedData _channelstage_floodplainstage;
        private UncertainPairedData _systemResponseFunction_stage_failureProbability;
        private double _topOfLeveeElevation;
        private List<UncertainPairedData> _damage_category_stage_damage;
        private int _impactAreaID;
        private ImpactAreaScenarioResults _impactAreaScenarioResults;
        private bool _leveeIsValid = false;

        public event MessageReportedEventHandler MessageReport;
        public event ProgressReportedEventHandler ProgressReport;
        public bool HasLevee
        {
            get
            {
                return !_systemResponseFunction_stage_failureProbability.CurveMetaData.IsNull;
            }
        }
        public int ImpactAreaID
        {
            get
            {
                return _impactAreaID;
            }
        }
        internal ImpactAreaScenarioSimulation(int impactAreaID)
        {
            _frequency_discharge = null;
            _frequency_discharge_graphical = new GraphicalUncertainPairedData(); //can we have both of these?
            _unregulated_regulated = new UncertainPairedData();//defaults to null
            _discharge_stage = new UncertainPairedData(); //defaults to null
            _frequency_stage = new GraphicalUncertainPairedData();//defaults to null
            _channelstage_floodplainstage = new UncertainPairedData();//defaults to null
            _systemResponseFunction_stage_failureProbability = new UncertainPairedData(); //defaults to null
            _damage_category_stage_damage = new List<UncertainPairedData>();//defaults to empty
            _impactAreaID = impactAreaID;
            _impactAreaScenarioResults = new ImpactAreaScenarioResults(_impactAreaID);
        }
        /// <summary>
        /// A simulation must be built with a stage damage function for compute default threshold to be true.
        /// </summary>
        /// <param name="randomProvider"></param>
        /// <param name="iterations"></param>
        /// <param name="computeDefaultThreshold"></param>
        /// <returns></returns>
        public ImpactAreaScenarioResults Compute(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, bool computeDefaultThreshold = true, bool giveMeADamageFrequency = false)
        {
            //Validate();
            if (!CanCompute(convergenceCriteria,randomProvider))
            {
                return _impactAreaScenarioResults;
            }
            int masterseed = 0;
            if (randomProvider is RandomProvider)
            {
                masterseed = randomProvider.Seed;
            }
            //TODO: levee is valid is not used
            _leveeIsValid = true;
            bool computeWithDamage = true;
            if(_damage_category_stage_damage.Count==0)
            {
                computeWithDamage = false;
            }
            else
            {
                AddEADKeys(convergenceCriteria);
            }
            if (computeDefaultThreshold == true)
            {//I am not sure if there is a better way to add the default threshold
                _impactAreaScenarioResults.PerformanceByThresholds.AddThreshold(ComputeDefaultThreshold(convergenceCriteria, computeWithDamage));
            }
            CreateHistogramsForAssuranceOfThresholds();
            ComputeIterations(convergenceCriteria, randomProvider, masterseed, computeWithDamage, giveMeADamageFrequency);
            _impactAreaScenarioResults.ParallelResultsAreConverged(.95, .05);
            return _impactAreaScenarioResults;
        }

        private void AddEADKeys(ConvergenceCriteria convergenceCriteria)
        {
            foreach (UncertainPairedData uncertainPairedData in _damage_category_stage_damage)
            {
                _impactAreaScenarioResults.ConsequenceResults.AddNewConsequenceResultObject(uncertainPairedData.CurveMetaData.DamageCategory, uncertainPairedData.CurveMetaData.AssetCategory, convergenceCriteria, _impactAreaID);
            }
        }

        private bool CanCompute(ConvergenceCriteria convergenceCriteria, interfaces.IProvideRandomNumbers randomProvider)
        {
            if (HasErrors)
            {
                if (ErrorLevel >= ErrorLevel.Fatal)
                {
                    ReportMessage(this, new MessageEventArgs(new Message($"The simulation for impact area {_impactAreaID} contains errors. The compute has been aborted." + Environment.NewLine)));
                    return false;
                }
                else
                {
                    ReportMessage(this, new MessageEventArgs(new Message($"The simulation for impact area {_impactAreaID} contains warnings:" + Environment.NewLine)));
                }
                //enumerate what the errors and warnings are 
                //TODO: HOW???

                foreach (PropertyRule propertyRule in RuleMap.Values)
                {
                    if (propertyRule.ErrorLevel > ErrorLevel.Unassigned)
                    {
                        foreach (Rule rule in propertyRule.Rules)
                        {
                            if (rule.ErrorLevel > ErrorLevel.Unassigned)
                            {
                                ReportMessage(this, new MessageEventArgs(new Message (rule.Message + Environment.NewLine)));
                            }
                        }
                    }
                }
                //TODO: alternative is to use GetErrors() 
                //StringBuilder stringBuilder = new StringBuilder();
                //foreach (string errorMessage in GetErrors())
                //{
                //    stringBuilder.Append(errorMessage);

                //}
                //ReportMessage(this, new MessageEventArgs(new Message(stringBuilder.ToString())));
            }
            if (randomProvider is MeanRandomProvider)
            {
                if (convergenceCriteria.MaxIterations != 1)
                {
                    string message = $"The simulation for impact area {_impactAreaID} was requested to provide a mean estimate, but asked for more than one iteration." + Environment.NewLine;
                    ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
                    ReportMessage(this, new MessageEventArgs(errorMessage)); return false;

                }
            }
            else
            {
                if (convergenceCriteria.MinIterations < 100)
                {
                    string message = $"The simulation for impact area {_impactAreaID} was requested to provide a random estimate, but asked for a minimum of one iteration." + Environment.NewLine;
                    ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
                    ReportMessage(this, new MessageEventArgs(errorMessage)); return false;

                }
            }
            return true;
        }

        private void ComputeIterations(ConvergenceCriteria convergenceCriteria, IProvideRandomNumbers randomProvider, int masterseed, bool computeWithDamage, bool giveMeADamageFrequency)
        {
            int progressChunks = 1;
            int _completedIterations = 0;
            int _ExpectedIterations = convergenceCriteria.MaxIterations;
            if (_ExpectedIterations > 100)
            {
                progressChunks = _ExpectedIterations / 100;
            }
            Random masterSeedList = new Random(masterseed);//must be seeded.
            int[] seeds = new int[convergenceCriteria.MaxIterations];
            for (int i = 0; i < convergenceCriteria.MaxIterations; i++)
            {
                seeds[i] = masterSeedList.Next();
            }
            int iterations = convergenceCriteria.MinIterations;
            //_leveeIsValid = LeveeIsValid();///this should be integrated into more formal validation routines above.

            while (!_impactAreaScenarioResults.IsConverged(computeWithDamage))
            {
                Parallel.For(0, iterations, i =>
                {
                    //check if it is a mean random provider or not
                    interfaces.IProvideRandomNumbers threadlocalRandomProvider;
                    if (randomProvider is MeanRandomProvider)
                    {
                        threadlocalRandomProvider = new MeanRandomProvider();
                    }
                    else
                    {
                        threadlocalRandomProvider = new RandomProvider(seeds[i]);
                    }
                    if (_frequency_stage.CurveMetaData.IsNull)
                    {
                        IPairedData frequencyDischarge;
                        if (_frequency_discharge_graphical.CurveMetaData.IsNull)
                        {
                            frequencyDischarge = BootstrapToPairedData(threadlocalRandomProvider, _frequency_discharge, 200);//ordinates defines the number of values in the frequency curve, more would be a better approximation.

                        }
                        else
                        {
                            frequencyDischarge = _frequency_discharge_graphical.SamplePairedData(threadlocalRandomProvider.NextRandom());
                        }
                        //if frequency_flow is not defined throw big errors.
                        //check if flow transform exists, and use it here
                        if (_unregulated_regulated.CurveMetaData.IsNull)
                        {
                            if (_discharge_stage.CurveMetaData.IsNull)
                            {
                                //complain loudly
                                string message = $"The stage-discharge function for impact area {_impactAreaID} is null. Compute aborted." + Environment.NewLine;
                                ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
                                ReportMessage(this, new MessageEventArgs(errorMessage));
                                return; 
                            }
                            else
                            {
                                IPairedData discharge_stage_sample = _discharge_stage.SamplePairedData(threadlocalRandomProvider.NextRandom());
                                IPairedData frequency_stage = discharge_stage_sample.compose(frequencyDischarge);
                                ComputeFromStageFrequency(threadlocalRandomProvider, frequency_stage, giveMeADamageFrequency, i, computeWithDamage);
                            }

                        }
                        else
                        {
                            IPairedData inflow_outflow_sample = _unregulated_regulated.SamplePairedData(threadlocalRandomProvider.NextRandom()); //should be a random number
                            IPairedData transformff = inflow_outflow_sample.compose(frequencyDischarge);
                            if (_discharge_stage.CurveMetaData.IsNull)
                            {
                                //complain loudly
                                string message = $"The stage-discharge function for impact area {_impactAreaID} is null. Compute aborted." + Environment.NewLine;
                                ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
                                ReportMessage(this, new MessageEventArgs(errorMessage)); 
                                return;
                            }
                            else
                            {
                                IPairedData discharge_stage_sample = _discharge_stage.SamplePairedData(threadlocalRandomProvider.NextRandom());//needs to be a random number
                                IPairedData frequency_stage = discharge_stage_sample.compose(transformff);
                                ComputeFromStageFrequency(threadlocalRandomProvider, frequency_stage, giveMeADamageFrequency, i, computeWithDamage);
                            }
                        }

                    }
                    else
                    {
                        IPairedData frequency_stage_sample = _frequency_stage.SamplePairedData(threadlocalRandomProvider.NextRandom());
                        ComputeFromStageFrequency(threadlocalRandomProvider, frequency_stage_sample, giveMeADamageFrequency, i, computeWithDamage);
                    }
                    Interlocked.Increment(ref _completedIterations);
                    if (_completedIterations % progressChunks == 0)//need an atomic integer count here.
                    {
                        double percentcomplete = ((double)_completedIterations) / ((double)_ExpectedIterations) * 100;
                        ReportProgress(this, new ProgressReportEventArgs((int)percentcomplete));
                    }

                });
                if (!_impactAreaScenarioResults.ResultsAreConverged(.95, .05, computeWithDamage))
                {//TODO: there is a weird case here - if remaining iterations are small, we divide by zero
                    iterations = _impactAreaScenarioResults.RemainingIterations(.95, .05, computeWithDamage);
                    _ExpectedIterations = _completedIterations + iterations;
                    progressChunks = _ExpectedIterations / 100;
                }
                else
                {
                    ReportMessage(this, new MessageEventArgs(new ComputeCompleteMessage(_completedIterations, _impactAreaID)));
                    iterations = 0;
                    break;
                }

            }
            _impactAreaScenarioResults.ForceDeQueue();
        }

        private void ComputeFromStageFrequency(interfaces.IProvideRandomNumbers randomProvider, IPairedData frequency_stage, bool giveMeADamageFrequency, int iteration, bool computeWithDamage)
        {

            //interior exterior
            if (_channelstage_floodplainstage.CurveMetaData.IsNull)
            {
                //levees
                if (_systemResponseFunction_stage_failureProbability.CurveMetaData.IsNull)
                {
                    if (computeWithDamage)
                    {
                        ComputeDamagesFromStageFrequency(randomProvider, frequency_stage, giveMeADamageFrequency, iteration);
                    }
                    ComputePerformance(frequency_stage, iteration);
                }
                else
                {
                    if (_leveeIsValid)
                    {
                        IPairedData systemResponse_sample = _systemResponseFunction_stage_failureProbability.SamplePairedData(randomProvider.NextRandom()); //needs to be a random number
                        //IPairedData frequency_stage_withLevee = frequency_stage.multiply(levee_curve_sample);
                        if(computeWithDamage)
                        {
                            ComputeDamagesFromStageFrequency_WithLevee(randomProvider, frequency_stage, systemResponse_sample, giveMeADamageFrequency, iteration);
                        }
                        ComputeLeveePerformance(frequency_stage, systemResponse_sample, iteration);
                    }

                }

            }
            else
            {
                IPairedData _channelstage_floodplainstage_sample = _channelstage_floodplainstage.SamplePairedData(randomProvider.NextRandom()); //needs to be a random number
                IPairedData frequency_floodplainstage = _channelstage_floodplainstage_sample.compose(frequency_stage);
                //levees
                if (_systemResponseFunction_stage_failureProbability.CurveMetaData.IsNull)
                {
                    if(computeWithDamage)
                    {
                        ComputeDamagesFromStageFrequency(randomProvider, frequency_floodplainstage, giveMeADamageFrequency, iteration);
                    }
                    ComputePerformance(frequency_floodplainstage, iteration);
                }
                else
                {
                    if (_leveeIsValid)
                    {
                        IPairedData systemResponse_sample = _systemResponseFunction_stage_failureProbability.SamplePairedData(randomProvider.NextRandom()); //needs to be a random number
                        //IPairedData frequency_floodplainstage_withLevee = frequency_floodplainstage.multiply(_levee_curve_sample);
                        if (computeWithDamage)
                        {
                            ComputeDamagesFromStageFrequency_WithLevee(randomProvider, frequency_floodplainstage, systemResponse_sample, giveMeADamageFrequency, iteration);
                        }
                        ComputeLeveePerformance(frequency_stage, systemResponse_sample, iteration);
                    }

                }

            }
        }
        private IPairedData BootstrapToPairedData(IProvideRandomNumbers randomProvider, ContinuousDistribution continuousDistribution, int ordinates)
        {
            //TODO: why is all this commented out code here? 
            double[] samples = randomProvider.NextRandomSequence(continuousDistribution.SampleSize);
            IDistribution bootstrap = continuousDistribution.Sample(samples);
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
        private void ComputeDamagesFromStageFrequency(IProvideRandomNumbers randomProvider, IPairedData frequency_stage, bool giveMeADamageFrequency, int iteration)
        {
            CurveMetaData metadata = new CurveMetaData("Total", "Total");
            PairedData totalDamageFrequency = new PairedData(null, null, metadata);

            foreach (UncertainPairedData pairedData in _damage_category_stage_damage)
            {
                IPairedData _stage_damage_sample = pairedData.SamplePairedData(randomProvider.NextRandom());//needs to be a random number
                IPairedData frequency_damage = _stage_damage_sample.compose(frequency_stage);
                double eadEstimate = frequency_damage.integrate();
                _impactAreaScenarioResults.ConsequenceResults.AddConsequenceRealization(eadEstimate, pairedData.CurveMetaData.DamageCategory, pairedData.CurveMetaData.AssetCategory, _impactAreaID, iteration);

                if (giveMeADamageFrequency)
                {
                    totalDamageFrequency = ComputeTotalDamageFrequency(totalDamageFrequency, (PairedData)frequency_damage);
                }
            }
            if (giveMeADamageFrequency)
            {
                ReportMessage(this, new MessageEventArgs(new FrequencyDamageMessage(totalDamageFrequency)));
            }
        }
        private void ComputeDamagesFromStageFrequency_WithLevee(IProvideRandomNumbers randomProvider, IPairedData frequency_stage, IPairedData systemResponse, bool giveMeADamageFrequency, int iteration)
        {
            //TODO "Total" could be represented as public static const string TOTAL = "Total";
            CurveMetaData metadata = new CurveMetaData("Total", "Total");
            PairedData totalDamageFrequency = new PairedData(null, null, metadata);

            foreach (UncertainPairedData pd in _damage_category_stage_damage)
            {
                IPairedData stage_damage_sample = pd.SamplePairedData(randomProvider.NextRandom());//needs to be a random number
                IPairedData stage_damage_sample_withLevee = stage_damage_sample.multiply(systemResponse);
                IPairedData frequency_damage = stage_damage_sample_withLevee.compose(frequency_stage);
                double eadEstimate = frequency_damage.integrate();
                _impactAreaScenarioResults.ConsequenceResults.AddConsequenceRealization(eadEstimate, pd.CurveMetaData.DamageCategory, pd.CurveMetaData.AssetCategory, _impactAreaID, iteration);
                if (giveMeADamageFrequency)
                {
                    totalDamageFrequency = ComputeTotalDamageFrequency(totalDamageFrequency, (PairedData)frequency_damage);
                }
            }
            if (giveMeADamageFrequency)
            {
                ReportMessage(this, new MessageEventArgs(new FrequencyDamageMessage(totalDamageFrequency)));
            }
        }
        //TODO: Review access modifiers. I think most if not all of the performance methods should be private.
        public void ComputePerformance(IPairedData frequency_stage, int iteration)
        {

            foreach (var thresholdEntry in _impactAreaScenarioResults.PerformanceByThresholds.ListOfThresholds)
            {
                double thresholdValue = thresholdEntry.ThresholdValue;
                double aep = 1 - frequency_stage.f_inverse(thresholdValue);
                thresholdEntry.SystemPerformanceResults.AddAEPForAssurance(aep, iteration);
                GetStageForNonExceedanceProbability(frequency_stage, thresholdEntry, iteration);
            }
        }
        //this method assumes that the levee fragility function spans the entire probability domain 
        //TODO why is this here but levee CNP is in system performance results?
        public void ComputeLeveePerformance(IPairedData frequency_stage, IPairedData levee_curve_sample, int iteration)
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
            foreach (var thresholdEntry in _impactAreaScenarioResults.PerformanceByThresholds.ListOfThresholds)
            {
                thresholdEntry.SystemPerformanceResults.AddAEPForAssurance(aep, iteration);
                GetStageForNonExceedanceProbability(frequency_stage, thresholdEntry, iteration);
            }

        }

        public void GetStageForNonExceedanceProbability(IPairedData frequency_stage, Threshold threshold, int iteration)
        {//TODO: Get rid of these hard coded doubles 
            double[] er101RequiredNonExceedanceProbabilities = new double[] { .9, .96, .98, .99, .996, .998 };
            foreach (double nonExceedanceProbability in er101RequiredNonExceedanceProbabilities)
            {
                double stageOfEvent = frequency_stage.f(nonExceedanceProbability);
                threshold.SystemPerformanceResults.AddStageForAssurance(nonExceedanceProbability, stageOfEvent, iteration);
            }
        }
        public void CreateHistogramsForAssuranceOfThresholds()
        {//TODO: get rid of these hard-coded doubles 
            double[] er101RequiredNonExceedanceProbabilities = new double[] { .9, .96, .98, .99, .996, .998 };
            foreach (var thresholdEntry in _impactAreaScenarioResults.PerformanceByThresholds.ListOfThresholds)
            {
                for (int i = 0; i < er101RequiredNonExceedanceProbabilities.Length; i++)
                {
                    thresholdEntry.SystemPerformanceResults.AddAssuranceHistogram(er101RequiredNonExceedanceProbabilities[i]);
                }
            }
        }




        private Threshold ComputeDefaultThreshold(ConvergenceCriteria convergenceCriteria, bool computeWithDamage)
        {
            MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            IPairedData frequencyStage = new PairedData(null, null);
            IPairedData totalStageDamage = ComputeTotalStageDamage(_damage_category_stage_damage);
            if (_systemResponseFunction_stage_failureProbability.CurveMetaData.IsNull)
            {
                if(_damage_category_stage_damage.Count == 0)
                {
                    double badThresholdStage = 0;
                    string message = "A valid default threshold cannot be calculated. A meaningless default threshold of 0 will be used. Please have an additional threshold for meaningful performance statistics" + Environment.NewLine;
                    ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
                    ReportMessage(this, new MessageEventArgs(errorMessage));
                    return new Threshold(DEFAULT_THRESHOLD_ID, convergenceCriteria, ThresholdEnum.InteriorStage, badThresholdStage);
                }

                if (_frequency_stage.CurveMetaData.IsNull)
                {
                    IPairedData frequencyFlow;
                    if (_frequency_discharge_graphical.CurveMetaData.IsNull)
                    {
                        frequencyFlow = BootstrapToPairedData(meanRandomProvider, _frequency_discharge, 200);
                    }
                    else
                    {
                        frequencyFlow = _frequency_discharge_graphical.SamplePairedData(meanRandomProvider.NextRandom());
                    }
                    if (_unregulated_regulated.CurveMetaData.IsNull)
                    {
                        if (_discharge_stage.CurveMetaData.IsNull)
                        {
                            double badThresholdStage = 0;
                            string message = "A rating curve must accompany a flow-frequency function. An arbitrary threshold is being used." + Environment.NewLine;
                            ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
                            ReportMessage(this, new MessageEventArgs(errorMessage)); 
                            return new Threshold(DEFAULT_THRESHOLD_ID, convergenceCriteria, ThresholdEnum.InteriorStage, badThresholdStage);

                        }
                        else
                        {
                            IPairedData flowStageSample = _discharge_stage.SamplePairedData(meanRandomProvider.NextRandom());
                            frequencyStage = flowStageSample.compose(frequencyFlow);
                        }
                    }
                    else
                    {
                        IPairedData inflowOutflowSample = _unregulated_regulated.SamplePairedData(meanRandomProvider.NextRandom());
                        IPairedData transformFlowFrequency = inflowOutflowSample.compose(frequencyFlow);
                        if (_discharge_stage.CurveMetaData.IsNull)
                        {
                            double badThresholdStage = 0;
                            string message = "A rating curve must accompany a flow-frequency function. An arbitrary threshold is being used." + Environment.NewLine;
                            ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
                            ReportMessage(this, new MessageEventArgs(errorMessage));
                            return new Threshold(DEFAULT_THRESHOLD_ID, convergenceCriteria, ThresholdEnum.InteriorStage, badThresholdStage);
                        }
                        else
                        {
                            IPairedData flowStageSample = _discharge_stage.SamplePairedData(meanRandomProvider.NextRandom());
                            frequencyStage = flowStageSample.compose(transformFlowFrequency);
                        }
                    }

                }
                else
                {
                    frequencyStage = _frequency_stage.SamplePairedData(meanRandomProvider.NextRandom());
                }

                IPairedData frequencyDamage = totalStageDamage.compose(frequencyStage);
                double thresholdDamage = THRESHOLD_DAMAGE_PERCENT * frequencyDamage.f(THRESHOLD_DAMAGE_RECURRENCE_INTERVAL);
                double thresholdStage = totalStageDamage.f_inverse(thresholdDamage);
                return new Threshold(DEFAULT_THRESHOLD_ID, convergenceCriteria, ThresholdEnum.InteriorStage, thresholdStage);
            }
            else
            {
                return new Threshold(DEFAULT_THRESHOLD_ID, _systemResponseFunction_stage_failureProbability, convergenceCriteria, ThresholdEnum.ExteriorStage, _topOfLeveeElevation);
            }
        }

        internal PairedData ComputeTotalStageDamage(List<UncertainPairedData> listOfUncertainPairedData)
        {
            CurveMetaData metadata = new CurveMetaData("Total", "Total");
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

        public ImpactAreaScenarioResults PreviewCompute()
        {

            MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            ImpactAreaScenarioResults results = this.Compute(meanRandomProvider, convergenceCriteria, false, true);
            return results;
        }
        public static SimulationBuilder builder(int impactAreaID)
        {
            return new SimulationBuilder(new ImpactAreaScenarioSimulation(impactAreaID));
        }

        private bool LeveeIsValid()
        {
            if (_systemResponseFunction_stage_failureProbability.CurveMetaData.IsNull) return false;
            if (_systemResponseFunction_stage_failureProbability.Yvals.Last().Type != IDistributionEnum.Deterministic)
            {
                string message = "There must exist a stage in the fragility curve with a certain probability of failure specified as a deterministic distribution" + Environment.NewLine;
                ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage));
                return false;
            }
            else if (_systemResponseFunction_stage_failureProbability.Yvals.Last().InverseCDF(0.5) != 1) //we should be given a deterministic distribution at the end where prob(failure) = 1
            { //the determinstic distribution could be normal with zero standard deviation, triangular or uniform with min and max = 1, doesn't matter
              //distributions where the user specifies zero variability should be passed to the model as a deterministic distribution 
              //this has been communicated 
                string message = "There must exist a stage in the fragility curve with a certain probability of failure specified as a deterministic distribution" + Environment.NewLine;
                ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage)); return false;
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
            int index = Array.BinarySearch(_systemResponseFunction_stage_failureProbability.Xvals, _topOfLeveeElevation);
            if (index > 0)
            {
                if (_systemResponseFunction_stage_failureProbability.Yvals[index].InverseCDF(0.5) != 1)
                {//top of levee elevation has some probability other than 1
                    string message = $"The top of levee elevation of {_topOfLeveeElevation} in the fragility function certain probability of failure specified as a deterministic distribution" + Environment.NewLine;
                    ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Major);
                    ReportMessage(this, new MessageEventArgs(errorMessage));
                }
            }
            else
            {   //top of levee elevation is not included in the fragility curve
                string message = $"The top of levee elevation of {_topOfLeveeElevation} in the fragility function certain probability of failure specified as a deterministic distribution" + Environment.NewLine;
                ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Major);
                ReportMessage(this, new MessageEventArgs(errorMessage));
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
        public bool Equals(ImpactAreaScenarioSimulation incomingImpactAreaScenarioSimulation)
        {
            bool sameLeveeElevation = _topOfLeveeElevation.Equals(incomingImpactAreaScenarioSimulation._topOfLeveeElevation);
            if (!sameLeveeElevation)
            {
                return false;
            }
            bool sameImpactArea = _impactAreaID.Equals(incomingImpactAreaScenarioSimulation._impactAreaID);
            if (!sameImpactArea)
            {
                return false;
            }
            bool leveeValidityMatches = _leveeIsValid.Equals(incomingImpactAreaScenarioSimulation._leveeIsValid);
            if (!leveeValidityMatches)
            {
                return false;
            }
            bool frequenceDischargeMatches = _frequency_discharge.Equals(incomingImpactAreaScenarioSimulation._frequency_discharge);
            if (!frequenceDischargeMatches)
            {
                return false;
            }
            bool frequencyDischargeGraphicalMatches = _frequency_discharge_graphical.Equals(incomingImpactAreaScenarioSimulation._frequency_discharge_graphical);
            if (!frequencyDischargeGraphicalMatches)
            {
                return false;
            }
            bool regulatedUnregulatedMatches = _unregulated_regulated.Equals(incomingImpactAreaScenarioSimulation._unregulated_regulated);
            if (!regulatedUnregulatedMatches)
            {
                return false;
            }
            bool dischargeStageMatches = _discharge_stage.Equals(incomingImpactAreaScenarioSimulation._discharge_stage);
            if (!dischargeStageMatches)
            {
                return false;
            }
            bool frequencyStageMatches = _frequency_stage.Equals(incomingImpactAreaScenarioSimulation._frequency_stage);
            if (!frequencyStageMatches)
            {
                return false;
            }
            bool interiorExteriorMatches = _channelstage_floodplainstage.Equals(incomingImpactAreaScenarioSimulation._channelstage_floodplainstage);
            if (!interiorExteriorMatches)
            {
                return false;
            }
            bool systemResponseMatches = _systemResponseFunction_stage_failureProbability.Equals(incomingImpactAreaScenarioSimulation._systemResponseFunction_stage_failureProbability);
            if (!systemResponseMatches)
            {
                return false;
            }
            bool resultsMatch = _impactAreaScenarioResults.Equals(incomingImpactAreaScenarioSimulation._impactAreaScenarioResults);
            if (!resultsMatch)
            {
                return false;
            }
            foreach (UncertainPairedData stageDamage in _damage_category_stage_damage)
            {
                foreach (UncertainPairedData incomingStageDamage in incomingImpactAreaScenarioSimulation._damage_category_stage_damage)
                {
                    if ((stageDamage.CurveMetaData.DamageCategory.Equals(incomingStageDamage.CurveMetaData.DamageCategory)) && (stageDamage.CurveMetaData.AssetCategory.Equals(incomingStageDamage.CurveMetaData.AssetCategory)))
                    {
                        bool stageDamagesMatch = stageDamage.Equals(incomingStageDamage);
                        if(!stageDamagesMatch)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public XElement WriteToXML()
        {
            XElement mainElement = new XElement("ImpactAreaScenarioSimulation");

            mainElement.SetAttributeValue("LeveeIsValid", _leveeIsValid);
            mainElement.SetAttributeValue("TopOfLeveeElevation", _topOfLeveeElevation);
            mainElement.SetAttributeValue("ImpactAreaID", _impactAreaID);
            bool frequencyDischargeIsNull = ((Statistics.Distributions.LogPearson3)_frequency_discharge).IsNull;
            mainElement.SetAttributeValue("FrequencyDischargeIsNull", frequencyDischargeIsNull);
            if (!frequencyDischargeIsNull)
            {
                XElement frequenceDischarge = _frequency_discharge.ToXML();
                frequenceDischarge.Name = "LogPearson3";
                mainElement.Add(frequenceDischarge);
            }

            XElement frequencyDischargeGraphical = _frequency_discharge_graphical.WriteToXML();
            frequencyDischargeGraphical.Name = "FrequencyDischargeGraphical";
            XElement regulatedUnregulated = _unregulated_regulated.WriteToXML();
            regulatedUnregulated.Name = "UnregulatedRegulated";
            XElement dischargeStage = _discharge_stage.WriteToXML();
            dischargeStage.Name = "DischargeStage";
            XElement frequencyStage = _frequency_stage.WriteToXML();
            frequencyStage.Name = "FrequencyStage";
            XElement interiorExterior = _channelstage_floodplainstage.WriteToXML();
            interiorExterior.Name = "InteriorExterior";
            XElement systemResponse = _systemResponseFunction_stage_failureProbability.WriteToXML();
            systemResponse.Name = "SystemResponse";
            XElement impactAreaScenarioResults = _impactAreaScenarioResults.WriteToXml();
            impactAreaScenarioResults.Name = "ImpactAreaScenarioResults";
            XElement stageDamageList = new XElement("stageDamageList");

            foreach (UncertainPairedData stageDamage in _damage_category_stage_damage)
            {
                XElement stageDamageElement = stageDamage.WriteToXML();
                stageDamageList.Add(stageDamageElement);
            }

            mainElement.Add(frequencyDischargeGraphical);
            mainElement.Add(regulatedUnregulated);
            mainElement.Add(dischargeStage);
            mainElement.Add(frequencyStage);
            mainElement.Add(interiorExterior);
            mainElement.Add(systemResponse);
            mainElement.Add(impactAreaScenarioResults);
            mainElement.Add(stageDamageList);

            return mainElement;
        }
        public static ImpactAreaScenarioSimulation ReadFromXML(XElement xElement)
        {
            bool frequencyDischargeIsNull = Convert.ToBoolean(xElement.Attribute("FrequencyDischargeIsNull").Value);
            ContinuousDistribution frequencyDischarge;
            if (!frequencyDischargeIsNull)
            {
                frequencyDischarge = (ContinuousDistribution)ContinuousDistribution.FromXML(xElement.Element("LogPearson3"));
            }
            else
            {
                frequencyDischarge = new Statistics.Distributions.LogPearson3();
            }
            GraphicalUncertainPairedData frequencyDischargeGraphical = GraphicalUncertainPairedData.ReadFromXML(xElement.Element("FrequencyDischargeGraphical"));
            UncertainPairedData regulatedUnregulated = UncertainPairedData.ReadFromXML(xElement.Element("UnregulatedRegulated"));
            UncertainPairedData stageDischarge = UncertainPairedData.ReadFromXML(xElement.Element("DischargeStage"));
            GraphicalUncertainPairedData frequencyStage = GraphicalUncertainPairedData.ReadFromXML(xElement.Element("FrequencyStage"));
            UncertainPairedData interiorExterior = UncertainPairedData.ReadFromXML(xElement.Element("InteriorExterior"));
            UncertainPairedData systemResponse = UncertainPairedData.ReadFromXML(xElement.Element("SystemResponse"));
            IContainImpactAreaScenarioResults impactAreaScenarioResults = ImpactAreaScenarioResults.ReadFromXML(xElement.Element("ImpactAreaScenarioResults"));
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
            foreach (XElement stageDamageElement in xElement.Element("stageDamageList").Elements())
            {
                UncertainPairedData stageDamage = UncertainPairedData.ReadFromXML(stageDamageElement);
                stageDamageList.Add(stageDamage);
            }

            bool leveeIsValid = Convert.ToBoolean(xElement.Attribute("LeveeIsValid").Value);
            double topOfLeveeElevation = Convert.ToDouble(xElement.Attribute("TopOfLeveeElevation").Value);
            int impactAreaID = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);

            ImpactAreaScenarioSimulation impactAreaScenarioSimulation = ImpactAreaScenarioSimulation.builder(impactAreaID)
                .withFlowFrequency(frequencyDischarge)
                .withFlowFrequency(frequencyDischargeGraphical)
                .withInflowOutflow(regulatedUnregulated)
                .withFlowStage(stageDischarge)
                .withLevee(systemResponse, topOfLeveeElevation)
                .withStageDamages(stageDamageList)
                .withFrequencyStage(frequencyStage)
                .withInteriorExterior(interiorExterior)
                .build();
            impactAreaScenarioSimulation._leveeIsValid = leveeIsValid;
            impactAreaScenarioSimulation._impactAreaScenarioResults = (ImpactAreaScenarioResults)impactAreaScenarioResults;
            return impactAreaScenarioSimulation;

        }
        public class SimulationBuilder
        {
            private ImpactAreaScenarioSimulation _sim;
            internal SimulationBuilder(ImpactAreaScenarioSimulation sim)
            {
                _sim = sim;
            }
            public ImpactAreaScenarioSimulation build()
            {
                _sim.Validate();

                //add validation here to test ranges and domains.
                return _sim;
            }
            public SimulationBuilder withFlowFrequency(ContinuousDistribution continuousDistribution)
            {   //TODO: I do not think the sample size validation works
                _sim._frequency_discharge = continuousDistribution;
                _sim.AddSinglePropertyRule("flow frequency", new Rule(() => { _sim._frequency_discharge.Validate(); return !_sim._frequency_discharge.HasErrors; }, _sim._frequency_discharge.GetErrors().ToString()));
                bool frequencyCurveSampleIsAtLeastTwo = _sim._frequency_discharge.SampleSize > 2;
                _sim.AddSinglePropertyRule("FlowFrequency", new Rule(() => { return frequencyCurveSampleIsAtLeastTwo; }, "Frequency function has a sample size less than two", HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Severe));
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withFlowFrequency(GraphicalUncertainPairedData graphicalUncertainPairedData)
            {
                _sim._frequency_discharge_graphical = graphicalUncertainPairedData;
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withInflowOutflow(UncertainPairedData uncertainPairedData)
            {
                _sim._unregulated_regulated = uncertainPairedData;
                _sim.AddSinglePropertyRule("inflow outflow", new Rule(() => { _sim._unregulated_regulated.Validate(); return !_sim._unregulated_regulated.HasErrors; }, _sim._unregulated_regulated.GetErrors().ToString()));

                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withFlowStage(UncertainPairedData uncertainPairedData)
            {
                _sim._discharge_stage = uncertainPairedData;
                _sim.AddSinglePropertyRule("flow stage", new Rule(() => { _sim._discharge_stage.Validate(); return !_sim._discharge_stage.HasErrors; }, _sim._discharge_stage.GetErrors().ToString()));

                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withFrequencyStage(GraphicalUncertainPairedData graphicalUncertainPairedData)
            {
                _sim._frequency_stage = graphicalUncertainPairedData;
                _sim.AddSinglePropertyRule("frequency_stage", new Rule(() => { _sim._frequency_stage.Validate(); return !_sim._frequency_stage.HasErrors; }, _sim._frequency_stage.GetErrors().ToString()));
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withInteriorExterior(UncertainPairedData uncertainPairedData)
            {
                _sim._channelstage_floodplainstage = uncertainPairedData;
                _sim.AddSinglePropertyRule("channelstage_floodplainstage", new Rule(() => { _sim._channelstage_floodplainstage.Validate(); return !_sim._channelstage_floodplainstage.HasErrors; }, _sim._channelstage_floodplainstage.GetErrors().ToString()));
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withLevee(UncertainPairedData uncertainPairedData, double topOfLeveeElevation)
            {
                _sim.AddSinglePropertyRule("levee", new Rule(() => _sim.LeveeIsValid(), "Levee is invalid."));
                _sim._systemResponseFunction_stage_failureProbability = uncertainPairedData;
                _sim._topOfLeveeElevation = topOfLeveeElevation;
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withStageDamages(List<UncertainPairedData> uncertainPairedDataList)
            {//TODO: I do not think the positive damage validation works.
                _sim._damage_category_stage_damage = uncertainPairedDataList;
                foreach (UncertainPairedData uncertainPairedData in _sim._damage_category_stage_damage)
                {
                    _sim.AddSinglePropertyRule(uncertainPairedData.CurveMetaData.DamageCategory + " stage damages", new Rule(() => { uncertainPairedData.Validate(); return !uncertainPairedData.HasErrors; }, uncertainPairedData.GetErrors().ToString()));
                    double median = uncertainPairedData.Yvals[uncertainPairedData.Yvals.Length - 1].InverseCDF(0.5);
                    bool stageDamageIsPositive = median > 0;
                    _sim.AddSinglePropertyRule("PositiveDamage", new Rule(() => { return stageDamageIsPositive; }, "Stage-damage reflects 0 damage for highest stage", HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Severe));
                }
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withPerformanceMetrics(ImpactAreaScenarioResults results)
            {
                _sim._impactAreaScenarioResults = results;
                return new SimulationBuilder(_sim);
            }
            public SimulationBuilder withAdditionalThreshold(Threshold threshold)
            {
                _sim._impactAreaScenarioResults.PerformanceByThresholds.AddThreshold(threshold);
                return new SimulationBuilder(_sim);
            }

            public SimulationBuilder forImpactArea(int impactAreaID)
            {
                _sim._impactAreaID = impactAreaID;
                return new SimulationBuilder(_sim);
            }
        }

    }

}