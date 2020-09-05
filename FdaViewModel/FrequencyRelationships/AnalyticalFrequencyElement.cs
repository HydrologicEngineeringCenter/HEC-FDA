using FdaViewModel.Editors;
using FdaViewModel.Utilities;
using Functions;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyElement : Utilities.ChildElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableConstant = "Analytical Frequency - ";

        private IFdaFunction _Distribution;
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

        //public IFdaFunction Distribution
        //{
        //    get { return _Distribution; }
        //    set { _Distribution = value; NotifyPropertyChanged(); }
        //}
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
            editflowfreq.Header = "Edit Analytical Flow Frequency Relationship";
            editflowfreq.Action = EditFlowFreq;

            NamedAction removeflowfreq = new NamedAction();
            removeflowfreq.Header = "Remove";
            removeflowfreq.Action = RemoveElement;

            NamedAction renameElement = new NamedAction();
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(editflowfreq);
            localActions.Add(removeflowfreq);
            localActions.Add(renameElement);

            Actions = localActions;

            //todo: i should create a curve and set the curve object so that the sci chart will have something to show.


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
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetFlowFrequencyManager()
                ,this, (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToEditor(editor, element),
                (editor, element) => AssignValuesFromEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                .WithSiblingRules(this);
               //.WithParentGuid(this.GUID)
               //.WithCanOpenMultipleTimes(false);

            AnalyticalFrequencyEditorVM vm = new AnalyticalFrequencyEditorVM(this,"Frequency", "Flow","Analytical Frequency", actionManager);// Name, Distribution, Description, _Owner);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditAnalyticalFrequency" + vm.Name);
            Navigate(tab, false, false);
            //if (!vm.WasCanceled)
            //{
            //    if (!vm.HasError)
            //    {
            //        vm.SaveWhileEditing();
            //    }
            //}
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
            //vm.Curve = element.Curve;
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
        //public override void Save()
        //{
        //    throw new NotImplementedException();
        //}

        //public override object[] RowData()
        //{
        //    return new object[] { Name, LastEditDate, Description, Distribution.GetMean, Distribution.GetStDev, Distribution.GetG, Distribution.GetSampleSize };
        //}

        //public override bool SavesToRow()
        //{
        //    return true;
        //}
        //public override bool SavesToTable()
        //{
        //    return false;
        //}
        //#endregion
        //#region Functions
        //public override string TableName
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

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
                //todo: Refactor: Commented Out
                //if (!Double.Equals(Distribution.GetMean, elem.Distribution.GetMean))
                //{
                //    retval = false;
                //}
                //if (!Double.Equals(Distribution.GetStDev, elem.Distribution.GetStDev))
                //{
                //    retval = false;
                //}
                //if (!Double.Equals(Distribution.GetG, elem.Distribution.GetG))
                //{
                //    retval = false;
                //}
            }
            else
            {
                retval = false;
            }
            return retval;
        }

        #endregion


        //todo i think i need a way to set the curve property on this so that the base classes work correctly.
        //private ICoordinatesFunction CreateFunction()
        //{
        //    if(IsAnalytical)
        //    {
        //        if(IsStandard)
        //        {

        //        }
        //    }
        //}
    }
}
