using Xunit;
using System.Collections.Generic;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.Model.structures;
using HEC.FDA.Model.hydraulics;

namespace HEC.FDA.ModelTest.unittests.hydraulics
{
    [Trait("Category", "Unit")]
    public class HydraulicProfileShould
    {
        private const string pathToNSIShapefile = @"..\..\..\fda-model-test\Resources\MuncieNSI\MuncieNSI.shp";
        private const string pathToIAShapefile = @"..\..\..\fda-model-test\Resources\MuncieImpactAreas\ImpactAreas.shp";
        private const string pathToResult = @"..\..\..\fda-model-test\Resources\MuncieResult\Muncie.p04.hdf";
        private const string pathToGrid = @"..\..\..\fda-model-test\Resources\MuncieGrid\WSE (Max).Terrain.muncie_clip.tif";

        [Theory]
        [InlineData(pathToResult, HydraulicDataSource.UnsteadyHDF)]
        [InlineData(pathToResult, HydraulicDataSource.SteadyHDF)]
        [InlineData(pathToGrid, HydraulicDataSource.WSEGrid)]

        public void GetWSE(string path, HydraulicDataSource dataSource)
        {
            StructureInventoryColumnMap map = new StructureInventoryColumnMap();
            HydraulicProfile profile = new HydraulicProfile(.01, path, dataSource, "Max");
            //Empty (default) occupancy types
            OccupancyType occupancyType = new OccupancyType();
            List<OccupancyType> occupancyTypes = new List<OccupancyType>() { occupancyType };
            Inventory inventory = new Inventory(pathToNSIShapefile, pathToIAShapefile, map, occupancyTypes);
            float[] wses = profile.GetWSE(inventory.GetPointMs());
            Assert.Equal(696, wses.Length); // All structures have a value
            Assert.Equal(947.244446, wses[0], 1); // first structure has correct WSE
        }
    }
}
