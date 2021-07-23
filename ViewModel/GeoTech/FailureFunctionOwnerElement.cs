using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Utilities;
using ViewModel.Editors;
using System.Collections.ObjectModel;
using Model;
using Functions;

namespace ViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 2:00:01 PM)]
    public class FailureFunctionOwnerElement : Utilities.ParentElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 2:00:01 PM
        #endregion
        #region Fields
        #endregion
        #region Properties
        
        #endregion
        #region Constructors
        public FailureFunctionOwnerElement( ):base()
        {
            Name = "Failure Functions";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addFailureFunction = new Utilities.NamedAction();
            addFailureFunction.Header = "Create New Failure Function Curve";
            addFailureFunction.Action = AddNewFailureFunction;

            //Utilities.NamedAction ImportRatingCurve = new Utilities.NamedAction();
            //ImportRatingCurve.Header = "Import Rating Curve From ASCII";
            //ImportRatingCurve.Action = ImportRatingCurvefromAscii;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addFailureFunction);
            //localActions.Add(ImportRatingCurve);

            Actions = localActions;

            StudyCache.FailureFunctionAdded += AddFailureFunctionElement;
            StudyCache.FailureFunctionRemoved += RemoveFailureFunctionElement;
            StudyCache.FailureFunctionUpdated += UpdateFailureFunctionElement;
        }
        #endregion
        #region Voids
        private void UpdateFailureFunctionElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddFailureFunctionElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveFailureFunctionElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        public void AddNewFailureFunction(object arg1, EventArgs arg2)
        {
            List<LeveeFeatureElement> leveeList = StudyCache.GetChildElementsOfType<LeveeFeatureElement>();
            ObservableCollection<LeveeFeatureElement> leveeCollection = new ObservableCollection<LeveeFeatureElement>();
            foreach(LeveeFeatureElement elem in leveeList)
            {
                leveeCollection.Add(elem);
            }

            //double[] xValues = new double[] { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            //Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] { new Statistics.None(95), new Statistics.None(96), new Statistics.None(97), new Statistics.None(99), new Statistics.None(104), new Statistics.None(109), new Statistics.None(110), new Statistics.None(114), new Statistics.None(116), new Statistics.None(119), new Statistics.None(120), new Statistics.None(121) };
            //Statistics.UncertainCurveIncreasing defaultCurve = new Statistics.UncertainCurveIncreasing(xValues, yValues, true, true, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            List<double> xValues = new List<double>() { 0,.2,.4,.6,.8,1 };
            List<double> yValues = new List<double>() { 1,2,3,4,5,6};
            Functions.ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            IFdaFunction defaultCurve = IFdaFunctionFactory.Factory( IParameterEnum.Rating, (IFunction)func);

            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetFailureFunctionManager()
                , (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToEditor(editor, element),
                (editor, element) => AssignValuesFromEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                 .WithSiblingRules(this);
               //.WithParentGuid(this.GUID)
               //.WithCanOpenMultipleTimes(true);

            Editors.CurveEditorVM vm = new Editors.FailureFunctionCurveEditorVM(defaultCurve, leveeCollection, "Frequency", "Stage", "Failure Function", actionManager);
            string header = "Create Failure Function";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "NewFailureFunction");
            Navigate(tab, false, false );
            //FailureFunctionEditorVM vm = new FailureFunctionEditorVM((foo) => SaveNewElement(foo), (bar) => AddOwnerRules(bar), leveeList);
            //if (!vm.WasCanceled)
            //{
            //    if (!vm.HasError)
            //    {
            //        //string creationDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

            //        //FailureFunctionElement ele = new FailureFunctionElement(vm.Name, creationDate, vm.Description, vm.Curve, vm.SelectedLateralStructure, this);
            //        //AddElement(ele);
            //        //AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(ele.Name, Utilities.Transactions.TransactionEnum.CreateNew, "", nameof(FailureFunctionElement)));
            //        vm.SaveWhileEditing();

            //    }
            //}
        }
        #endregion
        #region Functions
        #endregion
       
      

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

     

        //public override void AssignValuesFromCurveEditorToElement(BaseEditorVM editorVM, ChildElement element)
        //{
        //    FailureFunctionCurveEditorVM vm = (FailureFunctionCurveEditorVM)editorVM;
        //    FailureFunctionElement elem = (FailureFunctionElement)element;

        //    elem.Name = vm.Name;
        //    elem.Description = vm.Description;
        //    elem.Curve = vm.Curve;
        //    elem.SelectedLateralStructure = vm.SelectedLateralStructure;
            

        //    elem.UpdateTreeViewHeader(vm.Name);
        //}

        //public override void AssignValuesFromElementToCurveEditor(BaseEditorVM editorVM, ChildElement element)
        //{
        //    FailureFunctionCurveEditorVM vm = (FailureFunctionCurveEditorVM)editorVM;
        //    FailureFunctionElement elem = (FailureFunctionElement)element;


        //    vm.Name = element.Name;
        //    vm.Description = element.Description;
        //    vm.Curve = element.Curve;
        //    vm.SelectedLateralStructure = elem.SelectedLateralStructure;
        //}

        public  ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.FailureFunctionCurveEditorVM editorVM = (Editors.FailureFunctionCurveEditorVM)vm;

            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new FailureFunctionElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve, editorVM.SelectedLateralStructure);
            //return null;
        }

        public void AssignValuesFromEditorToElement(BaseEditorVM editorVM, ChildElement elem)
        {
            FailureFunctionCurveEditorVM vm = (FailureFunctionCurveEditorVM)editorVM;
            FailureFunctionElement element = (FailureFunctionElement)elem;

            element.Name = vm.Name;
            element.Description = vm.Description;
            element.Curve = vm.Curve;
            element.SelectedLateralStructure = vm.SelectedLateralStructure;
            element.UpdateTreeViewHeader(vm.Name);
        }

        public void AssignValuesFromElementToEditor(BaseEditorVM editorVM, ChildElement elem)
        {
            FailureFunctionCurveEditorVM vm = (FailureFunctionCurveEditorVM)editorVM;
            FailureFunctionElement element = (FailureFunctionElement)elem;

            vm.Name = element.Name;
            vm.Description = element.Description;
            vm.Curve = element.Curve;
            if(vm.EditorVM != null)
            {
                ICoordinatesFunction coordFunc = ICoordinatesFunctionsFactory.Factory(element.Curve.Coordinates, element.Curve.Interpolator);
                vm.EditorVM.Function =  coordFunc;
            }
            vm.SelectedLateralStructure = element.SelectedLateralStructure;
        }

        //public override void AddElementFromRowData(object[] rowData)
        //{
        //    AddElement(CreateElementFromRowData(rowData), false);
        //}
    }
}
