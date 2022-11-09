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

        private const string ParentDirectoryToUnsteadyResult = @"..\..\..\fda-model-test\Resources\MuncieResult";
        private const string UnsteadyHDFFileName = @"Muncie.p04.hdf";

        private const string ParentDirectoryToGrid = @"..\..\..\fda-model-test\Resources\MuncieGrid";
        private const string GridFileName = @"WSE (Max).Terrain.muncie_clip.tif";

        private const string ParentDirectoryToSteadyResult = @"..\..\..\fda-model-test\Resources\MuncieSteadyResult";
        private const string SteadyHDFFileName = @"Muncie.p09.hdf";

        private const string IANameColumnHeader = "Name";
        private const string SteadyHydraulicProfileName = "PF 8";

        private const string TerrainPath = @"..\..\..\fda-model-test\Resources\MuncieTerrain\Terrain (1)_30ft_clip.hdf";


        [Theory]
        [InlineData(ParentDirectoryToUnsteadyResult, UnsteadyHDFFileName, HydraulicDataSource.UnsteadyHDF, "Max",true,TerrainPath )]
        [InlineData(ParentDirectoryToSteadyResult, SteadyHDFFileName, HydraulicDataSource.SteadyHDF, SteadyHydraulicProfileName, false, TerrainPath)]
        [InlineData(ParentDirectoryToGrid, GridFileName, HydraulicDataSource.WSEGrid, "Max", false, TerrainPath)]

        public void GetWSE(string parentDirectory, string fileName, HydraulicDataSource dataSource, string profileName, bool useTerrainFile, string pathTerrain)
        {
            StructureInventoryColumnMap map = new StructureInventoryColumnMap(null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            HydraulicProfile profile = new HydraulicProfile(.01, fileName, profileName);
            OccupancyType occupancyType = new OccupancyType();
            List<OccupancyType> occupancyTypes = new List<OccupancyType>() { occupancyType };


            Inventory inventory = new Inventory(pathToNSIShapefile, pathToIAShapefile, map, occupancyTypes, IANameColumnHeader, useTerrainFile, pathTerrain);
            float[] wses = profile.GetWSE(inventory.GetPointMs(), dataSource, parentDirectory);
            Assert.Equal(696, wses.Length); // All structures have a value
            Assert.True( wses[0] > 900); // first structure has value for WSE
        }
    }
}
