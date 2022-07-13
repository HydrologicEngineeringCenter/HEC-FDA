using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class ImpactAreaFrequencyFunctionRowItem
    {
        private FrequencyElementWrapper _FrequencyFunction;
        public ImpactAreaRowItem ImpactArea { get; set; }
        
        public List<FrequencyElementWrapper> FrequencyFunctions { get; set; }

        public FrequencyElementWrapper FrequencyFunction 
        {
            get { return _FrequencyFunction; }
            set
            {
                _FrequencyFunction = value;

            }
        }

        public List<StageDischargeElementWrapper> StageDischargeFunctions { get; set; }

        public StageDischargeElementWrapper StageDischargeFunction { get; set; }


        //public ImpactAreaFrequencyFunctionRowItem( ImpactAreaRowItem impactArea, List<AnalyticalFrequencyElement> frequencyFunctions, 
        //    AnalyticalFrequencyElement frequencyFunction, List<RatingCurveElement> stageDischargeFunctions, RatingCurveElement stageDischargeFunction)
        //{
        //    StageDischargeFunctions = stageDischargeFunctions;
        //    FrequencyFunctions = frequencyFunctions;
        //    ImpactArea = impactArea;
        //    FrequencyFunction = frequencyFunction;
        //    StageDischargeFunction = stageDischargeFunction;
        //}

        public ImpactAreaFrequencyFunctionRowItem( ImpactAreaRowItem selectedImpactArea, List<AnalyticalFrequencyElement> frequencyFunctions,  List<RatingCurveElement> stageDischargeFunctions)
        {

            List<StageDischargeElementWrapper> stageDischargeWrappers = new List<StageDischargeElementWrapper>();
            //add blank row
            stageDischargeWrappers.Add(new StageDischargeElementWrapper());
            foreach(RatingCurveElement elem in stageDischargeFunctions)
            {
                stageDischargeWrappers.Add(new StageDischargeElementWrapper(elem));
            }
            StageDischargeFunctions = stageDischargeWrappers;
            StageDischargeFunction = StageDischargeFunctions[0];


            List<FrequencyElementWrapper> frequencyWrappers = new List<FrequencyElementWrapper>();
            //add blank row
            frequencyWrappers.Add(new FrequencyElementWrapper());
            foreach(AnalyticalFrequencyElement elem in frequencyFunctions)
            {
                frequencyWrappers.Add(new FrequencyElementWrapper(elem));
            }

            FrequencyFunctions = frequencyWrappers;
            FrequencyFunction = FrequencyFunctions[0];

            ImpactArea = selectedImpactArea;          
        }

        public FdaValidationResult ValidateRow()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (FrequencyFunction.Element == null)
            {
                vr.AddErrorMessage("Impact area " + ImpactArea.Name + " does not have a frequency function assignment which will result in poor estimates. Please define this assignment.");
                if (StageDischargeFunction.Element == null)
                {
                    vr.AddErrorMessage("Impact area " + ImpactArea.Name + " does not have a stage discharge assignment which will result in poor estimates. Please define this assignment.");
                }
            }
            else if (FrequencyFunction.Element.IsAnalytical || FrequencyFunction.Element.MyGraphicalVM.UseFlow)
            {
                //then a stage discharge is required
                if(StageDischargeFunction.Element == null)
                {
                    vr.AddErrorMessage("Impact area " + ImpactArea.Name + " does not have a stage discharge assignment which will result in poor estimates. Please define this assignment.");
                }
            }
            return vr;
        }

    }
}
