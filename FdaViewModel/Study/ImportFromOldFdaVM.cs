using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Study
{
    public class ImportFromOldFdaVM: Editors.BaseEditorVM
    {
        public event EventHandler Import;
        #region Fields
        private string _FolderPath;
        private string _StudyName;
        private string _Description;
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

        private StudyElement _StudyElement;
        #endregion
        #region Constructors
        //public NewStudyVM() : base(null)
        //{
        //    _Path = "C:\\temp\\FDA\\";
        //    _StudyName = "Example";
        //    _Description = "My description";
        //}

        public ImportFromOldFdaVM(StudyElement studyElement) : base(null)
        {
            _StudyElement = studyElement;
            _FolderPath = "C:\\temp\\FDA\\";
            _StudyName = "Example";
            _Description = "My description";
        }
        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            AddRule(nameof(FolderPath), () => FolderPath != null, "Path cannot be null.");
            AddRule(nameof(FolderPath), () => FolderPath != "", "Path cannot be null.");

            //path must be a valid path and not currently exist //possibly allow for creation of new directory here, but would require invalid character search.
            //AddRule(nameof(Path), () => System.IO.Directory.Exists(Path), "Directory must exist");
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

            //notes can be null.
        }

        public override void Save()
        {
            //create the sqlite database for this study
            _StudyElement.CreateStudyFromViewModel(_StudyName, _FolderPath);

            StructureInventoryLibrary.SharedData.StudyDatabase = new DatabaseManager.SQLiteManager(Storage.Connection.Instance.ProjectFile);

            //import all the data from the import file
            Import?.Invoke(this, new EventArgs());
        }


        #endregion
        #region Functions
        #endregion
    }
}
