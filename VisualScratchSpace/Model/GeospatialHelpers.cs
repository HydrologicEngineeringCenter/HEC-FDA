using Geospatial.Features;
using Geospatial.IO;
using HEC.FDA.Model.Spatial.Extensions;
using RasMapperLib;
using RasMapperLib.Mapping;
using RasMapperLib.Utilities;
using Utility.Logging;

namespace VisualScratchSpace.Model;

public class GeospatialHelpers
{
    /// <summary>
    /// Create a map of summary zone names (polygons) to their associated index points
    /// </summary>
    /// <param name="polygonPath"></param>
    /// <param name="pointsPath"></param>
    /// <returns></returns>
    public static Dictionary<string, PointM> QueryPolygons(string polygonPath, string pointsPath)
    {
        OperationResult polygonResult = ShapefileIO.TryRead(polygonPath, out PolygonFeatureCollection polygons);
        if (!polygonResult.Result) return new Dictionary<string, PointM>();

        OperationResult pointsResult = ShapefileIO.TryRead(pointsPath, out PointFeatureCollection points);
        if (!pointsResult.Result) return new Dictionary<string, PointM>();

        Dictionary<string, PointM> result = new();
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < polygons.Count; j++)
            {
                if (polygons[j].Contains(points[i]))
                {
                    var row = polygons.AttributeTable.Rows[j];
                    string summaryZone = row.TryGetValueAs("Name", $"Polygon {j.ToString()}").TrimEnd();
                    result[summaryZone] = Converter.ConvertPtM(points[i]);
                }
            }
        }        
        return result;
    }

    /// <summary>
    /// Get stages/WSE from a RAS result at specified points
    /// </summary>
    /// <param name="pts"></param>
    /// <param name="hydraulicsPath"></param>
    /// <returns></returns>
    public static float[] GetStageFromHDF(PointMs pts, string hydraulicsPath)
    {
        var rasResult = new RASResults(hydraulicsPath);
        var rasGeometry = rasResult.Geometry;
        var rasWSMap = new RASResultsMap(rasResult, MapTypes.Elevation);
        RASGeometryMapPoints mapPixels = rasGeometry.MapPixels(pts);
        float[] WSE = null;
        int profileIndex = RASResultsMap.MaxProfileIndex;

        float[] mockTerrainElevs = new float[pts.Count];
        rasResult.ComputeSwitch(rasWSMap, mapPixels, profileIndex, mockTerrainElevs, null, ref WSE);
        return WSE;
    }
}
