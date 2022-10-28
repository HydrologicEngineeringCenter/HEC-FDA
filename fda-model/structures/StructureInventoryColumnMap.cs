using System.Collections.Generic;

namespace HEC.FDA.Model.structures
{
    public class StructureInventoryColumnMap
    {
        public string StructureID { get; }
        public string OccupancyType { get; }
        public string DamageCatagory { get; }
        public string FirstFloorElev { get; }
        public string StructureValue { get; }
        public string FoundationHeight { get; }
        public string GroundElev { get; }
        public string ContentValue { get; }
        public string OtherValue { get; }
        public string VehicalValue { get; }
        public string BeginningDamageDepth { get; }
        public string YearInConstruction { get; }
        public string CBFips { get; }
        public List<string> ColumnHeaders
        {
            get
            {
                return new List<string> { StructureID, OccupancyType, DamageCatagory, FirstFloorElev, StructureValue, FoundationHeight, GroundElev, ContentValue, OtherValue, VehicalValue, BeginningDamageDepth,YearInConstruction,CBFips };
            }
        }

        public StructureInventoryColumnMap(string structureID = "fd_id", string occupancyType = "occtype", string damageCatagory = "st_damcat", string firstFloorElev = "ff_elev",
            string sructureValue = "val_struct", string foundationHeight = "found_ht", string groundElev = "ground_elv", string contentValue = "val_cont", string otherValue = "val_other",
            string vehicalValue = "val_vehic", string begDamDepth = "begDamDepth", string yearInConstruction = "yrbuilt", string cbfips = "cbfips")
        {
            StructureID = structureID;
            OccupancyType = occupancyType;
            DamageCatagory = damageCatagory;
            FirstFloorElev = firstFloorElev;
            StructureValue = sructureValue;
            FoundationHeight = foundationHeight;
            GroundElev = groundElev;
            ContentValue = contentValue;
            OtherValue = otherValue;
            VehicalValue = vehicalValue;
            BeginningDamageDepth = begDamDepth;
            YearInConstruction = yearInConstruction;
            CBFips = cbfips;
        }
    }
}
