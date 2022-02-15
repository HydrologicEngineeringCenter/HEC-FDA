using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport
{
    public class AlternativeComparisonReportOwnerElement: ParentElement
    {
        public AlternativeComparisonReportOwnerElement():base()
        {
            Name = "Alternative Comparison Report";
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addAlternativeAction = new NamedAction();
            addAlternativeAction.Header = "Create New Alternative Comparison Report...";
            addAlternativeAction.Action = AddNewAlternative;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addAlternativeAction);

            Actions = localActions;

            StudyCache.AlternativeCompReportAdded += AddAlternativeCompReportElement;
            StudyCache.AlternativeCompReportRemoved += RemoveAlternativeCompReportElement;
            StudyCache.AlternativeCompReportUpdated += UpdateAlternativeCompReportElement;
        }

        private void UpdateAlternativeCompReportElement(object sender, ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddAlternativeCompReportElement(object sender, ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveAlternativeCompReportElement(object sender, ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }

        public void AddNewAlternative(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);
          
            CreateNewAlternativeComparisonReportVM vm = new CreateNewAlternativeComparisonReportVM( actionManager);
            string header = "Create Alternative Comparison Report";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateNewAlternativeCompReport");
            Navigate(tab, false, true);
        }

    }
}
