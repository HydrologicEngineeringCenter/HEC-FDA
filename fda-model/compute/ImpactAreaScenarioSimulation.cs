using System;
using System.Collections.Generic;
using Statistics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Enumerations;
using System.Xml.Linq;
using HEC.MVVMFramework.Model.Messaging;
using System.Text;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.interfaces;

namespace HEC.FDA.Model.compute
{
    public class ImpactAreaScenarioSimulation : Validation, IReportMessage, IProgressReport
    {
        public const int IMPACT_AREA_SIM_COMPLETED = -1001;

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
        private double[] _RequiredExceedanceProbabilities = { 0.99900, 0.99000, 0.95000, 0.90000, 0.85000, 0.80000, 0.75000, 0.70000, 0.65000, 0.60000, 0.55000, 0.50000, 0.47500, 0.45000, 0.42500, 0.40000, 0.37500, 0.35000, 0.32500, 0.30000, 0.29000, 0.28000, 0.27000, 0.26000, 0.25000, 0.24000, 0.23000, 0.22000, 0.21000, 0.20000, 0.19500, 0.19000, 0.18500, 0.18000, 0.17500, 0.17000, 0.16500, 0.16000, 0.15500, 0.15000, 0.14500, 0.14000, 0.13500, 0.13000, 0.12500, 0.12000, 0.11500, 0.11000, 0.10500, 0.10000, 0.09500, 0.09000, 0.08500, 0.08000, 0.07500, 0.07000, 0.06500, 0.06000, 0.05900, 0.05800, 0.05700, 0.05600, 0.05500, 0.05400, 0.05300, 0.05200, 0.05100, 0.05000, 0.04900, 0.04800, 0.04700, 0.04600, 0.04500, 0.04400, 0.04300, 0.04200, 0.04100, 0.04000, 0.03900, 0.03800, 0.03700, 0.03600, 0.03500, 0.03400, 0.03300, 0.03200, 0.03100, 0.03000, 0.02900, 0.02800, 0.02700, 0.02600, 0.02500, 0.02400, 0.02300, 0.02200, 0.02100, 0.02000, 0.01950, 0.01900, 0.01850, 0.01800, 0.01750, 0.01700, 0.01650, 0.01600, 0.01550, 0.01500, 0.01450, 0.01400, 0.01350, 0.01300, 0.01250, 0.01200, 0.01150, 0.01100, 0.01050, 0.01000, 0.00950, 0.00900, 0.00850, 0.00800, 0.00750, 0.00700, 0.00650, 0.00600, 0.00550, 0.00500, 0.00490, 0.00450, 0.00400, 0.00350, 0.00300, 0.00250, 0.00200, 0.00195, 0.00190, 0.00185, 0.00180, 0.00175, 0.00170, 0.00165, 0.00160, 0.00155, 0.00150, 0.00145, 0.00140, 0.00135, 0.00130, 0.00125, 0.00120, 0.00115, 0.00110, 0.00105, 0.00100, 0.00095, 0.00090, 0.00085, 0.00080, 0.00075, 0.00070, 0.00065, 0.00060, 0.00055, 0.00050, 0.00045, 0.00040, 0.00035, 0.00030, 0.00025, 0.00020, 0.00015, 0.00010 };

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
            MessageHub.Register(this);
        }
        /// <summary>
        /// A simulation must be built with a stage damage function for compute default threshold to be true.
        /// </summary>
        /// <param name="randomProvider"></param>
        /// <param name="iterations"></param>
        /// <param name="computeDefaultThreshold"></param>
        /// <returns></returns>
        public ImpactAreaScenarioResults Compute(IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, bool computeDefaultThreshold = true, bool giveMeADamageFrequency = false, bool computeIsDeterministic = false)
        {
            //Validate();
            if (!CanCompute(convergenceCriteria, randomProvider))
            {
                _impactAreaScenarioResults = new ImpactAreaScenarioResults(_impactAreaID, true);
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
            if (_damage_category_stage_damage.Count == 0)
            {
                computeWithDamage = false;
            }
            else
            {
                CreateEADHistograms(convergenceCriteria);
            }
            if (computeDefaultThreshold == true)
            {//I am not sure if there is a better way to add the default threshold
                _impactAreaScenarioResults.PerformanceByThresholds.AddThreshold(ComputeDefaultThreshold(convergenceCriteria, computeWithDamage, computeIsDeterministic: true));
            }
            CreateHistogramsForAssuranceOfThresholds();
            MessageEventArgs beginComputeMessageArgs = new MessageEventArgs(new Message($"EAD and performance compute for the impact area with ID {_impactAreaID} has been initiated"));
            ReportMessage(this, beginComputeMessageArgs);
            ComputeIterations(convergenceCriteria, randomProvider, masterseed, computeWithDamage, giveMeADamageFrequency, computeIsDeterministic);
            _impactAreaScenarioResults.ParallelResultsAreConverged(.95, .05);
            MessageEventArgs endComputeMessageArgs = new MessageEventArgs(new Message($"EAD and performance compute for the impact area with ID {_impactAreaID} has completed"));
            ReportMessage(this, endComputeMessageArgs);
            return _impactAreaScenarioResults;
        }

        private void CreateEADHistograms(ConvergenceCriteria convergenceCriteria)
        {
            foreach (UncertainPairedData uncertainPairedData in _damage_category_stage_damage)
            {
                bool histogramIsZeroValued = false;
                double largeProbability = 0.999;
                double highPercentile = uncertainPairedData.Yvals[uncertainPairedData.Yvals.Length - 1].InverseCDF(largeProbability);
                if (highPercentile == 0)
                {
                    histogramIsZeroValued = true;
                }
                _impactAreaScenarioResults.ConsequenceResults.AddNewConsequenceResultObject(uncertainPairedData.CurveMetaData.DamageCategory, uncertainPairedData.CurveMetaData.AssetCategory, convergenceCriteria, _impactAreaID, histogramIsZeroValued);
            }
        }



        private bool CanCompute(ConvergenceCriteria convergenceCriteria, IProvideRandomNumbers randomProvider)
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
                    ReportMessage(this, new MessageEventArgs(new ErrorMessage($"The simulation for impact area {_impactAreaID} contains warnings:" + Environment.NewLine, ErrorLevel.Major)));
                }
                //enumerate what the errors and warnings are 
                StringBuilder errors = new StringBuilder();
                if (_frequency_discharge != null && _frequency_discharge.HasErrors)
                {
                    errors.AppendLine(nameof(_frequency_discharge) + $" has the following messages for the impact area with ID {_impactAreaID}:");
                    foreach (string s in _frequency_discharge.GetErrors())
                    {
                        errors.AppendLine(s);
                    }

                }
                if (!_frequency_discharge_graphical.IsNull && _frequency_discharge_graphical.HasErrors)
                {
                    errors.AppendLine(nameof(_frequency_discharge_graphical) + $" has the following messages for the impact area with ID {_impactAreaID}:");
                    foreach (string s in _frequency_discharge_graphical.GetErrors())
                    {
                        errors.AppendLine(s);
                    }

                }
                if (!_unregulated_regulated.IsNull && _unregulated_regulated.HasErrors)
                {
                    errors.AppendLine(nameof(_unregulated_regulated) + $" has the following messages for the impact area with ID {_impactAreaID}:");
                    foreach (string s in _unregulated_regulated.GetErrors())
                    {
                        errors.AppendLine(s);
                    }

                }
                if (!_discharge_stage.IsNull && _discharge_stage.HasErrors)
                {
                    errors.AppendLine(nameof(_discharge_stage) + $" has the following messages for the impact area with ID {_impactAreaID}:");
                    foreach (string s in _discharge_stage.GetErrors())
                    {
                        errors.AppendLine(s);
                    }

                }
                if (!_frequency_stage.IsNull && _frequency_stage.HasErrors)
                {
                    errors.AppendLine(nameof(_frequency_stage) + $" has the following messages for the impact area with ID {_impactAreaID}:");
                    foreach (string s in _frequency_stage.GetErrors())
                    {
                        errors.AppendLine(s);
                    }

                }
                if (!_channelstage_floodplainstage.IsNull && _channelstage_floodplainstage.HasErrors)
                {
                    errors.AppendLine(nameof(_channelstage_floodplainstage) + $" has the following messages for the impact area with ID {_impactAreaID}:");
                    foreach (string s in _channelstage_floodplainstage.GetErrors())
                    {
                        errors.AppendLine(s);
                    }

                }
                if (!_systemResponseFunction_stage_failureProbability.IsNull && _systemResponseFunction_stage_failureProbability.HasErrors)
                {
                    errors.AppendLine(nameof(_systemResponseFunction_stage_failureProbability) + $" has the following messages for the impact area with ID {_impactAreaID}:");
                    foreach (string s in _systemResponseFunction_stage_failureProbability.GetErrors())
                    {
                        errors.AppendLine(s);
                    }

                }
                foreach (UncertainPairedData relationship in _damage_category_stage_damage)
                {
                    if (!relationship.IsNull && relationship.HasErrors)
                    {
                        errors.AppendLine(nameof(_damage_category_stage_damage) + ": " + relationship.CurveMetaData.DamageCategory + ": " + relationship.CurveMetaData.AssetCategory + ": " + "has the following messages");
                        foreach (string s in relationship.GetErrors())
                        {
                            errors.AppendLine(s);
                        }

                    }
                }

