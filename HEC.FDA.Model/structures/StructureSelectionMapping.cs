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
        }

        private void SetExpectedTypes()
        {
            _expectedTypes = new Dictionary<string, Type>()
            {
                { StructureIDCol, typeof(string) },
                { OccTypeCol, typeof(string) },
                { FirstFloorElevCol, typeof(double) },
                { StructureValueCol, typeof(double) },
                { FoundationHeightCol, typeof(double) },
                { GroundElevCol, typeof(double) },
                { ContentValueCol, typeof(double) },
                { OtherValueCol, typeof(double) },
                { VehicleValueCol, typeof(double) },
                { BeginningDamageDepthCol, typeof(double) },
                { YearInConstructionCol, typeof(int) },
                { NotesCol, typeof(string) },
                { DescriptionCol, typeof(string) },
                { NumberOfStructuresCol, typeof(int) },
                { DamageCatagory, typeof(string) },
                { CBFips, typeof(string) }
            };
        }

        public Type GetExpectedType(string columnName)
        {
            if (_expectedTypes == null)
            {
                SetExpectedTypes();
            }
            return _expectedTypes[columnName];

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
