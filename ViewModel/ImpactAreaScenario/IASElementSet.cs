using Model;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ViewModel.ImpactAreaScenario.Results;
using ViewModel.Utilities;

namespace ViewModel.ImpactAreaScenario
{
    public class IASElementSet : ChildElement
    {
        #region Fields
        public const string IAS_SET = "IASSet";
        public const string NAME = "Name";
        public const string DESCRIPTION = "Description";
        public const string YEAR = "Year";

        public event EventHandler EditConditionsTreeElement;
        public event EventHandler RemoveConditionsTreeElement;
        public event EventHandler RenameConditionsTreeElement;
        public event EventHandler UpdateExpansionValueInTreeElement;

        private string _Description = "";
        private int _AnalysisYear;

        private bool _IsExpanded;

        private NamedAction _ViewResults = new NamedAction();
#endregion
        #region Properties

        /// <summary>
        /// These are the results after doing a compute. If a compute has not been
        /// done, then this will be null.
        /// </summary>
        public IConditionLocationYearResult ComputeResults { get; set; }

        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set { _IsExpanded = value; }
        }
        public bool IsBold
        {
            get { return false; }
        }
        public int FontSize
        {
            get { return 12; }
        }


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

        public List<SpecificIAS> SpecificIASElements { get; set; }

        #endregion
        #region Constructors

        
        public IASElementSet(string name, string description, int year, List<SpecificIAS> elems) : base()
        {
            SpecificIASElements = elems;
            Name = name;
            Description = description;
            AnalysisYear = year;

            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/Condition.png");

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
            string setName = setElem.Attribute(NAME).Value;
            string description = setElem.Attribute(DESCRIPTION).Value;
            int year = Int32.Parse(setElem.Attribute(YEAR).Value);

            IEnumerable<XElement> iasElements = setElem.Elements("IAS");
            SpecificIASElements = new List<SpecificIAS>();
            foreach(XElement elem in iasElements)
            {
                SpecificIASElements.Add(new SpecificIAS(elem));
            }

            Name = setName;
            Description = description;
            AnalysisYear = year;

            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/Condition.png");

            AddActions();
        }

        private void AddActions()
        {
            NamedAction edit = new NamedAction();
            edit.Header = "Edit Impact Area Scenario";
            edit.Action = EditCondition;

            NamedAction compute = new NamedAction();
            compute.Header = "Compute Impact Area Scenario";
            compute.Action = ComputeCondition;

            _ViewResults.Header = "View Results";
            _ViewResults.Action = ViewResults;

            NamedAction removeCondition = new NamedAction();
            removeCondition.Header = "Remove";
            removeCondition.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(edit);
            localActions.Add(compute);
            localActions.Add(_ViewResults);
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
            Saving.PersistenceFactory.GetIASManager().Remove(this);
        }

        

        /// <summary>
        /// Opens the conditions editor.
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public void EditCondition(object arg1, EventArgs arg2)
        {

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
               .WithSiblingRules(this);
            Editor.IASEditorVM vm = new Editor.IASEditorVM(this, actionManager);
            vm.RequestNavigation += Navigate;
            //ConditionsEditor = new IASPlotEditorVM(impactAreas, lp3Control, infOutControl, ratingControl, leveeFailureControl, extIntStageControl,
            //    stageDamageControl, damageFrequencyControl, this, actionManager);

            //ConditionsEditor.RequestNavigation += Navigate;
            string header = "Edit Impact Area Scenario";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditIAS");
            Navigate(tab, false, false);
        }


