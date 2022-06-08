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
        private List<double> _computeStages;
        public int Fid { get; }
        public PointM Point { get; }
        public double FoundationHeightMean { get; }
        public double ValueStructureMean { get; }
        public double ValueContentMean { get; }
        public double ValueVehicleMean { get; }
        public double ValueOtherMean { get; }
        public string DamageCatagory { get; }
        public string OccTypeName { get; }
        public int ImpactAreaID { get; }
        public string Cbfips { get; }

        public Structure(int fid, PointM point, double found_ht, double val_struct, double val_cont, double val_vehic, double val_other, string st_damcat, string occtype, int impactAreaID, string cbfips)
        {
            Fid = fid;
            Point = point;
            FoundationHeightMean = found_ht;
            ValueStructureMean = val_struct;
            ValueContentMean = val_cont;
            ValueVehicleMean = val_vehic;
            ValueOtherMean = val_other;
            DamageCatagory = st_damcat;
            OccTypeName = occtype;
            ImpactAreaID = impactAreaID;
            Cbfips = cbfips;
        }

        public DeterministicStructure Sample(int seed, DeterministicOccupancyType occtype)
        {
            Random random = new Random(seed);

            double foundHeightSample = FoundationHeightMean * occtype.FoundationHeightError;
            double structValueSample = ValueStructureMean * occtype.StructureValueError;
            double contentValueSample = ValueContentMean * occtype.ContentValueError;
            double vehicleValueSample = ValueVehicleMean * occtype.VehicleValueError;
            double otherValueSample = ValueOtherMean * occtype.OtherValueError;

            //load up the deterministic structure
            return new DeterministicStructure(Fid,ImpactAreaID,DamageCatagory,occtype, foundHeightSample,structValueSample,contentValueSample,vehicleValueSample,otherValueSample);
        }



    }
}