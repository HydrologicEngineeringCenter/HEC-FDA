using Xunit;
using System.Collections.Generic;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.Model.structures;
using HEC.FDA.Model.hydraulics;

namespace HEC.FDA.ModelTest.unittests.hydraulics
{
    [Trait("RunsOn", "Remote")]
    [Collection("Serial")]
    public class HydraulicProfileShould
    {
        private const string pathToNSIShapefile = @"..\..\..\fda-model-test\Resources\MuncieNSI\Muncie-SI_CRS2965.shp";

        private const string pathToIAShapefile = @"..\..\..\fda-model-test\Resources\MuncieImpactAreas\ImpactAreas.shp";

        private const string ParentDirectoryToUnsteadyResult = @"..\..\..\fda-model-test\Resources\MuncieResult";
        private const string UnsteadyHDFFileName = @"Muncie.p04.hdf";

        private const string ParentDirectoryToGrid = @"..\..\..\fda-model-test\Resources\MuncieGrid";
        private const string GridFileName = @"WSE (Max).Terrain.muncie_clip.tif";

        private const string ParentDirectoryToSteadyResult = @"..\..\..\fda-model-test\Resources\MuncieSteadyResult";
        private const string SteadyHDFFileName = @"Muncie.p10.hdf";

        private const string IANameColumnHeader = "Name";
        private const string SteadyHydraulicProfileName = "500";

        private const string TerrainPath = @"..\..\..\fda-model-test\Resources\MuncieTerrain\Terrain (1)_30ft_clip.hdf";


        [Theory]
        [InlineData(ParentDirectoryToUnsteadyResult, UnsteadyHDFFileName, HydraulicDataSource.UnsteadyHDF, "Max",true,TerrainPath )]
        [InlineData(ParentDirectoryToSteadyResult, SteadyHDFFileName, HydraulicDataSource.SteadyHDF, SteadyHydraulicProfileName, false, TerrainPath)]
        [InlineData(ParentDirectoryToGrid, GridFileName, HydraulicDataSource.WSEGrid, "Max", false, TerrainPath)]

        public void GetWSE(string parentDirectory, string fileName, HydraulicDataSource dataSource, string profileName, bool useTerrainFile, string pathTerrain)
        {
            StructureSelectionMapping map = new StructureSelectionMapping(false, false, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            HydraulicProfile profile = new HydraulicProfile(.01, fileName, profileName);
            OccupancyType occupancyType = OccupancyType.builder().build();
            Dictionary<string, OccupancyType> occupancyTypes = new Dictionary<string, OccupancyType>() { { "occtype", occupancyType } };


            Inventory inventory = new Inventory(pathToNSIShapefile, pathToIAShapefile, map, occupancyTypes, useTerrainFile, pathTerrain);
            float[] wses = profile.GetWSE(inventory.GetPointMs(), dataSource, parentDirectory);
            Assert.Equal(682, wses.Length); // All structures have a value
            Assert.True( wses[0] > 900); // first structure has value for WSE
        }
    }
}
