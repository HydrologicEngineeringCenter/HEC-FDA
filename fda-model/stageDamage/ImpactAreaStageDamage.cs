using paireddata;
using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using structures;

namespace stageDamage
{
    public class ImpactAreaStageDamage
{
        #region Fields 
        private ContinuousDistribution _AnalyticalFlowFrequency;
        private GraphicalUncertainPairedData _GraphicalFrequency;
        private UncertainPairedData _DischargeStage;
        private int _ImpactAreaID;

        #endregion
        #region Constructor
        public ImpactAreaStageDamage(ContinuousDistribution analyticalFlowFrequency, GraphicalUncertainPairedData graphicalFrequency, UncertainPairedData dischargeStage, int impactAreaID)
        {
            _AnalyticalFlowFrequency = analyticalFlowFrequency;
            _GraphicalFrequency = graphicalFrequency;
            _DischargeStage = dischargeStage;
            _ImpactAreaID = impactAreaID;

        }
        #endregion

        #region Methods
        public ImpactAreaStageDamageFunction Compute(compute.RandomProvider randomProvider, ConvergenceCriteria convergenceCriteria, List<double> stages, Inventory inventory, List<OccupancyType> occupancyType)
        {
            //I think we are going to have a list of ImpactAreaStageDamageFUnctions - one for each damage category 
            ImpactAreaStageDamageFunction results = new ImpactAreaStageDamageFunction();
            //This is where the meat of the compute lives 

            //Step 1: Find the min and max of stages 
            //Step 2: Find the deltas 

            //Step 3 compute damage by iterating over stages. 
            //One iteration gets 
            //Then we iterate between the min and the most frequent event in the hydraulic data set 
            //Then iterate from most frequent event to least frequent event 
            //Then iterate from least frequent event to the max 

            return results;
        }


        #endregion

    }
}
