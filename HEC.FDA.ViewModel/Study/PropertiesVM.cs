using System;

namespace HEC.FDA.ViewModel.Study
{
    public class PropertiesVM : Editors.BaseEditorVM
    {
        #region Properties
        private double _DiscountRate;
        public double DiscountRate
        {
            get { return _DiscountRate; }
            set { _DiscountRate = value; NotifyPropertyChanged(); }
        }

        private int _PeriodOfAnalysis;
        public int PeriodOfAnalysis
        {
            get { return _PeriodOfAnalysis; }
            set { _PeriodOfAnalysis = value; NotifyPropertyChanged(); }
        }

        private string _StudyName;
        public string StudyName
        {
            get { return _StudyName; }
            set { _StudyName = value; NotifyPropertyChanged(); }
        }

        private string _StudyPath;
        public string StudyPath
        {
            get { return _StudyPath; }
            set { _StudyPath = value; NotifyPropertyChanged(); }
        }

        private string _StudyDescription;
        public string StudyDescription
        {
            get { return _StudyDescription; }
            set { _StudyDescription = value; NotifyPropertyChanged(); }
        }

        private string _CreatedBy;
        public string CreatedBy
        {
            get { return _CreatedBy; }
            set { _CreatedBy = value; NotifyPropertyChanged(); }
        }

        private string _CreatedDate;
        public string CreatedDate
        {
            get { return _CreatedDate; }
            set { _CreatedDate = value; NotifyPropertyChanged(); }
        }

        private string _StudyNotes;
        public string StudyNotes
        {
            get { return _StudyNotes; }
            set { _StudyNotes = value; NotifyPropertyChanged(); }
        }

        private int _SurveyedYear;
        public int SurveyedYear
        {
            get { return _SurveyedYear; }
            set { _SurveyedYear = value; NotifyPropertyChanged(); }
        }

        private int _UpdatedYear;
        public int UpdatedYear
        {
            get { return _UpdatedYear; }
            set { _UpdatedYear = value; NotifyPropertyChanged(); }
        }

        private double _UpdatedPriceIndex;
        public double UpdatedPriceIndex
        {
            get { return _UpdatedPriceIndex; }
            set { _UpdatedPriceIndex = value; NotifyPropertyChanged(); }
        }

        public ConvergenceCriteriaVM ConvergenceCriteria { get; set; }
        public ProjectionPickerVM ProjectionPicker { get; set; } = new ProjectionPickerVM();
        #endregion

        #region Constructors      
        public PropertiesVM(StudyPropertiesElement elem) : base(elem, null)
        {
            ConvergenceCriteria = new ConvergenceCriteriaVM(elem.ConvergenceCriteria.ToXML());
            RegisterChildViewModel(ConvergenceCriteria);

            StudyName = elem.Name;
            StudyPath = elem.StudyPath;
            StudyDescription = elem.Description;
            CreatedBy = elem.CreatedBy;
            CreatedDate = elem.CreatedDate;
            StudyNotes = elem.StudyNotes;

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
                CreatedDate, StudyNotes, SurveyedYear, UpdatedYear, UpdatedPriceIndex, DiscountRate, PeriodOfAnalysis, ConvergenceCriteria, id);
            ProjectionPicker.Save();
            base.Save(elemToSave);
        }
        #endregion        
    }
}
