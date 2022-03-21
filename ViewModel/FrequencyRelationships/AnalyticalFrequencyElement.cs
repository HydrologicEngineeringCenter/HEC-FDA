using paireddata;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.TableWithPlot;
using System.Text;
using System.Windows;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyElement : CurveChildElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties  
        public int POR { get; set; }
        public bool IsAnalytical { get; set; }
        public bool IsStandard { get; set; }
        public double Mean { get; set; }
        public double StDev { get; set; }
        public double Skew { get; set; }
        public bool IsLogFlow { get; set; }
        public List<double> AnalyticalFlows { get; } = new List<double>();
        public List<double> GraphicalFlows { get; } = new List<double>();

        public UncertainPairedData PairedData { get; set; }

        #endregion
        #region Constructors
        public AnalyticalFrequencyElement(string name, string lastEditDate, string desc, int por, bool isAnalytical, bool isStandard,
            double mean, double stDev, double skew, bool isLogFlow, List<double> analyticalFlows, List<double> graphicalFlows, ComputeComponentVM function, int id) : base(id)
        {
            POR = por;
            IsAnalytical = isAnalytical;
            IsStandard = isStandard;
            Mean = mean;
            StDev = stDev;
            Skew = skew;
            IsLogFlow = isLogFlow;
            AnalyticalFlows = analyticalFlows;
            GraphicalFlows = graphicalFlows;
            LastEditDate = lastEditDate;
            Name = name;
            Description = desc;
            if (Description == null) Description = "";
            ComputeComponentVM = function;
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/FrequencyCurve.png");
            AddActions();
        }

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
            IsLogFlow = (bool)fitToFlowsElem.Attribute(FlowFrequencyPersistenceManager.IS_LOG);
            string flows = (string)fitToFlowsElem.Attribute(FlowFrequencyPersistenceManager.FLOWS);
            AnalyticalFlows = ConvertStringToFlows(flows);

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
            PairedData = UncertainPairedDataFactory.CreateLP3Data(lp3);

            ComputeComponentVM = new ComputeComponentVM();
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/FrequencyCurve.png");
            AddActions();
        }

        #endregion
        #region Voids
        private void AddActions()
        {
            NamedAction editflowfreq = new NamedAction();
            editflowfreq.Header = "Edit Analytical Flow Frequency Relationship...";
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
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetFlowFrequencyManager().Remove(this);
        }
        public void EditFlowFreq(object arg1, EventArgs arg2)
        {
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);

            AnalyticalFrequencyEditorVM vm = new AnalyticalFrequencyEditorVM(this,"Frequency", "Flow","Analytical Frequency", actionManager);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditAnalyticalFrequency" + vm.Name);
            Navigate(tab, false, false);
        }

        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            AnalyticalFrequencyElement elem = (AnalyticalFrequencyElement)elementToClone;
            return new AnalyticalFrequencyElement(elem.Name, elem.LastEditDate, elem.Description,elem.POR, elem.IsAnalytical, elem.IsStandard,
                elem.Mean, elem.StDev, elem.Skew, elem.IsLogFlow, elem.AnalyticalFlows, elem.GraphicalFlows, elem.ComputeComponentVM, elem.ID);
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
        public ContinuousDistribution GetDistribution()
        {
            return new LogPearson3(Mean, StDev, Skew, POR);
        }

    }
}
