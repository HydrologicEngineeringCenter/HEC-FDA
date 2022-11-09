using Statistics;
using System;
using System.Collections.Generic;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Model.Messaging;
using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.structures;
using HEC.FDA.Model.interfaces;
using RasMapperLib;

namespace HEC.FDA.Model.stageDamage
{
    public class ImpactAreaStageDamage : Validation, IReportMessage
    {
        #region Fields 
        private const double MIN_PROBABILITY = 0.0001;
        private const double MAX_PROBABILITY = 0.9999;
        private ContinuousDistribution _AnalyticalFlowFrequency;
        private GraphicalUncertainPairedData _GraphicalFrequency;
        private UncertainPairedData _DischargeStage;
        private int _ImpactAreaID;

        private Inventory _inventory;
        private HydraulicDataset _hydraulicDataset;

        private double _minStageForArea;
        private double _maxStageForArea;
        private ConvergenceCriteria convergenceCriteria;

        private int seed = 1234;
        private int _numInterpolatedStagesToCompute = 10;
        private int _numExtrapolatedStagesToCompute = 50;

        private string _HydraulicParentDirectory;
        #endregion

        #region Properties 
        public event MessageReportedEventHandler MessageReport;
        #endregion
        #region Constructor
        public ImpactAreaStageDamage(int impactAreaID, Inventory inventory, HydraulicDataset hydraulicDataset, ConvergenceCriteria convergence, string hydroParentDirectory,
            ContinuousDistribution analyticalFlowFrequency = null, GraphicalUncertainPairedData graphicalFrequency = null, UncertainPairedData dischargeStage = null)
        {
            //TODO: Validate provided functions here
            _HydraulicParentDirectory = hydroParentDirectory;
            _AnalyticalFlowFrequency = analyticalFlowFrequency;
            _GraphicalFrequency = graphicalFrequency;
            _DischargeStage = dischargeStage;
            _ImpactAreaID = impactAreaID;
            _inventory = inventory.GetInventoryTrimmmedToPolygon(impactAreaID);
            _inventory = inventory;
            _hydraulicDataset = hydraulicDataset;
            convergenceCriteria = convergence;
            SetMinAndMaxStage();
        }
        #endregion

