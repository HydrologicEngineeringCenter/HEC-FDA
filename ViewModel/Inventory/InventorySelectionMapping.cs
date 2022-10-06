using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Inventory
{
    public class InventorySelectionMapping
    {
        private const string SHAPEFILE_OCCTYPE = "ShapefileOcctype";
        private const string GROUP_ID = "GroupID";
        private const string ID = "ID";
        public const string INVENTORY_MAPPINGS = "InventoryMappings";
        private const string OCCTYPE_MAPPINGS = "OcctypeMappings";
        private const string OCCTYPE_MAPPING = "OcctypeMapping";

        private const string INVENTORY_COLUMN_SELECTIONS = "InventoryColumnSelections";
        private const string FIRST_FLOOR_ELEV_SELECTED = "FirstFloorElevSelected";
        private const string FROM_TERRAIN_FILE = "FromTerrainFileSelected";
        private const string STRUCTURE_ID = "StructureID";
        private const string OCCUPANCY_TYPE = "OccupancyType";

        private const string FIRST_FLOOR_ELEV = "FirstFloorElev";
        private const string STRUCTURE_VALUE = "StructureValue";
        private const string FOUNDATION_HEIGHT = "FoundationHeight";

        private const string GROUND_ELEV = "GroundElev";
        private const string CONTENT_VALUE = "ContentValue";
        private const string OTHER_VALUE = "OtherValue";
        private const string VEHICLE_VALUE = "VehicleValue";
        private const string MODULE = "Module";
        private const string BEG_DAMAGE_DEPTH = "BegDamDepth";

        private const string YEAR_IN_CONSTRUCTION = "YearInConstruction";
        private const string NOTES = "Notes";
        private const string NUMBER_OF_STRUCTURES = "NumberOfStructures";

        private const string VALUE = "Value";


        public bool IsUsingFirstFloorElevation { get; }
        public bool IsUsingTerrainFile { get; }
        public string StructureIDCol { get; }
        public string OccTypeCol { get; }
        public string FirstFloorElevCol { get; }
        public string StructureValueCol { get; }
        public string FoundationHeightCol { get; }
        public string GroundElevCol { get; }
        public string ContentValueCol { get; }
        public string OtherValueCol { get; }
        public string VehicleValueCol { get; }
        public string ModuleCol { get; }
        public string BeginningDamageDepthCol { get; }
        public string YearInConstructionCol { get; }
        public string NotesCol { get; }
        public string NumberOfStructuresCol { get; }
        //             occtype name, occtype reference
        public Dictionary<string, OcctypeReference> OcctypesDictionary { get; } = new Dictionary<string, OcctypeReference>();

        public InventorySelectionMapping(InventoryColumnSelectionsVM selections, Dictionary<string, OcctypeReference> occtypeDictionary)
        {
            IsUsingFirstFloorElevation = selections.FirstFloorElevationIsSelected;
            IsUsingTerrainFile = selections.FromTerrainFileIsSelected;
            StructureIDCol = selections._StructureIDRow.SelectedItem;
            OccTypeCol = selections._OccupancyTypeRow.SelectedItem;
            FirstFloorElevCol = selections._FirstFloorElevRow.SelectedItem;
            StructureValueCol = selections._StructureValueRow.SelectedItem;
            FoundationHeightCol = selections._FoundationHeightRow.SelectedItem;
            GroundElevCol = selections._GroundElevRow.SelectedItem;
            ContentValueCol = selections._ContentValueRow.SelectedItem;
            OtherValueCol = selections._OtherValueRow.SelectedItem;
            VehicleValueCol = selections._VehicleValueRow.SelectedItem;
            ModuleCol = selections._ModuleRow.SelectedItem;
            BeginningDamageDepthCol = selections._BegDamDepthRow.SelectedItem;
            YearInConstructionCol = selections._YearInConstructionRow.SelectedItem;
            NotesCol = selections._NotesRow.SelectedItem;
            NumberOfStructuresCol = selections._NumberOfStructuresRow.SelectedItem;

            OcctypesDictionary = occtypeDictionary;
        }



        public InventorySelectionMapping( XElement inventoryMappingElem)
        {
            XElement selections = inventoryMappingElem.Element(INVENTORY_COLUMN_SELECTIONS);

            IsUsingFirstFloorElevation = Convert.ToBoolean( selections.Attribute(FIRST_FLOOR_ELEV_SELECTED).Value);
            IsUsingTerrainFile = Convert.ToBoolean( selections.Attribute(FROM_TERRAIN_FILE).Value);

            StructureIDCol = selections.Element(STRUCTURE_ID).Attribute(VALUE).Value;

            OccTypeCol = selections.Element(OCCUPANCY_TYPE).Attribute(VALUE).Value;
            FirstFloorElevCol = selections.Element(FIRST_FLOOR_ELEV).Attribute(VALUE).Value;
            StructureValueCol = selections.Element(STRUCTURE_VALUE).Attribute(VALUE).Value;
            FoundationHeightCol = selections.Element(FOUNDATION_HEIGHT).Attribute(VALUE).Value;
            GroundElevCol = selections.Element(GROUND_ELEV).Attribute(VALUE).Value;
            ContentValueCol = selections.Element(CONTENT_VALUE).Attribute(VALUE).Value;
            OtherValueCol = selections.Element(OTHER_VALUE).Attribute(VALUE).Value;
            VehicleValueCol = selections.Element(VEHICLE_VALUE).Attribute(VALUE).Value;
            ModuleCol = selections.Element(MODULE).Attribute(VALUE).Value;
            BeginningDamageDepthCol = selections.Element(BEG_DAMAGE_DEPTH).Attribute(VALUE).Value;
            YearInConstructionCol = selections.Element(YEAR_IN_CONSTRUCTION).Attribute(VALUE).Value;
            NotesCol = selections.Element(NOTES).Attribute(VALUE).Value;
            NumberOfStructuresCol = selections.Element(NUMBER_OF_STRUCTURES).Attribute(VALUE).Value;

            XElement occtypeMappings = inventoryMappingElem.Element(OCCTYPE_MAPPINGS);
            IEnumerable<XElement> occtypeMappingElements = occtypeMappings.Elements(OCCTYPE_MAPPING);
            foreach(XElement occtypeMappingElement in occtypeMappingElements)
            {
                string shapefileOcctypeName = occtypeMappingElement.Attribute(SHAPEFILE_OCCTYPE).Value;
                int groupID = Convert.ToInt32(occtypeMappingElement.Attribute(GROUP_ID).Value);
                int id = Convert.ToInt32(occtypeMappingElement.Attribute(ID).Value);
                OcctypeReference otRef = new OcctypeReference(groupID, id);
                OcctypesDictionary.Add(shapefileOcctypeName, otRef);
            }
        }

       

        public XElement ToXML()
        {
            XElement mappingsElem = new XElement(INVENTORY_MAPPINGS);

            XElement columnSelectionsElem = new XElement(INVENTORY_COLUMN_SELECTIONS);
            columnSelectionsElem.SetAttributeValue(FIRST_FLOOR_ELEV_SELECTED, IsUsingFirstFloorElevation);
            columnSelectionsElem.SetAttributeValue(FROM_TERRAIN_FILE, IsUsingTerrainFile);

            columnSelectionsElem.Add(CreateColumnMappingXElement(STRUCTURE_ID, StructureIDCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(OCCUPANCY_TYPE, OccTypeCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(FIRST_FLOOR_ELEV, FirstFloorElevCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(STRUCTURE_VALUE, StructureValueCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(FOUNDATION_HEIGHT, FoundationHeightCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(GROUND_ELEV, GroundElevCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(CONTENT_VALUE, ContentValueCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(OTHER_VALUE, OtherValueCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(VEHICLE_VALUE, VehicleValueCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(MODULE, ModuleCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(BEG_DAMAGE_DEPTH, BeginningDamageDepthCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(YEAR_IN_CONSTRUCTION, YearInConstructionCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(NOTES, NotesCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(NUMBER_OF_STRUCTURES, NumberOfStructuresCol));

            XElement occtypesElem = new XElement(OCCTYPE_MAPPINGS);
            foreach(KeyValuePair<string, OcctypeReference> pair in OcctypesDictionary)
            {
                occtypesElem.Add(CreateOcctypeMappingXElement(pair.Key, pair.Value));
            }

            mappingsElem.Add(occtypesElem);
            mappingsElem.Add(columnSelectionsElem);
            return mappingsElem;

        }

        

        private XElement CreateOcctypeMappingXElement(String shapefileOcctype, OcctypeReference fDAOcctype)
        {
            XElement rowElem = new XElement(OCCTYPE_MAPPING);
            rowElem.SetAttributeValue(SHAPEFILE_OCCTYPE, shapefileOcctype);
            rowElem.SetAttributeValue(GROUP_ID, fDAOcctype.GroupID);
            rowElem.SetAttributeValue(ID, fDAOcctype.ID);
            return rowElem;
        }
     

        private XElement CreateColumnMappingXElement(string elemName, string value)
        {
            XElement rowElem = new XElement(elemName);
            rowElem.SetAttributeValue(VALUE, value);
            return rowElem;
        }

    }
}
