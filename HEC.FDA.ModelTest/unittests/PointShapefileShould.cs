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
public class PointShapefileShould
{
    [Fact]
    [Trait("RunsOn", "Remote")]
    public void InvalidateInvalidShapefile()
    {
        Assert.Throws<Exception>(() => new PointShapefile(Resources.StringResourcePaths.pathToIAShapefileNODBF));
    }

    [Fact]
    public void LoadValidShapefile()
    {
        PointShapefile shapefile = new PointShapefile(Resources.StringResourcePaths.pathToNSIShapefile);
        Assert.NotNull(shapefile);
    }

    [Fact]
    public void GetRowValues_ReturnCorrectValues()
    {
        // Arrange
        PointShapefile shapefile = new PointShapefile(Resources.StringResourcePaths.pathToNSIShapefile);
        int rowId = 0;
        string[] columnNames = { "fd_id", "occtype", "yrbuilt" };
        object[] expectedRowValues = {26813713, "RES1-1SNB",1900};

        // Act
        object[] actualRowValues = shapefile.GetRowValues(rowId, columnNames);

        // Assert
        Assert.Equal((int)expectedRowValues[0], (int)actualRowValues[0]);
        Assert.Equal((string)expectedRowValues[1], (string)actualRowValues[1]);
        Assert.Equal((int)expectedRowValues[2], (int)actualRowValues[2]);
    }

    [Fact]
    public void GetColumnValues_ReturnCorrectValues()
    {
        // Arrange
        PointShapefile shapefile = new PointShapefile(Resources.StringResourcePaths.pathToNSIShapefile);
        string columnName = "fd_id";
        int[] expectedColumnValues = {26813713, 26814025, 26816202};

        // Act
        int[] actualColumnValues = shapefile.GetColumnValues(columnName).Cast<int>().ToArray();

        // Assert
        Assert.Equal(expectedColumnValues, actualColumnValues[..3]);
    }

    [Fact]
    public void GetColumnType_ReturnCorrectType()
    {
        // Arrange
        PointShapefile shapefile = new PointShapefile(Resources.StringResourcePaths.pathToNSIShapefile);
        string columnName = "fd_id";
        Type expectedColumnType = typeof(int);

        // Act
        Type actualColumnType = shapefile.GetColumnType(columnName);

        // Assert
        Assert.Equal(expectedColumnType, actualColumnType);
    }
}
