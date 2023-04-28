using System.Xml.Linq;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using System;
using HEC.MVVMFramework.ViewModel.Implementations;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.TableWithPlot.Data;
using Importer;
using HEC.FDA.Model.paireddata;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor
{
    public class FEGraphicalVM : CurveComponentVM
    {
        private int _equivalentRecordLength = Utilities.DefaultData.PeriodOfRecord;
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
        public GraphicalUncertainPairedData GraphicalUncertainPairedData
        { 
            get{return new GraphicalUncertainPairedData(((GraphicalDataProvider)SelectedItem).Xs, ((GraphicalDataProvider)SelectedItem).Ys, EquivalentRecordLength,new CurveMetaData(), !UseFlow);}
           
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
                if (value == true)
                {
                    YLabel = Utilities.StringConstants.DISCHARGE;
                }
                else
                {
                    YLabel = Utilities.StringConstants.STAGE;
                }
                NotifyPropertyChanged();
            }
        }
        public FEGraphicalVM(string name, string xlabel, string ylabel) : base(name, xlabel,ylabel)
        {
            Options.Clear();
            Options.Add(new GraphicalDataProvider(UseFlow));
            SelectedItem = Options[0];
            Initialize();
        }
        public FEGraphicalVM(XElement vmEle)
        {
            LoadFromXML(vmEle);
            Initialize();
        }
        public FEGraphicalVM(ProbabilityFunction probabilityFunction)
        {
            LoadFromProbabilityFunction(probabilityFunction);
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
            ele.SetAttributeValue(nameof(EquivalentRecordLength), EquivalentRecordLength);
            ele.SetAttributeValue(nameof(UseFlow), UseFlow);
            return ele;
        }
        override public void LoadFromXML(XElement element)
        {
            EquivalentRecordLength = int.Parse(element.Attribute(nameof(EquivalentRecordLength)).Value);
            UseFlow = bool.Parse(element.Attribute(nameof(UseFlow)).Value);
            base.LoadFromXML(element);
        }
        /// <summary>
        /// This loads a default FEGraphicalVM from a ProbabilityFunction Object which is the output of the FDA1.4Import Helper. 
        /// </summary>
        /// <param name="pf"></param>
        private void LoadFromProbabilityFunction(ProbabilityFunction pf)
        {
            Options.Clear();
            Options.Add(new GraphicalDataProvider(UseFlow));
            SelectedItem = Options[0];
            Initialize();
            SelectedItem.Data.Clear();
            base.Name = Utilities.StringConstants.GRAPHICAL_FREQUENCY;
            base.XLabel = Utilities.StringConstants.EXCEEDANCE_PROBABILITY;
            double[] probs = pf.ExceedanceProbability;
            double[] ys;
            if (pf.ProbabilityDataTypeId == ProbabilityFunction.ProbabilityDataType.DISCHARGE_FREQUENCY)
            {
                ys = pf.Discharge;
                UseFlow = true; //This will also set the YLabel
            }
            else
            {
                ys = pf.Stage;
                UseFlow = false;//This will also set the YLabel
            }

            for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
            {
                SelectedItem.AddRow(i);
                SelectedItem.Data[i] = new GraphicalRow(probs[i], ys[i]);
            }
            EquivalentRecordLength = pf.EquivalentLengthOfRecord;
        }
        private void ConfidenceLimitsAction(object arg1, EventArgs arg2)
        {
            GraphicalUncertainPairedData graphical = GraphicalUncertainPairedData;
            PairedData upperNonExceedence = graphical.SamplePairedData(.975) as PairedData;
            PairedData lowerNonExceedence = graphical.SamplePairedData(.025) as PairedData;
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
                row.SetConfidenceLimits(Math.Round(lowerNonExceedence.f(probs[index]), 4), Math.Round(upperNonExceedence.f(probs[index]), 4));
            }
        }
        public GraphicalUncertainPairedData ToGraphicalUncertainPairedData()
        {
            CurveMetaData meta = new CurveMetaData(XLabel, YLabel,Name,"None",CurveTypesEnum.MonotonicallyIncreasing);
           return new GraphicalUncertainPairedData(((GraphicalDataProvider)SelectedItem).Xs, ((GraphicalDataProvider)SelectedItem).Ys , EquivalentRecordLength, meta, !UseFlow);
        }

        public bool Equals(FEGraphicalVM elem)
        {
            bool isEqual = true;

            if (!GraphicalUncertainPairedData.Equals(elem.GraphicalUncertainPairedData))
            {
                isEqual = false;
            }
            if (EquivalentRecordLength != elem.EquivalentRecordLength)
            {
                isEqual = false;
            }
            if (UseFlow != elem.UseFlow)
            {
                isEqual = false;
            }        
            return isEqual;
        }
    }
}
