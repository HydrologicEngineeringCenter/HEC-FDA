using System;
using System.Collections.Generic;
using System.Windows;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageOwnerElement : ParentElement
    {

        #region Constructors
        public AggregatedStageDamageOwnerElement() : base()
        {
            Name = StringConstants.AGGREGATED_STAGE_DAMAGE_FUNCTIONS;
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addDamageCurve = new NamedAction();
            addDamageCurve.Header = StringConstants.CREATE_NEW_STAGE_DAMAGE_MENU;
            addDamageCurve.Action = AddNewStageDamageCurveSet;

            NamedAction importDamageCurve = new NamedAction();
            importDamageCurve.Header = StringConstants.CreateImportFromFileMenuString(StringConstants.IMPORT_STAGE_DAMAGE_FROM_OLD_NAME);
            importDamageCurve.Action = ImportNewStageDamageCurveSet;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addDamageCurve);
            localActions.Add(importDamageCurve);

            Actions = localActions;

            StudyCache.StageDamageAdded += AddStageDamageElement;
            StudyCache.StageDamageRemoved += RemoveStageDamageElement;
            StudyCache.StageDamageUpdated += UpdateStageDamageElement;
        }
        #endregion
        #region Voids
        private void UpdateStageDamageElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement( e.NewElement);
        }
        private void AddStageDamageElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveStageDamageElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }

        public void ImportNewStageDamageCurveSet(object arg1, EventArgs arg2)
        {
            //this option is not allowed if you do not have an impact areas.
            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impactAreaElements.Count > 0)
            {
                ImportFromFDA1VM vm = new ImportStageDamageFromFDA1VM();
                string header = StringConstants.CreateImportHeader(StringConstants.IMPORT_STAGE_DAMAGE_FROM_OLD_NAME);
                DynamicTabVM tab = new DynamicTabVM(header, vm, header);
                Navigate(tab, false, true);
            }
            else
            {
                MessageBox.Show("An impact area set is required to create aggregated stage damage curves.", "Impact Area Set Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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

                AggregatedStageDamageEditorVM vm = new AggregatedStageDamageEditorVM( actionManager);
                string header = StringConstants.CREATE_NEW_STAGE_DAMAGE_HEADER;
                DynamicTabVM tab = new DynamicTabVM(header, vm, header);
                Navigate(tab, false, true);
            }
        }

        #endregion
    }
}
