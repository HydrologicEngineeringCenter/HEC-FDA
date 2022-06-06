using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml.Linq;
using compute;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario.Editor;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;
using metrics;

namespace HEC.FDA.ViewModel.ImpactAreaScenario
{
    public class IASElementSet : ChildElement
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

        public IASElementSet(string name, string description, string creationDate, int year, int stageDamageElementID, List<SpecificIAS> elems, int id) : base(id)
        {
            StageDamageID = stageDamageElementID;
            SpecificIASElements.AddRange( elems);
            Name = name;
            Description = description;
            AnalysisYear = year;

            CustomTreeViewHeader = new CustomHeaderVM(Name)
            {
                ImageSource = ImageSources.SCENARIO_IMAGE,
                Tooltip = StringConstants.CreateLastEditTooltip(creationDate)
            };

            AddActions();
        }

        /// <summary>
        /// The ctor used to load an element set from the database.
        /// </summary>
        /// <param name="xml"></param>
        public IASElementSet(string xml, int id) : base(id)
        {
            XDocument doc = XDocument.Parse(xml);
            XElement setElem = doc.Element(IAS_SET);
            Name = setElem.Attribute(NAME).Value;
            Description = setElem.Attribute(DESCRIPTION).Value;
            AnalysisYear = Int32.Parse(setElem.Attribute(YEAR).Value);
            StageDamageID = Int32.Parse(setElem.Attribute(STAGE_DAMAGE_ID).Value);
            LastEditDate = setElem.Attribute(LAST_EDIT_DATE).Value;

            IEnumerable<XElement> iasElements = setElem.Elements("IAS");
            foreach(XElement elem in iasElements)
            {
                SpecificIASElements.Add(new SpecificIAS(elem));
            }

            CustomTreeViewHeader = new CustomHeaderVM(Name)
            {
                ImageSource = ImageSources.SCENARIO_IMAGE,
                Tooltip = StringConstants.CreateLastEditTooltip(LastEditDate)
            };
            AddActions();
        }

        private bool HaveAllIASComputed()
        {
            bool allComputed = true;
            if(SpecificIASElements.Count == 0)
            {
                allComputed = false;
            }
            foreach(SpecificIAS ias in SpecificIASElements)
            {
                if(ias.ComputeResults == null)
                {
                    allComputed = false;
                    break;
                }
            }
            return allComputed;
        }

        private void AddActions()
        {
            NamedAction edit = new NamedAction();
            edit.Header = StringConstants.EDIT_SCENARIO_MENU;
            edit.Action = EditIASSet;

            NamedAction compute = new NamedAction();
            compute.Header = StringConstants.COMPUTE_SCENARIO_MENU;
            compute.Action = ComputeScenario;

            NamedAction viewResults = new NamedAction();
            viewResults.Header = StringConstants.VIEW_RESULTS_MENU;
            viewResults.Action = ViewResults;

            NamedAction removeCondition = new NamedAction();
            removeCondition.Header = StringConstants.REMOVE_MENU;
            removeCondition.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = StringConstants.RENAME_MENU;
            renameElement.Action = Rename;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(edit);
            localActions.Add(compute);
            localActions.Add(viewResults);
            localActions.Add(removeCondition);
            localActions.Add(renameElement);

            Actions = localActions;
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
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditIAS");
            Navigate(tab, false, false);
        }

        private ObservableCollection<ImpactAreaRowItem> GetStudyImpactAreaRowItems()
        {
            ObservableCollection<ImpactAreaRowItem> impactAreaRows = new ObservableCollection<ImpactAreaRowItem>();
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

            ObservableCollection<ImpactAreaRowItem> impactAreaRows = GetStudyImpactAreaRowItems();
            foreach (SpecificIAS ias in SpecificIASElements)
            {
                int impactAreaID = ias.ImpactAreaID;
                string impactAreaName = GetImpactAreaNameFromID(impactAreaRows, impactAreaID);
                if (impactAreaName != null && ias.ComputeResults != null)
                {
                    SpecificIASResultVM result = new SpecificIASResultVM(impactAreaName, ias.Thresholds, ias.ComputeResults);
                    results.Add(result);
                }
            }

            return results;
        }

        public ScenarioResults GetScenarioResults()
        {
            ScenarioResults results = new ScenarioResults();
            foreach (SpecificIAS ias in SpecificIASElements)
            {
                results.AddResults( ias.ComputeResults);
            }
            return results;
        }

        public List<ImpactAreaScenarioSimulation> GetSimulations()
        {
            List<ImpactAreaScenarioSimulation> results = new List<ImpactAreaScenarioSimulation>();
            foreach (SpecificIAS ias in SpecificIASElements)
            {
                results.Add(ias.Simulation);
            }
            return results;
        }

        private string GetImpactAreaNameFromID(ObservableCollection<ImpactAreaRowItem> rows, int id)
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
                DynamicTabVM tab = new DynamicTabVM(header, resultViewer, "resultViewer");
                Navigate(tab, false, false);
            }
            else
            {
                MessageBox.Show("There are no results to display.", "No Results", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void ComputeScenario(object arg1, EventArgs arg2)
        {
            ComputeScenarioVM vm = new ComputeScenarioVM(SpecificIASElements, ComputeCompleted);
            string header = "Compute Scenario";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ComputeScenario");
            Navigate(tab, false, false);
        }
        private void ComputeCompleted()
        {
            Application.Current.Dispatcher.Invoke(
            (Action)(() => 
            { 
                PersistenceFactory.GetIASManager().SaveExisting(this);
                MessageBox.Show("Compute Completed");
            }));
        }
        #endregion

        #region Functions
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            IASElementSet elem = (IASElementSet)elementToClone;
            IASElementSet newElem = new IASElementSet(elem.Name, elem.Description, elem.LastEditDate, elem.AnalysisYear,elem.StageDamageID, elem.SpecificIASElements, elem.ID);
            return newElem;
        }

        #endregion

        public XElement WriteToXML()
        {
            XElement setElement = new XElement(IAS_SET);
            setElement.SetAttributeValue(NAME, Name);
            setElement.SetAttributeValue(DESCRIPTION, Description);
            setElement.SetAttributeValue(YEAR, AnalysisYear);
            setElement.SetAttributeValue(STAGE_DAMAGE_ID, StageDamageID);

            setElement.SetAttributeValue(LAST_EDIT_DATE, LastEditDate);

            foreach(SpecificIAS elem in SpecificIASElements)
            {
                setElement.Add(elem.WriteToXML());
            }

            return setElement;
        }

    }
}
