using System;
using System.Collections.Generic;
using System.Windows;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactAreaScenario
{
    public class IASOwnerElement : ParentElement
    {
        #region Notes
        #endregion

        #region Constructors
        public IASOwnerElement( ) : base()
        {
            Name = StringConstants.SCENARIOS;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addCondition = new NamedAction();
            addCondition.Header = StringConstants.CREATE_NEW_SCENARIO_MENU;
            addCondition.Action = AddNewIASSet;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addCondition);

            Actions = localActions;
            StudyCache.IASElementAdded += AddIASElementSet;
            StudyCache.IASElementRemoved += RemoveConditionsElement;
            StudyCache.IASElementUpdated += UpdateIASElementSet;

            //the child elements
            StudyCache.ImpactAreaRemoved += ChildElementRemoved;
            StudyCache.FlowFrequencyRemoved += ChildElementRemoved;
            StudyCache.InflowOutflowRemoved += ChildElementRemoved;
            StudyCache.RatingRemoved += ChildElementRemoved;
            StudyCache.LeveeRemoved += ChildElementRemoved;
            StudyCache.ExteriorInteriorRemoved += ChildElementRemoved;
            StudyCache.StageDamageRemoved += ChildElementRemoved;

            StudyCache.ImpactAreaUpdated += ChildElementUpdated;
            StudyCache.FlowFrequencyUpdated += ChildElementUpdated;
            StudyCache.InflowOutflowUpdated += ChildElementUpdated;
            StudyCache.RatingUpdated += ChildElementUpdated;
            StudyCache.LeveeUpdated += ChildElementUpdated;
            StudyCache.ExteriorInteriorUpdated += ChildElementUpdated;
            StudyCache.StageDamageUpdated += ChildElementUpdated;
        }

        /// <summary>
        /// When an impact area or any of the curves gets modified (saved) then nothing needs to get
        /// updated in the DB or the study cache for the conditions element because it is just storing
        /// the id's of the sub elements which have not changed. If a conditions editor is open, then
        /// we need to update the curve values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ChildElementUpdated(object sender, Saving.ElementUpdatedEventArgs args)
        {
            int removedElementID = args.NewElement.ID;
            if (args.NewElement is ChildElement)
            {
                ChildElement childElem = (ChildElement)args.NewElement;
                Saving.PersistenceFactory.GetIASManager().UpdateIASTooltipsChildElementModified(childElem, removedElementID);
            }
        }

        /// <summary>
        /// When an impact area or any of the curves gets removed from the study, this event will get fired. 
        /// The persistence manager for the conditions will loop over all the conditions in the study and if
        /// this child element was being used by a condition, the id for that element in the condition will 
        /// be set to -1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ChildElementRemoved(object sender, Saving.ElementAddedEventArgs args)
        {
            int removedElementID = args.Element.ID;
            if (args.Element is ChildElement)
            {
                ChildElement childElem = (ChildElement)args.Element;
                Saving.PersistenceFactory.GetIASManager().UpdateIASTooltipsChildElementModified(childElem, removedElementID);
            }
        }

        #endregion
        #region Voids

        private void UpdateIASElementSet(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement( e.NewElement);
        }
        private void AddIASElementSet(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveConditionsElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }

        public void AddNewIASSet(object arg1, EventArgs arg2)
        {
            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impactAreaElements.Count == 0)
            {
                MessageBox.Show("An impact area is required to create an impact area scenario.", "No Impact Areas", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                EditorActionManager actionManager = new EditorActionManager()
                     .WithSiblingRules(this);
                Editor.IASEditorVM vm = new Editor.IASEditorVM(actionManager);
                vm.RequestNavigation += Navigate;
                DynamicTabVM tab = new DynamicTabVM(StringConstants.CREATE_NEW_SCENARIO_HEADER, vm, StringConstants.CREATE_NEW_SCENARIO_HEADER);
                Navigate(tab, false, false);
            }

        }
        #endregion

    }
}
