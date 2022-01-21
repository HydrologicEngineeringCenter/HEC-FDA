using ViewModel.Utilities;
using Functions;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.GeoTech
{
    public class LeveeFeatureOwnerElement : Utilities.ParentElement
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
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);


            Utilities.NamedAction add = new Utilities.NamedAction();
            add.Header = "Create New Levee Feature";
            add.Action = AddNewLeveeFeature;

            NamedAction importFromFile = new NamedAction();
            importFromFile.Header = "Import Occupancy Types from FDA 1.0...";
            importFromFile.Action = ImportFromFile;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
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
            List<double> xValues = new List<double>() { 0};
            List<double> yValues = new List<double>() { 0 };
            Functions.ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(xValues, yValues);
            IFdaFunction defaultCurve = IFdaFunctionFactory.Factory( IParameterEnum.LateralStructureFailure, (IFunction)func);

            LeveeFeatureEditorVM vm = new LeveeFeatureEditorVM(defaultCurve, actionManager);
            //StudyCache.AddSiblingRules(vm, this);
            //vm.AddSiblingRules(this);
            string header = "Create Levee";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateLevee");
            Navigate(tab, false, false);
          
        }

        #endregion
        #region Functions
        public ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            LeveeFeatureEditorVM editorVM = (LeveeFeatureEditorVM)vm;
            //Editors.CurveEditorVM vm = (Editors.CurveEditorVM)editorVM;
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            //return new RatingCurveElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve);
            return new LeveeFeatureElement(editorVM.Name, editDate, editorVM.Description, editorVM.Elevation, editorVM.IsUsingDefault, editorVM.Curve);
        }
        #endregion
    }
}
