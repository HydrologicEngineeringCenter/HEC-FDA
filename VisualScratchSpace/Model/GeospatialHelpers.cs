using Geospatial.Features;
using Geospatial.IO;
using HEC.FDA.Model.Spatial.Extensions;
using RasMapperLib;
using RasMapperLib.Utilities;
using Utility.Logging;

namespace VisualScratchSpace.Model;

public class GeospatialHelpers
{
    public static Dictionary<string, PointM>? QueryPolygons(string polygonPath, string pointsPath)
    {
        OperationResult polygonResult = ShapefileIO.TryRead(polygonPath, out PolygonFeatureCollection polygons);
        if (!polygonResult.Result) return null;

        OperationResult pointsResult = ShapefileIO.TryRead(pointsPath, out PointFeatureCollection points);
        if (!pointsResult.Result) return null;

        Dictionary<string, PointM> result = new();
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < polygons.Count; j++)
            {
                if (polygons[j].Contains(points[i]))
                {
                    var row = polygons.AttributeTable.Rows[j];
                    string summaryZone = row.TryGetValueAs("Name", $"Polygon {j.ToString()}");
                    result[summaryZone] = Converter.ConvertPtM(points[i]);
                }
            }
        }        
        return result;
    }
}
