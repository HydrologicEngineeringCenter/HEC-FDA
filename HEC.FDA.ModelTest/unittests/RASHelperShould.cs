using HEC.FDA.Model.structures;
using HEC.FDA.ModelTest.Resources;
using RasMapperLib;
using System.Collections.Generic;
using Utilities;
using Xunit;

namespace HEC.FDA.ModelTest.unittests;
[Collection("Serial")]
public class RASHelperShould
{
    [Fact]
    [Trait("RunsOn", "Remote")]
    public void InvalidateInvalidShapefile()
    {
        string erorr = "";
        bool valid = RASHelper.ShapefileIsValid(Resources.StringResourcePaths.pathToIAShapefileNODBF, ref erorr);
        Assert.False(valid);
    }

    [Fact]
    [Trait("RunsOn", "Remote")]
    public void ReturnAllComponentFilesOfAnHDFTerrain()
    {
        string error = "";
        List<string> terrainComponentFiles = RASHelper.GetTerrainComponentFiles(Resources.StringResourcePaths.TerrainPath,ref error);
        Assert.True(error.IsNullOrEmpty());
        Assert.Equal(terrainComponentFiles.Count, 5); 
    }
}
