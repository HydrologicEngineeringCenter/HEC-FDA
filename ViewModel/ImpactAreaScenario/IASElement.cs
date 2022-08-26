using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml.Linq;
using compute;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario.Editor;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;
using metrics;

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

        private string _Description = "";
        private int _AnalysisYear;
        private int _StageDamageID;

        #endregion

        #region Properties
        public ScenarioResults Results{get; set;}
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }

        public int AnalysisYear
        {
            get { return _AnalysisYear; }
            set { _AnalysisYear = value; NotifyPropertyChanged(); }
        }

        public int StageDamageID
        {
            get { return _StageDamageID; }
            set { _StageDamageID = value; NotifyPropertyChanged(); }
        }

        public List<SpecificIAS> SpecificIASElements { get; } = new List<SpecificIAS>();

        #endregion
        #region Constructors

        public IASElement(string name, string description, string creationDate, int year, int stageDamageElementID, List<SpecificIAS> elems, int id) 
            : base(name, creationDate, description, id)
        {
            StageDamageID = stageDamageElementID;
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

            AnalysisYear = int.Parse(setElem.Attribute(YEAR).Value);
            StageDamageID = int.Parse(setElem.Attribute(STAGE_DAMAGE_ID).Value);

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
        }

        private void AddActions()
        {
            AddDefaultActions(EditIASSet);
            NamedAction compute = new NamedAction();
            compute.Header = StringConstants.COMPUTE_SCENARIO_MENU;
            compute.Action = ComputeScenario;
            NamedAction viewResults = new NamedAction();
            viewResults.Header = StringConstants.VIEW_RESULTS_MENU;
            viewResults.Action = ViewResults;
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
            IASEditorVM vm = new IASEditorVM(this, actionManager);
            vm.RequestNavigation += Navigate;

            string header = "Edit Impact Area Scenario";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditIAS" + Name);
            Navigate(tab, false, false);
        }

        private List<ImpactAreaRowItem> GetStudyImpactAreaRowItems()
        {
            List<ImpactAreaRowItem> impactAreaRows = new List<ImpactAreaRowItem>();
            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impactAreaElements.Count > 0)
            {
                impactAreaRows = impactAreaElements[0].ImpactAreaRows;
            }
            return impactAreaRows;
        }

        public List<SpecificIASResultVM> GetResults()
        {
            List<SpecificIASResultVM> results = new List<SpecificIASResultVM>();
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
                        SpecificIASResultVM result = new SpecificIASResultVM(impactAreaName, impactAreaID, Results, damCats);
                        results.Add(result);
                    }
                }
            }
            return results;
        }

        private string GetImpactAreaNameFromID(List<ImpactAreaRowItem> rows, int id)
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

        private void ViewResults(object arg1, EventArgs arg2)
        {          
            List<SpecificIASResultVM> results = GetResults();
            if (results.Count>0)
            {
                IASResultsVM resultViewer = new IASResultsVM(results);
                string header = "Results for " + Name;
                DynamicTabVM tab = new DynamicTabVM(header, resultViewer, "resultViewer" + Name);
                Navigate(tab, false, false);
            }
            else
            {
                MessageBox.Show("There are no results to display.", "No Results", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void ComputeScenario(object arg1, EventArgs arg2)
        {
            ComputeScenarioVM vm = new ComputeScenarioVM(AnalysisYear, SpecificIASElements, ComputeCompleted);
            string header = "Compute Log For Scenario: " + Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ComputeLog" + Name);
            Navigate(tab, false, false);
        }
        private void ComputeCompleted(ScenarioResults results)
        {
            Results = results;
            Application.Current.Dispatcher.Invoke(
            (Action)(() => 
            {
                PersistenceFactory.GetIASManager().SaveExisting(this);
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
            XElement setElement = new XElement(IAS_SET);
            setElement.SetAttributeValue(YEAR, AnalysisYear);
            setElement.SetAttributeValue(STAGE_DAMAGE_ID, StageDamageID);

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
