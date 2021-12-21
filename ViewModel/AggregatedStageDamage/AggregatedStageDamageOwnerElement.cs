using System;
using System.Collections.Generic;
using System.Windows;
using ViewModel.Editors;
using ViewModel.ImpactArea;
using ViewModel.Inventory.OccupancyTypes;
using ViewModel.Utilities;

namespace ViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageOwnerElement : ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties  
        #endregion
        #region Constructors
        public AggregatedStageDamageOwnerElement() : base()
        {
            Name = "Aggregated Stage Damage Relationships";
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addDamageCurve = new NamedAction();
            addDamageCurve.Header = "Create New Aggregated Stage Damage Relationship";
            addDamageCurve.Action = AddNewStageDamageCurveSet;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addDamageCurve);

            Actions = localActions;

            StudyCache.StageDamageAdded += AddStageDamageElement;
            StudyCache.StageDamageRemoved += RemoveStageDamageElement;
            StudyCache.StageDamageUpdated += UpdateStageDamageElement;
        }
        #endregion
        #region Voids
        private void UpdateStageDamageElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddStageDamageElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveStageDamageElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        public void AddNewStageDamageCurveSet(object arg1, EventArgs arg2)
        {
            //An impact area is required
            List<ImpactAreaElement> impactAreaSet = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            List<OccupancyTypesElement> occTypeElem = StudyCache.GetChildElementsOfType<OccupancyTypesElement>();
            if (impactAreaSet.Count == 0)
            {
                MessageBox.Show("An impact area set is required to create aggregated stage damage curves.", "Impact Area Set Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if(occTypeElem.Count == 0)
            {
                MessageBox.Show("Occupancy types must be imported to create aggregated stage damage curves.", "Occupancy Types Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                //create action manager
                EditorActionManager actionManager = new EditorActionManager()
                     .WithSiblingRules(this);

                AggregatedStageDamageEditorVM vm = new AggregatedStageDamageEditorVM(null, "Stage - Damage", "Stage", "Damage", actionManager);
                DynamicTabVM tab = new DynamicTabVM("Create Damage Curve", vm, "AddNewDamageCurve");
                Navigate(tab, false, true);
            }
        }

        #endregion
        #region Functions

        public ChildElement CreateElementFromEditor(BaseEditorVM vm)
        {
            CurveEditorVM editorVM = (CurveEditorVM)vm;
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new AggregatedStageDamageElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve, CreationMethodEnum.UserDefined);
        }
        #endregion
    }
}
