using paireddata;
using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using structures;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Enumerations;
using interfaces;
using System.Xml.Linq;
using HEC.MVVMFramework.Model.Messaging;
using fda_model.hydraulics;
using metrics;
using compute;

namespace stageDamage
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
        private List<OccupancyType> _occupancyTypes;
        private HydraulicDataset _hydraulicDataset;


        #endregion

        #region Properties 
        public event MessageReportedEventHandler MessageReport;
        #endregion
        #region Constructor
        public ImpactAreaStageDamage(int impactAreaID, Inventory inventory, List<OccupancyType> occupancyTypes, HydraulicDataset hydraulicDataset,
            ContinuousDistribution analyticalFlowFrequency = null, GraphicalUncertainPairedData graphicalFrequency = null, UncertainPairedData dischargeStage = null)
        {
            //TODO: Validate provided functions here
            _AnalyticalFlowFrequency = analyticalFlowFrequency;
            _GraphicalFrequency = graphicalFrequency;
            _DischargeStage = dischargeStage;
            _ImpactAreaID = impactAreaID;
            _inventory = inventory;
            _hydraulicDataset = hydraulicDataset;
            _occupancyTypes = occupancyTypes;
        }
        #endregion

        #region Methods
        //TODO: This compute produces the uncertain paired data
        //That means that we need to have all hydraulic profiles available 
        //The list of stages to me appears to mean that we won't 
        public List<UncertainPairedData> Compute(RandomProvider randomProvider, ConvergenceCriteria convergenceCriteria)
        {
            int seed = 1234;

            //the list of stages makes up the x values of the stage damage UPD
            List<double> allStagesAtIndexLocation = new List<double>();
            //the list of consequence distribution results will be paired with the list of stages to produce a list of UPD
            List<UncertainPairedData> results = new List<UncertainPairedData>();
            List <ConsequenceDistributionResults> consequenceDistributionResults = new List<ConsequenceDistributionResults>();

            //Find the min stage and max stage for the impact area index location 
            double minStage;
            double maxStage;
            //TODO we need a stage-frequency function to reference for the compute below. 

            PairedData stageFrequency;

            if(_AnalyticalFlowFrequency != null)
            {
                if (_DischargeStage != null)
                {
                    double minFLow = _AnalyticalFlowFrequency.InverseCDF(MIN_PROBABILITY);
                    IPairedData minStagesOnRating = _DischargeStage.SamplePairedData(MIN_PROBABILITY);
                    minStage = minStagesOnRating.f(minFLow);

                    double maxFLow = _AnalyticalFlowFrequency.InverseCDF(MAX_PROBABILITY);
                    IPairedData maxStagesOnRating = _DischargeStage.SamplePairedData(MAX_PROBABILITY);
                    maxStage = maxStagesOnRating.f(maxFLow);

                    Tuple<double[],double[]>  flowFreqAsTuple = _AnalyticalFlowFrequency.ToCoordinates();
                    PairedData flowFrequencyPairedData = new PairedData(flowFreqAsTuple.Item1, flowFreqAsTuple.Item2);
                    stageFrequency = _DischargeStage.SamplePairedData(0.5).compose(flowFrequencyPairedData) as PairedData;
                }
                else
                {
                    string message = "A stage-discharge function must accompany a flow-frequency function but no such function was found. Stage-damage compute aborted" + Environment.NewLine;
                    ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
                    ReportMessage(this, new MessageEventArgs(errorMessage));
                    return results;
                }
            } else if (_GraphicalFrequency != null)
            {
                if(_GraphicalFrequency.UsesStagesNotFlows)
                {
                    IPairedData minStages = _GraphicalFrequency.SamplePairedData(MIN_PROBABILITY);
                    minStage = minStages.Yvals[0];
                    IPairedData maxStages = _GraphicalFrequency.SamplePairedData(MAX_PROBABILITY);
                    maxStage = maxStages.Yvals[maxStages.Yvals.Length - 1];
                    stageFrequency = _GraphicalFrequency.SamplePairedData(0.5) as PairedData;
                } else
                {
                    if (_DischargeStage != null)
                    {
                        PairedData flowFrequencyPairedData = _GraphicalFrequency.SamplePairedData(0.5) as PairedData;
                        stageFrequency = _DischargeStage.SamplePairedData(0.5).compose(flowFrequencyPairedData) as PairedData;

                        IPairedData minFlows = _GraphicalFrequency.SamplePairedData(MIN_PROBABILITY);
                        double minFlow = minFlows.Yvals[0];
                        IPairedData minStages = _DischargeStage.SamplePairedData(MIN_PROBABILITY);
                        minStage = minStages.f(minFlow);
                        IPairedData maxFlows = _GraphicalFrequency.SamplePairedData(MAX_PROBABILITY);
                        double maxFlow = maxFlows.Yvals[maxFlows.Yvals.Length - 1];
                        IPairedData maxStages = _DischargeStage.SamplePairedData(MAX_PROBABILITY);
                        maxStage = maxStages.f(maxFlow);
                    }
                    else
                    {
                        string message = "A stage-discharge function must accompany a flow-frequency function but no such function was found. Stage-damage compute aborted" + Environment.NewLine;
                        ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
                        ReportMessage(this, new MessageEventArgs(errorMessage));
                        return results;
                    }
                }
            }
            else
            {
                //use stages from hydarulics at index locations
                string message = "At this time, HEC-FDA does not allow a stage-damage compute without a frequency function. Stage-damage compute aborted" + Environment.NewLine;
                ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage));
                return results;
            }

            //TODO: Where do the results from the boundary profiles get computed?
            //that is, which step?


            //Hydraulic profiles will be sorted in descending exceedence probability (smallest profile is index 0) 
            List < HydraulicProfile > profileList = _hydraulicDataset.HydraulicProfiles;


            //Part 1: Stages between min stage at index location and the stage at the index location for the lowest profile 
            HydraulicProfile lowestProfile = profileList[0];
            float[] WSEAtLowest = lowestProfile.GetWSE(_inventory.GetPointMs());
            double stageAtProbabilityOfLowestProfile = stageFrequency.f(lowestProfile.Probability);
            //the delta is the difference between the min stage at the index location and the stage at the index location for the lowest profile 
            float indexStationLowerStageDelta = (float)(stageAtProbabilityOfLowestProfile - minStage); 
            int numIntermediateStagesToCompute = 15; //TODO: make this number meaningful.
            //this interval defines the interval in stages by which we'll compute damage 
            float interval = indexStationLowerStageDelta/ numIntermediateStagesToCompute;
            //Collect damage for first part of function 
            for(int i = 0; i < numIntermediateStagesToCompute; i++)
            {
                float[] WSEsParallelToIndexLocation = ExtrapolateFromBelowStagesAtIndexLocation(WSEAtLowest, interval, i, numIntermediateStagesToCompute);
                ConsequenceDistributionResults damageOrdinate = ComputeDamageOneCoordinate(seed, randomProvider, convergenceCriteria, _inventory, _occupancyTypes, _ImpactAreaID, WSEsParallelToIndexLocation);
                consequenceDistributionResults.Add(damageOrdinate);
                allStagesAtIndexLocation.Add(minStage + i * interval);
            }

            //Part 2: Stages between the lowest profile and highest profile 
            //I think we do the 8 profiles for now 
            //then figure out how to interpolate later 

            foreach (HydraulicProfile hydraulicProfile in profileList)
            {
                double stageAtIndexLocation = stageFrequency.f(hydraulicProfile.Probability);
                float[] stages = hydraulicProfile.GetWSE(_inventory.GetPointMs());
                ConsequenceDistributionResults damageOrdinate = ComputeDamageOneCoordinate(seed, randomProvider, convergenceCriteria, _inventory, _occupancyTypes, _ImpactAreaID, stages);
                consequenceDistributionResults.Add(damageOrdinate);
                allStagesAtIndexLocation.Add(stageAtIndexLocation);
            }

            //Part 3: Stages between the highest profile 
            float[] stagesAtHighestProfile = profileList[profileList.Count-1].GetWSE(_inventory.GetPointMs());
            double stageAtProbabilityOfHighestProfile = stageFrequency.f(profileList[profileList.Count-1].Probability);
            float indexStationUpperStageDelta = (float)(maxStage - stageAtProbabilityOfHighestProfile);
            float upperInterval = indexStationUpperStageDelta / numIntermediateStagesToCompute;

            for (int i = 0; i < numIntermediateStagesToCompute; i++)
            {
                float[] WSEsParallelToIndexLocation = ExtrapolateFromAboveAtIndexLocation(stagesAtHighestProfile, upperInterval, i, numIntermediateStagesToCompute);
                ConsequenceDistributionResults damageOrdinate = ComputeDamageOneCoordinate(seed, randomProvider, convergenceCriteria, _inventory, _occupancyTypes, i, WSEsParallelToIndexLocation);
                consequenceDistributionResults.Add(damageOrdinate);
                allStagesAtIndexLocation.Add(maxStage - upperInterval * (numIntermediateStagesToCompute - i));
            }
            results = ConsequenceDistributionResults.ToUncertainPairedData(allStagesAtIndexLocation, consequenceDistributionResults);
            return results;
        }

        private float[] ExtrapolateFromAboveAtIndexLocation(float[] stagesAtHighestProfile, float upperInterval, int i, int numIntermediateStagesToCompute)
        {
            float[] extrapolatedStages = new float[stagesAtHighestProfile.Length];
            foreach (float stage in stagesAtHighestProfile)
            {
                extrapolatedStages[i] = stage + upperInterval*i;
            }
            return extrapolatedStages;
        }

        private float[] ExtrapolateFromBelowStagesAtIndexLocation(float[] WSEsAtLowest, float interval, int i, int numIntermediateStagesToCompute)
        {
            float[] extrapolatedStages = new float[WSEsAtLowest.Length];
            foreach (float stage in WSEsAtLowest)
            {
                extrapolatedStages[i] = stage - interval*(numIntermediateStagesToCompute-i);
            }
            return extrapolatedStages;
        }

        //public for testing
        //assume that the inventory has already been trimmed 
        public ConsequenceDistributionResults ComputeDamageOneCoordinate(int seed, RandomProvider randomProvider, ConvergenceCriteria convergenceCriteria, Inventory inventory, List<OccupancyType> occupancyType, int impactAreaID, float[] wses)
        {
            double lowerProb = 0.025;
            double upperProb = .975;
            ConsequenceDistributionResults consequenceDistributionResults = new ConsequenceDistributionResults(convergenceCriteria);
            Int64 iteration = 0;
            while (consequenceDistributionResults.ResultsAreConverged(upperProb, lowerProb))
            {
                DeterministicInventory deterministicInventory = inventory.Sample(seed);
                ConsequenceResults consequenceResults = deterministicInventory.ComputeDamages(wses);
                consequenceDistributionResults.AddConsequenceRealization(consequenceResults,impactAreaID,iteration);
                iteration++;
            }
            return consequenceDistributionResults;
        }

        private void step1()
        {

        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        #endregion

    }
}
