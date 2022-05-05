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
        private OccupancyTypesEditorVM _OccTypeEditor;
        private bool _IsEditorOpen = false;
        #endregion
        #region Properties
        public OccupancyTypesElement SelectedOccTypeElement { get; set; }
       
        public List<OccupancyTypesElement> ListOfOccupancyTypesGroups { get;
            set; } = new List<OccupancyTypesElement>();

        #endregion
        #region Constructors
        public OccupancyTypesOwnerElement( ):base()
        {
            Name = StringConstants.OCCUPANCY_TYPES;
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction editOccupancyTypes = new NamedAction();
            editOccupancyTypes.Header = StringConstants.EDIT_OCCTYPE_MENU;
            editOccupancyTypes.Action = EditOccupancyTypes;

            NamedAction importFromFile = new NamedAction();
            importFromFile.Header = StringConstants.ImportFromOldFda(StringConstants.IMPORT_OCCTYPE_FROM_OLD_NAME);
            importFromFile.Action = ImportFromFile;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(editOccupancyTypes);
            localActions.Add(importFromFile);

            Actions = localActions;

            StudyCache.OccTypeElementAdded += OccTypeElementWasAdded;

            StudyCache.OccTypeElementRemoved += RemoveOccTypeElement;
            StudyCache.OccTypeElementUpdated += UpdateOccTypeElement;
        }

        #endregion
        #region Voids
        private void OccTypeElementWasAdded(object sender, ElementAddedEventArgs e)
        {
            OccupancyTypesElement elem = (OccupancyTypesElement)e.Element;
            ListOfOccupancyTypesGroups.Add(elem);
            if(_IsEditorOpen)
            {
                _OccTypeEditor.AddGroup(elem);
            }          
        }
        private void UpdateOccTypeElement(object sender, ElementUpdatedEventArgs e)
        {
            OccupancyTypesElement newElement = (OccupancyTypesElement)e.NewElement;
            int index = -1;
            for (int i = 0; i < ListOfOccupancyTypesGroups.Count; i++)
            {
                if (ListOfOccupancyTypesGroups[i].ID == newElement.ID)
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                ListOfOccupancyTypesGroups.RemoveAt(index);
                ListOfOccupancyTypesGroups.Insert(index, newElement);
            }
        }
        private void RemoveOccTypeElement(object sender, ElementAddedEventArgs e)
        {
            ListOfOccupancyTypesGroups.Remove((OccupancyTypesElement)e.Element);
        }
        private void EditOccupancyTypes(object arg1, EventArgs arg2)
        {
            _IsEditorOpen = true;
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager();

            _OccTypeEditor = new OccupancyTypesEditorVM( actionManager);
            _OccTypeEditor.RequestNavigation += Navigate;
            _OccTypeEditor.FillEditor(ListOfOccupancyTypesGroups);
            string header = StringConstants.EDIT_OCCTYPE_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, _OccTypeEditor, StringConstants.EDIT_OCCTYPE_HEADER);
            tab.RemoveTabEvent += Tab_RemoveTabEvent;
            tab.RemoveWindowEvent += Tab_RemoveTabEvent;
            Navigate(tab, false, false);
        }

        private void Tab_RemoveTabEvent(object sender, EventArgs e)
        {
            //i need to know if the editor is still open so that i can update the editor
            _IsEditorOpen = false;
        }

        public void ImportFromFile(object arg1, EventArgs arg2)
        {
            ImportFromFDA1VM vm = new ImportOcctypesFromFDA1VM();
            string header = StringConstants.IMPORT_OCCTYPE_FROM_OLD_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, StringConstants.IMPORT_OCCTYPE_FROM_OLD_HEADER);
            Navigate(tab, false, true);
        }
       
        #endregion
    }
}
