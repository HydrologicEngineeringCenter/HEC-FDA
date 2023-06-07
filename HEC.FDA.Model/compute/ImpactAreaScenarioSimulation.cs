using HEC.FDA.Model.extensions;
using HEC.FDA.Model.interfaces;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Model.Messaging;
using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HEC.FDA.Model.compute
{
    public class ImpactAreaScenarioSimulation : ValidationErrorLogger, IProgressReport
    {
        public const int IMPACT_AREA_SIM_COMPLETED = -1001;

        private const double THRESHOLD_DAMAGE_PERCENT = 0.05;
        private const double THRESHOLD_DAMAGE_RECURRENCE_INTERVAL = 0.99; //this is a non-exceedance probability 
        private const int DEFAULT_THRESHOLD_ID = 0;
        private ContinuousDistribution _FrequencyDischarge;
        private GraphicalUncertainPairedData _FrequencyDischargeGraphical;
        private UncertainPairedData _UnregulatedRegulated;
        private UncertainPairedData _DischargeStage;
        private GraphicalUncertainPairedData _FrequencyStage;
        private UncertainPairedData _ChannelStageFloodplainStage;
        private UncertainPairedData _SystemResponseFunction;
        private double _TopOfLeveeElevation;
        private List<UncertainPairedData> _DamageCategoryStageDamage;
        private int _ImpactAreaID;
        private ImpactAreaScenarioResults _ImpactAreaScenarioResults;
        private bool _LeveeIsValid = false;
        private readonly double[] _RequiredExceedanceProbabilities = { 0.99900, 0.99000, 0.95000, 0.90000, 0.85000, 0.80000, 0.75000, 0.70000,
            0.65000, 0.60000,0.55000, 0.50000, 0.47500, 0.45000, 0.42500, 0.40000, 0.37500, 0.35000, 0.32500, 0.30000, 0.29000, 0.28000, 0.27000, 0.26000,
            0.25000, 0.24000,0.23000, 0.22000, 0.21000, 0.20000, 0.19500, 0.19000, 0.18500, 0.18000, 0.17500, 0.17000, 0.16500, 0.16000, 0.15500, 0.15000,
            0.14500, 0.14000,0.13500, 0.13000, 0.12500, 0.12000, 0.11500, 0.11000, 0.10500, 0.10000, 0.09500, 0.09000, 0.08500, 0.08000, 0.07500, 0.07000,
            0.06500, 0.06000,0.05900, 0.05800, 0.05700, 0.05600, 0.05500, 0.05400, 0.05300, 0.05200, 0.05100, 0.05000, 0.04900, 0.04800, 0.04700, 0.04600,
            0.04500, 0.04400,0.04300, 0.04200, 0.04100, 0.04000, 0.03900, 0.03800, 0.03700, 0.03600, 0.03500, 0.03400, 0.03300, 0.03200, 0.03100, 0.03000,
            0.02900, 0.02800, 0.02700, 0.02600, 0.02500, 0.02400, 0.02300, 0.02200, 0.02100, 0.02000, 0.01950, 0.01900, 0.01850, 0.01800, 0.01750, 0.01700,
            0.01650, 0.01600,0.01550, 0.01500, 0.01450, 0.01400, 0.01350, 0.01300, 0.01250, 0.01200, 0.01150, 0.01100, 0.01050, 0.01000, 0.00950, 0.00900,
            0.00850, 0.00800,0.00750, 0.00700, 0.00650, 0.00600, 0.00550, 0.00500, 0.00490, 0.00450, 0.00400, 0.00350, 0.00300, 0.00250, 0.00200, 0.00195,
            0.00190, 0.00185,0.00180, 0.00175, 0.00170, 0.00165, 0.00160, 0.00155, 0.00150, 0.00145, 0.00140, 0.00135, 0.00130, 0.00125, 0.00120, 0.00115,
            0.00110, 0.00105, 0.00100, 0.00095, 0.00090, 0.00085, 0.00080, 0.00075, 0.00070, 0.00065, 0.00060, 0.00055, 0.00050, 0.00045, 0.00040, 0.00035,
            0.00030, 0.00025,0.00020, 0.00015, 0.00010 };

        public event ProgressReportedEventHandler ProgressReport;
        public bool HasLevee
        {
            get
            {
                return !_SystemResponseFunction.CurveMetaData.IsNull;
            }
        }
        public int ImpactAreaID
        {
            get
            {
                return _ImpactAreaID;
            }
        }
        internal ImpactAreaScenarioSimulation(int impactAreaID)
        {
            _FrequencyDischarge = null;
            _FrequencyDischargeGraphical = new GraphicalUncertainPairedData(); //can we have both of these?
            _UnregulatedRegulated = new UncertainPairedData();//defaults to null
            _DischargeStage = new UncertainPairedData(); //defaults to null
            _FrequencyStage = new GraphicalUncertainPairedData();//defaults to null
            _ChannelStageFloodplainStage = new UncertainPairedData();//defaults to null
            _SystemResponseFunction = new UncertainPairedData(); //defaults to null
            _DamageCategoryStageDamage = new List<UncertainPairedData>();//defaults to empty
            _ImpactAreaID = impactAreaID;
            _ImpactAreaScenarioResults = new ImpactAreaScenarioResults(_ImpactAreaID);
        }

        public ImpactAreaScenarioResults Compute(IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria)
        {
            return Compute(randomProvider, convergenceCriteria, new CancellationToken());
        }

        /// <summary>
        /// A simulation must be built with a stage damage function for compute default threshold to be true.
        /// </summary>
        /// <param name="randomProvider"></param>
        /// <param name="iterations"></param>
        /// <param name="computeDefaultThreshold"></param>
        /// <returns></returns>
        public ImpactAreaScenarioResults Compute(IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, CancellationToken cancellationToken,
            bool computeDefaultThreshold = true, bool giveMeADamageFrequency = false, bool computeIsDeterministic = false)
        {
            //Validate();
            if (!CanCompute(convergenceCriteria, randomProvider))
            {
                _ImpactAreaScenarioResults = new ImpactAreaScenarioResults(_ImpactAreaID, true);
                return _ImpactAreaScenarioResults;
            }
            int masterseed = 0;
            if (randomProvider is RandomProvider)
            {
                masterseed = randomProvider.Seed;
            }
            //TODO: levee is valid is not used
            _LeveeIsValid = true;
            bool computeWithDamage = true;

            if (_DamageCategoryStageDamage.Count == 0)
            {
                computeWithDamage = false;
            }
            else
            {
                List<(CurveMetaData, PairedData)> damageFrequencyFunctions = ComputeDamageFrequency(computeIsDeterministic);
                CreateEADHistograms(convergenceCriteria, damageFrequencyFunctions);
                if (computeDefaultThreshold == true)
                {//I am not sure if there is a better way to add the default threshold
                    _ImpactAreaScenarioResults.PerformanceByThresholds.AddThreshold(ComputeDefaultThreshold(convergenceCriteria, computeWithDamage, damageFrequencyFunctions));
                }
                //this is where we message out the damage frequency functions
                if (giveMeADamageFrequency)
                {
                    foreach ((CurveMetaData, PairedData) damCatDamageFrequency in damageFrequencyFunctions)
                    {
                        ReportMessage(this, new MessageEventArgs(new FrequencyDamageMessage(damCatDamageFrequency.Item2, damCatDamageFrequency.Item1.DamageCategory, damCatDamageFrequency.Item1.AssetCategory)));

                    }
                }
            }

            CreateHistogramsForAssuranceOfThresholds();
            MessageEventArgs beginComputeMessageArgs = new(new Message($"EAD and performance compute for the impact area with ID {_ImpactAreaID} has been initiated" + Environment.NewLine));
            ReportMessage(this, beginComputeMessageArgs);
            ComputeIterations(convergenceCriteria, randomProvider, masterseed, computeWithDamage, computeIsDeterministic, cancellationToken);
            _ImpactAreaScenarioResults.ParallelResultsAreConverged(.95, .05);
            MessageEventArgs endComputeMessageArgs = new(new Message($"EAD and performance compute for the impact area with ID {_ImpactAreaID} has completed successfully" + Environment.NewLine));
            ReportMessage(this, endComputeMessageArgs);
            return _ImpactAreaScenarioResults;
        }

        private void CreateEADHistograms(ConvergenceCriteria convergenceCriteria, List<(CurveMetaData, PairedData)> damageFrequencyFunctions)
        {
            //run the preview compute
            //I need to do this off of the damage frequency function instead
            foreach (UncertainPairedData stageDamage in _DamageCategoryStageDamage)
            {
                foreach ((CurveMetaData, PairedData) metaData in damageFrequencyFunctions)
                {
                    if (stageDamage.CurveMetaData.Equals(metaData.Item1))
                    {
                        _ImpactAreaScenarioResults.ConsequenceResults.AddNewConsequenceResultObject(metaData.Item1.DamageCategory, metaData.Item1.AssetCategory, convergenceCriteria, _ImpactAreaID);
                    }
                }
            }
        }

        private void LogSimulationErrors()
        {

            //get the highest error level. This is so that we can log the intro message at that error level so that the filter will either
            //show the intro message and an actual error message or it won't show either.
            List<string> validationIntroMessages = new();
            List<ValidationErrorLogger> validationObjects = new()
            {
                _FrequencyDischarge
            };
            validationIntroMessages.Add(nameof(_FrequencyDischarge) + $" has the following messages for the impact area with ID {_ImpactAreaID}:");

            validationObjects.Add(_FrequencyDischargeGraphical);
            validationIntroMessages.Add(nameof(_FrequencyDischargeGraphical) + $" has the following messages for the impact area with ID {_ImpactAreaID}:");

            validationObjects.Add(_UnregulatedRegulated);
            validationIntroMessages.Add(nameof(_UnregulatedRegulated) + $" has the following messages for the impact area with ID {_ImpactAreaID}:");

            validationObjects.Add(_DischargeStage);
            validationIntroMessages.Add(nameof(_DischargeStage) + $" has the following messages for the impact area with ID {_ImpactAreaID}:");

            validationObjects.Add(_FrequencyStage);
            validationIntroMessages.Add(nameof(_FrequencyStage) + $" has the following messages for the impact area with ID {_ImpactAreaID}:");

            validationObjects.Add(_ChannelStageFloodplainStage);
            validationIntroMessages.Add(nameof(_ChannelStageFloodplainStage) + $" has the following messages for the impact area with ID {_ImpactAreaID}:");

            validationObjects.Add(_SystemResponseFunction);
            validationIntroMessages.Add(nameof(_SystemResponseFunction) + $" has the following messages for the impact area with ID {_ImpactAreaID}:");

            foreach (UncertainPairedData relationship in _DamageCategoryStageDamage)
            {
                validationObjects.Add(relationship);
                validationIntroMessages.Add(nameof(_DamageCategoryStageDamage) + ": " + relationship.CurveMetaData.DamageCategory + ": " + relationship.CurveMetaData.AssetCategory + ": " + "has the following messages");
            }

            //TODO - is there a reason for keeping this commented out code? 
            //    ReportMessage(this, new MessageEventArgs(new ErrorMessage($"The simulation for impact area {_impactAreaID} contains warnings:", errorLevel)));

            //    _frequency_discharge?.LogErrors(nameof(_frequency_discharge) + $" has the following messages for the impact area with ID {_impactAreaID}:");
            //    _frequency_discharge_graphical?.LogErrors(nameof(_frequency_discharge_graphical) + $" has the following messages for the impact area with ID {_impactAreaID}:");
            //    _unregulated_regulated?.LogErrors(nameof(_unregulated_regulated) + $" has the following messages for the impact area with ID {_impactAreaID}:");
            //    _discharge_stage?.LogErrors(nameof(_discharge_stage) + $" has the following messages for the impact area with ID {_impactAreaID}:");
            //    _frequency_stage?.LogErrors(nameof(_frequency_stage) + $" has the following messages for the impact area with ID {_impactAreaID}:");
            //    _channelstage_floodplainstage?.LogErrors(nameof(_channelstage_floodplainstage) + $" has the following messages for the impact area with ID {_impactAreaID}:");
            //    _systemResponseFunction_stage_failureProbability?.LogErrors(nameof(_systemResponseFunction_stage_failureProbability) + $" has the following messages for the impact area with ID {_impactAreaID}:");

            //    foreach (UncertainPairedData relationship in _damage_category_stage_damage)
            //    {
            //        relationship?.LogErrors(nameof(_damage_category_stage_damage) + ": " + relationship.CurveMetaData.DamageCategory + ": " + relationship.CurveMetaData.AssetCategory + ": " + "has the following messages");
            //    }
            //}
        }

        private bool CanCompute(ConvergenceCriteria convergenceCriteria, IProvideRandomNumbers randomProvider)
        {
            bool canCompute = true;
            if (HasErrors)
            {
                if (ErrorLevel >= ErrorLevel.Fatal)
                {
                    ReportMessage(this, new MessageEventArgs(new Message($"The simulation for impact area {_ImpactAreaID} contains errors. The compute has been aborted." + Environment.NewLine)));
                    canCompute = false;
                }
                else
                {
                    LogSimulationErrors();
                    if (randomProvider is MedianRandomProvider)
                    {
                        if (convergenceCriteria.MaxIterations != 1)
                        {

                            string message = $"The simulation for impact area {_ImpactAreaID} was requested to provide a mean estimate, but asked for more than one iteration." + Environment.NewLine;
                            ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                            ReportMessage(this, new MessageEventArgs(errorMessage));
                            canCompute = false;
                        }
                    }
                    else if (convergenceCriteria.MinIterations < 100)
                    {
                        string message = $"The simulation for impact area {_ImpactAreaID} was requested to provide a random estimate, but asked for a minimum of one iteration." + Environment.NewLine;
                        ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                        ReportMessage(this, new MessageEventArgs(errorMessage));
                        canCompute = false;
                    }
                    //TODO if curves do not overlap we don't have a way here of saying HasErrors = true 
                    //Nor is there relevant messaging 
                    //Cody added a simple error message below, but I think we probably want the sim overlap method to return a string
                    // and not a bool so that it can pass out a better message. Maybe we should add my FDA Validation Result object into the model
                    //so that we can return that object.
                    bool curvesOverlap = SimulationCurvesHaveOverlap();
                    if (!curvesOverlap)
                    {
                        ErrorMessage errorMessage = new("The simulation contains curves that do not overlap.", ErrorLevel.Fatal);
                        ReportMessage(this, new MessageEventArgs(errorMessage));
                        canCompute = false;
                    }
                    //TODO if convergence criteria is not valid, we don't have a way of saying HasErrors = true 
                    //nor is there relevant messaging
                    convergenceCriteria.Validate();
                    if (convergenceCriteria.HasErrors)
                    {
                        canCompute = false;
                    }
                }
            }
            return canCompute;

        }

        private void ComputeIterations(ConvergenceCriteria convergenceCriteria, IProvideRandomNumbers randomProvider, int masterseed, bool computeWithDamage, bool computeIsDeterministic, CancellationToken cancellationToken)
        {
            long completedIterations = 0;
            long expectedIterations = convergenceCriteria.MinIterations;
            Random masterSeedList = new(masterseed);//must be seeded.
            int[] seeds = new int[convergenceCriteria.MaxIterations];
            for (int i = 0; i < convergenceCriteria.MaxIterations; i++)
            {
                seeds[i] = masterSeedList.Next();
            }
            Int64 computeChunks = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(convergenceCriteria.MinIterations) / convergenceCriteria.IterationCount));
            if (computeChunks < 1)
            {
                computeChunks = 1;
            }
            bool computeIsNotConverged = true;
            while (computeIsNotConverged)
            {
                int j = 0;
                while (j < computeChunks)
                {
                    try
                    {
                        Parallel.For(0, convergenceCriteria.IterationCount, i =>
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
                            if (_FrequencyStage.CurveMetaData.IsNull)
                            {
                                if (_DischargeStage.CurveMetaData.IsNull)
                                {
                                    //complain loudly
                                    string message = $"A stage-discharge function must accompany a discharge-frequency function but was not found for the impact area with ID {_ImpactAreaID}. Compute aborted." + Environment.NewLine;
                                    ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                                    ReportMessage(this, new MessageEventArgs(errorMessage));
                                    return;
                                }
                                IPairedData frequencyDischarge;
                                if (_FrequencyDischargeGraphical.CurveMetaData.IsNull)
                                {
                                    //If threadlocalRandomProvider is medianRandomProvider then we get a quasi-deterministic result
                                    frequencyDischarge = _FrequencyDischarge.BootstrapToPairedData(threadlocalRandomProvider, _RequiredExceedanceProbabilities );//ordinates defines the number of values in the frequency curve, more would be a better approximation.                                                                                                                  
                                }
                                else
                                {
                                    //If threadlocalRandomProvider is medianRandomProvider then we get a quasi-deterministic result
                                    frequencyDischarge = _FrequencyDischargeGraphical.SamplePairedData(threadlocalRandomProvider.NextRandom());
                                }
                                //if frequency_flow is not defined throw big errors.
                                //check if flow transform exists, and use it here
                                if (_UnregulatedRegulated.CurveMetaData.IsNull)
                                {
                                    IPairedData discharge_stage_sample = _DischargeStage.SamplePairedData(threadlocalRandomProvider.NextRandom(), computeIsDeterministic);
                                    IPairedData frequency_stage = discharge_stage_sample.compose(frequencyDischarge);
                                    ComputeFromStageFrequency(threadlocalRandomProvider, frequency_stage, i, computeWithDamage, computeIsDeterministic);
                                }
                                else
                                {
                                    IPairedData inflow_outflow_sample = _UnregulatedRegulated.SamplePairedData(threadlocalRandomProvider.NextRandom(), computeIsDeterministic); //should be a random number
                                    IPairedData transformff = inflow_outflow_sample.compose(frequencyDischarge);
                                    IPairedData discharge_stage_sample = _DischargeStage.SamplePairedData(threadlocalRandomProvider.NextRandom(), computeIsDeterministic);//needs to be a random number
                                    IPairedData frequency_stage = discharge_stage_sample.compose(transformff);
                                    ComputeFromStageFrequency(threadlocalRandomProvider, frequency_stage, i, computeWithDamage, computeIsDeterministic);
                                }

                            }
                            else
                            {
                                //if threadlocalRandomProvider is medianRandomProvider then we get a quasi-deterministic result
                                IPairedData frequency_stage_sample = _FrequencyStage.SamplePairedData(threadlocalRandomProvider.NextRandom());
                                ComputeFromStageFrequency(threadlocalRandomProvider, frequency_stage_sample, i, computeWithDamage, computeIsDeterministic);
                            }
                        });
                        _ImpactAreaScenarioResults.ConsequenceResults.PutDataIntoHistograms();
                        foreach (var thresholdEntry in _ImpactAreaScenarioResults.PerformanceByThresholds.ListOfThresholds)
                        {
                            thresholdEntry.SystemPerformanceResults.PutDataIntoHistograms();
                        }
                        completedIterations += convergenceCriteria.IterationCount;
                        //report progress
                        double percentcomplete = (completedIterations / (double)expectedIterations) * 100;
                        ReportProgress(this, new ProgressReportEventArgs((int)percentcomplete));
                        j++;
                    }
                    //I learned that you cannot throw an exception in a parallel for loop and expect it to work.
                    //According to the internet you need to catch an aggregateException and then use it find the
                    //exception that you care about and re-throw it.
                    catch (AggregateException ae)
                    {
                        foreach (var ex in ae.Flatten().InnerExceptions)
                        {
                            if (ex is TaskCanceledException)
                            {
                                throw new TaskCanceledException();
                            }
                        }
                    }
                }
                if (!_ImpactAreaScenarioResults.ResultsAreConverged(.95, .05, computeWithDamage))
                {
                    //recalculate compute chunks 
                    expectedIterations = _ImpactAreaScenarioResults.RemainingIterations(.95, .05, computeWithDamage);
                    computeChunks = Convert.ToInt64(expectedIterations / convergenceCriteria.IterationCount);
                    if (computeChunks == 0)
                    {
                        computeChunks = 1;
                    }
                    j = 0;
                }
                else
                {
                    ReportMessage(this, new MessageEventArgs(new ComputeCompleteMessage(completedIterations, _ImpactAreaID)));
                    computeIsNotConverged = false;
                    break;
                }

            }
            ReportProgress(this, new ProgressReportEventArgs(IMPACT_AREA_SIM_COMPLETED));
        }

        private void ComputeFromStageFrequency(IProvideRandomNumbers randomProvider, IPairedData frequency_stage, long iteration, bool computeWithDamage, bool computeIsDeterministic)
        {

            //interior exterior
            if (_ChannelStageFloodplainStage.CurveMetaData.IsNull)
            {
                //levees
                if (_SystemResponseFunction.CurveMetaData.IsNull)
                {
                    if (computeWithDamage)
                    {
                        ComputeDamagesFromStageFrequency(randomProvider, frequency_stage, iteration, computeIsDeterministic);
                    }
                    ComputePerformance(frequency_stage, Convert.ToInt32(iteration));
                }
                else
                {
                    if (_LeveeIsValid)
                    {
                        //TODO: why commented out and why still exists
                        IPairedData systemResponse_sample = _SystemResponseFunction.SamplePairedData(randomProvider.NextRandom(), computeIsDeterministic); //needs to be a random number
                                                                                                                                                           //IPairedData frequency_stage_withLevee = frequency_stage.multiply(levee_curve_sample);
                        if (computeWithDamage)
                        {
                            ComputeDamagesFromStageFrequency_WithLevee(randomProvider, frequency_stage, systemResponse_sample, iteration, computeIsDeterministic);
                        }
                        //If the system response function is the default function 
                        if (systemResponse_sample.Xvals.Length <= 2)
                        {
                            ComputePerformance(frequency_stage, Convert.ToInt32(iteration));
                        }
                        else
                        {
                            ComputeLeveePerformance(frequency_stage, systemResponse_sample, Convert.ToInt32(iteration));
                        }
                    }

                }

            }
            else
            {   //todo is there a reason for the starting underscore? 
                IPairedData _channelstage_floodplainstage_sample = _ChannelStageFloodplainStage.SamplePairedData(randomProvider.NextRandom(), computeIsDeterministic); //needs to be a random number
                IPairedData frequency_floodplainstage = _channelstage_floodplainstage_sample.compose(frequency_stage);
                //levees
                if (_SystemResponseFunction.CurveMetaData.IsNull)
                {
                    if (computeWithDamage)
                    {
                        ComputeDamagesFromStageFrequency(randomProvider, frequency_floodplainstage, iteration, computeIsDeterministic);
                    }
                    ComputePerformance(frequency_floodplainstage, Convert.ToInt32(iteration));
                }
                else
                {
                    if (_LeveeIsValid)
                    {//TODO: why commented out and why still exists
                        IPairedData systemResponse_sample = _SystemResponseFunction.SamplePairedData(randomProvider.NextRandom(), computeIsDeterministic); //needs to be a random number
                                                                                                                                                           //IPairedData frequency_floodplainstage_withLevee = frequency_floodplainstage.multiply(_levee_curve_sample);
                        if (computeWithDamage)
                        {
                            ComputeDamagesFromStageFrequency_WithLeveeAndInteriorExterior(randomProvider, _channelstage_floodplainstage_sample, frequency_stage, systemResponse_sample, iteration, computeIsDeterministic);
                        }
                        //If the system response function is the default function
                        if (systemResponse_sample.Xvals.Length <= 2)
                        {
                            ComputePerformance(frequency_stage, Convert.ToInt32(iteration));
                        }
                        else
                        {
                            ComputeLeveePerformance(frequency_stage, systemResponse_sample, Convert.ToInt32(iteration));
                        }
                    }

                }

            }
        }

        private void ComputeDamagesFromStageFrequency(IProvideRandomNumbers randomProvider, IPairedData frequency_stage, long iteration, bool computeIsDeterministic)
        {
            foreach (UncertainPairedData stageUncertainDamage in _DamageCategoryStageDamage)
            {
                //TODO: here we need to check if stage damage is zero 
                //if so, then skip this stuff and just add 0 to consequenceResults
                IPairedData _stage_damage_sample = stageUncertainDamage.SamplePairedData(randomProvider.NextRandom(), computeIsDeterministic);
                IPairedData frequency_damage = _stage_damage_sample.compose(frequency_stage);
                double eadEstimate = frequency_damage.integrate();
                _ImpactAreaScenarioResults.ConsequenceResults.AddConsequenceRealization(eadEstimate, stageUncertainDamage.CurveMetaData.DamageCategory, stageUncertainDamage.CurveMetaData.AssetCategory, _ImpactAreaID, iteration);
            }
        }
        private void ComputeDamagesFromStageFrequency_WithLevee(IProvideRandomNumbers randomProvider, IPairedData frequency_stage, IPairedData systemResponse, long iteration, bool computeIsDeterministic)
        {
            foreach (UncertainPairedData stageUncertainDamage in _DamageCategoryStageDamage)
            {
                IPairedData stage_damage_sample = stageUncertainDamage.SamplePairedData(randomProvider.NextRandom(), computeIsDeterministic);//needs to be a random number
                                                                                                                                             //here we need to compose with interior exterior 
                IPairedData stage_damage_sample_withLevee = stage_damage_sample.multiply(systemResponse);
                IPairedData frequency_damage = stage_damage_sample_withLevee.compose(frequency_stage);
                double eadEstimate = frequency_damage.integrate();
                _ImpactAreaScenarioResults.ConsequenceResults.AddConsequenceRealization(eadEstimate, stageUncertainDamage.CurveMetaData.DamageCategory, stageUncertainDamage.CurveMetaData.AssetCategory, _ImpactAreaID, iteration);
            }

        }
        private void ComputeDamagesFromStageFrequency_WithLeveeAndInteriorExterior(IProvideRandomNumbers randomProvider, IPairedData exterior_interior, IPairedData frequency_exteriorStage, IPairedData systemResponse, long iteration, bool computeIsDeterministic)
        {
            foreach (UncertainPairedData stageUncertainDamage in _DamageCategoryStageDamage)
            {   //TODO: why are we doing this stuff with the underscores? I think this needs to be cleaned up 
                IPairedData interiorStage_damage_sample = stageUncertainDamage.SamplePairedData(randomProvider.NextRandom(), computeIsDeterministic);//needs to be a random number
                IPairedData exteriorStage_damage_sample = interiorStage_damage_sample.compose(exterior_interior);
                IPairedData stage_damage_sample_withLevee = exteriorStage_damage_sample.multiply(systemResponse);
                IPairedData frequency_damage = stage_damage_sample_withLevee.compose(frequency_exteriorStage);
                double eadEstimate = frequency_damage.integrate();
                _ImpactAreaScenarioResults.ConsequenceResults.AddConsequenceRealization(eadEstimate, stageUncertainDamage.CurveMetaData.DamageCategory, stageUncertainDamage.CurveMetaData.AssetCategory, _ImpactAreaID, iteration);
            }

        }
        //TODO: Opportunity for refactor: move performance functions to system performance statistics
        public void ComputePerformance(IPairedData frequency_stage, int iteration)
        {

            foreach (var thresholdEntry in _ImpactAreaScenarioResults.PerformanceByThresholds.ListOfThresholds)
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
            double finalProbOfStageInRange = 1 - levee_frequency_stage.Xvals[^1];
            double finalAvgProbFailure = levee_frequency_stage.Yvals[^1];
            aep += finalProbOfStageInRange * finalAvgProbFailure;
            foreach (var thresholdEntry in _ImpactAreaScenarioResults.PerformanceByThresholds.ListOfThresholds)
            {
                thresholdEntry.SystemPerformanceResults.AddAEPForAssurance(aep, iteration);
                GetStageForNonExceedanceProbability(frequency_stage, thresholdEntry, iteration);
            }

        }

        public static void GetStageForNonExceedanceProbability(IPairedData frequency_stage, Threshold threshold, int iteration)
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
         //TODO: I think that we need to calculate bin width here 
            double[] er101RequiredNonExceedanceProbabilities = new double[] { .9, .96, .98, .99, .996, .998 };
            foreach (var thresholdEntry in _ImpactAreaScenarioResults.PerformanceByThresholds.ListOfThresholds)
            {
                for (int i = 0; i < er101RequiredNonExceedanceProbabilities.Length; i++)
                {
                    thresholdEntry.SystemPerformanceResults.AddStageAssuranceHistogram(er101RequiredNonExceedanceProbabilities[i]);
                }
            }
        }
        internal static PairedData ComputeTotalStageDamage(List<UncertainPairedData> listOfUncertainPairedData)
        {
            CurveMetaData metadata = new("Total", "Total");
            PairedData totalStageDamage = new(null, null, metadata);
            MedianRandomProvider meanRandomProvider = new();
            foreach (UncertainPairedData uncertainPairedData in listOfUncertainPairedData)
            {
                IPairedData stageDamageSample = uncertainPairedData.SamplePairedData(meanRandomProvider.NextRandom());
                totalStageDamage = totalStageDamage.SumYsForGivenX(stageDamageSample);
            }
            return totalStageDamage;
        }
        private Threshold ComputeDefaultThreshold(ConvergenceCriteria convergenceCriteria, bool computeWithDamage, List<(CurveMetaData, PairedData)> damageFrequencyFunctions)
        {
            if (_SystemResponseFunction.IsNull)
            {
                //this would happen if the intention is to compute without stage damage for NFIP 
                //but the user didn't enter a levee to evaluate 
                //but we can't calculate the default threshold because it otherwise depends on damage 
                if (computeWithDamage.Equals(false))
                {
                    string message = $"A threshold for Impact Area with ID {_ImpactAreaID} could not be calculated because there is no levee in memory. An arbitrary threshold is being used." + Environment.NewLine;
                    ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                    ReportMessage(this, new MessageEventArgs(errorMessage));
                    return new Threshold();
                }
                IPairedData totalStageDamage = ComputeTotalStageDamage(_DamageCategoryStageDamage);
                PairedData totalFrequencyDamage = new(new double[] { 0 }, new double[] { 0 });
                bool firstPass = true;
                foreach ((CurveMetaData, PairedData) metaData in damageFrequencyFunctions)
                {
                    if (firstPass)
                    {
                        totalFrequencyDamage = metaData.Item2;
                        firstPass = false;
                    }
                    else
                    {
                        totalFrequencyDamage = totalFrequencyDamage.SumYsForGivenX(metaData.Item2);
                    }
                }
                double thresholdDamage = THRESHOLD_DAMAGE_PERCENT * totalFrequencyDamage.f(THRESHOLD_DAMAGE_RECURRENCE_INTERVAL);
                double thresholdStage = totalStageDamage.f_inverse(thresholdDamage);
                return new Threshold(DEFAULT_THRESHOLD_ID, convergenceCriteria, ThresholdEnum.DefaultExteriorStage, thresholdStage);
            }
            else
            {
                //If the system response function is the default function 
                if (_SystemResponseFunction.Xvals.Length <= 2)
                {
                    return new Threshold(DEFAULT_THRESHOLD_ID, _SystemResponseFunction, convergenceCriteria, ThresholdEnum.TopOfLevee, _TopOfLeveeElevation);

                }
                else
                {
                    return new Threshold(DEFAULT_THRESHOLD_ID, _SystemResponseFunction, convergenceCriteria, ThresholdEnum.LeveeSystemResponse, _TopOfLeveeElevation);
                }
            }
        }



        private List<(CurveMetaData, PairedData)> ComputeDamageFrequency(bool computeIsDeterministic)
        {
            MedianRandomProvider meanRandomProvider = new();
            double[] xs = new double[] { 0 };
            double[] ys = new double[] { 0 };
            IPairedData frequencyStage;

            List<(CurveMetaData, PairedData)> damageFrequency = new();

            if (_DamageCategoryStageDamage.Count == 0)
            {
                string message = $"A valid damage frequency cannot be calculated for the impact area with ID {_ImpactAreaID} because no stage-damage functions were found. A meaningless default threshold of 0 will be used. Please have an additional threshold for meaningful performance statistics" + Environment.NewLine;
                ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage));
                PairedData fakePairedData = new(xs, ys);
                damageFrequency.Add((new CurveMetaData(), fakePairedData));
                return damageFrequency;
            }
            else
            {
                foreach (UncertainPairedData stageDamageFunction in _DamageCategoryStageDamage)
                {
                    if (_FrequencyStage.CurveMetaData.IsNull)
                    {
                        IPairedData frequencyFlow;
                        if (_FrequencyDischargeGraphical.CurveMetaData.IsNull)
                        {
                            frequencyFlow = _FrequencyDischarge.BootstrapToPairedData(meanRandomProvider, _RequiredExceedanceProbabilities);
                        }
                        else
                        {
                            frequencyFlow = _FrequencyDischargeGraphical.SamplePairedData(meanRandomProvider.NextRandom());
                        }
                        if (_UnregulatedRegulated.CurveMetaData.IsNull)
                        {
                            if (_DischargeStage.CurveMetaData.IsNull)
                            {
                                string message = $"A stage-discharge function must accompany a discharge-frequency function but was not found for the impact area with ID {_ImpactAreaID}. An arbitrary threshold is being used." + Environment.NewLine;
                                ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                                ReportMessage(this, new MessageEventArgs(errorMessage));
                                PairedData fakePairedData = new(xs, ys);
                                damageFrequency.Add((new CurveMetaData(), fakePairedData));
                                return damageFrequency;

                            }
                            else
                            {
                                IPairedData flowStageSample = _DischargeStage.SamplePairedData(meanRandomProvider.NextRandom(), computeIsDeterministic);
                                frequencyStage = flowStageSample.compose(frequencyFlow);
                            }
                        }
                        else
                        {
                            IPairedData inflowOutflowSample = _UnregulatedRegulated.SamplePairedData(meanRandomProvider.NextRandom(), computeIsDeterministic);
                            IPairedData transformFlowFrequency = inflowOutflowSample.compose(frequencyFlow);
                            if (_DischargeStage.CurveMetaData.IsNull)
                            {
                                string message = $"A stage-discharge function must accompany a discharge-frequency function but was not found for the impact area with ID {_ImpactAreaID}. An arbitrary threshold is being used." + Environment.NewLine;
                                ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                                ReportMessage(this, new MessageEventArgs(errorMessage));
                                PairedData fakePairedData = new(xs, ys);
                                damageFrequency.Add((new CurveMetaData(), fakePairedData));
                                return damageFrequency;
                            }
                            else
                            {
                                IPairedData flowStageSample = _DischargeStage.SamplePairedData(meanRandomProvider.NextRandom(), computeIsDeterministic);
                                frequencyStage = flowStageSample.compose(transformFlowFrequency);
                            }
                        }

                    }
                    else
                    {
                        frequencyStage = _FrequencyStage.SamplePairedData(meanRandomProvider.NextRandom());
                    }

                    IPairedData stageDamage = stageDamageFunction.SamplePairedData(meanRandomProvider.NextRandom(), computeIsDeterministic);
                    if (_ChannelStageFloodplainStage.IsNull)
                    {

                        damageFrequency.Add((stageDamageFunction.CurveMetaData, (PairedData)stageDamage.compose(new PairedData(xs, ys))));
                    }
                    else
                    {
                        IPairedData exteriorInterior = _ChannelStageFloodplainStage.SamplePairedData(meanRandomProvider.NextRandom(), computeIsDeterministic);
                        IPairedData frequencyInteriorStage = exteriorInterior.compose(new PairedData(xs, ys));
                        damageFrequency.Add((stageDamageFunction.CurveMetaData, (PairedData)stageDamage.compose(frequencyInteriorStage)));
                    }

                }
                return damageFrequency;
            }

        }






        internal static PairedData ComputeTotalDamageFrequency(PairedData pairedDataTotal, PairedData pairedDataToBeAddedToTotal)
        {
            pairedDataTotal = pairedDataTotal.SumYsForGivenX(pairedDataToBeAddedToTotal);
            return pairedDataTotal;
        }

        public ImpactAreaScenarioResults PreviewCompute()
        {

            MedianRandomProvider meanRandomProvider = new();
            ConvergenceCriteria convergenceCriteria = new(minIterations: 1, maxIterations: 1);
            ImpactAreaScenarioResults results = Compute(meanRandomProvider, convergenceCriteria, new CancellationTokenSource().Token, false, true, computeIsDeterministic: true);
            return results;
        }
        public static SimulationBuilder Builder(int impactAreaID)
        {
            return new SimulationBuilder(new ImpactAreaScenarioSimulation(impactAreaID));
        }

        private bool LeveeIsValid()
        {
            if (_SystemResponseFunction.CurveMetaData.IsNull) return false;
            if (_SystemResponseFunction.Yvals.Last().Type != IDistributionEnum.Deterministic)
            {
                string message = $"There must exist a stage in the fragility curve with a certain probability of failure specified as a deterministic distribution but was not found for the impact area with ID {_ImpactAreaID}" + Environment.NewLine;
                ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage));
                return false;
            }
            else if (_SystemResponseFunction.Yvals.Last().InverseCDF(0.5) != 1) //we should be given a deterministic distribution at the end where prob(failure) = 1
            { //the determinstic distribution could be normal with zero standard deviation, triangular or uniform with min and max = 1, doesn't matter
              //distributions where the user specifies zero variability should be passed to the model as a deterministic distribution 
              //this has been communicated 
                string message = $"There must exist a stage in the fragility curve with a certain probability of failure specified as a deterministic distribution for the impact area with ID {_ImpactAreaID}" + Environment.NewLine;
                ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
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
            int index = Array.BinarySearch(_SystemResponseFunction.Xvals, _TopOfLeveeElevation);
            if (index > 0)
            {
                if (_SystemResponseFunction.Yvals[index].InverseCDF(0.5) != 1)
                {//top of levee elevation has some probability other than 1
                    string message = $"The top of levee elevation of {_TopOfLeveeElevation} in the fragility function does not have certain probability of failure specified as a deterministic distribution for the impact area with ID {_ImpactAreaID}" + Environment.NewLine;
                    ErrorMessage errorMessage = new(message, ErrorLevel.Major);
                    ReportMessage(this, new MessageEventArgs(errorMessage));
                }
            }
            else
            {   //top of levee elevation is not included in the fragility curve
                string message = $"The top of levee elevation of {_TopOfLeveeElevation} in the fragility function does not have a certain probability of failure specified as a deterministic distribution for the impact area with ID {_ImpactAreaID}" + Environment.NewLine;
                ErrorMessage errorMessage = new(message, ErrorLevel.Major);
                ReportMessage(this, new MessageEventArgs(errorMessage));
            }
        }

        //TODO: Add messaging to indicate which curves do not overlap
        private bool SimulationCurvesHaveOverlap()
        {
            bool allCurvesHaveOverlap = true;
            if (_FrequencyStage.CurveMetaData.IsNull)
            {
                if (_DischargeStage.CurveMetaData.IsNull)
                {
                    string message = $"A stage-discharge function must accompany a discharge-frequency function but was not found for the impact area with ID {_ImpactAreaID}. Compute aborted." + Environment.NewLine;
                    ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                    ReportMessage(this, new MessageEventArgs(errorMessage));
                    allCurvesHaveOverlap = false;
                }
                if (_FrequencyDischargeGraphical.CurveMetaData.IsNull)
                {
                    bool nextTwoCurvesOverlap = true;
                    bool firstTwoCurvesOverlap;
                    if (_UnregulatedRegulated.CurveMetaData.IsNull)
                    {
                        firstTwoCurvesOverlap = CurvesHaveOverlap(_DischargeStage, _FrequencyDischarge);
                    }
                    else
                    {
                        firstTwoCurvesOverlap = CurvesHaveOverlap(_UnregulatedRegulated, _FrequencyDischarge);
                        nextTwoCurvesOverlap = CurvesHaveOverlap(_DischargeStage, _UnregulatedRegulated);
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
                    bool nextTwoCurvesOverlap = true;
                    bool firstTwoCurvesOverlap;
                    if (_UnregulatedRegulated.CurveMetaData.IsNull)
                    {
                        firstTwoCurvesOverlap = CurvesHaveOverlap(_DischargeStage, _FrequencyDischargeGraphical);
                    }
                    else
                    {
                        firstTwoCurvesOverlap = CurvesHaveOverlap(_UnregulatedRegulated, _FrequencyDischargeGraphical);
                        nextTwoCurvesOverlap = CurvesHaveOverlap(_DischargeStage, _UnregulatedRegulated);
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
                if (_ChannelStageFloodplainStage.CurveMetaData.IsNull)
                {
                    foreach (UncertainPairedData uncertainPairedData in _DamageCategoryStageDamage)
                    {
                        bool stageDamageOverlaps = CurvesHaveOverlap(uncertainPairedData, _DischargeStage);
                        if (!stageDamageOverlaps)
                        {
                            allCurvesHaveOverlap = stageDamageOverlaps;
                        }
                    }
                }
                else
                {
                    bool nextTwoCurvesHaveOverlap = CurvesHaveOverlap(_ChannelStageFloodplainStage, _DischargeStage);
                    if (!nextTwoCurvesHaveOverlap)
                    {
                        allCurvesHaveOverlap = nextTwoCurvesHaveOverlap;
                    }
                    foreach (UncertainPairedData uncertain in _DamageCategoryStageDamage)
                    {
                        bool stageDamageOverlaps = CurvesHaveOverlap(uncertain, _ChannelStageFloodplainStage);
                        if (!stageDamageOverlaps)
                        {
                            allCurvesHaveOverlap = stageDamageOverlaps;
                        }
                    }
                }
            }
            else
            {
                if (!_ChannelStageFloodplainStage.CurveMetaData.IsNull)
                {
                    bool nextTwoCurvesHaveOverlap = CurvesHaveOverlap(_ChannelStageFloodplainStage, _FrequencyStage);
                    if (!nextTwoCurvesHaveOverlap)
                    {
                        allCurvesHaveOverlap = nextTwoCurvesHaveOverlap;
                    }
                    foreach (UncertainPairedData uncertain in _DamageCategoryStageDamage)
                    {
                        bool stageDamageOverlaps = CurvesHaveOverlap(uncertain, _ChannelStageFloodplainStage);
                        if (!stageDamageOverlaps)
                        {
                            allCurvesHaveOverlap = stageDamageOverlaps;
                        }
                    }
                }
                else
                {
                    foreach (UncertainPairedData uncertain in _DamageCategoryStageDamage)
                    {
                        bool stageDamageOverlaps = CurvesHaveOverlap(uncertain, _FrequencyStage);
                        if (!stageDamageOverlaps)
                        {
                            allCurvesHaveOverlap = stageDamageOverlaps;
                        }
                    }
                }
            }
            return allCurvesHaveOverlap;
        }
        private static bool CurvesHaveOverlap(UncertainPairedData uncertainPairedData_f, UncertainPairedData uncertainPairedData_g)
        {
            double maxOfF = uncertainPairedData_f.Xvals[uncertainPairedData_f.Yvals.Length - 1];
            double minOfF = uncertainPairedData_f.Xvals[0];
            double minOfG = uncertainPairedData_g.Yvals[0].InverseCDF(.001);
            double maxOfG = uncertainPairedData_g.Yvals[^1].InverseCDF(.999);

            bool curvesOverlap = CurvesOverlap(maxOfF, minOfF, maxOfG, minOfG);
            return curvesOverlap;
        }
        private static bool CurvesHaveOverlap(UncertainPairedData uncertainPairedData_f, GraphicalUncertainPairedData uncertainPairedData_g)
        {
            double maxOfF = uncertainPairedData_f.Xvals[^1];
            double minOfF = uncertainPairedData_f.Xvals[0];
            double minOfG = uncertainPairedData_g.InputFlowOrStageValues[0];
            double maxOfG = uncertainPairedData_g.InputFlowOrStageValues[^1];

            bool curvesOverlap = CurvesOverlap(maxOfF, minOfF, maxOfG, minOfG);
            return curvesOverlap;
        }
        private static bool CurvesHaveOverlap(UncertainPairedData uncertainPairedData_f, ContinuousDistribution continuousDistribution_g)
        {
            double maxOfF = uncertainPairedData_f.Xvals[^1];
            double minOfF = uncertainPairedData_f.Xvals[0];
            double minOfG = continuousDistribution_g.InverseCDF(.001);
            double maxOfG = continuousDistribution_g.InverseCDF(.75);

            bool curvesOverlap = CurvesOverlap(maxOfF, minOfF, maxOfG, minOfG);
            return curvesOverlap;
        }
        private static bool CurvesOverlap(double maxOfF, double minOfF, double maxOfG, double minOfG)
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

        public void ReportProgress(object sender, ProgressReportEventArgs e)
        {
            ProgressReport?.Invoke(sender, e);
        }
        public bool Equals(ImpactAreaScenarioSimulation incomingImpactAreaScenarioSimulation)
        {
            bool sameLeveeElevation = _TopOfLeveeElevation.Equals(incomingImpactAreaScenarioSimulation._TopOfLeveeElevation);
            if (!sameLeveeElevation)
            {
                return false;
            }
            bool sameImpactArea = _ImpactAreaID.Equals(incomingImpactAreaScenarioSimulation._ImpactAreaID);
            if (!sameImpactArea)
            {
                return false;
            }
            bool leveeValidityMatches = _LeveeIsValid.Equals(incomingImpactAreaScenarioSimulation._LeveeIsValid);
            if (!leveeValidityMatches)
            {
                return false;
            }
            bool frequenceDischargeMatches = _FrequencyDischarge.Equals(incomingImpactAreaScenarioSimulation._FrequencyDischarge);
            if (!frequenceDischargeMatches)
            {
                return false;
            }
            bool frequencyDischargeGraphicalMatches = _FrequencyDischargeGraphical.Equals(incomingImpactAreaScenarioSimulation._FrequencyDischargeGraphical);
            if (!frequencyDischargeGraphicalMatches)
            {
                return false;
            }
            bool regulatedUnregulatedMatches = _UnregulatedRegulated.Equals(incomingImpactAreaScenarioSimulation._UnregulatedRegulated);
            if (!regulatedUnregulatedMatches)
            {
                return false;
            }
            bool dischargeStageMatches = _DischargeStage.Equals(incomingImpactAreaScenarioSimulation._DischargeStage);
            if (!dischargeStageMatches)
            {
                return false;
            }
            bool frequencyStageMatches = _FrequencyStage.Equals(incomingImpactAreaScenarioSimulation._FrequencyStage);
            if (!frequencyStageMatches)
            {
                return false;
            }
            bool interiorExteriorMatches = _ChannelStageFloodplainStage.Equals(incomingImpactAreaScenarioSimulation._ChannelStageFloodplainStage);
            if (!interiorExteriorMatches)
            {
                return false;
            }
            bool systemResponseMatches = _SystemResponseFunction.Equals(incomingImpactAreaScenarioSimulation._SystemResponseFunction);
            if (!systemResponseMatches)
            {
                return false;
            }
            bool resultsMatch = _ImpactAreaScenarioResults.Equals(incomingImpactAreaScenarioSimulation._ImpactAreaScenarioResults);
            if (!resultsMatch)
            {
                return false;
            }
            foreach (UncertainPairedData stageDamage in _DamageCategoryStageDamage)
            {
                foreach (UncertainPairedData incomingStageDamage in incomingImpactAreaScenarioSimulation._DamageCategoryStageDamage)
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
            XElement mainElement = new("ImpactAreaScenarioSimulation");

            mainElement.SetAttributeValue("LeveeIsValid", _LeveeIsValid);
            mainElement.SetAttributeValue("TopOfLeveeElevation", _TopOfLeveeElevation);
            mainElement.SetAttributeValue("ImpactAreaID", _ImpactAreaID);
            bool frequencyDischargeIsNull = ((Statistics.Distributions.LogPearson3)_FrequencyDischarge).IsNull;
            mainElement.SetAttributeValue("FrequencyDischargeIsNull", frequencyDischargeIsNull);
            if (!frequencyDischargeIsNull)
            {
                XElement frequenceDischarge = _FrequencyDischarge.ToXML();
                frequenceDischarge.Name = "LogPearson3";
                mainElement.Add(frequenceDischarge);
            }

            XElement frequencyDischargeGraphical = _FrequencyDischargeGraphical.WriteToXML();
            frequencyDischargeGraphical.Name = "FrequencyDischargeGraphical";
            XElement regulatedUnregulated = _UnregulatedRegulated.WriteToXML();
            regulatedUnregulated.Name = "UnregulatedRegulated";
            XElement dischargeStage = _DischargeStage.WriteToXML();
            dischargeStage.Name = "DischargeStage";
            XElement frequencyStage = _FrequencyStage.WriteToXML();
            frequencyStage.Name = "FrequencyStage";
            XElement interiorExterior = _ChannelStageFloodplainStage.WriteToXML();
            interiorExterior.Name = "InteriorExterior";
            XElement systemResponse = _SystemResponseFunction.WriteToXML();
            systemResponse.Name = "SystemResponse";
            XElement impactAreaScenarioResults = _ImpactAreaScenarioResults.WriteToXml();
            impactAreaScenarioResults.Name = "ImpactAreaScenarioResults";
            XElement stageDamageList = new("stageDamageList");

            foreach (UncertainPairedData stageDamage in _DamageCategoryStageDamage)
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
            List<UncertainPairedData> stageDamageList = new();
            foreach (XElement stageDamageElement in xElement.Element("stageDamageList").Elements())
            {
                UncertainPairedData stageDamage = UncertainPairedData.ReadFromXML(stageDamageElement);
                stageDamageList.Add(stageDamage);
            }

            bool leveeIsValid = Convert.ToBoolean(xElement.Attribute("LeveeIsValid").Value);
            double topOfLeveeElevation = Convert.ToDouble(xElement.Attribute("TopOfLeveeElevation").Value);
            int impactAreaID = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);

            ImpactAreaScenarioSimulation impactAreaScenarioSimulation = Builder(impactAreaID)
                .WithFlowFrequency(frequencyDischarge)
                .WithFlowFrequency(frequencyDischargeGraphical)
                .WithInflowOutflow(regulatedUnregulated)
                .WithFlowStage(stageDischarge)
                .WithLevee(systemResponse, topOfLeveeElevation)
                .WithStageDamages(stageDamageList)
                .WithFrequencyStage(frequencyStage)
                .WithInteriorExterior(interiorExterior)
                .Build();
            impactAreaScenarioSimulation._LeveeIsValid = leveeIsValid;
            impactAreaScenarioSimulation._ImpactAreaScenarioResults = (ImpactAreaScenarioResults)impactAreaScenarioResults;
            return impactAreaScenarioSimulation;

        }
        public class SimulationBuilder
        {
            private readonly ImpactAreaScenarioSimulation _Simulation;
            internal SimulationBuilder(ImpactAreaScenarioSimulation sim)
            {
                _Simulation = sim;
            }
            public ImpactAreaScenarioSimulation Build()
            {
                //TODO: The validation below is not very helpful. We only see that "XX has errors" but 
                //we are not informing the user what the errors are 
                //somehow we need to add the error messages of the object being validated to the error messages of the impact area scenario simulation 
                _Simulation.Validate();
                //add validation here to test ranges and domains.
                return _Simulation;
            }
            public SimulationBuilder WithFlowFrequency(ContinuousDistribution continuousDistribution)
            {   //TODO: I do not think the sample size validation works
                _Simulation._FrequencyDischarge = continuousDistribution;
                _Simulation.AddSinglePropertyRule("flow frequency", new Rule(() => { _Simulation._FrequencyDischarge.Validate(); return !_Simulation._FrequencyDischarge.HasErrors; }, string.Join(Environment.NewLine, _Simulation._FrequencyDischarge.GetErrors())));
                return new SimulationBuilder(_Simulation);
            }
            public SimulationBuilder WithFlowFrequency(GraphicalUncertainPairedData graphicalUncertainPairedData)
            {
                _Simulation._FrequencyDischargeGraphical = graphicalUncertainPairedData;
                return new SimulationBuilder(_Simulation);
            }
            public SimulationBuilder WithInflowOutflow(UncertainPairedData uncertainPairedData)
            {
                _Simulation._UnregulatedRegulated = uncertainPairedData;
                _Simulation.AddSinglePropertyRule("inflow outflow", new Rule(() => { _Simulation._UnregulatedRegulated.Validate(); return !_Simulation._UnregulatedRegulated.HasErrors; }, $"Inflow-Outflow has errors for the impact area with ID {_Simulation._ImpactAreaID}."));

                return new SimulationBuilder(_Simulation);
            }
            public SimulationBuilder WithFlowStage(UncertainPairedData uncertainPairedData)
            {
                _Simulation._DischargeStage = uncertainPairedData;
                _Simulation.AddSinglePropertyRule("flow stage", new Rule(() => { _Simulation._DischargeStage.Validate(); return !_Simulation._DischargeStage.HasErrors; }, $"Flow-Stage has errors  for the impact area with ID {_Simulation._ImpactAreaID}."));
                double stageMin = uncertainPairedData.Yvals[0].InverseCDF(p: 0.001);
                double stageMax = uncertainPairedData.Yvals[^1].InverseCDF(p: 0.999);
                _Simulation.AddSinglePropertyRule("stage range", new Rule(() => (stageMax - stageMin) < 1000, "The range of stages must be less than 1000. Ranges larger than this will cause memory problems", ErrorLevel.Fatal));
                return new SimulationBuilder(_Simulation);
            }
            public SimulationBuilder WithFrequencyStage(GraphicalUncertainPairedData graphicalUncertainPairedData)
            {
                _Simulation._FrequencyStage = graphicalUncertainPairedData;
                _Simulation.AddSinglePropertyRule("frequency_stage", new Rule(() => { _Simulation._FrequencyStage.Validate(); return !_Simulation._FrequencyStage.HasErrors; }, $"Frequency-Stage has errors  for the impact area with ID {_Simulation._ImpactAreaID}."));
                return new SimulationBuilder(_Simulation);
            }
            public SimulationBuilder WithInteriorExterior(UncertainPairedData uncertainPairedData)
            {
                _Simulation._ChannelStageFloodplainStage = uncertainPairedData;
                _Simulation.AddSinglePropertyRule("channelstage_floodplainstage", new Rule(() =>
                {
                    _Simulation._ChannelStageFloodplainStage.Validate();
                    return !_Simulation._ChannelStageFloodplainStage.HasErrors;
                }
                , $"There are errors in the InteriorExterior relationship for the impact area with ID {_Simulation._ImpactAreaID}."));
                return new SimulationBuilder(_Simulation);
            }
            public SimulationBuilder WithLevee(UncertainPairedData uncertainPairedData, double topOfLeveeElevation)
            {
                _Simulation.AddSinglePropertyRule("levee", new Rule(() => _Simulation.LeveeIsValid(), $"The levee is invalid  for the impact area with ID {_Simulation._ImpactAreaID}."));
                _Simulation._SystemResponseFunction = uncertainPairedData;
                _Simulation._TopOfLeveeElevation = topOfLeveeElevation;
                return new SimulationBuilder(_Simulation);
            }
            public SimulationBuilder WithStageDamages(List<UncertainPairedData> uncertainPairedDataList)
            {
                _Simulation._DamageCategoryStageDamage = uncertainPairedDataList;
                foreach (UncertainPairedData uncertainPairedData in _Simulation._DamageCategoryStageDamage)
                {
                    _Simulation.AddSinglePropertyRule(uncertainPairedData.CurveMetaData.DamageCategory + " stage damages", new Rule(() => { uncertainPairedData.Validate(); return !uncertainPairedData.HasErrors; }, $"Stage-damage errors ror the impact area with ID {_Simulation._ImpactAreaID}: " + uncertainPairedData.GetErrors().ToString()));
                }
                return new SimulationBuilder(_Simulation);
            }
            public SimulationBuilder WithPerformanceMetrics(ImpactAreaScenarioResults results)
            {
                _Simulation._ImpactAreaScenarioResults = results;
                return new SimulationBuilder(_Simulation);
            }
            public SimulationBuilder WithAdditionalThreshold(Threshold threshold)
            {
                _Simulation._ImpactAreaScenarioResults.PerformanceByThresholds.AddThreshold(threshold);
                return new SimulationBuilder(_Simulation);
            }

            public SimulationBuilder ForImpactArea(int impactAreaID)
            {
                _Simulation._ImpactAreaID = impactAreaID;
                return new SimulationBuilder(_Simulation);
            }
        }

    }

}
