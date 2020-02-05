using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using DbMemory;

namespace FdaCombinedMain
{
    public class ArgumentProcess
    {
        string _TheWorkingDirectory = string.Empty;

        public string _TheFileNameDirectory = string.Empty;
        public string _TheFileNameOut = string.Empty;

        public ArgumentProcess()
        {
            ;
        }
        //--------------------------------------------------------------------------------
        public string ProcessArgument(string arg, ref string theFileNameDirectory, ref string theFileNameout)
        {
            _TheFileNameOut = string.Empty;

            string fileName = string.Empty;
            string theFullFileName = string.Empty;

            fileName = (string)arg.Clone();
            theFullFileName = FormSubdirectoryNames(fileName, ref theFileNameDirectory, ref theFileNameout);

            return theFullFileName;
        }
        //--------------------------------------------------------------------------------
        public string FormSubdirectoryNames(string theFileName, ref string theFileNameDirectory, ref string theFileNameOut)
        {
            _TheWorkingDirectory = string.Empty;
            theFileNameOut = string.Empty;

            // Base filename only; assume working directory is file subdirectory
            if (theFileName[1] != ':')
            {
                if (theFileNameDirectory.Length < 1)
                {
                    _TheWorkingDirectory = Directory.GetCurrentDirectory();
                    int thelen = _TheWorkingDirectory.Length;
                    theFileNameDirectory = (string)_TheWorkingDirectory.Clone();
                    theFileNameOut = theFileNameDirectory + theFileName;
                }
                else
                {
                    int thelen = theFileNameDirectory.Length;
                    //if (theFileNameDirectory[thelen - 1] != '\\') theFileNameDirectory += "\\";
                    theFileNameOut = theFileNameDirectory + "\\" + theFileName;
                }
            }
            // Fully qualified theFileName - the filename 
            else
            {
                theFileNameOut = (string)theFileName.Clone();

                // strip off the filename to get filename directory
                char[] theFileNameChar = theFileName.Trim().ToCharArray();
                int ilast = theFileName.Trim().Length;
                int isl = 0;
                for(int i = ilast-1; isl < 1 && i > -1; i--)
                {
                    if(theFileNameChar[i] == '\\') isl++;
                    theFileNameChar[i] = ' ';
                    if (isl > 0) break;
                }
                theFileNameDirectory = new string(theFileNameChar).Trim();
            }
            return theFileNameOut;
        }
        //--------------------------------------------------------------------------------
        public string FormFullyQualifiedFileName(string baseName)
        {
            /*
             * If only the base filename is supplied, create a fully qualified filename
             * using the current default directory
            */
            string fullyQualifiedFileName = "";

            //Name is already fully qualified - could test for it
            if (baseName[1] == ':')
            {
                fullyQualifiedFileName = (string)baseName.Clone();
            }
            //Form fully qualified filename 
            else
            {
                fullyQualifiedFileName = Directory.GetCurrentDirectory();
                int theLen = fullyQualifiedFileName.Length;

                if (fullyQualifiedFileName[theLen - 1] != '\\') fullyQualifiedFileName += "\\";
                fullyQualifiedFileName += baseName;
            }
            return fullyQualifiedFileName;
        }

    }
}
