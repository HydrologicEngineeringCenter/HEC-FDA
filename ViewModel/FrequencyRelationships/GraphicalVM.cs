using paireddata;
using Statistics.GraphicalRelationships;
using System.Xml.Linq;
using Statistics.Distributions;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using System;
using HEC.MVVMFramework.ViewModel.Implementations;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.TableWithPlot.Data;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class GraphicalVM : ComputeComponentVM
    {
        private int _equivalentRecordLength = 5;
        private bool _useStage;
        private bool _useFlow = true;
        private NamedAction _confidenceLimits;


        public NamedAction ConfidenceLimits 
        { 
            get 
            { 
                return _confidenceLimits; 
            } 
            set 
            { 
                _confidenceLimits = value;
                NotifyPropertyChanged(); 
            }
        }
        public GraphicalUncertainPairedData MyGraphical
        {
            get{return new GraphicalUncertainPairedData(((GraphicalDataProvider)SelectedItem).Xs, ((GraphicalDataProvider)SelectedItem).Ys, EquivalentRecordLength,new CurveMetaData(), usingStagesNotFlows: true);}
           
        }
        public int EquivalentRecordLength
        {
            get{ return _equivalentRecordLength;}
            set
            {
                _equivalentRecordLength = value;
                NotifyPropertyChanged();
            }
        }
        public bool UseFlow
        {
            get { return _useFlow; }
            set
            { 
                _useFlow = value;
                if(value == true)
                {
                    YLabel = Utilities.StringConstants.DISCHARGE;
                }
                NotifyPropertyChanged(); 
            }
        }
        public bool UseStage
        {
            get { return _useStage; }
            set
            { 
                _useStage = value;
                if(value == true)
                {
                    YLabel = Utilities.StringConstants.STAGE;
                }
                NotifyPropertyChanged();
            }
        }
        public GraphicalVM(string name, string xlabel, string ylabel) : base(name, xlabel,ylabel)
        {
            Options.Clear();
            Options.Add(new GraphicalDataProvider());
            SelectedItem = Options[0];
            Initialize();
        }
        public GraphicalVM(XElement vmEle)
        {
            LoadFromXML(vmEle);
            Initialize();
        }
        private void Initialize()
        {
            ConfidenceLimits = new NamedAction();
            ConfidenceLimits.Name = "Compute Confidence Limits";
            ConfidenceLimits.Action = ConfidenceLimitsAction;
        }
        override public XElement ToXML()
        {
            XElement ele = base.ToXML();
            ele.SetAttributeValue("EquivalentRecordLength", EquivalentRecordLength);
            return ele;
        }
        override public void LoadFromXML(XElement element)
        {
            EquivalentRecordLength = int.Parse(element.Attribute("EquivalentRecordLength").Value);
            base.LoadFromXML(element);
        }
        private void ConfidenceLimitsAction(object arg1, EventArgs arg2)
        {
            GraphicalUncertainPairedData graffical = MyGraphical;
            PairedData upperNonExceedence = graffical.SamplePairedData(.975) as PairedData;
            PairedData lowerNonExceedence = graffical.SamplePairedData(.025) as PairedData;
            double[] probs = lowerNonExceedence.Xvals;

            foreach (GraphicalRow row in ((GraphicalDataProvider)SelectedItem).Data)
            {
                int binarySearchReturn = Array.BinarySearch(probs, 1 - row.X);
                int index;
                if(binarySearchReturn < 0)
                {
                    index = ~binarySearchReturn;
                }
                else
                {
                    index =binarySearchReturn;
                }
                row.SetConfidenceLimits(Math.Round(lowerNonExceedence.f(probs[index])), Math.Round(upperNonExceedence.f(probs[index])));
            }
        }
        public GraphicalUncertainPairedData ToGraphicalUncertainPairedData()
        {
           return new GraphicalUncertainPairedData(((GraphicalDataProvider)SelectedItem).Xs, ((GraphicalDataProvider)SelectedItem).Ys , EquivalentRecordLength, "Exceedence Probability", "Flow", "Graphical Flow Frequency");
        }
    }
}
