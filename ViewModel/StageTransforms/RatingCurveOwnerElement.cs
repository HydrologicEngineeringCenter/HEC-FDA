using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.StageTransforms
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
            UpdateElement(e.NewElement);
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
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);

            ComputeComponentVM computeComponentVM = new ComputeComponentVM("Rating Curve", "Stage", "Flow");
            RatingCurveEditorVM vm = new RatingCurveEditorVM(computeComponentVM, actionManager);
            string header = "Create Rating Curve " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateRatingCurve");
            Navigate(tab, false, true);          
        }
  
        #endregion
    }
}
