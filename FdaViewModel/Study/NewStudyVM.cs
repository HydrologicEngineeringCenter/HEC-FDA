using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Study
{
    public class NewStudyVM : Editors.BaseEditorVM
    {
        #region Notes
        #endregion
        #region Fields
        private string _Path;
        private string _StudyName;
        private string _Description;
        #endregion
        #region Properties
        //for testing delete me
        public FunctionsView.ViewModel.CoordinatesFunctionEditorVM Curve
        {
            get 
            {
                List<double> xs = new List<double>() { 1, 2, 3, 4 };
                List<double> ys = new List<double>() { 2,3,4,5 };

                // Functions.ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(xs, ys);
                //IFdaFunction function = ImpactAreaFunctionFactory.Factory(func, Model.Condition.ComputePoint.ImpactAreaFunctions.ImpactAreaFunctionEnum.InflowOutflow);

                //return new CurveGeneratorVM(function, Model.Condition.ComputePoint.ImpactAreaFunctions.ImpactAreaFunctionEnum.InflowOutflow);
                return null;
            }
        }
            //delete to here
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

        private StudyElement _StudyElement;
        #endregion
        #region Constructors
        //public NewStudyVM() : base(null)
        //{
        //    _Path = "C:\\temp\\FDA\\";
        //    _StudyName = "Example";
        //    _Description = "My description";
        //}

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

            //path must be a valid path and not currently exist //possibly allow for creation of new directory here, but would require invalid character search.
            //AddRule(nameof(Path), () => System.IO.Directory.Exists(Path), "Directory must exist");
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
            _StudyElement.CreateStudyFromViewModel(this);
        }


        #endregion
        #region Functions
        #endregion

    }
}
