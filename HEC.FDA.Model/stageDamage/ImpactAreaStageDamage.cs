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
using System.Threading.Tasks;

namespace HEC.FDA.Model.stageDamage
{
    public class ImpactAreaStageDamage : PropertyValidationHelper, IDontImplementValidationButMyPropertiesDo
    {
        #region Hard Coded Compute Settings
        private const double MIN_PROBABILITY = 0.0001;
        private const double MAX_PROBABILITY = 0.9999;
        private readonly ConvergenceCriteria _ConvergenceCriteria = new(minIterations: 500, maxIterations: 5000);
        #endregion

        #region Fields 

        private readonly ContinuousDistribution _AnalyticalFlowFrequency;
        private readonly GraphicalUncertainPairedData _GraphicalFrequency;
        private readonly UncertainPairedData _DischargeStage;
        private readonly UncertainPairedData _UnregulatedRegulated;
        private readonly int _AnalysisYear;
        private readonly HydraulicDataset _HydraulicDataset;

        private double _MinStageForArea;
        private double _MaxStageForArea;

        //we have a way to figure out how many additional stages for which 
        //to calculate damage (we start with about 8 points)
        //these are the number of stages interpolated at the top of the stage damage function
        private int _MoreInterpolationPoints;
        //these are the number of stages interpolated at the bottom and in the middle of the function
        private int _LessInterpolationPoints;

        private readonly string _HydraulicParentDirectory;
        private readonly PairedData _StageFrequency;
        private double[] _StagesAtIndexLocation;
        #endregion

        #region Properties 
        public Inventory Inventory { get; }
        public int ImpactAreaID { get; }

        public event ProgressReportedEventHandler ProgressReport;
        public event MessageReportedEventHandler MessageReport;
        #endregion

        #region Constructor
        public ImpactAreaStageDamage(int impactAreaID, Inventory inventory, HydraulicDataset hydraulicDataset, string hydroParentDirectory, int analysisYear = 9999, ContinuousDistribution analyticalFlowFrequency = null,
            GraphicalUncertainPairedData graphicalFrequency = null, UncertainPairedData dischargeStage = null, UncertainPairedData unregulatedRegulated = null, bool usingMockData = false)
        {
            _HydraulicParentDirectory = hydroParentDirectory;
            _AnalyticalFlowFrequency = analyticalFlowFrequency;
            _GraphicalFrequency = graphicalFrequency;
            _DischargeStage = dischargeStage;
            _UnregulatedRegulated = unregulatedRegulated;
            ImpactAreaID = impactAreaID;
            _AnalysisYear = analysisYear;
            if (usingMockData)
            {
                Inventory = inventory;
            }
            else
            {
                Inventory = inventory.GetInventoryTrimmedToImpactArea(impactAreaID);
            }
            _HydraulicDataset = hydraulicDataset;
            SetMinAndMaxStage();
            SetCoordinateQuantity();
            _StageFrequency = CreateStageFrequency();
        }
        #endregion

