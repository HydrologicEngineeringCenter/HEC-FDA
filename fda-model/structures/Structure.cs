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
        private List<double> _computeStages;
        public int Fid { get; }
        public PointM Point { get; }
        public double FirstFloorElevation { get; }
        public double ValueStructureMean { get; }
        public double ValueContentMean { get; internal set; }
        public double ValueVehicleMean { get; }
        public double ValueOtherMean { get; internal set; }
        public string DamageCatagory { get; }
        public string OccTypeName { get; }
        public int ImpactAreaID { get; }
        public string Cbfips { get; }

        public Structure(int fid, PointM point, double found_ht,  double groundElevation, double val_struct, double val_cont, double val_vehic, double val_other, string st_damcat, string occtype, int impactAreaID, string cbfips)
        {
            Fid = fid;
            Point = point;
            ValueStructureMean = val_struct;
            ValueContentMean = val_cont;
            ValueVehicleMean = val_vehic;
            ValueOtherMean = val_other;
            DamageCatagory = st_damcat;
            OccTypeName = occtype;
            ImpactAreaID = impactAreaID;
            Cbfips = cbfips;
            FirstFloorElevation = found_ht + groundElevation;
        }
        public Structure(int fid, PointM point, double firstFloorElevation, double val_struct, double val_cont, double val_vehic, double val_other, string st_damcat, string occtype, int impactAreaID, string cbfips)
        {
            Fid = fid;
            Point = point;
            ValueStructureMean = val_struct;
            ValueContentMean = val_cont;
            ValueVehicleMean = val_vehic;
            ValueOtherMean = val_other;
            DamageCatagory = st_damcat;
            OccTypeName = occtype;
            ImpactAreaID = impactAreaID;
            Cbfips = cbfips;
            FirstFloorElevation = firstFloorElevation;
 
        }
        public DeterministicStructure Sample(IProvideRandomNumbers randomProvider, OccupancyType occtype)
        {
            SampledStructureParameters sampledStructureParameters = occtype.Sample(randomProvider, ValueStructureMean, FirstFloorElevation, ValueContentMean, ValueOtherMean, ValueVehicleMean);
            //load up the deterministic structure
            return new DeterministicStructure(Fid,ImpactAreaID,DamageCatagory,sampledStructureParameters);
        }



    }
}