using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Study
{
    public class PropertiesVM : BaseViewModel
    {
        #region Notes
        #endregion
        #region Fields
        public static readonly string TableName = "Study Properties";
        private readonly string _StudyName;
        private readonly string _StudyPath;
        private string _StudyDescription;
        private readonly string _CreatedBy;
        private readonly string _CreatedDate;
        private string _StudyNotes;
        private MonetaryUnitsEnum _MonetaryUnit;
        private UnitsSystemEnum _UnitSystem;
        private int _SurveyedYear;
        private int _UpdatedYear;
        private Single _UpdatedPriceIndex;
        #endregion
        #region Properties
        public string StudyName {
            get { return _StudyName; }
        }
        public string StudyPath {
            get { return _StudyPath; }
        }
        public string StudyDescription {
            get { return _StudyDescription; }
            set { _StudyDescription = value;  NotifyPropertyChanged(); }
        }
        public string CreatedBy {
            get { return _CreatedBy; }
        }
        public string CreatedDate {
            get { return _CreatedDate; }
        }
        public string StudyNotes
        {
            get { return _StudyNotes; }
            set
            {
                _StudyNotes = value;
                NotifyPropertyChanged();
            }
        }
        public MonetaryUnitsEnum MonetaryUnit
        {
            get { return _MonetaryUnit; }
            set
            {
                _MonetaryUnit = value;
                NotifyPropertyChanged();
            }
        }
        public UnitsSystemEnum UnitSystem
        {
            get { return _UnitSystem; }
            set
            {
                _UnitSystem = value;
                NotifyPropertyChanged();
            }
        }
        public int SurveyedYear
        {
            get { return _SurveyedYear; }
            set
            {
                _SurveyedYear = value;
                NotifyPropertyChanged();
            }
        }
        public int UpdatedYear
        {
            get { return _UpdatedYear; }
            set
            {
                _UpdatedYear = value;
                NotifyPropertyChanged();
                //NotifyPropertyChanged(nameof(UpdatedPriceIndex));
            }
        }
        public Single UpdatedPriceIndex
        {
            get { return _UpdatedPriceIndex; }
            set
            {
                _UpdatedPriceIndex = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #region Constructors
        public PropertiesVM():base()
        {
            _StudyName = "Example Study Name";
            _StudyPath = "C:\\Temp\\FDA";
            _StudyDescription = "Example Study Description";
            _CreatedBy = System.Environment.UserName;
            _CreatedDate = DateTime.Now.ToShortDateString();
            _StudyNotes = "These are my notes";
            _MonetaryUnit = MonetaryUnitsEnum.Millions;
            _UnitSystem = UnitsSystemEnum.English;
            _SurveyedYear = DateTime.Now.Year - 1;
            _UpdatedYear = DateTime.Now.Year;
            _UpdatedPriceIndex = 0.01f;
        }
        public PropertiesVM(string studyName, string studyPath):base()
        {
            _StudyName = studyName;
            _StudyPath = studyPath;
            _StudyDescription = "";
            _CreatedBy = System.Environment.UserName;
            _CreatedDate = DateTime.Now.ToShortDateString();
            _StudyNotes = "";
            _MonetaryUnit = MonetaryUnitsEnum.Millions;
            _UnitSystem = UnitsSystemEnum.English;
            _SurveyedYear = DateTime.Now.Year - 1;
            _UpdatedYear = DateTime.Now.Year;
            _UpdatedPriceIndex = 0.01f;
        }
        public PropertiesVM(DataBase_Reader.DataTableView tbl)
        {
            if(tbl.TableName != TableName)
            {
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("Table name does not match.", FdaModel.Utilities.Messager.ErrorMessageEnum.Fatal & FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel));
            }else
            {
                object[] col = tbl.GetColumn("Value");
                //would rather this to be a bit safer...
                _StudyName = (string)col[0];
                _StudyPath = (string)col[1];
                _StudyDescription = (string)col[2];
                _CreatedBy = (string)col[3];
                _CreatedDate = (string)col[4];
                _StudyNotes = (string)col[5];
                if(!Enum.TryParse((string)col[6], out _MonetaryUnit))
                {
                    ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("Monetary unit did not match, setting to dollars.", FdaModel.Utilities.Messager.ErrorMessageEnum.Report & FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel));
                    _MonetaryUnit = MonetaryUnitsEnum.Dollars;
                }
                _SurveyedYear = Convert.ToInt32((string)col[7]);
                _UpdatedYear = Convert.ToInt32((string)col[8]);
                _UpdatedPriceIndex = Convert.ToSingle((string)col[9]);
            }
        }
        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            AddRule(nameof(SurveyedYear), () => SurveyedYear <= DateTime.Now.Year, "The Surveyed Year must not be in the future.");
            AddRule(nameof(UpdatedYear), () => UpdatedYear <= DateTime.Now.Year, "The Updated Year must not be in the future.");
            AddRule(nameof(UpdatedYear), () => UpdatedYear >= SurveyedYear, "The Updated Year must happen after the Surveyed Year.");
        }
        public  void Save()
        {
            if (!Storage.Connection.Instance.TableNames().Contains(TableName))
            {
                string[] names = new string[2];
                names[0] = "Parameter";
                names[1] = "Value";
                Type[] types = new Type[2];
                types[0] = typeof(string);
                types[1] = typeof(string);
                Storage.Connection.Instance.CreateTable(TableName, names, types);
                DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);
                tbl.AddRow(new object[] { "Study Name: ", StudyName });
                tbl.AddRow(new object[] { "Study Path: ", StudyPath });
                tbl.AddRow(new object[] { "Description: ", StudyDescription });
                tbl.AddRow(new object[] { "Created By: ", CreatedBy });
                tbl.AddRow(new object[] { "Created Date: ", CreatedDate});
                tbl.AddRow(new object[] { "Study Notes: ", StudyNotes });
                tbl.AddRow(new object[] { "Monetary Unit: ", MonetaryUnit });
                tbl.AddRow(new object[] { "Surveyed Year: ", SurveyedYear});
                tbl.AddRow(new object[] { "Updated Year: ", UpdatedYear });
                tbl.AddRow(new object[] { "Updated Price Index: ", UpdatedPriceIndex });
                tbl.ApplyEdits();
            }else
            {
                DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);
                tbl.EditCell(0, 1,StudyName);
                tbl.EditCell(1, 1,StudyPath);
                tbl.EditCell(2, 1, StudyDescription);
                tbl.EditCell(3, 1, CreatedBy);
                tbl.EditCell(4, 1,CreatedDate);
                tbl.EditCell(5, 1, StudyNotes);
                tbl.EditCell(6, 1, MonetaryUnit);
                tbl.EditCell(7, 1, SurveyedYear);
                tbl.EditCell(8, 1, UpdatedYear);
                tbl.EditCell(9, 1, UpdatedPriceIndex);
                if (!Storage.Connection.Instance.IsOpen) Storage.Connection.Instance.Open();
                tbl.ApplyEdits();
            }
        }
        #endregion        
        #region Functions
        #endregion
    }
}
