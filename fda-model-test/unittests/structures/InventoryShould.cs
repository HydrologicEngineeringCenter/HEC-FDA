﻿using Xunit;
using System.Collections.Generic;
using HEC.FDA.Model.structures;

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
            //Empty (default) occupancy types
            OccupancyType occupancyType = new OccupancyType();
            List<OccupancyType> occupancyTypes = new List<OccupancyType>() { occupancyType };
            Inventory inventory = new Inventory(pathToNSIShapefile, pathToIAShapefile, map, occupancyTypes);

            Assert.NotNull(inventory);
            Assert.Equal(3, inventory.ImpactAreas.Count);
            Assert.Equal(696, inventory.Structures.Count);
            Assert.Equal(4, inventory.DamageCategories.Count);
        }
    }
}