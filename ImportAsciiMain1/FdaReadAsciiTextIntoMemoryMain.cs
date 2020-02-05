using System;
using System.Linq;
using static System.Console;

using DbMemory;

namespace FdaCombinedMain
{
    class FdaReadAsciiTextIntoMemoryMain
    {
        static void Main(string[] args)
        {
            bool isExport = true;
            string fileNameDirectory = string.Empty;
            string fileNameImportFull = string.Empty;
            string fileNameExportFull = string.Empty;
            string exportFileNameBase = "Fda_ExportedData.txt";

            string DateOfProgram = "February 04, 2020";
            WriteLine($"Convert / Import / Export For HEC-FDA Version 2.0\nVersion dated {DateOfProgram}");

            ArgumentProcess procArg = new ArgumentProcess();

            /*
             * Arguments 
             *  1   Import Filename (can be fully qualified
            */
            //Process arguments
            //Open study
            // Import 

            int nuArgs = args.Count();
            int length = args.Length;


            //Stop Program, no arguments 
            if (nuArgs < 1)
            {
                WriteLine("\nProgram Stop");
                ReadKey();
                return;
            }
            // Argument supplied, should be only 1
            else if(nuArgs == 1)
            {
                fileNameImportFull = procArg.ProcessArgument(args[0], ref fileNameDirectory, ref fileNameImportFull);
                fileNameExportFull = procArg.ProcessArgument(exportFileNameBase, ref fileNameDirectory, ref fileNameExportFull);


                //Import Data
                AsciiImport import = new AsciiImport();
                import.ImportAsciiData(fileNameImportFull);
            }

            // Exporting for test
            if(isExport)
            {

                //Export what was imported
                AsciiExport export = new AsciiExport
                {
                    ExportPlans = true,
                    ExportYears = true,
                    ExportStreams = true,
                    ExportReaches = true,
                    ExportCategories = true,
                    ExportStructureModules = true,
                    ExportOccTypes = true,
                    ExportStructures = true,
                    ExportProbFuncs = true,
                    ExportRateFuncs = true,
                    ExportLevees = true,
                    ExportWsp = true,
                    ExportStageDamage = true
                };

                //export.exportAsciiData("c:\\hecfda\\_V200\\FdaMain\\Data\\theOutput.txt");
                export.ExportAsciiData(fileNameExportFull);

            }
        }

    }
}
