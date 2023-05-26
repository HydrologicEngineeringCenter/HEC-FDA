using HEC.FDA.Model.compute;
using HEC.FDA.Model.extensions;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;
using HEC.FDA.ViewModel.Utilities;
using Statistics.Distributions;
using System;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships;

public class FrequencyElement : ChildElement
{
    #region Fields
    private FrequencyEditorVM _frequencyEditorVM;
    #endregion

    #region Properties  
    public XElement FrequencyEditorXML { get => _frequencyEditorVM.ToXML(); }
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
    public UncertainPairedData LPIIIasUPD { get => LPIII.BootstrapToUncertainPairedData(new RandomProvider(1234).NextRandomSequenceSet(1000, LPIII.SampleSize), LogPearson3._RequiredExceedanceProbabilitiesForBootstrapping); }
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
    public FrequencyElement(string name, string lastEditDate, string desc, int id, FrequencyEditorVM vm) 
        : base(name, lastEditDate, desc, id)
    {
        _frequencyEditorVM = new FrequencyEditorVM(vm.ToXML());
        AddDefaultActions(EditFlowFreq, StringConstants.EDIT_FREQUENCY_FUNCTIONS_MENU);
    }
    //load from database
    public FrequencyElement(XElement flowFreqElem, int id) : base(flowFreqElem, id)
    {
        if (flowFreqElem.Name.LocalName.Equals(typeof(ChildElement).Name))
        {
            ConstructFromDeprecatedSave(flowFreqElem);
        }
        else
        {
            XElement freqEditVMEle = flowFreqElem.Element(typeof(FrequencyEditorVM).Name);
            _frequencyEditorVM = new FrequencyEditorVM(freqEditVMEle);
        }
        AddDefaultActions(EditFlowFreq, StringConstants.EDIT_FREQUENCY_FUNCTIONS_MENU);
    }

    private void ConstructFromDeprecatedSave(XElement flowFreqElem)
    {
        string IS_ANALYTICAL = "IsAnalytical";
        string ANALYTICAL_DATA = "AnalyticalData";
        string USES_MOMENTS = "UsesMoments";
        string POR_XML_TAG = "POR";
        string MOMENTS = "Moments";
        string MEAN = "Mean";
        string ST_DEV = "StDev";
        string SKEW = "Skew";
        string FIT_TO_FLOWS = "FitToFlows";
        string FLOWS = "Flows";

        FrequencyEditorVM vm = new();
        vm.IsGraphical = !(bool)flowFreqElem.Attribute(IS_ANALYTICAL);

        XElement analyticalElem = flowFreqElem.Element(ANALYTICAL_DATA);
        vm.AnalyticalVM.IsFitToFlows = !(bool)analyticalElem.Attribute(USES_MOMENTS);
        vm.AnalyticalVM.ParameterEntryVM.SampleSize = (int)analyticalElem.Attribute(POR_XML_TAG);

        XElement momentsElem = analyticalElem.Element(MOMENTS);
        vm.AnalyticalVM.ParameterEntryVM.Mean = (double)momentsElem.Attribute(MEAN);
        vm.AnalyticalVM.ParameterEntryVM.Standard_Deviation = (double)momentsElem.Attribute(ST_DEV);
        vm.AnalyticalVM.ParameterEntryVM.Skew = (double)momentsElem.Attribute(SKEW);

        XElement fitToFlowsElem = analyticalElem.Element(FIT_TO_FLOWS);
        string flows = (string)fitToFlowsElem.Attribute(FLOWS);
        if (!String.IsNullOrEmpty(flows))
        {
            string[] flowStrings = flows.Split(',');
            double[] flowdoubles = flowStrings.Select(flowString => Convert.ToDouble(flowString)).ToArray();
            vm.AnalyticalVM.FitToFlowVM.Data.Clear();
            foreach (double flow in flowdoubles)
            {
                vm.AnalyticalVM.FitToFlowVM.Data.Add(new FlowDoubleWrapper(flow));
            }
        }

        XElement graphiclVMele = flowFreqElem.Element("GraphicalVM");
        if (graphiclVMele != null)
        {
            vm.GraphicalVM.CurveComponentVM = new GraphicalVM(graphiclVMele);
        }
        _frequencyEditorVM = vm;
    }
    #endregion

    #region Voids
    public void EditFlowFreq(object arg1, EventArgs arg2)
    {
        EditorActionManager actionManager = new EditorActionManager()
            .WithSiblingRules(this);
        FrequencyEditorVM vm = new(this, actionManager);
        string header = "Edit " + vm.Name;
        DynamicTabVM tab = new(header, vm, "EditAnalyticalFrequency" + vm.Name);
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
