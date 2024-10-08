using Geospatial.Features;
using Geospatial.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Extensions;
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
    public object[] GetRowAsObjects(int id)
    {
        string[] columns = _pointCollection.AttributeTable.Columns.Select((c) => c.Name).ToArray();
        var row = Rows[id];
        object[] rowValues = new object[columns.Length];
        for (int i = 0; i < columns.Length; i++)
        {
            rowValues[i] = row.Value(columns[i]);
        }
        return rowValues;
    }
}
