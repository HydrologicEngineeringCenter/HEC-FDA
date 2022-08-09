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
        public Func<ChildElement> CreateElementFromEditorAction { get; set; }

        #endregion
        #region Constructors

        public RenameVM( ChildElement element, Func<ChildElement> createElementFromEditorAction) :base(element,null)
        {
            Name = element.Name;
            OldElement = element;
            AddSiblingRules(element);
            CreateElementFromEditorAction = createElementFromEditorAction;
        }
        #endregion

        public override void Save()
        {
            ElementToSave = CreateElementFromEditorAction();
            ElementToSave.Name = Name;
            ElementToSave.UpdateTreeViewHeader(Name);

            Saving.IElementManager savingManager = Saving.PersistenceFactory.GetElementManager(OldElement);
            if (savingManager != null)
            {
                savingManager.SaveExisting(ElementToSave);
            }
        }
    }
}
