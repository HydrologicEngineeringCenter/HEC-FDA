using OpenGLMapping;
using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;

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
        /// <summary>
        /// This is a unique id that can be used to identify the element in the map window and map tree view.
        /// This can be used when doing a rename from the study tree, or opening the attribute table from the study tree.
        /// </summary>
        private int _featureHashCode;
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
        public InventoryElement(StructureInventoryBaseElement structInventoryBaseElement, bool isImportedFromOldFDA, int id) : base(id)
        {
            IsImportedFromOldFDA = isImportedFromOldFDA;
            Name = structInventoryBaseElement.Name;
            CustomTreeViewHeader = new CustomHeaderVM(Name, ImageSources.INVENTORY_ELEMENT_IMAGE);

            Description = structInventoryBaseElement.Description;
            if(Description == null) { Description = ""; }

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

            TableContainsGeoData = true;

        }
        #endregion
        #region Voids
        public void OpenAttributeTable(object sender, EventArgs e)
        {
            LifeSimGIS.GeoPackageReader gpr = new LifeSimGIS.GeoPackageReader(Storage.Connection.Instance.Reader);

            LifeSimGIS.PointFeatures pointFeatures = new LifeSimGIS.PointFeatures();
            if (!IsImportedFromOldFDA)
            {
                pointFeatures = (LifeSimGIS.PointFeatures)gpr.ConvertToGisFeatures(_TableConstant + this.Name);
            }
            LifeSimGIS.VectorFeatures features = pointFeatures;
            
            DatabaseManager.DataTableView dtv = Storage.Connection.Instance.Reader.GetTableManager(_TableConstant + this.Name);

            OpenGLDrawInfo ogldi = new OpenGLDrawInfo(15, OpenGLDrawInfo.GlyphType.House1, true, new OpenTK.Graphics.Color4((byte)0, 0, 0, 255), true, new OpenTK.Graphics.Color4((byte)0, 0, 255, 200), true);

            OpenStructureAttributeTableEventArgs args = new OpenStructureAttributeTableEventArgs(Name, features, dtv, ogldi);
            args.MapFeatureHash = _featureHashCode;
        }

        #endregion
        #region Functions
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            InventoryElement elem = (InventoryElement)elementToClone;
            return new InventoryElement(elem.StructureInventory, elem.IsImportedFromOldFDA, elem.ID);
        }
        #endregion

    }
}
