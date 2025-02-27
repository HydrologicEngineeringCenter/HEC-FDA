using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.Spatial;
using RasMapperLib;

namespace ScratchSpace.EntryPoints;
public static class Beam
{
    public static void EntryPoint()
    {
        try
        {
            //string path = @"C:\Temp\_FDA\Jefferson City\Jeff City Sensitivity Analysis\Structure Inventories\New folder";
            string path = @"C:\Temp\_FDA\Jefferson City\Jeff City Sensitivity Analysis\Structure Inventories\JC Strucs";
            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }
            //Directory.Delete(@"C:\Temp\_FDA\Jefferson City\Jeff City Sensitivity Analysis\Structure Inventories\JC Strucs");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to delete directory " + ex.Message);
        }
        
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
