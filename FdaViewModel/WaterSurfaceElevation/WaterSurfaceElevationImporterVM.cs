using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using FdaViewModel.Editors;
using FdaViewModel.Utilities;
using System.IO;

namespace FdaViewModel.WaterSurfaceElevation
{
    //[Author(q0heccdm, 9 / 1 / 2017 8:31:13 AM)]
    public class WaterSurfaceElevationImporterVM:Editors.BaseEditorVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 9/1/2017 8:31:13 AM
        #endregion
        #region Fields
        private ObservableCollection<WaterSurfaceElevationRowItemVM> _ListOfRows;
        private List<PathAndProbability> _ListOfRelativePaths;
        private bool _IsDepthGridChecked;

        #endregion
        #region Properties
        public bool IsEditor { get; set; }
        public bool IsUsingTifFiles { get; set; }// it will either be all tif's or all vrt's. if there are flt's then i will convert them to tif's
        public List<PathAndProbability> ListOfRelativePaths
        {
            get { return _ListOfRelativePaths; }
            set { _ListOfRelativePaths = value; NotifyPropertyChanged(); }
        }
        public List<string> ListOfOriginalPaths { get; set; } //this is only for messaging out in the transaction log
   
        public bool IsDepthGridChecked
        {
            get { return _IsDepthGridChecked; }
            set { _IsDepthGridChecked = value; NotifyPropertyChanged(); }
        }
            public ObservableCollection<WaterSurfaceElevationRowItemVM> ListOfRows
        {
            get { return _ListOfRows; }
            set { _ListOfRows = value; NotifyPropertyChanged(); }
        }

        //public bool HasFatalError { get; internal set; }
        #endregion
        #region Constructors
        public WaterSurfaceElevationImporterVM(EditorActionManager actionManager):base(actionManager)
        {
            IsEditor = false;

            _ListOfRows = new ObservableCollection<WaterSurfaceElevationRowItemVM>();
        }
        /// <summary>
        /// Constructor used when editing an existing child node.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="actionManager"></param>
        public WaterSurfaceElevationImporterVM(WaterSurfaceElevationElement elem, EditorActionManager actionManager) : base(actionManager)
        {
            IsEditor = true;
            _ListOfRows = new ObservableCollection<WaterSurfaceElevationRowItemVM>();
            Name = elem.Name;
            Description = elem.Description;
            ListOfRelativePaths = elem.RelativePathAndProbability;
            IsDepthGridChecked = elem.IsDepthGrids;
            // looks like i have to rebuild this ListOfRows = elem.
            foreach(PathAndProbability pp in ListOfRelativePaths)
            {
                string filename = Path.GetFileName(pp.Path);
                AddRow(true, filename, pp.Path, pp.Probability);
            }

        }

        #endregion
        #region Voids
        public void AddRow(bool isChecked, string name, string path, double probability)
        {
            WaterSurfaceElevationRowItemVM newRow= new WaterSurfaceElevationRowItemVM(isChecked, name, path, probability);
            ListOfRows.Add(newRow);
            NotifyPropertyChanged("ListOfRows");
        }

