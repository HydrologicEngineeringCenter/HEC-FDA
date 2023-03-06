using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.IndexPoints
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

            StudyCache.IndexPointsAdded += AddIndexPointsElement;
            StudyCache.IndexPointsRemoved += RemoveIndexPointsElement;
            StudyCache.IndexPointsUpdated += UpdateIndexPointsElement;
        }

        private void UpdateIndexPointsElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.NewElement);
        }
        private void AddIndexPointsElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveIndexPointsElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }

        public void CreateNewIndexPoints(object arg1, EventArgs arg2)
        {
            EditorActionManager actionManager = new EditorActionManager()
                    .WithSiblingRules(this);

            IndexPointsEditorVM vm = new IndexPointsEditorVM(actionManager);
            string header = StringConstants.CREATE_INDEX_POINTS_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, StringConstants.CREATE_INDEX_POINTS_HEADER);
            Navigate(tab, false, false);
        }

    }
}
