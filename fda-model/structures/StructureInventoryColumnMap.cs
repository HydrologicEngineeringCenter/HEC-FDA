using System;
using System.Collections.Generic;

namespace HEC.FDA.Model.structures
{
    public class StructureInventoryColumnMap
    {
        public string StructureID { get; } = "fd_id";
        public string OccupancyType { get; } = "occtype";
        public string DamageCatagory { get; } = "st_damcat";
        public string FirstFloorElev { get; } = "ff_elev";
        public string StructureValue { get; } = "val_struct";
        public string FoundationHeight { get; } = "found_ht";
        public string GroundElev { get; } = "ground_elv";
        public string ContentValue { get; } = "val_cont";
        public string OtherValue { get; } = "val_other";
        public string VehicalValue { get; } = "val_vehic";
        public string BeginningDamageDepth { get; } = "begDamDepth";
        public string YearInConstruction { get; } = "yrbuilt";
        public string CBFips { get; } = "cbfips";
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

        public StructureInventoryColumnMap(string structureID , string occupancyType, string damageCatagory , string firstFloorElev ,
            string sructureValue, string foundationHeight , string groundElev , string contentValue , string otherValue ,
            string vehicalValue , string begDamDepth , string yearInConstruction, string cbfips )
        {
            if (!String.IsNullOrEmpty(structureID)){
                StructureID = structureID;
            }
            if (!String.IsNullOrEmpty(occupancyType))
            {
                OccupancyType = occupancyType;
            }
            if (!String.IsNullOrEmpty(damageCatagory))
            {
                DamageCatagory = damageCatagory;
            }
            if (!String.IsNullOrEmpty(firstFloorElev))
            {
                FirstFloorElev = firstFloorElev;
            }
            if (!String.IsNullOrEmpty(foundationHeight))
            {
                FoundationHeight =foundationHeight;
            }
            if (!String.IsNullOrEmpty(groundElev))
            {
                GroundElev = groundElev;
            }
            if (!String.IsNullOrEmpty(otherValue))
            {
                OtherValue = otherValue;
            }
            if (!String.IsNullOrEmpty(vehicalValue))
            {
                VehicalValue = vehicalValue;
            }
            if (!String.IsNullOrEmpty(begDamDepth))
            {
                BeginningDamageDepth = begDamDepth;
            }
            if (!String.IsNullOrEmpty(YearInConstruction))
            {
                YearInConstruction = yearInConstruction;
            }
            if (!String.IsNullOrEmpty(StructureValue))
            {
                StructureValue = sructureValue;
            }
            if (!String.IsNullOrEmpty(contentValue))
            {
                ContentValue = contentValue;
            }
            if (!String.IsNullOrEmpty(cbfips))
            {
                CBFips = cbfips;
            }
            
        }
    }
}
