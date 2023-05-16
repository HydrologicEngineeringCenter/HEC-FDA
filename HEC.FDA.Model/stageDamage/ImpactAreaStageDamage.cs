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
        #region Hard Coded Compute Settings
        private const double MIN_PROBABILITY = 0.0001;
        private const double MAX_PROBABILITY = 0.9999;
        #endregion

        #region Fields 

        private readonly ContinuousDistribution _AnalyticalFlowFrequency;
        private readonly GraphicalUncertainPairedData _GraphicalFrequency;
        private readonly UncertainPairedData _DischargeStage;
        private readonly UncertainPairedData _UnregulatedRegulated;
        private readonly int _AnalysisYear;
        private readonly HydraulicDataset _hydraulicDataset;

        private double _minStageForArea;
        private double _maxStageForArea;
        private readonly ConvergenceCriteria convergenceCriteria;

        private readonly int _numExtrapolatedStagesToCompute = 7;
        private readonly int _numInterpolatedStagesToCompute = 2;

        private readonly string _HydraulicParentDirectory;
        private readonly PairedData _StageFrequency;
        #endregion

        #region Properties 
        public Inventory Inventory { get; }
        public int ImpactAreaID { get; }
        public List<ValidationGroup> ValidationGroups { get; } = new List<ValidationGroup>();
        public event ProgressReportedEventHandler ProgressReport;
        #endregion

        #region Constructor
        public ImpactAreaStageDamage(int impactAreaID, Inventory inventory, HydraulicDataset hydraulicDataset, ConvergenceCriteria convergence, string hydroParentDirectory, int analysisYear = 9999,
            ContinuousDistribution analyticalFlowFrequency = null, GraphicalUncertainPairedData graphicalFrequency = null, UncertainPairedData dischargeStage = null, UncertainPairedData unregulatedRegulated = null,
            bool usingMockData = false)
        {
            //TODO: Validate provided functions here
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
            _hydraulicDataset = hydraulicDataset;
            convergenceCriteria = convergence;
            SetMinAndMaxStage();
            _StageFrequency = CreateStageFrequency();
            AddRules();

            ValidationGroup vg = new("Impact area stage damage with impact area id '" + ImpactAreaID + "' has the following errors:");
            vg.ChildGroups.AddRange(Inventory.ValidationGroups);
            ValidationGroups.Add(vg);
        }
        #endregion

        #region Methods
        private void AddRules()
        {
            AddSinglePropertyRule(nameof(Inventory), new Rule(() => { Inventory.Validate(); return !Inventory.HasErrors; }, $"The structure inventory has errors: " + Inventory.GetErrors().ToString(), Inventory.ErrorLevel));
            AddSinglePropertyRule(nameof(convergenceCriteria), new Rule(() => { convergenceCriteria.Validate(); return !convergenceCriteria.HasErrors; }, $"Convergence criteria has errors: " + convergenceCriteria.GetErrors().ToString(), convergenceCriteria.ErrorLevel));
            AddSinglePropertyRule(nameof(_StageFrequency), new Rule(() => _StageFrequency != null, $"The software was unable to calculate stage-frequency for the impact area with ID {ImpactAreaID}", ErrorLevel.Fatal));
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
                    ErrorMessage errorMessage = new(message, ErrorLevel.Fatal);
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
                    _maxStageForArea = maxStages.Yvals[^1];
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
                        
                        _minStageForArea = minStages.f(minFlow);
                        _maxStageForArea = maxStages.f(maxFlow);
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
                    if(_UnregulatedRegulated != null)
                    {
                        flowFrequencyPairedData = _UnregulatedRegulated.SamplePairedData(0.5, true).compose(flowFrequencyPairedData) as PairedData;
                    }
                    return  _DischargeStage.SamplePairedData(0.5, true).compose(flowFrequencyPairedData) as PairedData;
                }
            }
            else if (_GraphicalFrequency != null)
            {
                if (_GraphicalFrequency.UsesStagesNotFlows)
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

        public List<UncertainPairedData> Compute(IProvideRandomNumbers randomProvider)
        {
            Validate();
            List<UncertainPairedData> results = new();
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
                (List<double>, List<float[]>) wsesAtEachStructureByProfile = _hydraulicDataset.GetHydraulicDatasetInFloatsWithProbabilities(Inventory, _HydraulicParentDirectory);

                //Run the compute by dam cat to simplify data collection 
                foreach (string damageCategory in damCats)
                {
                    //These are the stages of the stage-damage function - the aggregation stages 
                    List<double> allStagesAtIndexLocation = new();

                    //There will be one ConsequenceDistributionResults object for each stage in the stage-damage function
                    //Each ConsequenceDistributionResults object holds a ConsequenceDistributionResult for each asset cat
                    List<ConsequenceDistributionResults> consequenceDistributionResults = new();

                    (Inventory, List<float[]>) inventoryAndWaterTupled = Inventory.GetInventoryAndWaterTrimmedToDamageCategory(damageCategory, wsesAtEachStructureByProfile.Item2);

                    //Run a first pass to generate sufficient sample size to generate good histograms
                    bool isFirstPass = true;
                    ComputeDamageWithUncertaintyAllCoordinates(ref consequenceDistributionResults, ref allStagesAtIndexLocation, damageCategory, randomProvider, inventoryAndWaterTupled, wsesAtEachStructureByProfile.Item1, isFirstPass);
                    bool functionIsNotConverged = IsTheFunctionNotConverged(consequenceDistributionResults);

                    isFirstPass = false;

                    while (functionIsNotConverged)
                    {
                        ComputeDamageWithUncertaintyAllCoordinates(ref consequenceDistributionResults, ref allStagesAtIndexLocation, damageCategory, randomProvider, inventoryAndWaterTupled, wsesAtEachStructureByProfile.Item1, isFirstPass);
                        functionIsNotConverged = IsTheFunctionNotConverged(consequenceDistributionResults);
                    }
                    //there should be four UncertainPairedData objects - one for each asset cat of the given dam cat level compute 
                    List<UncertainPairedData> tempResultsList = ConsequenceDistributionResults.ToUncertainPairedData(allStagesAtIndexLocation, consequenceDistributionResults, ImpactAreaID);
                    results.AddRange(tempResultsList);
                }
                return results;
            }
        }

        private void ComputeDamageWithUncertaintyAllCoordinates(ref List<ConsequenceDistributionResults> consequenceDistributionResults, ref List<double> allStagesAtIndexLocation, string damageCategory, IProvideRandomNumbers randomProvider, (Inventory, List<float[]>) inventoryAndWaterTupled, List<double> profileProbabilities, bool isFirstPass)
        {
            //For the first pass, we collect the results in dictionaries where the key is the string asset category 
            //after the first pass, we take the data in the dictionaries, pass the data into a histogram within the consequence distribution results, and test for convergence 
            //if the histograms are not converged, then we proceed for additional passes, this time adding osbervations to the histograms directly 
            List<Dictionary<string, List<double>>> assetCatDamagesAllCoordinates = new();
            int iterations = convergenceCriteria.MinIterations;
            bool dictionariesAreNotConstructed = true;
            for (int i = 0; i < iterations; i++)
            {
                List<DeterministicOccupancyType> deterministicOccTypes = Inventory.SampleOccupancyTypes(randomProvider);
                ComputeLowerStageDamage(ref assetCatDamagesAllCoordinates, ref allStagesAtIndexLocation, ref consequenceDistributionResults, damageCategory,  deterministicOccTypes, inventoryAndWaterTupled, profileProbabilities, isFirstPass, dictionariesAreNotConstructed);
                ComputeMiddleStageDamage(ref assetCatDamagesAllCoordinates, ref allStagesAtIndexLocation, ref consequenceDistributionResults, damageCategory,  deterministicOccTypes, inventoryAndWaterTupled, profileProbabilities, isFirstPass, dictionariesAreNotConstructed);
                ComputeUpperStageDamage(ref assetCatDamagesAllCoordinates, ref allStagesAtIndexLocation, ref consequenceDistributionResults, damageCategory,  deterministicOccTypes, inventoryAndWaterTupled, profileProbabilities, isFirstPass, dictionariesAreNotConstructed);
                dictionariesAreNotConstructed = false;
            }
            if (isFirstPass)
            {
                TransformDictionaryIntoConsequenceDistributionResults(ref consequenceDistributionResults, ref assetCatDamagesAllCoordinates, damageCategory);
            }
        }
        /// <summary>
        /// This method is used to transform the data found within the first pass data collection dictionaries into consequence distribution results
        /// the dictionaries are cleared out after the transformation takes place 
        /// </summary>
        private void TransformDictionaryIntoConsequenceDistributionResults(ref List<ConsequenceDistributionResults> consequenceDistributionResults, ref List<Dictionary<string, List<double>>> assetCatDamagesAllCoordinates, string damageCategory)
        {
            foreach (Dictionary<string, List<double>> dictionaryOfDamagesByAssetCategory in assetCatDamagesAllCoordinates)
            {
                ConsequenceDistributionResult structureConsequenceDistributionResult = new(damageCategory, StringConstants.STRUCTURE_ASSET_CATEGORY, convergenceCriteria, dictionaryOfDamagesByAssetCategory[StringConstants.STRUCTURE_ASSET_CATEGORY], ImpactAreaID);
                ConsequenceDistributionResult contentConsequenceDistributionResult = new(damageCategory, StringConstants.CONTENT_ASSET_CATEGORY, convergenceCriteria, dictionaryOfDamagesByAssetCategory[StringConstants.CONTENT_ASSET_CATEGORY], ImpactAreaID);
                ConsequenceDistributionResult vehicleConsequenceDistributionResult = new(damageCategory, StringConstants.VEHICLE_ASSET_CATEGORY, convergenceCriteria, dictionaryOfDamagesByAssetCategory[StringConstants.VEHICLE_ASSET_CATEGORY], ImpactAreaID);
                ConsequenceDistributionResult otherConsequenceDistributionResult = new(damageCategory, StringConstants.OTHER_ASSET_CATEGORY, convergenceCriteria, dictionaryOfDamagesByAssetCategory[StringConstants.OTHER_ASSET_CATEGORY], ImpactAreaID);
                List<ConsequenceDistributionResult> consequenceDistributionResultList = new() { structureConsequenceDistributionResult, contentConsequenceDistributionResult, vehicleConsequenceDistributionResult, otherConsequenceDistributionResult };
                ConsequenceDistributionResults consequenceDistResultsAtThisStage = new(consequenceDistributionResultList);
                consequenceDistributionResults.Add(consequenceDistResultsAtThisStage);
            }
            assetCatDamagesAllCoordinates.Clear();
        }

        private static bool IsTheFunctionNotConverged(List<ConsequenceDistributionResults> consequenceDistributionResults)
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
        private void ComputeLowerStageDamage(ref List<Dictionary<string, List<double>>> assetCatDamagesAllCoordinates, ref List<double> allStagesAtIndexLocation,
            ref List<ConsequenceDistributionResults> consequenceDistributionResults, string damageCategory,
            List<DeterministicOccupancyType> deterministicOccTypes, (Inventory, List<float[]>) inventoryAndWaterCoupled, List<double> profileProbabilities,
            bool isFirstPass, bool dictionariesAreNotConstructed)
        {
            //the probability of a profile is an EXCEEDANCE probability but in the model we use NONEXCEEDANCE PROBABILITY
            double stageAtProbabilityOfLowestProfile = _StageFrequency.f(1 - profileProbabilities.Max());
            //the delta is the difference between the min stage at the index location and the stage at the index location for the lowest profile 
            float indexStationLowerStageDelta = (float)(stageAtProbabilityOfLowestProfile - _minStageForArea);
            //this interval defines the interval in stages by which we'll compute damage 
            float interval = indexStationLowerStageDelta / _numExtrapolatedStagesToCompute;
            //Collect damage for first part of function up to and including the stages at the lowest profile 
            
            for (int i = 0; i < _numExtrapolatedStagesToCompute + 1; i++)
            {
                float[] WSEsParallelToIndexLocation = ExtrapolateFromBelowStagesAtIndexLocation(inventoryAndWaterCoupled.Item2[0], interval, i, _numExtrapolatedStagesToCompute);
                //this inventory is not trimmed 
                ConsequenceResult consequenceResult = inventoryAndWaterCoupled.Item1.ComputeDamages(WSEsParallelToIndexLocation, _AnalysisYear, damageCategory, deterministicOccTypes);
                if (isFirstPass)
                {
                    if (dictionariesAreNotConstructed)
                    {
                        Dictionary<string, List<double>> damages = ConvertConsequenceResultToDictionary(consequenceResult);
                        assetCatDamagesAllCoordinates.Add(damages);
                        allStagesAtIndexLocation.Add(_minStageForArea + i * interval);
                    }
                    else
                    {
                        AggregateResultsByDictionary(ref assetCatDamagesAllCoordinates, consequenceResult, i);
                    }
                }
                else
                {
                    consequenceDistributionResults[i].AddConsequenceRealization(consequenceResult, damageCategory, ImpactAreaID, i);
                }

            }
        }

        private static void AggregateResultsByDictionary(ref List<Dictionary<string, List<double>>> assetCatDamagesAllCoordinates, ConsequenceResult consequenceResult, int i)
        {
            assetCatDamagesAllCoordinates[i][StringConstants.STRUCTURE_ASSET_CATEGORY].Add(consequenceResult.StructureDamage);
            assetCatDamagesAllCoordinates[i][StringConstants.CONTENT_ASSET_CATEGORY].Add(consequenceResult.ContentDamage);
            assetCatDamagesAllCoordinates[i][StringConstants.VEHICLE_ASSET_CATEGORY].Add(consequenceResult.VehicleDamage);
            assetCatDamagesAllCoordinates[i][StringConstants.OTHER_ASSET_CATEGORY].Add(consequenceResult.OtherDamage);
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
        private static Dictionary<string, List<double>> ConvertConsequenceResultToDictionary(ConsequenceResult consequenceResult)
        {
            //there will be four dictionary entries for each stage
            //one dictionary entry for each asset category 
            Dictionary<string, List<double>> damages = new()
            {
                { StringConstants.STRUCTURE_ASSET_CATEGORY, new List<double>() { consequenceResult.StructureDamage } },
                { StringConstants.CONTENT_ASSET_CATEGORY, new List<double>() { consequenceResult.ContentDamage } },
                { StringConstants.VEHICLE_ASSET_CATEGORY, new List<double>() { consequenceResult.VehicleDamage } },
                { StringConstants.OTHER_ASSET_CATEGORY, new List<double>() { consequenceResult.OtherDamage } }
            };
            return damages;
        }
        /// <summary>
        /// This method calculates a stage damage function within the hydraulic profiles 
        /// </summary>
        private void ComputeMiddleStageDamage(ref List<Dictionary<string, List<double>>> assetCatDamagesAllCoordinates, ref List<double> allStagesAtIndexLocation,
            ref List<ConsequenceDistributionResults> consequenceDistributionResults, string damageCategory, List<DeterministicOccupancyType> deterministicOccTypes,
            (Inventory, List<float[]>) inventoryAndWaterCoupled, List<double> profileProbabilities, bool isFirstPass, bool dictionariesAreNotConstructed)
        {
            int numProfiles = profileProbabilities.Count;
            for (int i = 1; i < numProfiles; i++)
            {
                InterpolateBetweenProfiles(deterministicOccTypes, inventoryAndWaterCoupled.Item2[i - 1], profileProbabilities[i - 1], inventoryAndWaterCoupled.Item2[i], profileProbabilities[i], damageCategory, ref allStagesAtIndexLocation, ref assetCatDamagesAllCoordinates, isFirstPass, ref consequenceDistributionResults, dictionariesAreNotConstructed, i, inventoryAndWaterCoupled.Item1);
            }
        }
        //TODO: Why are there arguments here that appear to go unused? 
        private void InterpolateBetweenProfiles(List<DeterministicOccupancyType> occTypes,  float[] previousHydraulicProfile, double previousProbability, float[] currentHydraulicProfile, 
            double currentProbability, string damageCategory, ref List<double> allStagesAtIndexLocation, ref List<Dictionary<string,
                List<double>>> assetCatDamagesAllCoordinates, bool isFirstPass, ref List<ConsequenceDistributionResults> consequenceDistributionResults, bool dictionariesAreNotConstructed,
            int profileCount, Inventory inventory)
        {
            double previousStageAtIndexLocation = _StageFrequency.f(1 - previousProbability);
            double currentStageAtIndexLocation = _StageFrequency.f(1 - currentProbability);
            double stageDeltaAtIndexLocation = currentStageAtIndexLocation - previousStageAtIndexLocation;
            double intervalAtIndexLocation = stageDeltaAtIndexLocation / _numInterpolatedStagesToCompute;

            float[] intervalsAtStructures = CalculateIntervals(previousHydraulicProfile, currentHydraulicProfile);

            for (int i = 0; i < _numInterpolatedStagesToCompute; i++)
            {
                float[] stages = CalculateIncrementOfStages(previousHydraulicProfile, intervalsAtStructures, i+1);
                //what are the stages here for the first coordinate?
                ConsequenceResult consequenceResult = inventory.ComputeDamages(stages, _AnalysisYear, damageCategory, occTypes);
                if (isFirstPass)
                {
                    if (dictionariesAreNotConstructed)
                    {
                        double stageAtIndexLocation = previousStageAtIndexLocation + intervalAtIndexLocation * (i + 1);
                        Dictionary<string, List<double>> damages = ConvertConsequenceResultToDictionary(consequenceResult);
                        assetCatDamagesAllCoordinates.Add(damages);
                        allStagesAtIndexLocation.Add(stageAtIndexLocation);
                    }
                    else
                    {
                        AggregateResultsByDictionary(ref assetCatDamagesAllCoordinates, consequenceResult, i + _numExtrapolatedStagesToCompute + profileCount*_numInterpolatedStagesToCompute - 1);

                    }

                }
                else
                {
                    consequenceDistributionResults[i + _numExtrapolatedStagesToCompute + profileCount*_numInterpolatedStagesToCompute - 1].AddConsequenceRealization(consequenceResult, damageCategory, ImpactAreaID, i);
                }
            }
        }

        private static float[] CalculateIncrementOfStages(float[] previousStagesAtStructures, float[] intervalsAtStructures, int i)
        {
            float[] stages = new float[intervalsAtStructures.Length];
            for (int m = 0; m < stages.Length; m++)
            {
                stages[m] = previousStagesAtStructures[m] + intervalsAtStructures[m] * i;
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
        private void ComputeUpperStageDamage(ref List<Dictionary<string, List<double>>> assetCatDamagesAllCoordinates, ref List<double> allStagesAtIndexLocation, 
            ref List<ConsequenceDistributionResults> consequenceDistributionResults, string damageCategory, List<DeterministicOccupancyType> deterministicOccTypes, 
            (Inventory, List<float[]>) inventoryAndWaterCoupled, List<double> profileProbabilities, bool isFirstPass, bool dictionariesAreNotConstructed)
        {
            //the probability of a profile is an EXCEEDANCE probability but in the model we use NONEXCEEDANCE PROBABILITY
            double stageAtProbabilityOfHighestProfile = _StageFrequency.f(1 - profileProbabilities.Min());
            float indexStationUpperStageDelta = (float)(_maxStageForArea - stageAtProbabilityOfHighestProfile);
            float upperInterval = indexStationUpperStageDelta / _numExtrapolatedStagesToCompute;
            for (int i = 1; i < _numExtrapolatedStagesToCompute; i++)
            {
                float[] WSEsParallelToIndexLocation = ExtrapolateFromAboveAtIndexLocation(inventoryAndWaterCoupled.Item2[^1], upperInterval, i);
                ConsequenceResult consequenceResult = inventoryAndWaterCoupled.Item1.ComputeDamages(WSEsParallelToIndexLocation, _AnalysisYear, damageCategory, deterministicOccTypes);
                if (isFirstPass)
                {
                    if (dictionariesAreNotConstructed)
                    {
                        Dictionary<string, List<double>> damages = ConvertConsequenceResultToDictionary(consequenceResult);
                        assetCatDamagesAllCoordinates.Add(damages);
                        allStagesAtIndexLocation.Add(_maxStageForArea - upperInterval * (_numExtrapolatedStagesToCompute - i));
                    }
                    else
                    {
                        AggregateResultsByDictionary(ref assetCatDamagesAllCoordinates, consequenceResult, i + _numExtrapolatedStagesToCompute + _numInterpolatedStagesToCompute * (_hydraulicDataset.HydraulicProfiles.Count-1));

                    }

                }
                else
                {
                    consequenceDistributionResults[i + _numExtrapolatedStagesToCompute + _numInterpolatedStagesToCompute*(_hydraulicDataset.HydraulicProfiles.Count-1)].AddConsequenceRealization(consequenceResult, damageCategory, ImpactAreaID, i);
                }
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
            List<string> structureDetails = Inventory.StructureDetails();
            List<DeterministicOccupancyType> deterministicOccupancyTypes = Inventory.SampleOccupancyTypes(new compute.MedianRandomProvider());
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
                float[] stagesAtStructures = hydraulicProfile.GetWSE(Inventory.GetPointMs(), _hydraulicDataset.DataSource, _HydraulicParentDirectory);
                //first, create the header with the probability information on the hydraulic profile 
                //that will go in structureDetails[0]

                structureDetails[0] += $"{assetType} Damage At {hydraulicProfile.Probability}AEP,";
                List<ConsequenceResult> consequenceResultList = new();

                for (int i = 0; i < stagesAtStructures.Length; i++)
                {
                    ConsequenceResult consequenceResult = Inventory.Structures[i].ComputeDamage(stagesAtStructures[i], deterministicOccupancyType);
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
                float[] stagesAtStructures = hydraulicProfile.GetWSE(Inventory.GetPointMs(), _hydraulicDataset.DataSource, _HydraulicParentDirectory);
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
            foreach (IHydraulicProfile hydraulicProfile in _hydraulicDataset.HydraulicProfiles)
            {
                float[] stagesAtStructures = hydraulicProfile.GetWSE(Inventory.GetPointMs(), _hydraulicDataset.DataSource, _HydraulicParentDirectory);
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
