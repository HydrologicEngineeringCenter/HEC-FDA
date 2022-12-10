using Xunit;
using System.Collections.Generic;
using HEC.FDA.Model.structures;
using RasMapperLib;

namespace HEC.FDA.ModelTest.unittests.structures
{
    [Trait("Category", "Disk")]
    public class InventoryShould
    {
        private const string IANameColumnHeader = "Name";
        private const string pathToNSIShapefile = @"..\..\..\fda-model-test\Resources\MuncieNSI\Muncie-SI_CRS2965.shp";
        private const string pathToIAShapefile = @"..\..\..\fda-model-test\Resources\MuncieImpactAreas\ImpactAreas.shp";
        private const string pathToTerrainHDF = @"..\..\..\fda-model-test\Resources\MuncieTerrain\Terrain (1)_30ft_clip.hdf";

        private Inventory GetTestInventory(bool useTerrainFile)
        {
            StructureInventoryColumnMap map = new StructureInventoryColumnMap(null,null,null, null, null, null, null, null, null, null, null, null, null, null);
            //Empty (default) occupancy types
            OccupancyType occupancyType = new OccupancyType();
            List<OccupancyType> occupancyTypes = new List<OccupancyType>() { occupancyType };
            return new Inventory(pathToNSIShapefile, pathToIAShapefile, map, occupancyTypes, IANameColumnHeader, useTerrainFile, pathToTerrainHDF);
        }

        [Fact]
        public void ConstructFromValidShapefile()
        {
            Inventory inventory = GetTestInventory(false);
            Assert.NotNull(inventory);
            Assert.Equal(4, inventory.ImpactAreas.Count);
            Assert.Equal(682, inventory.Structures.Count);
            Assert.Equal(4, inventory.DamageCategories.Count);
        }

        [Fact]
        public void GetGroundElevationFromTerrain()
        {
            float[] groundelevs = Inventory.GetGroundElevationFromTerrain(pathToNSIShapefile, pathToTerrainHDF);
            Assert.Equal(682, groundelevs.Length);//Was 696. Is this min elevation or what?
            Assert.Equal(946.5,groundelevs[0], 1);
        }
         [Fact]
         public void GetImpactAreaFID()
        {
            Inventory inv = GetTestInventory(false);
            PointM pnt = inv.GetPointMs()[0];
            int actual = inv.GetImpactAreaFID(pnt);
            Assert.Equal(0, actual);
        }
        [Fact]
        public void ConstructsWithTerrainGroundElevs()
        {
            Inventory inv = GetTestInventory(true);
            Assert.Equal(682, inv.Structures.Count);//Was 696
            Assert.True(inv.Structures[0].FirstFloorElevation > 900);
        }
        [Fact]
        public void filterInventoryToIAPolygon()
        {
            Inventory inv = GetTestInventory(false);
            Inventory trimmedInv1 = inv.GetInventoryTrimmmedToPolygon(0);
            Inventory trimmedInv2 = inv.GetInventoryTrimmmedToPolygon(1);
            Inventory trimmedInv3 = inv.GetInventoryTrimmmedToPolygon(2);
            Inventory trimmedInv4 = inv.GetInventoryTrimmmedToPolygon(3);
            int countActual = inv.Structures.Count;
            int count1 = trimmedInv1.Structures.Count;
            int count2 = trimmedInv2.Structures.Count;
            int count3 = trimmedInv3.Structures.Count;
            int count4 = trimmedInv4.Structures.Count;
            Assert.Equal(countActual, count1 + count2 + count3 + count4);
        }
        [Fact]
        public void returnsUniqueImpactAreaIDs()
        {
            Inventory inv = GetTestInventory(false);
            List<int> uniqueImpactAreaIDs = inv.ImpactAreas;
            Assert.Equal(4, uniqueImpactAreaIDs.Count);
        }
    }
}
