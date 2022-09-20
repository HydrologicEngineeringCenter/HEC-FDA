using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fda_model.structures
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

        public StructureInventoryColumnMap(string structureID, string occupancyType,string damageCatagory, string firstFloorElev, string sructureValue, string foundationHeight, string groundElev, string contentValue, string otherValue, string vehicalValue, string begDamDepth, string yearInConstruction, string cbfips)
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
