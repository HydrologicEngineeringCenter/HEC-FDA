using paireddata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyOwnerElement : ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public AnalyticalFrequencyOwnerElement() : base()
        {
            Name = "Analyitical Flow Frequency Curves";
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);
            NamedAction createNew = new NamedAction();
            createNew.Header = "Create New Analyitical Flow Frequency Curve...";
            createNew.Action = AddNewFlowFrequencyCurve;

            NamedAction importFlowFreq = new NamedAction();
            importFlowFreq.Header = StringConstants.ImportFromOldFda("Analyitical Flow Frequency");
            importFlowFreq.Action = ImportFlowFreqFromAscii;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(createNew);
            localActions.Add(importFlowFreq);

            Actions = localActions;

            StudyCache.FlowFrequencyAdded += AddFlowFrequencyElement;
            StudyCache.FlowFrequencyRemoved += RemoveFlowFrequencyElement;
            StudyCache.FlowFrequencyUpdated += UpdateFlowFrequencyElement;
        }

        #endregion
        #region Voids
        private void UpdateFlowFrequencyElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void RemoveFlowFrequencyElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void AddFlowFrequencyElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }

        private void ImportFlowFreqFromAscii(object arg1, EventArgs arg2)
        {
            ImportFromFDA1VM vm = new ImportFrequencyFromFDA1VM();
            string header = "Import Frequency Curve";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportFrequencyCurve");
            Navigate(tab, false, true);
        }

        public void AddNewFlowFrequencyCurve(object arg1, EventArgs arg2)
        {
            //create save helper
            SaveHelper saveHelper = new SaveHelper(Saving.PersistenceFactory.GetFlowFrequencyManager()
                , (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToEditor(editor, element),
                (editor, element) => AssignValuesFromEditorToElement(editor, element));
            //create action manager
            EditorActionManager actionManager = new EditorActionManager()
                .WithSaveHelper(saveHelper)
               .WithSiblingRules(this);

            List<double> xs = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            List<double> ys = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            
            UncertainPairedData defaultCurve = UncertainPairedDataFactory.CreateDeterminateData(xs, ys,"Flow", "Frequency", "Flow-Frequency Function");

            AnalyticalFrequencyEditorVM vm = new AnalyticalFrequencyEditorVM(defaultCurve, "Flow - Frequency", "Frequency", "Flow", actionManager);
            string header = "Import Frequency";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportFrequency");
            Navigate(tab,false,false);
        }  
        #endregion
        #region Functions
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
            vm.Curve = element.Curve;
            vm.PeriodOfRecord = element.POR;
            vm.IsAnalytical = element.IsAnalytical;
            vm.IsStandard = element.IsStandard;
            vm.Mean = element.Mean;
            vm.StandardDeviation = element.StDev;
            vm.Skew = element.Skew;
            vm.IsLogFlow = element.IsLogFlow;

            ObservableCollection<FlowDoubleWrapper> flows = new ObservableCollection<FlowDoubleWrapper>();
            foreach(double d in element.AnalyticalFlows)
            {
                flows.Add(new FlowDoubleWrapper(d));
            }
            vm.AnalyticalFlows = flows;

            ObservableCollection<FlowDoubleWrapper> graphicalFlows = new ObservableCollection<FlowDoubleWrapper>();
            foreach (double d in element.GraphicalFlows)
            {
                graphicalFlows.Add(new FlowDoubleWrapper(d));
            }
            vm.GraphicalFlows = graphicalFlows;         
        }

        public  ChildElement CreateElementFromEditor(BaseEditorVM editorVM)
        {
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            AnalyticalFrequencyEditorVM vm = (AnalyticalFrequencyEditorVM)editorVM;
            double mean = vm.Mean;
            double stDev = vm.StandardDeviation;
            double skew = vm.Skew;
            int por = vm.PeriodOfRecord;
            bool isAnalytical = vm.IsAnalytical;
            bool isStandard = vm.IsStandard;
            bool isLogFlow = vm.IsLogFlow;
            List<double> analyticalFlows = new List<double>();
            foreach(FlowDoubleWrapper d in vm.AnalyticalFlows)
            {
                analyticalFlows.Add(d.Flow);
            }
            List<double> graphicalFlows = new List<double>();
            foreach(FlowDoubleWrapper d in vm.GraphicalFlows)
            {
                graphicalFlows.Add(d.Flow);
            }

            return new AnalyticalFrequencyElement(editorVM.Name, editDate, editorVM.Description,por, isAnalytical, isStandard,mean,stDev,skew,
                isLogFlow,analyticalFlows, graphicalFlows, vm.CreateFdaFunction());
        }    
        #endregion
    }
}
