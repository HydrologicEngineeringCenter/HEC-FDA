using HEC.FDA.Model.interfaces;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using RasMapperLib;

namespace HEC.FDA.Model.structures
{
    public class Structure: Validation 
    {
        #region Properties 
        //TODO: How are we going to handle missing data?
        //For now, we won't allow missing data 
        public int Fid { get; }
        public PointM Point { get; set; }
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
        #endregion

        #region Constructors 
        public Structure(int fid, PointM point, double firstFloorElevation, double val_struct, string st_damcat, string occtype, int impactAreaID, double val_cont =0, double val_vehic = 0, double val_other = 0, string cbfips = "unassigned", double beginDamage = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, double groundElevation = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, double foundationHeight = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, int year = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, int numStructures = 1)

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
            AddRules();

        }
        #endregion
        #region Methods 
        private void AddRules()
        {
            AddSinglePropertyRule(nameof(FirstFloorElevation), new Rule(() => FirstFloorElevation > -300, $"First floor elevation must be greater than -300, but is {FirstFloorElevation} for Structure {Fid}", ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(InventoriedStructureValue), new Rule(() => InventoriedStructureValue >= 0, $"The inventoried structure value must be greater than or equal to 0, but is {InventoriedStructureValue} for Structure {Fid}", ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(InventoriedContentValue), new Rule(() => InventoriedContentValue >= 0, $"The inventoried content value must be greater than or equal to 0, but is {InventoriedContentValue} for Structure {Fid}", ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(InventoriedOtherValue), new Rule(() => InventoriedOtherValue >= 0, $"The inventoried other value must be greater than or equal to 0, but is {InventoriedOtherValue} for Structure {Fid}", ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(InventoriedVehicleValue), new Rule(() => InventoriedVehicleValue >= 0, $"The inventoried vehicle value must be greater than or equal to 0, but is {InventoriedVehicleValue} for Structure {Fid}", ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(DamageCatagory), new Rule(() => DamageCatagory != null && DamageCatagory != "", $"Damage category should not be null but appears null for Structure {Fid}", ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(OccTypeName), new Rule(() => OccTypeName != null && OccTypeName != "", $"The occupancy type should not be null but appears null for Structure {Fid}", ErrorLevel.Fatal));
        }
        public DeterministicStructure Sample(IProvideRandomNumbers randomProvider, OccupancyType occtype, bool computeIsDeterministic)
        {
            SampledStructureParameters sampledStructureParameters = occtype.Sample(randomProvider, InventoriedStructureValue, FirstFloorElevation, InventoriedContentValue, InventoriedOtherValue, InventoriedVehicleValue, computeIsDeterministic);
            //load up the deterministic structure
            return new DeterministicStructure(Fid, ImpactAreaID, sampledStructureParameters, BeginningDamageDepth, NumberOfStructures, YearInService);
        }

        internal string ProduceDetails(double priceIndex)
        {
            string details = $"{Fid},{YearInService},{DamageCatagory},{OccTypeName},{Point.X},{Point.Y},{InventoriedStructureValue},{InventoriedStructureValue*priceIndex},{InventoriedContentValue},{InventoriedContentValue * priceIndex},{InventoriedOtherValue},{InventoriedOtherValue * priceIndex},{InventoriedVehicleValue},{InventoriedVehicleValue * priceIndex},{InventoriedStructureValue+InventoriedContentValue+InventoriedOtherValue+InventoriedStructureValue},{(InventoriedStructureValue + InventoriedContentValue + InventoriedOtherValue + InventoriedStructureValue) * priceIndex},{NumberOfStructures},{FirstFloorElevation},{GroundElevation},{FoundationHeight},{BeginningDamageDepth},";
            return details;
        }
        #endregion 
    }
}