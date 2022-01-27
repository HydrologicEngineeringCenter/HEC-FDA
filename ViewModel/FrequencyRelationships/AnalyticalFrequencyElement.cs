using Functions;
using Model;
using Statistics;
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
            double mean, double stDev, double skew, bool isLogFlow, List<double> analyticalFlows, List<double> graphicalFlows, IFdaFunction function) : base()
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

        public AnalyticalFrequencyElement(string xmlString)
        {
            XDocument doc = XDocument.Parse(xmlString);
            XElement flowFreqElem = doc.Element(FlowFrequencyPersistenceManager.FLOW_FREQUENCY);
            Name = (string)flowFreqElem.Attribute(FlowFrequencyPersistenceManager.NAME);
            Description = (string)flowFreqElem.Attribute(FlowFrequencyPersistenceManager.DESCRIPTION);
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

            //this is hacky but i need to set the curve property on this child element so that the curve editor can get the correct 
            //parameter type. I didn't want to mess with refactoring curve editor in any way because a lot of things use it.
            //This curve is not being used to display the plot or for saving. It can be filled with dummy data.
            //Cody 1/27/22
            ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(new List<double>() { 0, 1 }, new List<double>() { 0, 1 });
            Curve = IFdaFunctionFactory.Factory(IParameterEnum.OutflowFrequency, func);
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
            SaveUndoRedoHelper saveHelper = new SaveUndoRedoHelper(Saving.PersistenceFactory.GetFlowFrequencyManager()
                ,this, (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToEditor(editor, element),
                (editor, element) => AssignValuesFromEditorToElement(editor, element));
            //create action manager
            EditorActionManager actionManager = new EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
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
        public ChildElement CreateElementFromEditor(Editors.BaseEditorVM editorVM)
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
        public IDistribution GetDistribution()
        {
            return new Statistics.Distributions.LogPearson3(Mean, StDev, Skew, POR);
        }

    }
}
