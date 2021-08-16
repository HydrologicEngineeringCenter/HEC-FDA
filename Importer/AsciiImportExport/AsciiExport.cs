using System.IO;
using static System.Console;

namespace Importer
{
    public class AsciiExport : AsciiImportExport
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        #endregion
        #region Properties
        public bool ExportPlans
        { get; set; }
        public bool ExportYears
        { get; set; }
        public bool ExportStreams
        { get; set; }
        public bool ExportReaches
        { get; set; }
        public bool ExportCategories
        { get; set; }
        public bool ExportStructureModules
        { get; set; }
        public bool ExportStructureModuleAssignments
        { get; set; }
        public bool ExportOccTypes
        { get; set; }
        public bool ExportStructures
        { get; set; }
        public bool ExportWsp
        { get; set; }
        public bool ExportProbFuncs
        { get; set; }
        public bool ExportRateFuncs
        { get; set; }
        public bool ExportLevees
        { get; set; }
        public bool ExportStageDamage
        { get; set; }
        #endregion
        #region Constructors
        public AsciiExport()
        {
            ExportPlans = false;
            ExportYears = false;
            ExportStreams = false;
            ExportReaches = false;
            ExportCategories = false;
            ExportStructureModules = false;
            ExportStructureModuleAssignments = false;
            ExportOccTypes = false;
            ExportWsp = false;
            ExportProbFuncs = false;
            ExportRateFuncs = false;
            ExportLevees = false;
            ExportStageDamage = false;
        }
        #endregion
        #region Voids
        public void ExportAsciiData(string theExportFilename)
        {
            char delimiterChar = '\t';
            StreamWriter writer;

            //FileStream fileStreamExport = File.OpenWrite(theExportFilename);
            //using (var fileStreamExport = new FileStream(theExportFilename, FileMode.Truncate))
            using (var fileStreamExport = new FileStream(theExportFilename, FileMode.Create, FileAccess.Write))
            {
                using (writer = new StreamWriter(fileStreamExport))

                //using (StreamWriter writer = new StreamWriter(theExportFilename))
                {
                    writer.WriteLine($"Writing to the file: {theExportFilename}");

                    if (ExportPlans)
                    { GlobalVariables.mp_fdaStudy.GetPlanList().ExportPlans(writer, delimiterChar); }
                    if (ExportYears)
                    { GlobalVariables.mp_fdaStudy.GetYearList().Export(writer, delimiterChar); }
                    if (ExportStreams)
                    { GlobalVariables.mp_fdaStudy.GetStreamList().Export(writer, delimiterChar); }
                    if (ExportReaches)
                    { GlobalVariables.mp_fdaStudy.GetDamageReachList().Export(writer, delimiterChar); }
                    if (ExportCategories)
                    { GlobalVariables.mp_fdaStudy.GetDamageCategoryList().Export(writer, delimiterChar); }
                    if (ExportStructureModules)
                    { GlobalVariables.mp_fdaStudy.GetStructureModuleList().Export(writer, delimiterChar); }
                    WriteLine("\nExport the Occupancy types.");
                    if (ExportOccTypes)
                    { GlobalVariables.mp_fdaStudy.GetOccupancyTypeList().Export(writer, delimiterChar); }
                    WriteLine("\nExport the Structures.");
                    if (ExportStructures)
                    { GlobalVariables.mp_fdaStudy.GetStructureList().Export(writer, delimiterChar); }
                    if (ExportWsp)
                    { GlobalVariables.mp_fdaStudy.GetWspList().Export(writer, delimiterChar); }
                    if (ExportProbFuncs)
                    { GlobalVariables.mp_fdaStudy.GetProbabilityFuncList().Export(writer, delimiterChar); }
                    if (ExportRateFuncs)
                    { GlobalVariables.mp_fdaStudy.GetRatingFunctionList().Export(writer, delimiterChar); }
                    if (ExportLevees)
                    { GlobalVariables.mp_fdaStudy.GetLeveeList().Export(writer, delimiterChar); }

                    writer.WriteLine($"\n\nBegin to Export Aggregatted Damage Functions.");
                    WriteLine($"\n\nBegin to Export Aggregatted Damage Functions.");
                    if (ExportStageDamage)
                    { GlobalVariables.mp_fdaStudy.GetAggDamgFuncList().Export(writer, delimiterChar); }
                }
            }
            writer.Close();
            writer.Dispose();
        }
        #endregion
        #region Functions
        #endregion
    }
}
