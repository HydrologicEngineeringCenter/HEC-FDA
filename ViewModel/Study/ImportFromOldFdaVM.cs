using System;
using System.Linq;

namespace ViewModel.Study
{
    public class ImportFromOldFdaVM: Editors.BaseEditorVM
    {
        public event EventHandler Import;
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
            Import?.Invoke(this, new EventArgs());
        }

        #endregion
        #region Functions
        #endregion
    }
}
