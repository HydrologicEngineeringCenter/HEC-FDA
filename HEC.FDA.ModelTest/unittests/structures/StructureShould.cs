using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.structures;
using HEC.MVVMFramework.Base.Implementations;
using RasMapperLib;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using Xunit;

namespace HEC.FDA.ModelTest.unittests.structures
{
    [Trait("RunsOn", "Remote")]
    public class StructureShould
    {
        private static double[] depths = new double[] { 0, 1, 2, 3, 4, 5 };
        private static IDistribution[] percentDamages = new IDistribution[]
        {
            new Normal(0,0),
            new Normal(10,5),
            new Normal(20,6),
            new Normal(30,7),
            new Normal(40,8),
            new Normal(50,9)
        };
        private static CurveMetaData metaData = new CurveMetaData("Depths", "Percent Damage", "Depth-Percent Damage Function");
        private static UncertainPairedData _StructureDepthPercentDamageFunction = new UncertainPairedData(depths, percentDamages, metaData);
        private static UncertainPairedData _ContentDepthPercentDamageFunction = new UncertainPairedData(depths, percentDamages, metaData);
        private static FirstFloorElevationUncertainty firstFloorElevationUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Normal, 0.5);
        private static ValueUncertainty _structureValueUncertainty = new ValueUncertainty(IDistributionEnum.Normal, .1);
        private static ValueRatioWithUncertainty _contentToStructureValueRatio = new ValueRatioWithUncertainty(IDistributionEnum.Normal, 10, 90);
        private static string occupancyTypeName = "MyOccupancyType";
        private static string damageCategory = "DamageCategory";
        private static OccupancyType occupancyType = OccupancyType.Builder()
            .WithName(occupancyTypeName)
            .WithDamageCategory(damageCategory)
            .WithStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .WithContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .WithFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .WithStructureValueUncertainty(_structureValueUncertainty)
            .WithContentToStructureValueRatio(_contentToStructureValueRatio)
            .Build();
        private static string structureID = "44";
        private static PointM pointM = new PointM();
        private static double firstFloorElevation = 100;
        private static double inventoriedStructureValue = 1000;
        private static int impactAreaID = 55;
        private static float GroundElev = 0;
        private static Structure structure = new Structure(structureID, pointM, firstFloorElevation, inventoriedStructureValue, damageCategory, occupancyTypeName, impactAreaID, groundElevation: GroundElev);

        [Theory]
        [InlineData(102, 200, 180)]
        [InlineData(104, 400, 360)]
        public void ComputeStructureDamage(float wse, double expectedStructureDamage, double expectedContentDamage)
        {
            List<DeterministicOccupancyType> deterministicOccupancyTypes = new List<DeterministicOccupancyType>();
            deterministicOccupancyTypes.Add(occupancyType.Sample(iteration: 1, true));
            ConsequenceResult consequenceResult = structure.ComputeDamage(wse, deterministicOccupancyTypes);
            Assert.Equal(expectedStructureDamage, consequenceResult.StructureDamage, 0);
            Assert.Equal(expectedContentDamage, consequenceResult.ContentDamage, 0);
        }
        [Fact]
        public void ValidationShould()
        {
            Structure badStructure = new Structure(fid: "1", pointM, firstFloorElevation: -304, val_struct: -10, st_damcat: "", occtype: "", impactAreaID, val_cont: -10, val_other: -10, val_vehic: -10);
            badStructure.Validate();
            foreach (PropertyRule rule in badStructure.RuleMap.Values)
            {
                Assert.Single(rule.Errors);
                Assert.Equal(HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal, rule.ErrorLevel);
            }

        }

