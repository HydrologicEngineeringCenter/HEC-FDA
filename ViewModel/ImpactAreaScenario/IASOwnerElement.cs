using Model;
using System;
using System.Collections.Generic;
using ViewModel.AggregatedStageDamage;
using ViewModel.FlowTransforms;
using ViewModel.FrequencyRelationships;
using ViewModel.GeoTech;
using ViewModel.StageTransforms;
using ViewModel.Utilities;

namespace ViewModel.ImpactAreaScenario
{
    public class IASOwnerElement : ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties

        #endregion
        #region Constructors
        public IASOwnerElement( ) : base()
        {
            Name = "Impact Area Scenarios";
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addCondition = new NamedAction();
            addCondition.Header = "Create New Impact Area Scenario";
            addCondition.Action = AddNewCondition;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addCondition);

            Actions = localActions;
            StudyCache.IASElementAdded += AddConditionsElement;
            StudyCache.IASElementRemoved += RemoveConditionsElement;
            StudyCache.IASElementUpdated += UpdateConditionsElement;

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

            int removedElementID = args.ID;
            if (args.NewElement is ChildElement)
            {
                ChildElement childElem = (ChildElement)args.NewElement;
                //UpdateEditorWhileEditing(childElem, removedElementID);
                Saving.PersistenceFactory.GetIASManager().UpdateIASTooltipsChildElementModified(childElem, removedElementID, -1);
            }

            //these elements will be the sub elements of the condition (ie: rating, inflow-outflow, etc)
            //BaseFdaElement newElement = args.NewElement;
            //if (newElement is ChildElement)
            //{
            //    int elemID = args.ID;

            //    List<IASElementSet> conditionsElements = StudyCache.GetChildElementsOfType<IASElementSet>();
            //    foreach (IASElementSet condElem in conditionsElements)
            //    {
            //        //todo: this got broken when i changed to the IASElementSet
            //        //condElem.UpdateElementInEditor_ChildModified(elemID, (ChildElement)newElement);
            //    }
            //}

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
            int removedElementID = args.ID;
            if (args.Element is ChildElement)
            {
                ChildElement childElem = (ChildElement)args.Element;
                //UpdateEditorWhileEditing(childElem, removedElementID);
                Saving.PersistenceFactory.GetIASManager().UpdateIASTooltipsChildElementModified(childElem, removedElementID, -1);
            }
        }
        //private void UpdateEditorWhileEditing(ChildElement elem, int removedElementID)
        //{
        //    List<IASElementSet> conditionsElements = StudyCache.GetChildElementsOfType<IASElementSet>();
        //    foreach(IASElementSet condElem in conditionsElements)
        //    {
        //        //todo:what to do here.
        //        //if(condElem.ConditionsEditor != null)
        //        //{
        //        //    condElem.ConditionsEditor.UpdateEditorWhileEditing_ChildRemoved(removedElementID, elem);
        //        //}
        //    }
        //}

        #endregion
        #region Voids

        private void UpdateConditionsElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            //so if the element has an editor that is open (not null)
            //then we need to update it with the new element. I guess
            //we just care about the curves and the impact area.
            //((ConditionsElement)e.OldElement).UpdateElementInEditor_ChildRemoved((ConditionsElement)e.NewElement);
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddConditionsElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveConditionsElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }

        public void AddNewCondition(object arg1, EventArgs arg2)
        {

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                 .WithSiblingRules(this);
            Editor.IASEditorVM vm = new Editor.IASEditorVM(actionManager);
            vm.RequestNavigation += Navigate;
            DynamicTabVM tab = new DynamicTabVM("Impact Area Scenario Editor", vm, "CreateIAS");
            Navigate(tab, false, false);

        }
        #endregion

    }
}