        private void DisplayResults(IConditionLocationYearResult result)
        {
            IASResultsVM resultViewer = new IASResultsVM(Name);
            string header = "Results";
            DynamicTabVM tab = new DynamicTabVM(header, resultViewer, "resultViewer");
            Navigate(tab, false, false);
        }
        private void ViewResults(object arg1, EventArgs arg2)
        {
            DisplayResults(ComputeResults);
        }

        
        //Intentionally left blank until the compute is completed in the model. -cody 10/26/21
        private void ComputeCondition(object arg1, EventArgs arg2)
        {

            //EnterSeedVM enterSeedVM = new EnterSeedVM();
            //string header = "Enter Seed Value";
            //DynamicTabVM tab = new DynamicTabVM(header, enterSeedVM, "EnterSeed");
            //Navigate(tab, true, true);

            //int seedValue = enterSeedVM.Seed;

            //IConditionLocationYearSummary condition = null;

            //IFrequencyFunction frequencyFunction = GetFrequencyFunction();
            //List<ITransformFunction> transformFunctions = GetTransformFunctions();

            //bool hasLeveeFailure = LeveeFailureID != -1;
            //if (hasLeveeFailure)
            //{
            //    LeveeFeatureElement leveeFailureElement = (LeveeFeatureElement)StudyCache.GetChildElementOfType(typeof(LeveeFeatureElement), LeveeFailureID);
            //    ILateralStructure latStruct = ILateralStructureFactory.Factory(leveeFailureElement.Elevation, (ITransformFunction)leveeFailureElement.Curve); 
            //    //todo: Need to handle multiple thresholds
            //    condition = Saving.PersistenceFactory.GetIASManager().CreateIConditionLocationYearSummary(ImpactAreaID,
            //        AnalysisYear, frequencyFunction, transformFunctions, leveeFailureElement, ThresholdType, ThresholdValue);
            //}
            //else 
            //{ 
            //    condition = Saving.PersistenceFactory.GetIASManager().CreateIConditionLocationYearSummary(ImpactAreaID,
            //        AnalysisYear, frequencyFunction, transformFunctions, ThresholdType, ThresholdValue);
            //}

            //if (condition == null)
            //{
            //    return;
            //}

            //IConvergenceCriteria convergenceCriteria = IConvergenceCriteriaFactory.Factory();
            //Dictionary<IMetric, IConvergenceCriteria> metricsDictionary = new Dictionary<IMetric, IConvergenceCriteria>();
            //foreach (IMetric metric in condition.Metrics)
            //{
            //    metricsDictionary.Add(metric, IConvergenceCriteriaFactory.Factory());
            //}

            //IReadOnlyDictionary<IMetric, IConvergenceCriteria> metrics = new ReadOnlyDictionary<IMetric, IConvergenceCriteria>(metricsDictionary);

            //IConditionLocationYearResult result = new ConditionLocationYearResult(condition, metrics, seedValue);
            //result.Compute();
            //ComputeResults = result;
            //Saving.PersistenceFactory.GetIASManager().SaveConditionResults(result, this.GetElementID(), frequencyFunction, transformFunctions);

            //DisplayResults(result);

        }
        #endregion
        #region Voids
        
        //private void UpdateIsElementExpanded(object sender, EventArgs e)
        //{
        //    if (UpdateExpansionValueInTreeElement != null)
        //    {
        //        UpdateExpansionValueInTreeElement.Invoke(this, e);
        //    }
        //}
        //private void RenameConditionsTreeElem(object sender, EventArgs e)
        //{
        //    if (RenameConditionsTreeElement != null)
        //    {
        //        RenameConditionsTreeElement.Invoke(this, e);
        //    }
        //}
        //private void RemoveConditionsTreeElem(object sender, EventArgs e)
        //{
        //    if (RemoveConditionsTreeElement != null)
        //    {
        //        RemoveConditionsTreeElement.Invoke(this, e);
        //    }
        //}

        //private void EditConditionsTreeElem(object sender, EventArgs e)
        //{
        //    if (EditConditionsTreeElement != null)
        //    {
        //        EditConditionsTreeElement.Invoke(this, e);
        //    }
        //}

        /// <summary>
        /// This will update an element that has been modified while the editor is open.
        /// For example a rating curve that was modified.
        /// </summary>
        /// <param name="elemID">The ID of the element that was modified (ie: rating curve)</param>
        /// <param name="newElement">The new element (ie: rating element)</param>
        //public void UpdateElementInEditor_ChildModified(int elemID, ChildElement newElement)
        //{
        //    //todo: i think i need to set this to null when it closes? not sure.
        //    if (ConditionsEditor != null)
        //    {
        //        ConditionsEditor.UpdateEditorWhileEditing_ChildModified(elemID, newElement);
        //    }
        //}

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
