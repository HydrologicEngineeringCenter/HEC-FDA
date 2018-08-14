using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaModel.Utilities
{
    public sealed class Initialize
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        #endregion
        #region Voids
        #endregion
        #region Functions
        public static void InitializeGDAL()
        {
            try
            {
                Environment.SetEnvironmentVariable("GDAL_TIFF_OVR_BLOCKSIZE", "256");
                string dir = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                dir = new Uri(dir).LocalPath;
                dir = System.IO.Path.GetDirectoryName(dir);
                string ToolDir = dir + "\\GDAL\\bin";
                string DataDir = dir + "\\GDAL\\data";
                string PluginDir = dir + "\\GDAL\\bin\\gdalplugins";
                string WMSDir = dir + "\\GDAL\\Web Map Services";
                GDALAssist.GDALSetup.Initialize(ToolDir, DataDir, PluginDir, WMSDir);
            }
            catch(Exception ex)
            {
                Messager.Logger.Instance.ReportMessage(new Messager.ErrorMessage(ex.InnerException.ToString() + "\n Failed to initialize GDAL, check if the GDAL directory is next to the FdaModel.dll", Messager.ErrorMessageEnum.Fatal | Messager.ErrorMessageEnum.Model));
            }
        }
        public static void DisposeGDAL()
        {
            GDALAssist.GDALSetup.Dispose();
        }
        #endregion
    }
}
