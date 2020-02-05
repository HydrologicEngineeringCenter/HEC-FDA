using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DbMemory
{
    [Serializable]
    public class FileNameManager
    {
        //FileNameManager.cs
        //enum StandardOrder {FD_ID, FD_ALPHA_NAME};

        //==============================================================================
        //  Base class for any file. Handles the subdirectory and file names.
        //==============================================================================

        //Filename related
        //enum SIZE_OF_FILENAME { FILENAME_SIZE_MGR = 512 };

        public string mp_fileNameShort;
        public string mp_fileNameFull;
        public string mp_directoryName;
        public string mp_directoryNameCurrent;
        public string mp_fileNameAlias; //TODO;31MAY07;rdc critical;

        //short sty filename with .sty removed
        public string mp_fileNameBase;

        //------------------------------------------------------------------------------
        //Methods

        public FileNameManager()
        {
            constructInitialize();
        }
        public FileNameManager(ref FileNameManager theMgr)
        {
            constructInitialize();

            mp_fileNameShort = theMgr.mp_fileNameShort;
            mp_fileNameFull = theMgr.mp_fileNameFull;
            mp_directoryName = theMgr.mp_directoryName;
            mp_fileNameAlias = theMgr.mp_fileNameAlias;
            mp_fileNameBase = theMgr.mp_fileNameBase;
            mp_directoryNameCurrent = theMgr.mp_directoryNameCurrent;
        }
        public void constructInitialize()
        {
            mp_fileNameShort = string.Empty;
            mp_fileNameFull = string.Empty;
            mp_directoryName = string.Empty;
            mp_fileNameAlias = string.Empty;
            mp_fileNameBase = string.Empty;
            mp_directoryNameCurrent = Directory.GetCurrentDirectory();
        }
        public string SetCurrentDirectory()
        {
            mp_directoryNameCurrent = Directory.GetCurrentDirectory();
            return mp_directoryNameCurrent;
        }
        public string GetCurrentDirectory()
        {
            return mp_directoryNameCurrent;
        }
        public string setFileShortName(string name)
        {
            mp_fileNameShort = name.Trim();
            return mp_fileNameShort;
        }
        public string getFileShortName() {return mp_fileNameShort;        }
        public string getFileBaseName() { return mp_fileNameBase; }
        public string setPathname(string pathname)
        {
            mp_directoryName = string.Empty;
            mp_directoryName = pathname.Trim();
            return mp_directoryName;
        }
        public string getPathname() { return mp_directoryName; }
        public string setDirectoryname(string dn) { return setPathname(dn); }
        public string getDirectoryname() { return getPathname(); }
        public string formFileNameFull()
        {
            int nf = 0;
            int ns = mp_fileNameShort.Length;
            int np = mp_directoryName.Length;
            mp_fileNameFull = string.Empty;

            if (np > 0)
            {
                mp_fileNameFull = mp_directoryName;
                nf = mp_fileNameFull.Length;
                char theLastChar = mp_fileNameFull[nf - 1];
                if (theLastChar != Study.PATH_SEPARATOR)
                    mp_fileNameFull += Study.PATH_SEPARATOR;
            }
            else
            {
                mp_fileNameFull = string.Empty;
            }
            mp_fileNameFull += mp_fileNameShort.Trim();

            return mp_fileNameFull;
        }
        public string formFileNameFull(string pathname)
        {
            setPathname(pathname);
            return formFileNameFull();
        }

        public string formFileNameFull(string shortName, string pathname)
        {
            setFileShortName(shortName);
            setPathname(pathname);
            return formFileNameFull();
        }
        public string getFileNameFull()
        {
            return mp_fileNameFull;
        }
        public string setFileNameFull(string fullFn)
        {
            mp_fileNameFull = fullFn.Trim();
            return mp_fileNameFull;
        }
        public string setPartsAndFull(string fullFn)
        {
            setFileNameFull(fullFn);
            mp_directoryName = Path.GetDirectoryName(fullFn);
            mp_fileNameBase = Path.GetFileNameWithoutExtension(fullFn);
            return mp_fileNameFull;
        }

        public string setFilenames(string  directory, string shortName)
        {
            setDirectoryname(directory);
            setFileShortName(shortName);
            formFileNameFull();
            return mp_fileNameFull;
        }


        public string parseStudyFilename()
        {
            /*
              Picks off the base part of the sty filename from the fully qualified 
                filename. (Both the directory and the .sty are pulled off and the 
                remainder is returned).
              mp_fileNameFull must have been previously defined.
            */

            string theSubDirectoryNameCurrent = Directory.GetCurrentDirectory();
            string theSubDirectoryNameRoot = Path.GetPathRoot(mp_fileNameFull);
            mp_directoryName = Path.GetDirectoryName(mp_fileNameFull);
            mp_fileNameBase = Path.GetFileNameWithoutExtension(mp_fileNameFull).ToUpper();
            return mp_fileNameBase;
        }
        public string SetTheDbfFile(string theDbfFileName)
        {
            /*
             * theDbfFileName can be the fullly qualified name or the unqualified
             *  filename. The filename must include the extension.
             * check the second charactor for : to see if full path is supplied.;
             * check first charecter to see if \\ is supplied;
             * Get directory name and check for blank.
            */
            char[] chkColon = theDbfFileName.ToCharArray();
            if (chkColon[1] == ':')
            {
                //assume full pathname supplied
            }
            else if (chkColon[0] == '\\')
            {
                //assume full pathname supplied relative to root
            }
            else
            {
                theDbfFileName = Directory.GetCurrentDirectory() + "\\" + theDbfFileName;
            }


            string theSubDirectoryNameCurrent = Directory.GetCurrentDirectory();
            string theSubDirectoryNameRoot = Path.GetPathRoot(theDbfFileName);
            string theSubDirectoryName = Path.GetDirectoryName(theDbfFileName);
            mp_fileNameBase = Path.GetFileNameWithoutExtension(theDbfFileName).ToUpper();

            mp_fileNameFull = theDbfFileName;
            return mp_fileNameFull;
        }

    }
}
