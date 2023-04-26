using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Results;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport
{
    public class AlternativeComparisonReportOwnerElement: ParentElement
    {
        public AlternativeComparisonReportOwnerElement():base()
        {
            Name = StringConstants.ALTERNATIVE_COMP_REPORTS;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addAlternativeAction = new NamedAction();
            addAlternativeAction.Header = StringConstants.CREATE_NEW_ALTERNATIVE_COMP_REPORTS_MENU;
            addAlternativeAction.Action = AddNewAlternative;

            NamedAction viewSummaryMenu = new NamedAction();
            viewSummaryMenu.Header = "View Alternatives Summary Results...";
            viewSummaryMenu.Action = ComputeAlternatives;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addAlternativeAction);
            localActions.Add(viewSummaryMenu);

            Actions = localActions;

            StudyCache.AlternativeCompReportAdded += AddAlternativeCompReportElement;
            StudyCache.AlternativeCompReportRemoved += RemoveAlternativeCompReportElement;
            StudyCache.AlternativeCompReportUpdated += UpdateAlternativeCompReportElement;
        }

        private void UpdateAlternativeCompReportElement(object sender, ElementUpdatedEventArgs e)
        {
            UpdateElement( e.NewElement);
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
            string header = StringConstants.CREATE_NEW_ALTERNATIVE_COMP_REPORTS_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, header);
            Navigate(tab, false, true);
        }

        public void ComputeAlternatives(object arg1, EventArgs arg2)
        {
            ScenarioSelectorVM vm = new ScenarioSelectorVM();
            vm.RequestNavigation += Navigate;
            DynamicTabVM tab = new DynamicTabVM(StringConstants.CREATE_NEW_SCENARIO_HEADER, vm, StringConstants.CREATE_NEW_SCENARIO_HEADER);
            Navigate(tab, false, false);

        }

    }
}
