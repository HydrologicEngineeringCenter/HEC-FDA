using Geospatial.Features;
using Geospatial.IO;
using HEC.FDA.Model.Spatial.Extensions;
using System;
using System.Collections.Generic;

namespace HEC.FDA.Model.Spatial;
public class ShapefileHelper
{
    private string _shpPath;
    public ShapefileHelper(string shpPath)
    {
        _shpPath = shpPath;
    }

    public Dictionary<string, Type> GetColumns()
    {
        ShapefileIO.TryRead(_shpPath, out PolygonFeatureCollection polygons);
        var columns = polygons.AttributeTable.Columns;

        Dictionary<string, Type> columnMap = [];
        foreach (var column in columns)
        {
            columnMap[column.Name] = column.Type;
        }
        return columnMap;
    }

    public List<string> GetColumnValues(string column)
    {
        ShapefileIO.TryRead(_shpPath, out PolygonFeatureCollection polygons);
        var rows = polygons.AttributeTable.Rows;
        var columnType = polygons.AttributeTable.GetColumn(column).Type;
        List<string> values = [];
        for (int i = 0; i < rows.Count; i++)
        {
            if (columnType == typeof(int))
            {
                int intValue = rows[i].TryGetValueAs(column, 0);
                values.Add(intValue.ToString());
            }
            else if (columnType == typeof(double))
            {
                double doubleValue = rows[i].TryGetValueAs(column, 0.0);
                values.Add(doubleValue.ToString());
            }
            else if (columnType == typeof(string))
            {
                string stringValue = rows[i].TryGetValueAs(column, string.Empty);
                values.Add(stringValue);
            }
            else
            {
                values.Add(string.Empty);
            }
        }
        return values;
    }


}
