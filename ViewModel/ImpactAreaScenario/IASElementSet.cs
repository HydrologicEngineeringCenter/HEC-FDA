using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml.Linq;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;


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

        private string _Description = "";
        private int _AnalysisYear;

        private List<metrics.Results> _Results = new List<metrics.Results>();

        #endregion

        #region Properties
        public bool HasComputed { get; set; }

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

        public List<SpecificIAS> SpecificIASElements { get; } = new List<SpecificIAS>();

        #endregion
        #region Constructors

        public IASElementSet(string name, string description, string creationDate, int year, List<SpecificIAS> elems, int id) : base(id)
        {
            SpecificIASElements.AddRange( elems);
            Name = name;
            Description = description;
            AnalysisYear = year;

            CustomTreeViewHeader = new CustomHeaderVM(Name)
            {
                ImageSource = ImageSources.SCENARIO_IMAGE,
                Tooltip = StringConstants.CreateChildNodeTooltip(creationDate)
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
            LastEditDate = setElem.Attribute(LAST_EDIT_DATE).Value;

            IEnumerable<XElement> iasElements = setElem.Elements("IAS");
            foreach(XElement elem in iasElements)
            {
                SpecificIASElements.Add(new SpecificIAS(elem));
            }

            CustomTreeViewHeader = new CustomHeaderVM(Name)
            {
                ImageSource = ImageSources.SCENARIO_IMAGE,
                Tooltip = StringConstants.CreateChildNodeTooltip(LastEditDate)
            };
            AddActions();
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
            Editor.IASEditorVM vm = new Editor.IASEditorVM(this, actionManager);
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

        private List<SpecificIASResultVM> GetResults()
        {
            List<SpecificIASResultVM> results = new List<SpecificIASResultVM>();

            ObservableCollection<ImpactAreaRowItem> impactAreaRows = GetStudyImpactAreaRowItems();
            int i = 0;
            foreach (SpecificIAS ias in SpecificIASElements)
            {
                int impactAreaID = ias.ImpactAreaID;
                string impactAreaName = GetImpactAreaNameFromID(impactAreaRows, impactAreaID);
                if (impactAreaName != null)
                {
                    SpecificIASResultVM result = new SpecificIASResultVM(impactAreaName, ias.Thresholds, _Results[i]);
                    results.Add(result);
                }
                i++;
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
            if (results.Count > 0)
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
            HasComputed = true;
            foreach(SpecificIAS ias in SpecificIASElements)
            {
                _Results.Add( ias.ComputeScenario(arg1, arg2));
            }
            //todo: i am just saving here to trigger the update event. Once we have the real compute we will want to save the results.
            PersistenceFactory.GetIASManager().SaveExisting(this);
        }
        #endregion

        #region Functions
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            IASElementSet elem = (IASElementSet)elementToClone;
            IASElementSet newElem = new IASElementSet(elem.Name, elem.Description, elem.LastEditDate, elem.AnalysisYear, elem.SpecificIASElements, elem.ID);
            return newElem;
        }

        #endregion

        public XElement WriteToXML()
        {
            XElement setElement = new XElement(IAS_SET);
            setElement.SetAttributeValue(NAME, Name);
            setElement.SetAttributeValue(DESCRIPTION, Description);
            setElement.SetAttributeValue(YEAR, AnalysisYear);
            setElement.SetAttributeValue(LAST_EDIT_DATE, LastEditDate);

            foreach(SpecificIAS elem in SpecificIASElements)
            {
                setElement.Add(elem.WriteToXML());
            }

            return setElement;
        }

    }
}
