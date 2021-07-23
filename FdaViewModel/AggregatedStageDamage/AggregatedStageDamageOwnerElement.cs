using FdaViewModel.Editors;
using FdaViewModel.Utilities;
using Functions;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageOwnerElement : Utilities.ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        
        #endregion
        #region Constructors
        public AggregatedStageDamageOwnerElement() : base()
        {
            Name = "Aggregated Stage Damage Relationships";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addDamageCurve = new Utilities.NamedAction();
            addDamageCurve.Header = "Create New Aggregated Stage Damage Relationship";
            addDamageCurve.Action = AddNewDamageCurve;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addDamageCurve);

            Actions = localActions;

            StudyCache.StageDamageAdded += AddStageDamageElement;
            StudyCache.StageDamageRemoved += RemoveStageDamageElement;
            StudyCache.StageDamageUpdated += UpdateStageDamageElement;
        }
        #endregion
        #region Voids
        private void UpdateStageDamageElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddStageDamageElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveStageDamageElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        public void AddNewDamageCurve(object arg1, EventArgs arg2)
        {
            List<double> xValues = new List<double>() { 1,2,3,4,5,6 };
            List<double> yValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000 };
            Functions.ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            IFdaFunction defaultCurve = IFdaFunctionFactory.Factory( IParameterEnum.Rating, (IFunction)func);

            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetStageDamageManager()
                , (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                 .WithSiblingRules(this);

            AggregatedStageDamageEditorVM vm = new AggregatedStageDamageEditorVM(defaultCurve, "Stage - Damage", "Stage", "Damage", actionManager);
            DynamicTabVM tab = new DynamicTabVM("Create Damage Curve", vm, "AddNewDamageCurve");
            Navigate(tab, false, true);           
        }

        public void AssignValuesFromElementToCurveEditor(BaseEditorVM editorVM, ChildElement element)
        {

        }
        #endregion
        #region Functions

        public ChildElement CreateElementFromEditor(BaseEditorVM vm)
        {
            CurveEditorVM editorVM = (CurveEditorVM)vm;
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new AggregatedStageDamageElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve, CreationMethodEnum.UserDefined);
        }
        #endregion
    }
}
