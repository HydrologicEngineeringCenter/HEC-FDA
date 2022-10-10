using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.structures;
using HEC.FDA.ViewModel.ImpactArea;
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
            string[] files = Directory.GetFiles(Connection.Instance.InventoryDirectory + "\\" + Name);
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
            return new StructureInventoryColumnMap(
                structureID: SelectionMappings.StructureIDCol,
                occupancyType: SelectionMappings.OccTypeCol,
                firstFloorElev: SelectionMappings.FirstFloorElevCol,
                sructureValue: SelectionMappings.StructureValueCol,
                foundationHeight: SelectionMappings.FoundationHeightCol, groundElev: SelectionMappings.GroundElevCol, contentValue: SelectionMappings.ContentValueCol,
                otherValue: SelectionMappings.OtherValueCol, vehicalValue: SelectionMappings.VehicleValueCol, begDamDepth: SelectionMappings.BeginningDamageDepthCol,
                yearInConstruction: SelectionMappings.YearInConstructionCol

                ) ;
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

        public Model.structures.Inventory CreateModelInventory(ImpactAreaElement impactAreaElement)
        {
            List<OccupancyType> occupancyTypes = CreateModelOcctypes();
            string pointShapefilePath = GetStructuresPointShapefile();
            string impAreaShapefilePath = GetImpactAreaShapefile(impactAreaElement.Name);
            StructureInventoryColumnMap structureInventoryColumnMap = CreateColumnMap();
            Model.structures.Inventory inv = new Model.structures.Inventory(pointShapefilePath, impAreaShapefilePath,
                structureInventoryColumnMap, occupancyTypes, impactAreaElement.GetNameToIDPairs());
            return inv;
        }

        private FirstFloorElevationUncertainty CreateFirstFloorUncertainty(Statistics.ContinuousDistribution foundationHeightUncertainty)
        {
            //foundationHeightUncertainty.ToCoordinates
            //todo: not sure how to handle this
            FirstFloorElevationUncertainty firstFloorElevationUncertainty = new FirstFloorElevationUncertainty(foundationHeightUncertainty.Type, .5);

            return firstFloorElevationUncertainty;
        }

        private OccupancyType CreateModelOcctype(OccupancyTypes.OcctypeReference otRef)
        {
            OccupancyTypes.IOccupancyType ot = otRef.GetOccupancyType();
            UncertainPairedData structureUPD = ot.StructureItem.Curve.SelectedItemToPairedData();
            UncertainPairedData contentUPD = ot.ContentItem.Curve.SelectedItemToPairedData();
            UncertainPairedData vehicleUPD = ot.VehicleItem.Curve.SelectedItemToPairedData();
            UncertainPairedData otherUPD = ot.OtherItem.Curve.SelectedItemToPairedData();

            Statistics.ContinuousDistribution foundationHeightUncertainty = ot.FoundationHeightUncertainty;
            FirstFloorElevationUncertainty firstFloorElevationUncertainty = CreateFirstFloorUncertainty(foundationHeightUncertainty);

            OccupancyType occupancyType = OccupancyType.builder()
        .withName(ot.Name)
        .withDamageCategory(ot.DamageCategory)
        .withStructureDepthPercentDamage(structureUPD)
        .withContentDepthPercentDamage(contentUPD)
        .withVehicleDepthPercentDamage(vehicleUPD)
        .withOtherDepthPercentDamage(otherUPD)

        .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
        //.withStructureValueUncertainty(_structureValueUncertainty)
        //.withContentToStructureValueRatio(_contentToStructureValueRatio)
        .build();
            return occupancyType;
        }

        private List<OccupancyType> CreateModelOcctypes()
        {
            List<OccupancyType> occupancyTypes = new List<OccupancyType>();
            Dictionary<string, OccupancyTypes.OcctypeReference> occtypesDictionary = SelectionMappings.OcctypesDictionary;
            foreach(OccupancyTypes.OcctypeReference otRef in occtypesDictionary.Values)
            {
                occupancyTypes.Add(CreateModelOcctype(otRef));
            }
      
            return occupancyTypes;
        }


    }
}
