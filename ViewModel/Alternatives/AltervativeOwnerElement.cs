using System;
using System.Collections.Generic;
using ViewModel.Editors;
using ViewModel.Utilities;

namespace ViewModel.Alternatives
{
    public class AltervativeOwnerElement: ParentElement
    {
        public AltervativeOwnerElement():base()
        {
            Name = "Alternatives";
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addAlternative = new NamedAction();
            addAlternative.Header = "Add Alternative...";
            addAlternative.Action = AddNewAlternative;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addAlternative);

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
            string header = "Create Alternative";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateNewAlternative");
            Navigate(tab, false, true);
        }

        private void UpdateAlternativeElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddAlternativeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveAlternativeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
    }
}
