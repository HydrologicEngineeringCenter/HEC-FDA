using HEC.FDA.Model.structures;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Utilities;

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
            FdaValidationResult vr = new();
            if (TerrainPath.IsNullOrEmpty())
            {
                vr.AddErrorMessage("A terrain file is required.");
            }
            else
            {
                string pathExtension = Path.GetExtension(TerrainPath);
                switch (pathExtension)
                {
                    case HDF:
                        List<string> terrainCompFiles = RASHelper.GetTerrainComponentFiles(TerrainPath, null);
                        if (!FilesExist(terrainCompFiles))
                        {
                            vr.AddErrorMessage("The file selected is missing it's component files.");
                        }
                        break;
                    case TIF:
                        // do nothing
                        break;
                    default:
                        vr.AddErrorMessage("The file selected has an extension type of: '" + pathExtension + "'. Only .tif, and .hdf are supported.");
                        break;
                }
            }
            return vr;
        }

        private static bool FilesExist(IEnumerable<string> files)
        {
              foreach(string file in files)
            {
                if (!File.Exists(file))
                {
                    return false;
                }
            }
            return true;
        }

        public override void Save()
        {
            FdaValidationResult isValidResult = IsValid();
            if (isValidResult.IsValid)
            {
                TerrainElementPersistenceManager manager = PersistenceFactory.GetTerrainManager();

                int id = PersistenceFactory.GetTerrainManager().GetNextAvailableId();
                //add a dummy element to the parent
                string fileName = Path.GetFileName(TerrainPath);
                TerrainElement t = new(Name, fileName, id, true);
                StudyCache.GetParentElementOfType<TerrainOwnerElement>().AddElement(t);
                TerrainElement newElement = new(Name, fileName, id)
                {
                    LastEditDate = DateTime.Now.ToString("G")
                };
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