                Message mess = new Message(errors.ToString());
                ReportMessage(this, new MessageEventArgs(mess));

            }
            if (randomProvider is MedianRandomProvider)
            {
                if (convergenceCriteria.MaxIterations != 1)
                {
                    string message = $"The simulation for impact area {_impactAreaID} was requested to provide a mean estimate, but asked for more than one iteration." + Environment.NewLine;
                    ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
                    ReportMessage(this, new MessageEventArgs(errorMessage)); return false;

                }
            }
            else
            {
                if (convergenceCriteria.MinIterations < 100)
                {
                    string message = $"The simulation for impact area {_impactAreaID} was requested to provide a random estimate, but asked for a minimum of one iteration." + Environment.NewLine;
                    ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
                    ReportMessage(this, new MessageEventArgs(errorMessage)); return false;

                }
            }
            //TODO if curves do not overlap we don't have a way here of saying HasErrors = true 
            //Nor is there relevant messaging 
            bool curvesOverlap = SimulationCurvesHaveOverlap();
            if (!curvesOverlap)
            {
                return false;
            }
            //TODO if convergence criteria is not valid, we don't have a way of saying HasErrors = true 
            //nor is there relevant messaging
            convergenceCriteria.Validate();
            if (convergenceCriteria.HasErrors)
            {
                return false;
            }
            return true;
        }

        private void ComputeIterations(ConvergenceCriteria convergenceCriteria, IProvideRandomNumbers randomProvider, int masterseed, bool computeWithDamage, bool giveMeADamageFrequency, bool computeIsDeterministic)
        {
            long progressChunks = 1;
            long _completedIterations = 0;
            long _ExpectedIterations = convergenceCriteria.MaxIterations;
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
            long iterations = convergenceCriteria.MinIterations;
            //_leveeIsValid = LeveeIsValid();///this should be integrated into more formal validation routines above.

            while (!_impactAreaScenarioResults.IsConverged(computeWithDamage))
            {
                Parallel.For(0, iterations, i =>
                {
                    //check if it is a mean random provider or not
                    IProvideRandomNumbers threadlocalRandomProvider;
                    if (randomProvider is MedianRandomProvider)
                    {
                        threadlocalRandomProvider = new MedianRandomProvider();
                    }
                    else
                    {
                        threadlocalRandomProvider = new RandomProvider(seeds[i]);
                    }
                    if (_frequency_stage.CurveMetaData.IsNull)
                    {
                        if (_discharge_stage.CurveMetaData.IsNull)
                        {
                            //complain loudly
                            string message = $"A stage-discharge function must accompany a discharge-frequency function but was not found for the impact area with ID {_impactAreaID}. Compute aborted." + Environment.NewLine;
                            ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
                            ReportMessage(this, new MessageEventArgs(errorMessage));
                            return;
                        }
                        IPairedData frequencyDischarge;
                        if (_frequency_discharge_graphical.CurveMetaData.IsNull)
                        {
                            //If threadlocalRandomProvider is medianRandomProvider then we get a deterministic result
                            frequencyDischarge = BootstrapToPairedData(threadlocalRandomProvider, _frequency_discharge, 200);//ordinates defines the number of values in the frequency curve, more would be a better approximation.
                        }
                        else
                        {
                            //If threadlocalRandomProvider is medianRandomProvider then we get a deterministic result
                            frequencyDischarge = _frequency_discharge_graphical.SamplePairedData(threadlocalRandomProvider.NextRandom());
                        }
                        //if frequency_flow is not defined throw big errors.
                        //check if flow transform exists, and use it here
                        if (_unregulated_regulated.CurveMetaData.IsNull)
                        {
                            IPairedData discharge_stage_sample = _discharge_stage.SamplePairedData(threadlocalRandomProvider.NextRandom(), computeIsDeterministic);
                            IPairedData frequency_stage = discharge_stage_sample.compose(frequencyDischarge);
                            ComputeFromStageFrequency(threadlocalRandomProvider, frequency_stage, giveMeADamageFrequency, i, computeWithDamage, computeIsDeterministic);
                        }
                        else
                        {
                            IPairedData inflow_outflow_sample = _unregulated_regulated.SamplePairedData(threadlocalRandomProvider.NextRandom(), computeIsDeterministic); //should be a random number
                            IPairedData transformff = inflow_outflow_sample.compose(frequencyDischarge);
                            IPairedData discharge_stage_sample = _discharge_stage.SamplePairedData(threadlocalRandomProvider.NextRandom(), computeIsDeterministic);//needs to be a random number
                            IPairedData frequency_stage = discharge_stage_sample.compose(transformff);
                            ComputeFromStageFrequency(threadlocalRandomProvider, frequency_stage, giveMeADamageFrequency, i, computeWithDamage, computeIsDeterministic);
                        }

                    }
                    else
                    {
                        //if threadlocalRandomProvider is medianRandomProvider then we get a deterministic result
                        IPairedData frequency_stage_sample = _frequency_stage.SamplePairedData(threadlocalRandomProvider.NextRandom());
                        ComputeFromStageFrequency(threadlocalRandomProvider, frequency_stage_sample, giveMeADamageFrequency, i, computeWithDamage, computeIsDeterministic);
                    }
                    Interlocked.Increment(ref _completedIterations);
                    if (_completedIterations % progressChunks == 0)//need an atomic integer count here.
                    {
                        double percentcomplete = _completedIterations / (double)_ExpectedIterations * 100;
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
            ReportProgress(this, new ProgressReportEventArgs(IMPACT_AREA_SIM_COMPLETED));
            _impactAreaScenarioResults.ForceDeQueue();
        }

        private void ComputeFromStageFrequency(IProvideRandomNumbers randomProvider, IPairedData frequency_stage, bool giveMeADamageFrequency, long iteration, bool computeWithDamage, bool computeIsDeterministic)
        {

            //interior exterior
            if (_channelstage_floodplainstage.CurveMetaData.IsNull)
            {
                //levees
                if (_systemResponseFunction_stage_failureProbability.CurveMetaData.IsNull)
                {
                    if (computeWithDamage)
                    {
                        ComputeDamagesFromStageFrequency(randomProvider, frequency_stage, giveMeADamageFrequency, iteration, computeIsDeterministic);
                    }
                    ComputePerformance(frequency_stage, iteration);
                }
                else
                {
                    if (_leveeIsValid)
                    {
                        //TODO: why commented out and why still exists
                        IPairedData systemResponse_sample = _systemResponseFunction_stage_failureProbability.SamplePairedData(randomProvider.NextRandom(), computeIsDeterministic); //needs to be a random number
                        //IPairedData frequency_stage_withLevee = frequency_stage.multiply(levee_curve_sample);
                        if (computeWithDamage)
                        {
                            ComputeDamagesFromStageFrequency_WithLevee(randomProvider, frequency_stage, systemResponse_sample, giveMeADamageFrequency, iteration, computeIsDeterministic);
                        }
                        ComputeLeveePerformance(frequency_stage, systemResponse_sample, iteration);
                    }

                }

            }
            else
            {   //todo is there a reason for the starting underscore? 
                IPairedData _channelstage_floodplainstage_sample = _channelstage_floodplainstage.SamplePairedData(randomProvider.NextRandom(), computeIsDeterministic); //needs to be a random number
                IPairedData frequency_floodplainstage = _channelstage_floodplainstage_sample.compose(frequency_stage);
                //levees
                if (_systemResponseFunction_stage_failureProbability.CurveMetaData.IsNull)
                {
                    if (computeWithDamage)
                    {
                        ComputeDamagesFromStageFrequency(randomProvider, frequency_floodplainstage, giveMeADamageFrequency, iteration, computeIsDeterministic);
                    }
                    ComputePerformance(frequency_floodplainstage, iteration);
                }
                else
                {
                    if (_leveeIsValid)
                    {//TODO: why commented out and why still exists
                        IPairedData systemResponse_sample = _systemResponseFunction_stage_failureProbability.SamplePairedData(randomProvider.NextRandom(), computeIsDeterministic); //needs to be a random number
                        //IPairedData frequency_floodplainstage_withLevee = frequency_floodplainstage.multiply(_levee_curve_sample);
                        if (computeWithDamage)
                        {
                            ComputeDamagesFromStageFrequency_WithLeveeAndInteriorExterior(randomProvider, _channelstage_floodplainstage_sample, frequency_stage, systemResponse_sample, giveMeADamageFrequency, iteration, computeIsDeterministic);
                        }
                        ComputeLeveePerformance(frequency_stage, systemResponse_sample, iteration);
                    }

                }

            }
        }
        private IPairedData BootstrapToPairedData(IProvideRandomNumbers randomProvider, ContinuousDistribution continuousDistribution, int ordinates)
        {
            double[] samples = randomProvider.NextRandomSequence(continuousDistribution.SampleSize);
            IDistribution bootstrap = continuousDistribution.Sample(samples);
            double[] x = new double[_RequiredExceedanceProbabilities.Length];
            double[] y = new double[_RequiredExceedanceProbabilities.Length];
            for (int i = 0; i < _RequiredExceedanceProbabilities.Length; i++)
            {
                //same exceedance probs as graphical and as 1.4.3
                double prob = 1 - _RequiredExceedanceProbabilities[i];
                x[i] = prob;

                //y values in increasing order 
                y[i] = bootstrap.InverseCDF(prob);
            }
            return new PairedData(x, y);

        }
        private void ComputeDamagesFromStageFrequency(IProvideRandomNumbers randomProvider, IPairedData frequency_stage, bool giveMeADamageFrequency, long iteration, bool computeIsDeterministic)
        {
            CurveMetaData metadata = new CurveMetaData("Total", "Total");
            PairedData totalDamageFrequency = new PairedData(null, null, metadata);

            foreach (UncertainPairedData stageDamageWithUncertainty in _damage_category_stage_damage)
            {
                //TODO: here we need to check if stage damage is zero 
                //if so, then skip this stuff and just add 0 to consequenceResults
                IPairedData _stage_damage_sample = stageDamageWithUncertainty.SamplePairedData(randomProvider.NextRandom(), computeIsDeterministic);
                IPairedData frequency_damage = _stage_damage_sample.compose(frequency_stage);
                double eadEstimate = frequency_damage.integrate();
                _impactAreaScenarioResults.ConsequenceResults.AddConsequenceRealization(eadEstimate, stageDamageWithUncertainty.CurveMetaData.DamageCategory, stageDamageWithUncertainty.CurveMetaData.AssetCategory, _impactAreaID, iteration);

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
        private void ComputeDamagesFromStageFrequency_WithLevee(IProvideRandomNumbers randomProvider, IPairedData frequency_stage, IPairedData systemResponse, bool giveMeADamageFrequency, long iteration, bool computeIsDeterministic)
        {
            //TODO "Total" could be represented as public static const string TOTAL = "Total";
            CurveMetaData metadata = new CurveMetaData("Total", "Total");
            PairedData totalDamageFrequency = new PairedData(null, null, metadata);

            foreach (UncertainPairedData pd in _damage_category_stage_damage)
            {
                IPairedData stage_damage_sample = pd.SamplePairedData(randomProvider.NextRandom(), computeIsDeterministic);//needs to be a random number
                //here we need to compose with interior exterior 
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
        private void ComputeDamagesFromStageFrequency_WithLeveeAndInteriorExterior(IProvideRandomNumbers randomProvider, IPairedData exterior_interior, IPairedData frequency_exteriorStage, IPairedData systemResponse, bool giveMeADamageFrequency, long iteration, bool computeIsDeterministic)
        {
            //TODO "Total" could be represented as public static const string TOTAL = "Total";
            CurveMetaData metadata = new CurveMetaData("Total", "Total");
            PairedData totalDamageFrequency = new PairedData(null, null, metadata);

            foreach (UncertainPairedData stageUncertainDamage in _damage_category_stage_damage)
            {   //TODO: why are we doing this stuff with the underscores? I think this needs to be cleaned up 
                IPairedData interiorStage_damage_sample = stageUncertainDamage.SamplePairedData(randomProvider.NextRandom(), computeIsDeterministic);//needs to be a random number
                IPairedData exteriorStage_damage_sample = interiorStage_damage_sample.compose(exterior_interior);
                IPairedData stage_damage_sample_withLevee = exteriorStage_damage_sample.multiply(systemResponse);
                IPairedData frequency_damage = stage_damage_sample_withLevee.compose(frequency_exteriorStage);
                double eadEstimate = frequency_damage.integrate();
                _impactAreaScenarioResults.ConsequenceResults.AddConsequenceRealization(eadEstimate, stageUncertainDamage.CurveMetaData.DamageCategory, stageUncertainDamage.CurveMetaData.AssetCategory, _impactAreaID, iteration);
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
        public void ComputePerformance(IPairedData frequency_stage, long iteration)
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
        public void ComputeLeveePerformance(IPairedData frequency_stage, IPairedData levee_curve_sample, long iteration)
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

        public void GetStageForNonExceedanceProbability(IPairedData frequency_stage, Threshold threshold, long iteration)
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




        private Threshold ComputeDefaultThreshold(ConvergenceCriteria convergenceCriteria, bool computeWithDamage, bool computeIsDeterministic)
        {
            //TODO do we need the compute with damage bool?
            MedianRandomProvider meanRandomProvider = new MedianRandomProvider();
            IPairedData frequencyStage = new PairedData(null, null);
            IPairedData totalStageDamage = ComputeTotalStageDamage(_damage_category_stage_damage);
            if (_systemResponseFunction_stage_failureProbability.CurveMetaData.IsNull)
            {
                if (_damage_category_stage_damage.Count == 0)
                {
                    string message = $"A valid default threshold cannot be calculated for the impact area with ID {_impactAreaID} because no stage-damage functions were found. A meaningless default threshold of 0 will be used. Please have an additional threshold for meaningful performance statistics" + Environment.NewLine;
                    ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
                    ReportMessage(this, new MessageEventArgs(errorMessage));
                    return new Threshold();
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
                            string message = $"A stage-discharge function must accompany a discharge-frequency function but was not found for the impact area with ID {_impactAreaID}. An arbitrary threshold is being used." + Environment.NewLine;
                            ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
                            ReportMessage(this, new MessageEventArgs(errorMessage));
                            return new Threshold();

                        }
                        else
                        {
                            IPairedData flowStageSample = _discharge_stage.SamplePairedData(meanRandomProvider.NextRandom(), computeIsDeterministic);
                            frequencyStage = flowStageSample.compose(frequencyFlow);
                        }
                    }
                    else
                    {
                        IPairedData inflowOutflowSample = _unregulated_regulated.SamplePairedData(meanRandomProvider.NextRandom(), computeIsDeterministic);
                        IPairedData transformFlowFrequency = inflowOutflowSample.compose(frequencyFlow);
                        if (_discharge_stage.CurveMetaData.IsNull)
                        {
                            string message = $"A stage-discharge function must accompany a discharge-frequency function but was not found for the impact area with ID {_impactAreaID}. An arbitrary threshold is being used." + Environment.NewLine;
                            ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
                            ReportMessage(this, new MessageEventArgs(errorMessage));
                            return new Threshold();
                        }
                        else
                        {
                            IPairedData flowStageSample = _discharge_stage.SamplePairedData(meanRandomProvider.NextRandom(), computeIsDeterministic);
                            frequencyStage = flowStageSample.compose(transformFlowFrequency);
                        }
                    }

                }
                else
                {
                    frequencyStage = _frequency_stage.SamplePairedData(meanRandomProvider.NextRandom());
                }
                IPairedData frequencyDamage;
                if (_channelstage_floodplainstage.IsNull)
                {
                    frequencyDamage = totalStageDamage.compose(frequencyStage);
                }
                else
                {
                    IPairedData exteriorInterior = _channelstage_floodplainstage.SamplePairedData(meanRandomProvider.NextRandom(), computeIsDeterministic);
                    IPairedData frequencyInteriorStage = exteriorInterior.compose(frequencyStage);
                    frequencyDamage = totalStageDamage.compose(frequencyInteriorStage);
                }
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
            MedianRandomProvider meanRandomProvider = new MedianRandomProvider();
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

            MedianRandomProvider meanRandomProvider = new MedianRandomProvider();
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            ImpactAreaScenarioResults results = Compute(meanRandomProvider, convergenceCriteria, false, true, computeIsDeterministic: true);
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
                string message = $"There must exist a stage in the fragility curve with a certain probability of failure specified as a deterministic distribution but was not found for the impact area with ID {_impactAreaID}" + Environment.NewLine;
                ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage));
                return false;
            }
            else if (_systemResponseFunction_stage_failureProbability.Yvals.Last().InverseCDF(0.5) != 1) //we should be given a deterministic distribution at the end where prob(failure) = 1
            { //the determinstic distribution could be normal with zero standard deviation, triangular or uniform with min and max = 1, doesn't matter
              //distributions where the user specifies zero variability should be passed to the model as a deterministic distribution 
              //this has been communicated 
                string message = $"There must exist a stage in the fragility curve with a certain probability of failure specified as a deterministic distribution for the impact area with ID {_impactAreaID}" + Environment.NewLine;
                ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
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
                    string message = $"The top of levee elevation of {_topOfLeveeElevation} in the fragility function does not have certain probability of failure specified as a deterministic distribution for the impact area with ID {_impactAreaID}" + Environment.NewLine;
                    ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Major);
                    ReportMessage(this, new MessageEventArgs(errorMessage));
                }
            }
            else
            {   //top of levee elevation is not included in the fragility curve
                string message = $"The top of levee elevation of {_topOfLeveeElevation} in the fragility function does not have a certain probability of failure specified as a deterministic distribution for the impact area with ID {_impactAreaID}" + Environment.NewLine;
                ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Major);
                ReportMessage(this, new MessageEventArgs(errorMessage));
            }
        }
        //TODO: Add messaging to indicate which curves do not overlap
        private bool SimulationCurvesHaveOverlap()
        {
            bool allCurvesHaveOverlap = true;
            if (_frequency_stage.CurveMetaData.IsNull)
            {
                if (_discharge_stage.CurveMetaData.IsNull)
                {
                    string message = $"A stage-discharge function must accompany a discharge-frequency function but was not found for the impact area with ID {_impactAreaID}. Compute aborted." + Environment.NewLine;
                    ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
                    ReportMessage(this, new MessageEventArgs(errorMessage));
                    allCurvesHaveOverlap = false;
                }
                if (_frequency_discharge_graphical.CurveMetaData.IsNull)
                {
                    bool firstTwoCurvesOverlap = true;
                    bool nextTwoCurvesOverlap = true;
                    if (_unregulated_regulated.CurveMetaData.IsNull)
                    {
                        firstTwoCurvesOverlap = CurvesHaveOverlap(_discharge_stage, _frequency_discharge);
                    }
                    else
                    {
                        firstTwoCurvesOverlap = CurvesHaveOverlap(_unregulated_regulated, _frequency_discharge);
                        nextTwoCurvesOverlap = CurvesHaveOverlap(_discharge_stage, _unregulated_regulated);
                    }
                    if (!firstTwoCurvesOverlap)
                    {
                        allCurvesHaveOverlap = firstTwoCurvesOverlap;
                    }
                    if (!nextTwoCurvesOverlap)
                    {
                        allCurvesHaveOverlap = nextTwoCurvesOverlap;
                    }
                }
                else
                {
                    bool firstTwoCurvesOverlap = true;
                    bool nextTwoCurvesOverlap = true;
                    if (_unregulated_regulated.CurveMetaData.IsNull)
                    {
                        firstTwoCurvesOverlap = CurvesHaveOverlap(_discharge_stage, _frequency_discharge_graphical);
                    }
                    else
                    {
                        firstTwoCurvesOverlap = CurvesHaveOverlap(_unregulated_regulated, _frequency_discharge_graphical);
                        nextTwoCurvesOverlap = CurvesHaveOverlap(_discharge_stage, _unregulated_regulated);
                    }
                    if (!firstTwoCurvesOverlap)
                    {
                        allCurvesHaveOverlap = firstTwoCurvesOverlap;
                    }
                    if (!nextTwoCurvesOverlap)
                    {
                        allCurvesHaveOverlap = nextTwoCurvesOverlap;
                    }
                }
                if (_channelstage_floodplainstage.CurveMetaData.IsNull)
                {
                    foreach (UncertainPairedData uncertainPairedData in _damage_category_stage_damage)
                    {
                        bool stageDamageOverlaps = CurvesHaveOverlap(uncertainPairedData, _discharge_stage);
                        if (!stageDamageOverlaps)
                        {
                            allCurvesHaveOverlap = stageDamageOverlaps;
                        }
                    }
                }
                else
                {
                    bool nextTwoCurvesHaveOverlap = CurvesHaveOverlap(_channelstage_floodplainstage, _discharge_stage);
                    if (!nextTwoCurvesHaveOverlap)
                    {
                        allCurvesHaveOverlap = nextTwoCurvesHaveOverlap;
                    }
                    foreach (UncertainPairedData uncertain in _damage_category_stage_damage)
                    {
                        bool stageDamageOverlaps = CurvesHaveOverlap(uncertain, _channelstage_floodplainstage);
                        if (!stageDamageOverlaps)
                        {
                            allCurvesHaveOverlap = stageDamageOverlaps;
                        }
                    }
                }
            }
            else
            {
                if (!_channelstage_floodplainstage.CurveMetaData.IsNull)
                {
                    bool nextTwoCurvesHaveOverlap = CurvesHaveOverlap(_channelstage_floodplainstage, _frequency_stage);
                    if (!nextTwoCurvesHaveOverlap)
                    {
                        allCurvesHaveOverlap = nextTwoCurvesHaveOverlap;
                    }
                    foreach (UncertainPairedData uncertain in _damage_category_stage_damage)
                    {
                        bool stageDamageOverlaps = CurvesHaveOverlap(uncertain, _channelstage_floodplainstage);
                        if (!stageDamageOverlaps)
                        {
                            allCurvesHaveOverlap = stageDamageOverlaps;
                        }
                    }
                }
                else
                {
                    foreach (UncertainPairedData uncertain in _damage_category_stage_damage)
                    {
                        bool stageDamageOverlaps = CurvesHaveOverlap(uncertain, _frequency_stage);
                        if (!stageDamageOverlaps)
                        {
                            allCurvesHaveOverlap = stageDamageOverlaps;
                        }
                    }
                }
            }
            return allCurvesHaveOverlap;
        }
        private bool CurvesHaveOverlap(UncertainPairedData uncertainPairedData_f, UncertainPairedData uncertainPairedData_g)
        {
            double maxOfF = uncertainPairedData_f.Xvals[uncertainPairedData_f.Yvals.Length - 1];
            double minOfF = uncertainPairedData_f.Xvals[0];
            double minOfG = uncertainPairedData_g.Yvals[0].InverseCDF(.001);
            double maxOfG = uncertainPairedData_g.Yvals[uncertainPairedData_g.Yvals.Length - 1].InverseCDF(.999);

            bool curvesOverlap = CurvesOverlap(maxOfF, minOfF, maxOfG, minOfG);
            return curvesOverlap;
        }
        private bool CurvesHaveOverlap(UncertainPairedData uncertainPairedData_f, GraphicalUncertainPairedData uncertainPairedData_g)
        {
            double maxOfF = uncertainPairedData_f.Xvals[uncertainPairedData_f.Xvals.Length - 1];
            double minOfF = uncertainPairedData_f.Xvals[0];
            double minOfG = uncertainPairedData_g.InputFlowOrStageValues[0];
            double maxOfG = uncertainPairedData_g.InputFlowOrStageValues[uncertainPairedData_g.InputFlowOrStageValues.Length - 1];

            bool curvesOverlap = CurvesOverlap(maxOfF, minOfF, maxOfG, minOfG);
            return curvesOverlap;
        }
        private bool CurvesHaveOverlap(UncertainPairedData uncertainPairedData_f, ContinuousDistribution continuousDistribution_g)
        {
            double maxOfF = uncertainPairedData_f.Xvals[uncertainPairedData_f.Xvals.Length - 1];
            double minOfF = uncertainPairedData_f.Xvals[0];
            double minOfG = continuousDistribution_g.InverseCDF(.001);
            double maxOfG = continuousDistribution_g.InverseCDF(.75);

            bool curvesOverlap = CurvesOverlap(maxOfF, minOfF, maxOfG, minOfG);
            return curvesOverlap;
        }
        private bool CurvesOverlap(double maxOfF, double minOfF, double maxOfG, double minOfG)
        {
            bool curvesOverlap = true;
            double overlapThreshold = 0.95;
            double rangeOfF = maxOfF - minOfF;
            double rangeOfG = maxOfF - minOfG;
            double minDifference = Math.Abs(minOfG - minOfF);
            double maxDifference = Math.Abs(maxOfG - maxOfF);
            double minDiffRelativeToF = minDifference / rangeOfF;
            double minDiffRelativeToG = minDifference / rangeOfG;
            double maxDiffRelativeToF = maxDifference / rangeOfF;
            double maxDiffRelativeToG = maxDifference / rangeOfG;

            if (minDiffRelativeToF > overlapThreshold || minDiffRelativeToG > overlapThreshold || maxDiffRelativeToF > overlapThreshold || maxDiffRelativeToG > overlapThreshold)
            {
                curvesOverlap = false;
            }

            return curvesOverlap;
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
                    if (stageDamage.CurveMetaData.DamageCategory.Equals(incomingStageDamage.CurveMetaData.DamageCategory) && stageDamage.CurveMetaData.AssetCategory.Equals(incomingStageDamage.CurveMetaData.AssetCategory))
                    {
                        bool stageDamagesMatch = stageDamage.Equals(incomingStageDamage);
                        if (!stageDamagesMatch)
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

            ImpactAreaScenarioSimulation impactAreaScenarioSimulation = builder(impactAreaID)
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
            private ImpactAreaScenarioSimulation _simulation;
            internal SimulationBuilder(ImpactAreaScenarioSimulation sim)
            {
                _simulation = sim;
            }
            public ImpactAreaScenarioSimulation build()
            {
                //TODO: The validation below is not very helpful. We only see that "XX has errors" but 
                //we are not informing the user what the errors are 
                //somehow we need to add the error messages of the object being validated to the error messages of the impact area scenario simulation 
                _simulation.Validate();
                //add validation here to test ranges and domains.
                return _simulation;
            }
            public SimulationBuilder withFlowFrequency(ContinuousDistribution continuousDistribution)
            {   //TODO: I do not think the sample size validation works
                _simulation._frequency_discharge = continuousDistribution;
                _simulation.AddSinglePropertyRule("flow frequency", new Rule(() => { _simulation._frequency_discharge.Validate(); return !_simulation._frequency_discharge.HasErrors; }, string.Join(Environment.NewLine, _simulation._frequency_discharge.GetErrors())));
                return new SimulationBuilder(_simulation);
            }
            public SimulationBuilder withFlowFrequency(GraphicalUncertainPairedData graphicalUncertainPairedData)
            {
                _simulation._frequency_discharge_graphical = graphicalUncertainPairedData;
                return new SimulationBuilder(_simulation);
            }
            public SimulationBuilder withInflowOutflow(UncertainPairedData uncertainPairedData)
            {
                _simulation._unregulated_regulated = uncertainPairedData;
                _simulation.AddSinglePropertyRule("inflow outflow", new Rule(() => { _simulation._unregulated_regulated.Validate(); return !_simulation._unregulated_regulated.HasErrors; }, $"Inflow-Outflow has errors for the impact area with ID {_simulation._impactAreaID}."));

                return new SimulationBuilder(_simulation);
            }
            public SimulationBuilder withFlowStage(UncertainPairedData uncertainPairedData)
            {
                _simulation._discharge_stage = uncertainPairedData;
                _simulation.AddSinglePropertyRule("flow stage", new Rule(() => { _simulation._discharge_stage.Validate(); return !_simulation._discharge_stage.HasErrors; }, $"Flow-Stage has errors  for the impact area with ID {_simulation._impactAreaID}."));

                return new SimulationBuilder(_simulation);
            }
            public SimulationBuilder withFrequencyStage(GraphicalUncertainPairedData graphicalUncertainPairedData)
            {
                _simulation._frequency_stage = graphicalUncertainPairedData;
                _simulation.AddSinglePropertyRule("frequency_stage", new Rule(() => { _simulation._frequency_stage.Validate(); return !_simulation._frequency_stage.HasErrors; }, $"Frequency-Stage has errors  for the impact area with ID {_simulation._impactAreaID}."));
                return new SimulationBuilder(_simulation);
            }
            public SimulationBuilder withInteriorExterior(UncertainPairedData uncertainPairedData)
            {
                _simulation._channelstage_floodplainstage = uncertainPairedData;
                _simulation.AddSinglePropertyRule("channelstage_floodplainstage", new Rule(() =>
                {
                    _simulation._channelstage_floodplainstage.Validate();
                    return !_simulation._channelstage_floodplainstage.HasErrors;
                }
                , $"There are errors in the InteriorExterior relationship for the impact area with ID {_simulation._impactAreaID}."));
                return new SimulationBuilder(_simulation);
            }
            public SimulationBuilder withLevee(UncertainPairedData uncertainPairedData, double topOfLeveeElevation)
            {
                _simulation.AddSinglePropertyRule("levee", new Rule(() => _simulation.LeveeIsValid(), $"The levee is invalid  for the impact area with ID {_simulation._impactAreaID}."));
                _simulation._systemResponseFunction_stage_failureProbability = uncertainPairedData;
                _simulation._topOfLeveeElevation = topOfLeveeElevation;
                return new SimulationBuilder(_simulation);
            }
            public SimulationBuilder withStageDamages(List<UncertainPairedData> uncertainPairedDataList)
            {
                _simulation._damage_category_stage_damage = uncertainPairedDataList;
                foreach (UncertainPairedData uncertainPairedData in _simulation._damage_category_stage_damage)
                {
                    _simulation.AddSinglePropertyRule(uncertainPairedData.CurveMetaData.DamageCategory + " stage damages", new Rule(() => { uncertainPairedData.Validate(); return !uncertainPairedData.HasErrors; }, $"Stage-damage errors ror the impact area with ID {_simulation._impactAreaID}:" + uncertainPairedData.GetErrors().ToString()));
                }
                return new SimulationBuilder(_simulation);
            }
            public SimulationBuilder withPerformanceMetrics(ImpactAreaScenarioResults results)
            {
                _simulation._impactAreaScenarioResults = results;
                return new SimulationBuilder(_simulation);
            }
            public SimulationBuilder withAdditionalThreshold(Threshold threshold)
            {
                _simulation._impactAreaScenarioResults.PerformanceByThresholds.AddThreshold(threshold);
                return new SimulationBuilder(_simulation);
            }

            public SimulationBuilder forImpactArea(int impactAreaID)
            {
                _simulation._impactAreaID = impactAreaID;
                return new SimulationBuilder(_simulation);
            }
        }

    }

}