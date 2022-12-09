using Xunit;
using HEC.FDA.Model.structures;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.metrics;

namespace HEC.FDA.ModelTest.unittests.structures
{
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
        private static int yearInService = 1820;

        [Theory]
        [InlineData(100, 0, 0, -999, 1, 1900, 1)]//begDamDept is not in the inventory
        [InlineData(104, 400, 200, 4, 1, 1900, 1)]//begDamDept = depthabovefoundationHeight -- positive damages
        [InlineData(108, 0, 0, 8, 1, 1800, 1)]//analysis year is before year in service, damage should be zero 
        [InlineData(104, 400, 200, 4, 2, 1900, 1)]//begDamDept = depthabovefoundationHeight -- positive damages
        [InlineData(108, 800, 400, 8, 2, 1900, 1)]//begDamDept = depthabovefoundationHeight -- positive damages
        [InlineData(104, 400, 200, 4, 1, 1900, 2)]//begDamDept = depthabovefoundationHeight -- positive damages
        [InlineData(108, 800, 400, 8, 1, 1900, 3)]//begDamDept = depthabovefoundationHeight -- positive damages
        [InlineData(104, 0, 0, 4.1, 1, 1900, 1)]//begDamDept > depthabovefoundationHeight -- zero damages
        [InlineData(108, 0, 0, 8.1, 1, 1900, 2)]//begDamDept > depthabovefoundationHeight -- zero damages
        [InlineData(104, 400, 200, 3.9, 2, 1900, 1)]//begDamDept < depthabovefoundationHeight -- positive damages
        [InlineData(108, 800, 400, 7.9, 1, 1900, 3)]//begDamDept < depthabovefoundationHeight -- positive damages
        public void ComputeDamageCorrectlyBegDamDepthEqualToDepthAboveFFE(float waterSurfaceElevation, double expectedStructureDamage, double expectedContentDamage, double beginningDamageDepth, double priceIndex, int analysisYear, int numberOfStructures)
        {
            DeterministicStructure deterministicStructure = new DeterministicStructure(structureID, impactAreaID, sampledStructureParameters, beginningDamageDepth, numberOfStructures, yearInService);

            ConsequenceResult consequenceResult = deterministicStructure.ComputeDamage(waterSurfaceElevation, priceIndex, analysisYear);
            Assert.Equal(expectedStructureDamage * numberOfStructures * priceIndex, consequenceResult.StructureDamage, 0);
            Assert.Equal(expectedContentDamage * numberOfStructures * priceIndex, consequenceResult.ContentDamage, 0);
        }
    }
}
