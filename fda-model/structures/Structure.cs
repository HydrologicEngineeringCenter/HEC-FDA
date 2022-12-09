using HEC.FDA.Model.interfaces;
using RasMapperLib;
using System;

namespace HEC.FDA.Model.structures
{
    public class Structure
    {
        //TODO: How are we going to handle missing data?
        //For now, we won't allow missing data 
        public int Fid { get; }
        public PointM Point { get; }
        public double FirstFloorElevation { get; }
        public double GroundElevation { get; }
        public double InventoriedStructureValue { get; }
        public double InventoriedContentValue { get; set; }
        public double InventoriedVehicleValue { get; set; }
        public double InventoriedOtherValue { get; set; }
        public string DamageCatagory { get; }
        public string OccTypeName { get; }
        public int ImpactAreaID { get; }
        public string Cbfips { get; set; }
        internal double BeginningDamageDepth { get; }
        internal double FoundationHeight { get; }
        internal int YearInService { get; }
        internal int NumberOfStructures { get; }

        public Structure(int fid, PointM point, double firstFloorElevation, double val_struct, string st_damcat, string occtype, int impactAreaID, double val_cont =0, double val_vehic = 0, double val_other = 0, string cbfips = "unassigned", double beginDamage = 0, double groundElevation = -999, double foundationHeight = -999, int year = -999, int numStructures = 1)

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
            GroundElevation = groundElevation;
            FoundationHeight = foundationHeight;
            YearInService = year;
            NumberOfStructures = numStructures;
            BeginningDamageDepth = beginDamage;


        }
        public DeterministicStructure Sample(IProvideRandomNumbers randomProvider, OccupancyType occtype, bool computeIsDeterministic)
        {
            SampledStructureParameters sampledStructureParameters = occtype.Sample(randomProvider, InventoriedStructureValue, FirstFloorElevation, InventoriedContentValue, InventoriedOtherValue, InventoriedVehicleValue, computeIsDeterministic);
            //load up the deterministic structure
            return new DeterministicStructure(Fid, ImpactAreaID, sampledStructureParameters, BeginningDamageDepth, NumberOfStructures);
        }

        internal string ProduceDetails(double priceIndex)
        {
            string details = $"{Fid},{YearInService},{DamageCatagory},{OccTypeName},{Point.X},{Point.Y},{InventoriedStructureValue},{InventoriedStructureValue*priceIndex},{InventoriedContentValue},{InventoriedContentValue * priceIndex},{InventoriedOtherValue},{InventoriedOtherValue * priceIndex},{InventoriedVehicleValue},{InventoriedVehicleValue * priceIndex},{InventoriedStructureValue+InventoriedContentValue+InventoriedOtherValue+InventoriedStructureValue},{(InventoriedStructureValue + InventoriedContentValue + InventoriedOtherValue + InventoriedStructureValue) * priceIndex},{NumberOfStructures},{FirstFloorElevation},{GroundElevation},{FoundationHeight},{BeginningDamageDepth},";
            return details;
        }
    }
}