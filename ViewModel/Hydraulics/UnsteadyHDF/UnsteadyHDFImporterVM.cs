using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace HEC.FDA.ViewModel.Hydraulics.UnsteadyHDF
{
    public class UnsteadyHDFImporterVM:BaseEditorVM
    {
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
            set { _SelectedPath = value; FileSelected(value); NotifyPropertyChanged(); }
        }

        public bool IsDepthGridChecked
        {
            get { return _IsDepthGridChecked; }
            set { _IsDepthGridChecked = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<WaterSurfaceElevationRowItemVM> ListOfRows { get; } = new ObservableCollection<WaterSurfaceElevationRowItemVM>();
        #endregion
        #region Constructors
        public UnsteadyHDFImporterVM(EditorActionManager actionManager) : base(actionManager)
        {
        }
        /// <summary>
        /// Constructor used when editing an existing child node.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="actionManager"></param>
        public UnsteadyHDFImporterVM(HydraulicElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
            SelectedPath = Connection.Instance.HydraulicsDirectory + "\\" + elem.Name;
            _ID = elem.ID;
            Name = elem.Name;
            _OriginalFolderName = elem.Name;
            Description = elem.Description;
            IsDepthGridChecked = elem.IsDepthGrids;
            foreach (PathAndProbability pp in elem.RelativePathAndProbability)
            {
                string path = Connection.Instance.HydraulicsDirectory + "\\" + pp.Path;
                string folderName = Path.GetFileName(pp.Path);
                _OriginalFolderNames.Add(folderName);
                AddRow(folderName, path, pp.Probability, false);
            }
        }
        #endregion
        #region Voids
        public void AddRow(string name, string path, double probability, bool isEnabled = true)
        {
            WaterSurfaceElevationRowItemVM newRow = new WaterSurfaceElevationRowItemVM(name, path, probability, isEnabled);
            ListOfRows.Add(newRow);
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
                string newPath = Path.Combine(target.FullName, fi.Name);
                fi.CopyTo(newPath, true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        #endregion

        #region validation
        private FdaValidationResult ValidateImporter()
        {
            FdaValidationResult vr = new FdaValidationResult();

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

        #endregion

        private FdaValidationResult IsFileValid(string file)
        {
            FdaValidationResult vr = new FdaValidationResult();
            int firstPeriodIndex = file.IndexOf(".");
            if(firstPeriodIndex != -1)
            {
                string substring = file.Substring(firstPeriodIndex + 1);
                Regex r = new Regex("p??.hdf");
                if(!r.Match(substring).Success)
                {
                    //failed
                    vr.AddErrorMessage("Ignoring file that did not match the pattern of '*.p##.hdf'. " + file);
                }         
            }
            else
            {
                //wrong format no period found in file path
                vr.AddErrorMessage("Ignoring file that did not match the pattern of '*.p##.hdf'. " + file);
            }

            return vr;
        }

        public void FileSelected(string fullpath)
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (fullpath != null && IsCreatingNewElement)
            {
                ListOfRows.Clear();

                string[] files = Directory.GetFiles(fullpath);
                List<string> validFiles = new List<string>();
                foreach(string file in files)
                {
                    FdaValidationResult fileValidResult = IsFileValid(file);
                    if(fileValidResult.IsValid)
                    {
                        validFiles.Add(file);
                    }
                    else
                    {
                        vr.AddErrorMessage(fileValidResult.ErrorMessage);
                    }
                }

                //warn users that these directories were ignored.
                string[] directories = Directory.GetDirectories(fullpath);
                if(directories.Length > 0)
                {
                    vr.AddErrorMessage("Ignoring subdirectories.");
                }

                if (validFiles.Count == 0)
                {
                    vr.AddErrorMessage("No valid hdf files were detected. Files must match pattern '*.p##.hdf'.");
                }     
                else
                {
                    double prob = 0;
                    foreach (string file in validFiles)
                    {
                        prob += .1;
                        AddRow(Path.GetFileName(file), Path.GetFullPath(file), prob);
                    }
                    if (!vr.IsValid)
                    {
                        vr.InsertMessage(0, "Some files or subdirectories are being ignored:\n");
                        MessageBox.Show(vr.ErrorMessage, "Invalid Files", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        public override void Save()
        {
            if (IsCreatingNewElement)
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
            //the user can not change files when editing, so the only changes would be new names and probs.    
            //if name is different then we need to update the directory name in the study hydraulics folder.
            FdaValidationResult validResult = ValidateImporter();
            if (validResult.IsValid)
            {
                InTheProcessOfSaving = true;
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
                    newPathProbs.Add(new PathAndProbability(newName, ListOfRows[i].Probability));
                }

                HydraulicElement elementToSave = new HydraulicElement(Name, Description, newPathProbs, IsDepthGridChecked, HydraulicType.Unsteady, _ID);
                Saving.PersistenceManagers.HydraulicPersistenceManager manager = Saving.PersistenceFactory.GetWaterSurfaceManager();
                manager.SaveExisting(elementToSave);
                SavingText = "Last Saved: " + elementToSave.LastEditDate;
                HasChanges = false;
                HasSaved = true;
                _OriginalFolderName = Name;
            }
            else
            {
                MessageBox.Show(validResult.ErrorMessage, "Invalid Values", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveNew()
        {
            FdaValidationResult validResult = ValidateImporter();
            if (validResult.IsValid)
            {
                string destinationDirectory = Connection.Instance.HydraulicsDirectory + "\\" + Name;
                Directory.CreateDirectory(destinationDirectory);

                List<PathAndProbability> pathProbs = new List<PathAndProbability>();
                foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
                {
                    _OriginalFolderNames.Add(row.Name);
                    string directoryName = Path.GetFileName(row.Name);
                    pathProbs.Add(new PathAndProbability(directoryName, row.Probability));                    

                    File.Copy(row.Path, destinationDirectory + "\\" + row.Name);
                }

                int id = GetElementID<HydraulicElement>();
                HydraulicElement elementToSave = new HydraulicElement(Name, Description, pathProbs, IsDepthGridChecked, HydraulicType.Unsteady, id);
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
