using FdaViewModel.Editors;
using FdaViewModel.Utilities;
using Functions;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FdaViewModel.FrequencyRelationships
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
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);
            Utilities.NamedAction addRatingCurve = new Utilities.NamedAction();
            addRatingCurve.Header = "Create New Analyitical Flow Frequency Curve";
            addRatingCurve.Action = AddNewFlowFrequencyCurve;

            Utilities.NamedAction ImportRatingCurve = new Utilities.NamedAction();
            ImportRatingCurve.Header = "Import Analyitical Flow Frequency Curve From ASCII";
            ImportRatingCurve.Action = ImportRatingCurvefromAscii;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addRatingCurve);
            //localActions.Add(ImportRatingCurve);

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

        private void ImportRatingCurvefromAscii(object arg1, EventArgs arg2)
        {
            throw new NotImplementedException();
        }

        public void AddNewFlowFrequencyCurve(object arg1, EventArgs arg2)
        {

            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetFlowFrequencyManager()
                , (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToEditor(editor, element),
                (editor, element) => AssignValuesFromEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
               .WithSiblingRules(this);
            //.WithParentGuid(this.GUID)
            //.WithCanOpenMultipleTimes(true);
            List<double> xValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            List<double> yValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            Functions.ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            IFdaFunction defaultCurve = IFdaFunctionFactory.Factory( IParameterEnum.Rating, (IFunction)func);
            AnalyticalFrequencyEditorVM vm = new AnalyticalFrequencyEditorVM(defaultCurve, "Flow - Frequency", "Frequency", "Flow", actionManager);
            //LogPearsonIII curve = new Statistics.LogPearsonIII(4, .4, .5, 50);
            //Probabilities = 

            //Editors.AnalyticalFrequencyCurveEditor vm = new AnalyticalFrequencyCurveEditor(new UncertainCurveIncreasing(UncertainCurveDataCollection.DistributionsEnum.LogPearsonIII), actionManager);
            vm.Probabilities = new System.Collections.ObjectModel.ObservableCollection<double>() { .99, .95, .9, .8, .7, .6, .5, .4, .3, .2, .1, .05, .01 };
            //StudyCache.AddSiblingRules(vm, this);
            //vm.AddSiblingRules(this);
            string header = "Import Frequency";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportFrequency");
            Navigate(tab,false,false);
            //if (!vm.WasCanceled)
            //{
            //    if (!vm.HasError)
            //    {
            //        string creationDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

            //        Statistics.LogPearsonIII lpiii = new Statistics.LogPearsonIII(vm.Mean, vm.StandardDeviation, vm.Skew, vm.SampleSize);//are the default probabilities editable in the model?
            //        AnalyticalFrequencyElement afe = new AnalyticalFrequencyElement(vm.Name, creationDate, vm.Description, lpiii, this);
            //        AddElement(afe);
            //        AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(afe.Name, Utilities.Transactions.TransactionEnum.CreateNew, "Initial Name: " + afe.Name + " Description: " + afe.Description + " Mean: " + afe.Distribution.GetMean + " Standard Deviation: " + afe.Distribution.GetStDev + " Skew: " + afe.Distribution.GetG + " EYOR: " + afe.Distribution.GetSampleSize, nameof(AnalyticalFrequencyElement)));
            //    }
            //}
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

        public  ChildElement CreateElementFromEditor(Editors.BaseEditorVM editorVM)
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
            //return null;
        }
       
        #endregion
    }
}
