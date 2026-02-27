using System;
using System.Collections.Generic;
using System.Windows;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Results;
using HEC.FDA.ViewModel.Utilities;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

            NamedAction addCondition = new()
            {
                Header = StringConstants.CREATE_NEW_SCENARIO_MENU,
                Action = AddNewIASSet
            };

            NamedAction computeAllMenu = new()
            {
                Header = StringConstants.COMPUTE_SCENARIOS_MENU,
                Action = ComputeScenarios
            };

            NamedAction viewSummaryResultsMenu = new()
            {
                Header = StringConstants.VIEW_SUMMARY_RESULTS_MENU,
                Action = ViewSummaryResults
            };

            List<NamedAction> localActions = new()
            {
                addCondition,
                computeAllMenu,
                viewSummaryResultsMenu
            };

            Actions = localActions;
            StudyCache.IASElementAdded += AddIASElementSet;
            StudyCache.IASElementRemoved += RemoveIASElementSet;
            StudyCache.IASElementUpdated += UpdateIASElementSet;

            //the child elements
            StudyCache.ImpactAreaRemoved += ChildElementRemoved;
            StudyCache.FlowFrequencyRemoved += ChildElementRemoved;
            StudyCache.InflowOutflowRemoved += ChildElementRemoved;
            StudyCache.RatingRemoved += ChildElementRemoved;
            StudyCache.LeveeRemoved += ChildElementRemoved;
            StudyCache.ExteriorInteriorRemoved += ChildElementRemoved;
            StudyCache.StageDamageRemoved += ChildElementRemoved;
            StudyCache.StageLifeLossRemoved += ChildElementRemoved;

            StudyCache.ImpactAreaUpdated += ChildElementUpdated;
            StudyCache.FlowFrequencyUpdated += ChildElementUpdated;
            StudyCache.InflowOutflowUpdated += ChildElementUpdated;
            StudyCache.RatingUpdated += ChildElementUpdated;
            StudyCache.LeveeUpdated += ChildElementUpdated;
            StudyCache.ExteriorInteriorUpdated += ChildElementUpdated;
            StudyCache.StageDamageUpdated += ChildElementUpdated;
            StudyCache.StageLifeLossUpdated += ChildElementUpdated;
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
            if (args.NewElement is ChildElement childElem)
            {
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
            if (args.Element is ChildElement childElem)
            {
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
        private void RemoveIASElementSet(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }

        private void ComputeScenarios(object arg1, EventArgs arg2)
        {
            ScenarioSelectorVM vm = new();
            vm.RequestNavigation += Navigate;
            //todo: add to string constants
            DynamicTabVM tab = new(StringConstants.COMPUTE_SCENARIOS_HEADER, vm, StringConstants.COMPUTE_SCENARIOS_HEADER);
            Navigate(tab, false, false);
        }

        private void ViewSummaryResults(object arg1, EventArgs arg2)
        {
            List<IASElement> elems = StudyCache.GetChildElementsOfType<IASElement>();
            List<IASElement> elemsWithResults = new();
            foreach(IASElement elem in elems)
            {
                if(elem.Results != null)
                {
                    elemsWithResults.Add(elem);
                }
            }

            ScenarioDamageSummaryVM vm = new(elemsWithResults);
            DynamicTabVM tab = new(StringConstants.VIEW_SUMMARY_RESULTS_HEADER, vm, StringConstants.VIEW_SUMMARY_RESULTS_HEADER);
            Navigate(tab, false, false);
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
                Editor.IASEditorVM vm = new(actionManager);
                vm.RequestNavigation += Navigate;
                string uniqueSuffix = DateTime.Now.ToString();
                DynamicTabVM tab = new(StringConstants.CREATE_NEW_SCENARIO_HEADER, vm, StringConstants.CREATE_NEW_SCENARIO_HEADER + uniqueSuffix);
                Navigate(tab, false, false);
            }

        }
        #endregion

    }
}
