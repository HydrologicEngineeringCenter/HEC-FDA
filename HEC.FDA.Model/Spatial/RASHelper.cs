using Geospatial.Features;
using Geospatial.GDALAssist;
using Geospatial.GDALAssist.Vectors;
using Geospatial.IO;
using Geospatial.Vectors;
using HEC.FDA.Model.Spatial.Extensions;
using RasMapperLib;
using RasMapperLib.Mapping;
using RasMapperLib.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities;
using Utility.Extensions;
using Utility.Logging;

namespace HEC.FDA.Model.Spatial;

public static class RASHelper
{
    public static float[] SamplePointsFromTerrain(string pointShapefilePath, string rasterPath, Projection rasterProjection)
    {
        OperationResult or = ShapefileIO.TryRead(pointShapefilePath, out PointFeatureCollection pointFeatures, rasterProjection);
        if (!or.Result)
        {
            throw new Exception("Failed to read shapefile: " + pointShapefilePath);
        }
        IVectorCollection<Geospatial.Vectors.Point> points = pointFeatures.Features;
        return SamplePointsFromTerrain(points, rasterPath);
    }
    public static float[] SamplePointsFromTerrain(IVectorCollection<Geospatial.Vectors.Point> points, string rasterPath)
    {
        string extension = System.IO.Path.GetExtension(rasterPath);
        float[] groundelevs;
        switch (extension)
        {
            case ".hdf":
                //This is supporting a HEC-RAS 6.x terrain, which is why we're in RasMapperLib and using PointMs.
                TerrainLayer layer = new("thisNameIsNotUsed", rasterPath);
                PointM[] pointms = GeospatialPointsToPointMs(points);
                groundelevs = layer.ComputePointElevations(pointms);
                break;
            case ".tif":
                groundelevs = SamplePointsOnTiff(points, rasterPath);
                break;
            default:
                throw new Exception("The file type is invalid.");
        }
        return groundelevs;
    }

    private static PointM[] GeospatialPointsToPointMs(IVectorCollection<Geospatial.Vectors.Point> points)
    {
        PointM[] pointMs = new PointM[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            pointMs[i] = Converter.ConvertPtM(points[i]);
        }
        return pointMs;
    }

    /// <summary>
    /// Expects the points to be in the same projection as the raster.
    /// </summary>
    public static float[] SamplePointsOnTiff(PointMs pts, string filePath)
    {
        List<Geospatial.Vectors.Point> geospatialpts = Converter.Convert(pts);
        PointCollection vectorCollection = new(geospatialpts);
        return SamplePointsOnTiff(vectorCollection, filePath);
    }

    public static float[] SamplePointsOnTiff(IVectorCollection<Geospatial.Vectors.Point> geospatialpts, string filePath)
    {
        GdalBandedRaster<float> resultsGrid = new(filePath);
        Memory<Geospatial.Vectors.Point> points = new([.. geospatialpts]);
        float[] elevationData = new float[points.Length];
        resultsGrid.SamplePoints(points, 0, elevationData); //this writes float.MinValue for NoData.
        OverwriteNoDataValues(ref elevationData);
        return elevationData;
    }

    /// <summary>
    /// Checks for float.MinValue and replaces it with Geospatial.Constants.NoDataF
    /// </summary>
    private static void OverwriteNoDataValues(ref float[] elevations)
    {
        for (int i = 0; i < elevations.Length; i++)
        {
            if (elevations[i] == float.MinValue)
            {
                elevations[i] = Geospatial.Constants.NoDataF;
            }
        }
    }

    /// <summary>
    /// Takes a file path, checks whether it has a .hdf or .tif extension, and returns the projection of the file.
    /// </summary>
    /// <param name="TerrainPath"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Projection GetProjectionFromTerrain(string TerrainPath)
    {
        string terrainExtension = System.IO.Path.GetExtension(TerrainPath);
        string terrainTif;
        string error = "";
        switch (terrainExtension)
        {
            case ".hdf":
                TerrainLayer terrain = new("ThisNameIsNotUSed", TerrainPath);
                List<string> files = GetAllSourceFilesFromTerrainSAFE(terrain, ref error);
                if (!error.IsNullOrEmpty())
                {
                    throw new Exception(error);
                }
                terrainTif = files[2]; //index 0 is always the hdf. 1 is the vrt. 2 is the first tif.  
                break;
            case ".tif":
                terrainTif = TerrainPath;
                break;
            default:
                throw new Exception("Unsupported File Type");
        }
        GDALRaster raster = new(terrainTif);
        return raster.GetProjection();
    }

    public static Projection GetVectorProjection(string fileName)
    {
        VectorDataset vector = new(fileName);
        VectorLayer vectorLayer = vector.GetLayer(0);
        Projection projection = vectorLayer.GetProjection();
        return projection;
    }

    public static Projection GetVectorProjection(FeatureLayer featureLayer)
    {
        string siFilename = featureLayer.SourceFilename;
        return GetVectorProjection(siFilename);
    }

