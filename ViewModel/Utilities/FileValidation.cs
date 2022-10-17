using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Utilities
{
    internal static class FileValidation
    {

        /// <summary>
        /// Used to make sure that the required shapefiles are where they are expected to be.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static FdaValidationResult DirectoryHasOneFileMatchingPattern(string directoryPath, string pattern)
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath, pattern);
                if (files.Length == 0)
                {
                    vr.AddErrorMessage("The directory does not contain a file that matches the pattern: " + pattern);
                }
                else if (files.Length > 1)
                {
                    //more than one file discovered
                    vr.AddErrorMessage("The directory contains multiple files that matche the pattern: " + pattern);
                }
            }
            else
            {
                vr.AddErrorMessage("The directory does not exist: " + directoryPath);
            }
            return vr;
        }

    }
}
