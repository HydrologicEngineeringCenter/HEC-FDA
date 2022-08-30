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
            ImpactAreaStageDamageFunction results = new ImpactAreaStageDamageFunction();
            //This is where the meat of the compute lives 
        }


        #endregion

    }
}
