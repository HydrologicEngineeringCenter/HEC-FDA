using Geospatial.GDALAssist;
using Geospatial.GDALAssist.Vectors;
using RasMapperLib;
using RasMapperLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace HEC.FDA.Model.structures;

public static class RASHelper
{
    public static float[] GetGroundElevationFromRASTerrain(PointFeatureLayer pointLayer, TerrainLayer terrain, Projection studyProjection)
    {
        Projection siProjection = GetVectorProjection(pointLayer);
        PointMs pointMs = new(pointLayer.Points().Select(p => p.PointM()));
        if (!studyProjection.IsNull())
        {
            if (!studyProjection.IsEqual(siProjection))
            {
                pointMs = ReprojectPoints(studyProjection, siProjection, pointMs);
            }
        }
        return terrain.ComputePointElevations(pointMs);
    }
    public static int GetImpactAreaFID(PointM point, List<Polygon> ImpactAreas)
    {
        for (int i = 0; i < ImpactAreas.Count; i++)
        {
            if (ImpactAreas[i].Contains(point))
            {
                return i;
            }
        }
        return -9999;
    }
    public static Projection GetVectorProjection(FeatureLayer featureLayer)
    {
        string siFilename = featureLayer.SourceFilename;
        VectorDataset vector = new(siFilename);
        VectorLayer vectorLayer = vector.GetLayer(0);
        Projection projection = vectorLayer.GetProjection();
        return projection;
    }
    public static PointM ReprojectPoint(PointM point, Projection newProjection, Projection currentProjection)
    {

        Geospatial.Vectors.Point p = Converter.Convert(point);
        Geospatial.Vectors.Point newp = VectorExtensions.Reproject(p, currentProjection, newProjection);
        return Converter.ConvertPtM(newp);
    }

    public static PointMs ReprojectPoints(Projection studyProjection, Projection siProjection, PointMs pointMs)
    {
        PointMs reprojPointMs = new();
        foreach (PointM pt in pointMs)
        {
            reprojPointMs.Add(ReprojectPoint(pt, studyProjection, siProjection));
        }
        return reprojPointMs;

    }
    public static Polygon ReprojectPolygon(Polygon polygon, Projection newProjection, Projection currentProjection)
    {
        Geospatial.Vectors.Polygon poly = Converter.Convert(polygon);
        Geospatial.Vectors.Polygon reprojPoly = VectorExtensions.Reproject(poly, currentProjection, newProjection);
        return Converter.Convert(reprojPoly);

    }

    public static T TryGet<T>(object value, T defaultValue = default)
where T : struct
    {
        if (value == null)
            return defaultValue;
        else if (value == DBNull.Value)
            return defaultValue;
        else
        {
            var retn = value as T?;
            if (retn.HasValue)
                return retn.Value;
            else
                return defaultValue;
        }
    }
    public static T TryGetObj<T>(object value, T defaultValue = default)
        where T : class
    {
        if (value == null)
            return defaultValue;
        else if (value == DBNull.Value)
            return defaultValue;
        else
        {
            if (value is T retn)
                return retn;
            else
                return defaultValue;
        }
    }
    public static T GetRowValueForColumn<T>(System.Data.DataRow row, string mappingColumnName, T defaultValue) where T : struct
    {
        T retval = defaultValue;
        if (mappingColumnName != null && row.Table.Columns.Contains(mappingColumnName))
        {
            //column could have wrong data type, or be null, or dbnull
            retval = TryGet<T>(row[mappingColumnName], defaultValue);
        }
        return retval;
    }
    public static string GetRowValueForColumn(System.Data.DataRow row, string mappingColumnName, string defaultValue)
    {
        string retval = defaultValue;
        if (mappingColumnName != null && row.Table.Columns.Contains(mappingColumnName))
        {
            //column could have wrong data type, or be null, or dbnull
            retval = TryGetObj<string>(row[mappingColumnName], defaultValue);
        }
        return retval;
    }

    public static List<Polygon> LoadImpactAreasFromSourceFiles(PolygonFeatureLayer impactAreaSet, Projection studyProjection)
    {
        List<Polygon> polygons = impactAreaSet.Polygons().ToList();
        Projection impactAreaPrj = GetVectorProjection(impactAreaSet);
        if (studyProjection.IsNull())
        {
            return polygons;
        }
        if (impactAreaPrj.IsEqual(studyProjection))
        {
            return polygons;
        }
        else
        {
            return ReprojectPolygons(studyProjection, polygons, impactAreaPrj);
        }
    }

    public static List<Polygon> ReprojectPolygons(Projection studyProjection, List<Polygon> polygons, Projection impactAreaPrj)
    {
        List<Polygon> ImpactAreas = new();
        foreach (Polygon poly in polygons)
        {
            Polygon newPoly = ReprojectPolygon(poly, impactAreaPrj, studyProjection);
            ImpactAreas.Add(newPoly);
        }
        return ImpactAreas;
    }
}