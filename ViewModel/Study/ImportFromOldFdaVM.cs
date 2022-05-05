using System;
using System.Collections.Generic;
using System.Linq;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.ViewModel.Validation;
using static Importer.AsciiImport;
using HEC.MVVMFramework.Base.Enumerations;
using System.IO;

namespace HEC.FDA.ViewModel.Study
{
    public class ImportFromOldFdaVM: ImportFromFDA1VM
    {
        #region Fields
        private StudyElement _StudyElement;
        private string _FolderPath;
        private string _StudyName;
        private List<ChildElement> _FlowFrequencyElements = new List<ChildElement>();
        private List<ChildElement> _InflowOutflowElements = new List<ChildElement>();
        private List<ChildElement> _RatingElements = new List<ChildElement>();
        private List<ChildElement> _ExteriorInteriorElements = new List<ChildElement>();
        private List<ChildElement> _LeveeElements = new List<ChildElement>();
        private List<ChildElement> _OcctypesElements = new List<ChildElement>();
        #endregion
        #region Properties       
        public string FolderPath
        {
            get { return _FolderPath; }
            set
            {
                if (!_FolderPath.Equals(value))
                {
                    _FolderPath = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string StudyName
        {
            get { return _StudyName; }
            set
            {
                if (!_StudyName.Equals(value))
                {
                    _StudyName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion
        #region Constructors

        public ImportFromOldFdaVM(StudyElement studyElement) : base()
        {
            _StudyElement = studyElement;
            _FolderPath = "C:\\temp\\FDA\\";
            _StudyName = "Example";
        }
        #endregion
        #region Voids
        public override void Import()
        {
            RunSetupLogic();
            base.Import();
        }
        public override void AddValidationRules()
        {
            AddSinglePropertyRule(nameof(FolderPath), new Rule(() => { return FolderPath != null; }, "Path cannot be null.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(FolderPath), new Rule(() => { return FolderPath != ""; }, "Path cannot be null.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(FolderPath), new Rule(() => { return IsPathValid();}, "Path contains invalid characters.", ErrorLevel.Severe));

            AddSinglePropertyRule(nameof(StudyName), new Rule(() => { return StudyName != null; }, "Study Name cannot be null.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(StudyName), new Rule(() => { return StudyName != ""; }, "Study Name cannot be null.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(StudyName), new Rule(() =>
            {
                return !File.Exists(Path + "\\" + StudyName + "\\" + StudyName + ".sqlite");
            }, "A study with that name already exists.", ErrorLevel.Severe));

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
            FlowFrequencyPersistenceManager flowFreqManager = Saving.PersistenceFactory.GetFlowFrequencyManager();
            foreach (ChildElement elem in _FlowFrequencyElements)
            {
                flowFreqManager.SaveNew(elem);
            }

            InflowOutflowPersistenceManager inOutManager = Saving.PersistenceFactory.GetInflowOutflowManager();
            foreach (ChildElement elem in _InflowOutflowElements)
            {
                inOutManager.SaveNew(elem);
            }

            RatingElementPersistenceManager ratingManager = Saving.PersistenceFactory.GetRatingManager();
            foreach (ChildElement elem in _RatingElements)
            {
                ratingManager.SaveNew(elem);
            }
            
            ExteriorInteriorPersistenceManager extIntManager = Saving.PersistenceFactory.GetExteriorInteriorManager();
            foreach (ChildElement elem in _ExteriorInteriorElements)
            {
                extIntManager.SaveNew(elem);
            }

            //we can't import stage damages at this time because an impact area set is required first.

            LeveePersistenceManager leveeManager = Saving.PersistenceFactory.GetLeveeManager();
            foreach (ChildElement elem in _LeveeElements)
            {
                leveeManager.SaveNew(elem);
            }

            OccTypePersistenceManager occtypeManager = Saving.PersistenceFactory.GetOccTypeManager();
            foreach (ChildElement elem in _OcctypesElements)
            {
                occtypeManager.SaveNew(elem);
            }

        }

        public void RunSetupLogic()
        {
            //create the sqlite database for this study
            string studyDescription = "";
            //todo: is there a way to get the description from an old fda study?
            _StudyElement.CreateNewStudy(_StudyName, _FolderPath, studyDescription);

            StructureInventoryLibrary.SharedData.StudyDatabase = new DatabaseManager.SQLiteManager(Storage.Connection.Instance.ProjectFile);
        }

        #endregion
    }
}
