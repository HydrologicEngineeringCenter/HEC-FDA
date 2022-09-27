using structures;
using System.IO;
using System.Reflection;
using Xunit;
using fda_model.structures;
using RasMapperLib;
using System.Collections.Generic;

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

        [Fact]
        public void SaveGroundElevationFromTerrainToShapefile()
        {
            string OutputFilePath = Path.GetTempPath() + "MuncieNSI.shp";
            string sourceDir = @"..\..\..\Resources\MuncieNSI\";
            string destDir = Path.GetTempPath();
            string pathToTerrain = @"..\..\..\Resources\MuncieTerrain\Terrain (1)_30ft_clip.hdf";
            List<string> outputFiles = new List<string>();

            var nsiShapefileFiles = Directory.GetFiles(sourceDir);
            foreach (var filePath in nsiShapefileFiles)
            {
                var newFile = filePath.Replace(sourceDir, destDir);
                File.Copy(filePath, newFile, true);
                outputFiles.Add(newFile);
            }

            Inventory.SaveGroundElevationFromTerrainToShapefile(OutputFilePath, pathToTerrain);
            PointFeatureLayer SI = new PointFeatureLayer("structures",OutputFilePath);
            var row = SI.FeatureRow(0);
            var value = Inventory.TryGet<double>(row["ground_elv"]);
            Assert.True(value > 600);

            foreach(string file in outputFiles)
            {
                File.Delete(file);
            }
        }
    }
}
