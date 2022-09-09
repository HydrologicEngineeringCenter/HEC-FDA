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

namespace stageDamage
{
    public class ImpactAreaStageDamage : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields 
        private const double MIN_PROBABILITY = 0.0001;
        private const double MAX_PROBABILITY = 0.9999;
        private ContinuousDistribution _AnalyticalFlowFrequency;
        private GraphicalUncertainPairedData _GraphicalFrequency;
        private UncertainPairedData _DischargeStage;
        private int _ImpactAreaID;

        #endregion

        #region Properties 
        public event MessageReportedEventHandler MessageReport;
        #endregion
        #region Constructor
        public ImpactAreaStageDamage(int impactAreaID, ContinuousDistribution analyticalFlowFrequency = null, GraphicalUncertainPairedData graphicalFrequency = null, UncertainPairedData dischargeStage = null)
        {
            //TODO: Validate provided functions here
            _AnalyticalFlowFrequency = analyticalFlowFrequency;
            _GraphicalFrequency = graphicalFrequency;
            _DischargeStage = dischargeStage;
            _ImpactAreaID = impactAreaID;

        }
        #endregion

        #region Methods
        //TODO: This compute produces the uncertain paired data
        //That means that we need to have all hydraulic profiles available 
        //The list of stages to me appears to mean that we won't 
        public List<UncertainPairedData> Compute(compute.RandomProvider randomProvider, ConvergenceCriteria convergenceCriteria, List<double> stages, Inventory inventory, List<OccupancyType> occupancyType)
        {
            List<string> damageCategories = inventory.GetUniqueDamageCatagories();
            //I think we are going to have a list of ImpactAreaStageDamageFUnctions - one for each damage category 
            List<UncertainPairedData> results = new List<UncertainPairedData>();
            //This is where the meat of the compute lives 

            //Find the min stage and max stage for the impact area index location 
            double minStage;
            double maxStage;
            //TODO we need a stage-frequency function to reference for the compute below. 

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
                } else
                {
                    if (_DischargeStage != null)
                    {
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


            //Step 2: Find the deltas 

            //have to get water
            //this will consist of a hydraulic profile that will have info on terrain, WSEs, we need to give set of points, 
            //will spit back list of double indexed to given points
            //we'll need to identify the AEP 
            //because we want the most frequent for the first part of this algorithm

            List<double> depths = inventory.getWater(water);
            

            //Step 3 compute damage by iterating over stages. 
            //One iteration gets 
            //Then we iterate between the min and the most frequent event in the hydraulic data set 
            //Then iterate from most frequent event to least frequent event 
            //Then iterate from least frequent event to the max 

            return results;
        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        #endregion

    }
}
