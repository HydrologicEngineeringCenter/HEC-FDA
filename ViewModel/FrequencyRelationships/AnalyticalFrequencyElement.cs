using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using paireddata;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyElement : CurveChildElement
    {
        #region Notes
        #endregion

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
        public AnalyticalFrequencyElement(string name, string lastEditDate, string desc, int por, bool isAnalytical, bool isStandard,
            double mean, double stDev, double skew, List<double> analyticalFlows, GraphicalVM graphicalVM, ComputeComponentVM function, int id) : base(id)
        {
            POR = por;
            IsAnalytical = isAnalytical;
            IsStandard = isStandard;
            Mean = mean;
            StDev = stDev;
            Skew = skew;
            AnalyticalFlows = analyticalFlows;
            LastEditDate = lastEditDate;
            Name = name;
            Description = desc;
            if (Description == null) Description = "";
            ComputeComponentVM = function;
            MyGraphicalVM = graphicalVM;
            CustomTreeViewHeader = new CustomHeaderVM(Name)
            {
                ImageSource = ImageSources.FREQUENCY_IMAGE,
                Tooltip = StringConstants.CreateLastEditTooltip(lastEditDate)
            };

            AddActions();
        }
        //load from database
        public AnalyticalFrequencyElement(string name, string description, string xmlString, int id) : base(id)
        {
            XDocument doc = XDocument.Parse(xmlString);
            XElement flowFreqElem = doc.Element(FlowFrequencyPersistenceManager.FLOW_FREQUENCY);
            Name = name;
            Description = description;
            LastEditDate = (string)flowFreqElem.Attribute(FlowFrequencyPersistenceManager.LAST_EDIT_DATE);
            IsAnalytical = (bool)flowFreqElem.Attribute(FlowFrequencyPersistenceManager.IS_ANALYTICAL);

            XElement analyticalElem = flowFreqElem.Element(FlowFrequencyPersistenceManager.ANALYTICAL_DATA);
            IsStandard = (bool)analyticalElem.Attribute(FlowFrequencyPersistenceManager.USES_MOMENTS);
            POR = (int)analyticalElem.Attribute(FlowFrequencyPersistenceManager.POR);

            XElement momentsElem = analyticalElem.Element(FlowFrequencyPersistenceManager.MOMENTS);
            Mean = (double)momentsElem.Attribute(FlowFrequencyPersistenceManager.MEAN);
            StDev = (double)momentsElem.Attribute(FlowFrequencyPersistenceManager.ST_DEV);
            Skew = (double)momentsElem.Attribute(FlowFrequencyPersistenceManager.SKEW);

            XElement fitToFlowsElem = analyticalElem.Element(FlowFrequencyPersistenceManager.FIT_TO_FLOWS);
            string flows = (string)fitToFlowsElem.Attribute(FlowFrequencyPersistenceManager.FLOWS);
            if (!String.IsNullOrEmpty(flows))
            {
                AnalyticalFlows = ConvertStringToFlows(flows);
            }


            ComputeComponentVM = new ComputeComponentVM(StringConstants.ANALYTICAL_FREQUENCY, StringConstants.EXCEEDANCE_PROBABILITY, StringConstants.DISCHARGE);
            XElement graphiclVMele = flowFreqElem.Element("GraphicalVM");
            if(graphiclVMele != null)
            {
                MyGraphicalVM = new GraphicalVM(graphiclVMele);
            }
            CustomTreeViewHeader = new CustomHeaderVM(Name)
            {
                ImageSource = ImageSources.FREQUENCY_IMAGE,
                Tooltip = StringConstants.CreateLastEditTooltip(LastEditDate)
            };

            PairedData = CreatePairedData();

            AddActions();
        }

        #endregion
        #region Voids
        private void AddActions()
        {
            NamedAction editflowfreq = new NamedAction();
            editflowfreq.Header = StringConstants.EDIT_FREQUENCY_FUNCTIONS_MENU;
            editflowfreq.Action = EditFlowFreq;

            NamedAction removeflowfreq = new NamedAction();
            removeflowfreq.Header = StringConstants.REMOVE_MENU;
            removeflowfreq.Action = RemoveElement;

            NamedAction renameElement = new NamedAction();
            renameElement.Header = StringConstants.RENAME_MENU;
            renameElement.Action = Rename;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(editflowfreq);
            localActions.Add(removeflowfreq);
            localActions.Add(renameElement);

            Actions = localActions;
        }

        public void EditFlowFreq(object arg1, EventArgs arg2)
        {
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);

            AnalyticalFrequencyEditorVM vm = new AnalyticalFrequencyEditorVM(this, actionManager);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditAnalyticalFrequency" + vm.Name);
            Navigate(tab, false, false);
        }

        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            AnalyticalFrequencyElement elem = (AnalyticalFrequencyElement)elementToClone;
            return new AnalyticalFrequencyElement(elem.Name, elem.LastEditDate, elem.Description,elem.POR, elem.IsAnalytical, elem.IsStandard,
                elem.Mean, elem.StDev, elem.Skew, elem.AnalyticalFlows, elem.MyGraphicalVM, elem.ComputeComponentVM, elem.ID);
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
            if(badStrings.Count > 0)
            {

                string msg = "An error occured while creating the frequency relationship '" + Name + "'." + Environment.NewLine +
                    "The following flow texts were not able to be converted to numeric values: ";
                string errorVals = string.Join(Environment.NewLine + "\t",badStrings);              
                MessageBox.Show(msg + Environment.NewLine + "\t" + errorVals, "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

    }
}
