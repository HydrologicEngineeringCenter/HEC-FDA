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
        public List<Tuple<string, Type>> ColumnHeaders
        {
            get
            {
                var rtn = new List<Tuple<string, Type>>();
                rtn.Add(new Tuple<string, Type>(StructureID, typeof(int)));
                rtn.Add(new Tuple<string, Type>(OccupancyType, typeof(string)));
                rtn.Add(new Tuple<string, Type>(DamageCatagory, typeof(string)));
                rtn.Add(new Tuple<string, Type>(FirstFloorElev, typeof(double)));
                rtn.Add(new Tuple<string, Type>(StructureValue, typeof(double)));
                rtn.Add(new Tuple<string, Type>(FoundationHeight, typeof(double)));
                rtn.Add(new Tuple<string, Type>(GroundElev, typeof(double)));
                rtn.Add(new Tuple<string, Type>(ContentValue, typeof(double)));
                rtn.Add(new Tuple<string, Type>(OtherValue, typeof(double)));
                rtn.Add(new Tuple<string, Type>(VehicalValue, typeof(double)));
                rtn.Add(new Tuple<string, Type>(BeginningDamageDepth, typeof(double)));
                rtn.Add(new Tuple<string, Type>(YearInConstruction, typeof(double)));
                rtn.Add(new Tuple<string, Type>(CBFips, typeof(double)));
                return rtn;
            }
        }

        public StructureInventoryColumnMap(string structureID , string occupancyType, string damageCatagory , string firstFloorElev ,
            string sructureValue, string foundationHeight , string groundElev , string contentValue , string otherValue ,
            string vehicalValue , string begDamDepth , string yearInConstruction, string cbfips, string numStructures )
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
                FoundationHeight = foundationHeight;
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
            if (!String.IsNullOrEmpty(yearInConstruction))
            {
                YearInConstruction = yearInConstruction;
            }
            if (!String.IsNullOrEmpty(sructureValue))
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
            if (!String.IsNullOrEmpty(numStructures))
            {
                NumberOfStructures = numStructures;
            }
            
        }
    }
}
