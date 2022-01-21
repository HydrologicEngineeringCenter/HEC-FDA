using System;
using System.Collections.Generic;
using System.Linq;
using ViewModel.AggregatedStageDamage;
using ViewModel.FlowTransforms;
using ViewModel.FrequencyRelationships;
using ViewModel.GeoTech;
using ViewModel.Inventory.OccupancyTypes;
using ViewModel.Saving.PersistenceManagers;
using ViewModel.StageTransforms;
using ViewModel.Utilities;
using static Importer.AsciiImport;

namespace ViewModel.Study
{
    public class ImportFromOldFdaVM: ImportFromFDA1VM
    {
        #region Fields
        private StudyElement _StudyElement;
        private string _FolderPath;
        private string _StudyName;
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
        public override void AddValidationRules()
        {
            AddRule(nameof(FolderPath), () => FolderPath != null, "Path cannot be null.");
            AddRule(nameof(FolderPath), () => FolderPath != "", "Path cannot be null.");

            //path must not contain invalid characters
            AddRule(nameof(FolderPath), () =>
            {
                foreach (Char c in System.IO.Path.GetInvalidPathChars())
                {
                    if (FolderPath.Contains(c))
                    {
                        return false;
                    }
                }
                if (FolderPath.Contains('?')) return false;
                return true;
            }, "Path contains invalid characters.");
            //study name must not be null
            AddRule(nameof(StudyName), () => StudyName != null, "Study Name cannot be null.");
            AddRule(nameof(StudyName), () => StudyName != "", "Study Name cannot be null.");

            //check if folder with that name already exists
            AddRule(nameof(StudyName), () =>
            {
                if (System.IO.File.Exists(FolderPath + "\\" + StudyName + "\\" + StudyName + ".sqlite"))
                {
                    return false;
                }
                return true;
            }, "A study with that name already exists.");
        }

        private List<ChildElement> _FlowFrequencyElements = new List<ChildElement>();
        private List<ChildElement> _InflowOutflowElements = new List<ChildElement>();
        private List<ChildElement> _RatingElements = new List<ChildElement>();
        private List<ChildElement> _ExteriorInteriorElements = new List<ChildElement>();
        private List<ChildElement> _LeveeElements = new List<ChildElement>();
        private List<ChildElement> _OcctypesElements = new List<ChildElement>();

        public override ImportOptions GetImportOptions()
        {
            return ImportOptions.ImportEverything;
        }

