using System;
using HEC.FDA.ViewModel.Editors;

namespace HEC.FDA.ViewModel.Utilities
{
    //[Author(q0heccdm, 11 / 22 / 2016 9:05:37 AM)]
    public class RenameVM:BaseEditorVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 11/22/2016 9:05:37 AM
        #endregion
        #region Fields
        #endregion
        #region Properties
        public ChildElement OldElement { get; set; }
        public ChildElement ElementToSave { get; set; }
        public Func<ChildElement, ChildElement> CreateElementFromEditorAction { get; set; }

        #endregion
        #region Constructors

        public RenameVM( ChildElement element, Func<ChildElement,ChildElement> createElementFromEditorAction) :base(element,null)
        {
            Name = element.Name;
            OldElement = element;
            AddSiblingRules(element);
            CreateElementFromEditorAction = createElementFromEditorAction;
        }
        #endregion

        public override void Save()
        {
            ElementToSave = CreateElementFromEditorAction(OldElement);
            ElementToSave.Name = Name;
            ElementToSave.UpdateTreeViewHeader(Name);

            //here i need to do some stuff with the tree view
            //renaming an element needs to rename the same element in the tree view
            //if it is in the map window then i need to take it out and put it back in so i can modify the file
            OldElement.RenameMapTreeViewItem(OldElement, new EventArgs());

            Saving.IElementManager savingManager = Saving.PersistenceFactory.GetElementManager(OldElement);
            if (savingManager != null)
            {
                savingManager.SaveExisting(ElementToSave);
            }
        }
    }
}
