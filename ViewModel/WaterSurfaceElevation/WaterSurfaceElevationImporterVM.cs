using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
            IsDepthGridChecked = elem.IsDepthGrids;
            foreach(PathAndProbability pp in elem.RelativePathAndProbability)
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

        #region copy files

        private void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        private void CopyAll(DirectoryInfo source, DirectoryInfo target)
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
        #endregion

        #region validation
        private FdaValidationResult ValidateImporter()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (ListOfRows.Count != 8)
            {
                vr.AddErrorMessage("Eight hydraulic files are required to import.");
            }
            List<double> probs = new List<double>();
            foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
            {
                vr.AddErrorMessage(row.IsValid().ErrorMessage);
                probs.Add(row.Probability);
            }
            if (probs.Count != probs.Distinct().Count())
            {
                vr.AddErrorMessage("Duplicate probabilities found. Probabilities must be unique.");
            }
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

        #endregion

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
            if (validDirectories.Count < 8)
            {
                string dirName = Path.GetFileNameWithoutExtension(fullpath);
                importResult.InsertMessage(0, "Directory '" + dirName + "' did not contain 8 valid subdirectories." +
                    " The selected directory must have 8 subdirectories that each contain one .vrt file and at least one .tif file.\n");
                MessageBox.Show(importResult.ErrorMessage, "Invalid Directory Structure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if(validDirectories.Count>8)
            {
                string dirName = Path.GetFileNameWithoutExtension(fullpath);
                importResult.InsertMessage(0, "Directory '" + dirName + "' contains more than 8 valid subdirectories." +
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

        public override void Save()
        {
            FdaValidationResult validResult = ValidateImporter();
            if (validResult.IsValid)
            {
                List<PathAndProbability> pathProbs = new List<PathAndProbability>();
                foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
                {
                    string directoryName = Path.GetFileName(row.Name);
                    pathProbs.Add(new PathAndProbability(Name + "\\" + directoryName, row.Probability));

                    CopyWaterSurfaceFilesToStudyDirectory(row.Path, row.Name, row.Probability);
                }
                
                int id = GetElementID(Saving.PersistenceFactory.GetWaterSurfaceManager());
                WaterSurfaceElevationElement elementToSave = new WaterSurfaceElevationElement(Name, Description, pathProbs, IsDepthGridChecked, id);
                Saving.PersistenceManagers.WaterSurfaceAreaPersistenceManager manager = Saving.PersistenceFactory.GetWaterSurfaceManager();
                base.Save(elementToSave);
            }
            else
            {
                MessageBox.Show(validResult.ErrorMessage, "Invalid Values", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
