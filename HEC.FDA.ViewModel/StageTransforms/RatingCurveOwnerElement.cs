using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.StageTransforms
{
    public class RatingCurveOwnerElement : ParentElement
    {

        #region Constructors
        public RatingCurveOwnerElement( ) : base()
        {
            Name = StringConstants.STAGE_DISCHARGE_FUNCTIONS;
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addRatingCurve = new NamedAction();
            addRatingCurve.Header = StringConstants.CREATE_STAGE_DISCHARGE_MENU;
            addRatingCurve.Action = AddNewRatingCurve;

            NamedAction ImportRatingCurve = new NamedAction();
            ImportRatingCurve.Header = StringConstants.CreateImportFromFileMenuString(StringConstants.IMPORT_STAGE_DISCHARGE_FROM_OLD_NAME);
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
            string header = StringConstants.CreateImportHeader(StringConstants.IMPORT_STAGE_DISCHARGE_FROM_OLD_NAME);
            DynamicTabVM tab = new DynamicTabVM(header, vm, header);
            Navigate(tab, false, true);
        }

        public void AddNewRatingCurve(object arg1, EventArgs arg2)
        {
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);

            CurveComponentVM curveComponentVM = DefaultData.RatingComputeComponent();
            RatingCurveEditorVM vm = new RatingCurveEditorVM(curveComponentVM, actionManager);
            string header = StringConstants.CREATE_STAGE_DISCHARGE_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, StringConstants.CREATE_STAGE_DISCHARGE_HEADER);
            Navigate(tab, false, true);          
        }
  
        #endregion
    }
}
