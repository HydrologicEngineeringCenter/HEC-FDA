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

        #endregion
        #region Properties
        public bool IsInMapWindow { get; set; }
        public bool IsImportedFromOldFDA { get; set; }
        public DefineSIAttributesVM DefineSIAttributes { get; set; }
      
        #endregion
        #region Constructors
        public InventoryElement(string name, string description, bool isImportedFromOldFDA, int id) 
            : base(name,"", description, id)
        {
            IsImportedFromOldFDA = isImportedFromOldFDA;

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
