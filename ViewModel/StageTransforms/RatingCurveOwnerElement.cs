using paireddata;
using System;
using System.Collections.Generic;
using ViewModel.Editors;
using ViewModel.Utilities;

namespace ViewModel.StageTransforms
{
    public class RatingCurveOwnerElement : ParentElement
    {
        #region Notes
        #endregion
        #region Fields

        #endregion
        #region Properties
        
        #endregion
        #region Constructors
        public RatingCurveOwnerElement( ) : base()
        {
            Name = "Rating Curves";
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addRatingCurve = new NamedAction();
            addRatingCurve.Header = "Create New Rating Curve";
            addRatingCurve.Action = AddNewRatingCurve;

            NamedAction ImportRatingCurve = new NamedAction();
            ImportRatingCurve.Header = StringConstants.ImportFromOldFda("Rating Curve");
            ImportRatingCurve.Action = ImportRatingCurvefromAscii;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addRatingCurve);
            localActions.Add(ImportRatingCurve);

            Actions = localActions;

            StudyCache.RatingAdded += AddRatingCurveElement;
            StudyCache.RatingRemoved += RemoveRatingCurveElement;
            StudyCache.RatingUpdated += UpdateRatingCurveElement;
        }

        #endregion
        #region Voids
        private void UpdateRatingCurveElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddRatingCurveElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveRatingCurveElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);        
        }
        private void ImportRatingCurvefromAscii(object arg1, EventArgs arg2)
        {
            ImportFromFDA1VM vm = new ImportRatingsFromFDA1VM();
            string header = "Import Rating Curve";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportRatingCurve");
            Navigate(tab, false, true);
        }

      

        public void AddNewRatingCurve(object arg1, EventArgs arg2)
        {
            //List<double> xValues = new List<double>() { 1000, 10000, 15000,20000,50000 };//, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            //List<double> yValues = new List<double>() { 1,2,3,4,5};//, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            //ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            //IFunction function = IFunctionFactory.Factory(func.Coordinates, func.Interpolator);
            //IFdaFunction defaultCurve = IFdaFunctionFactory.Factory( IParameterEnum.Rating, function);
            
            //create save helper
            SaveUndoRedoHelper saveHelper = new SaveUndoRedoHelper( Saving.PersistenceFactory.GetRatingManager()
                , (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            EditorActionManager actionManager = new EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                .WithSiblingRules(this);

            UncertainPairedData defaultCurve = DefaultPairedData.CreateDefaultNormalUncertainPairedData("Stage", "Flow", "Rating Curve");

            CurveEditorVM vm = new CurveEditorVM(defaultCurve,  "Outflow", "Exterior Stage", "Outflow - Exterior Stage", actionManager);          
            string header = "Create Rating Curve " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateRatingCurve");
            Navigate(tab, false, true);
            
        }

    
        #endregion
        #region Functions
      
        public  ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new RatingCurveElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve);
        }

        #endregion
    }
}
