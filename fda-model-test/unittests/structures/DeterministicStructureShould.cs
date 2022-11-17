using Xunit;
using HEC.FDA.Model.structures;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.metrics;
using System.ComponentModel;

namespace HEC.FDA.ModelTest.unittests.structures
{
    [Trait("Category", "Unit")]
    public class DeterministicStructureShould
    {
        private static string occupancyTypeName = "Res1-1NB";
        private static string occupancyTypeDamageCategory = "Residential";
        private static double[] depths = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        private static double[] percentDamage = new double[] { 0, .10, .20, .30, .40, .50, .60, .70, .80 };
        private static PairedData structureDepthPercentDamage = new PairedData(depths, percentDamage);
        private static PairedData contentDepthPercentDamage = new PairedData(depths, percentDamage);
        private static double sampledFirstFloorElevation = 100;
        private static double sampledStructureValue = 1000;
        private static double sampledContentValue = 500;
        private static bool computeContentDamage = true;
        private static bool computeVehicleDamage = false;
        private static bool computeOtherDamage = false;
        private static SampledStructureParameters sampledStructureParameters = new SampledStructureParameters(occupancyTypeName, occupancyTypeDamageCategory, structureDepthPercentDamage, sampledFirstFloorElevation, sampledStructureValue, computeContentDamage, computeVehicleDamage, computeOtherDamage, contentDepthPercentDamage, sampledContentValue);
        private static int structureID = 44;
        private static int impactAreaID = 55;
        private static double beginningDamageDepth = -999;
        private static DeterministicStructure deterministicStructure = new DeterministicStructure(structureID, impactAreaID, sampledStructureParameters, beginningDamageDepth);


        [Theory]
        [InlineData(100, 0, 0)]
        [InlineData(104, 400, 200)]
        [InlineData(108, 800, 400)]
        public void DeterministicStructureShouldComputeDamageCorrectly(float waterSurfaceElevation, double expectedStructureDamage, double expectedContentDamage)
        {
            ConsequenceResult consequenceResult = deterministicStructure.ComputeDamage(waterSurfaceElevation);
            Assert.Equal(expectedStructureDamage, consequenceResult.StructureDamage, 0);
            Assert.Equal(expectedContentDamage, consequenceResult.ContentDamage, 0);
        }







    }
}