        #region Methods
        //larger ranges need more points to preserve information content 
        private void SetCoordinateQuantity()
        {
            //depth-percent damage functions typically defined at half-foot intervals
            //this preserves the level of information content 
            double feetPerCoordinate = 0.5;
            double range = _MaxStageForArea - _MinStageForArea;
            int setsOfCoordinatesBetweenProfiles = 10;
            int coordinateQuantity = Convert.ToInt32(Math.Ceiling((range / feetPerCoordinate)/ setsOfCoordinatesBetweenProfiles));

            //require at least two coordinates to interpolate and extrapolate 
            if (coordinateQuantity < 4)
            {
                coordinateQuantity = 4;
            }
            _MoreInterpolationPoints = coordinateQuantity * 4;
            _LessInterpolationPoints = coordinateQuantity;

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

                    _MinStageForArea = minStagesOnRating.f(minFLow);
                    _MaxStageForArea = maxStagesOnRating.f(maxFLow);
                }
                else
                {
                    string message = "A stage-discharge function must accompany a flow-frequency function but no such function was found. Stage-damage compute aborted" + Environment.NewLine;
                    ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                    ReportMessage(this, new MessageEventArgs(errorMessage));
                }
            }
            else if (_GraphicalFrequency != null)
            {
                if (_GraphicalFrequency.GraphicalDistributionWithLessSimple.UsingStagesNotFlows)
                {
                    IPairedData minStages = _GraphicalFrequency.SamplePairedData(MIN_PROBABILITY);
                    _MinStageForArea = minStages.Yvals[0];
                    IPairedData maxStages = _GraphicalFrequency.SamplePairedData(MAX_PROBABILITY);
                    _MaxStageForArea = maxStages.Yvals[^1];
                }
                else
                {
                    if (_DischargeStage != null)
                    {
                        IPairedData minFlows = _GraphicalFrequency.SamplePairedData(MIN_PROBABILITY);
                        double minFlow = minFlows.Yvals[0];
                        IPairedData maxFlows = _GraphicalFrequency.SamplePairedData(MAX_PROBABILITY);
                        double maxFlow = maxFlows.Yvals[^1];

                        if (_UnregulatedRegulated != null)
                        {
                            minFlow = _UnregulatedRegulated.SamplePairedData(MIN_PROBABILITY).f(minFlow);
                            maxFlow = _UnregulatedRegulated.SamplePairedData(MAX_PROBABILITY).f(maxFlow);
                        }

                        IPairedData minStages = _DischargeStage.SamplePairedData(MIN_PROBABILITY);
                        IPairedData maxStages = _DischargeStage.SamplePairedData(MAX_PROBABILITY);

                        _MinStageForArea = minStages.f(minFlow);
                        _MaxStageForArea = maxStages.f(maxFlow);
                    }
                    else
                    {
                        string message = "A stage-discharge function must accompany a flow-frequency function but no such function was found. Stage-damage compute aborted" + Environment.NewLine;
                        ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                        ReportMessage(this, new MessageEventArgs(errorMessage));
                    }
                }
            }
            else
            {
                //use stages from hydarulics at index locations
                string message = "At this time, HEC-FDA does not allow a stage-damage compute without a frequency function. Stage-damage compute aborted" + Environment.NewLine;
                ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage));
            }
        }
        /// <summary>
        /// This method grabs the input summary relationships and generates the median stage frequency function 
        /// The frequencies in the function are used to align the aggregation stages to the stages at the structures 
        /// </summary>
        private PairedData CreateStageFrequency()
        {
            if (_AnalyticalFlowFrequency != null)
            {
                if (_DischargeStage != null)
                {
                    Tuple<double[], double[]> flowFreqAsTuple = _AnalyticalFlowFrequency.ToCoordinates();
                    PairedData flowFrequencyPairedData = new(flowFreqAsTuple.Item1, flowFreqAsTuple.Item2);
                    if (_UnregulatedRegulated != null)
                    {
                        flowFrequencyPairedData = _UnregulatedRegulated.SamplePairedData(0.5, true).compose(flowFrequencyPairedData) as PairedData;
                    }
                    return _DischargeStage.SamplePairedData(0.5, true).compose(flowFrequencyPairedData) as PairedData;
                }
            }
            else if (_GraphicalFrequency != null)
            {
                if (_GraphicalFrequency.GraphicalDistributionWithLessSimple.UsingStagesNotFlows)
                {
                    return _GraphicalFrequency.SamplePairedData(0.5, true) as PairedData;
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
                        return _DischargeStage.SamplePairedData(0.5, true).compose(flowFrequencyPairedData) as PairedData;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Begins the second loop of the Scenario Stage Damage Compute. 
        /// Scenario SD 
        /// Impact Area SD <--
        /// Damage Catagory 
        /// Compute Chunk
        /// Iteration
        /// Structure
        /// W.S.Profile
        /// </summary>
        /// <param name="randomProvider"></param>
        /// <returns></returns>
        public (List<UncertainPairedData>, List<UncertainPairedData>) Compute(IProvideRandomNumbers randomProvider)
        {
            Validate();
            (List<UncertainPairedData>, List<UncertainPairedData>) results = new(new List<UncertainPairedData>(), new List<UncertainPairedData>());
            if (ErrorLevel >= ErrorLevel.Major)
            {
                string message = "At least one component of the stage-damage compute has a major error or worse. The compute has been aborted. Empty stage-damage functions have been returned";
                ErrorMessage errorMessage = new(message, ErrorLevel);
                ReportMessage(this, new MessageEventArgs(errorMessage));
                return results;
            }
            else
            {

                List<string> damCats = Inventory.GetDamageCategories();
                (List<double>, List<float[]>) wsesAtEachStructureByProfile = _HydraulicDataset.GetHydraulicDatasetInFloatsWithProbabilities(Inventory, _HydraulicParentDirectory);
                _StagesAtIndexLocation = ComputeStagesAtIndexLocation(wsesAtEachStructureByProfile.Item1);
                //Run the compute by dam cat to simplify data collection 
                foreach (string damageCategory in damCats)
                {

                    (Inventory, List<float[]>) inventoryAndWaterTupled = Inventory.GetInventoryAndWaterTrimmedToDamageCategory(damageCategory, wsesAtEachStructureByProfile.Item2);


                    //There will be one ConsequenceDistributionResults object for each stage in the stage-damage function
                    //Each ConsequenceDistributionResults object holds a ConsequenceDistributionResult for each asset cat
                    List<StudyAreaConsequencesBinned> consequenceDistributionResults = ComputeDamageWithUncertaintyAllCoordinates(damageCategory, randomProvider, inventoryAndWaterTupled, wsesAtEachStructureByProfile.Item1);

                    //there should be four UncertainPairedData objects - one for each asset cat of the given dam cat level compute 
                    (List<UncertainPairedData>, List<UncertainPairedData>) tempResultsList = StudyAreaConsequencesBinned.ToUncertainPairedData(_StagesAtIndexLocation.ToList(), consequenceDistributionResults, ImpactAreaID);
                    //damage
                    results.Item1.AddRange(tempResultsList.Item1);
                    //quantity damaged elements
                    results.Item2.AddRange(tempResultsList.Item2);
                    //clear data
                }
                return results;
            }
        }
        /// <summary>
        /// Begins the third loop of the Scenario Stage Damage Compute. 
        /// Scenario SD 
        /// Impact Area SD 
        /// Damage Catagory <--
        /// Compute Chunk
        /// Iteration
        /// Structure
        /// W.S.Profile
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="randomProvider"></param>
        /// <param name="inventoryAndWaterTupled"></param>
        /// <param name="profileProbabilities"></param>
        /// <returns></returns>
        private List<StudyAreaConsequencesBinned> ComputeDamageWithUncertaintyAllCoordinates(string damageCategory, IProvideRandomNumbers randomProvider, (Inventory, List<float[]>) inventoryAndWaterTupled, List<double> profileProbabilities)
        {

            //damage for each stage
            List<StudyAreaConsequencesBinned> consequenceDistributionResults = CreateConsequenceDistributionResults(damageCategory);
            bool computeIsDeterministic = false;
            if (randomProvider is MedianRandomProvider)
            {
                computeIsDeterministic = true;
            }
            int iterations = 1;
            if (_ConvergenceCriteria.MinIterations >= 100)
            {
                iterations = 100;
            }
            int computeChunks = Convert.ToInt32(_ConvergenceCriteria.MinIterations / iterations);
            int sampleSize = 0;
            bool stageDamageFunctionsAreNotConverged = true;
            while (stageDamageFunctionsAreNotConverged)
            {

                /// Begins the fourth loop of the Scenario Stage Damage Compute. 
                /// Scenario SD 
                /// Impact Area SD 
                /// Damage Catagory 
                /// Compute Chunk <--
                /// Iteration
                /// Structure
                /// W.S.Profile
                for (int j = 0; j < computeChunks; j++)
                {
                    /// Begins the fifth loop of the Scenario Stage Damage Compute. 
                    /// Scenario SD 
                    /// Impact Area SD 
                    /// Damage Catagory 
                    /// Compute Chunk 
                    /// Iteration <--
                    /// Structure
                    /// W.S.Profile
                    for (int i = 0; i < iterations; i++)
                    {
                        List<DeterministicOccupancyType> deterministicOccTypes = Inventory.SampleOccupancyTypes(randomProvider, computeIsDeterministic);
                        ComputeLowerStageDamage(ref consequenceDistributionResults, damageCategory, deterministicOccTypes, inventoryAndWaterTupled, profileProbabilities, i);
                        ComputeMiddleStageDamage(ref consequenceDistributionResults, damageCategory, deterministicOccTypes, inventoryAndWaterTupled, profileProbabilities, i);
                        ComputeUpperStageDamage(ref consequenceDistributionResults, damageCategory, deterministicOccTypes, inventoryAndWaterTupled, profileProbabilities, i);
                        inventoryAndWaterTupled.Item1.ResetStructureWaterIndexTracking();
                        sampleSize += 1;
                    }
                    double percentComplete = sampleSize / _ConvergenceCriteria.MaxIterations;
                    ReportProgress(this, new ProgressReportEventArgs((int)percentComplete));
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

        private static void DumpDataIntoDistributions(ref List<StudyAreaConsequencesBinned> consequenceDistributionResultsList)
        {
            foreach (StudyAreaConsequencesBinned consequenceDistributionResults in consequenceDistributionResultsList)
            {
                consequenceDistributionResults.PutDataIntoHistograms();
            }
        }

        private List<StudyAreaConsequencesBinned> CreateConsequenceDistributionResults(string damageCategory)
        {
            List<StudyAreaConsequencesBinned> consequenceDistributionResultsList = new();

            for (int i = 0; i < _StagesAtIndexLocation.Length; i++)
            {
                List<AggregatedConsequencesBinned> consequenceDistributionResultList = new()
                {
                    new(damageCategory, utilities.StringGlobalConstants.STRUCTURE_ASSET_CATEGORY, _ConvergenceCriteria, ImpactAreaID),
                    new(damageCategory, utilities.StringGlobalConstants.CONTENT_ASSET_CATEGORY, _ConvergenceCriteria, ImpactAreaID),
                    new(damageCategory, utilities.StringGlobalConstants.OTHER_ASSET_CATEGORY, _ConvergenceCriteria, ImpactAreaID),
                    new(damageCategory, utilities.StringGlobalConstants.VEHICLE_ASSET_CATEGORY, _ConvergenceCriteria, ImpactAreaID)
                };
                StudyAreaConsequencesBinned consequenceDistributionResults = new(consequenceDistributionResultList);
                consequenceDistributionResultsList.Add(consequenceDistributionResults);
            }
            return consequenceDistributionResultsList;
        }

        private double[] ComputeStagesAtIndexLocation(List<double> profileProbabilities)
        {
            //less stages at the bottom, more stages at the top, less stages in between
            int quantityStages = _LessInterpolationPoints + _MoreInterpolationPoints + (profileProbabilities.Count - 1) * _LessInterpolationPoints;
            double[] stages = new double[quantityStages];
            //extrapolate lower stages
            double stageAtProbabilityOfLowestProfile = _StageFrequency.f(1 - profileProbabilities.Max());
            float indexStationLowerStageDelta = (float)(stageAtProbabilityOfLowestProfile - _MinStageForArea);
            float interval = indexStationLowerStageDelta / _LessInterpolationPoints;
            int stageIndex = 0;
            for (int i = 0; i < _LessInterpolationPoints + 1; i++)
            {
                stages[i] = (_MinStageForArea + i * interval);
                stageIndex++;
            }

            //interpolate intermediate stages
            int numProfiles = profileProbabilities.Count;
            for (int i = 1; i < numProfiles; i++)
            {
                double previousProbability = profileProbabilities[i - 1];
                double currentProbability = profileProbabilities[i];

                for (int j = 0; j < _LessInterpolationPoints; j++)
                {
                    double previousStageAtIndexLocation = _StageFrequency.f(1 - previousProbability);
                    double currentStageAtIndexLocation = _StageFrequency.f(1 - currentProbability);
                    double stageDeltaAtIndexLocation = currentStageAtIndexLocation - previousStageAtIndexLocation;
                    double intervalAtIndexLocation = stageDeltaAtIndexLocation / _LessInterpolationPoints;
                    double stageAtIndexLocation = previousStageAtIndexLocation + intervalAtIndexLocation * (j + 1);
                    stages[stageIndex] = stageAtIndexLocation;
                    stageIndex++;
                }
            }

            //extrapolate upper stages
            double stageAtProbabilityOfHighestProfile = _StageFrequency.f(1 - profileProbabilities.Min());
            float indexStationUpperStageDelta = (float)(_MaxStageForArea - stageAtProbabilityOfHighestProfile);
            float upperInterval = indexStationUpperStageDelta / _MoreInterpolationPoints;
            for (int i = 1; i < _MoreInterpolationPoints; i++)
            {

                stages[stageIndex] = (_MaxStageForArea - upperInterval * (_MoreInterpolationPoints - i));
                stageIndex++;
            }

            return stages;

        }

        private static bool IsTheFunctionNotConverged(List<StudyAreaConsequencesBinned> consequenceDistributionResults)
        {
            double lowerProb = 0.025;
            double upperProb = 0.975;
            foreach (StudyAreaConsequencesBinned consequences in consequenceDistributionResults)
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
        private void ComputeLowerStageDamage(ref List<StudyAreaConsequencesBinned> parallelConsequenceResultCollection, string damageCategory, List<DeterministicOccupancyType> deterministicOccTypes, (Inventory, List<float[]>) inventoryAndWaterCoupled, List<double> profileProbabilities, int iterationIndex)
        {

            float interval = CalculateLowerIncrementOfStages(profileProbabilities);
            List<float[]> stagesAtAllStructuresAllEvents = new();
            for (int stageIndex = 0; stageIndex < _LessInterpolationPoints + 1; stageIndex++)
            {
                //for each stage, add the consequenceResult to the consequenceResultArray in the correct place
                float[] WSEsParallelToIndexLocation = ExtrapolateFromBelowStagesAtIndexLocation(inventoryAndWaterCoupled.Item2[0], interval, stageIndex, _LessInterpolationPoints);
                //Can we modify the below to push more of the calculation into the parallelization. So, instead of passing in a float[] wses, it might be a float[][].
                stagesAtAllStructuresAllEvents.Add(WSEsParallelToIndexLocation);
            }
            List<ConsequenceResult> consequenceResults = inventoryAndWaterCoupled.Item1.ComputeDamages(stagesAtAllStructuresAllEvents, _AnalysisYear, damageCategory, deterministicOccTypes);
            int i = 0;
            foreach (ConsequenceResult consequenceResult in consequenceResults)
            {
                parallelConsequenceResultCollection[i].AddConsequenceRealization(consequenceResult, damageCategory, ImpactAreaID, iterationIndex);
                i++;
            }
        }

        private float CalculateLowerIncrementOfStages(List<double> profileProbabilities)
        {
            //the probability of a profile is an EXCEEDANCE probability but in the model we use NONEXCEEDANCE PROBABILITY
            double stageAtProbabilityOfLowestProfile = _StageFrequency.f(1 - profileProbabilities.Max());
            //the delta is the difference between the min stage at the index location and the stage at the index location for the lowest profile 
            float indexStationLowerStageDelta = (float)(stageAtProbabilityOfLowestProfile - _MinStageForArea);
            //this interval defines the interval in stages by which we'll compute damage 
            float interval = indexStationLowerStageDelta / _LessInterpolationPoints;
            //Collect damage for first part of function up to and including the stages at the lowest profile 
            return interval;
        }

        public static float[] ExtrapolateFromBelowStagesAtIndexLocation(float[] WSEsAtLowest, float interval, int i, int numInterpolatedStagesToCompute)
        {
            float[] extrapolatedStages = new float[WSEsAtLowest.Length];
            for (int j = 0; j < WSEsAtLowest.Length; j++)
            {
                extrapolatedStages[j] = WSEsAtLowest[j] - interval * (numInterpolatedStagesToCompute - i);
            }
            return extrapolatedStages;
        }

        /// <summary>
        /// This method calculates a stage damage function within the hydraulic profiles 
        /// </summary>
        private void ComputeMiddleStageDamage(ref List<StudyAreaConsequencesBinned> parallelConsequenceResultCollection, string damageCategory, List<DeterministicOccupancyType> deterministicOccTypes, (Inventory, List<float[]>) inventoryAndWaterCoupled, List<double> profileProbabilities, int iterationIndex)
        {
            int numProfiles = profileProbabilities.Count;
            int stageIndex = _LessInterpolationPoints + 1;
            for (int profileIndex = 1; profileIndex < numProfiles; profileIndex++)
            {
                InterpolateBetweenProfiles(ref parallelConsequenceResultCollection, deterministicOccTypes, inventoryAndWaterCoupled.Item2[profileIndex - 1], inventoryAndWaterCoupled.Item2[profileIndex], damageCategory, inventoryAndWaterCoupled.Item1, stageIndex, iterationIndex);
                stageIndex += _LessInterpolationPoints;
            }
        }


        private void InterpolateBetweenProfiles(ref List<StudyAreaConsequencesBinned> parallelConsequenceResultCollection, List<DeterministicOccupancyType> occTypes, float[] previousHydraulicProfile, float[] currentHydraulicProfile, string damageCategory, Inventory inventory, int stageIndex, int iterationIndex)
        {
            float[] intervalsAtStructures = CalculateIntervals(previousHydraulicProfile, currentHydraulicProfile);
            List<float[]> stagesAllStructuresAllStages = new();
            for (int interpolatorIndex = 0; interpolatorIndex < _LessInterpolationPoints; interpolatorIndex++)
            {
                float[] stages = CalculateIncrementOfStages(previousHydraulicProfile, intervalsAtStructures, interpolatorIndex + 1);
                stagesAllStructuresAllStages.Add(stages);
            }
            int i = 0;
            List<ConsequenceResult> consequenceResults = inventory.ComputeDamages(stagesAllStructuresAllStages, _AnalysisYear, damageCategory, occTypes);
            foreach (ConsequenceResult consequenceResult in consequenceResults)
            {
                parallelConsequenceResultCollection[stageIndex + i].AddConsequenceRealization(consequenceResult,damageCategory, ImpactAreaID, iterationIndex);
                i++;
            }
        }

        private static float[] CalculateIncrementOfStages(float[] previousStagesAtStructures, float[] intervalsAtStructures, int interpolatorIndex)
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
                intervals[j] = (currentStagesAtStructures[j] - previousStagesAtStructures[j]) / _LessInterpolationPoints;
            }
            return intervals;
        }
        /// <summary>
        /// this method calculates the stage damage function for stages higher than the least frequent profile 
        /// </summary>
        private void ComputeUpperStageDamage(ref List<StudyAreaConsequencesBinned> parallelConsequenceResultCollection, string damageCategory, List<DeterministicOccupancyType> deterministicOccTypes, (Inventory, List<float[]>) inventoryAndWaterCoupled, List<double> profileProbabilities, int iterationIndex)
        {
            //the probability of a profile is an EXCEEDANCE probability but in the model we use NONEXCEEDANCE PROBABILITY
            int stageIndex = _LessInterpolationPoints + _LessInterpolationPoints * (profileProbabilities.Count - 1);
            double stageAtProbabilityOfHighestProfile = _StageFrequency.f(1 - profileProbabilities.Min());
            float indexStationUpperStageDelta = (float)(_MaxStageForArea - stageAtProbabilityOfHighestProfile);
            float upperInterval = indexStationUpperStageDelta / _MoreInterpolationPoints;

            List<float[]> stagesAllStructuresAllEvents = new();
            for (int extrapolatorIndex = 1; extrapolatorIndex < _MoreInterpolationPoints; extrapolatorIndex++)
            {
                float[] WSEsParallelToIndexLocation = ExtrapolateFromAboveAtIndexLocation(inventoryAndWaterCoupled.Item2[^1], upperInterval, extrapolatorIndex);
                stagesAllStructuresAllEvents.Add(WSEsParallelToIndexLocation);
            }
            List<ConsequenceResult> consequenceResults = inventoryAndWaterCoupled.Item1.ComputeDamages(stagesAllStructuresAllEvents, _AnalysisYear, damageCategory, deterministicOccTypes);
            int i = 1;
            foreach (ConsequenceResult consequenceResult in consequenceResults)
            {
                parallelConsequenceResultCollection[stageIndex + i].AddConsequenceRealization(consequenceResult, damageCategory, ImpactAreaID, iterationIndex);
                i++;
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
        //TODO:
        //Do something with this. 
        public void ReportProgress(object sender, ProgressReportEventArgs e)
        {
            MessageHub.Register(this);
            ProgressReport?.Invoke(sender, e);
            MessageHub.Unregister(this);
        }
        internal List<string> ProduceImpactAreaStructureDetails()
        {
            //this list will be the size of the number of structures + 1 where the first string is the header
            List<DeterministicOccupancyType> deterministicOccupancyTypes = Inventory.SampleOccupancyTypes(new compute.MedianRandomProvider(), computeIsDeterministic: true);
            List<string> structureDetails = Inventory.StructureDetails(deterministicOccupancyTypes);
            //here I need to add to structure details: occ types, impact area,
            StagesToStrings(ref structureDetails);
            DepthsToStrings(ref structureDetails);
            DamagesToStrings(StringGlobalConstants.STRUCTURE_ASSET_CATEGORY, deterministicOccupancyTypes, ref structureDetails);
            DamagesToStrings(StringGlobalConstants.CONTENT_ASSET_CATEGORY, deterministicOccupancyTypes, ref structureDetails);
            DamagesToStrings(StringGlobalConstants.OTHER_ASSET_CATEGORY, deterministicOccupancyTypes, ref structureDetails);
            DamagesToStrings(StringGlobalConstants.VEHICLE_ASSET_CATEGORY, deterministicOccupancyTypes, ref structureDetails);

            return structureDetails;
        }

        private void DamagesToStrings(string assetType, List<DeterministicOccupancyType> deterministicOccupancyType, ref List<string> structureDetails)
 {
            foreach (IHydraulicProfile hydraulicProfile in _HydraulicDataset.HydraulicProfiles)
            {
                float[] stagesAtStructures = hydraulicProfile.GetWSE(Inventory.GetPointMs(), _HydraulicDataset.DataSource, _HydraulicParentDirectory);
                //first, create the header with the probability information on the hydraulic profile 
                //that will go in structureDetails[0]

                structureDetails[0] += $"{assetType} Damage At {hydraulicProfile.Probability}AEP,";
                List<ConsequenceResult> consequenceResultList = new();

                for (int i = 0; i < stagesAtStructures.Length; i++)
                {
                    ConsequenceResult consequenceResult = Inventory.Structures[i].ComputeDamage(stagesAtStructures[i], deterministicOccupancyType);
                    consequenceResultList.Add(consequenceResult);
                }

                if (assetType == StringGlobalConstants.STRUCTURE_ASSET_CATEGORY)
                {
                    for (int i = 0; i < stagesAtStructures.Length; i++)
                    {
                        double structureDamage = consequenceResultList[i].StructureDamage;
                        structureDetails[i + 1] += $"{structureDamage},";
                    }

                }
                else if (assetType == StringGlobalConstants.CONTENT_ASSET_CATEGORY)
                {
                    for (int i = 0; i < stagesAtStructures.Length; i++)
                    {
                        double contentDamage = consequenceResultList[i].ContentDamage;
                        structureDetails[i + 1] += $"{contentDamage},";
                    }

                }
                else if (assetType == StringGlobalConstants.VEHICLE_ASSET_CATEGORY)
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
            foreach (IHydraulicProfile hydraulicProfile in _HydraulicDataset.HydraulicProfiles)
            {
                float[] stagesAtStructures = hydraulicProfile.GetWSE(Inventory.GetPointMs(), _HydraulicDataset.DataSource, _HydraulicParentDirectory);
                //first, create the header with the probability information on the hydraulic profile 
                //that will go in structureDetails[0]
                structureDetails[0] += $"DepthAboveFirstFloorOf{hydraulicProfile.Probability}AEP,";
                for (int i = 0; i < stagesAtStructures.Length; i++)
                {
                    structureDetails[i + 1] += $"{stagesAtStructures[i] - Inventory.Structures[i].FirstFloorElevation},";
                }
            }
        }

        private void StagesToStrings(ref List<string> structureDetails)
        {
            foreach (IHydraulicProfile hydraulicProfile in _HydraulicDataset.HydraulicProfiles)
            {
                float[] stagesAtStructures = hydraulicProfile.GetWSE(Inventory.GetPointMs(), _HydraulicDataset.DataSource, _HydraulicParentDirectory);
                //first, create the header with the probability information on the hydraulic profile 
                //that will go in structureDetails[0]
                structureDetails[0] += $"StageOf{hydraulicProfile.Probability}AEP,";
                for (int i = 0; i < stagesAtStructures.Length; i++)
                {
                    structureDetails[i + 1] += $"{stagesAtStructures[i]},";
                }
            }
        }

        public string GetErrorsFromProperties()
        {
            string errors = "";
            ErrorLevel minErrorLevel = ErrorLevel.Unassigned;
            if (_AnalyticalFlowFrequency != null) { errors += _AnalyticalFlowFrequency.GetErrorMessages(minErrorLevel, nameof(_AnalyticalFlowFrequency) + Environment.NewLine); }
            if(_GraphicalFrequency != null) { errors += _GraphicalFrequency.GetErrorMessages(minErrorLevel, nameof(_GraphicalFrequency) + Environment.NewLine); }
            if(_DischargeStage != null) { errors += _DischargeStage.GetErrorMessages(minErrorLevel, nameof(_DischargeStage) + Environment.NewLine); }
            if(_UnregulatedRegulated != null) { errors += _UnregulatedRegulated.GetErrorMessages(minErrorLevel, nameof(_UnregulatedRegulated) + Environment.NewLine); }
            errors += Inventory.GetErrorsFromProperties();
            return errors;
        }

        public void Validate()
        {
            HasErrors = false;
            ErrorLevel = ErrorLevel.Unassigned;
            if(_AnalyticalFlowFrequency != null) { ValidateProperty(_AnalyticalFlowFrequency); }
            if(_GraphicalFrequency !=  null) { ValidateProperty(_GraphicalFrequency);}
            if(_DischargeStage != null) { ValidateProperty(_DischargeStage);}
            if(_UnregulatedRegulated != null) { ValidateProperty(_UnregulatedRegulated);}
            Inventory.Validate();
        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageHub.Register(this);
            MessageReport?.Invoke(sender, e);
            MessageHub.Unregister(this);
        }
        #endregion
    }
}
