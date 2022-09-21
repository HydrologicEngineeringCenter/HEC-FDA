using Xunit;
using fda_model.hydraulics;
using fda_model.hydraulics.enums;
using structures;
using fda_model.structures;

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
            Inventory inventory = new Inventory(pathToNSIShapefile, pathToIAShapefile, map);
            float[] wses = profile.GetWSE(inventory.GetPointMs());
            Assert.Equal(696, wses.Length);
        }
    }
}
