using interfaces;
using RasMapperLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace structures
{
    public class Structure
    {
        //TODO: How are we going to handle missing data?
        //For now, we won't allow missing data 
        public int Fid { get; }
        public PointM Point { get; }
        public double FirstFloorElevation { get; }
        public double InventoriedStructureValue { get; }
        public double InventoriedContentValue { get; internal set; }
        public double InventoriedVehicleValue { get; }
        public double InventoriedOtherValue { get; internal set; }
        public string DamageCatagory { get; }
        public string OccTypeName { get; }
        public int ImpactAreaID { get; }
        public string Cbfips { get; }

        public Structure(int fid, PointM point, double firstFloorElevation, double val_struct, string st_damcat, string occtype, int impactAreaID, double val_cont = -999, double val_vehic = -999, double val_other = -999, string cbfips = "unassigned")
        {
            Fid = fid;
            Point = point;
            InventoriedStructureValue = val_struct;
            InventoriedContentValue = val_cont;
            InventoriedVehicleValue = val_vehic;
            InventoriedOtherValue = val_other;
            DamageCatagory = st_damcat;
            OccTypeName = occtype;
            ImpactAreaID = impactAreaID;
            Cbfips = cbfips;
            FirstFloorElevation = firstFloorElevation;
 
        }
        public DeterministicStructure Sample(IProvideRandomNumbers randomProvider, OccupancyType occtype)
        {
            SampledStructureParameters sampledStructureParameters = occtype.Sample(randomProvider, InventoriedStructureValue, FirstFloorElevation, InventoriedContentValue, InventoriedOtherValue, InventoriedVehicleValue);
            //load up the deterministic structure
            return new DeterministicStructure(Fid,ImpactAreaID,sampledStructureParameters);
        }



    }
}