using Model;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ViewModel.Editors;
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
        public List<double> AnalyticalFlows { get; set; }
        public List<double> GraphicalFlows { get; set; }

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
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/FrequencyCurve.png");

            Description = desc;
            if (Description == null) Description = "";
            Curve = function;
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


        #endregion
        #region Voids
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetFlowFrequencyManager().Remove(this);
        }
        public void EditFlowFreq(object arg1, EventArgs arg2)
        {

            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new SaveUndoRedoHelper(Saving.PersistenceFactory.GetFlowFrequencyManager()
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
            foreach(double d in flows)
            {
                flowWrappers.Add(new FlowDoubleWrapper(d));
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

        public IDistribution GetDistribution()
        {
            return new Statistics.Distributions.LogPearson3(Mean, StDev, Skew, POR);
        }

    }
}
