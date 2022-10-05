using HEC.FDA.Model.structures;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
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

        //todo: maybe replace my mapping with this object?
        private StructureInventoryColumnMap CreateColumnMap()
        {
            return new StructureInventoryColumnMap();
        }

        public string GetImpactAreaDirectory(string impactAreaName)
        {
            return Connection.Instance.ImpactAreaDirectory + "\\" + impactAreaName;
        }

        public string GetImpactAreaShapefile(string impactAreaName)
        {
            return Directory.GetFiles(GetImpactAreaDirectory(impactAreaName), "*.shp")[0];
        }

        private string GetStructuresDirectory()
        {
            return Connection.Instance.InventoryDirectory + "\\" + Name;
        }

        private string GetStructuresPointShapefile()
        {
            return Directory.GetFiles(GetStructuresDirectory(), "*.shp")[0];
        }

        public Model.structures.Inventory CreateModelInventory(string impactAreaName)
        {
            List<OccupancyType> occupancyTypes = CreateModelOcctypes();
            //Storage.Connection.Instance.InventoryDirectory.
            string pointShapefilePath = GetStructuresPointShapefile();
            string impAreaShapefilePath = GetImpactAreaShapefile(impactAreaName);
            StructureInventoryColumnMap structureInventoryColumnMap = CreateColumnMap();
            StructureInventoryColumnMap colMap = new StructureInventoryColumnMap();
            Model.structures.Inventory inv = new Model.structures.Inventory(pointShapefilePath, impAreaShapefilePath,
                structureInventoryColumnMap, occupancyTypes);
            return inv;
        }

        private List<OccupancyType> CreateModelOcctypes()
        {
        //    double[] expectedPercentDamage = new double[] { 0, .10, .20, .30, .40, .50 };
        //    CurveMetaData metaData = new CurveMetaData("Depths", "Percent Damage", "Depth-Percent Damage Function");
        //    UncertainPairedData _StructureDepthPercentDamageFunction = new UncertainPairedData(depths, percentDamages, metaData);
        //    UncertainPairedData _ContentDepthPercentDamageFunction = new UncertainPairedData(depths, percentDamages, metaData);
        //    FirstFloorElevationUncertainty firstFloorElevationUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Normal, 0.5);
        //    ValueUncertainty _structureValueUncertainty = new ValueUncertainty(IDistributionEnum.Normal, .1);
        //    ValueRatioWithUncertainty _contentToStructureValueRatio = new ValueRatioWithUncertainty(IDistributionEnum.Normal, .1, .9);
        //    expectedCSVR = 0.9;
        //    MedianRandomProvider medianRandomProvider = new MedianRandomProvider();
        //    string name = "MyOccupancyType";
        //    string damageCategory = "DamageCategory";


        //    OccupancyType occupancyType = OccupancyType.builder()
        //.withName(name)
        //.withDamageCategory(damageCategory)
        //.withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
        //.withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
        //.withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
        //.withStructureValueUncertainty(_structureValueUncertainty)
        //.withContentToStructureValueRatio(_contentToStructureValueRatio)
        //.build();
            return new List<OccupancyType>();
        }


    }
}
