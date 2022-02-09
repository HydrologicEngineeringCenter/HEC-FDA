using paireddata;
using System;
using System.Collections.Generic;
using ViewModel.Utilities;

namespace ViewModel.StageTransforms
{
    public class ExteriorInteriorOwnerElement :ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public ExteriorInteriorOwnerElement( ) : base()
        {
            Name = "Exterior Interior Relationships";
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

           NamedAction addExteriorInterior = new NamedAction();
            addExteriorInterior.Header = "Create New Exterior Interior Relationship";
            addExteriorInterior.Action = AddNewExteriorInteriorCurve;

           NamedAction ImportFromAscii = new NamedAction();
            ImportFromAscii.Header = StringConstants.ImportFromOldFda("Exterior Interior Relationship");
            ImportFromAscii.Action = ImportFromASCII;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addExteriorInterior);
            localActions.Add(ImportFromAscii);

            Actions = localActions;

            StudyCache.ExteriorInteriorAdded += AddExteriorInteriorElement;
            StudyCache.ExteriorInteriorRemoved += RemoveExteriorInteriorElement;
            StudyCache.ExteriorInteriorUpdated += UpdateExteriorInteriorElement;
        }
        #endregion
        #region Voids
        private void UpdateExteriorInteriorElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddExteriorInteriorElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveExteriorInteriorElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void ImportFromASCII(object arg1, EventArgs arg2)
        {
            ImportFromFDA1VM vm = new ImportExteriorInteriorFromFDA1VM();
            string header = "Import Exterior Interior Curve";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportExteriorInteriorCurve");
            Navigate(tab, false, true);
        }

        public void AddNewExteriorInteriorCurve(object arg1, EventArgs arg2)
        {
            List<double> xValues = new List<double>() { 1,2,3,4,5,6 };
            List<double> yValues = new List<double>() { 1,2,3,4,5,6 };
            UncertainPairedData defaultCurve = UncertainPairedDataFactory.CreateDeterminateData(xValues,yValues, "Stage", "Flow", "Rating Curve");

            //create save helper
            Editors.SaveHelper saveHelper = new Editors.SaveHelper(Saving.PersistenceFactory.GetExteriorInteriorManager()
                , (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveHelper(saveHelper)
                .WithSiblingRules(this);

            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(defaultCurve, "Exterior Stage", "Interior Stage", "Exterior - Interior Stage", actionManager);
            string header = "Create Exterior Interior";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateExteriorInterior");
            Navigate(tab, false, true);
        }

        #endregion
        #region Functions

        public  ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new ExteriorInteriorElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve);
        }

        #endregion
    }
}
