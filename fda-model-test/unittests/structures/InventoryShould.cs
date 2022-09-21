using structures;
using System.IO;
using System.Reflection;
using Xunit;
using fda_model.structures;

namespace fda_model_test.unittests.structures
{
    public class InventoryShould
    {
        [Fact]
        public void ConstructFromValidShapefile()
        {
            string pathToNSIShapefile = @"..\..\..\Resources\MuncieNSI\MuncieNSI.shp";
            string pathToIAShapefile = @"..\..\..\Resources\MuncieImpactAreas\ImpactAreas.shp";
            StructureInventoryColumnMap map = new StructureInventoryColumnMap();

            Inventory inventory = new Inventory(pathToNSIShapefile, pathToIAShapefile, map);

            Assert.NotNull(inventory);
            Assert.Equal(3, inventory.ImpactAreas.Count);
            Assert.Equal(696, inventory.Structures.Count);
            Assert.Equal(4, inventory.DamageCategories.Count);
        }
    }
}
