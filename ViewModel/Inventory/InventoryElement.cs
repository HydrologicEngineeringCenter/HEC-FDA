using OpenGLMapping;
using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Inventory
{
    //[Author(q0heccdm, 12 / 1 / 2016 2:21:18 PM)]
    public class InventoryElement : ChildElement
    {

        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/1/2016 2:21:18 PM
        #endregion
        #region Fields
        private string _TableConstant = Saving.PersistenceManagers.StructureInventoryPersistenceManager.STRUCTURE_INVENTORY_TABLE_CONSTANT;

        private StructureInventoryBaseElement _StructureInventory;

        #endregion
        #region Properties
        public bool IsInMapWindow { get; set; }
        public bool IsImportedFromOldFDA { get; set; }
        public DefineSIAttributesVM DefineSIAttributes { get; set; }
       
        public StructureInventoryBaseElement StructureInventory
        {
            get { return _StructureInventory; }
            set { _StructureInventory = value; NotifyPropertyChanged(); }
        }
      
        #endregion
        #region Constructors
        public InventoryElement(StructureInventoryBaseElement structInventoryBaseElement, bool isImportedFromOldFDA, int id) 
            : base(structInventoryBaseElement.Name,"", structInventoryBaseElement.Description, id)
        {
            IsImportedFromOldFDA = isImportedFromOldFDA;

            StructureInventory = structInventoryBaseElement;

            List<NamedAction> localactions = new List<NamedAction>();

            NamedAction removeInventory = new NamedAction();
            removeInventory.Header = StringConstants.REMOVE_MENU;
            removeInventory.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = StringConstants.RENAME_MENU;
            renameElement.Action = Rename;

            localactions.Add(removeInventory);
            localactions.Add(renameElement);

            Actions = localactions;

        }
        #endregion

        #region Functions

        public override XElement ToXML()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
