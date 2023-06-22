using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace HEC.FDA.ViewModel.Hydraulics.UnsteadyHDF
{
    public class UnsteadyHDFImporterVM:BaseEditorVM
    {
        #region Fields
        private List<string> _OriginalFileNames = new List<string>();
        private string _SelectedPath;
        #endregion
        #region Properties
        public string SelectedPath
        {
            get { return _SelectedPath; }
            set { _SelectedPath = value; FileSelected(value); NotifyPropertyChanged(); }
        }

        public ObservableCollection<WaterSurfaceElevationRowItemVM> ListOfRows { get; } = new ObservableCollection<WaterSurfaceElevationRowItemVM>();
        #endregion
        #region Constructors
        public UnsteadyHDFImporterVM(EditorActionManager actionManager) : base(actionManager)
        {
            AddRule(nameof(ListOfRows), () => ListOfRows.Count > 0, "Invalid directory selected.");
        }
        /// <summary>
        /// Constructor used when editing an existing child node.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="actionManager"></param>
        public UnsteadyHDFImporterVM(HydraulicElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
            SelectedPath = Connection.Instance.HydraulicsDirectory + "\\" + elem.Name;
            foreach (HydraulicProfile pp in elem.DataSet.HydraulicProfiles)
            {
                string path = Connection.Instance.HydraulicsDirectory + "\\" + pp.FileName;
                string folderName = Path.GetFileName(pp.FileName);
                _OriginalFileNames.Add(folderName);
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
            FdaValidationResult vrErrors = new FdaValidationResult();
            FdaValidationResult vrWarnings = new FdaValidationResult();

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
                        vrWarnings.AddErrorMessage(fileValidResult.ErrorMessage);
                    }
                }

                //warn users that these directories were ignored.
                string[] directories = Directory.GetDirectories(fullpath);
                if(directories.Length > 0)
                {
                    vrWarnings.AddErrorMessage("Ignoring subdirectories.");
                }

                if (validFiles.Count == 0)
                {
                    vrErrors.AddErrorMessage("No valid hdf files were detected. You must select a directory that contains files that match pattern '*.p##.hdf'.");
                }     
                else
                {
                    foreach (string file in validFiles)
                    {
                        AddRow(getUnsteadyRASResultName(file), Path.GetFullPath(file), 0);
                    }
                }
                if(!vrWarnings.IsValid)
                {
                    MessageBox.Show(vrWarnings.ErrorMessage, "Warnings", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (!vrErrors.IsValid)
                {
                    MessageBox.Show(vrErrors.ErrorMessage, "Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string getUnsteadyRASResultName(string file)
        {
            RasMapperLib.RASResults result = new RasMapperLib.RASResults(file);
            if(result == null)
            {
                return "INVALID";
            }
            return result.PlanAttributes.PlanTitle;
        }

        public override void Save()
        {
            FdaValidationResult validResult = ValidateImporter();
            if (validResult.IsValid)
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
            else
            {
                MessageBox.Show(validResult.ErrorMessage, "Invalid Values", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RenameDirectoryInStudy()
        {
            if (!Name.Equals(OriginalElement.Name))
            {
                string sourceFilePath = Connection.Instance.HydraulicsDirectory + "\\" + OriginalElement.Name;
                string destinationFilePath = Connection.Instance.HydraulicsDirectory + "\\" + Name;
                Directory.Move(sourceFilePath, destinationFilePath);
            }
        }

        private void SaveExisting()
        {
            //the user can not change files when editing, so the only changes would be new names and probs.    
            //if name is different then we need to update the directory name in the study hydraulics folder.

            RenameDirectoryInStudy();
            //might have to rename the sub folders.
            List<HydraulicProfile> newPathProbs = new List<HydraulicProfile>();
            for (int i = 0; i < ListOfRows.Count; i++)
            {
                string newName = ListOfRows[i].Name;
                string originalName = _OriginalFileNames[i];
                if (!newName.Equals(originalName))
                {
                    string sourceFilePath = Connection.Instance.HydraulicsDirectory + "\\" + Name + "\\" + originalName;
                    string destinationFilePath = Connection.Instance.HydraulicsDirectory + "\\" + Name + "\\" + newName;
                    Directory.Move(sourceFilePath, destinationFilePath);
                    _OriginalFileNames[i] = newName;
                }
                newPathProbs.Add(new HydraulicProfile(ListOfRows[i].Probability, newName));
            }

            HydraulicElement elementToSave = new HydraulicElement(Name, Description, newPathProbs, HydraulicDataSource.UnsteadyHDF, OriginalElement.ID);
            base.Save(elementToSave);
        }

        private void SaveNew()
        {
            string destinationDirectory = Connection.Instance.HydraulicsDirectory + "\\" + Name;
            Directory.CreateDirectory(destinationDirectory);

            List<HydraulicProfile> pathProbs = new List<HydraulicProfile>();
            foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
            {
                _OriginalFileNames.Add(row.Name);
                string directoryName = Path.GetFileName(row.Name);
                pathProbs.Add(new HydraulicProfile( row.Probability, directoryName));

                File.Copy(row.Path, destinationDirectory + "\\" + row.Name);
            }

            int id = GetElementID<HydraulicElement>();
            HydraulicElement elementToSave = new HydraulicElement(Name, Description, pathProbs, HydraulicDataSource.UnsteadyHDF, id);
            base.Save(elementToSave);
        }
        #endregion
    }
}