        #region Methods
        private void SetMinAndMaxStage()
        {
            if (_AnalyticalFlowFrequency != null)
            {
                if (_DischargeStage != null)
                {
                    double minFLow = _AnalyticalFlowFrequency.InverseCDF(MIN_PROBABILITY);
                    IPairedData minStagesOnRating = _DischargeStage.SamplePairedData(MIN_PROBABILITY);
                    _minStageForArea = minStagesOnRating.f(minFLow);

                    double maxFLow = _AnalyticalFlowFrequency.InverseCDF(MAX_PROBABILITY);
                    IPairedData maxStagesOnRating = _DischargeStage.SamplePairedData(MAX_PROBABILITY);
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
                        IPairedData minStages = _DischargeStage.SamplePairedData(MIN_PROBABILITY);
                        _minStageForArea = minStages.f(minFlow);
                        IPairedData maxFlows = _GraphicalFrequency.SamplePairedData(MAX_PROBABILITY);
                        double maxFlow = maxFlows.Yvals[maxFlows.Yvals.Length - 1];
                        IPairedData maxStages = _DischargeStage.SamplePairedData(MAX_PROBABILITY);
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
        private PairedData CreateStageFrequency()
        {
            PairedData stageFrequency;
            if (_AnalyticalFlowFrequency != null)
            {
                if (_DischargeStage != null)
                {
                    Tuple<double[], double[]> flowFreqAsTuple = _AnalyticalFlowFrequency.ToCoordinates();
                    PairedData flowFrequencyPairedData = new PairedData(flowFreqAsTuple.Item1, flowFreqAsTuple.Item2);
                    return stageFrequency = _DischargeStage.SamplePairedData(0.5).compose(flowFrequencyPairedData) as PairedData;
                }
            }
            else if (_GraphicalFrequency != null)
            {
                if (_GraphicalFrequency.UsesStagesNotFlows)
                {
                    return stageFrequency = _GraphicalFrequency.SamplePairedData(0.5) as PairedData;
                }
                else
                {
                    if (_DischargeStage != null)
                    {
                        PairedData flowFrequencyPairedData = _GraphicalFrequency.SamplePairedData(0.5) as PairedData;
                        return stageFrequency = _DischargeStage.SamplePairedData(0.5).compose(flowFrequencyPairedData) as PairedData;
                    }
                }
            }
            return null;
        }
        private void ComputeLowerStageDamage(IProvideRandomNumbers randomProvider, PairedData stageFrequency, ref List<double> allStagesAtIndexLocation, 
            ref List<ConsequenceDistributionResults> consequenceDistributionResults)
        {
            //Part 1: Stages between min stage at index location and the stage at the index location for the lowest profile 
            PointMs pointMs = _inventory.GetPointMs();
            HydraulicProfile lowestProfile = _hydraulicDataset.HydraulicProfiles[0];
            float[] WSEAtLowest = lowestProfile.GetWSE(pointMs, _hydraulicDataset.DataSource, _HydraulicParentDirectory);
            HydraulicProfile nextProfile = _hydraulicDataset.HydraulicProfiles[1];
            float[] WSEAtNext = nextProfile.GetWSE(pointMs, _hydraulicDataset.DataSource, _HydraulicParentDirectory);
            HydraulicDataset.CorrectDryStructureWSEs(ref WSEAtLowest, _inventory.GroundElevations, WSEAtNext );
            //the probability of a profile is an EXCEEDANCE probability but in the model we use NONEXCEEDANCE PROBABILITY
            double stageAtProbabilityOfLowestProfile = stageFrequency.f(1-lowestProfile.Probability);
            //the delta is the difference between the min stage at the index location and the stage at the index location for the lowest profile 
            float indexStationLowerStageDelta = (float)(stageAtProbabilityOfLowestProfile - _minStageForArea);
            //this interval defines the interval in stages by which we'll compute damage 
            float interval = indexStationLowerStageDelta / _numExtrapolatedStagesToCompute;
            //Collect damage for first part of function up to and including the stages at the lowest profile 
            for (int i = 0; i < _numExtrapolatedStagesToCompute + 1; i++)
            {
                float[] WSEsParallelToIndexLocation = ExtrapolateFromBelowStagesAtIndexLocation(WSEAtLowest, interval, i);
                ConsequenceDistributionResults damageOrdinate = ComputeDamageOneCoordinate(randomProvider, convergenceCriteria, _inventory, WSEsParallelToIndexLocation);
                consequenceDistributionResults.Add(damageOrdinate);
                allStagesAtIndexLocation.Add(_minStageForArea + i * interval);
            }


        }
        private void ComputeMiddleStageDamage(IProvideRandomNumbers randomProvider, PairedData stageFrequency, ref List<double> allStagesAtIndexLocation, ref List<ConsequenceDistributionResults> consequenceDistributionResults)
        {
            //Part 2: Stages between the lowest profile and highest profile, excluding the lowest profile 
            int numProfiles = _hydraulicDataset.HydraulicProfiles.Count;
            for (int i = 1; i < numProfiles; i++)
            {
                HydraulicProfile previousHydraulicProfile = _hydraulicDataset.HydraulicProfiles[i-1];
                HydraulicProfile currentHydraulicProfile = _hydraulicDataset.HydraulicProfiles[i];
                HydraulicProfile nextHydraulicProfile = null;
                if(i < numProfiles - 1) //if we're on the highest profile
                {
                    nextHydraulicProfile = _hydraulicDataset.HydraulicProfiles[i+1];
                }
                InterpolateBetweenProfiles(randomProvider, previousHydraulicProfile, currentHydraulicProfile, nextHydraulicProfile, stageFrequency, ref allStagesAtIndexLocation, ref consequenceDistributionResults);
            }

        }

        private void InterpolateBetweenProfiles(IProvideRandomNumbers randomProvider, HydraulicProfile previousHydraulicProfile, HydraulicProfile currentHydraulicProfile, HydraulicProfile nextHydraulicProfile, PairedData stageFrequency, ref List<double> allStagesAtIndexLocation, ref List<ConsequenceDistributionResults> consequenceDistributionResults)
        {
            double previousStageAtIndexLocation = stageFrequency.f(1 - previousHydraulicProfile.Probability);
            double currentStageAtIndexLocation = stageFrequency.f(1 - currentHydraulicProfile.Probability);
            double stageDeltaAtIndexLocation = currentStageAtIndexLocation - previousStageAtIndexLocation;
            double intervalAtIndexLocation = stageDeltaAtIndexLocation / _numInterpolatedStagesToCompute;

            PointMs pointMs = _inventory.GetPointMs();
            float[] previousStagesAtStructures = previousHydraulicProfile.GetWSE(pointMs, _hydraulicDataset.DataSource, _HydraulicParentDirectory);
            float[] currentStagesAtStructures = currentHydraulicProfile.GetWSE(pointMs, _hydraulicDataset.DataSource, _HydraulicParentDirectory);
            float[] nextStagesAtStructures = nextHydraulicProfile.GetWSE(pointMs,_hydraulicDataset.DataSource,_HydraulicParentDirectory);

            HydraulicDataset.CorrectDryStructureWSEs(ref previousStagesAtStructures, _inventory.GroundElevations, currentStagesAtStructures);
            HydraulicDataset.CorrectDryStructureWSEs(ref currentStagesAtStructures, _inventory.GroundElevations, nextStagesAtStructures);

            float[] intervalsAtStructures = CalculateIntervals(previousStagesAtStructures, currentStagesAtStructures);

            for (int i = 1; i < _numInterpolatedStagesToCompute; i++)
            {
                double stageAtIndexLocation = previousStageAtIndexLocation + intervalAtIndexLocation * i;
                float[] stages = CalculateIncrementOfStages(previousStagesAtStructures, intervalsAtStructures, i);
                ConsequenceDistributionResults damageOrdinate = ComputeDamageOneCoordinate(randomProvider, convergenceCriteria, _inventory, stages);
                consequenceDistributionResults.Add(damageOrdinate);
                allStagesAtIndexLocation.Add(stageAtIndexLocation);
            }
        }

        private float[] CalculateIncrementOfStages(float[] previousStagesAtStructures, float[] intervalsAtStructures, int i)
        {
            float[] stages = new float[intervalsAtStructures.Length];
            for (int m = 0; m < stages.Length; m++)
            {
                stages[m] = previousStagesAtStructures[m] + intervalsAtStructures[m]*i;
            }
            return stages;
        }

        private float[] CalculateIntervals(float[] previousStagesAtStructures, float[] currentStagesAtStructures)
        {
           float[] intervals = new float[previousStagesAtStructures.Length];
            for (int j = 0; j < previousStagesAtStructures.Length; j++)
            {
                intervals[j] = (currentStagesAtStructures[j] - previousStagesAtStructures[j])/_numInterpolatedStagesToCompute;
            }
            return intervals;
        }

        private void ComputeUpperStageDamage(IProvideRandomNumbers randomProvider, PairedData stageFrequency, ref List<double> allStagesAtIndexLocation, ref List<ConsequenceDistributionResults> consequenceDistributionResults)
        {
            //Part 3: Stages between the highest profile 
            List<HydraulicProfile> profileList = _hydraulicDataset.HydraulicProfiles;
            float[] stagesAtStructuresHighestProfile = profileList[profileList.Count - 1].GetWSE(_inventory.GetPointMs(), _hydraulicDataset.DataSource, _HydraulicParentDirectory);
            HydraulicDataset.CorrectDryStructureWSEs(ref stagesAtStructuresHighestProfile, _inventory.GroundElevations);
            double stageAtProbabilityOfHighestProfile = stageFrequency.f(1-profileList[profileList.Count - 1].Probability);
            float indexStationUpperStageDelta = (float)(_maxStageForArea - stageAtProbabilityOfHighestProfile);
            float upperInterval = indexStationUpperStageDelta / _numExtrapolatedStagesToCompute;
            for (int stepCount = 1; stepCount < _numExtrapolatedStagesToCompute; stepCount++)
            {
                float[] WSEsParallelToIndexLocation = ExtrapolateFromAboveAtIndexLocation(stagesAtStructuresHighestProfile, upperInterval, stepCount);
                ConsequenceDistributionResults damageOrdinate = ComputeDamageOneCoordinate(randomProvider, convergenceCriteria, _inventory, WSEsParallelToIndexLocation);
                consequenceDistributionResults.Add(damageOrdinate);
                allStagesAtIndexLocation.Add(_maxStageForArea - upperInterval * (_numExtrapolatedStagesToCompute - stepCount));
            }
        }
        public List<UncertainPairedData> Compute(IProvideRandomNumbers randomProvider)
        {
            List<double> allStagesAtIndexLocation = new List<double>();
            List<ConsequenceDistributionResults> consequenceDistributionResults = new List<ConsequenceDistributionResults>();
            PairedData stageFrequency = CreateStageFrequency();

            ComputeLowerStageDamage(randomProvider, stageFrequency, ref allStagesAtIndexLocation, ref consequenceDistributionResults);
            ComputeMiddleStageDamage(randomProvider, stageFrequency, ref allStagesAtIndexLocation, ref consequenceDistributionResults);
            ComputeUpperStageDamage(randomProvider, stageFrequency, ref allStagesAtIndexLocation, ref consequenceDistributionResults);

            List<UncertainPairedData> results = ConsequenceDistributionResults.ToUncertainPairedData(allStagesAtIndexLocation, consequenceDistributionResults);
            return results;
        }
        public float[] ExtrapolateFromAboveAtIndexLocation(float[] stagesAtStructuresHighestProfile, float upperInterval, int stepCount)
        {
            float[] extrapolatedStages = new float[stagesAtStructuresHighestProfile.Length];
            foreach (float structureStage in stagesAtStructuresHighestProfile)
            {
                extrapolatedStages[stepCount] = structureStage + upperInterval * stepCount;
            }
            return extrapolatedStages;
        }
        public float[] ExtrapolateFromBelowStagesAtIndexLocation(float[] WSEsAtLowest, float interval, int i)
        {
            float[] extrapolatedStages = new float[WSEsAtLowest.Length];
            foreach (float stage in WSEsAtLowest)
            {
                extrapolatedStages[i] = stage - interval * (_numInterpolatedStagesToCompute - i);
            }
            return extrapolatedStages;
        }
        //public and static for testing
        //assume that the inventory has already been trimmed 
        public static ConsequenceDistributionResults ComputeDamageOneCoordinate(IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, Inventory inventory, float[] wses)
        {
            double lowerProb = 0.025;
            double upperProb = .975;
            ConsequenceDistributionResults consequenceDistributionResults = new ConsequenceDistributionResults(convergenceCriteria);
            long iteration = 0;
            while (consequenceDistributionResults.ResultsAreConverged(upperProb, lowerProb))
            {
                DeterministicInventory deterministicInventory = inventory.Sample(randomProvider);
                ConsequenceResults consequenceResults = deterministicInventory.ComputeDamages(wses);
                consequenceDistributionResults.AddConsequenceRealization(consequenceResults, iteration);
                iteration++;
            }
            return consequenceDistributionResults;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        #endregion
    }
}
