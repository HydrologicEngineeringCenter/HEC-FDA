using Xunit;
using System.Collections.Generic;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.Model.structures;
using HEC.FDA.Model.hydraulics;

namespace fda_model_test.unittests
{
    [Trait("Category", "Unit")]
    public class HydraulicProfileShould
    {
        private const string pathToNSIShapefile = @"..\..\..\fda-model-test\Resources\MuncieNSI\MuncieNSI.shp";
        private const string pathToIAShapefile = @"..\..\..\fda-model-test\Resources\MuncieImpactAreas\ImpactAreas.shp";
        private const string pathToResult = @"..\..\..\fda-model-test\Resources\MuncieResult\Muncie.p04.hdf";
        private const string pathToTerrain = @"..\..\..\fda-model-test\Resources\MuncieTerrain\Terrain (1)_30ft_clip.hdf";

        [Fact]
        public void GetWSEFromHDF()
        {
            StructureInventoryColumnMap map = new StructureInventoryColumnMap();
            HydraulicProfile profile = new HydraulicProfile(.01, pathToResult, HydraulicDataSource.UnsteadyHDF, "Max", pathToTerrain);
            //Empty (default) occupancy types
            OccupancyType occupancyType = new OccupancyType();
            List<OccupancyType> occupancyTypes = new List<OccupancyType>() { occupancyType };
            Inventory inventory = new Inventory(pathToNSIShapefile, pathToIAShapefile, map, occupancyTypes);
            float[] wses = profile.GetWSE(inventory.GetPointMs());
            Assert.Equal(696, wses.Length); // All structures have a value
            Assert.Equal(947.244446, wses[0],1); // first structure has correct WSE
            Assert.Equal(-9999, wses[1]); // second structure is dry and reports -9999
        }
    }
}