        public override List<ChildElement> CreateElements(bool checkForNameConflict = true)
        {
            ImportFrequencyFromFDA1VM freqVM = new ImportFrequencyFromFDA1VM();
            _FlowFrequencyElements.AddRange(freqVM.CreateElements(false));
            ImportLog += freqVM.ImportLog;

            ImportInflowOutflowFromFDA1VM inOutVM = new ImportInflowOutflowFromFDA1VM();
            _InflowOutflowElements.AddRange(inOutVM.CreateElements(false));
            ImportLog += inOutVM.ImportLog;

            ImportRatingsFromFDA1VM ratingsVM = new ImportRatingsFromFDA1VM();
            _RatingElements.AddRange(ratingsVM.CreateElements(false));
            ImportLog += ratingsVM.ImportLog;

            ImportExteriorInteriorFromFDA1VM extIntVM = new ImportExteriorInteriorFromFDA1VM();
            _ExteriorInteriorElements.AddRange(extIntVM.CreateElements(false));
            ImportLog += extIntVM.ImportLog;

            ImportLeveeElementFromFDA1VM leveeVM = new ImportLeveeElementFromFDA1VM();
            _LeveeElements.AddRange(leveeVM.CreateElements(false));
            ImportLog += leveeVM.ImportLog;

            //we can't import stage damages at this time because an impact area set is required first.
            ImportLog += "\nStage damage curves cannot be imported at this time.\n" +
                "Impact areas are required before stage damages can be imported.\n";

            //occtypes needs to have the path so that it can get the correct "group name".
            ImportOcctypesFromFDA1VM occtypesVM = new ImportOcctypesFromFDA1VM();
            occtypesVM.Path = Path;
            _OcctypesElements.AddRange(occtypesVM.CreateElements(false));
            ImportLog += occtypesVM.ImportLog;

            //i don't actually care about the return list here. It won't be used.
            return new List<ChildElement>();
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

        public override void RunSetupLogic()
        {
            //create the sqlite database for this study
            string studyDescription = "";
            //todo: is there a way to get the description from an old fda study?
            _StudyElement.CreateStudyFromViewModel(_StudyName, _FolderPath, studyDescription);

            StructureInventoryLibrary.SharedData.StudyDatabase = new DatabaseManager.SQLiteManager(Storage.Connection.Instance.ProjectFile);
        }

        //public override void Save()
        //{
        //    //create the sqlite database for this study
        //    string studyDescription = "";
        //    //todo: is there a way to get the description from an old fda study?
        //    _StudyElement.CreateStudyFromViewModel(_StudyName, _FolderPath, studyDescription);

        //    StructureInventoryLibrary.SharedData.StudyDatabase = new DatabaseManager.SQLiteManager(Storage.Connection.Instance.ProjectFile);

        //    DoImport(
        //        vr=>
        //        {
        //            if (vr.IsValid)
        //            {
        //                SaveElements();
        //            }
        //            else
        //            {
        //                ImportUpdates += "Import Failed:" + Environment.NewLine + vr.ErrorMessage;

        //            }
        //        }
        //        );


        ////import all the data from the import file
        //AsyncLogger logger = new AsyncLogger();
        //AsciiImport importer = new AsciiImport(logger);
        //importer.ImportAsciiData(ImportFilePath, AsciiImport.ImportOptions.ImportEverything);

        ////ratings
        //RatingFunctionList ratings = GlobalVariables.mp_fdaStudy.GetRatingFunctionList();
        //List<RatingCurveElement> ratingCurveElements = ImportFromFDA1Helper.CreateRatingElements(ratings);



        ////occtypes
        //OccupancyTypeList occupancyTypeList = GlobalVariables.mp_fdaStudy.GetOccupancyTypeList();
        ////Saving.PersistenceFactory.GetOccTypeManager().SaveFDA1Elements(occupancyTypeList, importer._FileName);


        ////rating curves
        ////RatingFunctionList ratings = GlobalVariables.mp_fdaStudy.GetRatingFunctionList();
        ////List<RatingCurveElement> ratingCurveElements = ImportFromFDA1Helper.CreateRatingElements(ratings);
        ////Saving.PersistenceManagers.RatingElementPersistenceManager manager = Saving.PersistenceFactory.GetRatingManager();
        ////foreach (RatingCurveElement elem in ratingCurveElements)
        ////{
        ////    manager.SaveNew(elem);
        ////}
        //ImportRatingsFromFDA1VM ratingsVM = new ImportRatingsFromFDA1VM();
        //ratingsVM.Path = ImportFilePath;
        //ratingsVM.Import();

        ////levees
        //LeveeList leveeList = GlobalVariables.mp_fdaStudy.GetLeveeList();
        //foreach (KeyValuePair<string, Levee> kvp in leveeList.Levees)
        //{
        //    Levee lev =  kvp.Value;

        //    if (lev.FailureFunctionPairs.Count > 0)
        //    {
        //        Saving.PersistenceFactory.GetFailureFunctionManager().SaveFDA1Elements(lev);
        //    }
        //    if (lev.ExteriorInteriorPairs.Count > 0)
        //    {
        //        Saving.PersistenceFactory.GetExteriorInteriorManager().SaveFDA1Element(lev);
        //    }

        //}

        ////probability functions
        //ProbabilityFunctionList probFuncs = GlobalVariables.mp_fdaStudy.GetProbabilityFuncList();
        //foreach (KeyValuePair<string, ProbabilityFunction> kvp in probFuncs.ProbabilityFunctions)
        //{
        //    ProbabilityFunction pf = kvp.Value;
        //    FrequencyFunctionType typeID = pf.ProbabilityFunctionTypeId;
        //    if(typeID == FrequencyFunctionType.ANALYTICAL || typeID == FrequencyFunctionType.GRAPHICAL)
        //    {
        //        Saving.PersistenceFactory.GetFlowFrequencyManager().SaveFDA1Element(pf);
        //    }
        //    else if(pf.NumberOfTransFlowPoints>0)
        //    {
        //        //Saving.PersistenceFactory.GetInflowOutflowManager().SaveFDA1Element(pf);
        //    }
        //}

        ////aggregated stage damages
        ////AggregateDamageFunctionList aggDamageList = GlobalVariables.mp_fdaStudy.GetAggDamgFuncList();
        ////Saving.PersistenceFactory.GetStageDamageManager().SaveFDA1Elements(aggDamageList);





        ////OccupancyTypeList occtypes = GlobalVariables.mp_fdaStudy.GetOccupancyTypeList();

        ////this line should write all the applicable data to the database.
        ////It creates the view model objects and then writes them to the sqlite db in the "flush" 
        ////for each fda element type. The sqlite db needs to already exist.
        //}

        #endregion
        #region Functions
        #endregion
    }
}
