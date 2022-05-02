using HEC.MVVMFramework.ViewModel.Validation;
using System;
using System.Linq;
using HEC.MVVMFramework.Base.Enumerations;

namespace HEC.FDA.ViewModel.Study
{
    public class NewStudyVM : Editors.BaseEditorVM
    {
        #region Notes
        #endregion
        #region Fields
        private StudyElement _StudyElement;
        private string _Path;
        private string _StudyName;
        private string _Description;
        #endregion
        #region Properties
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
            _Path = "C:\\temp\\FDA\\";
            _StudyName = "Example";
            _Description = "My description";
        }
        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            AddSinglePropertyRule(nameof(Path), new Rule(() => { return Path != null; }, "Path cannot be null.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Path), new Rule(() => { return Path != ""; }, "Path cannot be null.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Path), new Rule(() =>
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
            }, "Path contains invalid characters.", ErrorLevel.Severe));

            AddSinglePropertyRule(nameof(StudyName), new Rule(() => { return StudyName != null; }, "Study Name cannot be null.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(StudyName), new Rule(() => { return StudyName != ""; }, "Study Name cannot be null.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(StudyName), new Rule(() => 
            {
                return !System.IO.File.Exists(Path + "\\" + StudyName + "\\" + StudyName + ".sqlite");
            }, "A study with that name already exists.", ErrorLevel.Severe));

        }

        public override void Save()
        {
            _StudyElement.CreateNewStudy(_StudyName, _Path, _Description);
        }

        #endregion

    }
}