        [Fact]
        public void SELA_StructureDamage_Should()
        {
            //1STY-PIER OccType
            double[] structureDepths = new double[] { -1.1, -1, -0.5, 0, 0.5, 1, 1.5, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            double[] structurePercentDamageMin = new double[] { 0, 1.5, 1.5, 7.5, 18.8, 41.5, 41.6, 44.7, 44.8, 44.9, 46.3, 46.4, 46.5, 46.6, 68.3, 68.4, 77.6, 77.7, 77.8, 77.9, 78 };
            double[] structurePercentDamageMostLikely = new double[] { 0, 4, 5.4, 20.5, 40.5, 41.5, 45.1, 52.3, 53.1, 57.1, 66.7, 66.8, 66.9, 67, 74.3, 74.4, 84.4, 84.5, 84.6, 84.7, 84.8 };
            double[] structurePercentDamageMax = new double[] { 0, 9.5, 9.5, 33.5, 63.3, 64.8, 65, 69.9, 70, 71.2, 80.5, 80.6, 80.7, 80.8, 81.1, 99.5, 99.6, 99.7, 99.8, 99.9, 100 };
            UncertainPairedData structureDepthPercentDamage = CreateTriangularUncertainPairedData(structureDepths, structurePercentDamageMin, structurePercentDamageMostLikely, structurePercentDamageMax);

            double[] contentDepths = new double[] { 0, 0.5, 1, 1.5, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            double[] contentPercentDamageMin = new double[] { 0, 18.7, 30.1, 37.4, 45.6, 59.1, 70.5, 76.4, 77.7, 77.8, 80.4, 80.5, 80.6, 80.7, 80.8, 80.9, 81, 81.1 };
            double[] contentPercentDamageMostLikely = new double[] { 0, 28.1, 41.8, 49.3, 62.9, 82.1, 84.6, 91.2, 91.3, 91.4, 91.5, 91.6, 91.7, 91.8, 91.9, 92, 92.1, 92.2 };
            double[] contentPercentDamageMax = new double[] { 0, 28.1, 41.8, 49.3, 62.9, 82.1, 84.6, 91.2, 91.3, 91.4, 91.5, 91.6, 91.7, 91.8, 91.9, 92, 92.1, 92.2 };
            UncertainPairedData contentDepthPercentDamage = CreateTriangularUncertainPairedData(contentDepths, contentPercentDamageMin, contentPercentDamageMostLikely, contentPercentDamageMax);
            FirstFloorElevationUncertainty firstFloorElevationUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Normal, 0.59);
            ValueUncertainty structureValueUncertainty = new ValueUncertainty(IDistributionEnum.Triangular, 69, 116);
            ValueRatioWithUncertainty csvr = new ValueRatioWithUncertainty(IDistributionEnum.Normal, 25.53, 69);

            OccupancyType oneStryPier = OccupancyType.Builder()
               .WithName(occupancyTypeName)
               .WithDamageCategory(damageCategory)
               .WithStructureDepthPercentDamage(structureDepthPercentDamage)
               .WithContentDepthPercentDamage(contentDepthPercentDamage)
               .WithFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
               .WithStructureValueUncertainty(structureValueUncertainty)
               .WithContentToStructureValueRatio(csvr)
               .Build();

            //Structure 233375 232549
            Structure structure232549 = new Structure(fid: "232549", point: pointM, firstFloorElevation: -2.35625, val_struct: 74.946944, st_damcat: damageCategory, occtype: occupancyTypeName, impactAreaID: impactAreaID);
            Structure structure233375 = new Structure(fid: "233375", point: pointM, firstFloorElevation: -3.3375, val_struct: 88.204817, st_damcat: damageCategory, occtype: occupancyTypeName, impactAreaID: impactAreaID);

            //0.002 AEP Stages 
            float wse233375 = -4.17f;
            float wse232549 = -1.27f;

            (double, double, double, double) consequenceResult233375 = structure233375.ComputeDamage(wse233375, oneStryPier.Sample(iteration: 1, true));
            (double, double, double, double) consequenceResult232549 = structure232549.ComputeDamage(wse232549, oneStryPier.Sample(iteration: 1, true));

            //percent damage externally interpolated from depth-percent damage function
            double expectedStructureDamage232549 = 0.4213 * structure232549.InventoriedStructureValue;
            double expectedStructureDamage233375 = 0.04476 * structure233375.InventoriedStructureValue;

            Assert.Equal(expectedStructureDamage232549, consequenceResult232549.Item1, .01);
            Assert.Equal(expectedStructureDamage233375, consequenceResult233375.Item1, .01);
        }


        private UncertainPairedData CreateTriangularUncertainPairedData(double[] structureDepths, double[] structurePercentDamageMin, double[] structurePercentDamageMostLikely, double[] structurePercentDamageMax)
        {
            Triangular[] triangulars = new Triangular[structureDepths.Length];
            for (int i = 0; i < structureDepths.Length; i++)
            {
                triangulars[i] = new Triangular(structurePercentDamageMin[i], structurePercentDamageMostLikely[i], structurePercentDamageMax[i]);
            }
            CurveMetaData curveMetaData = new CurveMetaData(xlabel: "x", ylabel: "y", name: "name");
            return new UncertainPairedData(structureDepths, triangulars, curveMetaData);
        }
    }
}