    public static PointM ReprojectPoint(PointM point, Projection newProjection, Projection currentProjection)
    {
        Geospatial.Vectors.Point p = Converter.Convert(point);
        Geospatial.Vectors.Point newp = VectorExtensions.Reproject(p, currentProjection, newProjection);
        return Converter.ConvertPtM(newp);
    }

    /// <summary>
    /// returns all the component files of the terrain. Does not gaurantee they exist, just that they should for the terrain to be complete. 
    /// </summary>
    /// <param name="terrainPath"></param>
    /// <returns></returns>
    public static List<string> GetTerrainComponentFiles(string terrainPath, ref string error)
    {
        string terrainExtension = System.IO.Path.GetExtension(terrainPath);
        switch (terrainExtension)
        {
            case ".hdf":
                TerrainLayer terrain = new("ThisNameIsNotUSed", terrainPath);
                List<string> files = GetAllSourceFilesFromTerrainSAFE(terrain, ref error);
                return files;
            case ".tif":
                return new() { terrainPath };
            default:
                throw new Exception("Unsupported File Type");
        }
    }
    public static bool ShapefileIsValid(string path, ref string error)
    {
        return ShapefileStorage.IsValid(path, ref error);
    }
    public static bool IsPolygonShapefile(string path, ref string error)
    {
        bool valid = ShapefileIO.IsPolygonShapefile(path);
        if (!valid)
        {
            error += " Not a polygon shapefile. ";
        }
        return valid;
    }
    public static bool IsPointShapefile(string path, ref string error)
    {
        bool valid = ShapefileIO.IsPointShapefile(path);
        if (!valid)
        {
            error += " Not a point shapefile. ";
        }
        return valid;
    }
    /// <summary>
    /// Checks that the actual file and all component files exist. 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static bool TerrainIsValid(string path, ref string error)
    {
        if (!File.Exists(path))
        {
            error += $"File {path} does not exist. ";
            return true;
        }
        string terrainExtension = System.IO.Path.GetExtension(path);
        switch (terrainExtension)
        {
            case ".hdf":
                TerrainLayer terrain = new("ThisNameIsNotUSed", path);
                List<string> files = GetAllSourceFilesFromTerrainSAFE(terrain, ref error);
                bool allFilesExist = true;
                foreach (string file in files)
                {
                    if (!File.Exists(file))
                    {
                        error += $"File {file} does not exist. " + Environment.NewLine;
                        allFilesExist = false;
                    }
                }
                return allFilesExist;
            case ".tif":
                return File.Exists(path);
            default:
                error += $"Unsupported file type: {terrainExtension}";
                return false;
        }
    }

    public static PointMs GetPointMsFromShapefile(string path)
    {
        PointFeatureLayer pointLayer = new("thisNameIsntUsed", path);
        PointMs pointMs = new(pointLayer.Points().Select(p => p.PointM()));
        return pointMs;
    }

