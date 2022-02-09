using paireddata;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using ViewModel.Editors;
using ViewModel.Saving.PersistenceManagers;
using ViewModel.Utilities;

namespace ViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyElement : ChildElement
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

        #endregion
        #region Constructors
        public AnalyticalFrequencyElement(string name, string lastEditDate, string desc, int por, bool isAnalytical, bool isStandard,
            double mean, double stDev, double skew, bool isLogFlow, List<double> analyticalFlows, List<double> graphicalFlows, UncertainPairedData function) : base()
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
            Curve = function; 
            
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/FrequencyCurve.png");
            AddActions();
        }

        public AnalyticalFrequencyElement(string name, string description, string xmlString)
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
            Curve = UncertainPairedDataFactory.CreateLP3Data(lp3);

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
            //create save helper
            SaveHelper saveHelper = new SaveHelper(Saving.PersistenceFactory.GetFlowFrequencyManager()
                ,this, (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToEditor(editor, element),
                (editor, element) => AssignValuesFromEditorToElement(editor, element));
            //create action manager
            EditorActionManager actionManager = new EditorActionManager()
                .WithSaveHelper(saveHelper)
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
                elem.Mean, elem.StDev, elem.Skew, elem.IsLogFlow, elem.AnalyticalFlows, elem.GraphicalFlows, elem.Curve);
        }

        public void AssignValuesFromEditorToElement(BaseEditorVM editorVM, ChildElement elem)
        {
            AnalyticalFrequencyEditorVM vm = (AnalyticalFrequencyEditorVM)editorVM;
            AnalyticalFrequencyElement element = (AnalyticalFrequencyElement)elem;
            element.Name = vm.Name;
            element.Description = vm.Description;
            element.Curve = vm.Curve;
            element.UpdateTreeViewHeader(vm.Name);
        }

        public void AssignValuesFromElementToEditor(BaseEditorVM editorVM, ChildElement elem)
        {
            AnalyticalFrequencyEditorVM vm = (AnalyticalFrequencyEditorVM)editorVM;
            AnalyticalFrequencyElement element = (AnalyticalFrequencyElement)elem;

            vm.Name = element.Name;
            vm.Description = element.Description;
            vm.LastEditDate = element.LastEditDate;
            vm.PeriodOfRecord = element.POR;
            vm.IsAnalytical = element.IsAnalytical;
            vm.IsStandard = element.IsStandard;
            vm.Mean = element.Mean;
            vm.StandardDeviation = element.StDev;
            vm.Skew = element.Skew;
            vm.IsLogFlow = element.IsLogFlow;
            vm.AnalyticalFlows = ConvertDoublesToFlowWrappers(element.AnalyticalFlows);
            vm.GraphicalFlows =  ConvertDoublesToFlowWrappers(element.GraphicalFlows);
        }

        private ObservableCollection<FlowDoubleWrapper> ConvertDoublesToFlowWrappers(List<double> flows)
        {
            ObservableCollection<FlowDoubleWrapper> flowWrappers = new ObservableCollection<FlowDoubleWrapper>();
            if (flows != null)
            {
                foreach (double d in flows)
                {
                    flowWrappers.Add(new FlowDoubleWrapper(d));
                }
            }
            return flowWrappers;
        }
        public ChildElement CreateElementFromEditor(BaseEditorVM editorVM)
        {
            //will be formatted like: 2/27/2009 12:12:22 PM
            string editDate = DateTime.Now.ToString("G"); 
            AnalyticalFrequencyEditorVM vm = (AnalyticalFrequencyEditorVM)editorVM;
            double mean = vm.Mean;
            double stDev = vm.StandardDeviation;
            double skew = vm.Skew;
            int por = vm.PeriodOfRecord;
            bool isAnalytical = vm.IsAnalytical;
            bool isStandard = vm.IsStandard;
            bool isLogFlow = vm.IsLogFlow;
            List<double> analyticalFlows = new List<double>();
            foreach (FlowDoubleWrapper d in vm.AnalyticalFlows)
            {
                analyticalFlows.Add(d.Flow);
            }
            List<double> graphicalFlows = new List<double>();
            foreach (FlowDoubleWrapper d in vm.GraphicalFlows)
            {
                graphicalFlows.Add(d.Flow);
            }
            return new AnalyticalFrequencyElement(editorVM.Name, editDate, editorVM.Description, por, isAnalytical, isStandard, mean, stDev, skew,
                isLogFlow, analyticalFlows, graphicalFlows, vm.CreateFdaFunction());
        }
        
        public override bool Equals(object obj)
        {
            bool retval = true;
            if (obj.GetType() == typeof(AnalyticalFrequencyElement))
            {
                AnalyticalFrequencyElement elem = (AnalyticalFrequencyElement)obj;
                if (!Name.Equals(elem.Name))
                {
                    retval = false;
                }
                if(Description == null)
                {
                    Description = "";
                }
                if (!Description.Equals(elem.Description))
                {
                    retval = false;
                }
                if (!LastEditDate.Equals(elem.LastEditDate))
                {
                    retval = false;
                }
            }
            else
            {
                retval = false;
            }
            return retval;
        }

        #endregion

        List<double> ConvertStringToFlows(string flows)
        {
            List<double> flowDoubles = new List<double>();
            try
            {
                string[] flowStrings = flows.Split(',');

                foreach (string flow in flowStrings)
                {
                    double d = Convert.ToDouble(flow);
                    flowDoubles.Add(d);
                }
            }
            catch (Exception e)
            {
                //couldn't convert to doubles
            }
            return flowDoubles;
        }
        public ContinuousDistribution GetDistribution()
        {
            return new LogPearson3(Mean, StDev, Skew, POR);
        }

    }
}
