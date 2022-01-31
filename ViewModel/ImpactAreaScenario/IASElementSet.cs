using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using ViewModel.ImpactArea;
using ViewModel.ImpactAreaScenario.Results;
using ViewModel.Utilities;
using ViewModel.Saving;
using ViewModel.Editors;

namespace ViewModel.ImpactAreaScenario
{
    public class IASElementSet : ChildElement
    {
        #region Fields
        public const string IAS_SET = "IASSet";
        public const string NAME = "Name";
        public const string DESCRIPTION = "Description";
        public const string YEAR = "Year";

        private string _Description = "";
        private int _AnalysisYear;
        #endregion

        #region Properties
        public bool HasComputed { get; set; }
        /// <summary>
        /// These are the results after doing a compute. If a compute has not been
        /// done, then this will be null.
        /// </summary>
        //public IConditionLocationYearResult ComputeResults { get; set; }

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

        public IASElementSet(string name, string description, int year, List<SpecificIAS> elems) : base()
        {
            SpecificIASElements.AddRange( elems);
            Name = name;
            Description = description;
            AnalysisYear = year;

            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/ImpactAreaScenario_20x20.png");

            AddActions();
        }

        /// <summary>
        /// The ctor used to load an element set from the database.
        /// </summary>
        /// <param name="xml"></param>
        public IASElementSet(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            XElement setElem = doc.Element(IAS_SET);
            Name = setElem.Attribute(NAME).Value;
            Description = setElem.Attribute(DESCRIPTION).Value;
            AnalysisYear = Int32.Parse(setElem.Attribute(YEAR).Value);

            IEnumerable<XElement> iasElements = setElem.Elements("IAS");
            foreach(XElement elem in iasElements)
            {
                SpecificIASElements.Add(new SpecificIAS(elem));
            }

            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/Condition.png");
            AddActions();
        }

        private void AddActions()
        {
            NamedAction edit = new NamedAction();
            edit.Header = "Edit Impact Area Scenario...";
            edit.Action = EditIASSet;

            NamedAction compute = new NamedAction();
            compute.Header = "Compute Impact Area Scenario...";
            compute.Action = ComputeScenario;

            NamedAction viewResults = new NamedAction();
            viewResults.Header = "View Results...";
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
        /// Deletes a conditions element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveElement(object sender, EventArgs e)
        {
            PersistenceFactory.GetIASManager().Remove(this);
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
            //this is kind of messy. Quite a bit of code to get the name of the impact area from the impact area id.
            //todo: get a list of result objects
            List<metrics.Results> iasResults = new List<metrics.Results>();

            ObservableCollection<ImpactAreaRowItem> impactAreaRows = GetStudyImpactAreaRowItems();
            foreach (SpecificIAS ias in SpecificIASElements)
            {
                int impactAreaID = ias.ImpactAreaID;
                string impactAreaName = GetImpactAreaNameFromID(impactAreaRows, impactAreaID);
                if (impactAreaName != null)
                {
                    SpecificIASResultVM result = new SpecificIASResultVM(impactAreaName, ias.Thresholds, null);
                    results.Add(result);
                }
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
            IASResultsVM resultViewer = new IASResultsVM(results);
            string header = "Results for " + Name;
            DynamicTabVM tab = new DynamicTabVM(header, resultViewer, "resultViewer");
            Navigate(tab, false, false);
        }
        
        private void ComputeScenario(object arg1, EventArgs arg2)
        {
            HasComputed = true;
            foreach(SpecificIAS ias in SpecificIASElements)
            {
                ias.ComputeScenario(arg1, arg2);
            }
            //i am just saving here to trigger the update event. Once we have the real compute we will want to save the results.
            PersistenceFactory.GetIASManager().SaveExisting(this, this);
        }
        #endregion
        #region Voids
        

        #endregion
        #region Functions
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            IASElementSet elem = (IASElementSet)elementToClone;
            IASElementSet newElem = new IASElementSet(elem.Name, elem.Description, elem.AnalysisYear, elem.SpecificIASElements);
            return newElem;
        }

        #endregion

        public XElement WriteToXML()
        {
            XElement setElement = new XElement(IAS_SET);
            setElement.SetAttributeValue(NAME, Name);
            setElement.SetAttributeValue(DESCRIPTION, Description);
            setElement.SetAttributeValue(YEAR, AnalysisYear);

            foreach(SpecificIAS elem in SpecificIASElements)
            {
                setElement.Add(elem.WriteToXML());
            }

            return setElement;
        }

    }
}