    public static Dictionary<string, PointM> QueryPolygons(string polygonPath, string pointsPath, string polygonColumnName)
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
                    object value = row.TryGetValueAs<object>(polygonColumnName, null);
                    string polygonName = value?.ToString()?.TrimEnd() ?? $"Polygon {j}";
                    result[polygonName] = Converter.ConvertPtM(points[i]);
                }
            }
        }
        return result;
    }

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
    #region HACKS

    ///This contains a workaround for an issue in RASMapper, where to query the component files of an HDF, RASMapper was generating a VRT using GDAL .exes that don't exist
    ///in the version 7 build of GDAL. It also protects against another issue where when specified with a relative path, GetDependency Files returns duplicate files, not recognizing 
    /// that the relative path is the same as the absolute path.
    private static List<string> GetAllSourceFilesFromTerrainSAFE(TerrainLayer terrain, ref string error)
    {
        List<string> sourcefiles = new();
        bool realVRTFileExists = File.Exists(terrain.VRTFilename);
        if (!realVRTFileExists) //This is a hack to get around a bug in RasMapper  which throws an exception trying to create a VRT when we query the source files.
        {
            CreateFakeVRTForTerrain(terrain);
        }
        try
        {
            sourcefiles = terrain.GetDependencyFiles();
        }
        catch (Exception ex)
        {
            error += ex.Message;
        }
        finally
        {
            //delete that fake vrt. I don't want to store fake data to disk if I don't have to.
            if (!realVRTFileExists)
            {
                File.Delete(terrain.VRTFilename);
            }
            //Get Dependency files returns duplicate files if the terrain layer is created with a relative path. There's matching relative and absolute paths for the same file.
            //This is a hack to get around that.
            HashSet<string> finalListFiles = new();
            foreach (string file in sourcefiles)
            {
                finalListFiles.Add(System.IO.Path.GetFullPath(file));
            }
            sourcefiles = finalListFiles.ToList();
        }
        return sourcefiles;
    }
    private static void CreateFakeVRTForTerrain(TerrainLayer terrain)
    {
        using StreamWriter writer = new(terrain.VRTFilename);
        writer.Write(vrtFileContents);
        writer.Flush();
    }
    private static string vrtFileContents = "<VRTDataset rasterXSize=\"512\" rasterYSize=\"512\">\r\n  <SRS dataAxisToSRSAxisMapping=\"1,2\">PROJCS[\"NAD83 / Indiana East (ftUS)\",GEOGCS[\"NAD83\",DATUM[\"North_American_Datum_1983\",SPHEROID[\"GRS 1980\",6378137,298.257222101004,AUTHORITY[\"EPSG\",\"7019\"]],AUTHORITY[\"EPSG\",\"6269\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4269\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",37.5],PARAMETER[\"central_meridian\",-85.6666666666667],PARAMETER[\"scale_factor\",0.999966666666667],PARAMETER[\"false_easting\",328083.333333333],PARAMETER[\"false_northing\",820208.333333333],UNIT[\"US survey foot\",0.304800609601219,AUTHORITY[\"EPSG\",\"9003\"]],AXIS[\"Easting\",EAST],AXIS[\"Northing\",NORTH]]</SRS>\r\n  <GeoTransform>  4.0033784867792000e+05,  3.0000000000000000e+01,  0.0000000000000000e+00,  1.8115535402635999e+06,  0.0000000000000000e+00, -3.0000000000000000e+01</GeoTransform>\r\n  <VRTRasterBand dataType=\"Float32\" band=\"1\">\r\n    <Metadata>\r\n      <MDI key=\"STATISTICS_MAXIMUM\">980.65625</MDI>\r\n      <MDI key=\"STATISTICS_MEAN\">950.96604100505</MDI>\r\n      <MDI key=\"STATISTICS_MINIMUM\">910.6875</MDI>\r\n      <MDI key=\"STATISTICS_STDDEV\">10.26791635529</MDI>\r\n    </Metadata>\r\n    <NoDataValue>-9999</NoDataValue>\r\n    <ColorInterp>Gray</ColorInterp>\r\n    <Histograms>\r\n      <HistItem>\r\n        <HistMin>910.6875</HistMin>\r\n        <HistMax>980.65625</HistMax>\r\n        <BucketCount>256</BucketCount>\r\n        <IncludeOutOfRange>1</IncludeOutOfRange>\r\n        <Approximate>0</Approximate>\r\n        <HistCounts>14|12|12|12|20|21|19|18|23|25|40|20|32|26|44|31|36|42|36|30|36|53|49|39|45|30|45|32|34|29|42|29|31|19|29|27|29|30|30|27|32|47|38|27|60|49|50|49|57|64|52|55|78|94|132|113|150|172|239|214|311|257|263|251|291|333|373|452|471|518|467|562|591|602|532|754|800|860|780|851|890|839|773|885|816|832|781|980|1003|1043|951|1019|992|1060|997|1002|946|921|862|1025|1058|1104|1120|1260|1228|1309|1267|1406|1364|1406|1302|1610|1670|1537|1404|1564|1650|1762|1547|1872|1952|1856|1656|1849|1992|2093|1830|2104|2229|2076|2526|2706|2656|2284|2538|2473|2482|2225|2600|2549|2459|2248|2604|2678|2864|2471|2889|2949|2789|2303|2578|2449|2438|2109|2401|2376|2318|2048|2326|2355|2448|2081|2229|2218|2183|2111|2442|2567|2716|2277|2668|2834|2968|2621|3011|2886|2779|2535|2741|2461|2336|2004|2235|2458|2331|2051|2223|2311|2147|1795|1938|1924|1640|1898|1940|2093|1915|2032|1671|1532|1353|1400|1334|1224|1090|995|911|972|720|803|773|686|473|445|380|368|292|289|227|221|154|122|97|70|59|50|49|32|21|28|19|12|8|8|5|6|2|0|1|0|0|0|0|1|0|1|0|3|0|1|0|0|0|0|2|1</HistCounts>\r\n      </HistItem>\r\n    </Histograms>\r\n    <ComplexSource>\r\n      <SourceFilename relativeToVRT=\"1\">Terip.Resampled_clip.tif</SourceFilename>\r\n      <SourceBand>1</SourceBand>\r\n      <SourceProperties RasterXSize=\"512\" RasterYSize=\"512\" DataType=\"Float32\" BlockXSize=\"256\" BlockYSize=\"256\" />\r\n      <SrcRect xOff=\"0\" yOff=\"0\" xSize=\"512\" ySize=\"512\" />\r\n      <DstRect xOff=\"0\" yOff=\"0\" xSize=\"512\" ySize=\"512\" />\r\n      <NODATA>-9999</NODATA>\r\n    </ComplexSource>\r\n  </VRTRasterBand>\r\n</VRTDataset>";
    #endregion


}