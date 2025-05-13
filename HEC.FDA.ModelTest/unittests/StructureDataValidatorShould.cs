using Geospatial.Features;
using Geospatial.IO;
using HEC.FDA.Model.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Memory;
using Xunit;

namespace HEC.FDA.ModelTest.unittests;
[Trait("RunsOn", "Remote")]
public class StructureDataValidatorShould
{
    private static PointFeatureCollection pointShapefile = GetFeatures(Resources.StringResourcePaths.pathToNSIShapefile);

    private static PointFeatureCollection GetFeatures(string pointShapefilePath)
    {
        if(ShapefileIO.TryRead(pointShapefilePath, out PointFeatureCollection collection));
        {
            return collection;
        }
        throw new Exception("Failed to read shapefile");
    }

    [Fact]
    public void RowHasValuesForColumns_AllFieldsPresent_ReturnsTrue()
    {
        //Act
        bool valid = StructureDataValidator.RowHasValuesForColumns(pointShapefile, 0, new List<string> { "fd_id", "occtype", "yrbuilt" }, out List<string> missingValues);

        //Assert
        Assert.True(valid);
        Assert.True(missingValues.Count == 0);
    }

    [Fact]
    public void RowHasValuesForColumns_MissingFields_ReturnsFalse()
    {
        bool valid = StructureDataValidator.RowHasValuesForColumns(pointShapefile, 0, ["teachers"], out List<string> columnsWithMissingData);
        Assert.False(valid);
        Assert.True(columnsWithMissingData.Count == 1);
    }

    [Fact]
    public void RowsHaveValueForColumn_AllRowsHaveValue_ReturnsTrue()
    {
        bool valid = StructureDataValidator.RowsHaveValueForColumn(pointShapefile, "fd_id", out List<int> rowsWithMissingData);
        Assert.True(valid);
        Assert.True(rowsWithMissingData.Count == 0);

    }

    [Fact]
    public void RowsHaveValueForColumn_MissingValues_ReturnsFalse()
    {
        bool valid = StructureDataValidator.RowsHaveValueForColumn(pointShapefile, "teachers", out List<int> rowsWithMissingData);
        Assert.False(valid);
        Assert.True(rowsWithMissingData.Count == 1);
    }

    [Fact]
    public void AllRowsHaveUniqueValueForColumn_UniqueValues_ReturnsTrue()
    {
        //Act
        bool valid = StructureDataValidator.AllRowsHaveUniqueValueForColumn<int>(pointShapefile, "TARGET_FID", out List<int> rowsWithDupes);

        //Assert
        Assert.True(valid);
        Assert.True(rowsWithDupes.Count == 0);
    }

    [Fact]
    public void AllRowsHaveUniqueValueForColumn_DuplicateValues_ReturnsFalse()
    {
        //Act
        bool valid = StructureDataValidator.AllRowsHaveUniqueValueForColumn<int>(pointShapefile, "num_story", out List<int> rowsWithDupes);

        //Assert
        Assert.False(valid);
        Assert.True(rowsWithDupes.Count > 0);

    }
}