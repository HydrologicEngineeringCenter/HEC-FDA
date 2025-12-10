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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        private List<UncertainPairedData> _StageDamageFunctions;
        private List<UncertainPairedData> _StageLifeLossFunctions;
        private int _ImpactAreaID;
        private ImpactAreaScenarioResults _ImpactAreaScenarioResults;
        #endregion

        #region Properties 
        [StoredProperty("DamageCategoryStageNonFailureDamage")]
        public List<UncertainPairedData> _NonFailureStageDamageFunctions { get; private set; }
        public event ProgressReportedEventHandler ProgressReport;
        [StoredProperty("NonFailRiskIncluded")]
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
            _StageDamageFunctions = new List<UncertainPairedData>();//defaults to empty
            _StageLifeLossFunctions = new List<UncertainPairedData>(); // defaults to empty
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
            if (!CanCompute(convergenceCriteria))
            {
                _ImpactAreaScenarioResults = new ImpactAreaScenarioResults(_ImpactAreaID, true); //I would like to just return regular Null here but I'm unsure who is relying on this behavior. BBB
                return _ImpactAreaScenarioResults;
            }


            PopulateRandomNumbers(convergenceCriteria);
            bool computeWithDamage = true;
            bool computeWithLifeLoss = true;

            if (_StageDamageFunctions.Count == 0)
            {
                _ImpactAreaScenarioResults.AddZeroConsequencesResult(_ImpactAreaID);
                computeWithDamage = false;
                ThresholdEnum thresholdEnum;
                if (_SystemResponseFunction.Xvals != null)
                {
                    if (_SystemResponseFunction.Xvals.Length <= 2)
                    {
                        thresholdEnum = ThresholdEnum.TopOfLevee;
                    }
                    else
                    {
                        thresholdEnum = ThresholdEnum.LeveeSystemResponse;
                    }
                    Threshold systemResponseThreshold = new(thresholdID: 0, _SystemResponseFunction, convergenceCriteria, thresholdEnum, _TopOfLeveeElevation);
                    _ImpactAreaScenarioResults.PerformanceByThresholds.AddThreshold(systemResponseThreshold);
                }
            }
            else
            {
                List<(CurveMetaData, PairedData)> damageFrequencyFunctions = ComputeConsequenceFrequency(_StageDamageFunctions);
                _ImpactAreaScenarioResults.DamageFrequencyFunctions = damageFrequencyFunctions;
                CreateEAConsequenceHistograms(convergenceCriteria, damageFrequencyFunctions, _StageDamageFunctions);

                //if the default threshold is user provided, we'll just move on. 
                bool computeDefualtThreshold = !_ImpactAreaScenarioResults.PerformanceByThresholds.ListOfThresholds.Select(x => x.ThresholdID).Contains(0); // 0 is the threshold ID for the default threshold
                if (computeDefualtThreshold)
                {//I am not sure if there is a better way to add the default threshold
                    _ImpactAreaScenarioResults.PerformanceByThresholds.AddThreshold(ComputeDefaultThreshold(convergenceCriteria, computeWithDamage, damageFrequencyFunctions));
                }
            }

            if (_StageLifeLossFunctions.Count > 0)
            {
                List<(CurveMetaData, PairedData)> lifeLossFrequencyFunctions = ComputeConsequenceFrequency(_StageLifeLossFunctions);
                _ImpactAreaScenarioResults.LifeLossFrequencyFunctions = lifeLossFrequencyFunctions;
                CreateEAConsequenceHistograms(convergenceCriteria, lifeLossFrequencyFunctions, _StageLifeLossFunctions);
            }
            else
            {
                computeWithLifeLoss = false;
            }

            CreateHistogramsForAssuranceOfThresholds();
            MessageEventArgs beginComputeMessageArgs = new(new Message($"EAD and performance compute for the impact area with ID {_ImpactAreaID} has been initiated" + Environment.NewLine));
            ReportMessage(this, beginComputeMessageArgs);
            ComputeIterations(convergenceCriteria, computeWithDamage, computeWithLifeLoss, computeIsDeterministic, cancellationToken);
            _ImpactAreaScenarioResults.ParallelResultsAreConverged(.95, .05);
            MessageEventArgs endComputeMessageArgs = new(new Message($"EAD and performance compute for the impact area with ID {_ImpactAreaID} has completed successfully" + Environment.NewLine));
            ReportMessage(this, endComputeMessageArgs);
            return _ImpactAreaScenarioResults;
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
            foreach (UncertainPairedData stageDamage in _StageDamageFunctions)
            {
                stageDamage.GenerateRandomNumbers(STAGE_DAMAGE_SEED, quantityOfRandomNumbers);
            }
            foreach (UncertainPairedData stageDamage in _NonFailureStageDamageFunctions)
            {
                stageDamage.GenerateRandomNumbers(STAGE_DAMAGE_SEED, quantityOfRandomNumbers);
            }
            foreach (UncertainPairedData stageLifeLoss in _StageLifeLossFunctions)
            {
                stageLifeLoss.GenerateRandomNumbers(STAGE_LIFELOSS_SEED, quantityOfRandomNumbers);
            }
        }

        private void CreateEAConsequenceHistograms(ConvergenceCriteria convergenceCriteria, List<(CurveMetaData, PairedData)> consequenceFrequencyFunctions, List<UncertainPairedData> stageConsequenceFunctions)
        {
            //run the preview compute
            //I need to do this off of the damage frequency function instead
            foreach (UncertainPairedData stageConsequence in stageConsequenceFunctions)
            {
                foreach ((CurveMetaData, PairedData) metaData in consequenceFrequencyFunctions)
                {
                    if (stageConsequence.CurveMetaData.Equals(metaData.Item1))
                    {
                        _ImpactAreaScenarioResults.ConsequenceResults.AddNewConsequenceResultObject(metaData.Item1.DamageCategory, metaData.Item1.AssetCategory, convergenceCriteria, _ImpactAreaID);
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

            foreach (UncertainPairedData relationship in _StageDamageFunctions)
            {
                validationObjects.Add(relationship);
                validationIntroMessages.Add(nameof(_StageDamageFunctions) + ": " + relationship.CurveMetaData.DamageCategory + ": " + relationship.CurveMetaData.AssetCategory + ": " + "has the following messages");
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
            if (_StageDamageFunctions.Count == 0 && _StageLifeLossFunctions.Count == 0)
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
            if (NonFailRiskIncluded)
            {
                if (HasLevee.Equals(false))
                {
                    string errorMessage = $"T he simulation for impact area with ID {ImpactAreaID} was configured to calculate nonfail risk but a levee was not specified, therefore nonfail risk will not be calculated.";
                    ErrorMessage leveeMissing = new(errorMessage, ErrorLevel.Major);
                    ReportMessage(this, new MessageEventArgs(leveeMissing));
                }
            }
        }

        private void ComputeIterations(ConvergenceCriteria convergenceCriteria, bool computeWithDamage, bool computeWithLifeLoss, bool computeIsDeterministic, CancellationToken cancellationToken)
        {
            long completedIterations = 0;
            long expectedIterations = convergenceCriteria.MinIterations; //only used in % complete
            Int64 iterationsPerComputeChunk = convergenceCriteria.IterationCount;
            Int64 computeChunkQuantity = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(convergenceCriteria.MinIterations) / iterationsPerComputeChunk));
            if (computeChunkQuantity < 1)
            {
                computeChunkQuantity = 1;
            }
            bool computeIsNotConverged = true;
            while (computeIsNotConverged)
            {
                int computeChunk = 0;
                while (computeChunk < computeChunkQuantity)
                {
                    try
                    {
                        Parallel.For(0, iterationsPerComputeChunk, thisChunkIteration =>
                        {
                            long iterationsCompletedByPriorComputeChunks = computeChunk * iterationsPerComputeChunk;

                            //thisComputeIteration is used for pulling the correct random number in sampling whereas thisChunkIteration is used for saving a result in the correct place of a temp results array
                            long thisComputeIteration = iterationsCompletedByPriorComputeChunks + thisChunkIteration;

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
                                PairedData frequencyDischarge;
                                if (_FrequencyDischargeGraphical.CurveMetaData.IsNull)
                                {
                                    frequencyDischarge = _FrequencyDischarge.BootstrapToPairedData(thisComputeIteration, utilities.DoubleGlobalStatics.RequiredExceedanceProbabilities, computeIsDeterministic);
                                }
                                else
                                {
                                    frequencyDischarge = _FrequencyDischargeGraphical.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                                }
                                //if frequency_flow is not defined throw big errors.
                                //check if flow transform exists, and use it here
                                if (_UnregulatedRegulated.CurveMetaData.IsNull)
                                {
                                    PairedData discharge_stage_sample = _DischargeStage.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                                    PairedData frequency_stage = discharge_stage_sample.compose(frequencyDischarge);
                                    ComputeFromStageFrequency(frequency_stage, thisComputeIteration, thisChunkIteration, computeWithDamage, computeWithLifeLoss, computeIsDeterministic);
                                }
                                else
                                {
                                    PairedData inflow_outflow_sample = _UnregulatedRegulated.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                                    PairedData transformff = inflow_outflow_sample.compose(frequencyDischarge);
                                    PairedData discharge_stage_sample = _DischargeStage.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                                    PairedData frequency_stage = discharge_stage_sample.compose(transformff);
                                    ComputeFromStageFrequency(frequency_stage, thisComputeIteration, thisChunkIteration, computeWithDamage, computeWithLifeLoss, computeIsDeterministic);
                                }

                            }
                            else
                            {
                                //if threadlocalRandomProvider is medianRandomProvider then we get a quasi-deterministic result
                                PairedData frequency_stage_sample = _FrequencyStage.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                                ComputeFromStageFrequency(frequency_stage_sample, thisComputeIteration, thisChunkIteration, computeWithDamage, computeWithLifeLoss, computeIsDeterministic);
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
                        computeChunk++;
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
                    computeChunkQuantity = Convert.ToInt64(expectedIterations / convergenceCriteria.IterationCount);
                    if (computeChunkQuantity == 0)
                    {
                        computeChunkQuantity = 1;
                    }
                    computeChunk = 0;
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

        private void ComputeFromStageFrequency(PairedData frequency_stage, long thisComputeIteration, long thisChunkIteration, bool computeWithDamage, bool computeWithLifeLoss, bool computeIsDeterministic)
        {
            //interior exterior
            if (_ChannelStageFloodplainStage.CurveMetaData.IsNull)
            {
                //levees
                if (_SystemResponseFunction.CurveMetaData.IsNull)
                {
                    if (computeWithDamage)
                    {
                        ComputeConsequencesFromStageFrequency(frequency_stage, thisComputeIteration, thisChunkIteration, computeIsDeterministic, _StageDamageFunctions); ///a;lsdfj;lasdfj;lasdfj;lkf
                    }
                    if (computeWithLifeLoss)
                    {
                        ComputeConsequencesFromStageFrequency(frequency_stage, thisComputeIteration, thisChunkIteration, computeIsDeterministic, _StageLifeLossFunctions);

                    }
                    ComputePerformance(frequency_stage, Convert.ToInt32(thisChunkIteration));
                }
                else
                {
                    PairedData systemResponse_sample = _SystemResponseFunction.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                    if (computeWithDamage)
                    {
                        ComputeDamagesFromStageFrequency_WithLevee(frequency_stage, systemResponse_sample, thisComputeIteration, thisChunkIteration, computeIsDeterministic);
                    }
                    //If the system response function is the default function 
                    if (systemResponse_sample.Xvals.Length <= 2)
                    {
                        ComputePerformance(frequency_stage, Convert.ToInt32(thisChunkIteration));
                    }
                    else
                    {
                        ComputeLeveePerformance(frequency_stage, systemResponse_sample, Convert.ToInt32(thisChunkIteration));
                    }


                }

            }
            else
            {
                PairedData _channelstage_floodplainstage_sample = _ChannelStageFloodplainStage.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                PairedData frequency_floodplainstage = _channelstage_floodplainstage_sample.compose(frequency_stage);
                //levees
                if (_SystemResponseFunction.CurveMetaData.IsNull)
                {
                    if (computeWithDamage)
                    {
                        ComputeConsequencesFromStageFrequency(frequency_floodplainstage, thisComputeIteration, thisChunkIteration, computeIsDeterministic, _StageDamageFunctions);
                    }
                    if (computeWithLifeLoss)
                    {
                        ComputeConsequencesFromStageFrequency(frequency_floodplainstage, thisComputeIteration, thisChunkIteration, computeIsDeterministic, _StageLifeLossFunctions);
                    }

                    ComputePerformance(frequency_floodplainstage, Convert.ToInt32(thisChunkIteration));
                }
                else
                {
                    PairedData systemResponse_sample = _SystemResponseFunction.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                    if (computeWithDamage)
                    {
                        ComputeDamagesFromStageFrequency_WithLeveeAndInteriorExterior(_channelstage_floodplainstage_sample, frequency_stage, systemResponse_sample, thisComputeIteration, thisChunkIteration, computeIsDeterministic);
                    }
                    //If the system response function is the default function
                    if (systemResponse_sample.Xvals.Length <= 2)
                    {
                        ComputePerformance(frequency_stage, Convert.ToInt32(thisChunkIteration));
                    }
                    else
                    {
                        ComputeLeveePerformance(frequency_stage, systemResponse_sample, Convert.ToInt32(thisChunkIteration));
                    }
                }

            }
        }

        private void ComputeConsequencesFromStageFrequency(PairedData frequency_stage, long thisComputeIteration, long thisChunkIteration, bool computeIsDeterministic, List<UncertainPairedData> consequenceFunctions)
        {
            foreach (UncertainPairedData stageUncertainConsequences in consequenceFunctions)
            {
                //TODO: here we need to check if stage damage is zero 
                //if so, then skip this stuff and just add 0 to consequenceResults
                PairedData _stage_consequences_sample = stageUncertainConsequences.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                PairedData frequency_consequences = _stage_consequences_sample.compose(frequency_stage);
                double eaConsequencesEstimate = frequency_consequences.integrate();
                _ImpactAreaScenarioResults.ConsequenceResults.AddConsequenceRealization(eaConsequencesEstimate, stageUncertainConsequences.CurveMetaData.DamageCategory, stageUncertainConsequences.CurveMetaData.AssetCategory, _ImpactAreaID, thisChunkIteration);
            }
        }
        private void ComputeDamagesFromStageFrequency_WithLevee(PairedData frequency_stage, PairedData systemResponse, long thisComputeIteration, long thisChunkIteration, bool computeIsDeterministic)
        {
            foreach (UncertainPairedData stageUncertainDamage in _StageDamageFunctions)
            {
                PairedData stageDamageFailSample = stageUncertainDamage.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                PairedData validatedSystemResponse = EnsureBottomAndTopHaveCorrectProbabilities(systemResponse);
                PairedData stageDamageFailAdjusted = stageDamageFailSample.multiply(validatedSystemResponse);

                if (NonFailRiskIncluded)
                {
                    foreach (UncertainPairedData stageUncertainNonFailureDamage in _NonFailureStageDamageFunctions)
                    {
                        if (stageUncertainNonFailureDamage.DamageCategory == stageUncertainDamage.DamageCategory
                            && stageUncertainNonFailureDamage.AssetCategory == stageUncertainDamage.AssetCategory)
                        {
                            PairedData inverseOfSystemResponse = CalculateFailureProbComplement(validatedSystemResponse);
                            PairedData stageDamNonFail = stageUncertainNonFailureDamage.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                            PairedData stageDamNonFailAdjusted = stageDamNonFail.multiply(inverseOfSystemResponse);
                            //stageDam Fail + StageDam Non-Fail
                            stageDamageFailAdjusted = stageDamageFailAdjusted.SumYsForGivenX(stageDamNonFailAdjusted);
                        }
                    }
                }
                PairedData frequency_damage = stageDamageFailAdjusted.compose(frequency_stage);
                double eadEstimate = frequency_damage.integrate();
                _ImpactAreaScenarioResults.ConsequenceResults.AddConsequenceRealization(eadEstimate, stageUncertainDamage.CurveMetaData.DamageCategory, stageUncertainDamage.CurveMetaData.AssetCategory, _ImpactAreaID, thisChunkIteration);
            }

        }

        private void ComputeDamagesFromStageFrequency_WithLeveeAndInteriorExterior(PairedData exterior_interior, PairedData frequency_exteriorStage, PairedData systemResponse, long thisComputeIteration, long thisChunkIteration, bool computeIsDeterministic)
        {
            foreach (UncertainPairedData stageUncertainDamage in _StageDamageFunctions)
            {
                PairedData interiorStage_damage_sample = stageUncertainDamage.SamplePairedData(thisComputeIteration, computeIsDeterministic);
                PairedData exteriorStage_damage_sample = interiorStage_damage_sample.compose(exterior_interior);
                PairedData validatedSystemResponse = EnsureBottomAndTopHaveCorrectProbabilities(systemResponse);
                PairedData stage_damage_sample_withLevee = exteriorStage_damage_sample.multiply(validatedSystemResponse);
                PairedData frequency_damage = stage_damage_sample_withLevee.compose(frequency_exteriorStage);
                double eadEstimate = frequency_damage.integrate();
                _ImpactAreaScenarioResults.ConsequenceResults.AddConsequenceRealization(eadEstimate, stageUncertainDamage.CurveMetaData.DamageCategory, stageUncertainDamage.CurveMetaData.AssetCategory, _ImpactAreaID, thisChunkIteration);
            }

        }

        private static PairedData CalculateFailureProbComplement(PairedData validatedSystemResponse)
        {
            double[] probabilityOfNonFailure = new double[validatedSystemResponse.Yvals.Length];
            for (int i = 0; i < probabilityOfNonFailure.Length; i++)
            {
                probabilityOfNonFailure[i] = 1 - (validatedSystemResponse.Yvals[i]);
            }
            PairedData complementOfSystemResponse = new(validatedSystemResponse.Xvals, probabilityOfNonFailure);
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
                PairedData totalStageDamage = ComputeTotalStageDamage(_StageDamageFunctions);
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

        internal static PairedData ComputeTotalDamageFrequency(PairedData pairedDataTotal, PairedData pairedDataToBeAddedToTotal)
        {
            pairedDataTotal = pairedDataTotal.SumYsForGivenX(pairedDataToBeAddedToTotal);
            return pairedDataTotal;
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

                for (int i = 0; i < systemResponseFunction.Xvals.Length; i++)
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
            foreach (UncertainPairedData stageDamage in _StageDamageFunctions)
            {
                foreach (UncertainPairedData incomingStageDamage in incomingImpactAreaScenarioSimulation._StageDamageFunctions)
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
        //TODO: Finish out serialization refactor 
        // Constants for XML element names
        private const string FREQUENCY_DISCHARGE_GRAPHICAL_ELEMENT_NAME = "FrequencyDischargeGraphical";
        private const string REGULATED_UNREGULATED_ELEMENT_NAME = "UnregulatedRegulated";
        private const string DISCHARGE_STAGE_ELEMENT_NAME = "DischargeStage";
        private const string FREQUENCY_STAGE_ELEMENT_NAME = "FrequencyStage";
        private const string INTERIOR_EXTERIOR_ELEMENT_NAME = "InteriorExterior";
        private const string SYSTEM_RESPONSE_ELEMENT_NAME = "SystemResponse";
        private const string IMPACT_AREA_SCENARIO_RESULTS_ELEMENT_NAME = "ImpactAreaScenarioResults";
        private const string STAGE_DAMAGE_LIST_ELEMENT_NAME = "stageDamageList";
        private const string NON_FAIL_STAGE_DAMAGE_LIST_ELEMENT_NAME = "nonFailStageDamageList";
        private const string IMPACT_AREA_ID_ELEMENT_NAME = "ImpactAreaID";
        private const string TOP_OF_LEVEE_ELEVATION_ELEMENT_NAME = "TopOfLeveeElevation";
        private const string FREQUENCY_DISCHARGE_IS_NULL_ELEMENT_NAME = "FrequencyDischargeIsNull";
        private const string IMPACT_AREA_SCENARIO_SIMULATION = "ImpactAreaScenarioSimulation";
        private const string LOG_PEARSON_3 = "LogPearson3";

        public XElement WriteToXML()
        {
            XElement mainElement = new(IMPACT_AREA_SCENARIO_SIMULATION);

            mainElement.SetAttributeValue(TOP_OF_LEVEE_ELEVATION_ELEMENT_NAME, _TopOfLeveeElevation);
            mainElement.SetAttributeValue(IMPACT_AREA_ID_ELEMENT_NAME, _ImpactAreaID);
            bool frequencyDischargeIsNull = ((Statistics.Distributions.LogPearson3)_FrequencyDischarge).IsNull;
            mainElement.SetAttributeValue(FREQUENCY_DISCHARGE_IS_NULL_ELEMENT_NAME, frequencyDischargeIsNull);
            if (!frequencyDischargeIsNull)
            {
                XElement frequenceDischarge = _FrequencyDischarge.ToXML();
                frequenceDischarge.Name = LOG_PEARSON_3;
                mainElement.Add(frequenceDischarge);
            }

            XElement frequencyDischargeGraphical = _FrequencyDischargeGraphical.WriteToXML();
            frequencyDischargeGraphical.Name = FREQUENCY_DISCHARGE_GRAPHICAL_ELEMENT_NAME;
            XElement regulatedUnregulated = _UnregulatedRegulated.WriteToXML();
            regulatedUnregulated.Name = REGULATED_UNREGULATED_ELEMENT_NAME;
            XElement dischargeStage = _DischargeStage.WriteToXML();
            dischargeStage.Name = DISCHARGE_STAGE_ELEMENT_NAME;
            XElement frequencyStage = _FrequencyStage.WriteToXML();
            frequencyStage.Name = FREQUENCY_STAGE_ELEMENT_NAME;
            XElement interiorExterior = _ChannelStageFloodplainStage.WriteToXML();
            interiorExterior.Name = INTERIOR_EXTERIOR_ELEMENT_NAME;
            XElement systemResponse = _SystemResponseFunction.WriteToXML();
            systemResponse.Name = SYSTEM_RESPONSE_ELEMENT_NAME;
            XElement impactAreaScenarioResults = _ImpactAreaScenarioResults.WriteToXml();
            impactAreaScenarioResults.Name = IMPACT_AREA_SCENARIO_RESULTS_ELEMENT_NAME;
            XElement stageDamageList = new(STAGE_DAMAGE_LIST_ELEMENT_NAME);

            foreach (UncertainPairedData stageDamage in _StageDamageFunctions)
            {
                XElement stageDamageElement = stageDamage.WriteToXML();
                stageDamageList.Add(stageDamageElement);
            }

            string nonFailStageDamageTag = Serialization.GetXMLTagFromProperty(GetType(), nameof(_NonFailureStageDamageFunctions));
            XElement nonFailStageDamageList = new(nonFailStageDamageTag);

            foreach (UncertainPairedData stageDamage in _NonFailureStageDamageFunctions)
            {
                XElement stageDamageElement = stageDamage.WriteToXML();
                nonFailStageDamageList.Add(stageDamageElement);
            }

            mainElement.Add(frequencyDischargeGraphical);
            mainElement.Add(regulatedUnregulated);
            mainElement.Add(dischargeStage);
            mainElement.Add(frequencyStage);
            mainElement.Add(interiorExterior);
            mainElement.Add(systemResponse);
            mainElement.Add(impactAreaScenarioResults);
            mainElement.Add(stageDamageList);
            mainElement.Add(nonFailStageDamageList);

            return mainElement;
        }
        public static ImpactAreaScenarioSimulation ReadFromXML(XElement xElement)
        {
            Type type = typeof(ImpactAreaScenarioSimulation);

            string nonFailStageDamageTag = Serialization.GetXMLTagFromProperty(type, nameof(_NonFailureStageDamageFunctions));
            List<UncertainPairedData> nonFailtageDamageList = new();
            foreach (XElement stageDamageElement in xElement.Element(nonFailStageDamageTag).Elements())
            {
                UncertainPairedData stageDamage = UncertainPairedData.ReadFromXML(stageDamageElement);
                nonFailtageDamageList.Add(stageDamage);
            }

            bool frequencyDischargeIsNull = Convert.ToBoolean(xElement.Attribute(FREQUENCY_DISCHARGE_IS_NULL_ELEMENT_NAME).Value);
            ContinuousDistribution frequencyDischarge;
            if (!frequencyDischargeIsNull)
            {
                frequencyDischarge = (ContinuousDistribution)ContinuousDistribution.FromXML(xElement.Element(LOG_PEARSON_3));
            }
            else
            {
                frequencyDischarge = new Statistics.Distributions.LogPearson3();
            }
            GraphicalUncertainPairedData frequencyDischargeGraphical = GraphicalUncertainPairedData.ReadFromXML(xElement.Element(FREQUENCY_DISCHARGE_GRAPHICAL_ELEMENT_NAME));
            UncertainPairedData regulatedUnregulated = UncertainPairedData.ReadFromXML(xElement.Element(REGULATED_UNREGULATED_ELEMENT_NAME));
            UncertainPairedData stageDischarge = UncertainPairedData.ReadFromXML(xElement.Element(DISCHARGE_STAGE_ELEMENT_NAME));
            GraphicalUncertainPairedData frequencyStage = GraphicalUncertainPairedData.ReadFromXML(xElement.Element(FREQUENCY_STAGE_ELEMENT_NAME));
            UncertainPairedData interiorExterior = UncertainPairedData.ReadFromXML(xElement.Element(INTERIOR_EXTERIOR_ELEMENT_NAME));
            UncertainPairedData systemResponse = UncertainPairedData.ReadFromXML(xElement.Element(SYSTEM_RESPONSE_ELEMENT_NAME));
            ImpactAreaScenarioResults impactAreaScenarioResults = ImpactAreaScenarioResults.ReadFromXML(xElement.Element(IMPACT_AREA_SCENARIO_RESULTS_ELEMENT_NAME));
            List<UncertainPairedData> stageDamageList = new();
            foreach (XElement stageDamageElement in xElement.Element(STAGE_DAMAGE_LIST_ELEMENT_NAME).Elements())
            {
                UncertainPairedData stageDamage = UncertainPairedData.ReadFromXML(stageDamageElement);
                stageDamageList.Add(stageDamage);
            }

            double topOfLeveeElevation = Convert.ToDouble(xElement.Attribute(TOP_OF_LEVEE_ELEVATION_ELEMENT_NAME).Value);
            int impactAreaID = Convert.ToInt32(xElement.Attribute(IMPACT_AREA_ID_ELEMENT_NAME).Value);

            ImpactAreaScenarioSimulation impactAreaScenarioSimulation = Builder(impactAreaID)
                           .WithFlowFrequency(frequencyDischarge)
                           .WithFlowFrequency(frequencyDischargeGraphical)
                           .WithInflowOutflow(regulatedUnregulated)
                           .WithFlowStage(stageDischarge)
                           .WithLevee(systemResponse, topOfLeveeElevation)
                           .WithStageDamages(stageDamageList)
                           .WithFrequencyStage(frequencyStage)
                           .WithInteriorExterior(interiorExterior)
                           .WithNonFailureStageDamage(nonFailtageDamageList)
                           .Build();
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
                _Simulation._SystemResponseFunction = uncertainPairedData;
                _Simulation._TopOfLeveeElevation = topOfLeveeElevation;
                return new SimulationBuilder(_Simulation);
            }
            public SimulationBuilder WithStageDamages(List<UncertainPairedData> uncertainPairedDataList)
            {
                _Simulation._StageDamageFunctions = uncertainPairedDataList;
                foreach (UncertainPairedData uncertainPairedData in _Simulation._StageDamageFunctions)
                {
                    _Simulation.AddSinglePropertyRule(
                        uncertainPairedData.CurveMetaData.DamageCategory + " stage damages",
                        new Rule(
                            () => { uncertainPairedData.Validate(); return !uncertainPairedData.HasErrors; },
                            $"Stage-damage errors ror the impact area with ID {_Simulation._ImpactAreaID}: " + uncertainPairedData.GetErrors().ToString()));
                }
                return new SimulationBuilder(_Simulation);
            }

            public SimulationBuilder WithStageLifeLoss(List<UncertainPairedData> uncertainPairedDataList)
            {
                _Simulation._StageLifeLossFunctions = uncertainPairedDataList;
                foreach (var upd in _Simulation._StageLifeLossFunctions)
                {
                    _Simulation.AddSinglePropertyRule(
                        upd.CurveMetaData.DamageCategory + " stage life loss",
                        new Rule(
                            () => { upd.Validate(); return !upd.HasErrors; },
                            $"Stage life loss errors for the impact area with ID {_Simulation._ImpactAreaID}: " + upd.GetErrors().ToString()));
                }
                return new SimulationBuilder(_Simulation);
            }

            public SimulationBuilder WithFakeStageLifeLoss()
            {
                double[] stages = { 935, 940, 945, 950 };
                CurveMetaData lifeLossMetaData = new("xLabel", "yLabel", "name", "LifeLoss", "LifeLoss");
                IDistribution[] lifeLoss =
                [
                    new Uniform(10, 10, 10),
                    new Uniform(12, 18, 10),
                    new Uniform(13, 19, 10),
                    new Uniform(14, 20, 10),
                ];
                UncertainPairedData stageLifeLoss = new(stages, lifeLoss, lifeLossMetaData);
                _Simulation._StageLifeLossFunctions.Add(stageLifeLoss);
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
                _Simulation.NonFailRiskIncluded = true;
                return new SimulationBuilder(_Simulation);
            }
        }

    }

}
