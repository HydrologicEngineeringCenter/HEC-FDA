using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace HEC.FDA.ViewModel.Watershed
{
    //[Author("q0heccdm", "10 / 11 / 2016 11:13:25 AM")]
    public class TerrainBrowserVM:BaseEditorVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/11/2016 11:13:25 AM
        #endregion
        #region Fields
        private const string VRT = ".vrt";
        private const string FLT = ".flt";
        private const string TIF = ".tif";
        private const string HDF = ".hdf";

        private string _TerrainPath;
        #endregion
        #region Properties

        /// <summary>
        /// This is the new location for the terrain file. We are copying the original file and placing it within the study directory.
        /// </summary>
        public string TerrainPath
        {
            get { return _TerrainPath; }
            set { _TerrainPath = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public TerrainBrowserVM( EditorActionManager actionManager) : base(actionManager)
        {
        }

        public override FdaValidationResult IsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (TerrainPath != null && TerrainPath != "")
            {
                //check extension
                string pathExtension = Path.GetExtension(TerrainPath);
                if (VRT.Equals(pathExtension, StringComparison.OrdinalIgnoreCase))
                {
                    //if we have a vrt then we need to check that we only have one and that there are tifs next to it. 
                    FdaValidationResult vrtResult = IsVRTPathValid();
                    vr.AddErrorMessage(vrtResult.ErrorMessage);
                }
                else if (FLT.Equals(pathExtension, StringComparison.OrdinalIgnoreCase) || TIF.Equals(pathExtension, StringComparison.OrdinalIgnoreCase) || HDF.Equals(pathExtension, StringComparison.OrdinalIgnoreCase))
                {
                    //No special validation required.
                }
                else
                {
                    vr.AddErrorMessage("The file selected has an extension type of: '" + pathExtension + "'. Only .vrt, .tif, .hdf, and .flt are supported.");
                }
            }
            else
            {
                vr.AddErrorMessage("A terrain file is required.");
            }
            return vr;
        }

        private FdaValidationResult IsVRTPathValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            string dirName = Path.GetDirectoryName(TerrainPath);
            vr.AddErrorMessage(ContainsVRTAndTIF(dirName).ErrorMessage);
            return vr;
        }

        private FdaValidationResult ContainsVRTAndTIF(string directoryPath)
        {
            FdaValidationResult vr = new FdaValidationResult();

            List<string> tifFiles = new List<string>();
            List<string> vrtFiles = new List<string>();

            string[] fileList = Directory.GetFiles(directoryPath);
            foreach (string file in fileList)
            {
                if (Path.GetExtension(file) == TIF)
                {
                    tifFiles.Add(file);
                }
                if (Path.GetExtension(file) == VRT)
                {
                    vrtFiles.Add(file);
                }
            }

            string dirName = Path.GetFileNameWithoutExtension(directoryPath);

            vr.AddErrorMessage(ValidateVRTFile(vrtFiles, dirName).ErrorMessage);
            vr.AddErrorMessage(ValidateTIFFiles(tifFiles, dirName).ErrorMessage);

            return vr;
        }
        private FdaValidationResult ValidateTIFFiles(List<string> tifFiles, string directoryName)
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (tifFiles.Count == 0)
            {
                vr.AddErrorMessage("Directory " + directoryName + ": No .tif files found.");
            }
            return vr;
        }

        private FdaValidationResult ValidateVRTFile(List<string> vrtFiles, string directoryName)
        {
            FdaValidationResult vr = new FdaValidationResult();

            if (vrtFiles.Count == 0)
            {
                vr.AddErrorMessage("Directory " + directoryName + ": No .vrt file found.");
            }
            else if (vrtFiles.Count > 1)
            {
                vr.AddErrorMessage("Directory " + directoryName + ": More than one .vrt file found.");
            }
            return vr;
        }

        public override void Save()
        {
            FdaValidationResult isValidResult = IsValid();
            if (isValidResult.IsValid)
            {
                TerrainElementPersistenceManager manager = PersistenceFactory.GetTerrainManager();

                int id = Saving.PersistenceFactory.GetTerrainManager().GetNextAvailableId();
                //add a dummy element to the parent
                string fileName = Path.GetFileName(TerrainPath);
                TerrainElement t = new TerrainElement(Name, fileName, id, true);
                StudyCache.GetParentElementOfType<TerrainOwnerElement>().AddElement(t);
                TerrainElement newElement = new TerrainElement(Name, fileName, id);
                newElement.LastEditDate = DateTime.Now.ToString("G");
                manager.SaveNew(TerrainPath, newElement);
                IsCreatingNewElement = false;
                HasChanges = false;
                HasSaved = true;
                OriginalElement = newElement;
            }
            else
            {
                MessageBox.Show(isValidResult.ErrorMessage, "Invalid File Path", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

    }
}
