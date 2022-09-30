using System;
using System.Xml.Linq;
using HEC.FDA.Statistics.Convergence;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Study
{
    public class StudyPropertiesElement: ChildElement
    {
        private const string STUDY_PROPERTIES = "StudyProperties";
        private const string NAME = "Name";
        private const string PATH = "Path";
        private const string DESCRIPTION = "Description";
        private const string CREATED_BY = "CreatedBy";
        private const string CREATED_DATE = "CreatedDate";
        private const string STUDY_NOTES = "StudyNotes";
        private const string MONETARY_UNIT = "MonetaryUnit";
        private const string UNIT_SYSTEM = "UnitSystem";
        private const string SURVEYED_YEAR = "SurveyedYear";
        private const string UPDATED_YEAR = "UpdatedYear";
        private const string UPDATED_PRICE_INDEX = "UpdatedPriceIndex";
        private const string DISCOUNT_RATE = "DiscountRate";
        private const string PERIOD_OF_ANALYSIS = "PeriodOfAnalysis";
        private const string CONVERGENCE_CRITERIA = "ConvergenceCriteriaVM";


        #region Fields

        #endregion
        #region Properties
        public ConvergenceCriteriaVM ConvergenceCriteria { get; }

        public double DiscountRate { get; }
        
        public int PeriodOfAnalysis { get; }
  
        public string StudyPath { get; }
             
        public string CreatedBy { get; }

        public string CreatedDate { get; }

        public string StudyNotes { get; }

        public MonetaryUnitsEnum MonetaryUnit { get; }

        public UnitsSystemEnum UnitSystem { get; }

        public int SurveyedYear { get; }

        public int UpdatedYear { get; }

        public double UpdatedPriceIndex { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// ctor when creating the default study properties. This should only get called once for the
        /// life of the study. It gets created when the study gets created and then should only get updated.
        /// </summary>
        /// <param name="studyName"></param>
        /// <param name="studyPath"></param>
        /// <param name="description"></param>
        public StudyPropertiesElement(string studyName, string studyPath, string description,ConvergenceCriteriaVM convCriteria, int id)
            :base(studyName, "", description, id)
        {
            Name = studyName;
            StudyPath = studyPath;
            Description = description;
            CreatedBy = Environment.UserName;
            CreatedDate = DateTime.Now.ToShortDateString();
            StudyNotes = "";
            MonetaryUnit = MonetaryUnitsEnum.Millions;
            UnitSystem = UnitsSystemEnum.English;
            SurveyedYear = DateTime.Now.Year - 1;
            UpdatedYear = DateTime.Now.Year;
            UpdatedPriceIndex = 0.01;
            DiscountRate = .025;
            PeriodOfAnalysis = 50;
            ConvergenceCriteria = convCriteria;
        }

        public StudyPropertiesElement(string name, string path, string description, string createdBy, string createdDate, string studyNotes,
            MonetaryUnitsEnum monetaryUnits, UnitsSystemEnum unitSystem, int surveyedYear, int updatedYear, double priceIndex, double discountRate, 
            int periodOfAnalysis, ConvergenceCriteriaVM convCriteria, int id) 
            : base(name, createdDate, description, id)
        {
            Name = name;
            StudyPath = path;
            Description = description;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
            StudyNotes = studyNotes;
            MonetaryUnit = monetaryUnits;
            UnitSystem = unitSystem;
            SurveyedYear = surveyedYear;
            UpdatedYear = updatedYear;
            UpdatedPriceIndex = priceIndex;
            DiscountRate = discountRate;
            PeriodOfAnalysis = periodOfAnalysis;
            ConvergenceCriteria = convCriteria;
        }
       
        public StudyPropertiesElement(StudyPropertiesElement elem, int id):base(elem.Name, elem.LastEditDate, elem.Description, id)
        {
            Name = elem.Name;
            StudyPath = elem.StudyPath;
            Description = elem.Description;
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
            ConvergenceCriteria = elem.ConvergenceCriteria;
        }

        /// <summary>
        /// ctor used to create an element from the database
        /// </summary>
        /// <param name="xml"></param>
        public StudyPropertiesElement(XElement studyProperty, int id):base(studyProperty, id)
        {
            StudyPath = studyProperty.Attribute(PATH).Value;
            CreatedBy = studyProperty.Attribute(CREATED_BY).Value;
            CreatedDate = studyProperty.Attribute(CREATED_DATE).Value;
            StudyNotes = studyProperty.Attribute(STUDY_NOTES).Value;
            if(studyProperty.Element(CONVERGENCE_CRITERIA) != null)
            {
                ConvergenceCriteria = new ConvergenceCriteriaVM( studyProperty.Element(CONVERGENCE_CRITERIA));
            }
            else
            {
                //this is for backwards compatibility
                ConvergenceCriteria = new ConvergenceCriteriaVM();
            }

            MonetaryUnitsEnum monetaryUnitsEnum = MonetaryUnitsEnum.Dollars;
            if (!Enum.TryParse(studyProperty.Attribute(MONETARY_UNIT).Value, out monetaryUnitsEnum))
            {
                MonetaryUnit = MonetaryUnitsEnum.Dollars;
            }
            else
            {
                MonetaryUnit = monetaryUnitsEnum;
            }

            UnitsSystemEnum unitSystemEnum = UnitsSystemEnum.English;
            if (!Enum.TryParse(studyProperty.Attribute(UNIT_SYSTEM).Value, out unitSystemEnum))
            {
                UnitSystem = UnitsSystemEnum.English;
            }
            else
            {
                UnitSystem = unitSystemEnum;
            }

            SurveyedYear = Convert.ToInt32(studyProperty.Attribute(SURVEYED_YEAR).Value);
            UpdatedYear = Convert.ToInt32(studyProperty.Attribute(UPDATED_YEAR).Value);
            UpdatedPriceIndex = Convert.ToDouble(studyProperty.Attribute(UPDATED_PRICE_INDEX).Value);
            DiscountRate = Convert.ToDouble(studyProperty.Attribute(DISCOUNT_RATE).Value);
            PeriodOfAnalysis = Convert.ToInt32(studyProperty.Attribute(PERIOD_OF_ANALYSIS).Value);
        }

        public override XElement ToXML()
        {
            XElement studyPropsElem = new XElement(STUDY_PROPERTIES);

            studyPropsElem.Add(CreateHeaderElement());

            studyPropsElem.SetAttributeValue(PATH, StudyPath);
            studyPropsElem.SetAttributeValue(CREATED_BY, CreatedBy);
            studyPropsElem.SetAttributeValue(CREATED_DATE, CreatedDate);
            studyPropsElem.SetAttributeValue(STUDY_NOTES, StudyNotes);
            studyPropsElem.SetAttributeValue(MONETARY_UNIT, MonetaryUnit);
            studyPropsElem.SetAttributeValue(UNIT_SYSTEM, UnitSystem);
            studyPropsElem.SetAttributeValue(SURVEYED_YEAR, SurveyedYear);
            studyPropsElem.SetAttributeValue(UPDATED_YEAR, UpdatedYear);
            studyPropsElem.SetAttributeValue(UPDATED_PRICE_INDEX, UpdatedPriceIndex);
            studyPropsElem.SetAttributeValue(DISCOUNT_RATE, DiscountRate);
            studyPropsElem.SetAttributeValue(PERIOD_OF_ANALYSIS, PeriodOfAnalysis);

            studyPropsElem.Add(ConvergenceCriteria.ToXML());

            return studyPropsElem;
        }

        public ConvergenceCriteria GetStudyConvergenceCriteria()
        {
            return ConvergenceCriteria.ToConvergenceCriteria();
        }
        #endregion
    }
}
