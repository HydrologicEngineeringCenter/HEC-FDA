using Statistics.Distributions;
using Statistics;
using RasMapperLib;
using Xunit;
using HEC.FDA.Model.structures;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.compute;
using HEC.MVVMFramework.Base.Implementations;

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
        private static MedianRandomProvider medianRandomProvider = new MedianRandomProvider();
        private static string occupancyTypeName = "MyOccupancyType";
        private static string damageCategory = "DamageCategory";
        private static OccupancyType occupancyType = OccupancyType.builder()
            .withName(occupancyTypeName)
            .withDamageCategory(damageCategory)
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();
        private static int structureID = 44;
        private static PointM pointM = new PointM();
        private static double firstFloorElevation = 100;
        private static double inventoriedStructureValue = 1000;
        private static int impactAreaID = 55;
        private static float GroundElev = 0;
        private static Structure structure = new Structure(structureID, pointM, firstFloorElevation, inventoriedStructureValue, damageCategory, occupancyTypeName, impactAreaID, groundElevation: GroundElev);

        [Theory]
        [InlineData(1000, 100, 900, new double[] { 0, 10, 20, 30, 40, 50 })]
        public void StructureShouldSampleCorrectly(double expectedStructureValue, double expectedFirstFloorElevation, double expectedContentValue, double[] expectedPercentDamage)
        {
            DeterministicStructure deterministicStructure = structure.Sample(medianRandomProvider, occupancyType, computeIsDeterministic: true);
            Assert.Equal(expectedStructureValue, deterministicStructure.StructValueSample);
            Assert.Equal(expectedFirstFloorElevation, deterministicStructure.FirstFloorElevation);
            Assert.Equal(expectedContentValue, deterministicStructure.ContentValueSample);
            Assert.Equal(expectedPercentDamage, deterministicStructure.SampledStructureParameters.StructPercentDamagePairedData.Yvals);
        }

        [Fact]
        public void ValidationShould()
        {
            Structure badStructure = new Structure(fid: 1, pointM, firstFloorElevation: -304, val_struct: -10, st_damcat: "", occtype: "", impactAreaID, val_cont: -10, val_other: -10, val_vehic: -10);
            badStructure.Validate();
            foreach (PropertyRule rule in badStructure.RuleMap.Values)
            {
                Assert.Single(rule.Errors);
                Assert.Equal(HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal, rule.ErrorLevel);
            }

        }
    }
}
