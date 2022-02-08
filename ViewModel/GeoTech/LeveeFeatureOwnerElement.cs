using paireddata;
using System;
using System.Collections.Generic;
using ViewModel.Utilities;

namespace ViewModel.GeoTech
{
    public class LeveeFeatureOwnerElement : ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public LeveeFeatureOwnerElement( ) : base()
        {
            Name = "Levee Features";
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction add = new NamedAction();
            add.Header = "Create New Levee Feature";
            add.Action = AddNewLeveeFeature;

            NamedAction importFromFile = new NamedAction();
            importFromFile.Header = StringConstants.ImportFromOldFda("Levees");
            importFromFile.Action = ImportFromFile;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(add);
            localActions.Add(importFromFile);

            Actions = localActions;

            StudyCache.LeveeAdded += AddLeveeElement;
            StudyCache.LeveeRemoved += RemoveLeveeElement;
            StudyCache.LeveeUpdated += UpdateLeveeElement;
        }
        #endregion
        #region Voids
        private void UpdateLeveeElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
     
        private void RemoveLeveeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void AddLeveeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }

        public void ImportFromFile(object arg1, EventArgs arg2)
        {
            ImportFromFDA1VM vm = new ImportLeveeElementFromFDA1VM();
            string header = "Import Levee";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportLevee");
            Navigate(tab, false, true);
        }

        public void AddNewLeveeFeature(object arg1, EventArgs arg2)
        {
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(
                Saving.PersistenceFactory.GetLeveeManager(),
                 (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                .WithSiblingRules(this);

            //create default curve 
            List<double> xs = new List<double>() {0};
            List<double> ys = new List<double>() {0};
            UncertainPairedData defaultCurve = UncertainPairedDataFactory.CreateDeterminateData(xs, ys, "Probabilty", "Elevation", "Failure Function");

            LeveeFeatureEditorVM vm = new LeveeFeatureEditorVM(defaultCurve, actionManager);
            string header = "Create Levee";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateLevee");
            Navigate(tab, false, false);       
        }

        #endregion
        #region Functions
        public ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            LeveeFeatureEditorVM editorVM = (LeveeFeatureEditorVM)vm;
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new LeveeFeatureElement(editorVM.Name, editDate, editorVM.Description, editorVM.Elevation, editorVM.IsUsingDefault, editorVM.Curve);
        }
        #endregion
    }
}
