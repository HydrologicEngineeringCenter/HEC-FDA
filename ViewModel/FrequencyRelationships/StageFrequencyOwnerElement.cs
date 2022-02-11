using System;
using System.Collections.Generic;
using ViewModel.Utilities;

namespace ViewModel.FrequencyRelationships
{
    class StageFrequencyOwnerElement : ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public StageFrequencyOwnerElement( ) : base()
        {
            Name = "Stage Frequency Curves";

            NamedAction addRatingCurve = new NamedAction();
            addRatingCurve.Header = "Create New Stage Frequency Curve";
            addRatingCurve.Action = AddNewRatingCurve;

            NamedAction ImportRatingCurve = new NamedAction();
            ImportRatingCurve.Header = StringConstants.ImportFromOldFda("Stage Frequency Curve");
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
