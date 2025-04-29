using Geospatial.Features;
using Geospatial.IO;
using HEC.FDA.Model.Spatial;
using HEC.FDA.Model.structures;
using HEC.FDA.ModelTest.Resources;
using RasMapperLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using Xunit;

namespace HEC.FDA.ModelTest.unittests;
[Collection("Serial")]
[Trait("RunsOn", "Remote")]
public class PointShapefileShould
{
    private static PointFeatureCollection pointShapefile = GetFeatures(Resources.StringResourcePaths.pathToNSIShapefile);

    private static PointFeatureCollection GetFeatures(string pointShapefilePath)
    {
        if (ShapefileIO.TryRead(pointShapefilePath, out PointFeatureCollection collection)) ;
        {
            return collection;
        }
        throw new Exception("Failed to read shapefile");
    }

    [Fact]
    public void InvalidateInvalidShapefile()
    {
        Assert.False(ShapefileIO.TryRead(Resources.StringResourcePaths.pathToIAShapefileNODBF, out PolygonFeatureCollection collection));
    }

    [Fact]
    public void LoadValidShapefile()
    {
        Assert.True(ShapefileIO.TryRead(Resources.StringResourcePaths.pathToIAShapefile, out PolygonFeatureCollection collection));
    }

    [Fact]
    public void GetColumnValues_ReturnCorrectValues()
    {
        // Arrange
        if (!ShapefileIO.TryRead(Resources.StringResourcePaths.pathToNSIShapefile, out PointFeatureCollection shapefile))
        {
            throw new Exception("Failed to read shapefile");
        }
        string columnName = "fd_id";
        int[] expectedColumnValues = {26813713, 26814025, 26816202};

        // Act
        IEnumerable<object> actualColumnValues = shapefile.AttributeTable.Rows.Select((r) => (r.Value(columnName)));
        int[] intvals = actualColumnValues.Cast<int>().ToArray();


        // Assert
        Assert.Equal(expectedColumnValues, intvals[..3]);
    }

    [Fact]
    public void GetColumnType_ReturnCorrectType()
    {
        // Arrange
        if (!ShapefileIO.TryRead(Resources.StringResourcePaths.pathToNSIShapefile, out PointFeatureCollection shapefile))
        {
            throw new Exception("Failed to read shapefile");
        }
        string columnName = "fd_id";
        Type expectedColumnType = typeof(int);

        // Act
        Type actualColumnType = shapefile.AttributeTable.GetColumn(columnName).Type;

        // Assert
        Assert.Equal(expectedColumnType, actualColumnType);
    }
}
