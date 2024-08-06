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
        GDALSetup.InitializeMultiplatform();
    }

    public void Dispose()
    {
        //I don't care. 
    }
}
