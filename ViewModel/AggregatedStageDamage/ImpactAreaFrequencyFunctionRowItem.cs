using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class ImpactAreaFrequencyFunctionRowItem:BaseViewModel
    {
        public static string IMPACT_AREA_FREQUENCY_ROW = "ImpactAreaFrequencyRow";
        public static string FREQUENCY_ELEMENT_ID = "FrequencyElementID";
        public static string IMPACT_AREA_ID = "ImpactAreaID";
        public static string STAGE_DISCHARGE_ELEMENT_ID = "StageDischargeElementID";

        private StageDischargeElementWrapper _StageDischargeFunction;
        private FrequencyElementWrapper _FrequencyFunction;

        public ImpactAreaRowItem ImpactArea { get; }
        
        public List<FrequencyElementWrapper> FrequencyFunctions { get;  }

        public FrequencyElementWrapper FrequencyFunction
        {
            get { return _FrequencyFunction; }
            set { _FrequencyFunction = value; NotifyPropertyChanged(); }
        }

        public List<StageDischargeElementWrapper> StageDischargeFunctions { get;  }

        public StageDischargeElementWrapper StageDischargeFunction
        {
            get { return _StageDischargeFunction; }
            set { _StageDischargeFunction = value; NotifyPropertyChanged(); }
        }

        public ImpactAreaFrequencyFunctionRowItem( ImpactAreaRowItem selectedImpactArea, List<AnalyticalFrequencyElement> frequencyFunctions,  List<RatingCurveElement> stageDischargeFunctions)
        {
            StageDischargeFunctions = CreateStageDischargeWrappers(stageDischargeFunctions);
            StageDischargeFunction = StageDischargeFunctions[0];

            FrequencyFunctions = CreateFrequencyWrappers(frequencyFunctions);
            FrequencyFunction = FrequencyFunctions[0];

            ImpactArea = selectedImpactArea;          
        }

        public ImpactAreaFrequencyFunctionRowItem(XElement rowXML)
        {
            int impID = Convert.ToInt32(rowXML.Attribute(IMPACT_AREA_ID)?.Value);
            int freqID = Convert.ToInt32(rowXML.Attribute(FREQUENCY_ELEMENT_ID)?.Value);
            int stageDischargeID = Convert.ToInt32(rowXML.Attribute(STAGE_DISCHARGE_ELEMENT_ID)?.Value);

            //now get the elements from the study cache and match them up
            List<ImpactAreaElement> impAreaElems = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impAreaElems.Count > 0)
            {
                foreach(ImpactAreaRowItem row in impAreaElems[0].ImpactAreaRows)
                {
                    if(row.ID == impID)
                    {
                        ImpactArea = row;
                        break;
                    }
                }
                //we should always be able to find the impact area. We delete the stage damages if the user deletes the impact areas.
            }
            List<AnalyticalFrequencyElement> analyticalFrequencyElements = StudyCache.GetChildElementsOfType<AnalyticalFrequencyElement>();
            AnalyticalFrequencyElement selectedFrequencyFunction = null;
            foreach (AnalyticalFrequencyElement elem in analyticalFrequencyElements)
            {
                if(elem.ID == freqID)
                {
                    selectedFrequencyFunction = elem;
                    break;
                }
            }

            List<RatingCurveElement> ratingCurveElements = StudyCache.GetChildElementsOfType<RatingCurveElement>();
            RatingCurveElement selectedStageDischargeFunction = null;
            foreach (RatingCurveElement elem in ratingCurveElements)
            {
                if(elem.ID == stageDischargeID)
                {
                    selectedStageDischargeFunction = elem;
                    break;
                }
            }

            FrequencyFunctions = CreateFrequencyWrappers(analyticalFrequencyElements);
            StageDischargeFunctions = CreateStageDischargeWrappers(ratingCurveElements);

            SelectSelectedFrequencyFunction(selectedFrequencyFunction);
            SelectSelectedStageDischargeFunction(selectedStageDischargeFunction);
        }

        private void SelectSelectedStageDischargeFunction(RatingCurveElement selectedStageDischargeFunction)
        {
            if (selectedStageDischargeFunction != null)
            {
                //find the wrapper that this belongs to
                foreach (StageDischargeElementWrapper wrapper in StageDischargeFunctions)
                {
                    if (wrapper.Element != null && wrapper.Element.ID == selectedStageDischargeFunction.ID)
                    {
                        StageDischargeFunction = wrapper;
                        break;
                    }
                }
            }
        }

        private void SelectSelectedFrequencyFunction(AnalyticalFrequencyElement selectedFrequencyFunction)
        {
            if (selectedFrequencyFunction != null)
            {
                foreach (FrequencyElementWrapper wrapper in FrequencyFunctions)
                {
                    if (wrapper.Element != null && wrapper.Element.ID == selectedFrequencyFunction.ID)
                    {
                        FrequencyFunction = wrapper;
                        break;
                    }
                }
            }
        }

        private List<FrequencyElementWrapper> CreateFrequencyWrappers(List<AnalyticalFrequencyElement> frequencyFunctions)
        {
            List<FrequencyElementWrapper> frequencyWrappers = new List<FrequencyElementWrapper>();
            //add blank row
            frequencyWrappers.Add(new FrequencyElementWrapper());
            foreach (AnalyticalFrequencyElement elem in frequencyFunctions)
            {
                frequencyWrappers.Add(new FrequencyElementWrapper(elem));
            }
            return frequencyWrappers;
        }

        private List<StageDischargeElementWrapper> CreateStageDischargeWrappers(List<RatingCurveElement> stageDischargeFunctions)
        {
            List<StageDischargeElementWrapper> stageDischargeWrappers = new List<StageDischargeElementWrapper>();
            //add blank row
            stageDischargeWrappers.Add(new StageDischargeElementWrapper());
            foreach (RatingCurveElement elem in stageDischargeFunctions)
            {
                stageDischargeWrappers.Add(new StageDischargeElementWrapper(elem));
            }

            return stageDischargeWrappers;
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
            else if (IsStageDischargeRequired())
            {
                if (StageDischargeFunction.Element == null)
                {
                    vr.AddErrorMessage("Impact area " + ImpactArea.Name + " does not have a stage discharge assignment which will result in poor estimates. Please define this assignment.");
                }
            }
            return vr;
        }

        public bool IsStageDischargeRequired()
        {
            return (FrequencyFunction.Element.IsAnalytical || FrequencyFunction.Element.MyGraphicalVM.UseFlow);
        }

        public XElement WriteToXML()
        {
            XElement impactAreaFrequencyRowElement = new XElement(IMPACT_AREA_FREQUENCY_ROW);
            int impArea = -1;
            if(ImpactArea != null)
            {
                impArea = ImpactArea.ID;
            }
            impactAreaFrequencyRowElement.SetAttributeValue(IMPACT_AREA_ID, impArea);

            int freqID = -1;
            if(FrequencyFunction != null && FrequencyFunction.Element != null)
            {
                freqID = FrequencyFunction.Element.ID;
            }
            impactAreaFrequencyRowElement.SetAttributeValue(FREQUENCY_ELEMENT_ID, freqID);

            int stageDischargeID = -1;
            if(StageDischargeFunction != null && StageDischargeFunction.Element != null)
            {
                stageDischargeID = StageDischargeFunction.Element.ID;
            }
            impactAreaFrequencyRowElement.SetAttributeValue(STAGE_DISCHARGE_ELEMENT_ID, stageDischargeID);

            return impactAreaFrequencyRowElement;
        }




        public ImpactAreaFrequencyFunctionRowItem Clone()
        {
            return new ImpactAreaFrequencyFunctionRowItem(WriteToXML());
        }

    }
}
