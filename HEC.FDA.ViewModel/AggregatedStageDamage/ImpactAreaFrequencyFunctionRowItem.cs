using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class ImpactAreaFrequencyFunctionRowItem:BaseViewModel
    {
        public static string IMPACT_AREA_FREQUENCY_ROW = "ImpactAreaFrequencyRow";
        public static string FREQUENCY_ELEMENT_ID = "FrequencyElementID";
        public static string IMPACT_AREA_ID = "ImpactAreaID";
        public static string STAGE_DISCHARGE_ELEMENT_ID = "StageDischargeElementID";
        public static string REGULATED_UNREGULATED_ID = "RegulatedUnregulatedElementID";

        private StageDischargeElementWrapper _StageDischargeFunction;
        private FrequencyElementWrapper _FrequencyFunction;
        private RegulatedUnregulatedElementWrapper _RegulatedUnregulatedFunction;

        public ImpactAreaRowItem ImpactArea { get; }
        
        public ObservableCollection<FrequencyElementWrapper> FrequencyFunctions { get;  }
        public ObservableCollection<RegulatedUnregulatedElementWrapper> RegulatedUnregulatedFunctions { get; }

        public FrequencyElementWrapper FrequencyFunction
        {
            get { return _FrequencyFunction; }
            set { _FrequencyFunction = value; NotifyPropertyChanged(); }
        }
        public RegulatedUnregulatedElementWrapper RegulatedUnregulatedFunction
        {
            get { return _RegulatedUnregulatedFunction; }
            set { _RegulatedUnregulatedFunction = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<StageDischargeElementWrapper> StageDischargeFunctions { get;  }

        public StageDischargeElementWrapper StageDischargeFunction
        {
            get { return _StageDischargeFunction; }
            set { _StageDischargeFunction = value; NotifyPropertyChanged(); }
        }

        public ImpactAreaFrequencyFunctionRowItem( ImpactAreaRowItem selectedImpactArea, List<FrequencyElement> frequencyFunctions,  
            List<StageDischargeElement> stageDischargeFunctions, List<InflowOutflowElement> regulatedUnregulatedElements)
        {
            StageDischargeFunctions = CreateStageDischargeWrappers(stageDischargeFunctions);
            StageDischargeFunction = StageDischargeFunctions[0];

            FrequencyFunctions = CreateFrequencyWrappers(frequencyFunctions);
            FrequencyFunction = FrequencyFunctions[0];

            //I am not selecting one because we default to the blank row.
            RegulatedUnregulatedFunctions = CreateRegulatedUnregulatedWrappers(regulatedUnregulatedElements);

            ImpactArea = selectedImpactArea;          
        }

        public ImpactAreaFrequencyFunctionRowItem(XElement rowXML)
        {
            int impID = Convert.ToInt32(rowXML.Attribute(IMPACT_AREA_ID)?.Value);
            int freqID = Convert.ToInt32(rowXML.Attribute(FREQUENCY_ELEMENT_ID)?.Value);
            int stageDischargeID = Convert.ToInt32(rowXML.Attribute(STAGE_DISCHARGE_ELEMENT_ID)?.Value);
            int regUnregID = Convert.ToInt32(rowXML.Attribute(REGULATED_UNREGULATED_ID)?.Value);

            //now get the elements from the study cache and match them up
            if (StudyCache != null)
            {

                List<ImpactAreaElement> impAreaElems = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
                if (impAreaElems.Count > 0)
                {
                    foreach (ImpactAreaRowItem row in impAreaElems[0].ImpactAreaRows)
                    {
                        if (row.ID == impID)
                        {
                            ImpactArea = row;
                            break;
                        }
                    }
                    //we should always be able to find the impact area. We delete the stage damages if the user deletes the impact areas.
                }

                List<FrequencyElement> analyticalFrequencyElements = StudyCache.GetChildElementsOfType<FrequencyElement>();
                FrequencyElement selectedFrequencyFunction = null;
                foreach (FrequencyElement elem in analyticalFrequencyElements)
                {
                    if (elem.ID == freqID)
                    {
                        selectedFrequencyFunction = elem;
                        break;
                    }
                }

                List<InflowOutflowElement> regulatedUnregulatedElements = StudyCache.GetChildElementsOfType<InflowOutflowElement>();
                InflowOutflowElement selectedRegulatedUnregulatedFunction = null;
                foreach (InflowOutflowElement elem in regulatedUnregulatedElements)
                {
                    if (elem.ID == regUnregID)
                    {
                        selectedRegulatedUnregulatedFunction = elem;
                        break;
                    }
                }

                List<StageDischargeElement> ratingCurveElements = StudyCache.GetChildElementsOfType<StageDischargeElement>();
                StageDischargeElement selectedStageDischargeFunction = null;
                foreach (StageDischargeElement elem in ratingCurveElements)
                {
                    if (elem.ID == stageDischargeID)
                    {
                        selectedStageDischargeFunction = elem;
                        break;
                    }
                }
                FrequencyFunctions = CreateFrequencyWrappers(analyticalFrequencyElements);
                StageDischargeFunctions = CreateStageDischargeWrappers(ratingCurveElements);
                RegulatedUnregulatedFunctions = CreateRegulatedUnregulatedWrappers(regulatedUnregulatedElements);

                SelectSelectedFrequencyFunction(selectedFrequencyFunction);
                SelectSelectedStageDischargeFunction(selectedStageDischargeFunction);
                SelectSelectedRegulatedUnregulatedFunction(selectedRegulatedUnregulatedFunction);
            }
        }

        private void SelectSelectedStageDischargeFunction(StageDischargeElement selectedStageDischargeFunction)
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

        private void SelectSelectedFrequencyFunction(FrequencyElement selectedFrequencyFunction)
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

        private void SelectSelectedRegulatedUnregulatedFunction(InflowOutflowElement selectedRegUnregFunction)
        {
            if (selectedRegUnregFunction != null)
            {
                foreach (RegulatedUnregulatedElementWrapper wrapper in RegulatedUnregulatedFunctions)
                {
                    if (wrapper.Element != null && wrapper.Element.ID == selectedRegUnregFunction.ID)
                    {
                        RegulatedUnregulatedFunction = wrapper;
                        break;
                    }
                }
            }
        }

        private ObservableCollection<FrequencyElementWrapper> CreateFrequencyWrappers(List<FrequencyElement> frequencyFunctions)
        {
            ObservableCollection<FrequencyElementWrapper> frequencyWrappers = new ObservableCollection<FrequencyElementWrapper>();
            //add blank row
            frequencyWrappers.Add(new FrequencyElementWrapper());
            foreach (FrequencyElement elem in frequencyFunctions)
            {
                frequencyWrappers.Add(new FrequencyElementWrapper(elem));
            }
            return frequencyWrappers;
        }
        private ObservableCollection<RegulatedUnregulatedElementWrapper> CreateRegulatedUnregulatedWrappers(List<InflowOutflowElement> inflowOutflowElement)
        {
            ObservableCollection<RegulatedUnregulatedElementWrapper> regWrappers = new ObservableCollection<RegulatedUnregulatedElementWrapper>();
            //add blank row
            regWrappers.Add(new RegulatedUnregulatedElementWrapper());
            foreach (InflowOutflowElement elem in inflowOutflowElement)
            {
                regWrappers.Add(new RegulatedUnregulatedElementWrapper(elem));
            }
            return regWrappers;
        }

        private ObservableCollection<StageDischargeElementWrapper> CreateStageDischargeWrappers(List<StageDischargeElement> stageDischargeFunctions)
        {
            ObservableCollection<StageDischargeElementWrapper> stageDischargeWrappers = new ObservableCollection<StageDischargeElementWrapper>();
            //add blank row
            stageDischargeWrappers.Add(new StageDischargeElementWrapper());
            foreach (StageDischargeElement elem in stageDischargeFunctions)
            {
                stageDischargeWrappers.Add(new StageDischargeElementWrapper(elem));
            }

            return stageDischargeWrappers;
        }

        public FdaValidationResult ValidateRow()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (FrequencyFunction == null || FrequencyFunction.Element == null)
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
            return (FrequencyFunction.Element.IsAnalytical || FrequencyFunction.Element.GraphicalUsesFlow);
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

            int regulatedUnregulatedID = -1;
            if (RegulatedUnregulatedFunction != null && RegulatedUnregulatedFunction.Element != null)
            {
                regulatedUnregulatedID = RegulatedUnregulatedFunction.Element.ID;
            }
            impactAreaFrequencyRowElement.SetAttributeValue(REGULATED_UNREGULATED_ID, regulatedUnregulatedID);

            return impactAreaFrequencyRowElement;
        }




        public ImpactAreaFrequencyFunctionRowItem Clone()
        {
            return new ImpactAreaFrequencyFunctionRowItem(WriteToXML());
        }

    }
}
