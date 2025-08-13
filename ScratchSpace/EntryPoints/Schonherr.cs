using HEC.FDA.Model.Spatial;

namespace ScratchSpace.EntryPoints;
internal class Schonherr
{
    public static void EntryPoint()
    {
        ShapefileHelper bad = new(@"C:\FDA_Test_Data\WKS20230525\WKS20230525\re-exported.shp");
        var badColumns = bad.GetColumns();
        foreach (KeyValuePair<string, Type> kvp in badColumns)
        {
            Console.WriteLine($"Key = {kvp.Key}, Value = {kvp.Value}");
        }

        Console.WriteLine("\r\nName:");
        var nameValues = bad.GetColumnValues("Name");
        foreach (string name in nameValues) Console.WriteLine(name);

        Console.WriteLine("\r\nfid:");
        var fidValues = bad.GetColumnValues("fid");
        foreach (string fid in fidValues) Console.WriteLine(fid);
    }
}
