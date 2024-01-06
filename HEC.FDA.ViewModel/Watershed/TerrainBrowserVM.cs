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
                if (pathExtension.Equals(HDF) || pathExtension.Equals(TIF))
                {
                    string errorMessage = "";
                    if(!RASHelper.TerrainIsValid(TerrainPath, ref errorMessage))
                    {
                        vr.AddErrorMessage(errorMessage);
                    }
                }
                else
                {
                    vr.AddErrorMessage("The file selected has an extension type of: '" + pathExtension + "'. Only .tif, and .hdf are supported.");
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

        /// <summary>
        /// Checks to see if the study has a terrain and returns the full path to the terrain file.
        /// </summary>
        /// <returns>The full path to the terrain file if it's been imported, or an empty string if there is no terrain.</returns>
        public static string GetTerrainFile()
        {
            string filePath = "";
            List<TerrainElement> terrainElements = StudyCache.GetChildElementsOfType<TerrainElement>();
            if (terrainElements.Count > 0)
            {
                //there can only be one terrain in the study
                TerrainElement elem = terrainElements[0];
                filePath = Storage.Connection.Instance.TerrainDirectory + "\\" + elem.Name + "\\" + elem.FileName;
            }
            return filePath;
        }

        #endregion

    }
}
