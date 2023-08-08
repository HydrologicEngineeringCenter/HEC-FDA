using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport
{
    public class AlternativeComparisonReportOwnerElement: ParentElement
    {
        public AlternativeComparisonReportOwnerElement():base()
        {
            Name = StringConstants.ALTERNATIVE_COMP_REPORTS;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addAlternativeAction = new()
            {
                Header = StringConstants.CREATE_NEW_ALTERNATIVE_COMP_REPORTS_MENU,
                Action = AddNewAlternative
            };

            List<NamedAction> localActions = new()
            {
                addAlternativeAction
            };

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
          
            CreateNewAlternativeComparisonReportVM vm = new( actionManager);
            string header = StringConstants.CREATE_NEW_ALTERNATIVE_COMP_REPORTS_HEADER;
            string uniqueSuffix = DateTime.Now.ToString();
            DynamicTabVM tab = new(header, vm, header + uniqueSuffix);
            Navigate(tab, false, true);
        }

    }
}
