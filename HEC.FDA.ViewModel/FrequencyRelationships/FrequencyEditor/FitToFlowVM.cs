using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.TableWithPlot.Data;
using HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces;
using HEC.MVVMFramework.ViewModel.Implementations;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;

public class FitToFlowVM : ParameterEntryVM, IDisplayWithFDADataGrid
{
    #region Properties
    private ObservableCollection<object> _flows = new();
    public ObservableCollection<object> Data { get => _flows; } //Property Named Data to work as an FdaDataGridControl DataContext.
    #endregion

    #region Constructors
    public FitToFlowVM() : base()
    {
        LoadDefaultFlows();
        Compute = new NamedAction
        {
            Name = "Fit LP3 to Flows",
            Action = ComputeAction
        };
        ComputeAction(null, null);
    }

    public FitToFlowVM(XElement xElement) : base(xElement)
    {
    }
    #endregion

    #region Named Actions
    //Named Actions always have a backing field, "Property" and a {PropertyName}Action method in the class that gets fired when the action is triggered. Also need to be instatiated in the constructor.

    //NA Backing Fields
    private NamedAction _compute;
    //NA "Properties" for Binding
    public NamedAction Compute { get { return _compute; } set { _compute = value; NotifyPropertyChanged(); } }
    //NA Methods are titled with "Action" as convention
    private void ComputeAction(object arg1, EventArgs arg2)
    {
        double[] flows = Data.Select(x => ((FlowDoubleWrapper)x).Flow).ToArray();
        var newLP3 = LP3Distribution.Fit(flows);
        LP3Distribution = (LogPearson3)newLP3;
        NotifyPropertyChanged(nameof(Mean));
        NotifyPropertyChanged(nameof(SampleSize));
        NotifyPropertyChanged(nameof(Skew));
        NotifyPropertyChanged(nameof(Standard_Deviation));
        UpdateTable();
    }

    #endregion

    #region Methods
    private void LoadDefaultFlows()
    {
        for (int i = 1; i < 11; i++)
        {
            FlowDoubleWrapper fdw = new FlowDoubleWrapper(i * 1000);
            Data.Add(fdw);
        }
    }
    #endregion

    #region Save and Load
    public override XElement ToXMLInternal(XElement ele)
    {
        string flows = String.Join(",", Data.Select((x) => ((FlowDoubleWrapper)x).Flow).ToArray());
        ele.SetAttributeValue(nameof(Data), flows);
        return ele;
    }
    public override void FromXMLInternal(XElement ele)
    {
        string flowsAsString = ele.Attribute(nameof(Data)).Value;
        string[] splitFlows = flowsAsString.Split(',');
        double[] doubleFlows = splitFlows.Select((x) => Double.Parse(x)).ToArray();
        _flows = new ObservableCollection<object>(doubleFlows.Select((x) => new FlowDoubleWrapper(x)).ToArray());
    }
    #endregion

    #region Data Grid
    public void RemoveRows(List<int> rowIndices)
    {
        for (int i = rowIndices.Count() - 1; i >= 0; i--)
        {
            Data.RemoveAt(rowIndices[i]);
        }
    }

    public void AddRow(int i)
    {
        Data.Insert(i, new FlowDoubleWrapper(0));
    }
    #endregion
}
