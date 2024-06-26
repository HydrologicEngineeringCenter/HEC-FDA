﻿using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Alternatives.Results.BatchCompute;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Results;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Alternatives
{
    public class AlternativeOwnerElement: ParentElement
    {
        public AlternativeOwnerElement():base()
        {
            Name = StringConstants.ALTERNATIVES;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addAlternative = new NamedAction();
            addAlternative.Header = StringConstants.CREATE_NEW_ALTERNATIVE_MENU;
            addAlternative.Action = AddNewAlternative;

            NamedAction viewSummaryMenu = new NamedAction();
            viewSummaryMenu.Header = "View Alternative Summary Results...";
            viewSummaryMenu.Action = ComputeAlternatives;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addAlternative);
            localActions.Add(viewSummaryMenu);

            Actions = localActions;

            StudyCache.AlternativeAdded += AddAlternativeElement;
            StudyCache.AlternativeRemoved += RemoveAlternativeElement;
            StudyCache.AlternativeUpdated += UpdateAlternativeElement;
        }

        public void AddNewAlternative(object arg1, EventArgs arg2)
        {
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);

            CreateNewAlternativeVM vm = new CreateNewAlternativeVM( actionManager);
            string header = StringConstants.CREATE_NEW_ALTERNATIVE_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, header);
            Navigate(tab, false, true);
        }

        private void UpdateAlternativeElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement( e.NewElement);
        }
        private void AddAlternativeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveAlternativeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        public void ComputeAlternatives(object arg1, EventArgs arg2)
        {
            AlternativeSelectorVM vm = new AlternativeSelectorVM();
            vm.RequestNavigation += Navigate;
            DynamicTabVM tab = new DynamicTabVM("View Alternative Results", vm, "ViewAlternativeResults");
            Navigate(tab, false, false);
        }
    }
}
