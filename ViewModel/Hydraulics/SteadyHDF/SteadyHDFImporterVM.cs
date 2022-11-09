using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace HEC.FDA.ViewModel.Hydraulics.SteadyHDF
{
    public class SteadyHDFImporterVM:BaseEditorVM
    {
        #region Fields
        private string _SelectedPath;
        #endregion

        #region Properties
        public string SelectedPath
        {
            get { return _SelectedPath; }
            set { _SelectedPath = value; PopulateRows(value); NotifyPropertyChanged(); }
        }

        public ObservableCollection<WaterSurfaceElevationRowItemVM> ListOfRows { get; } = new ObservableCollection<WaterSurfaceElevationRowItemVM>();
        #endregion
        #region Constructors
        public SteadyHDFImporterVM(EditorActionManager actionManager) : base(actionManager)
        {
            AddRule(nameof(ListOfRows), () => ListOfRows.Count > 0, "Invalid file selected.");
        }
        /// <summary>
        /// Constructor used when editing an existing child node.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="actionManager"></param>
        public SteadyHDFImporterVM(HydraulicElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
            SelectedPath = Connection.Instance.HydraulicsDirectory + "\\" + elem.Name;
            Name = elem.Name;
            Description = elem.Description;
            foreach (HydraulicProfile pp in elem.DataSet.HydraulicProfiles)
            {
                string path = Connection.Instance.HydraulicsDirectory + "\\" + pp.FileName;
                string folderName = Path.GetFileName(pp.FileName);
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
            if (firstPeriodIndex != -1)
            {
                string substring = file.Substring(firstPeriodIndex + 1);
                Regex r = new Regex("p??.hdf");
                if (!r.Match(substring).Success)
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

        public void PopulateRows(string fullpath)
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (fullpath != null && IsCreatingNewElement)
            {
                ListOfRows.Clear();

                FdaValidationResult fileValidResult = IsFileValid(fullpath);
                if (fileValidResult.IsValid)
                {

                    string[] profileNames = GetProfileNamesFromFilePath(fullpath);
                    double prob = 0;
                    foreach (string name in profileNames)
                    {
                        prob += .1;
                        AddRow(Path.GetFileName(name), Path.GetFullPath(name), prob);
                    }
                    if (!vr.IsValid)
                    {
                        vr.InsertMessage(0, "Some files or subdirectories are being ignored:\n");
                        MessageBox.Show(vr.ErrorMessage, "Invalid Files", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    vr.AddErrorMessage(fileValidResult.ErrorMessage);
                    MessageBox.Show(vr.ErrorMessage, "Invalid Files", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private string[] GetProfileNamesFromFilePath(string fullpath)
        {
            string[] profileNames = null;
            try
            {
                RasMapperLib.RASResults result = new RasMapperLib.RASResults(fullpath);
                profileNames = result.ProfileNames;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error getting profile names from selected file: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return profileNames;
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

        private void CopyFileToStudyDirectory()
        {
            string destinationDirectory = Connection.Instance.HydraulicsDirectory + "\\" + Name;
            Directory.CreateDirectory(destinationDirectory);
            string originalFileName = Path.GetFileName(SelectedPath);
            File.Copy(SelectedPath, destinationDirectory + "\\" + originalFileName);
        }

        private void SaveNew()
        {
            CopyFileToStudyDirectory();

            List<HydraulicProfile> pathProbs = new List<HydraulicProfile>();
            foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
            {
                pathProbs.Add(new HydraulicProfile( row.Probability, SelectedPath, row.Name));
            }

            int id = GetElementID<HydraulicElement>();
            HydraulicElement elementToSave = new HydraulicElement(Name, Description, pathProbs, HydraulicDataSource.SteadyHDF, id);
            base.Save(elementToSave);            
        }

        private void SaveExisting()
        {
            //the user can not change files when editing, so the only changes would be new names and probs.    
            //if name is different then we need to update the directory name in the study hydraulics folder.
            RenameDirectoryInStudy();

            List<HydraulicProfile> newPathProbs = new List<HydraulicProfile>();
            for (int i = 0; i < ListOfRows.Count; i++)
            {
                newPathProbs.Add(new HydraulicProfile( ListOfRows[i].Probability, SelectedPath,ListOfRows[i].Name));
            }
            HydraulicElement elemToSave = new HydraulicElement(Name, Description, newPathProbs, HydraulicDataSource.SteadyHDF, OriginalElement.ID);
            base.Save(elemToSave);
        }

        #endregion
    }
}
