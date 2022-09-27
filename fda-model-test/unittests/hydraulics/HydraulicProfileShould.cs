using Xunit;
using fda_model.hydraulics;
using fda_model.hydraulics.enums;
using structures;
using fda_model.structures;
using System.Linq;
using System.Collections.Generic;

namespace fda_model_test.unittests
{
    public class HydraulicProfileShould
    {
        private const string pathToNSIShapefile = @"..\..\..\Resources\MuncieNSI\MuncieNSI.shp";
        private const string pathToIAShapefile = @"..\..\..\Resources\MuncieImpactAreas\ImpactAreas.shp";
        private const string pathToResult = @"..\..\..\Resources\MuncieResult\Muncie.p04.hdf";
        private const string pathToTerrain = @"..\..\..\Resources\MuncieTerrain\Terrain (1)_30ft_clip.hdf";

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
            Assert.Equal(947.244446, wses[0]); // first structure has correct WSE
            Assert.Equal(-9999, wses[1]); // second structure is dry and reports -9999
        }
    }
}
