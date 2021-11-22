using System;
using System.Linq;

namespace ViewModel.Study
{
    public class ExistingStudyVM : Editors.BaseEditorVM
    {
        #region Notes
        #endregion
        #region Fields
        private string _Path;
        private string _StudyName;
        #endregion
        #region Properties
        public string Path
        {
            get { return _Path; }
            set
            {
                if (_Path!=value)
                {
                    _Path = value;
                    NotifyPropertyChanged();
                    if (System.IO.File.Exists(_Path) && System.IO.Path.GetExtension(_Path).ToLower() == ".sqlite")
                    {
                        StudyName = System.IO.Path.GetFileNameWithoutExtension(Path);
                        Storage.Connection.Instance.ProjectFile = _Path;
                        //todo: what on earth is all this stuff in this setter? -cody 11/18/21
                        //DatabaseManager.DataTableView dtv = Storage.Connection.Instance.GetTable("Study Properties");
                        //if (dtv != null)
                        //{
                        //    PropertiesVM pvm = new PropertiesVM(dtv);
                        //    Description = pvm.StudyDescription;
                        //}
                    }else
                    {
                        //ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("You did not select a sqlite file", FdaModel.Utilities.Messager.ErrorMessageEnum.Fatal));
                    }
                }
            }
        }

        public StudyElement StudyElement { get; set; }
        public string StudyName
        {
            get { return _StudyName; }
            set
            {
                if (_StudyName!=value)
                {
                    _StudyName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        #endregion
        #region Constructors
        public ExistingStudyVM(StudyElement studyElement) : base(null)
        {
            StudyElement = studyElement;
        }
        #endregion
        #region Voids
        public override void Save()
        {
            StudyElement.OpenStudyFromFilePath(StudyName, Path);
        }

        public override void AddValidationRules()
        {
            AddRule(nameof(Path), () => Path != null, "Path cannot be null.");
            AddRule(nameof(Path), () => Path != "", "Path cannot be null.");
            AddRule(nameof(Path), () =>
            {
                return System.IO.File.Exists(Path);
            }, "File does not exist.");

            AddRule(nameof(Path), () =>
            {
                return System.IO.Path.GetExtension(Path) == ".sqlite";
            }, "Selected file is the wrong file type. File must be '*.sqlite'");

            AddRule(nameof(Path), () =>
            {
                if (Path != null && Path != "")
                {
                    foreach (Char c in System.IO.Path.GetInvalidPathChars())
                    {

                        if (Path.Contains(c))
                        {

                            return false;
                        }
                    }
                    if (Path.Contains('?')) return false;
                }
                return true;
            }, "Path contains invalid characters.");
        }
  
        #endregion
        #region Functions
        #endregion
    }
}
