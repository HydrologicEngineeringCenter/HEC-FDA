using System;
using System.Linq;

namespace ViewModel.Study
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
            AddRule(nameof(Path), () => Path != null, "Path cannot be null.");
            AddRule(nameof(Path), () => Path != "", "Path cannot be null.");

            //path must not contain invalid characters
            AddRule(nameof(Path), () =>
            {
                foreach (Char c in System.IO.Path.GetInvalidPathChars())
                {
                    
                    if (Path.Contains(c))
                    {
                        
                        return false;
                    }
                }
                if (Path.Contains('?')) return false;
                return true;
            },"Path contains invalid characters.");
        //study name must not be null
            AddRule(nameof(StudyName), () => StudyName != null, "Study Name cannot be null.");
            AddRule(nameof(StudyName), () => StudyName != "", "Study Name cannot be null.");

            //check if folder with that name already exists
            AddRule(nameof(StudyName), () =>
            {
                
                if(System.IO.File.Exists(Path +"\\"+StudyName +"\\"+ StudyName + ".sqlite"))
                {
                    return false;
                }
                return true;
            }, "A study with that name already exists.");

            //notes can be null.
        }

        public override void Save()
        {
            _StudyElement.CreateStudyFromViewModel(_StudyName, _Path, _Description);
        }


        #endregion
        #region Functions
        #endregion

    }
}
