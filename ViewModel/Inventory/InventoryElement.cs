using HEC.FDA.ViewModel.Utilities;
using System;
using System.IO;
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

            AddDefaultActions(EditElement, StringConstants.EDIT_STRUCTURES_MENU);
        }

        public InventoryElement(XElement inventoryElem, int id)
           : base(inventoryElem, id)
        {
            IsImportedFromOldFDA = Convert.ToBoolean( inventoryElem.Attribute(IMPORTED_FROM_OLD_FDA).Value);
            SelectionMappings = new InventorySelectionMapping(inventoryElem.Element(InventorySelectionMapping.INVENTORY_MAPPINGS));


            AddDefaultActions(EditElement,StringConstants.EDIT_STRUCTURES_MENU);
        }

        #endregion

        public void EditElement(object sender, EventArgs e)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
               .WithSiblingRules(this);
            ImportStructuresFromShapefileVM vm = new ImportStructuresFromShapefileVM(this, actionManager);
            string header = StringConstants.IMPORT_STRUCTURE_INVENTORIES_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, StringConstants.IMPORT_STRUCTURE_INVENTORIES_HEADER);
            Navigate(tab, false, false);

        }

        public override XElement ToXML()
        {
            XElement inventoryElem = new XElement(StringConstants.ELEMENT_XML_TAG);
            inventoryElem.Add(CreateHeaderElement());
            inventoryElem.SetAttributeValue(IMPORTED_FROM_OLD_FDA, IsImportedFromOldFDA);
            inventoryElem.Add(SelectionMappings.ToXML());
            return inventoryElem;
        }

        /// <summary>
        /// Gets a file from this inventory elements folder in the study directory.
        /// </summary>
        /// <param name="extension">the extension needs to include the period. ie: ".dbf"</param>
        /// <returns></returns>
        public string GetFilePath(string extension)
        {
            string path = null;
            string[] files = Directory.GetFiles(Storage.Connection.Instance.InventoryDirectory + "\\" + Name);
            foreach (string file in files)
            {
                if(Path.GetExtension(file).Equals(extension))
                {
                    path = file;
                    break;
                }
            }
            return path;
        }
     
    }
}
