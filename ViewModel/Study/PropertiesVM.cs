using System;
using System.Windows;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.ViewModel.Validation;
using HEC.MVVMFramework.Base.Enumerations;


namespace HEC.FDA.ViewModel.Study
{
    public class PropertiesVM : Editors.BaseEditorVM
    {
        #region Notes
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
            AddSinglePropertyRule(nameof(SurveyedYear), new Rule(() => { return SurveyedYear <= DateTime.Now.Year; }, "The Surveyed Year must not be in the future.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(UpdatedYear), new Rule(() => { return UpdatedYear <= DateTime.Now.Year; }, "The Updated Year must not be in the future.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(UpdatedYear), new Rule(() => { return UpdatedYear >= SurveyedYear; }, "The Updated Year must happen after the Surveyed Year.", ErrorLevel.Severe));
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
                int id = Saving.PersistenceFactory.GetStudyPropertiesManager().GetNextAvailableId();
                StudyPropertiesElement elemToSave = new StudyPropertiesElement(StudyName, StudyPath, StudyDescription, CreatedBy,
                    CreatedDate, StudyNotes, MonetaryUnit, UnitSystem, SurveyedYear, UpdatedYear, UpdatedPriceIndex, DiscountRate, PeriodOfAnalysis, id);

                Saving.PersistenceManagers.StudyPropertiesPersistenceManager manager = Saving.PersistenceFactory.GetStudyPropertiesManager();
                manager.SaveExisting( elemToSave);
            }
        }
        #endregion        
    }
}
