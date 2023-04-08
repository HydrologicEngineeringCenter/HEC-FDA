using HEC.FDA.Model.interfaces;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using RasMapperLib;
using HEC.FDA.Model.metrics;
using System.Collections.Generic;
using System;

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
            AddSinglePropertyRule(nameof(FirstFloorElevation), new Rule(() => FirstFloorElevation > -300, $"First floor elevation must be greater than -300, but is {FirstFloorElevation} for Structure {Fid}", ErrorLevel.Major));
            AddSinglePropertyRule(nameof(InventoriedStructureValue), new Rule(() => InventoriedStructureValue >= 0, $"The inventoried structure value must be greater than or equal to 0, but is {InventoriedStructureValue} for Structure {Fid}", ErrorLevel.Major));
            AddSinglePropertyRule(nameof(InventoriedContentValue), new Rule(() => InventoriedContentValue >= 0, $"The inventoried content value must be greater than or equal to 0, but is {InventoriedContentValue} for Structure {Fid}", ErrorLevel.Major));
            AddSinglePropertyRule(nameof(InventoriedOtherValue), new Rule(() => InventoriedOtherValue >= 0, $"The inventoried other value must be greater than or equal to 0, but is {InventoriedOtherValue} for Structure {Fid}", ErrorLevel.Major));
            AddSinglePropertyRule(nameof(InventoriedVehicleValue), new Rule(() => InventoriedVehicleValue >= 0, $"The inventoried vehicle value must be greater than or equal to 0, but is {InventoriedVehicleValue} for Structure {Fid}", ErrorLevel.Major));
            AddSinglePropertyRule(nameof(DamageCatagory), new Rule(() => DamageCatagory != null, $"Damage category should not be null but appears null for Structure {Fid}", ErrorLevel.Major));
            AddSinglePropertyRule(nameof(OccTypeName), new Rule(() => OccTypeName != null, $"The occupancy type should not be null but appears null for Structure {Fid}", ErrorLevel.Major));
        }

        public ConsequenceResult ComputeDamage(float waterSurfaceElevation, List<DeterministicOccupancyType> deterministicOccupancyTypeList, double priceIndex = 1, int analysisYear = 9999)
        {
            //Create a default deterministic occupancy type 
            DeterministicOccupancyType deterministicOccupancyType = new DeterministicOccupancyType();

            //see if we can match an occupancy type from the provided list to the structure occ type name 
            foreach (DeterministicOccupancyType deterministicOccupancy in deterministicOccupancyTypeList)
            {
                if (deterministicOccupancy.OccupancyTypeName == OccTypeName)
                {
                    deterministicOccupancyType = deterministicOccupancy;
                }
            }
            ConsequenceResult consequenceResult = new ConsequenceResult(DamageCatagory);

            //TODO: We need a way to make sure that the sampled first floor elevation is reasonable 
            //that is hard when we throw away the foundation height 
            double sampledFFE = 0;
            if (deterministicOccupancyType.IsFirstFloorElevationLogNormal)
            {
                sampledFFE = FirstFloorElevation * (deterministicOccupancyType.FirstFloorElevationOffset);
            } else
            {
                sampledFFE = FirstFloorElevation + deterministicOccupancyType.FirstFloorElevationOffset;
            }
            double depthabovefoundHeight = waterSurfaceElevation - sampledFFE;
            double sampledStructureValue;
            double structDamage = 0;
            double contDamage = 0;
            double vehicleDamage = 0;
            double otherDamage = 0;
            if (analysisYear > YearInService)
            {
                //Beginning damage depth is relative to the first floor elevation and so a beginning damage depth of -1 means that damage begins 1 foot below the first floor elevation

                if (BeginningDamageDepth <= depthabovefoundHeight)
                {
                    //Structure
                    double structDamagepercent = deterministicOccupancyType.StructPercentDamagePairedData.f(depthabovefoundHeight);
                    if (structDamagepercent > 100)
                    {
                        structDamagepercent = 100;
                    }
                    if (structDamagepercent < 0)
                    {
                        structDamagepercent = 0;
                    }
                    if (deterministicOccupancyType.IsStructureValueLogNormal)
                    {
                        sampledStructureValue = Math.Pow((deterministicOccupancyType.StructureValueOffset), Math.Log(InventoriedStructureValue))*(InventoriedStructureValue);
                        structDamage = (structDamagepercent / 100) * priceIndex * NumberOfStructures * sampledStructureValue;
                    } else
                    {
                        sampledStructureValue = InventoriedStructureValue * (deterministicOccupancyType.StructureValueOffset);
                        structDamage = (structDamagepercent / 100) * priceIndex * NumberOfStructures * sampledStructureValue;
                    }

                    //Content
                    if (deterministicOccupancyType.ComputeContentDamage)
                    {
                        double contentDamagePercent = deterministicOccupancyType.ContentPercentDamagePairedData.f(depthabovefoundHeight);
                        if (contentDamagePercent > 100)
                        {
                            contentDamagePercent = 100;
                        }
                        if (contentDamagePercent < 0)
                        {
                            contentDamagePercent = 0;
                        }
                        if (deterministicOccupancyType.UseCSVR)
                        {
                            contDamage = (contentDamagePercent / 100) * priceIndex * NumberOfStructures * (deterministicOccupancyType.ContentToStructureValueRatio / 100) * sampledStructureValue;
                        }
                        else
                        {
                            if (deterministicOccupancyType.IsContentValueLogNormal)
                            {
                                double sampledContentValue = Math.Pow(deterministicOccupancyType.ContentValueOffset, Math.Log(InventoriedContentValue)) * (InventoriedContentValue);
                                contDamage = (contentDamagePercent/100) * priceIndex * NumberOfStructures * (sampledContentValue);
                            } else
                            {
                                double sampledContentValue = (InventoriedContentValue * (deterministicOccupancyType.ContentValueOffset));
                                contDamage = (contentDamagePercent / 100) * priceIndex * NumberOfStructures * sampledContentValue;

                            }
                        }
                    }

                    //Vehicle
                    if (deterministicOccupancyType.ComputeVehicleDamage)
                    {
                        double vehicleDamagePercent = deterministicOccupancyType.VehiclePercentDamagePairedData.f(depthabovefoundHeight);
                        if (vehicleDamagePercent > 100)
                        {
                            vehicleDamagePercent = 100;
                        }
                        if (vehicleDamagePercent < 0)
                        {
                            vehicleDamagePercent = 0;
                        }
                        if (deterministicOccupancyType.IsVehicleValueLogNormal)
                        {
                            double sampledVehicleValue = Math.Pow(deterministicOccupancyType.VehicleValueOffset, Math.Log(InventoriedVehicleValue))*(InventoriedVehicleValue);
                            vehicleDamage = (vehicleDamagePercent / 100) * priceIndex * NumberOfStructures * sampledVehicleValue;
                        } else
                        {
                            double sampledVehicleValue = InventoriedVehicleValue * (deterministicOccupancyType.VehicleValueOffset);
                            vehicleDamage = (vehicleDamagePercent / 100) * priceIndex * NumberOfStructures* sampledVehicleValue;
                        }
                    }

                    //Other
                    if (deterministicOccupancyType.ComputeOtherDamage)
                    {
                        double otherDamagePercent = deterministicOccupancyType.OtherPercentDamagePairedData.f(depthabovefoundHeight);
                        if (otherDamagePercent > 100)
                        {
                            otherDamagePercent = 100;
                        }
                        if (otherDamagePercent < 0)
                        {
                            otherDamagePercent = 0;
                        }
                        if (deterministicOccupancyType.UseOSVR)
                        {
                            otherDamage = (otherDamagePercent / 100) * priceIndex * NumberOfStructures * (sampledStructureValue) * (deterministicOccupancyType.OtherToStructureValueRatio / 100);
                        } else
                        {
                            if (deterministicOccupancyType.IsOtherValueLogNormal)
                            {
                                double sampledOtherValue = Math.Pow(deterministicOccupancyType.OtherValueOffset, Math.Log(InventoriedOtherValue)) * (InventoriedOtherValue);
                                otherDamage = (otherDamagePercent / 100) * priceIndex * NumberOfStructures * sampledOtherValue;
                            } else
                            {
                                double sampledOtherValue = (InventoriedOtherValue) * (deterministicOccupancyType.OtherValueOffset);
                                otherDamage = (otherDamagePercent / 100) * priceIndex * NumberOfStructures*sampledOtherValue;
                            }
                        }
                    }
                }
            }
            consequenceResult.IncrementConsequence(structDamage, contDamage, vehicleDamage, otherDamage);

            return consequenceResult;
        }
        internal string ProduceDetails(double priceIndex)
        {
            string details = $"{Fid},{YearInService},{DamageCatagory},{OccTypeName},{Point.X},{Point.Y},{InventoriedStructureValue},{InventoriedStructureValue*priceIndex},{InventoriedContentValue},{InventoriedContentValue * priceIndex},{InventoriedOtherValue},{InventoriedOtherValue * priceIndex},{InventoriedVehicleValue},{InventoriedVehicleValue * priceIndex},{InventoriedStructureValue+InventoriedContentValue+InventoriedOtherValue+InventoriedStructureValue},{(InventoriedStructureValue + InventoriedContentValue + InventoriedOtherValue + InventoriedStructureValue) * priceIndex},{NumberOfStructures},{FirstFloorElevation},{GroundElevation},{FoundationHeight},{BeginningDamageDepth},";
            return details;
        }
        #endregion 
    }
}