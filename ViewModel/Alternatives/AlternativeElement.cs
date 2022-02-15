using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using HEC.FDA.ViewModel.Alternatives.Results;
using HEC.FDA.ViewModel.Alternatives.Results.ResultObject;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Study;

namespace HEC.FDA.ViewModel.Alternatives
{
    public class AlternativeElement : ChildElement
    {
        private const string ALTERNATIVE = "Alternative";
        private const string NAME = "Name";
        private const string DESCRIPTION = "Description";
        private const string IAS_SET = "IASSet";
        private const string ID = "ID";

        public List<int> IASElementSets { get; } = new List<int>();

        #region Constructors

        /// <summary>
        /// Ctor for constructing new alternative element.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="IASElements"></param>
        public AlternativeElement(string name, string description, List<int> IASElements)
        {
            Name = name;
            Description = description;
            IASElementSets.AddRange(IASElements);
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/Alternatives_20x20.png");
            AddActions();
        }

        /// <summary>
        /// Ctor for loading an element from the database.
        /// </summary>
        /// <param name="xml"></param>
        public AlternativeElement(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            XElement altElement = doc.Element(ALTERNATIVE);
            Name = altElement.Attribute(NAME).Value;
            Description = altElement.Attribute(DESCRIPTION).Value;

            IEnumerable<XElement> iasElements = altElement.Elements(IAS_SET);
            foreach (XElement elem in iasElements)
            {
                int iasID = Int32.Parse(elem.Attribute(ID).Value);
                IASElementSets.Add(iasID);
            }
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/Alternatives_20x20.png");
            AddActions();
        }

        #endregion

        private void AddActions()
        {
            NamedAction edit = new NamedAction();
            edit.Header = "Edit Alternative...";
            edit.Action = EditAlternative;

            NamedAction compute = new NamedAction();
            compute.Header = "Calculate Average Annual Equivalent Damage...";
            compute.Action = ComputeAlternative;

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

        public void RemoveElement(object sender, EventArgs e)
        {
            PersistenceFactory.GetAlternativeManager().Remove(this);
        }
        public void ComputeAlternative(object arg1, EventArgs arg2)
        {
            FdaValidationResult vr = RunPreComputeValidation();
            if(!vr.IsValid)
            {
                MessageBox.Show(vr.ErrorMessage, "Invalid Setup", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            //todo: Run calculations. waiting for hec to put the new model in.
            IASElementSet[] elems = GetElementsFromID();
            //grab the result objects off the ias elements and run the calculation.

        }

        private IASElementSet[] GetElementsFromID()
        {
            IASElementSet[] iASElems = new IASElementSet[] { null, null };
            int firstID = IASElementSets[0];
            int secondID = IASElementSets[1];
            //get the current ias elements in the study
            List<IASElementSet> currentElementSets = StudyCache.GetChildElementsOfType<IASElementSet>();
            foreach (IASElementSet set in currentElementSets)
            {
                int setID = set.GetElementID();
                if (setID == firstID)
                {
                    iASElems[0] = set;
                }
                else if (setID == secondID)
                {
                    iASElems[1] = set;
                }
            }
            return iASElems;
        }

        private FdaValidationResult RunPreComputeValidation()
        {
            FdaValidationResult vr = new FdaValidationResult();
            //do the ias elements still exist:
            IASElementSet[] iASElems = GetElementsFromID();
            IASElementSet firstElem = iASElems[0];
            IASElementSet secondElem = iASElems[1];

            if (firstElem == null || secondElem == null)
            {
                vr.AddErrorMessage("There are no longer two impact area scenarios linked to this alternative.");
            }
            else
            {
                //are the years still different
                int firstYear = firstElem.AnalysisYear;
                int secondYear = secondElem.AnalysisYear;
                if (firstYear == secondYear)
                {
                    vr.AddErrorMessage("The selected impact area scenarios both have the same analysis year.");
                    vr.AddErrorMessage("Different years are required to run the calculation.");
                }
            }

            return vr;
        }

        private AlternativeResult CreateAlternativeResult()
        {
            AlternativeResult altResult = null;
            StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement();

            double discountRate = studyPropElem.DiscountRate;
            int period = studyPropElem.PeriodOfAnalysis;
            YearResult yr1 = new YearResult(2021, new DamageWithUncertaintyVM(.123, 1), new DamageByImpactAreaVM(), new DamageByDamCatVM());
            YearResult yr2 = new YearResult(2022, new DamageWithUncertaintyVM(.456, 4), new DamageByImpactAreaVM(), new DamageByDamCatVM());

            EADResult eadResult = new EADResult(new List<YearResult>() { yr1, yr2 });
            AAEQResult aaeqResult = new AAEQResult(new DamageWithUncertaintyVM(discountRate, period, .7, 8), new DamageByImpactAreaVM(discountRate, period), new DamageByDamCatVM());
            altResult = new AlternativeResult(eadResult, aaeqResult);

            return altResult;
        }
        public void ViewResults(object arg1, EventArgs arg2)
        {
            AlternativeResultsVM vm = new AlternativeResultsVM(CreateAlternativeResult());
            string header = "Alternative Results: " + Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "AlternativeResults" + Name);
            Navigate(tab, false, true);
        }

        public void EditAlternative(object arg1, EventArgs arg2)
        {
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);

            CreateNewAlternativeVM vm = new CreateNewAlternativeVM(this, actionManager);
            string header = "Edit Alternative " + Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditAlternative" + Name);
            Navigate(tab, false, true);
        }

        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            if(elementToClone is AlternativeElement)
            {
                AlternativeElement elem = (AlternativeElement)elementToClone;
                AlternativeElement elemToReturn = new AlternativeElement(elem.Name, elem.Description, elem.IASElementSets);
                return elemToReturn;
            }
            return null;
        }

        public string WriteToXML()
        {
            XElement altElement = new XElement(ALTERNATIVE);
            altElement.SetAttributeValue(NAME, Name);
            altElement.SetAttributeValue(DESCRIPTION, Description);

            foreach (int elemID in IASElementSets)
            {
                XElement setElement = new XElement(IAS_SET);
                setElement.SetAttributeValue(ID, elemID);
                altElement.Add(setElement);
            }
            return altElement.ToString();
        }
    }
}