        public override void AddValidationRules()
        {

            AddRule(nameof(Name), () => Name != null, "Name cannot be null.");
            AddRule(nameof(Name), () => Name != "", "Name cannot be null.");
           
            ////don't allow clicking with a name that already exists
            //AddRule(nameof(Name), () =>
            //{
                
            //    foreach (Utilities.OwnedElement ele in ParentElement.Elements)
            //    {
            //        if(Name == ele.Name)
            //        {
            //            return false;
            //        }
            //    }

            //        return true;
                

            //}, "A water surface profile with that name already exists.");



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

             }, "You have fewer than 8 files selected. You will get better results if you select more files.",false);
            AddRule(nameof(ListOfRows), () =>
            {
                List<double> probabilitiesList = new List<double>();
                foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
                {
                    if (row.IsChecked == true)
                    {
                        if(probabilitiesList.Contains(row.Probability))
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

            //AddRule(nameof(ListOfRows), () =>
            //{
            //    bool allFilesHaveToBeVRT = false;
            //    bool atLeastOneFileIsATif = false;
            //    bool atLeastOneFileIsAFlt = false;
            //    foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
            //    {
            //       if(row.IsVRT == true)
            //        {
            //            allFilesHaveToBeVRT = true;
            //        }
            //       else if(row.IsFLT == true)
            //        {
            //            atLeastOneFileIsAFlt = true;
            //        }
            //       else if(row.IsTIF == true)
            //        {
            //            atLeastOneFileIsATif = true;

            //        }
            //    }



            //    return true;


            //}, "cody");

        }



        //public override void Save()
        //{
        //    //load the list of relative paths
        //    //   WHEN DOES THIS GET CALLED??????
        //    _ListOfRelativePaths = new List<PathAndProbability>();
        //    foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
        //    {
        //        if (row.IsChecked)
        //        {
        //            string relativePath = System.IO.Path.GetDirectoryName(row.Path) + "\\" + System.IO.Path.GetFileName(row.Path);
        //            _ListOfRelativePaths.Add(new PathAndProbability(relativePath, row.Probability));
        //        }
        //    }
        //}
        private void StoreTheOriginalPaths()
        {
            ListOfOriginalPaths = new List<string>();

            foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
            {
                
                if (row.IsChecked)
                {
                    ListOfOriginalPaths.Add(row.Path);
                }
            }
        }

        public override bool RunSpecialValidation()
        {


            StoreTheOriginalPaths();
            ListOfRelativePaths = new List<PathAndProbability>();

            bool atLeastOneFileIsVRT = false;
            bool atLeastOneFileIsTif = false;
            bool atLeastOneFileIsFlt = false;

            int numberOfSelectedRows = 0;

            string rowExtension;
            foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
            {
                if (row.IsChecked == false) { continue; }

                numberOfSelectedRows++;
                rowExtension = System.IO.Path.GetExtension(row.Path);
                switch (rowExtension)
                {
                    case ".vrt":
                        {
                            atLeastOneFileIsVRT = true;
                            break;
                        }
                    case ".flt":
                        {
                            atLeastOneFileIsFlt = true;
                            break;
                        }
                    case ".tif":
                        {
                            atLeastOneFileIsTif = true;
                            break;
                        }
                }

            }


            if (atLeastOneFileIsVRT == true)
            {
                if (atLeastOneFileIsFlt == true || atLeastOneFileIsTif == true)
                {
                    Utilities.CustomMessageBoxVM msgBoxVM = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.OK, 
                        "Cannot mix .vrt and other file types.\nAll files need to be .vrt or .tif.");
                    string header = "Incompatible File Types";
                    DynamicTabVM tab = new DynamicTabVM(header, msgBoxVM, "IncompatibleFileTypes");
                    Navigate(tab, true, true);
                    return false;
                }
                else if (numberOfSelectedRows < 8)
                {
                    Utilities.CustomMessageBoxVM msgBoxVM = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.Yes_No, 
                        "You have only selected " + numberOfSelectedRows + " files. You will get better results with 8 or more files.\n\nDo you want to continue?");
                    string header = "Small Number of Files Selected";
                    DynamicTabVM tab = new DynamicTabVM(header, msgBoxVM, "SmallNumberOfFilesSelected");
                    Navigate(tab, true, true);
                    if (msgBoxVM.ClickedButton == Utilities.CustomMessageBoxVM.ButtonsEnum.Yes)
                    {
                        //close the form and save the wse's
                        if (HasFatalError == true)
                        {
                            return false;
                        }
                        else
                        {
                            //i need to copy the files over to the new location
                            foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
                            {
                                if (row.IsChecked == true)
                                {
                                    if (CopyWaterSurfaceFilesToStudyDirectory(row.Path, row.Name, row.Probability) == false) { return false; }
                                }
                            }
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //close the form and save the wse's
                    if (HasFatalError == true)
                    {
                        return false;
                    }
                    else
                    {
                        //i need to copy the files over to the new location
                        foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
                        {
                            if (row.IsChecked == true)
                            {
                                if (CopyWaterSurfaceFilesToStudyDirectory(row.Path, row.Name, row.Probability) == false) { return false; }
                            }
                        }
                        return true;
                    }

                }
            }

            if (atLeastOneFileIsFlt == true)
            {
                Utilities.CustomMessageBoxVM msgBoxVM = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.Yes_No, "At least one of your files has an extension of *.flt. HEC-Fda only accepts all *.vrt files or all *.tif files.\n\nWould you like to convert your *.flt files to *.tif files?");
                string header = "Change flt to tif";
                DynamicTabVM tab = new DynamicTabVM(header, msgBoxVM, "ChangeFltToTif");
                Navigate(tab, true, true);
                if (msgBoxVM.ClickedButton == Utilities.CustomMessageBoxVM.ButtonsEnum.Yes)
                {
                    //change flt to tif and proceed somehow
                }
                else
                {
                    return false;
                }
            }
            if (atLeastOneFileIsTif == true)
            {
                if (numberOfSelectedRows < 8)
                {
                    Utilities.CustomMessageBoxVM msgBoxVM = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.Yes_No, "You have only selected " + numberOfSelectedRows + " files. You will get better results with 8 or more files.\n\nDo you want to continue?");
                    string header = "SmallNumberOfFilesSelected";
                    DynamicTabVM tab = new DynamicTabVM(header, msgBoxVM, "SmallNumberOfFilesSelected");
                    Navigate(tab, true, true);
                    if (msgBoxVM.ClickedButton == Utilities.CustomMessageBoxVM.ButtonsEnum.Yes)
                    {
                        //close the form and save the wse's
                        if (HasFatalError == true)
                        {
                            return false;
                        }
                        else
                        {
                            //i need to copy the files over to the new location
                            foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
                            {
                                if (row.IsChecked == true)
                                {
                                    if (CopyWaterSurfaceFilesToStudyDirectory(row.Path, row.Name, row.Probability) == false) { return false; }
                                }
                            }
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //close the form and save the wse's
                    if (HasFatalError == true)
                    {
                        return false;
                    }
                    else
                    {
                        //i need to copy the files over to the new location
                        foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
                        {
                            if (row.IsChecked == true)
                            {
                                if (CopyWaterSurfaceFilesToStudyDirectory(row.Path, row.Name, row.Probability) == false) { return false; }
                            }
                        }
                        return true;
                    }
                }
            }


            return false;//shouldn't ever get to here unless nothing was selected.

        }

        private bool CopyWaterSurfaceFilesToStudyDirectory(string path, string nameWithExtension,double probability)
        {
            string destinationFilePath = Storage.Connection.Instance.HydraulicsDirectory + "\\"+ Name + "\\" + nameWithExtension;
            string destinationDirectory = Storage.Connection.Instance.HydraulicsDirectory + "\\" + Name;
            if (!System.IO.Directory.Exists(destinationDirectory))
            {
                System.IO.Directory.CreateDirectory(destinationDirectory);
            }
            try
            {
                System.IO.File.Copy(path, destinationFilePath);
            }
            catch (Exception e)
            {
                Utilities.CustomMessageBoxVM msgBoxVM = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.OK, "An error occured while trying to copy the selected files into the hydraulics directory in your study.\n\n" + e.Message);
                string header = "Error Copying Files";
                DynamicTabVM tab = new DynamicTabVM(header, msgBoxVM, "ErrorCopyingFiles");
                Navigate(tab, true, true);
                return false;
            }
            //string relativePath = System.IO.Path.GetDirectoryName(destinationFilePath) + "\\" + System.IO.Path.GetFileName(destinationFilePath);
            ListOfRelativePaths.Add(new PathAndProbability(Name + "\\" + nameWithExtension, probability));
            return true;
        }

        public override void Save()
        {
            WaterSurfaceElevationElement elementToSave = new WaterSurfaceElevationElement(Name, Description, ListOfRelativePaths, IsDepthGridChecked);
            Saving.PersistenceManagers.WaterSurfaceAreaPersistenceManager manager = Saving.PersistenceFactory.GetWaterSurfaceManager();
            if (IsImporter && HasSaved == false)
            {
                manager.SaveNew(elementToSave);
                HasSaved = true;
                OriginalElement = elementToSave;
            }
            else
            {
                manager.SaveExisting((WaterSurfaceElevationElement)OriginalElement, elementToSave, 0);
            }
        }
        #endregion
        #region Functions
        #endregion
    }
}
