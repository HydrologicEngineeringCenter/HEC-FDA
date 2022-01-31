using Functions;
using Model;
using System;
using System.Collections.Generic;
using ViewModel.Utilities;

namespace ViewModel.FlowTransforms
{
    public class InflowOutflowOwnerElement : ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties

        #endregion
        #region Constructors
        public InflowOutflowOwnerElement( ) : base()
        {
            Name = "Inflow Outflow Relationships";
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addInflowOutflow = new NamedAction();
            addInflowOutflow.Header = "Create New Inflow Outflow Relationship...";
            addInflowOutflow.Action = AddInflowOutflow;

            NamedAction importInflowOutflow = new NamedAction();
            importInflowOutflow.Header = StringConstants.ImportFromOldFda("Inflow-Outflow");
            importInflowOutflow.Action = ImportInflowOutflow;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addInflowOutflow);
            localActions.Add(importInflowOutflow);

            Actions = localActions;

            StudyCache.InflowOutflowAdded += AddInflowOutflowElement;
            StudyCache.InflowOutflowRemoved += RemoveInflowOutflowElement;
            StudyCache.InflowOutflowUpdated += UpdateInflowOutflowElement;
        }
        #endregion
        #region Voids
        private void UpdateInflowOutflowElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddInflowOutflowElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveInflowOutflowElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }

        public void ImportInflowOutflow(object arg1, EventArgs arg2)
        {
            ImportFromFDA1VM vm = new ImportInflowOutflowFromFDA1VM();
            string header = "Import Inflow Outflow Curves";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportInflowOutflowCurve");
            Navigate(tab, false, true);
        }

        public void AddInflowOutflow(object arg1, EventArgs arg2)
        {
            //List<double> xValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            //List<double> yValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            //Functions.ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            //IFdaFunction defaultCurve = IFdaFunctionFactory.Factory( IParameterEnum.Rating, (IFunction)func);

            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetInflowOutflowManager()
                , (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                .WithSiblingRules(this);

            //.WithParentGuid(this.GUID)
            //.WithCanOpenMultipleTimes(true);
            UncertainPairedData defaultCurve = Utilities.DefaultPairedData.CreateDefaultNormalUncertainPairedData("Inflow", "Outflow", "Inflow-Outflow");
            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(defaultCurve, "Inflow", "Outflow", "Inflow - Outflow", actionManager);

            string title = "Create Inflow Outflow";
            DynamicTabVM tab = new DynamicTabVM(title, vm, "NewInflowOutflow" + Name);
            Navigate( tab, false, false);
        }
      
        #endregion
        #region Functions  
        public  ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;

            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new InflowOutflowElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve);
        }
        #endregion
    }
}
