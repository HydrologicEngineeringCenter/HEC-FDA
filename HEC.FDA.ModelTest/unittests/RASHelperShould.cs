using HEC.FDA.Model.structures;
using HEC.FDA.ModelTest.Resources;
using System.Collections.Generic;
using Xunit;

namespace HEC.FDA.ModelTest.unittests;
public class RASHelperShould
{
    [Fact]
    public void InvalidateInvalidShapefile()
    {
        string erorr = "";
        bool valid = RASHelper.ShapefileIsValid(Resources.StringResourcePaths.pathToIAShapefileNODBF, ref erorr);
        Assert.False(valid);
    }

    [Fact]
    public void ReturnAllComponentFilesOfAnHDFTerrain()
    {
       string terrainpath = Resources.StringResourcePaths.TerrainPath;
       List<string> terrainComponentFiles = RASHelper.GetTerrainComponentFiles(terrainpath);
        List<string> Freeport = RASHelper.GetTerrainComponentFiles(@"D:\FDA Data\Bugs\Freeport Run 1-4-24\Terrains\Freeport Terrain\NOAA_FPV04_LAS_GeotiffClipRes4_WOPv2.WOP.hdf");
        Assert.Equal(terrainComponentFiles.Count, 3);
    }
}
