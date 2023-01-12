using System;
using System.Xml.Linq;

namespace HEC.FDA.Model.structures
{
    public class StructureSelectionMapping
    {
        public const string INVENTORY_MAPPINGS = "InventoryMappings";

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
        private const string BEG_DAMAGE_DEPTH = "BegDamDepth";

        private const string YEAR_IN_CONSTRUCTION = "YearInConstruction";
        private const string NOTES = "Notes";
        private const string DESCRIPTION = "Description";
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
        public string BeginningDamageDepthCol { get; }
        public string YearInConstructionCol { get; }

        public string NotesCol { get; }
        public string DescriptionCol { get; }
        public string NumberOfStructuresCol { get; }

        public string DamageCatagory { get; }
        public string CBFips { get; }

        public StructureSelectionMapping(bool FirstFloorElevationIsSelected, bool FromTerrainFileIsSelected, string structureIDRow,
            string occtype, string firstFloorElev, string structureValue, string foundHeight, string groundElev, string contentValue,
            string otherValue, string vehicleValue, string beginningDamageDepth, string yearInConstruction, string notes,
            string description, string numberOfStructures)
        {
            IsUsingFirstFloorElevation = FirstFloorElevationIsSelected;
            IsUsingTerrainFile = FromTerrainFileIsSelected;
            StructureIDCol = structureIDRow;
            OccTypeCol = occtype;
            FirstFloorElevCol = firstFloorElev;
            StructureValueCol = structureValue;
            FoundationHeightCol = foundHeight;
            GroundElevCol = groundElev;
            ContentValueCol = contentValue;
            OtherValueCol = otherValue;
            VehicleValueCol = vehicleValue;
            BeginningDamageDepthCol = beginningDamageDepth;
            YearInConstructionCol = yearInConstruction;
            NotesCol = notes;
            DescriptionCol = description;
            NumberOfStructuresCol = numberOfStructures;
        }

        public StructureSelectionMapping(XElement inventoryMappingElem)
        {
            XElement selections = inventoryMappingElem.Element(INVENTORY_COLUMN_SELECTIONS);

            IsUsingFirstFloorElevation = Convert.ToBoolean(selections.Attribute(FIRST_FLOOR_ELEV_SELECTED).Value);
            IsUsingTerrainFile = Convert.ToBoolean(selections.Attribute(FROM_TERRAIN_FILE).Value);

            StructureIDCol = selections.Element(STRUCTURE_ID).Attribute(VALUE).Value;

            OccTypeCol = selections.Element(OCCUPANCY_TYPE).Attribute(VALUE).Value;
            FirstFloorElevCol = selections.Element(FIRST_FLOOR_ELEV).Attribute(VALUE).Value;
            StructureValueCol = selections.Element(STRUCTURE_VALUE).Attribute(VALUE).Value;
            FoundationHeightCol = selections.Element(FOUNDATION_HEIGHT).Attribute(VALUE).Value;
            GroundElevCol = selections.Element(GROUND_ELEV).Attribute(VALUE).Value;
            ContentValueCol = selections.Element(CONTENT_VALUE).Attribute(VALUE).Value;
            OtherValueCol = selections.Element(OTHER_VALUE).Attribute(VALUE).Value;
            VehicleValueCol = selections.Element(VEHICLE_VALUE).Attribute(VALUE).Value;
            BeginningDamageDepthCol = selections.Element(BEG_DAMAGE_DEPTH).Attribute(VALUE).Value;
            YearInConstructionCol = selections.Element(YEAR_IN_CONSTRUCTION).Attribute(VALUE).Value;
            NotesCol = selections.Element(NOTES).Attribute(VALUE).Value;
            //for backwards compatability, check if it exists
            XElement descriptionElem = selections.Element(DESCRIPTION);
            if (descriptionElem != null)
            {
                DescriptionCol = descriptionElem.Attribute(VALUE).Value;
            }
            else
            {
                DescriptionCol = "";
            }
            NumberOfStructuresCol = selections.Element(NUMBER_OF_STRUCTURES).Attribute(VALUE).Value;
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
            columnSelectionsElem.Add(CreateColumnMappingXElement(BEG_DAMAGE_DEPTH, BeginningDamageDepthCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(YEAR_IN_CONSTRUCTION, YearInConstructionCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(NOTES, NotesCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(DESCRIPTION, DescriptionCol));
            columnSelectionsElem.Add(CreateColumnMappingXElement(NUMBER_OF_STRUCTURES, NumberOfStructuresCol));

            mappingsElem.Add(columnSelectionsElem);
            return mappingsElem;
        }

        private XElement CreateColumnMappingXElement(string elemName, string value)
        {
            XElement rowElem = new XElement(elemName);
            rowElem.SetAttributeValue(VALUE, value);
            return rowElem;
        }  

    }
}
