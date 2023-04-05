using System;

namespace HEC.FDA.ViewModel.Study
{
    public class PropertiesVM : Editors.BaseEditorVM
    {
        #region Notes
        #endregion

        private int _SurveyedYear;
        private double _DiscountRate;

        #region Properties
        public double DiscountRate
        {
            get { return _DiscountRate; }
            set { _DiscountRate = value; NotifyPropertyChanged(); }
        }
        public int PeriodOfAnalysis { get; set; }
        public string StudyName { get; set; }
        public string StudyPath { get; set; }
        public string StudyDescription { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string StudyNotes { get; set; }
        public MonetaryUnitsEnum MonetaryUnit { get; set; }
        public UnitsSystemEnum UnitSystem { get; set; }
        public int SurveyedYear
        {
            get { return _SurveyedYear; }
            set { _SurveyedYear = value; NotifyPropertyChanged(); }
        }
        public int UpdatedYear { get; set; }
        public double UpdatedPriceIndex { get; set; }

        public ConvergenceCriteriaVM ConvergenceCriteria { get; set; }
        public ProjectionPickerVM ProjectionPicker { get; set; } = new ProjectionPickerVM();

        #endregion
        #region Constructors      

        public PropertiesVM(StudyPropertiesElement elem):base(elem, null)
        {
            ConvergenceCriteria = new ConvergenceCriteriaVM(elem.ConvergenceCriteria.ToXML());
            RegisterChildViewModel(ConvergenceCriteria);

            StudyName = elem.Name;
            StudyPath = elem.StudyPath;
            StudyDescription = elem.Description;
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
            AddRule(nameof(DiscountRate), () => DiscountRate >= 0 && DiscountRate <= 100, "Discount Rate must be between 0 and 100.");
            AddRule(nameof(PeriodOfAnalysis), () => PeriodOfAnalysis >= 0 && PeriodOfAnalysis <= 500, "Period of Analysis must be between 0 and 500.");
        }

        public override void Save()
        {
            //the properties are unique in that it gets saved when the study is created. This editor
            //is, therefore, always in 'edit mode'. We are always saving an existing element.
            //there is only one row of study properties. The id will always be 1.
            int id = 1;
            StudyPropertiesElement elemToSave = new StudyPropertiesElement(StudyName, StudyPath, StudyDescription, CreatedBy,
                CreatedDate, StudyNotes, MonetaryUnit, UnitSystem, SurveyedYear, UpdatedYear, UpdatedPriceIndex, DiscountRate, PeriodOfAnalysis, ConvergenceCriteria, id);
            ProjectionPicker.Save(); 
            base.Save(elemToSave);
        }
        #endregion        
    }
}
