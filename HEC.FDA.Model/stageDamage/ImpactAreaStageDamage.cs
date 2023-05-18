using HEC.FDA.Model.compute;
using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.Interfaces;
using HEC.FDA.Model.interfaces;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.structures;
using HEC.FDA.Model.utilities;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Model.Messaging;
using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.Model.stageDamage
{
    public class ImpactAreaStageDamage : ValidationErrorLogger, IContainValidationGroups
    {
        #region Fields 
        private const double MIN_PROBABILITY = 0.0001;
        private const double MAX_PROBABILITY = 0.9999;
        private ContinuousDistribution _AnalyticalFlowFrequency;
        private GraphicalUncertainPairedData _GraphicalFrequency;
        private UncertainPairedData _DischargeStage;
        private UncertainPairedData _UnregulatedRegulated;
        private int _ImpactAreaID;
        private int _AnalysisYear;
        private bool _usingMockData;
        private Inventory _inventory;
        private HydraulicDataset _hydraulicDataset;
        private int _StageBinWidth = 1;

        private double _minStageForArea;
        private double _maxStageForArea;
        private ConvergenceCriteria _ConvergenceCriteria;

        private int _numExtrapolatedStagesToCompute = 7;
        private int _numInterpolatedStagesToCompute = 2;

        private string _HydraulicParentDirectory;
        private PairedData _StageFrequency;
        private double[] _StagesAtIndexLocation;
        #endregion

        #region Properties 
        public Inventory Inventory
        {
            get { return _inventory; }
        }
        public int ImpactAreaID
        {
            get { return _ImpactAreaID; }
        }

        public List<ValidationGroup> ValidationGroups { get; } = new List<ValidationGroup>();

        public event ProgressReportedEventHandler ProgressReport;
        #endregion
        #region Constructor
        public ImpactAreaStageDamage(int impactAreaID, Inventory inventory, HydraulicDataset hydraulicDataset, ConvergenceCriteria convergence, string hydroParentDirectory, int analysisYear = 9999,
            ContinuousDistribution analyticalFlowFrequency = null, GraphicalUncertainPairedData graphicalFrequency = null, UncertainPairedData dischargeStage = null, UncertainPairedData unregulatedRegulated = null, bool usingMockData = false, string projectionFile = "")
        {
            //TODO: Validate provided functions here
            _HydraulicParentDirectory = hydroParentDirectory;
            _AnalyticalFlowFrequency = analyticalFlowFrequency;
            _GraphicalFrequency = graphicalFrequency;
            _DischargeStage = dischargeStage;
            _UnregulatedRegulated = unregulatedRegulated;
            _ImpactAreaID = impactAreaID;
            _AnalysisYear = analysisYear;
            _usingMockData = usingMockData;
            if (_usingMockData)
            {
                _inventory = inventory;
            }
            else
            {
                _inventory = inventory.GetInventoryTrimmedToImpactArea(impactAreaID);
            }
            _hydraulicDataset = hydraulicDataset;
            _ConvergenceCriteria = convergence;
            SetMinAndMaxStage();
            _StageFrequency = CreateStageFrequency();
            AddRules();

            ValidationGroup vg = new ValidationGroup("Impact area stage damage with impact area id '" + ImpactAreaID + "' has the following errors:");
            vg.ChildGroups.AddRange(_inventory.ValidationGroups);
            ValidationGroups.Add(vg);
        }
        #endregion

        #region Methods
        private void AddRules()
        {
            AddSinglePropertyRule(nameof(_inventory), new Rule(() => { _inventory.Validate(); return !_inventory.HasErrors; }, $"The structure inventory has errors: " + _inventory.GetErrors().ToString(), _inventory.ErrorLevel));
            AddSinglePropertyRule(nameof(_ConvergenceCriteria), new Rule(() => { _ConvergenceCriteria.Validate(); return !_ConvergenceCriteria.HasErrors; }, $"Convergence criteria has errors: " + _ConvergenceCriteria.GetErrors().ToString(), _ConvergenceCriteria.ErrorLevel));
            AddSinglePropertyRule(nameof(_StageFrequency), new Rule(() => _StageFrequency != null, $"The software was unable to calculate stage-frequency for the impact area with ID {_ImpactAreaID}", ErrorLevel.Fatal));
            if (_AnalyticalFlowFrequency != null)
            {
                AddSinglePropertyRule(nameof(_AnalyticalFlowFrequency), new Rule(() => { _AnalyticalFlowFrequency.Validate(); return !_AnalyticalFlowFrequency.HasErrors; }, $"The analytical flow-frequency function has errors: " + _AnalyticalFlowFrequency.GetErrors().ToString(), _AnalyticalFlowFrequency.ErrorLevel));
            }
            if (_GraphicalFrequency != null)
            {
                AddSinglePropertyRule(nameof(_GraphicalFrequency), new Rule(() => { _GraphicalFrequency.Validate(); return !_GraphicalFrequency.HasErrors; }, "The graphical frequency function has errors: " + _GraphicalFrequency.GetErrors().ToString(), _GraphicalFrequency.ErrorLevel));
            }
            if (_DischargeStage != null)
            {
                AddSinglePropertyRule(nameof(_DischargeStage), new Rule(() => { _DischargeStage.Validate(); return !_DischargeStage.HasErrors; }, "The stage-discharge function has errors: " + _DischargeStage.GetErrors().ToString(), _DischargeStage.ErrorLevel));
            }
        }
        /// <summary>
        /// This method is used to identify the minimum stage at the index location and the maximum stage at the index location for which we will calculate damage 
        /// </summary>
        private void SetMinAndMaxStage()
        {
            if (_AnalyticalFlowFrequency != null)
            {
                if (_DischargeStage != null)
                {
                    IPairedData minStagesOnRating = _DischargeStage.SamplePairedData(MIN_PROBABILITY);
                    IPairedData maxStagesOnRating = _DischargeStage.SamplePairedData(MAX_PROBABILITY);

                    double minFLow = _AnalyticalFlowFrequency.InverseCDF(MIN_PROBABILITY);
                    double maxFLow = _AnalyticalFlowFrequency.InverseCDF(MAX_PROBABILITY);

                    if (_UnregulatedRegulated != null)
                    {
                        minFLow = _UnregulatedRegulated.SamplePairedData(MIN_PROBABILITY).f(minFLow);
                        maxFLow = _UnregulatedRegulated.SamplePairedData(MAX_PROBABILITY).f(maxFLow);
                    }

                    _minStageForArea = minStagesOnRating.f(minFLow);
                    _maxStageForArea = maxStagesOnRating.f(maxFLow);
                }
                else
                {
                    string message = "A stage-discharge function must accompany a flow-frequency function but no such function was found. Stage-damage compute aborted" + Environment.NewLine;
                    ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
                    ReportMessage(this, new MessageEventArgs(errorMessage));
                }
            }
            else if (_GraphicalFrequency != null)
            {
                if (_GraphicalFrequency.UsesStagesNotFlows)
                {
                    IPairedData minStages = _GraphicalFrequency.SamplePairedData(MIN_PROBABILITY);
                    _minStageForArea = minStages.Yvals[0];
                    IPairedData maxStages = _GraphicalFrequency.SamplePairedData(MAX_PROBABILITY);
                    _maxStageForArea = maxStages.Yvals[maxStages.Yvals.Length - 1];
                }
                else
                {
                    if (_DischargeStage != null)
                    {
                        IPairedData minFlows = _GraphicalFrequency.SamplePairedData(MIN_PROBABILITY);
                        double minFlow = minFlows.Yvals[0];
                        IPairedData maxFlows = _GraphicalFrequency.SamplePairedData(MAX_PROBABILITY);
                        double maxFlow = maxFlows.Yvals[maxFlows.Yvals.Length - 1];

                        if (_UnregulatedRegulated != null)
                        {
                            minFlow = _UnregulatedRegulated.SamplePairedData(MIN_PROBABILITY).f(minFlow);
                            maxFlow = _UnregulatedRegulated.SamplePairedData(MAX_PROBABILITY).f(maxFlow);
                        }

                        IPairedData minStages = _DischargeStage.SamplePairedData(MIN_PROBABILITY);
                        IPairedData maxStages = _DischargeStage.SamplePairedData(MAX_PROBABILITY);

                        _minStageForArea = minStages.f(minFlow);
                        _maxStageForArea = maxStages.f(maxFlow);
                    }
                    else
                    {
                        string message = "A stage-discharge function must accompany a flow-frequency function but no such function was found. Stage-damage compute aborted" + Environment.NewLine;
                        ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
                        ReportMessage(this, new MessageEventArgs(errorMessage));
                    }
                }
            }
            else
            {
                //use stages from hydarulics at index locations
                string message = "At this time, HEC-FDA does not allow a stage-damage compute without a frequency function. Stage-damage compute aborted" + Environment.NewLine;
                ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage));
            }
        }
        /// <summary>
        /// This method grabs the input summary relationships and generates the median stage frequency function 
        /// The frequencies in the function are used to align the aggregation stages to the stages at the structures 
        /// </summary>
        private PairedData CreateStageFrequency()
        {
            PairedData stageFrequency;
            if (_AnalyticalFlowFrequency != null)
            {
                if (_DischargeStage != null)
                {
                    Tuple<double[], double[]> flowFreqAsTuple = _AnalyticalFlowFrequency.ToCoordinates();
                    PairedData flowFrequencyPairedData = new PairedData(flowFreqAsTuple.Item1, flowFreqAsTuple.Item2);
                    if (_UnregulatedRegulated != null)
                    {
                        flowFrequencyPairedData = _UnregulatedRegulated.SamplePairedData(0.5, true).compose(flowFrequencyPairedData) as PairedData;
                    }
                    return stageFrequency = _DischargeStage.SamplePairedData(0.5, true).compose(flowFrequencyPairedData) as PairedData;
                }
            }
            else if (_GraphicalFrequency != null)
            {
                if (_GraphicalFrequency.UsesStagesNotFlows)
                {
                    return stageFrequency = _GraphicalFrequency.SamplePairedData(0.5, true) as PairedData;
                }
                else
                {
                    if (_DischargeStage != null)
                    {
                        PairedData flowFrequencyPairedData = _GraphicalFrequency.SamplePairedData(0.5, true) as PairedData;
                        if (_UnregulatedRegulated != null)
                        {
                            flowFrequencyPairedData = _UnregulatedRegulated.SamplePairedData(0.5, true).compose(flowFrequencyPairedData) as PairedData;
                        }
                        return stageFrequency = _DischargeStage.SamplePairedData(0.5, true).compose(flowFrequencyPairedData) as PairedData;
                    }
                }
            }
            return null;
        }

        public List<UncertainPairedData> Compute(IProvideRandomNumbers randomProvider)
        {
            Validate();
            List<UncertainPairedData> results = new List<UncertainPairedData>();
            if (ErrorLevel >= ErrorLevel.Major)
            {
                string message = "At least one component of the stage-damage compute has a major error or worse. The compute has been aborted. Empty stage-damage functions have been returned";
                ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel);
                ReportMessage(this, new MessageEventArgs(errorMessage));
                return results;
            }
            else
            {

                List<string> damCats = _inventory.GetDamageCategories();
                (List<double>, List<float[]>) wsesAtEachStructureByProfile = _hydraulicDataset.GetHydraulicDatasetInFloatsWithProbabilities(_inventory, _HydraulicParentDirectory);
                _StagesAtIndexLocation = ComputeStagesAtIndexLocation(wsesAtEachStructureByProfile.Item1);
                //Run the compute by dam cat to simplify data collection 
                foreach (string damageCategory in damCats)
                {

                    (Inventory, List<float[]>) inventoryAndWaterTupled = _inventory.GetInventoryAndWaterTrimmedToDamageCategory(damageCategory, wsesAtEachStructureByProfile.Item2);


                    //There will be one ConsequenceDistributionResults object for each stage in the stage-damage function
                    //Each ConsequenceDistributionResults object holds a ConsequenceDistributionResult for each asset cat
                    List<ConsequenceDistributionResults> consequenceDistributionResults = ComputeDamageWithUncertaintyAllCoordinates(damageCategory, randomProvider, inventoryAndWaterTupled, wsesAtEachStructureByProfile.Item1);

                    //there should be four UncertainPairedData objects - one for each asset cat of the given dam cat level compute 
                    List<UncertainPairedData> tempResultsList = ConsequenceDistributionResults.ToUncertainPairedData(_StagesAtIndexLocation.ToList(), consequenceDistributionResults, _ImpactAreaID);
                    results.AddRange(tempResultsList);
                    //clear data
                }
                return results;
            }
        }

        private List<ConsequenceDistributionResults> ComputeDamageWithUncertaintyAllCoordinates(string damageCategory, IProvideRandomNumbers randomProvider, (Inventory, List<float[]>) inventoryAndWaterTupled, List<double> profileProbabilities)
        {

            //damage for each stage
            List<ConsequenceDistributionResults> consequenceDistributionResults = CreateConsequenceDistributionResults(damageCategory);

            int iterations = 1;
            if (_ConvergenceCriteria.MinIterations >= 100)
            {
                iterations = 100;
            }
            int computeChunks = Convert.ToInt32(_ConvergenceCriteria.MinIterations / iterations);
            bool stageDamageFunctionsAreNotConverged = true;
            while (stageDamageFunctionsAreNotConverged)
            {

                //InitializeParallelArrays(ref parallelConsequenceResultCollection);
                for (int j = 0; j < computeChunks; j++)
                {
                    for (int i = 0; i < iterations; i++)
                    {
                        List<DeterministicOccupancyType> deterministicOccTypes = _inventory.SampleOccupancyTypes(randomProvider);
                        ComputeLowerStageDamage(ref consequenceDistributionResults, damageCategory, deterministicOccTypes, inventoryAndWaterTupled, profileProbabilities, i);
                        ComputeMiddleStageDamage(ref consequenceDistributionResults, damageCategory, deterministicOccTypes, inventoryAndWaterTupled, profileProbabilities, i);
                        ComputeUpperStageDamage(ref consequenceDistributionResults, damageCategory, deterministicOccTypes, inventoryAndWaterTupled, profileProbabilities, i);
                    }

                    DumpDataIntoDistributions(ref consequenceDistributionResults);
                }
                stageDamageFunctionsAreNotConverged = IsTheFunctionNotConverged(consequenceDistributionResults);
                if (stageDamageFunctionsAreNotConverged)
                {
                    //TODO: I am going to hard-wire in an additional 10000 iterations for now. 
                    //At some point we can estimate iterations remaining - but that is computationally expensive 
                    computeChunks = 100;
                }
            }
            return consequenceDistributionResults;
        }

        private void DumpDataIntoDistributions(ref List<ConsequenceDistributionResults> consequenceDistributionResultsList)
        {
            foreach (ConsequenceDistributionResults consequenceDistributionResults in consequenceDistributionResultsList)
            {
                consequenceDistributionResults.PutDataIntoHistograms();
            }
        }

        private List<ConsequenceDistributionResults> CreateConsequenceDistributionResults(string damageCategory)
        {
            List<ConsequenceDistributionResults> consequenceDistributionResultsList = new List<ConsequenceDistributionResults>();

            for (int i = 0; i < _StagesAtIndexLocation.Length; i++)
            {
                ConsequenceDistributionResults consequenceDistributionResults = new ConsequenceDistributionResults(_ConvergenceCriteria);
                consequenceDistributionResults.AddNewConsequenceResultObject(damageCategory, utilities.StringConstants.STRUCTURE_ASSET_CATEGORY, _ConvergenceCriteria, _StageBinWidth, ImpactAreaID);
                consequenceDistributionResults.AddNewConsequenceResultObject(damageCategory, utilities.StringConstants.CONTENT_ASSET_CATEGORY, _ConvergenceCriteria, _StageBinWidth, ImpactAreaID);
                consequenceDistributionResults.AddNewConsequenceResultObject(damageCategory, utilities.StringConstants.OTHER_ASSET_CATEGORY, _ConvergenceCriteria, _StageBinWidth, ImpactAreaID);
                consequenceDistributionResults.AddNewConsequenceResultObject(damageCategory, utilities.StringConstants.VEHICLE_ASSET_CATEGORY, _ConvergenceCriteria, _StageBinWidth, ImpactAreaID);
                consequenceDistributionResultsList.Add(consequenceDistributionResults);
            }
            return consequenceDistributionResultsList;
        }

        private double[] ComputeStagesAtIndexLocation(List<double> profileProbabilities)
        {
            //extrapolate lower stages
            int quantityStages = _numExtrapolatedStagesToCompute * 2 + (profileProbabilities.Count - 1) * _numInterpolatedStagesToCompute;
            double[] stages = new double[quantityStages];
            double stageAtProbabilityOfLowestProfile = _StageFrequency.f(1 - profileProbabilities.Max());
            float indexStationLowerStageDelta = (float)(stageAtProbabilityOfLowestProfile - _minStageForArea);
            float interval = indexStationLowerStageDelta / _numExtrapolatedStagesToCompute;
            int stageIndex = 0;
            for (int i = 0; i < _numExtrapolatedStagesToCompute + 1; i++)
            {
                stages[i] = (_minStageForArea + i * interval);
                stageIndex++;
            }

            //interpolate intermediate stages
            int numProfiles = profileProbabilities.Count;
            for (int i = 1; i < numProfiles; i++)
            {
                double previousProbability = profileProbabilities[i - 1];
                double currentProbability = profileProbabilities[i];

                for (int j = 0; j < _numInterpolatedStagesToCompute; j++)
                {
                    double previousStageAtIndexLocation = _StageFrequency.f(1 - previousProbability);
                    double currentStageAtIndexLocation = _StageFrequency.f(1 - currentProbability);
                    double stageDeltaAtIndexLocation = currentStageAtIndexLocation - previousStageAtIndexLocation;
                    double intervalAtIndexLocation = stageDeltaAtIndexLocation / _numInterpolatedStagesToCompute;
                    double stageAtIndexLocation = previousStageAtIndexLocation + intervalAtIndexLocation * (j + 1);
                    stages[stageIndex] = stageAtIndexLocation;
                    stageIndex++;
                }
            }

            //extrapolate upper stages
            double stageAtProbabilityOfHighestProfile = _StageFrequency.f(1 - profileProbabilities.Min());
            float indexStationUpperStageDelta = (float)(_maxStageForArea - stageAtProbabilityOfHighestProfile);
            float upperInterval = indexStationUpperStageDelta / _numExtrapolatedStagesToCompute;
            for (int i = 1; i < _numExtrapolatedStagesToCompute; i++)
            {

                stages[stageIndex] = (_maxStageForArea - upperInterval * (_numExtrapolatedStagesToCompute - i));
                stageIndex++;
            }

            return stages;

        }


        private bool IsTheFunctionNotConverged(List<ConsequenceDistributionResults> consequenceDistributionResults)
        {
            double lowerProb = 0.025;
            double upperProb = 0.975;
            foreach (ConsequenceDistributionResults consequences in consequenceDistributionResults)
            {
                bool isConverged = consequences.ResultsAreConverged(upperProb, lowerProb);
                if (!isConverged)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This method computes damage at stages lower than the most frequent profile 
        /// </summary>
        private void ComputeLowerStageDamage(ref List<ConsequenceDistributionResults> parallelConsequenceResultCollection, string damageCategory, List<DeterministicOccupancyType> deterministicOccTypes, (Inventory, List<float[]>) inventoryAndWaterCoupled, List<double> profileProbabilities, int iterationIndex)
        {

            float interval = CalculateLowerIncrementOfStages(profileProbabilities);
            for (int stageIndex = 0; stageIndex < _numExtrapolatedStagesToCompute + 1; stageIndex++)
            {
                //for each stage, add the consequenceResult to the consequenceResultArray in the correct place
                float[] WSEsParallelToIndexLocation = ExtrapolateFromBelowStagesAtIndexLocation(inventoryAndWaterCoupled.Item2[0], interval, stageIndex, _numExtrapolatedStagesToCompute);
                //this inventory is not trimmed 
                ConsequenceResult consequenceResult = inventoryAndWaterCoupled.Item1.ComputeDamages(WSEsParallelToIndexLocation, _AnalysisYear, damageCategory, deterministicOccTypes);
                parallelConsequenceResultCollection[stageIndex].AddConsequenceRealization(consequenceResult, damageCategory, ImpactAreaID, iterationIndex);
            }
        }

        private float CalculateLowerIncrementOfStages(List<double> profileProbabilities)
        {
            //the probability of a profile is an EXCEEDANCE probability but in the model we use NONEXCEEDANCE PROBABILITY
            double stageAtProbabilityOfLowestProfile = _StageFrequency.f(1 - profileProbabilities.Max());
            //the delta is the difference between the min stage at the index location and the stage at the index location for the lowest profile 
            float indexStationLowerStageDelta = (float)(stageAtProbabilityOfLowestProfile - _minStageForArea);
            //this interval defines the interval in stages by which we'll compute damage 
            float interval = indexStationLowerStageDelta / _numExtrapolatedStagesToCompute;
            //Collect damage for first part of function up to and including the stages at the lowest profile 
            return interval;
        }

        public static float[] ExtrapolateFromBelowStagesAtIndexLocation(float[] WSEsAtLowest, float interval, int i, int numExtrapolatedStagesToCompute)
        {
            float[] extrapolatedStages = new float[WSEsAtLowest.Length];
            for (int j = 0; j < WSEsAtLowest.Length; j++)
            {
                extrapolatedStages[j] = WSEsAtLowest[j] - interval * (numExtrapolatedStagesToCompute - i);
            }
            return extrapolatedStages;
        }

        /// <summary>
        /// This method calculates a stage damage function within the hydraulic profiles 
        /// </summary>
        private void ComputeMiddleStageDamage(ref List<ConsequenceDistributionResults> parallelConsequenceResultCollection, string damageCategory, List<DeterministicOccupancyType> deterministicOccTypes, (Inventory, List<float[]>) inventoryAndWaterCoupled, List<double> profileProbabilities, int iterationIndex)
        {
            int numProfiles = profileProbabilities.Count;
            int stageIndex = _numExtrapolatedStagesToCompute + 1;
            for (int profileIndex = 1; profileIndex < numProfiles; profileIndex++)
            {
                InterpolateBetweenProfiles(ref parallelConsequenceResultCollection, deterministicOccTypes, inventoryAndWaterCoupled.Item2[profileIndex - 1], profileProbabilities[profileIndex - 1], inventoryAndWaterCoupled.Item2[profileIndex], profileProbabilities[profileIndex], damageCategory, profileIndex, inventoryAndWaterCoupled.Item1, stageIndex, iterationIndex);
                stageIndex += _numInterpolatedStagesToCompute;
            }
        }
        private void InterpolateBetweenProfiles(ref List<ConsequenceDistributionResults> parallelConsequenceResultCollection, List<DeterministicOccupancyType> occTypes, float[] previousHydraulicProfile, double previousProbability, float[] currentHydraulicProfile, double currentProbability, string damageCategory, int profileCount, Inventory inventory, int stageIndex, int iterationIndex)
        {
            float[] intervalsAtStructures = CalculateIntervals(previousHydraulicProfile, currentHydraulicProfile);
            for (int interpolatorIndex = 0; interpolatorIndex < _numInterpolatedStagesToCompute; interpolatorIndex++)
            {
                float[] stages = CalculateIncrementOfStages(previousHydraulicProfile, intervalsAtStructures, interpolatorIndex + 1);
                ConsequenceResult consequenceResult = inventory.ComputeDamages(stages, _AnalysisYear, damageCategory, occTypes);
                parallelConsequenceResultCollection[stageIndex + interpolatorIndex].AddConsequenceRealization(consequenceResult,damageCategory, ImpactAreaID, iterationIndex);
            }
        }

        private float[] CalculateIncrementOfStages(float[] previousStagesAtStructures, float[] intervalsAtStructures, int interpolatorIndex)
        {
            float[] stages = new float[intervalsAtStructures.Length];
            for (int m = 0; m < stages.Length; m++)
            {
                stages[m] = previousStagesAtStructures[m] + intervalsAtStructures[m] * interpolatorIndex;
            }
            return stages;
        }

        private float[] CalculateIntervals(float[] previousStagesAtStructures, float[] currentStagesAtStructures)
        {
            float[] intervals = new float[previousStagesAtStructures.Length];
            for (int j = 0; j < previousStagesAtStructures.Length; j++)
            {
                intervals[j] = (currentStagesAtStructures[j] - previousStagesAtStructures[j]) / _numInterpolatedStagesToCompute;
            }
            return intervals;
        }
        /// <summary>
        /// this method calculates the stage damage function for stages higher than the least frequent profile 
        /// </summary>
        private void ComputeUpperStageDamage(ref List<ConsequenceDistributionResults> parallelConsequenceResultCollection, string damageCategory, List<DeterministicOccupancyType> deterministicOccTypes, (Inventory, List<float[]>) inventoryAndWaterCoupled, List<double> profileProbabilities, int iterationIndex)
        {
            //the probability of a profile is an EXCEEDANCE probability but in the model we use NONEXCEEDANCE PROBABILITY
            int stageIndex = _numExtrapolatedStagesToCompute + _numInterpolatedStagesToCompute * profileProbabilities.Count - 2;
            double stageAtProbabilityOfHighestProfile = _StageFrequency.f(1 - profileProbabilities.Min());
            float indexStationUpperStageDelta = (float)(_maxStageForArea - stageAtProbabilityOfHighestProfile);
            float upperInterval = indexStationUpperStageDelta / _numExtrapolatedStagesToCompute;
            for (int extrapolatorIndex = 1; extrapolatorIndex < _numExtrapolatedStagesToCompute; extrapolatorIndex++)
            {
                float[] WSEsParallelToIndexLocation = ExtrapolateFromAboveAtIndexLocation(inventoryAndWaterCoupled.Item2[inventoryAndWaterCoupled.Item2.Count - 1], upperInterval, extrapolatorIndex);
                ConsequenceResult consequenceResult = inventoryAndWaterCoupled.Item1.ComputeDamages(WSEsParallelToIndexLocation, _AnalysisYear, damageCategory, deterministicOccTypes);
                parallelConsequenceResultCollection[stageIndex + extrapolatorIndex].AddConsequenceRealization(consequenceResult, damageCategory, ImpactAreaID, iterationIndex);
            }
        }

        //this is public and static for testing
        public static float[] ExtrapolateFromAboveAtIndexLocation(float[] stagesAtStructuresHighestProfile, float upperInterval, int stepCount)
        {
            float[] extrapolatedStages = new float[stagesAtStructuresHighestProfile.Length];
            for (int i = 0; i < stagesAtStructuresHighestProfile.Length; i++)
            {
                extrapolatedStages[i] = stagesAtStructuresHighestProfile[i] + upperInterval * stepCount;
            }
            return extrapolatedStages;
        }
        public void ReportProgress(object sender, ProgressReportEventArgs e)
        {
            ProgressReport?.Invoke(sender, e);
        }
        internal List<string> ProduceImpactAreaStructureDetails()
        {
            //this list will be the size of the number of structures + 1 where the first string is the header
            List<string> structureDetails = _inventory.StructureDetails();
            List<DeterministicOccupancyType> deterministicOccupancyTypes = _inventory.SampleOccupancyTypes(new compute.MedianRandomProvider());
            //DeterministicInventory deterministicInventory = .Sample(, computeIsDeterministic: true);
            StagesToStrings(ref structureDetails);
            DepthsToStrings(ref structureDetails);
            DamagesToStrings(StringConstants.STRUCTURE_ASSET_CATEGORY, deterministicOccupancyTypes, ref structureDetails);
            DamagesToStrings(StringConstants.CONTENT_ASSET_CATEGORY, deterministicOccupancyTypes, ref structureDetails);
            DamagesToStrings(StringConstants.OTHER_ASSET_CATEGORY, deterministicOccupancyTypes, ref structureDetails);
            DamagesToStrings(StringConstants.VEHICLE_ASSET_CATEGORY, deterministicOccupancyTypes, ref structureDetails);

            return structureDetails;
        }

        private void DamagesToStrings(string assetType, List<DeterministicOccupancyType> deterministicOccupancyType, ref List<string> structureDetails)
        {
            foreach (IHydraulicProfile hydraulicProfile in _hydraulicDataset.HydraulicProfiles)
            {
                float[] stagesAtStructures = hydraulicProfile.GetWSE(_inventory.GetPointMs(), _hydraulicDataset.DataSource, _HydraulicParentDirectory);
                //first, create the header with the probability information on the hydraulic profile 
                //that will go in structureDetails[0]

                structureDetails[0] += $"{assetType} Damage At {hydraulicProfile.Probability}AEP,";
                List<ConsequenceResult> consequenceResultList = new List<ConsequenceResult>();

                for (int i = 0; i < stagesAtStructures.Length; i++)
                {
                    ConsequenceResult consequenceResult = _inventory.Structures[i].ComputeDamage(stagesAtStructures[i], deterministicOccupancyType);
                    consequenceResultList.Add(consequenceResult);
                }

                if (assetType == StringConstants.STRUCTURE_ASSET_CATEGORY)
                {
                    for (int i = 0; i < stagesAtStructures.Length; i++)
                    {
                        double structureDamage = consequenceResultList[i].StructureDamage;
                        structureDetails[i + 1] += $"{structureDamage},";
                    }

                }
                else if (assetType == StringConstants.CONTENT_ASSET_CATEGORY)
                {
                    for (int i = 0; i < stagesAtStructures.Length; i++)
                    {
                        double contentDamage = consequenceResultList[i].ContentDamage;
                        structureDetails[i + 1] += $"{contentDamage},";
                    }

                }
                else if (assetType == StringConstants.VEHICLE_ASSET_CATEGORY)
                {
                    for (int i = 0; i < stagesAtStructures.Length; i++)
                    {
                        double vehicleDamage = consequenceResultList[i].VehicleDamage;
                        structureDetails[i + 1] += $"{vehicleDamage},";
                    }

                }
                else
                {
                    for (int i = 0; i < stagesAtStructures.Length; i++)
                    {
                        double otherDamage = consequenceResultList[i].OtherDamage;
                        structureDetails[i + 1] += $"{otherDamage},";
                    }
                }

            }
        }

        private void DepthsToStrings(ref List<string> structureDetails)
        {
            foreach (IHydraulicProfile hydraulicProfile in _hydraulicDataset.HydraulicProfiles)
            {
                float[] stagesAtStructures = hydraulicProfile.GetWSE(_inventory.GetPointMs(), _hydraulicDataset.DataSource, _HydraulicParentDirectory);
                //first, create the header with the probability information on the hydraulic profile 
                //that will go in structureDetails[0]
                structureDetails[0] += $"DepthAboveFirstFloorOf{hydraulicProfile.Probability}AEP,";
                for (int i = 0; i < stagesAtStructures.Length; i++)
                {
                    structureDetails[i + 1] += $"{stagesAtStructures[i] - _inventory.Structures[i].FirstFloorElevation},";
                }
            }
        }

        private void StagesToStrings(ref List<string> structureDetails)
        {
            foreach (IHydraulicProfile hydraulicProfile in _hydraulicDataset.HydraulicProfiles)
            {
                float[] stagesAtStructures = hydraulicProfile.GetWSE(_inventory.GetPointMs(), _hydraulicDataset.DataSource, _HydraulicParentDirectory);
                //first, create the header with the probability information on the hydraulic profile 
                //that will go in structureDetails[0]
                structureDetails[0] += $"StageOf{hydraulicProfile.Probability}AEP,";
                for (int i = 0; i < stagesAtStructures.Length; i++)
                {
                    structureDetails[i + 1] += $"{stagesAtStructures[i]},";
                }
            }
        }
        #endregion
    }
}
