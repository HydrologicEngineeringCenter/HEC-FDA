using System;
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
        public Dictionary<string,Type> ColumnHeaders
        {
            get
            {
               var rtn = new Dictionary<string, Type>() ;
                rtn.Add(StructureID, typeof(int));
                rtn.Add(OccupancyType, typeof(string));
                rtn.Add(DamageCatagory, typeof(string));
                rtn.Add(FirstFloorElev, typeof(double));
                rtn.Add(StructureValue, typeof(double));
                rtn.Add(FoundationHeight, typeof(double));
                rtn.Add(GroundElev, typeof(double));
                rtn.Add(ContentValue, typeof(double));
                rtn.Add(OtherValue, typeof(double));
                rtn.Add(VehicalValue, typeof(double));
                rtn.Add(BeginningDamageDepth, typeof(double));
                rtn.Add(YearInConstruction, typeof(double));
                rtn.Add(CBFips, typeof(double));
                return rtn;
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
