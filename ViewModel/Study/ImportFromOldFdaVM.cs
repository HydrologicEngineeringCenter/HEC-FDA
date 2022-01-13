using Importer;
using System;
using System.Collections.Generic;
using System.Linq;
using static Importer.ProbabilityFunction;

namespace ViewModel.Study
{
    public class ImportFromOldFdaVM: Editors.BaseEditorVM
    {
        #region Fields
        private StudyElement _StudyElement;
        private string _FolderPath;
        private string _StudyName;
        #endregion
        #region Properties       
        public string ImportFilePath
        {
            get;set;
        }

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

        public ImportFromOldFdaVM(StudyElement studyElement) : base(null)
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

        public override void Save()
        {
            //create the sqlite database for this study
            string studyDescription = "";
            //todo: is there a way to get the description from an old fda study?
            _StudyElement.CreateStudyFromViewModel(_StudyName, _FolderPath, studyDescription);

            StructureInventoryLibrary.SharedData.StudyDatabase = new DatabaseManager.SQLiteManager(Storage.Connection.Instance.ProjectFile);

            //import all the data from the import file
            AsciiImport importer = new AsciiImport();
            importer.ImportAsciiData(ImportFilePath, AsciiImport.ImportOptions.ImportEverything);

            //occtypes
            OccupancyTypeList occupancyTypeList = GlobalVariables.mp_fdaStudy.GetOccupancyTypeList();
            Saving.PersistenceFactory.GetOccTypeManager().SaveFDA1Elements(occupancyTypeList, importer._FileName);


            //rating curves
            RatingFunctionList ratings = GlobalVariables.mp_fdaStudy.GetRatingFunctionList();
            Saving.PersistenceFactory.GetRatingManager().SaveFDA1Elements(ratings);

            //levees
            LeveeList leveeList = GlobalVariables.mp_fdaStudy.GetLeveeList();
            foreach (KeyValuePair<string, Levee> kvp in leveeList.Levees)
            {
                Levee lev =  kvp.Value;

                if (lev.FailureFunctionPairs.Count > 0)
                {
                    Saving.PersistenceFactory.GetFailureFunctionManager().SaveFDA1Elements(lev);
                }
                if (lev.ExteriorInteriorPairs.Count > 0)
                {
                    Saving.PersistenceFactory.GetExteriorInteriorManager().SaveFDA1Element(lev);
                }

            }

            //probability functions
            ProbabilityFunctionList probFuncs = GlobalVariables.mp_fdaStudy.GetProbabilityFuncList();
            foreach (KeyValuePair<string, ProbabilityFunction> kvp in probFuncs.ProbabilityFunctions)
            {
                ProbabilityFunction pf = kvp.Value;
                FrequencyFunctionType typeID = pf.ProbabilityFunctionTypeId;
                if(typeID == FrequencyFunctionType.ANALYTICAL || typeID == FrequencyFunctionType.GRAPHICAL)
                {
                    Saving.PersistenceFactory.GetFlowFrequencyManager().SaveFDA1Element(pf);
                }
                else if(pf.NumberOfTransFlowPoints>0)
                {
                    Saving.PersistenceFactory.GetInflowOutflowManager().SaveFDA1Element(pf);
                }
            }

            //aggregated stage damages
            //AggregateDamageFunctionList aggDamageList = GlobalVariables.mp_fdaStudy.GetAggDamgFuncList();
            //Saving.PersistenceFactory.GetStageDamageManager().SaveFDA1Elements(aggDamageList);

            



            //OccupancyTypeList occtypes = GlobalVariables.mp_fdaStudy.GetOccupancyTypeList();

            //this line should write all the applicable data to the database.
            //It creates the view model objects and then writes them to the sqlite db in the "flush" 
            //for each fda element type. The sqlite db needs to already exist.
        }

        #endregion
        #region Functions
        #endregion
    }
}
