using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    //[Author(q0heccdm, 7 / 6 / 2017 10:22:36 AM)]
    public class OccupancyTypesOwnerElement : ParentElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/6/2017 10:22:36 AM
        #endregion
        #region Fields

        #endregion
        #region Properties

        #endregion
        #region Constructors
        public OccupancyTypesOwnerElement( ):base()
        {
            Name = StringConstants.OCCUPANCY_TYPES;
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction importFromFile = new NamedAction();
            importFromFile.Header = StringConstants.CreateImportFromFileMenuString(StringConstants.IMPORT_OCCTYPE_FROM_OLD_NAME);
            importFromFile.Action = ImportFromFile;

            NamedAction createNew = new NamedAction();
            createNew.Header = StringConstants.CREATE_NEW_OCCTYPE_MENU;
            createNew.Action = CreateNew;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(createNew);
            localActions.Add(importFromFile);

            Actions = localActions;

            StudyCache.OccTypeElementAdded += OccTypeElementWasAdded;
            StudyCache.OccTypeElementRemoved += RemoveOccTypeElement;
            StudyCache.OccTypeElementUpdated += UpdateOccTypeElement;
        }

        #endregion
        #region Voids
        private void UpdateOccTypeElement(object sender, ElementUpdatedEventArgs e)
        {
            UpdateElement(e.NewElement);
        }
        private void RemoveOccTypeElement(object sender, ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void OccTypeElementWasAdded(object sender, ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }       

        public void ImportFromFile(object arg1, EventArgs arg2)
        {
            ImportFromFDA1VM vm = new ImportOcctypesFromFDA1VM();
            string header = StringConstants.CreateImportHeader(StringConstants.IMPORT_OCCTYPE_FROM_OLD_NAME);
            DynamicTabVM tab = new DynamicTabVM(header, vm, header);
            Navigate(tab, false, true);
        }

        public void CreateNew(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);
            OccupancyTypesEditorVM vm = new OccupancyTypesEditorVM(actionManager);
            vm.RequestNavigation += Navigate;
            string header = StringConstants.CREATE_NEW_OCCTYPE_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, header);
            Navigate(tab, false, true);
        }

        #endregion
    }
}
