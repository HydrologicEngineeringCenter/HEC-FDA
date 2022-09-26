using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    class StageFrequencyOwnerElement : ParentElement
    {
        #region Notes
        #endregion

        #region Constructors
        public StageFrequencyOwnerElement( ) : base()
        {
            Name = StringConstants.AGGREGATED_STAGE_DAMAGE_FUNCTIONS;

            NamedAction addRatingCurve = new NamedAction();
            addRatingCurve.Header = StringConstants.CREATE_NEW_STAGE_DAMAGE_MENU;
            addRatingCurve.Action = AddNewRatingCurve;

            NamedAction ImportRatingCurve = new NamedAction();
            ImportRatingCurve.Header = StringConstants.CreateImportFromFileMenuString(StringConstants.IMPORT_STAGE_DAMAGE_FROM_OLD_NAME);
            ImportRatingCurve.Action = ImportRatingCurvefromAscii;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addRatingCurve);
            localActions.Add(ImportRatingCurve);

            Actions = localActions;
        }
        #endregion
        #region Voids
        private void ImportRatingCurvefromAscii(object arg1, EventArgs arg2)
        {
            throw new NotImplementedException();
        }

        private void AddNewRatingCurve(object arg1, EventArgs arg2)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
