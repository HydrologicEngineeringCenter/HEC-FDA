using Statistics.Distributions;
using Statistics;
using RasMapperLib;
using Xunit;
using HEC.FDA.Model.structures;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.compute;
using System.Collections.Generic;
using HEC.FDA.Model.metrics;

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
        [InlineData(102, 200, 180)]
        [InlineData(104, 400, 360)]
        public void ComputeStructureDamage(float wse, double expectedStructureDamage, double expectedContentDamage)
        {
            List<DeterministicOccupancyType> deterministicOccupancyTypes = new List<DeterministicOccupancyType>();
            deterministicOccupancyTypes.Add(occupancyType.Sample(medianRandomProvider, true));
            ConsequenceResult consequenceResult = structure.ComputeDamage(wse, deterministicOccupancyTypes);
            Assert.Equal(expectedStructureDamage, consequenceResult.StructureDamage,0);
            Assert.Equal(expectedContentDamage, consequenceResult.ContentDamage,0);
        }

        //TODO: Replace tests in this class 
        //TODO: Replace deterministic tests into here
        //TODO: Rewrite the below test to make sure that we retrieve the correct occupancy type

        //The test that needs to be re-written is the following 
        //and will basically test line 75 of Structure.cs
        //[Fact]
        //public void occtypesDictionaryCorrectlyMapsOcctypeNameToOcctype()
        //{
        //    OccupancyType ot = StageDamageShould.residentialOccupancyTypeNormalDists;
        //    //the occtype created above has an occtype name of Residential_One_Story_No_Basement_Normal
        //    StructureSelectionMapping map = new StructureSelectionMapping(false, false, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        //    //link the occtype name of "NA" to the occtype
        //    Dictionary<string, OccupancyType> occupancyTypes = new Dictionary<string, OccupancyType>() { { "NA", ot } };
        //    //this inventory has hundreds of structures that have an occtype name of "NA"
        //    Inventory inv = new Inventory(pathToNSIShapefile, pathToIAShapefile, map, occupancyTypes, false, pathToTerrainHDF);
        //    //if the struction in the inventory has an occtype name that isn't in the above dictionary then it will get removed 
        //    //from the inventory during the sample.


        //    int inventoryCount = inv.Structures.Count;
        //    DeterministicInventory deterministicInventory = inv.Sample(new MedianRandomProvider(), false);
        //    int afterSampleCount = deterministicInventory.Inventory.Count;

        //    Assert.Equal(inventoryCount, afterSampleCount);
        //}
    }
}
