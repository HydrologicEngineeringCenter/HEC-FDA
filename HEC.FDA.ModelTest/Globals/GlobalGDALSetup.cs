using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Geospatial.GDALAssist;
using HEC;
using HEC.FDA;
using HEC.FDA.ModelTest;
using HEC.FDA.ModelTest.Globals;

namespace HEC.FDA.ModelTest.Globals;
public class GlobalGDALSetup : IDisposable
{
    public GlobalGDALSetup()
    {
        InitializeGDAL();
    }

    private void InitializeGDAL()
    {
        using StreamWriter writer = new StreamWriter(@"C:\Temp\temp.txt");
        if (GDALSetup.IsInitialized)
        {
            writer.WriteLine("I'm already initialized.");
        }

        GDALSetup.InitializeMultiplatform(@"C:\Programs\7.x Development\GDAL");

        writer.Write("I'm Running from " + Assembly.GetExecutingAssembly().Location);
        writer.Write(GDALSetup.LoadedVersion + "\n");
        writer.Write(GDALSetup.ToolDirectory + "\n");
    }

    public void Dispose()
    {
        //I don't care. 
    }
}
