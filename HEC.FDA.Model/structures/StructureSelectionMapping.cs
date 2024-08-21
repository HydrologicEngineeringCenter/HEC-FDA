using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.Model.structures
{
    public class StructureSelectionMapping
    {
        public const string INVENTORY_MAPPINGS = "InventoryMappings";

        private const string INVENTORY_COLUMN_SELECTIONS = "InventoryColumnSelections";
        private const string FIRST_FLOOR_ELEV_SELECTED = "FirstFloorElevSelected";
        private const string FROM_TERRAIN_FILE = "FromTerrainFileSelected";
        public const string STRUCTURE_ID = "StructureID";
        public const string OCCUPANCY_TYPE = "OccupancyType";

        public const string FIRST_FLOOR_ELEV = "FirstFloorElev";
        public const string STRUCTURE_VALUE = "StructureValue";
        public const string FOUNDATION_HEIGHT = "FoundationHeight";

        public const string GROUND_ELEV = "GroundElev";
        public const string CONTENT_VALUE = "ContentValue";
        public const string OTHER_VALUE = "OtherValue";
        public const string VEHICLE_VALUE = "VehicleValue";
        public const string BEG_DAMAGE_DEPTH = "BegDamDepth";

        public const string YEAR_IN_CONSTRUCTION = "YearInConstruction";
        public const string NOTES = "Notes";
        public const string DESCRIPTION = "Description";
        public const string NUMBER_OF_STRUCTURES = "NumberOfStructures";

        private const string VALUE = "Value";

        private Dictionary<string, Type> _expectedTypes;

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
            BuildExpectedTypesDictionary();
        }

        public StructureSelectionMapping(XElement inventoryMappingElem)
        {
            XElement selections = inventoryMappingElem.Element(INVENTORY_COLUMN_SELECTIONS);

            IsUsingFirstFloorElevation = Convert.ToBoolean(selections.Attribute(FIRST_FLOOR_ELEV_SELECTED).Value);
            IsUsingTerrainFile = Convert.ToBoolean(selections.Attribute(FROM_TERRAIN_FILE).Value);

            StructureIDCol = selections.Element(STRUCTURE_ID).Attribute(VALUE).Value;

            OccTypeCol = GetXMLValue(selections, OCCUPANCY_TYPE);
            FirstFloorElevCol = GetXMLValue(selections, FIRST_FLOOR_ELEV);
            StructureValueCol = GetXMLValue(selections, STRUCTURE_VALUE);
            FoundationHeightCol = GetXMLValue(selections, FOUNDATION_HEIGHT);
            GroundElevCol = GetXMLValue(selections, GROUND_ELEV);
            ContentValueCol = GetXMLValue(selections, CONTENT_VALUE);
            OtherValueCol = GetXMLValue(selections, OTHER_VALUE);
            VehicleValueCol = GetXMLValue(selections, VEHICLE_VALUE);
            BeginningDamageDepthCol = GetXMLValue(selections, BEG_DAMAGE_DEPTH);
            YearInConstructionCol = GetXMLValue(selections, YEAR_IN_CONSTRUCTION);
            NotesCol = GetXMLValue(selections, NOTES);
            DescriptionCol = GetXMLValue(selections, DESCRIPTION);
            NumberOfStructuresCol = GetXMLValue(selections, NUMBER_OF_STRUCTURES);
            BuildExpectedTypesDictionary();
        }

        private void BuildExpectedTypesDictionary()
        {
            _expectedTypes = new Dictionary<string, Type>();

            if (!string.IsNullOrEmpty(StructureIDCol))
            {
                _expectedTypes.Add(STRUCTURE_ID, typeof(string));
            }

            if (!string.IsNullOrEmpty(OccTypeCol))
            {
                _expectedTypes.Add(OCCUPANCY_TYPE, typeof(string));
            }

            if (!string.IsNullOrEmpty(FirstFloorElevCol))
            {
                _expectedTypes.Add(FIRST_FLOOR_ELEV, typeof(double));
            }

            if (!string.IsNullOrEmpty(StructureValueCol))
            {
                _expectedTypes.Add(STRUCTURE_VALUE, typeof(double));
            }

            if (!string.IsNullOrEmpty(FoundationHeightCol))
            {
                _expectedTypes.Add(FOUNDATION_HEIGHT, typeof(double));
            }

            if (!string.IsNullOrEmpty(GroundElevCol))
            {
                _expectedTypes.Add(GROUND_ELEV, typeof(double));
            }

            if (!string.IsNullOrEmpty(ContentValueCol))
            {
                _expectedTypes.Add(CONTENT_VALUE, typeof(double));
            }

            if (!string.IsNullOrEmpty(OtherValueCol))
            {
                _expectedTypes.Add(OTHER_VALUE, typeof(double));
            }

            if (!string.IsNullOrEmpty(VehicleValueCol))
            {
                _expectedTypes.Add(VEHICLE_VALUE, typeof(double));
            }

            if (!string.IsNullOrEmpty(BeginningDamageDepthCol))
            {
                _expectedTypes.Add(BEG_DAMAGE_DEPTH, typeof(double));
            }

            if (!string.IsNullOrEmpty(YearInConstructionCol))
            {
                _expectedTypes.Add(YEAR_IN_CONSTRUCTION, typeof(int));
            }

            if (!string.IsNullOrEmpty(NotesCol))
            {
                _expectedTypes.Add(NOTES, typeof(string));
            }

            if (!string.IsNullOrEmpty(DescriptionCol))
            {
                _expectedTypes.Add(DESCRIPTION, typeof(string));
            }

            if (!string.IsNullOrEmpty(NumberOfStructuresCol))
            {
                _expectedTypes.Add(NUMBER_OF_STRUCTURES, typeof(int));
            }
        }

        public Type GetExpectedType(string valueName)
        {
            return _expectedTypes[valueName];
        }

        private static string GetXMLValue(XElement parentElem, string elemName)
        {
            string xmlValue = string.Empty;
            if (parentElem.Element(elemName) != null && parentElem.Element(elemName).Attribute(VALUE) != null)
            {
                xmlValue = parentElem.Element(elemName).Attribute(VALUE).Value;
            }
            return xmlValue;
        }

        public XElement ToXML()
        {
            XElement mappingsElem = new(INVENTORY_MAPPINGS);

            XElement columnSelectionsElem = new(INVENTORY_COLUMN_SELECTIONS);
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

        private static XElement CreateColumnMappingXElement(string elemName, string value)
        {
            XElement rowElem = new(elemName);
            rowElem.SetAttributeValue(VALUE, value);
            return rowElem;
        }

    }
}
