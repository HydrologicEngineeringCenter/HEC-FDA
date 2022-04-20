using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Storage;
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
        private int _ID;
        private List<string> _OriginalFolderNames = new List<string>();
        private string _OriginalFolderName;
        private string _SelectedPath;

        #endregion
        #region Properties
        public string SelectedPath
        {
            get { return _SelectedPath; }
            set { _SelectedPath = value; NotifyPropertyChanged(); }
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
        }
        /// <summary>
        /// Constructor used when editing an existing child node.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="actionManager"></param>
        public WaterSurfaceElevationImporterVM(WaterSurfaceElevationElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
            SelectedPath = Connection.Instance.HydraulicsDirectory + "\\" + elem.Name;
            _ID = elem.ID;
            Name = elem.Name;
            _OriginalFolderName = elem.Name;
            Description = elem.Description;
            IsDepthGridChecked = elem.IsDepthGrids;
            foreach(PathAndProbability pp in elem.RelativePathAndProbability)
            {
                string path = Connection.Instance.HydraulicsDirectory + "\\" + pp.Path;
                string folderName = Path.GetFileName(pp.Path);
                _OriginalFolderNames.Add(folderName);
                AddRow(folderName, path, pp.Probability, false);
            }
        }
        #endregion
        #region Voids
        public void AddRow( string name, string path, double probability, bool isEnabled = true)
        {
            WaterSurfaceElevationRowItemVM newRow= new WaterSurfaceElevationRowItemVM( name, path, probability, isEnabled);
            ListOfRows.Add(newRow);
            //NotifyPropertyChanged("ListOfRows");
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

        private void CopyWaterSurfaceFilesToStudyDirectory(string path, string nameWithExtension)
        {
            string destinationFilePath = Connection.Instance.HydraulicsDirectory + "\\"+ Name + "\\" + nameWithExtension;
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

            string dirName = Path.GetFileName(directoryPath);

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
            if (IsCreatingNewElement)
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
                foreach (string directory in directories)
                {
                    FdaValidationResult result = ContainsVRTAndTIF(directory);
                    if (result.IsValid)
                    {
                        validDirectories.Add(directory);
                    }
                    else
                    {
                        importResult.AddErrorMessage(result.ErrorMessage);
                    }
                }

                string errorMsg = " The selected directory must have 8 subdirectories that each contain one .vrt file and at least one .tif file.\n";

                //we require 8 valid directories
                if (validDirectories.Count < 8)
                {
                    string dirName = Path.GetFileName(fullpath);
                    importResult.InsertMessage(0, "Directory '" + dirName + "' did not contain 8 valid subdirectories." + errorMsg);
                    MessageBox.Show(importResult.ErrorMessage, "Invalid Directory Structure", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (validDirectories.Count > 8)
                {
                    string dirName = Path.GetFileName(fullpath);
                    importResult.InsertMessage(0, "Directory '" + dirName + "' contains more than 8 valid subdirectories." + errorMsg);
                    MessageBox.Show(importResult.ErrorMessage, "Invalid Directory Structure", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    double prob = 0;
                    foreach (string dir in validDirectories)
                    {
                        prob += .1;
                        AddRow(Path.GetFileName(dir), Path.GetFullPath(dir), prob);
                    }
                    //we might have some message for the user?
                    if (!importResult.IsValid)
                    {
                        importResult.InsertMessage(0, "The selected directory contains 8 valid subdirectories and will ignore the following:\n");
                        MessageBox.Show(importResult.ErrorMessage, "Valid Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        public override void Save()
        {
            if(IsCreatingNewElement)
            {
                SaveNew();            
            }
            else
            {
                SaveExisting();
            }
        }

        private void SaveExisting()
        {
            InTheProcessOfSaving = true;
            //the user can not change files when editing, so the only changes would be new names and probs.    
            //if name is different then we need to update the directory name in the study hydraulics folder.
            if (!Name.Equals(_OriginalFolderName))
            {
                string sourceFilePath = Connection.Instance.HydraulicsDirectory + "\\" + _OriginalFolderName;
                string destinationFilePath = Connection.Instance.HydraulicsDirectory + "\\" + Name;
                Directory.Move(sourceFilePath, destinationFilePath);
            }
            //might have to rename the sub folders.
            List<PathAndProbability> newPathProbs = new List<PathAndProbability>();
            for (int i = 0; i < ListOfRows.Count; i++)
            {
                string newName = ListOfRows[i].Name;
                string originalName = _OriginalFolderNames[i];
                if (!newName.Equals(originalName))
                {
                    string sourceFilePath = Connection.Instance.HydraulicsDirectory + "\\" + Name + "\\" + originalName;
                    string destinationFilePath = Connection.Instance.HydraulicsDirectory + "\\" + Name + "\\" + newName;
                    Directory.Move(sourceFilePath, destinationFilePath);
                    _OriginalFolderNames[i] = newName;
                }
                newPathProbs.Add(new PathAndProbability(Name + "\\" + newName, ListOfRows[i].Probability));
            }

            WaterSurfaceElevationElement elementToSave = new WaterSurfaceElevationElement(Name, Description, newPathProbs, IsDepthGridChecked, _ID);
            Saving.PersistenceManagers.WaterSurfaceAreaPersistenceManager manager = Saving.PersistenceFactory.GetWaterSurfaceManager();
            manager.SaveExisting(elementToSave, _OriginalFolderName);
            SavingText = "Last Saved: " + elementToSave.LastEditDate;
            HasChanges = false;
            HasSaved = true;
            _OriginalFolderName = Name;
        }

        private void SaveNew()
        {
            FdaValidationResult validResult = ValidateImporter();
            if (validResult.IsValid)
            {
                List<PathAndProbability> pathProbs = new List<PathAndProbability>();
                foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
                {
                    _OriginalFolderNames.Add(row.Name);
                    string directoryName = Path.GetFileName(row.Name);
                    pathProbs.Add(new PathAndProbability(Name + "\\" + directoryName, row.Probability));

                    CopyWaterSurfaceFilesToStudyDirectory(row.Path, row.Name);
                }

                int id = GetElementID(Saving.PersistenceFactory.GetWaterSurfaceManager());
                WaterSurfaceElevationElement elementToSave = new WaterSurfaceElevationElement(Name, Description, pathProbs, IsDepthGridChecked, id);
                base.Save(elementToSave);
                _OriginalFolderName = Name;
                _ID = id;
            }
            else
            {
                MessageBox.Show(validResult.ErrorMessage, "Invalid Values", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
