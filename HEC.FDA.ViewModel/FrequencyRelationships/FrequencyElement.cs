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
    public bool IsAnalytical { get => !_frequencyEditorVM.IsGraphical; }
    public bool GraphicalUsesFlow { get => _frequencyEditorVM.MyGraphicalVM.UseFlow; }
    public LogPearson3 LPIII
    {
        get
        {
            if (IsAnalytical)
            {
                return _frequencyEditorVM.ParameterEntryVM.LP3Distribution;
            }
            return null;
        }
    }
    public UncertainPairedData LPIIIasUPD { get => LPIII.BootstrapToUncertainPairedData(new RandomProvider(1234), LogPearson3._RequiredExceedanceProbabilitiesForBootstrapping); }
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
                return _frequencyEditorVM.MyGraphicalVM.CreateGraphicalUncertainPairedData();
            }
        }
    }
    #endregion

    #region Constructors
    //fresh editor
    public FrequencyElement(string name, string lastEditDate, string desc, int id, FrequencyEditorVM vm)
        : base(name, lastEditDate, desc, id)
    {
        _frequencyEditorVM = vm;
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
        string POR_XML_TAG = "POR";
        string MOMENTS = "Moments";
        string MEAN = "Mean";
        string ST_DEV = "StDev";
        string SKEW = "Skew";

        FrequencyEditorVM vm = new()
        {
            IsGraphical = !(bool)flowFreqElem.Attribute(IS_ANALYTICAL)
        };

        XElement analyticalElem = flowFreqElem.Element(ANALYTICAL_DATA);
        vm.ParameterEntryVM.SampleSize = (int)analyticalElem.Attribute(POR_XML_TAG);

        XElement momentsElem = analyticalElem.Element(MOMENTS);
        vm.ParameterEntryVM.Mean = (double)momentsElem.Attribute(MEAN);
        vm.ParameterEntryVM.Standard_Deviation = (double)momentsElem.Attribute(ST_DEV);
        vm.ParameterEntryVM.Skew = (double)momentsElem.Attribute(SKEW);

        XElement graphiclVMele = flowFreqElem.Element("GraphicalVM");
        if (graphiclVMele != null)
        {
            vm.MyGraphicalVM = new GraphicalVM(graphiclVMele);
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
        vm.InitializeConfidenceBounds();
        string header = "Edit " + vm.Name;
        DynamicTabVM tab = new(header, vm, "EditAnalyticalFrequency" + vm.Name);
        Navigate(tab, false, false);
    }
    #endregion

    public override XElement ToXML()
    {
        XElement flowFrequencyElement = new(GetType().Name);
        flowFrequencyElement.Add(CreateHeaderElement());
        flowFrequencyElement.Add(_frequencyEditorVM.ToXML());
        return flowFrequencyElement;
    }
}
