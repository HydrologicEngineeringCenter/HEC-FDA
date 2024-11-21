using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows;
using System.Xml.Linq;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario.Editor;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactAreaScenario
{
    public class IASElement : ChildElement
    {
        #region Fields
        public const string IAS_SET = "IASSet";
        public const string NAME = "Name";
        public const string DESCRIPTION = "Description";
        public const string YEAR = "Year";
        public const string LAST_EDIT_DATE = "LastEditDate";
        public const string STAGE_DAMAGE_ID = "StageDamageID";
        public const string NON_FAILURE_STAGE_DAMAGE_ID = "NonFailureStageDamageID";
        public const string HAS_NON_FAILURE_STAGE_DAMAGE = "HasNonFailureStageDamage";

        private string _AnalysisYear;
        private int _StageDamageID;
        private int _NonFailureStageDamageID;

        #endregion

        #region Properties
        public bool UpdateComputeDate { get; set; }
        public ScenarioResults Results{get; set;}

        public string AnalysisYear
        {
            get { return _AnalysisYear; }
            set { _AnalysisYear = value; NotifyPropertyChanged(); }
        }

        public int StageDamageID
        {
            get { return _StageDamageID; }
            set { _StageDamageID = value; NotifyPropertyChanged(); }
        }
        public int NonFailureStageDamageID
        {
            get { return _NonFailureStageDamageID; }
            set { _NonFailureStageDamageID = value; NotifyPropertyChanged(); }
        }
        public bool HasNonFailureStageDamage
        {
            get;set;
        }

        public List<SpecificIAS> SpecificIASElements { get; } = new List<SpecificIAS>();

        #endregion
        #region Constructors

        public IASElement(string name, string description, string creationDate, string year, int stageDamageElementID, int nonFailureStageDamageID, bool hasNonFailureStageDamage, List<SpecificIAS> elems, int id) 
            : base(name, creationDate, description, id)
        {
            StageDamageID = stageDamageElementID;
            NonFailureStageDamageID = nonFailureStageDamageID;
            HasNonFailureStageDamage = hasNonFailureStageDamage;
            SpecificIASElements.AddRange( elems);
            AnalysisYear = year;

            AddActions();
        }

        /// <summary>
        /// The ctor used to load an element set from the database.
        /// </summary>
        /// <param name="xml"></param>
        public IASElement(XElement setElem, int id) : base(setElem,id)
        {
           
            AnalysisYear = setElem.Attribute(YEAR).Value;
            StageDamageID = int.Parse(setElem.Attribute(STAGE_DAMAGE_ID).Value);
            //the non-failure stuff is a new addition. Check that it exists first for backwards compatibility.
            XAttribute nonFailureID = setElem.Attribute(NON_FAILURE_STAGE_DAMAGE_ID);
            if(nonFailureID != null)
            {
                NonFailureStageDamageID = int.Parse(nonFailureID.Value);
                HasNonFailureStageDamage = Boolean.Parse(setElem.Attribute(HAS_NON_FAILURE_STAGE_DAMAGE).Value);
            }
            else
            {
                NonFailureStageDamageID = -1;
                HasNonFailureStageDamage = false;
            }

            IEnumerable<XElement> iasElements = setElem.Elements("IAS");
            foreach(XElement elem in iasElements)
            {
                SpecificIASElements.Add(new SpecificIAS(elem));
            }

            XElement resultsElem = setElem.Element("ScenarioResults");
            if(resultsElem != null)
            {
                Results = ScenarioResults.ReadFromXML(resultsElem);
            }

            AddActions();
            IASTooltipHelper.UpdateTooltip(this);
        }

        private void AddActions()
        {
            AddDefaultActions(EditIASSet, StringConstants.EDIT_SCENARIO_MENU);
            NamedAction compute = new()
            {
                Header = StringConstants.COMPUTE_SCENARIO_MENU,
                Action = ComputeScenario
            };

            NamedAction viewResults = new()
            {
                Header = StringConstants.VIEW_RESULTS_MENU,
                Action = ViewResults
            };

            NamedAction viewThresholds = new()
            {
                Header = StringConstants.VIEW_THRESHOLDS_MENU,
                Action = ViewThresholds
            };

            Actions.Insert(1, viewThresholds);
            Actions.Insert(1, viewResults);
            Actions.Insert(1, compute);
        }

        /// <summary>
        /// Opens the conditions editor.
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public void EditIASSet(object arg1, EventArgs arg2)
        {
            EditorActionManager actionManager = new EditorActionManager()
               .WithSiblingRules(this);
            IASEditorVM vm = new(this, actionManager);
            vm.RequestNavigation += Navigate;

            string header = "Edit Impact Area Scenario";
            DynamicTabVM tab = new(header, vm, "EditIAS" + Name);
            Navigate(tab, false, false);
        }

        private static List<ImpactAreaRowItem> GetStudyImpactAreaRowItems()
        {
            List<ImpactAreaRowItem> impactAreaRows = new();
            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impactAreaElements.Count > 0)
            {
                impactAreaRows = impactAreaElements[0].ImpactAreaRows;
            }
            return impactAreaRows;
        }

        public List<SpecificIASResultVM> GetResults()
        {
            List<SpecificIASResultVM> results = new();
            if (Results != null)
            {
                List<string> damCats = Results.GetDamageCategories();
                List<ImpactAreaRowItem> impactAreaRows = GetStudyImpactAreaRowItems();
                foreach (SpecificIAS ias in SpecificIASElements)
                {
                    int impactAreaID = ias.ImpactAreaID;
                    string impactAreaName = GetImpactAreaNameFromID(impactAreaRows, impactAreaID);
                    if (impactAreaName != null)
                    {
                        SpecificIASResultVM result = new(impactAreaName, impactAreaID, Results, damCats);
                        results.Add(result);
                    }
                }
            }
            return results;
        }
        public static Dictionary<int,string> GetImpactAreaNamesFromIDs()
        {
            List<ImpactAreaRowItem> impactAreaRows = GetStudyImpactAreaRowItems();
            Dictionary<int, string> impactAreaNames = [];
            foreach (ImpactAreaRowItem row in impactAreaRows)
            {
                impactAreaNames.Add(row.ID, row.Name);  
            }
            return impactAreaNames;
        }
        private static string GetImpactAreaNameFromID(List<ImpactAreaRowItem> rows, int id)
        {
            string rowName = null;
            foreach(ImpactAreaRowItem row in rows)
            {
                if(row.ID == id)
                {
                    rowName = row.Name;
                    break;
                }
            }
            return rowName;
        }

        private void ViewThresholds(object arg1, EventArgs arg2)
        {
            ViewThresholdsVM vm = new(this);
            string header = "Thresholds for " + Name;
            DynamicTabVM tab = new(header, vm, "ThresholdsFor" + Name);
            Navigate(tab, false, false);
        }

        private void ViewResults(object arg1, EventArgs arg2)
        {          
            List<SpecificIASResultVM> results = GetResults();
            if (results.Count>0)
            {
                ScenarioResultsVM resultViewer = new(results);
                string header = "Results for " + Name;
                DynamicTabVM tab = new(header, resultViewer, "resultViewer" + Name + DateTime.Now); //this name has to be unique or else we'll just keep pulling the same tab every time even if we have recomputed results. Cost me 2 whole days. 
                Navigate(tab, false, false);
            }
            else
            {
                MessageBox.Show("There are no results to display.", "No Results", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        public FdaValidationResult CanCompute()
        {
            FdaValidationResult vr = new();

            if (SpecificIASElements.Count > 0)
            {
                foreach (SpecificIAS ias in SpecificIASElements)
                {
                    FdaValidationResult canComputeScenario = ias.CanComputeScenario();
                    if (!canComputeScenario.IsValid)
                    {
                        vr.AddErrorMessage(canComputeScenario.ErrorMessage);
                    }
                }
            }
            return vr;
        }

        private void ComputeScenario(object arg1, EventArgs arg2)
        {
            ComputeScenarioVM vm = new(this, ComputeCompleted);
            string header = "Compute Log For Scenario: " + Name;
            DynamicTabVM tab = new(header, vm, "ComputeLog" + Name);
            Navigate(tab, false, false);
        }
        private void ComputeCompleted(IASElement elem, ScenarioResults results)
        {
            Results = results;
            Application.Current.Dispatcher.Invoke(
            (Action)(() => 
            {
                this.UpdateComputeDate = true;
                PersistenceFactory.GetIASManager().SaveExisting(this);
                this.UpdateComputeDate = false;
                IASTooltipHelper.UpdateTooltip(this);
                MessageBoxResult messageBoxResult = MessageBox.Show("Compute completed. Would you like to view the results?", Name + " Compute Complete", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    ViewResults(this, new EventArgs());
                }
                
            }));
        }
        #endregion

        public override XElement ToXML()
        {
            XElement setElement = new(IAS_SET);
            setElement.SetAttributeValue(YEAR, AnalysisYear);
            setElement.SetAttributeValue(STAGE_DAMAGE_ID, StageDamageID);
            setElement.SetAttributeValue(NON_FAILURE_STAGE_DAMAGE_ID, NonFailureStageDamageID);
            setElement.SetAttributeValue(HAS_NON_FAILURE_STAGE_DAMAGE, HasNonFailureStageDamage);

            setElement.Add(CreateHeaderElement());

            foreach(SpecificIAS elem in SpecificIASElements)
            {
                setElement.Add(elem.WriteToXML());
            }
            if(Results != null)
            {
                XElement resultsElem = Results.WriteToXML();
                setElement.Add(resultsElem);
            }
            return setElement;
        }

    }
}
