using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.ImpactArea
{
    public class IndexPointsOwnerElement : ParentElement
    {

        public IndexPointsOwnerElement() : base()
        {
            Name = StringConstants.INDEX_POINTS;
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction createNewIndexPoints = new NamedAction();
            createNewIndexPoints.Header = StringConstants.CREATE_INDEX_POINTS_MENU;
            createNewIndexPoints.Action = CreateNewIndexPoints;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(createNewIndexPoints);

            Actions = localActions;
        }

        public void CreateNewIndexPoints(object arg1, EventArgs arg2)
        {
            //todo: do something.
        }

    }
}
