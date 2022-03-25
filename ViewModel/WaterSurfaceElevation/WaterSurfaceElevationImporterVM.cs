using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;
using System.IO;
using System.Windows;

namespace HEC.FDA.ViewModel.WaterSurfaceElevation
{
    //[Author(q0heccdm, 9 / 1 / 2017 8:31:13 AM)]
    public class WaterSurfaceElevationImporterVM:BaseEditorVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 9/1/2017 8:31:13 AM
        #endregion
        #region Fields
        private bool _IsDepthGridChecked;
        private bool _IsEditor;
        #endregion
        #region Properties
        public bool IsEditor
        {
            get { return _IsEditor; }
            set { _IsEditor = value; NotifyPropertyChanged(); }
        }
        public bool IsUsingTifFiles { get; set; }// it will either be all tif's or all vrt's. if there are flt's then i will convert them to tif's
        public List<PathAndProbability> ListOfRelativePaths{ get; } = new List<PathAndProbability>();
        public List<string> ListOfOriginalPaths { get; set; } //this is only for messaging out in the transaction log
   
        public bool IsDepthGridChecked
        {
            get { return _IsDepthGridChecked; }
            set { _IsDepthGridChecked = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<WaterSurfaceElevationRowItemVM> ListOfRows { get; } = new ObservableCollection<WaterSurfaceElevationRowItemVM>(); 

        #endregion
        #region Constructors
        public WaterSurfaceElevationImporterVM(EditorActionManager actionManager):base(actionManager)
        {
            IsEditor = false;
        }
        /// <summary>
        /// Constructor used when editing an existing child node.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="actionManager"></param>
        public WaterSurfaceElevationImporterVM(WaterSurfaceElevationElement elem, EditorActionManager actionManager) : base(actionManager)
        {
            IsEditor = true;
            Name = elem.Name;
            Description = elem.Description;
            ListOfRelativePaths = elem.RelativePathAndProbability;
            IsDepthGridChecked = elem.IsDepthGrids;
            foreach(PathAndProbability pp in ListOfRelativePaths)
            {
                string filename = Path.GetFileName(pp.Path);
                AddRow(true, filename, pp.Path, pp.Probability, false);
            }
        }

        #endregion
        #region Voids
        public void AddRow(bool isChecked, string name, string path, double probability, bool isEnabled = true)
        {
            WaterSurfaceElevationRowItemVM newRow= new WaterSurfaceElevationRowItemVM(isChecked, name, path, probability, isEnabled);
            ListOfRows.Add(newRow);
            NotifyPropertyChanged("ListOfRows");
        }

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != null, "Name cannot be null.");
            AddRule(nameof(Name), () => Name != "", "Name cannot be null.");

            AddRule(nameof(ListOfRows), () =>
             {
                 int numberOfSelectedRows = 0;
                 foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
                 {
                     if (row.IsChecked == true)
                     {
                         numberOfSelectedRows++;
                     }
                 }

                 if (numberOfSelectedRows < 8)
                 {
                     return false;
                 }
                 else
                 {
                     return true;
                 }

             }, "You have fewer than 8 files selected. You will get better results if you select more files.", false);
            AddRule(nameof(ListOfRows), () =>
            {
                List<double> probabilitiesList = new List<double>();
                foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
                {
                    if (row.IsChecked == true)
                    {
                        if (probabilitiesList.Contains(row.Probability))
                        {
                            //error
                            return false;
                        }
                        else
                        {
                            probabilitiesList.Add(row.Probability);
                        }
                    }
                }

                return true;

            }, "Duplicate probabilities are not allowed.", true);
        }


        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        private void CopyWaterSurfaceFilesToStudyDirectory(string path, string nameWithExtension,double probability)
        {
            string destinationFilePath = Storage.Connection.Instance.HydraulicsDirectory + "\\"+ Name + "\\" + nameWithExtension;
            Copy(path, destinationFilePath);
        }

       

        private FdaValidationResult ContainsVRTAndTIF(string directoryPath)
        {
            FdaValidationResult vr = new FdaValidationResult();

            List<string> tifFiles = new List<string>();
            List<string> vrtFiles = new List<string>();

            string[] fileList = Directory.GetFiles(directoryPath);
            foreach (string file in fileList)
            {
                if (Path.GetExtension(file) == ".tif") 
                { 
                    tifFiles.Add(file); 
                }
                if (Path.GetExtension(file) == ".vrt") 
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

        public void FileSelected(string fullpath)
        {
            FdaValidationResult importResult = new FdaValidationResult();
            ListOfRows.Clear();
            //clear out any already existing rows
            if (!Directory.Exists(fullpath))
            {
                return;
            }

            List<string> validDirectories = new List<string>();
            string[] directories = Directory.GetDirectories(fullpath);
            foreach(string directory in directories)
            {
                FdaValidationResult result = ContainsVRTAndTIF(directory);
                if(result.IsValid)
                {
                    validDirectories.Add(directory);
                }
                else
                {
                    importResult.AddErrorMessage(result.ErrorMessage);
                }
            }

            //we require 8 valid directories
            if (validDirectories.Count != 8)
            {
                string dirName = Path.GetFileNameWithoutExtension(fullpath);
                importResult.InsertMessage(0, "Directory '" + dirName + "' did not contain 8 valid subdirectories." +
                    " The selected directory must have 8 subdirectories that each contain one .vrt file and at least one .tif file.\n");
                MessageBox.Show(importResult.ErrorMessage, "Invalid Directory Structure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                double prob = 0;
                foreach (string dir in validDirectories)
                {
                    prob += .1;
                    AddRow(true, Path.GetFileName(dir), Path.GetFullPath(dir), prob);
                }
                //we might have some message for the user?
                if(!importResult.IsValid)
                {
                    importResult.InsertMessage(0, "The selected directory contains 8 valid subdirectories and will ignore the following:\n");
                    MessageBox.Show(importResult.ErrorMessage, "Valid Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private FdaValidationResult ValidateImporter()
        {
            FdaValidationResult vr = new FdaValidationResult();

            return vr;
        }

        public override void Save()
        {
            
            //validate?
            foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
            {
                string nameWithExtension = Path.GetFileName(row.Path);
                ListOfRelativePaths.Add(new PathAndProbability(Name + "\\" + nameWithExtension, row.Probability));
                CopyWaterSurfaceFilesToStudyDirectory(row.Path, row.Name, row.Probability);
            }

            int id = GetElementID(Saving.PersistenceFactory.GetWaterSurfaceManager());
            WaterSurfaceElevationElement elementToSave = new WaterSurfaceElevationElement(Name, Description, ListOfRelativePaths, IsDepthGridChecked, id);
            Saving.PersistenceManagers.WaterSurfaceAreaPersistenceManager manager = Saving.PersistenceFactory.GetWaterSurfaceManager();
            base.Save(elementToSave);
        }
        #endregion

    }
}
