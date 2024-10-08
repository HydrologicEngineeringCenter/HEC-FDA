using Geospatial.Features;
using Geospatial.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Logging;
using Utility.Memory;

namespace HEC.FDA.Model.Spatial;
public class PointShapefile
{
    private const string SHAPEFILE_LOAD_FAILED = "The point shapefile has not been loaded. Call LoadStructureShapefile() first";
    private const string SHAPEFILE_CORRUPT = "The point shapefile failed to load correctly.";
    private PointFeatureCollection _pointCollection;

    public IReadOnlyList<TableRow> Rows { get => _pointCollection.AttributeTable.Rows; }
    public string[] ColumnNames { get => _pointCollection.AttributeTable.Columns.Select((c) => c.Name).ToArray(); }

    public PointShapefile(string pointShapefilePath)
    {
        OperationResult res = ShapefileWriter.TryReadShapefile(pointShapefilePath, out _pointCollection);
        if (!res) 
        {
            throw new Exception(SHAPEFILE_CORRUPT);
        }
    }

    /// <summary>
    /// Gets the row values as objects for the row of the specified id.
    /// </summary>
    public object[] GetRowValues(int id, string[] columnNames)
    {
        var row = Rows[id];
        object[] rowValues = new object[columnNames.Length];
        for (int i = 0; i < columnNames.Length; i++)
        {
            rowValues[i] = row.Value(columnNames[i]);
        }
        return rowValues;
    }

    /// <summary>
    /// Gets the column values as objects for all rows of the specified column.
    /// </summary>
    public object[] GetColumnValues(string columnName)
    {
        object[] columnvals = new object[Rows.Count];
        for(int i=0;i<columnvals.Length;i++)
        {
            columnvals[i] = Rows[i].Value(columnName);
        }
        return columnvals;
    }

    public Type GetColumnType(string columnName)
    {
        return _pointCollection.AttributeTable.GetColumn(columnName).Type;
    }
}
