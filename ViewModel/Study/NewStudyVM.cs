using System;
using System.IO;

namespace HEC.FDA.ViewModel.Study
{
    public class NewStudyVM : Editors.BaseEditorVM
    {
        #region Notes
        #endregion
        #region Fields
        private StudyElement _StudyElement;
        private string _Path = "";
        private string _StudyName = "";
        private string _Description = "";
        #endregion
        #region Properties
        public string Description
        {
            get { return _Description; }
            set { _Description = value;}
        }
        public string Path { get { return _Path; }
        set
            {
                if (!_Path.Equals(value)){
                    _Path = value;
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

        public NewStudyVM(StudyElement studyElement) : base(null)
        {
            _StudyElement = studyElement;
        }
        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            AddRule(nameof(Path), () => Path != null, "Path cannot be null.");
            AddRule(nameof(Path), () => Path != "", "Path cannot be null.");
            AddRule(nameof(StudyName), () => StudyName != null, "Study Name cannot be null.");
            AddRule(nameof(StudyName), () => StudyName != "", "Study Name cannot be null.");

            //path must not contain invalid characters
            AddRule(nameof(Path), () => IsPathValid(), "Path contains invalid characters.");

            //check if folder with that name already exists
            AddRule(nameof(StudyName), () => !File.Exists(Path + "\\" + StudyName + "\\" + StudyName + ".sqlite"), "A study with that name already exists.");
        }

        private bool IsPathValid()
        {
            bool pathIsValid = true;
            if (Path != null && Path != "")
            {
                foreach (Char c in System.IO.Path.GetInvalidPathChars())
                {
                    if (Path.Contains(c))
                    {
                        pathIsValid = false;
                        break;
                    }
                }
                if (Path.Contains('?'))
                {
                    pathIsValid = false;
                }
            }
            return pathIsValid;
        }
        public override void Save()
        {
            _StudyElement.CreateNewStudy(_StudyName, _Path, _Description);
        }

        #endregion

    }
}
