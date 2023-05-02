using HEC.FDA.Model.compute;
using HEC.FDA.Model.extensions;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;
using HEC.FDA.ViewModel.Utilities;
using HEC.Plotting.Core.DataModel;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyElement : ChildElement
    {
        #region Fields
        private readonly FrequencyEditorVM _frequencyEditorVM;
        #endregion

        #region Properties  
        public XElement FrequencyEditorProperties { get => _frequencyEditorVM.ToXML(); }
        public bool IsAnalytical { get => _frequencyEditorVM.IsAnalytic; }
        public bool IsStandard { get => _frequencyEditorVM.AnalyticalVM.IsParameterEntry;}
        public bool GraphicalUsesFlow { get => ((GraphicalVM)(_frequencyEditorVM.GraphicalVM.CurveComponentVM)).UseFlow; }
        public LogPearson3 LPIII
        {
            get
            {
                if(IsAnalytical)
                {
                    if (IsStandard)
                    {
                        return _frequencyEditorVM.AnalyticalVM.ParameterEntryVM.LP3Distribution;
                    }
                    else
                    {
                        return _frequencyEditorVM.AnalyticalVM.FitToFlowVM.LP3Distribution;
                    }
                }
                return null;
            }
        }
        public UncertainPairedData LPIIIasUPD { get => LPIII.BootstrapToUncertainPairedData(new RandomProvider(1234), LogPearson3._RequiredExceedanceProbabilitiesForBootstrapping, 1000, 10); }
        public GraphicalUncertainPairedData GraphicalUncertainPairedData
        {
            get
            {
                if (IsAnalytical)
                {
                    return null;
                }
                else
                {
                    return ((GraphicalVM)_frequencyEditorVM.GraphicalVM.CurveComponentVM).GraphicalUncertainPairedData;
                }
            }
        }

        #endregion

        #region Constructors
        //fresh editor
        public AnalyticalFrequencyElement(string name, string lastEditDate, string desc, int id, FrequencyEditorVM vm) 
            : base(name, lastEditDate, desc, id)
        {
            _frequencyEditorVM = vm;
            AddDefaultActions(EditFlowFreq, StringConstants.EDIT_FREQUENCY_FUNCTIONS_MENU);
        }
        //load from database
        public AnalyticalFrequencyElement(XElement flowFreqElem, int id) : base(flowFreqElem, id)
        {
            XElement freqEditVMEle = flowFreqElem.Element(typeof(FrequencyEditorVM).Name);
            _frequencyEditorVM = new FrequencyEditorVM(freqEditVMEle);
            AddDefaultActions(EditFlowFreq, StringConstants.EDIT_FREQUENCY_FUNCTIONS_MENU);
        }
        #endregion

        #region Voids
        public void EditFlowFreq(object arg1, EventArgs arg2)
        {
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);
            FrequencyEditorVM vm = new(this, actionManager);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditAnalyticalFrequency" + vm.Name);
            Navigate(tab, false, false);
        }
        #endregion

        public override XElement ToXML()
        {
            XElement flowFrequencyElement = new XElement(GetType().Name);
            flowFrequencyElement.Add(CreateHeaderElement());
            flowFrequencyElement.Add(_frequencyEditorVM.ToXML());
            return flowFrequencyElement;
        }
    }
}
