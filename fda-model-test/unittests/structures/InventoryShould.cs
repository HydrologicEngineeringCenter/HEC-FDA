using Xunit;
using System.Collections.Generic;
using HEC.FDA.Model.structures;
using RasMapperLib;

namespace HEC.FDA.ModelTest.unittests.structures
{
    [Trait("Category", "Unit")]
    public class InventoryShould
    {
        private const string IANameColumnHeader = "Name";
        private const string pathToNSIShapefile = @"..\..\..\fda-model-test\Resources\MuncieNSI\MuncieNSI.shp";
        private const string pathToIAShapefile = @"..\..\..\fda-model-test\Resources\MuncieImpactAreas\ImpactAreas.shp";
        private const string pathToTerrainHDF = @"..\..\..\fda-model-test\Resources\MuncieTerrain\Terrain (1)_30ft_clip.hdf";

        private Inventory GetTestInventory()
        {
            StructureInventoryColumnMap map = new StructureInventoryColumnMap();
            //Empty (default) occupancy types
            OccupancyType occupancyType = new OccupancyType();
            List<OccupancyType> occupancyTypes = new List<OccupancyType>() { occupancyType };
            return new Inventory(pathToNSIShapefile, pathToIAShapefile, map, occupancyTypes, IANameColumnHeader);
        }

        [Fact]
        public void ConstructFromValidShapefile()
        {
            Inventory inventory = GetTestInventory();
            Assert.NotNull(inventory);
            Assert.Equal(3, inventory.ImpactAreas.Count);
            Assert.Equal(696, inventory.Structures.Count);
            Assert.Equal(4, inventory.DamageCategories.Count);
        }

        [Fact]
        public void GetGroundElevationFromTerrain()
        {
            Inventory inventory = GetTestInventory();
            float[] groundelevs = Inventory.GetGroundElevationFromTerrain(pathToNSIShapefile, pathToTerrainHDF);
            Assert.Equal(696, groundelevs.Length);
            Assert.Equal(946.5,groundelevs[0], 1);
        }
         [Fact]
         public void GetImpactAreaFID()
        {
            Inventory inv = GetTestInventory();
            PointM pnt = inv.GetPointMs()[0];
            int actual = Inventory.GetImpactAreaFID(pnt, pathToIAShapefile);
            Assert.Equal(0, actual);
        }
    }
}
