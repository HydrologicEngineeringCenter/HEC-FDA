using HEC.FDA.Model.structures;
using RasMapperLib;

namespace ScratchSpace.EntryPoints;
public static class Beam
{
    public static void EntryPoint()
    {
        string tiffile = @"C:\Temp\FDA Bugs\NewLondonOrleans\NewLondonOrleans\Hydraulic Data\FWOP\FWOP 0.002 AEP\WSE (Max).Northern_Gulf_of_Mexico_Topobathy_DEM_14.tif";
        string shapefile = @"C:\Temp\FDA Bugs\NewLondonOrleans\NewLondonOrleans\Structure Inventories\SELA Structures\SELA_Struc_Final.shp";

        PointMs pts = RASHelper.GetPointMsFromShapefile(shapefile);
        float[] wses = RASHelper.SamplePointsOnTiff(pts, tiffile);

        int i = 8968;

        Console.WriteLine($"WSEs: {wses.Length}");
            Console.WriteLine($"Structure {i+1}| WSE: {wses[i]} X: {pts[i].X} Y: {pts[i].Y}");
        Console.Read();
    }
}
