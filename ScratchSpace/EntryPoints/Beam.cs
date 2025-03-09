using Geospatial.Features;
using Geospatial.GDALAssist;
using Geospatial.GDALAssist.Vectors;
using Geospatial.IO;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.Spatial;
using RasMapperLib;

namespace ScratchSpace.EntryPoints;
public static class Beam
{
    public static void EntryPoint()
    {
        GDALSetup.InitializeMultiplatform();
        //string tiff = @"C:\Temp\_FDA\MySAC38_FragTest\MySAC38_FragTest\Hydraulic Data\Grids\80\WSE80Pct.tif";
        //string structureInv = @"C:\Temp\_FDA\MySAC38_FragTest\MySAC38_FragTest\Structure Inventories\Structs\Structure.shp";
        //string impactArea = @"C:\Temp\_FDA\MySAC38_FragTest\MySAC38_FragTest\Impact Areas\IA\Sac38_Zone_10N.shp";
        //Projection tiffProj = RASHelper.GetProjectionFromTerrain(tiff);
        //Projection siProj = RASHelper.GetVectorProjection(structureInv);
        //Projection iaProj = RASHelper.GetVectorProjection(impactArea);
        //bool same = tiffProj.Equals(iaProj);
        //PolygonFeatureLayer ias = new("",impactArea);
        //PolygonFeatureCollection pgsOut = new();
        //ShapefileWriter.TryReadShapefile(impactArea,out pgsOut);
        //var gon = pgsOut.Features[0];
        //var reprogon = gon.Reproject(iaProj, tiffProj);
        //Console.WriteLine(gon);
        //Console.WriteLine(reprogon);
        //List<Polygon> polygons = ias.Polygons().ToList();
        ////This reprojection is incorrect and failing. Move shapefile reader. 
        //List<Polygon> reproPolys = RASHelper.ReprojectPolygons(tiffProj, polygons, iaProj);
        //Console.WriteLine("garbage");

    }

    private static void TestIntegrate()
    {
        double[] xs = [.000001, 0.25, 0.5, 0.75, .999999];
        double[] ys = [0, 6000, 8600, 136000, 500000];
        PairedData pairedData = new(xs, ys);
        double integral = pairedData.integrate();
        Console.WriteLine(integral);
        Console.Read();
    }

    private static void Whatever()
    {
        string tiffile = @"C:\Temp\FDA Bugs\NewLondonOrleans\NewLondonOrleans\Hydraulic Data\FWOP\FWOP 0.002 AEP\WSE (Max).Northern_Gulf_of_Mexico_Topobathy_DEM_14.tif";
        string shapefile = @"C:\Temp\FDA Bugs\NewLondonOrleans\NewLondonOrleans\Structure Inventories\SELA Structures\SELA_Struc_Final.shp";

        PointMs pts = RASHelper.GetPointMsFromShapefile(shapefile);
        float[] wses = RASHelper.SamplePointsOnTiff(pts, tiffile);

        int i = 8968;

        Console.WriteLine($"WSEs: {wses.Length}");
        Console.WriteLine($"Structure {i + 1}| WSE: {wses[i]} X: {pts[i].X} Y: {pts[i].Y}");
        Console.Read();
    }
}
