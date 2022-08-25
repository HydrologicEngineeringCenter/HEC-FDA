using HEC.FDA.ViewModel.Utilities;
using System;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Inventory
{
    //[Author(q0heccdm, 12 / 1 / 2016 2:21:18 PM)]
    public class InventoryElement : ChildElement, IHaveStudyFiles
    {

        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/1/2016 2:21:18 PM
        #endregion
        #region Fields
        private const string IMPORTED_FROM_OLD_FDA = "ImportedFromOldFDA";

        #endregion
        #region Properties
        public bool IsImportedFromOldFDA { get; set; }
        public InventorySelectionMapping SelectionMappings {get;}
        #endregion
        #region Constructors
        public InventoryElement(string name, string description, InventorySelectionMapping selections,  bool isImportedFromOldFDA, int id) 
            : base(name,"", description, id)
        {
            SelectionMappings = selections;
            IsImportedFromOldFDA = isImportedFromOldFDA;

            AddDefaultActions();
        }

        public InventoryElement(XElement inventoryElem, int id)
           : base(inventoryElem, id)
        {
            IsImportedFromOldFDA = Convert.ToBoolean( inventoryElem.Attribute(IMPORTED_FROM_OLD_FDA).Value);
            SelectionMappings = new InventorySelectionMapping(inventoryElem.Element(InventorySelectionMapping.INVENTORY_MAPPINGS));


            AddDefaultActions();
        }

        #endregion

        public override XElement ToXML()
        {
            XElement inventoryElem = new XElement(StringConstants.ELEMENT_XML_TAG);
            inventoryElem.Add(CreateHeaderElement());
            inventoryElem.SetAttributeValue(IMPORTED_FROM_OLD_FDA, IsImportedFromOldFDA);
            inventoryElem.Add(SelectionMappings.ToXML());
            return inventoryElem;
        }
     
    }
}
