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

        private const string ParentDirectoryToResult = @"..\..\..\fda-model-test\Resources\MuncieResult";
        private const string RESULT_NAME = @"Muncie.p04.hdf";

        private const string ParentDirectoryToGrid = @"..\..\..\fda-model-test\Resources\MuncieGrid\WSE (Max).Terrain";
        private const string GRID_NAME = @"muncie_clip.tif";


        [Theory]
        [InlineData(ParentDirectoryToResult, RESULT_NAME, HydraulicDataSource.UnsteadyHDF)]
        [InlineData(ParentDirectoryToResult, RESULT_NAME, HydraulicDataSource.SteadyHDF)]
        //[InlineData(pathToGrid, HydraulicDataSource.WSEGrid)]

        public void GetWSE(string parentDirectory, string profileName, HydraulicDataSource dataSource)
        {
            StructureInventoryColumnMap map = new StructureInventoryColumnMap();
            HydraulicProfile profile = new HydraulicProfile(.01, profileName);
            //Empty (default) occupancy types
            OccupancyType occupancyType = new OccupancyType();
            List<OccupancyType> occupancyTypes = new List<OccupancyType>() { occupancyType };


            Inventory inventory = new Inventory(pathToNSIShapefile, pathToIAShapefile, map, occupancyTypes);
            float[] wses = profile.GetWSE(inventory.GetPointMs(), dataSource, parentDirectory);
            Assert.Equal(696, wses.Length); // All structures have a value
            Assert.Equal(947.244446, wses[0], 1); // first structure has correct WSE
        }
    }
}
