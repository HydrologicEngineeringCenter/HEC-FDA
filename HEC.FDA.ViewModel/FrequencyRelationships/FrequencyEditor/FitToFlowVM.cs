using HEC.FDA.ViewModel.TableWithPlot;
using HEC.MVVMFramework.ViewModel.Implementations;
using Statistics.Distributions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor
{
    public class FitToFlowVM:ParameterEntryVM
    {
        #region Backing Fields
        private ObservableCollection<FlowDoubleWrapper> _flows = new();
        #endregion

        #region Properties
        public ObservableCollection<FlowDoubleWrapper> Data //This is called data because FdaDataGridControl binds to a property called Data
        {
            get { return _flows; }
            set { _flows = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region Constructors
        public FitToFlowVM():base()
        {
            LoadDefaultFlows();
            Compute = new NamedAction
            {
                Name = "Fit LP3 to Flows",
                Action = ComputeAction
            };
            ComputeAction(null,null);
        }

        public FitToFlowVM(XElement xElement) : base(xElement)
        {
            FromXML(xElement);
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
            double[] flows = Data.Select(x => x.Flow).ToArray();
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
        public XElement ToXML()
        {
            XElement ele = base.ToXML();
            string flows = String.Join(",", Data.Select((x) => x.Flow).ToArray());
            ele.SetAttributeValue(nameof(Data), flows);
            return ele;
        }
        public void FromXML(XElement ele)
        {
            string flowsAsString = ele.Attribute(nameof(Data)).Value;
            string [] splitFlows = flowsAsString.Split(',');
            double [] doubleFlows = splitFlows.Select((x) => Double.Parse(x)).ToArray();
            Data = new ObservableCollection<FlowDoubleWrapper>(doubleFlows.Select((x) => new FlowDoubleWrapper(x)).ToArray());
        }
        #endregion
    }
}
