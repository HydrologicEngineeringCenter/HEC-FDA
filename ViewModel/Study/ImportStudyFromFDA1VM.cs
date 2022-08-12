using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using static Importer.AsciiImport;

namespace HEC.FDA.ViewModel.Study
{
    public class ImportStudyFromFDA1VM: ImportFromFDA1VM
    {
        #region Fields
        private StudyElement _StudyElement;
        private string _FolderPath;
        private string _StudyName;
        private string _Description = "";
        private List<ChildElement> _FlowFrequencyElements = new List<ChildElement>();
        private List<ChildElement> _InflowOutflowElements = new List<ChildElement>();
        private List<ChildElement> _RatingElements = new List<ChildElement>();
        private List<ChildElement> _ExteriorInteriorElements = new List<ChildElement>();
        private List<ChildElement> _LeveeElements = new List<ChildElement>();
        private List<ChildElement> _OcctypesElements = new List<ChildElement>();
        private bool _HaventImported = true;
        #endregion
        #region Properties

        public bool HaventImported
        {
            get { return _HaventImported; }
            set { _HaventImported = value; NotifyPropertyChanged(); }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
        public string FolderPath
        {
            get { return _FolderPath; }
            set { _FolderPath = value; NotifyPropertyChanged();}
        }
        public string StudyName
        {
            get { return _StudyName; }
            set { _StudyName = value;NotifyPropertyChanged();}
        }

        #endregion
        #region Constructors

        public ImportStudyFromFDA1VM(StudyElement studyElement) : base()
        {
            _StudyElement = studyElement;
            Validate();
        }
        #endregion
        #region Voids
        public override void Import()
        {
            FdaValidationResult validationResult = ValidateEditor();
            if(validationResult.IsValid)
            {
                RunSetupLogic();
                base.Import();
                HaventImported = false;
            }
            else
            {
                MessageBox.Show(validationResult.ErrorMessage, "Invalid Entries", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        public override void AddValidationRules()
        {
            AddRule(nameof(StudyName), () => !string.IsNullOrWhiteSpace(StudyName), "Name cannot be blank or whitespace.");
            AddRule(nameof(FolderPath), () => !string.IsNullOrWhiteSpace(FolderPath), "Study path cannot be blank or whitespace.");

            //path must not contain invalid characters
            AddRule(nameof(FolderPath), () => IsPathValid(), "Path contains invalid characters.");

            //check if folder with that name already exists
            AddRule(nameof(StudyName), () =>!File.Exists(FolderPath + "\\" + StudyName + "\\" + StudyName + ".sqlite") , "A study with that name already exists.");
        }
        private bool IsPathValid()
        {
            bool pathIsValid = true;
            if (FolderPath != null && FolderPath != "")
            {
                foreach (Char c in System.IO.Path.GetInvalidPathChars())
                {
                    if (FolderPath.Contains(c))
                    {
                        pathIsValid = false;
                        break;
                    }
                }
                if (FolderPath.Contains('?'))
                {
                    pathIsValid = false;
                }
            }
            return pathIsValid;
        }

        public override ImportOptions GetImportOptions()
        {
            return ImportOptions.ImportEverything;
        }

        public override void CreateElements(bool checkForNameConflict = true)
        {
            ImportFrequencyFromFDA1VM freqVM = new ImportFrequencyFromFDA1VM();
            freqVM.CreateElements(false);
            _FlowFrequencyElements.AddRange(freqVM.ElementsToImport);
            ImportLog += freqVM.ImportLog;

            ImportInflowOutflowFromFDA1VM inOutVM = new ImportInflowOutflowFromFDA1VM();
            inOutVM.CreateElements(false);
            _InflowOutflowElements.AddRange(inOutVM.ElementsToImport);
            ImportLog += inOutVM.ImportLog;

            ImportRatingsFromFDA1VM ratingsVM = new ImportRatingsFromFDA1VM();
            ratingsVM.CreateElements(false);
            _RatingElements.AddRange(ratingsVM.ElementsToImport);
            ImportLog += ratingsVM.ImportLog;

            ImportExteriorInteriorFromFDA1VM extIntVM = new ImportExteriorInteriorFromFDA1VM();
            extIntVM.CreateElements(false);
            _ExteriorInteriorElements.AddRange(extIntVM.ElementsToImport);
            ImportLog += extIntVM.ImportLog;

            ImportLeveeElementFromFDA1VM leveeVM = new ImportLeveeElementFromFDA1VM();
            leveeVM.CreateElements(false);
            _LeveeElements.AddRange(leveeVM.ElementsToImport);
            ImportLog += leveeVM.ImportLog;

            //we can't import stage damages at this time because an impact area set is required first.
            ImportLog += Environment.NewLine + "Stage damage curves cannot be imported at this time." + Environment.NewLine +
                "Impact areas are required before stage damages can be imported." + Environment.NewLine;

            //occtypes needs to have the path so that it can get the correct "group name".
            ImportOcctypesFromFDA1VM occtypesVM = new ImportOcctypesFromFDA1VM();
            occtypesVM.Path = Path;
            occtypesVM.CreateElements(false);
            _OcctypesElements.AddRange(occtypesVM.ElementsToImport);
            ImportLog += occtypesVM.ImportLog;
        }

        public override void SaveElements()
        {
            IElementManager flowFreqManager = PersistenceFactory.GetElementManager<AnalyticalFrequencyElement>();
            foreach (ChildElement elem in _FlowFrequencyElements)
            {
                flowFreqManager.SaveNew(elem);
            }

            IElementManager inOutManager = PersistenceFactory.GetElementManager<InflowOutflowElement>();
            foreach (ChildElement elem in _InflowOutflowElements)
            {
                inOutManager.SaveNew(elem);
            }

            IElementManager ratingManager = PersistenceFactory.GetElementManager<StageDischargeElement>();
            foreach (ChildElement elem in _RatingElements)
            {
                ratingManager.SaveNew(elem);
            }

            IElementManager extIntManager = PersistenceFactory.GetElementManager<ExteriorInteriorElement>();
            foreach (ChildElement elem in _ExteriorInteriorElements)
            {
                extIntManager.SaveNew(elem);
            }

            //we can't import stage damages at this time because an impact area set is required first.

            IElementManager leveeManager = PersistenceFactory.GetElementManager<LateralStructureElement>();
            foreach (ChildElement elem in _LeveeElements)
            {
                leveeManager.SaveNew(elem);
            }

            IElementManager occtypeManager = PersistenceFactory.GetOccTypeManager();
            foreach (ChildElement elem in _OcctypesElements)
            {
                occtypeManager.SaveNew(elem);
            }

        }

        public void RunSetupLogic()
        {
            //create the sqlite database for this study
            _StudyElement.CreateNewStudy(_StudyName, _FolderPath, _Description);

            StructureInventoryLibrary.SharedData.StudyDatabase = new DatabaseManager.SQLiteManager(Storage.Connection.Instance.ProjectFile);
        }

        #endregion

        private FdaValidationResult ValidateEditor()
        {
            FdaValidationResult result = new FdaValidationResult();
            Validate();
            if(HasFatalError)
            {
                result.AddErrorMessage(Error);
            }
            return result;
        }

    }
}
