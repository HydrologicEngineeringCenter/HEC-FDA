using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class FrequencyElement : CurveChildElement
    {
        #region Notes
        #endregion

        private const string IS_ANALYTICAL = "IsAnalytical";
        private const string ANALYTICAL_DATA = "AnalyticalData";
        private const string USES_MOMENTS = "UsesMoments";
        private const string POR_XML_TAG = "POR";
        private const string MOMENTS = "Moments";
        private const string MEAN = "Mean";
        private const string ST_DEV = "StDev";
        private const string SKEW = "Skew";
        private const string FIT_TO_FLOWS = "FitToFlows";
        private const string FLOWS = "Flows";

        #region Properties  
        public int POR { get; set; }
        public bool IsAnalytical { get; set; }
        public bool IsStandard { get; set; }
        public double Mean { get; set; }
        public double StDev { get; set; }
        public double Skew { get; set; }
        public List<double> AnalyticalFlows { get; } = new List<double>();
        public UncertainPairedData PairedData { get; set; }
        public GraphicalVM MyGraphicalVM { get; set; }

        #endregion
        #region Constructors
        //fresh editor
        public FrequencyElement(string name, string lastEditDate, string desc, int por, bool isAnalytical, bool isStandard,
            double mean, double stDev, double skew, List<double> analyticalFlows, GraphicalVM graphicalVM, CurveComponentVM function, int id) 
            : base(name, lastEditDate, desc, function, id)
        {
            POR = por;
            IsAnalytical = isAnalytical;
            IsStandard = isStandard;
            Mean = mean;
            StDev = stDev;
            Skew = skew;
            AnalyticalFlows = analyticalFlows;

            MyGraphicalVM = graphicalVM;

            PairedData = CreatePairedData();
            AddDefaultActions(EditFlowFreq, StringConstants.EDIT_FREQUENCY_FUNCTIONS_MENU);
        }
        //load from database
        public FrequencyElement(XElement flowFreqElem, int id) : base(flowFreqElem, id)
        {            
            ReadHeaderXElement(flowFreqElem.Element(HEADER_XML_TAG));          

            IsAnalytical = (bool)flowFreqElem.Attribute(IS_ANALYTICAL);

            XElement analyticalElem = flowFreqElem.Element(ANALYTICAL_DATA);
            IsStandard = (bool)analyticalElem.Attribute(USES_MOMENTS);
            POR = (int)analyticalElem.Attribute(POR_XML_TAG);

            XElement momentsElem = analyticalElem.Element(MOMENTS);
            Mean = (double)momentsElem.Attribute(MEAN);
            StDev = (double)momentsElem.Attribute(ST_DEV);
            Skew = (double)momentsElem.Attribute(SKEW);

            XElement fitToFlowsElem = analyticalElem.Element(FIT_TO_FLOWS);
            string flows = (string)fitToFlowsElem.Attribute(FLOWS);
            if (!String.IsNullOrEmpty(flows))
            {
                AnalyticalFlows = ConvertStringToFlows(flows);
            }

            CurveComponentVM = CurveComponentVM.CreateCurveComponentVM(flowFreqElem); 

            XElement graphiclVMele = flowFreqElem.Element("GraphicalVM");
            if(graphiclVMele != null)
            {
                MyGraphicalVM = new GraphicalVM(graphiclVMele);
            }

            PairedData = CreatePairedData();

            AddDefaultActions(EditFlowFreq, StringConstants.EDIT_FREQUENCY_FUNCTIONS_MENU);
        }

        #endregion
        #region Voids


        public void EditFlowFreq(object arg1, EventArgs arg2)
        {
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);

            FrequencyEditorVM vm = new FrequencyEditorVM(this, actionManager);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditAnalyticalFrequency" + vm.Name);
            Navigate(tab, false, false);
        }
   
        #endregion

        List<double> ConvertStringToFlows(string flows)
        {
            List<double> flowDoubles = new List<double>();
            List<string> badStrings = new List<string>();
            
            string[] flowStrings = flows.Split(',');

            foreach (string flow in flowStrings)
            {
                try
                {
                    double d = Convert.ToDouble(flow);
                    flowDoubles.Add(d);
                }
                catch (Exception e)
                {
                    badStrings.Add(flow);
                }
            }
            if (badStrings.Count > 0)
            {
                //todo: send this to log file? Can't have a message box when unit testing.

                //string msg = "An error occured while creating the frequency relationship '" + Name + "'." + Environment.NewLine +
                //    "The following flow texts were not able to be converted to numeric values: ";
                //string errorVals = string.Join("\n\t", badStrings);
                //MessageBox.Show(msg + "\n\t" + errorVals, "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return flowDoubles;           
        }

        public LogPearson3 CreateAnalyticalLP3Distribution()
        {
            LogPearson3 lp3 = new LogPearson3();
            if (IsStandard)
            {
                lp3 = new LogPearson3(Mean, StDev, Skew, POR);
            }
            else
            {
                //this is fit to flow
                lp3 = (LogPearson3)lp3.Fit(AnalyticalFlows.ToArray());
            }
            return lp3;
        }

        private UncertainPairedData CreateGraphicalPairedData()
        {
            GraphicalUncertainPairedData graphicalData = MyGraphicalVM.ToGraphicalUncertainPairedData();

            ContinuousDistribution[] stageOrLogFlowDistributions = graphicalData.StageOrLogFlowDistributions;
            double[] exceedanceProbs = graphicalData.ExceedanceProbabilities;

            List<double> xs = new List<double>();
            foreach(double exceed in exceedanceProbs)
            {
                xs.Add(1-exceed);
            }

            List<double> ys = new List<double>();
            foreach(ContinuousDistribution cont in stageOrLogFlowDistributions)
            {
                ys.Add( cont.InverseCDF(.5));
            }

            return UncertainPairedDataFactory.CreateDeterminateData(xs.ToArray(), ys.ToArray(), graphicalData.XLabel, graphicalData.YLabel, graphicalData.Name);
        }

        public UncertainPairedData CreatePairedData()
        {
            UncertainPairedData frequencyData = null;
            if (IsAnalytical)
            {
                LogPearson3 lp3 = CreateAnalyticalLP3Distribution();
                frequencyData = UncertainPairedDataFactory.CreateLP3Data(lp3);
            }
            else
            {
                frequencyData = CreateGraphicalPairedData();
            }
            return frequencyData;
        }

        public override XElement ToXML()
        {
            XElement flowFreqElem = base.ToXML();

            flowFreqElem.SetAttributeValue(IS_ANALYTICAL, IsAnalytical);

            XElement analyticalElem = new XElement(ANALYTICAL_DATA);
            flowFreqElem.Add(analyticalElem);
            analyticalElem.SetAttributeValue(USES_MOMENTS, IsStandard);
            analyticalElem.SetAttributeValue(POR_XML_TAG, POR);

            XElement momentsElem = new XElement(MOMENTS);
            analyticalElem.Add(momentsElem);
            momentsElem.SetAttributeValue(MEAN, Mean);
            momentsElem.SetAttributeValue(ST_DEV, StDev);
            momentsElem.SetAttributeValue(SKEW, Skew);

            XElement fitToFlowsElem = new XElement(FIT_TO_FLOWS);
            analyticalElem.Add(fitToFlowsElem);
            fitToFlowsElem.SetAttributeValue(FLOWS, ConvertFlowsToString(AnalyticalFlows));

            XElement graphicalElem = MyGraphicalVM.ToXML();
            flowFreqElem.Add(graphicalElem);

            return flowFreqElem;
        }

        private string ConvertFlowsToString(List<double> flows)
        {
            if (flows.Count == 0)
            {
                return "";
            }
            else
            {
                return string.Join(",",flows);
            }           
        }

        public bool Equals(FrequencyElement elem)
        {
            bool isEqual = true;

            if (!AreHeaderDataEqual(elem))
            {
                isEqual = false;
            }
            if (POR != elem.POR)
            {
                isEqual = false;
            }
            if (IsAnalytical != elem.IsAnalytical)
            {
                isEqual = false;
            }
            if (IsStandard != elem.IsStandard)
            {
                isEqual = false;
            }
            if (Mean != elem.Mean)
            {
                isEqual = false;
            }
            if (StDev != elem.StDev)
            {
                isEqual = false;
            }
            if (Skew != elem.Skew)
            {
                isEqual = false;
            }
            if (AnalyticalFlows.Count != elem.AnalyticalFlows.Count)
            {
                isEqual = false;
            }
            for(int i = 0; i < elem.AnalyticalFlows.Count; i++)
            {
                if(AnalyticalFlows[i] != elem.AnalyticalFlows[i])
                {
                    isEqual = false;
                    break;
                }
            }

            if(!PairedData.Equals(elem.PairedData))
            {
                isEqual=false;
            }

            if(!MyGraphicalVM.Equals(elem.MyGraphicalVM))
            {
                isEqual = false;
            }

            return isEqual;
        }

    }
}
