using System;
using System.Linq;
using System.Windows;
using ViewModel.Utilities;

namespace ViewModel.Study
{
    public class PropertiesVM : Editors.BaseEditorVM
    {
        #region Notes
        #endregion
        #region Fields
        //private StudyPropertiesElement _CurrentElement;

        //public static readonly string TableName = "Study Properties";
        //private  string _StudyName;
        //private readonly string _StudyPath;
        //private string _StudyDescription;
        //private readonly string _CreatedBy;
        //private readonly string _CreatedDate;
        //private string _StudyNotes;
        //private MonetaryUnitsEnum _MonetaryUnit;
        //private UnitsSystemEnum _UnitSystem;
        //private int _SurveyedYear;
        //private int _UpdatedYear;
        //private Single _UpdatedPriceIndex;
        //private double _DiscountRate;
        //private int _PeriodOfAnalysis;
        #endregion
        #region Properties

        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public string StudyName { get; set; }
        public string StudyPath { get; set; }
        public string StudyDescription { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string StudyNotes { get; set; }
        public MonetaryUnitsEnum MonetaryUnit { get; set; }
        public UnitsSystemEnum UnitSystem { get; set; }
        public int SurveyedYear { get; set; }
        public int UpdatedYear { get; set; }

        public double UpdatedPriceIndex { get; set; }
        
        #endregion
        #region Constructors
        //public PropertiesVM():base(null)
        //{
        //    _StudyName = "Example Study Name";
        //    _StudyPath = "C:\\Temp\\FDA";
        //    _StudyDescription = "Example Study Description";
        //    _CreatedBy = System.Environment.UserName;
        //    _CreatedDate = DateTime.Now.ToShortDateString();
        //    _StudyNotes = "These are my notes";
        //    _MonetaryUnit = MonetaryUnitsEnum.Millions;
        //    _UnitSystem = UnitsSystemEnum.English;
        //    _SurveyedYear = DateTime.Now.Year - 1;
        //    _UpdatedYear = DateTime.Now.Year;
        //    _UpdatedPriceIndex = 0.01f;
        //}
        /// <summary>
        /// This is called when creating a new study
        /// </summary>
        /// <param name="studyName"></param>
        /// <param name="studyPath"></param>
        //public PropertiesVM(string studyName, string studyPath, string studyDescription):base(null)
        //{
        //    _StudyName = studyName;
        //    _StudyPath = studyPath;
        //    _StudyDescription = studyDescription;
        //    _CreatedBy = Environment.UserName;
        //    _CreatedDate = DateTime.Now.ToShortDateString();
        //    _StudyNotes = "";
        //    _MonetaryUnit = MonetaryUnitsEnum.Millions;
        //    _UnitSystem = UnitsSystemEnum.English;
        //    _SurveyedYear = DateTime.Now.Year - 1;
        //    _UpdatedYear = DateTime.Now.Year;
        //    _UpdatedPriceIndex = 0.01f;
        //    _DiscountRate = .025;
        //    _PeriodOfAnalysis = 50;
        //}

        public PropertiesVM(StudyPropertiesElement elem):base(elem, null)
        {
            StudyName = elem.Name;
            StudyPath = elem.StudyPath;
            StudyDescription = elem.StudyDescription;
            CreatedBy = elem.CreatedBy;
            CreatedDate = elem.CreatedDate;
            StudyNotes = elem.StudyNotes;
            MonetaryUnit = elem.MonetaryUnit;
            UnitSystem = elem.UnitSystem;
            SurveyedYear = elem.SurveyedYear;
            UpdatedYear = elem.UpdatedYear;
            UpdatedPriceIndex = elem.UpdatedPriceIndex;
            DiscountRate = elem.DiscountRate;
            PeriodOfAnalysis = elem.PeriodOfAnalysis;
        }

        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            AddRule(nameof(SurveyedYear), () => SurveyedYear <= DateTime.Now.Year, "The Surveyed Year must not be in the future.");
            AddRule(nameof(UpdatedYear), () => UpdatedYear <= DateTime.Now.Year, "The Updated Year must not be in the future.");
            AddRule(nameof(UpdatedYear), () => UpdatedYear >= SurveyedYear, "The Updated Year must happen after the Surveyed Year.");
        }
        private FdaValidationResult Validate()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if(DiscountRate<0 || DiscountRate>100)
            {
                vr.AddErrorMessage("Discount Rate must be between 0 and 100.");
            }
            if(PeriodOfAnalysis<0 || PeriodOfAnalysis>500)
            {
                vr.AddErrorMessage("Period of Analysis must be between 0 and 500.");
            }
            return vr;

        }
        public override void Save()
        {
            FdaValidationResult result = Validate();
            if (!result.IsValid)
            {
                MessageBox.Show(result.ErrorMessage.ToString(), "Cannot Save", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                //the properties are unique in that it gets saved when the study is created. This editor
                //is, therefore, always in 'edit mode'. We are always saving an existing element.
                StudyPropertiesElement elemToSave = new StudyPropertiesElement(StudyName, StudyPath, StudyDescription, CreatedBy,
                    CreatedDate, StudyNotes, MonetaryUnit, UnitSystem, SurveyedYear, UpdatedYear, UpdatedPriceIndex, DiscountRate, PeriodOfAnalysis);

                Saving.PersistenceManagers.StudyPropertiesPersistenceManager manager = Saving.PersistenceFactory.GetStudyPropertiesManager();
                manager.SaveExisting(CurrentElement, elemToSave);
            }
        }
        #endregion        
        #region Functions
        #endregion
    }
}
