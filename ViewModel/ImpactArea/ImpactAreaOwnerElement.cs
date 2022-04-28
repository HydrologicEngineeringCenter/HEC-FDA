using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactArea
{
    public class ImpactAreaOwnerElement : ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public ImpactAreaOwnerElement( ) : base()
        {
            Name = StringConstants.IMPACT_AREA_SET;
            CustomTreeViewHeader = new CustomHeaderVM(Name);
            IsBold = false;
            NamedAction add = new NamedAction();
            add.Header = StringConstants.IMPORT_IMPACT_AREA_SET_MENU;
            add.Action = AddNew;

            List<NamedAction> localactions = new List<NamedAction>();
            localactions.Add(add);

            Actions = localactions;

            StudyCache.ImpactAreaAdded += AddImpactAreaElement;
            StudyCache.ImpactAreaRemoved += RemoveImpactAreaElement;
            StudyCache.ImpactAreaUpdated += UpdateImpactAreaElement;
        }
        #endregion
        #region Voids
        private void UpdateImpactAreaElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement( e.NewElement);
        }
        private void AddImpactAreaElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveImpactAreaElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        #endregion
        public void AddNew(object arg1, EventArgs arg2)
        {
            //check to see if one already exists
            List<ImpactAreaElement> impAreaElems = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impAreaElems.Count == 0)
            {              
                EditorActionManager actionManager = new EditorActionManager()
                    .WithSiblingRules(this);

                ImpactAreaImporterVM vm = new ImpactAreaImporterVM(actionManager);
                string header = StringConstants.IMPORT_IMPACT_AREA_SET_HEADER;
                DynamicTabVM tab = new DynamicTabVM(header, vm, StringConstants.IMPORT_IMPACT_AREA_SET_HEADER);
                Navigate(tab, false, false);
            }
            else
            {
                MessageBox.Show("Only one impact area set is allowed.", "Impact Area Set Already Exists", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
