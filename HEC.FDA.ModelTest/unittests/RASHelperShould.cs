using Amazon.S3.Model;
using Geospatial.Features;
using Geospatial.Vectors;
using HEC.FDA.Model.Spatial;
using HEC.FDA.Model.structures;
using HEC.FDA.ModelTest.Resources;
using RasMapperLib;
using System.Collections.Generic;
using Utilities;
using Utility.Memory;
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
        List<string> terrainComponentFiles = RASHelper.GetTerrainComponentFiles(Resources.StringResourcePaths.TerrainPath, ref error);
        Assert.True(error.IsNullOrEmpty());
        Assert.Equal(terrainComponentFiles.Count, 5);
    }

    [Fact]
    [Trait("RunsOn", "Remote")]
    public void PolygonsContainPoints()
    {
        // === 1. Polygon Feature Collection (3 non-overlapping polygons) ===
        var polygons = new PolygonFeatureCollection();
        polygons.AttributeTable.AddColumn("Name", typeof(string));

        var row = polygons.Add(new Geospatial.Vectors.Polygon { { 0, 0 }, { 10, 0 }, { 10, 10 }, { 0, 10 } });
        row["Name"] = "A";

        row = polygons.Add(new Geospatial.Vectors.Polygon { { 20, 0 }, { 30, 0 }, { 30, 10 }, { 20, 10 } });
        row["Name"] = "B";

        row = polygons.Add(new Geospatial.Vectors.Polygon { { 40, 0 }, { 50, 0 }, { 50, 10 }, { 40, 10 } });
        row["Name"] = "C";

        // === 2. Points with 1-1 mapping (exactly one point inside each polygon) ===
        var pointsOneToOne = new PointFeatureCollection(new PointCollection(), new Table());
        pointsOneToOne.AttributeTable.AddColumn("Label", typeof(string));

        row = pointsOneToOne.Add(new Geospatial.Vectors.Point(5, 5));     // inside polygon A
        row["Label"] = "InA";

        row = pointsOneToOne.Add(new Geospatial.Vectors.Point(25, 5));    // inside polygon B
        row["Label"] = "InB";

        row = pointsOneToOne.Add(new Geospatial.Vectors.Point(45, 5));    // inside polygon C
        row["Label"] = "InC";

        // === 3. Points WITHOUT 1-1 mapping ===
        //    - Polygon A gets 2 points
        //    - Polygon B gets 0 points
        //    - Polygon C gets 1 point
        //    - 1 point is outside all polygons
        var pointsNoMatch = new PointFeatureCollection(new PointCollection(), new Table());
        pointsNoMatch.AttributeTable.AddColumn("Label", typeof(string));

        row = pointsNoMatch.Add(new Geospatial.Vectors.Point(3, 3));      // inside polygon A
        row["Label"] = "InA_1";

        row = pointsNoMatch.Add(new Geospatial.Vectors.Point(7, 7));      // inside polygon A (duplicate)
        row["Label"] = "InA_2";

        row = pointsNoMatch.Add(new Geospatial.Vectors.Point(45, 5));     // inside polygon C
        row["Label"] = "InC";

        row = pointsNoMatch.Add(new Geospatial.Vectors.Point(100, 100));  // outside all polygons
        row["Label"] = "Outside";

        bool containsPointsOneToOne = RASHelper.TryMapPolygonsToPoints(polygons, pointsOneToOne, "Name", out _);
        bool containsPointsNoMatch = RASHelper.TryMapPolygonsToPoints(polygons, pointsNoMatch, "Name", out _);
        Assert.True(containsPointsOneToOne);
        Assert.False(containsPointsNoMatch);
    }
}
