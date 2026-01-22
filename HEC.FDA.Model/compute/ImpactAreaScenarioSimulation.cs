using HEC.FDA.Model.extensions;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.utilities;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Model.Messaging;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HEC.FDA.Model.compute
{
    [StoredProperty("ImpactAreaScenarioSimulation")]
    public class ImpactAreaScenarioSimulation : ValidationErrorLogger, IProgressReport
    {
        #region Fields 
        private const int FREQUENCY_SEED = 1234;
        private const int FLOW_REGULATION_SEED = 2345;
        private const int STAGE_FLOW_SEED = 3456;
        private const int EXTERIOR_INTERIOR_SEED = 4567;
        private const int SYSTEM_RESPONSE_SEED = 5678;
        private const int STAGE_DAMAGE_SEED = 6789;
        private const int STAGE_LIFELOSS_SEED = 7891;
        public const int IMPACT_AREA_SIM_COMPLETED = -1001;
        private const double THRESHOLD_DAMAGE_PERCENT = 0.05;
        private const double THRESHOLD_DAMAGE_RECURRENCE_INTERVAL = 0.99; //this is a non-exceedance probability 
        public const int DEFAULT_THRESHOLD_ID = 0;
        private ContinuousDistribution _FrequencyDischarge;
        private GraphicalUncertainPairedData _FrequencyDischargeGraphical;
        private UncertainPairedData _UnregulatedRegulated;
        private UncertainPairedData _DischargeStage;
        private GraphicalUncertainPairedData _FrequencyStage;
        private UncertainPairedData _ChannelStageFloodplainStage;
        private UncertainPairedData _SystemResponseFunction;
        private double _TopOfLeveeElevation;
        private bool _HasFailureStageDamage;
        private bool _HasFailureStageLifeLoss;
        private bool _HasNonFailureStageLifeLoss;
        private List<UncertainPairedData> _FailureStageDamageFunctions;
        private List<UncertainPairedData> _FailureStageLifeLossFunctions;
        private List<UncertainPairedData> _NonFailureStageLifeLossFunctions;
        private int _ImpactAreaID;
        private ImpactAreaScenarioResults _ImpactAreaScenarioResults;
        private bool _HasNonFailureStageDamage;
        #endregion

        #region Properties 
        public List<UncertainPairedData> _NonFailureStageDamageFunctions { get; private set; }
        public event ProgressReportedEventHandler ProgressReport;
        public bool NonFailRiskIncluded { get; private set; } = false;
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
        #endregion 

        internal ImpactAreaScenarioSimulation(int impactAreaID)
        {
            _FrequencyDischarge = null;
            _FrequencyDischargeGraphical = new GraphicalUncertainPairedData(); //can we have both of these?
            _UnregulatedRegulated = new UncertainPairedData();//defaults to null
            _DischargeStage = new UncertainPairedData(); //defaults to null
            _FrequencyStage = new GraphicalUncertainPairedData();//defaults to null
            _ChannelStageFloodplainStage = new UncertainPairedData();//defaults to null
            _SystemResponseFunction = new UncertainPairedData(); //defaults to null
            _FailureStageDamageFunctions = new List<UncertainPairedData>();//defaults to empty
            _FailureStageLifeLossFunctions = new List<UncertainPairedData>(); // defaults to empty
            _NonFailureStageLifeLossFunctions = new List<UncertainPairedData>(); // defaults to empty
            _NonFailureStageDamageFunctions = new List<UncertainPairedData>(); //empty 
            _ImpactAreaID = impactAreaID;
            _ImpactAreaScenarioResults = new ImpactAreaScenarioResults(_ImpactAreaID); //defaults to null
        }

        /// <summary>
        /// This code path currently only used by tests. 
        /// </summary>
        public ImpactAreaScenarioResults Compute(ConvergenceCriteria convergenceCriteria, bool computeIsDeterministic = false)
        {
            return Compute(convergenceCriteria, new CancellationToken(), computeIsDeterministic);
        }

        /// <summary>
        /// A simulation must be built with a stage damage function for compute default threshold to be true.
        /// Random numbers are generated for a full compute with uncertainty, during which the correct random number is pulled for the current iteration 
        /// All sampling methods include a computeIsDeterministic argument that bypasses the iteration number for the retrieval of the deterministic representation of the variable 
        /// </summary>
        public ImpactAreaScenarioResults Compute(ConvergenceCriteria convergenceCriteria, CancellationToken cancellationToken, bool computeIsDeterministic = false)
        {
            //Validate the configuration is runnable through the CanCompute method
            if (!CanCompute(convergenceCriteria))
            {
                _ImpactAreaScenarioResults = new ImpactAreaScenarioResults(_ImpactAreaID, true); //I would like to just return regular Null here but I'm unsure who is relying on this behavior. BBB
                return _ImpactAreaScenarioResults;
            }

            //set up results histograms.
            InitializeConsequenceHistograms(convergenceCriteria, out List<(CurveMetaData, PairedData)> damageFrequencyFunctions);
            SetupPerformanceThresholds(convergenceCriteria, damageFrequencyFunctions);


            //The actual compute here ************
            MessageEventArgs beginComputeMessageArgs = new(new Message($"EAD and performance compute for the impact area with ID {_ImpactAreaID} has been initiated" + Environment.NewLine));
            ReportMessage(this, beginComputeMessageArgs);
            //Prepare montecarlo sampling by generating random numbers for each object that will be sampled during the compute
            PopulateRandomNumbers(convergenceCriteria);
            ComputeIterations(convergenceCriteria, computeIsDeterministic, cancellationToken);
            _ImpactAreaScenarioResults.ParallelResultsAreConverged(.95, .05);
            MessageEventArgs endComputeMessageArgs = new(new Message($"EAD and performance compute for the impact area with ID {_ImpactAreaID} has completed successfully" + Environment.NewLine));
            ReportMessage(this, endComputeMessageArgs);

            return _ImpactAreaScenarioResults;
        }

        private void SetupPerformanceThresholds(ConvergenceCriteria convergenceCriteria, List<(CurveMetaData, PairedData)> damageFrequencyFunctions)
        {
            bool defaultThresholdExists = _ImpactAreaScenarioResults.PerformanceByThresholds.ListOfThresholds.Select(x => x.ThresholdID).Contains(0); // 0 is the threshold ID for the default threshold

            //always add levees as thresholds. 
            if (_SystemResponseFunction.Xvals != null)
            {
                Threshold systemResponseThreshold = DetermineSystemResponseThreshold(convergenceCriteria);
                _ImpactAreaScenarioResults.PerformanceByThresholds.AddThreshold(systemResponseThreshold);
            }

            //if the user set the default threshold themself, just use that. Otherwise calculate it... 
            else if (!defaultThresholdExists && damageFrequencyFunctions.Count > 0)
            {
                Threshold defaultThreshold = ComputeDefaultThreshold(convergenceCriteria, damageFrequencyFunctions);
                _ImpactAreaScenarioResults.PerformanceByThresholds.AddThreshold(defaultThreshold);
            }

            CreateHistogramsForAssuranceOfThresholds();
        }

        /// <summary>
        /// </summary>
        /// <param name="damageFrequencyFunctions">Usefull outside this method to set the default threshold for an economic damage compute. </param>
        private void InitializeConsequenceHistograms(ConvergenceCriteria convergenceCriteria, out List<(CurveMetaData, PairedData)> damageFrequencyFunctions)
        {
            if (_HasFailureStageDamage)
            {
                damageFrequencyFunctions = ComputeConsequenceFrequency(_FailureStageDamageFunctions);
                //these are here only for the Preview Compute. 
                _ImpactAreaScenarioResults.DamageFrequencyFunctions = damageFrequencyFunctions;
                CreateEAConsequenceHistograms(convergenceCriteria, damageFrequencyFunctions, _FailureStageDamageFunctions, ConsequenceType.Damage, RiskType.Fail);
            }
            else damageFrequencyFunctions = null;

            if (_HasNonFailureStageDamage)
            {
                var nonFailureDamageFrequencyFunctions = ComputeConsequenceFrequency(_NonFailureStageDamageFunctions);
                CreateEAConsequenceHistograms(convergenceCriteria, nonFailureDamageFrequencyFunctions, _NonFailureStageDamageFunctions, ConsequenceType.Damage, RiskType.Non_Fail);
            }

            if (_HasNonFailureStageLifeLoss)
            {
                var nonFailureLifeLossFrequencyFunctions = ComputeConsequenceFrequency(_NonFailureStageLifeLossFunctions);
                CreateEAConsequenceHistograms(convergenceCriteria, nonFailureLifeLossFrequencyFunctions, _NonFailureStageLifeLossFunctions, ConsequenceType.LifeLoss, RiskType.Non_Fail);
            }

            if (_HasFailureStageLifeLoss)
            {
                var lifeLossFrequencyFunctions = ComputeConsequenceFrequency(_FailureStageLifeLossFunctions);
                CreateEAConsequenceHistograms(convergenceCriteria, lifeLossFrequencyFunctions, _FailureStageLifeLossFunctions, ConsequenceType.LifeLoss, RiskType.Fail);
            }
        }

        private Threshold DetermineSystemResponseThreshold(ConvergenceCriteria convergenceCriteria)
        {
            ThresholdEnum thresholdEnum;
            if (_SystemResponseFunction.Xvals.Length <= 2)
            {
                thresholdEnum = ThresholdEnum.TopOfLevee;
            }
            else
            {
                thresholdEnum = ThresholdEnum.LeveeSystemResponse;
            }
            return new Threshold(thresholdID: 0, _SystemResponseFunction, convergenceCriteria, thresholdEnum, _TopOfLeveeElevation);
        }

        //This method tells each object that will be sampled in the full compute to generate random numbers for sampling 
        private void PopulateRandomNumbers(ConvergenceCriteria convergenceCriteria)
        {
            //generate slightly more random numbers than max iterations because it is possible that we keep iterating beyond max 
            //before re-checking for convergence 
            int quantityOfRandomNumbers = Convert.ToInt32(convergenceCriteria.MaxIterations * 1.25);

            if (_FrequencyDischarge != null)
            {
                _FrequencyDischarge.GenerateRandomSamplesofNumbers(FREQUENCY_SEED, quantityOfRandomNumbers);
            }
            if (!_FrequencyDischargeGraphical.CurveMetaData.IsNull)
            {
                _FrequencyDischargeGraphical.GenerateRandomNumbers(FREQUENCY_SEED, quantityOfRandomNumbers);
            }
            if (!_UnregulatedRegulated.CurveMetaData.IsNull)
            {
                _UnregulatedRegulated.GenerateRandomNumbers(FLOW_REGULATION_SEED, quantityOfRandomNumbers);
            }
            if (!_DischargeStage.CurveMetaData.IsNull)
            {
                _DischargeStage.GenerateRandomNumbers(STAGE_FLOW_SEED, quantityOfRandomNumbers);
            }
            if (!_FrequencyStage.CurveMetaData.IsNull)
            {
                _FrequencyStage.GenerateRandomNumbers(FREQUENCY_SEED, quantityOfRandomNumbers);
            }
            if (!_ChannelStageFloodplainStage.CurveMetaData.IsNull)
            {
                _ChannelStageFloodplainStage.GenerateRandomNumbers(EXTERIOR_INTERIOR_SEED, quantityOfRandomNumbers);
            }
            if (!_SystemResponseFunction.CurveMetaData.IsNull)
            {
                _SystemResponseFunction.GenerateRandomNumbers(SYSTEM_RESPONSE_SEED, quantityOfRandomNumbers);
            }
            foreach (UncertainPairedData stageDamage in _FailureStageDamageFunctions)
            {
                stageDamage.GenerateRandomNumbers(STAGE_DAMAGE_SEED, quantityOfRandomNumbers);
            }
            foreach (UncertainPairedData stageDamage in _NonFailureStageDamageFunctions)
            {
                stageDamage.GenerateRandomNumbers(STAGE_DAMAGE_SEED, quantityOfRandomNumbers);
            }
            foreach (UncertainPairedData stageLifeLoss in _FailureStageLifeLossFunctions)
            {
                stageLifeLoss.GenerateRandomNumbers(STAGE_LIFELOSS_SEED, quantityOfRandomNumbers);
            }
            foreach (UncertainPairedData stageLifeLoss in _NonFailureStageLifeLossFunctions)
            {
                stageLifeLoss.GenerateRandomNumbers(STAGE_LIFELOSS_SEED, quantityOfRandomNumbers);
            }
        }

        private void CreateEAConsequenceHistograms(ConvergenceCriteria convergenceCriteria, List<(CurveMetaData, PairedData)> consequenceFrequencyFunctions, List<UncertainPairedData> stageConsequenceFunctions, ConsequenceType consequenceType, RiskType riskType)
        {
            //run the preview compute
            //I need to do this off of the damage frequency function instead
            foreach (UncertainPairedData stageConsequence in stageConsequenceFunctions)
            {
                foreach ((CurveMetaData, PairedData) metaData in consequenceFrequencyFunctions)
                {
                    if (stageConsequence.CurveMetaData.Equals(metaData.Item1))
                    {
                        _ImpactAreaScenarioResults.ConsequenceResults.AddNewConsequenceResultObject(metaData.Item1.DamageCategory, metaData.Item1.AssetCategory, convergenceCriteria, _ImpactAreaID, consequenceType, riskType);
                    }
                }
            }
        }

        private void LogSimulationPropertyRuleErrors()
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

            foreach (UncertainPairedData relationship in _FailureStageDamageFunctions)
            {
                validationObjects.Add(relationship);
                validationIntroMessages.Add(nameof(_FailureStageDamageFunctions) + ": " + relationship.CurveMetaData.DamageCategory + ": " + relationship.CurveMetaData.AssetCategory + ": " + "has the following messages");
            }
        }

        private bool CanCompute(ConvergenceCriteria convergenceCriteria)
        {
            bool canCompute = true;
            if (ErrorLevel >= ErrorLevel.Fatal)
            {
                ReportMessage(this, new MessageEventArgs(new Message($"The simulation for impact area {_ImpactAreaID} contains errors. The compute has been aborted." + Environment.NewLine)));
                canCompute = false;
            }
            LogSimulationPropertyRuleErrors();
            convergenceCriteria.Validate();
            ValidateNonFail();
            if (convergenceCriteria.HasErrors)
            {
                canCompute = false;
                string message = $"The convergence criteria established in study properties are not valid: {convergenceCriteria.GetErrorMessages()}";
                ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage));
            }
            if (_FailureStageDamageFunctions.Count == 0 && _FailureStageLifeLossFunctions.Count == 0)
            {
                if (!HasLevee)
                {
                    canCompute = false;
                }
            }
            return canCompute;

        }

        private void ValidateNonFail()
        {
            if (_HasNonFailureStageDamage || _HasNonFailureStageLifeLoss)
            {
                if (HasLevee.Equals(false))
                {
                    string errorMessage = $"T he simulation for impact area with ID {ImpactAreaID} was configured to calculate nonfail risk but a levee was not specified, therefore nonfail risk will not be calculated.";
                    ErrorMessage leveeMissing = new(errorMessage, ErrorLevel.Major);
                    ReportMessage(this, new MessageEventArgs(leveeMissing));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="convergenceCriteria"></param>
        /// <param name="computeWithDamage"></param>
        /// <param name="computeWithLifeLoss"></param>
        /// <param name="computeIsDeterministic"> Do not do monte-carlo sampling</param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="TaskCanceledException"></exception>
        private void ComputeIterations(ConvergenceCriteria convergenceCriteria, bool computeIsDeterministic, CancellationToken cancellationToken)
        {
            long completedIterations = 0;
            long expectedIterations = convergenceCriteria.MinIterations; //only used in % complete
            int iterationsPerComputeChunk = convergenceCriteria.IterationCount;
            int computeChunkQuantity = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(convergenceCriteria.MinIterations) / iterationsPerComputeChunk));
            if (computeChunkQuantity < 1)
            {
                computeChunkQuantity = 1;
            }
            bool computeIsNotConverged = true;
            while (computeIsNotConverged)
            {
                for (int i = 0; i < computeChunkQuantity; i++)
                {
                    try
                    {
                        long iterationsStart = i * iterationsPerComputeChunk;
                        Parallel.For(0, iterationsPerComputeChunk, chunkIteration =>
                        {
                            long computeIteration = iterationsStart + chunkIteration;

                            PairedData frequency_stage_sample = GetFrequencyStageSample(computeIsDeterministic, computeIteration);
                            ComputeFromStageFrequency(frequency_stage_sample, computeIteration, chunkIteration, computeIsDeterministic);

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
                if (!_ImpactAreaScenarioResults.ResultsAreConverged(.95, .05, _HasFailureStageDamage))
                {
                    //recalculate compute chunks 
                    expectedIterations = _ImpactAreaScenarioResults.RemainingIterations(.95, .05, _HasFailureStageDamage);
                    computeChunkQuantity = Convert.ToInt32(expectedIterations / convergenceCriteria.IterationCount);
                    if (computeChunkQuantity == 0)
                    {
                        computeChunkQuantity = 1;
                    }
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

        private PairedData GetFrequencyStageSample(bool computeIsDeterministic, long thisComputeIteration)
        {
            PairedData frequency_stage_sample;
            if (_FrequencyStage.CurveMetaData.IsNull)
            {
                Debug.Assert(!_DischargeStage.CurveMetaData.IsNull);
                PairedData frequencyDischarge;

                if (_FrequencyDischargeGraphical.CurveMetaData.IsNull)
                {
                    frequencyDischarge = _FrequencyDischarge.BootstrapToPairedData(thisComputeIteration, utilities.DoubleGlobalStatics.RequiredExceedanceProbabilities, computeIsDeterministic);
                }
                else
                {
                    frequencyDischarge = _FrequencyDischargeGraphical.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                }
                frequency_stage_sample = GetStageFreq(computeIsDeterministic, thisComputeIteration, frequencyDischarge);
            }
            else
            {
                frequency_stage_sample = _FrequencyStage.SamplePairedData(thisComputeIteration, computeIsDeterministic);
            }

            return frequency_stage_sample;
        }

        private PairedData GetStageFreq(bool computeIsDeterministic, long thisComputeIteration, PairedData frequencyDischarge)
        {
            PairedData frequency_stage;
            if (_UnregulatedRegulated.CurveMetaData.IsNull)
            {
                PairedData discharge_stage_sample = _DischargeStage.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                frequency_stage = discharge_stage_sample.compose(frequencyDischarge);
            }
            else
            {
                PairedData inflow_outflow_sample = _UnregulatedRegulated.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                PairedData transformff = inflow_outflow_sample.compose(frequencyDischarge);
                PairedData discharge_stage_sample = _DischargeStage.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                frequency_stage = discharge_stage_sample.compose(transformff);
            }
            return frequency_stage;
        }

        /// <summary>
        /// </summary>
        /// <param name="frequency_stage"></param>
        /// <param name="thisComputeIteration"> used for pulling the correct random number in sampling</param>
        /// <param name="thisChunkIteration"> used for saving a result in the correct place of a temp results array </param>
        /// <param name="computeIsDeterministic"></param>
        private void ComputeFromStageFrequency(PairedData frequency_stage, long thisComputeIteration, long thisChunkIteration, bool computeIsDeterministic)
        {
            if (_SystemResponseFunction.CurveMetaData.IsNull)
            {
                if (_HasFailureStageDamage)
                {
                    ComputeConsequencesFromStageFrequency(frequency_stage, thisComputeIteration, thisChunkIteration, computeIsDeterministic, _FailureStageDamageFunctions, ConsequenceType.Damage);
                }
                if (_HasFailureStageLifeLoss)
                {
                    ComputeConsequencesFromStageFrequency(frequency_stage, thisComputeIteration, thisChunkIteration, computeIsDeterministic, _FailureStageLifeLossFunctions, ConsequenceType.LifeLoss);

                }
                ComputePerformance(frequency_stage, Convert.ToInt32(thisChunkIteration));
            }
            else
            {
                PairedData systemResponse_sample = _SystemResponseFunction.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                if (_HasFailureStageDamage)
                {
                    ComputeDamagesFromStageFrequency_WithLevee(frequency_stage, systemResponse_sample, thisComputeIteration, thisChunkIteration, computeIsDeterministic, ConsequenceType.Damage);
                }
                if (_HasFailureStageLifeLoss)
                {
                    ComputeDamagesFromStageFrequency_WithLevee(frequency_stage, systemResponse_sample, thisComputeIteration, thisChunkIteration, computeIsDeterministic, ConsequenceType.LifeLoss);
                }

                //If the system response function is the default function 
                if (systemResponse_sample.Xvals.Count <= 2)
                {
                    ComputePerformance(frequency_stage, Convert.ToInt32(thisChunkIteration));
                }
                else
                {
                    ComputeLeveePerformance(frequency_stage, systemResponse_sample, Convert.ToInt32(thisChunkIteration));
                }
            }
        }

        private void ComputeConsequencesFromStageFrequency(PairedData frequency_stage, long thisComputeIteration, long thisChunkIteration, bool computeIsDeterministic, List<UncertainPairedData> consequenceFunctions, ConsequenceType consequenceType)
        {
            foreach (UncertainPairedData stageUncertainConsequences in consequenceFunctions)
            {
                //short circuit if the damage function is all zeros
                if (stageUncertainConsequences.Yvals[^1].InverseCDF(1) == 0)
                {
                    _ImpactAreaScenarioResults.ConsequenceResults.AddConsequenceRealization(0, stageUncertainConsequences.CurveMetaData.DamageCategory, stageUncertainConsequences.CurveMetaData.AssetCategory, _ImpactAreaID, thisChunkIteration, consequenceType);
                    continue;
                }
                PairedData _stage_consequences_sample = stageUncertainConsequences.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                PairedData frequency_consequences = _stage_consequences_sample.compose(frequency_stage);
                double eaConsequencesEstimate = frequency_consequences.integrate();
                _ImpactAreaScenarioResults.ConsequenceResults.AddConsequenceRealization(eaConsequencesEstimate, stageUncertainConsequences.CurveMetaData.DamageCategory, stageUncertainConsequences.CurveMetaData.AssetCategory, _ImpactAreaID, thisChunkIteration, consequenceType);
            }
        }

        private void ComputeDamagesFromStageFrequency_WithLevee(PairedData frequency_stage, PairedData systemResponse, long thisComputeIteration, long thisChunkIteration, bool computeIsDeterministic, ConsequenceType type)
        {
            List<UncertainPairedData> failureStageDamageFunctions;
            List<UncertainPairedData> nonfailureStageDamageFunctions;

            if (type == ConsequenceType.LifeLoss)
            {
                failureStageDamageFunctions = _FailureStageLifeLossFunctions;
                nonfailureStageDamageFunctions = _NonFailureStageLifeLossFunctions;
            }
            // ConsequenceType can only be LifeLoss or Damage
            else
            {
                failureStageDamageFunctions = _FailureStageDamageFunctions;
                nonfailureStageDamageFunctions = _NonFailureStageDamageFunctions;
            }

            PairedData validatedSystemResponse = EnsureBottomAndTopHaveCorrectProbabilities(systemResponse);
            foreach (UncertainPairedData stageUncertainDamage in failureStageDamageFunctions)
            {
                PairedData stageDamageFailSample = stageUncertainDamage.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                PairedData stageDamageFailAdjusted = stageDamageFailSample.multiply(validatedSystemResponse);
                PairedData stageFreqFail = stageDamageFailAdjusted.compose(frequency_stage); //Save me for FN Plot
                double failEad = stageFreqFail.integrate();
                _ImpactAreaScenarioResults.ConsequenceResults.AddConsequenceRealization(failEad, stageUncertainDamage.CurveMetaData.DamageCategory, stageUncertainDamage.CurveMetaData.AssetCategory, _ImpactAreaID, thisChunkIteration, type, RiskType.Fail);
            }

            //if we have nonfail damage functions, compute those too.
            if (nonfailureStageDamageFunctions.Count > 0)
            {
                PairedData complementSystemResponse = CalculateFailureProbComplement(validatedSystemResponse);
                foreach (UncertainPairedData stageUncertainNonFailureDamage in nonfailureStageDamageFunctions)
                {
                    PairedData stageDamNonFail = stageUncertainNonFailureDamage.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                    PairedData stageDamNonFailAdjusted = stageDamNonFail.multiply(complementSystemResponse);
                    PairedData stageFreqNonFail = stageDamNonFailAdjusted.compose(frequency_stage); //Save me for FN Plot
                    double nonFailEad = stageFreqNonFail.integrate();
                    _ImpactAreaScenarioResults.ConsequenceResults.AddConsequenceRealization(nonFailEad, stageUncertainNonFailureDamage.CurveMetaData.DamageCategory, stageUncertainNonFailureDamage.CurveMetaData.AssetCategory, _ImpactAreaID, thisChunkIteration, type, RiskType.Non_Fail);

                }
            }

        }

        private static PairedData CalculateFailureProbComplement(PairedData validatedSystemResponse)
        {
            double[] probabilityOfNonFailure = new double[validatedSystemResponse.Yvals.Count];
            for (int i = 0; i < probabilityOfNonFailure.Length; i++)
            {
                probabilityOfNonFailure[i] = 1 - (validatedSystemResponse.Yvals[i]);
            }
            PairedData complementOfSystemResponse = new(validatedSystemResponse.Xvals.ToArray(), probabilityOfNonFailure);
            return complementOfSystemResponse;
        }
        //TODO: Opportunity for refactor: move performance functions to system performance statistics
        public void ComputePerformance(PairedData frequency_stage, int thisChunkIteration)
        {
            foreach (var thresholdEntry in _ImpactAreaScenarioResults.PerformanceByThresholds.ListOfThresholds)
            {
                double thresholdValue = thresholdEntry.ThresholdValue;
                double aep = 1 - frequency_stage.f_inverse(thresholdValue);
                thresholdEntry.SystemPerformanceResults.AddAEPForAssurance(aep, thisChunkIteration);
                GetStageForNonExceedanceProbability(frequency_stage, thresholdEntry, thisChunkIteration);
            }
        }
        //this method assumes that the levee fragility function spans the entire probability domain 
        public void ComputeLeveePerformance(PairedData frequency_stage, PairedData levee_curve_sample, int thisChunkIteration)
        {
            PairedData levee_frequency_stage = levee_curve_sample.compose(frequency_stage);
            double aep = 0;
            //extrapolate below
            if (levee_frequency_stage.Xvals[0] != 0)
            {
                double initialProbOfStageInRange = levee_frequency_stage.Xvals[0] - 0;
                double initialProbFailure = (levee_frequency_stage.Yvals[0] + 0) / 2;
                aep += initialProbOfStageInRange * initialProbFailure;
            }
            //within function range
            for (int i = 1; i < levee_frequency_stage.Xvals.Count; i++)
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
                thresholdEntry.SystemPerformanceResults.AddAEPForAssurance(aep, thisChunkIteration);
                GetStageForNonExceedanceProbability(frequency_stage, thresholdEntry, thisChunkIteration);
            }

        }
        public static void GetStageForNonExceedanceProbability(PairedData frequency_stage, Threshold threshold, int thisChunkIteration)
        {//TODO: Get rid of these hard coded doubles 
            double[] er101RequiredNonExceedanceProbabilities = new double[] { .9, .96, .98, .99, .996, .998 };
            foreach (double nonExceedanceProbability in er101RequiredNonExceedanceProbabilities)
            {
                double stageOfEvent = frequency_stage.f(nonExceedanceProbability);
                threshold.SystemPerformanceResults.AddStageForAssurance(nonExceedanceProbability, stageOfEvent, thisChunkIteration);
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
            long iteration = 1;
            foreach (UncertainPairedData uncertainPairedData in listOfUncertainPairedData)
            {
                PairedData stageDamageSample = uncertainPairedData.SamplePairedData(iteration, retrieveDeterministicRepresentation: true);
                totalStageDamage = totalStageDamage.SumYsForGivenX(stageDamageSample);
            }
            return totalStageDamage;
        }
        private Threshold ComputeDefaultThreshold(ConvergenceCriteria convergenceCriteria, List<(CurveMetaData, PairedData)> damageFrequencyFunctions)
        {
            if (!_SystemResponseFunction.IsNull)
            {
                throw new Exception("A default threshold cannot be calculated for an impact area with a levee.");
            }
            PairedData totalStageDamage = ComputeTotalStageDamage(_FailureStageDamageFunctions);
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


        /// <summary>
        /// Computes deterministic consequence-frequency curves for each provided stage-consequence function.
        /// </summary>
        /// <remarks>If required supporting functions are missing, the method returns a default
        /// consequence-frequency curve with zero values and reports a fatal error. The computation is performed
        /// deterministically, and the output list corresponds one-to-one with the input functions.</remarks>
        /// <param name="stageConsequenceFunctions">A list of uncertain paired data objects representing stage-consequence functions to be evaluated. Cannot be
        /// null; may be empty to indicate no available functions.</param>
        /// <returns>A list of tuples, each containing curve metadata and a paired data object representing the computed
        /// consequence-frequency relationship for each input function. If no valid functions are provided, the list
        /// contains a single default entry with zero values.</returns>
        private List<(CurveMetaData, PairedData)> ComputeConsequenceFrequency(List<UncertainPairedData> stageConsequenceFunctions)
        {
            long fakeIterationNumberNotUsedInThisPartOfTheComputeBecauseItIsDeterministic = 1;
            bool computeIsDeterministic = true;
            double[] xs = new double[] { 0 };
            double[] ys = new double[] { 0 };
            PairedData frequencyStage;

            List<(CurveMetaData, PairedData)> consequenceFrequency = new();

            if (stageConsequenceFunctions.Count == 0)
            {
                string message = $"A valid consequence frequency cannot be calculated for the impact area with ID {_ImpactAreaID} because no consequence-damage functions were found. A meaningless default threshold of 0 will be used. Please have an additional threshold for meaningful performance statistics" + Environment.NewLine;
                ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage));
                PairedData fakePairedData = new(xs, ys);
                consequenceFrequency.Add((new CurveMetaData(), fakePairedData));
                return consequenceFrequency;
            }
            else
            {
                foreach (UncertainPairedData function in stageConsequenceFunctions)
                {
                    if (_FrequencyStage.CurveMetaData.IsNull)
                    {
                        PairedData frequencyFlow;
                        if (_FrequencyDischargeGraphical.CurveMetaData.IsNull)
                        {
                            frequencyFlow = _FrequencyDischarge.BootstrapToPairedData(fakeIterationNumberNotUsedInThisPartOfTheComputeBecauseItIsDeterministic, utilities.DoubleGlobalStatics.RequiredExceedanceProbabilities, computeIsDeterministic);
                        }
                        else
                        {
                            frequencyFlow = _FrequencyDischargeGraphical.SamplePairedData(fakeIterationNumberNotUsedInThisPartOfTheComputeBecauseItIsDeterministic, computeIsDeterministic);
                        }
                        if (_UnregulatedRegulated.CurveMetaData.IsNull)
                        {
                            if (_DischargeStage.CurveMetaData.IsNull)
                            {
                                string message = $"A stage-discharge function must accompany a discharge-frequency function but was not found for the impact area with ID {_ImpactAreaID}. An arbitrary threshold is being used." + Environment.NewLine;
                                ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                                ReportMessage(this, new MessageEventArgs(errorMessage));
                                PairedData fakePairedData = new(xs, ys);
                                consequenceFrequency.Add((new CurveMetaData(), fakePairedData));
                                return consequenceFrequency;

                            }
                            else
                            {
                                PairedData flowStageSample = _DischargeStage.SamplePairedData(fakeIterationNumberNotUsedInThisPartOfTheComputeBecauseItIsDeterministic, computeIsDeterministic);
                                frequencyStage = flowStageSample.compose(frequencyFlow);
                            }
                        }
                        else
                        {
                            PairedData inflowOutflowSample = _UnregulatedRegulated.SamplePairedData(fakeIterationNumberNotUsedInThisPartOfTheComputeBecauseItIsDeterministic, computeIsDeterministic);
                            PairedData transformFlowFrequency = inflowOutflowSample.compose(frequencyFlow);
                            if (_DischargeStage.CurveMetaData.IsNull)
                            {
                                string message = $"A stage-discharge function must accompany a discharge-frequency function but was not found for the impact area with ID {_ImpactAreaID}. An arbitrary threshold is being used." + Environment.NewLine;
                                ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                                ReportMessage(this, new MessageEventArgs(errorMessage));
                                PairedData fakePairedData = new(xs, ys);
                                consequenceFrequency.Add((new CurveMetaData(), fakePairedData));
                                return consequenceFrequency;
                            }
                            else
                            {
                                PairedData flowStageSample = _DischargeStage.SamplePairedData(fakeIterationNumberNotUsedInThisPartOfTheComputeBecauseItIsDeterministic, computeIsDeterministic);
                                frequencyStage = flowStageSample.compose(transformFlowFrequency);
                            }
                        }

                    }
                    else
                    {
                        frequencyStage = _FrequencyStage.SamplePairedData(fakeIterationNumberNotUsedInThisPartOfTheComputeBecauseItIsDeterministic, computeIsDeterministic);
                    }

                    PairedData stageConsequence = function.SamplePairedData(fakeIterationNumberNotUsedInThisPartOfTheComputeBecauseItIsDeterministic, computeIsDeterministic);
                    if (_ChannelStageFloodplainStage.IsNull)
                    {
                        PairedData consequenceFreq = (PairedData)stageConsequence.compose(frequencyStage);
                        consequenceFrequency.Add((function.CurveMetaData, consequenceFreq));
                    }
                    else
                    {
                        PairedData exteriorInterior = _ChannelStageFloodplainStage.SamplePairedData(fakeIterationNumberNotUsedInThisPartOfTheComputeBecauseItIsDeterministic, computeIsDeterministic);
                        PairedData frequencyInteriorStage = exteriorInterior.compose(frequencyStage);
                        consequenceFrequency.Add((function.CurveMetaData, (PairedData)stageConsequence.compose(frequencyInteriorStage)));
                    }

                }
                return consequenceFrequency;
            }

        }

        public ImpactAreaScenarioResults PreviewCompute()
        {
            ConvergenceCriteria convergenceCriteria = new(minIterations: 1, maxIterations: 1);
            ImpactAreaScenarioResults results = Compute(convergenceCriteria, new CancellationTokenSource().Token, computeIsDeterministic: true);
            return results;
        }
        public static SimulationBuilder Builder(int impactAreaID)
        {
            return new SimulationBuilder(new ImpactAreaScenarioSimulation(impactAreaID));
        }
        private static PairedData EnsureBottomAndTopHaveCorrectProbabilities(PairedData systemResponseFunction)
        {

            bool systemResponseIsComplete = (systemResponseFunction.Yvals.Contains(0) && systemResponseFunction.Yvals.Contains(1));
            if (systemResponseIsComplete)
            {
                return systemResponseFunction;
            }
            else
            {
                // make the fragility function begin with 0 prob failure and end with 1 prob failure 
                List<double> tempXvals = new(); //xvals are stages
                List<double> tempYvals = new(); //yvals are prob failure 

                double buffer = .001; //buffer to define point just above and just below the multiplying curve.

                double belowFragilityCurveValue = 0.0;
                double stageToAddBelowFragility = systemResponseFunction.Xvals[0] - buffer;

                tempXvals.Add(stageToAddBelowFragility);
                tempYvals.Add(belowFragilityCurveValue);

                for (int i = 0; i < systemResponseFunction.Xvals.Count; i++)
                {
                    tempXvals.Add(systemResponseFunction.Xvals[i]);
                    tempYvals.Add(systemResponseFunction.Yvals[i]);
                }

                double aboveFragilityCurveValue = 1.0;
                double stageToAddAboveFragility = systemResponseFunction.Xvals[^1] + buffer;

                tempXvals.Add(stageToAddAboveFragility);
                tempYvals.Add(aboveFragilityCurveValue);

                PairedData newSystemREsponse = new(tempXvals.ToArray(), tempYvals.ToArray());
                return newSystemREsponse;
            }
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
            foreach (UncertainPairedData stageDamage in _FailureStageDamageFunctions)
            {
                foreach (UncertainPairedData incomingStageDamage in incomingImpactAreaScenarioSimulation._FailureStageDamageFunctions)
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
                _Simulation._SystemResponseFunction = uncertainPairedData;
                _Simulation._TopOfLeveeElevation = topOfLeveeElevation;
                return new SimulationBuilder(_Simulation);
            }
            public SimulationBuilder WithStageDamages(List<UncertainPairedData> uncertainPairedDataList)
            {
                _Simulation._FailureStageDamageFunctions = uncertainPairedDataList;
                foreach (UncertainPairedData uncertainPairedData in _Simulation._FailureStageDamageFunctions)
                {
                    _Simulation.AddSinglePropertyRule(
                        uncertainPairedData.CurveMetaData.DamageCategory + " stage damages",
                        new Rule(
                            () => { uncertainPairedData.Validate(); return !uncertainPairedData.HasErrors; },
                            $"Stage-damage errors ror the impact area with ID {_Simulation._ImpactAreaID}: " + uncertainPairedData.GetErrorMessages()));
                }
                _Simulation._HasFailureStageDamage = true;
                return new SimulationBuilder(_Simulation);
            }

            public SimulationBuilder WithStageLifeLoss(List<UncertainPairedData> uncertainPairedDataList)
            {
                _Simulation._FailureStageLifeLossFunctions = uncertainPairedDataList;
                foreach (var upd in _Simulation._FailureStageLifeLossFunctions)
                {
                    _Simulation.AddSinglePropertyRule(
                        upd.CurveMetaData.DamageCategory + " stage life loss",
                        new Rule(
                            () => { upd.Validate(); return !upd.HasErrors; },
                            $"Stage life loss errors for the impact area with ID {_Simulation._ImpactAreaID}: " + upd.GetErrorMessages()));
                }
                _Simulation._HasFailureStageLifeLoss = true;
                return new SimulationBuilder(_Simulation);
            }

            public SimulationBuilder WithNonFailureStageLifeLoss(List<UncertainPairedData> uncertainPairedDataList)
            {
                _Simulation._NonFailureStageLifeLossFunctions = uncertainPairedDataList;
                foreach (var upd in _Simulation._NonFailureStageLifeLossFunctions)
                {
                    _Simulation.AddSinglePropertyRule(
                        upd.CurveMetaData.DamageCategory + " stage life loss",
                        new Rule(
                            () => { upd.Validate(); return !upd.HasErrors; },
                            $"Stage life loss errors for the impact area with ID {_Simulation._ImpactAreaID}: " + upd.GetErrorMessages()));
                }
                _Simulation._HasNonFailureStageLifeLoss = true;
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

            public SimulationBuilder WithNonFailureStageDamage(List<UncertainPairedData> stageDamageFunctions)
            {
                _Simulation._NonFailureStageDamageFunctions = stageDamageFunctions;
                _Simulation._HasNonFailureStageDamage = true;
                return new SimulationBuilder(_Simulation);
            }
        }

    }

}
