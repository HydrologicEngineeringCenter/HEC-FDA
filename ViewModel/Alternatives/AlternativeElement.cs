using HEC.FDA.ViewModel.Alternatives.Results;
using HEC.FDA.ViewModel.Alternatives.Results.ResultObject;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Alternatives
{
    public class AlternativeElement : ChildElement
    {
        private const string ALTERNATIVE = "Alternative";
        private const string NAME = "Name";
        private const string DESCRIPTION = "Description";
        public const string LAST_EDIT_DATE = "LastEditDate";
        private const string IAS_SET = "IASSet";
        private const string ID_STRING = "ID";

        public List<int> IASElementSets { get; } = new List<int>();

        #region Constructors

        /// <summary>
        /// Ctor for constructing new alternative element.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="IASElements"></param>
        public AlternativeElement(string name, string description,string creationDate, List<int> IASElements, int id):base(id)
        {
            Name = name;
            Description = description;
            IASElementSets.AddRange(IASElements);
            LastEditDate = creationDate;
            CustomTreeViewHeader = new CustomHeaderVM(Name)
            {
                ImageSource = ImageSources.ALTERNATIVE_IMAGE,
                Tooltip = StringConstants.CreateLastEditTooltip(LastEditDate)
            };
            AddActions();
        }

        /// <summary>
        /// Ctor for loading an element from the database.
        /// </summary>
        /// <param name="xml"></param>
        public AlternativeElement(string xml, int id) : base(id)
        {
            XDocument doc = XDocument.Parse(xml);
            XElement altElement = doc.Element(ALTERNATIVE);
            Name = altElement.Attribute(NAME).Value;
            Description = altElement.Attribute(DESCRIPTION).Value;
            LastEditDate = altElement.Attribute(LAST_EDIT_DATE).Value;

            IEnumerable<XElement> iasElements = altElement.Elements(IAS_SET);
            foreach (XElement elem in iasElements)
            {
                int iasID = Int32.Parse(elem.Attribute(ID_STRING).Value);
                IASElementSets.Add(iasID);
            }
            CustomTreeViewHeader = new CustomHeaderVM(Name)
            {
                ImageSource = ImageSources.ALTERNATIVE_IMAGE,
                Tooltip = StringConstants.CreateLastEditTooltip(LastEditDate)
            };    
            AddActions();
        }
        #endregion

        private void AddActions()
        {
            NamedAction edit = new NamedAction();
            edit.Header = StringConstants.EDIT_ALTERNATIVE_MENU;
            edit.Action = EditAlternative;

            NamedAction compute = new NamedAction();
            compute.Header = StringConstants.CALCULATE_AED_MENU;
            compute.Action = ComputeAlternative;

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

        public void ComputeAlternative(object arg1, EventArgs arg2)
        {
            FdaValidationResult vr = RunPreComputeValidation();
            if (vr.IsValid)
            {
                //todo: Run calculations. waiting for hec to put the new model in.
                //grab the result objects off the ias elements and run the calculation.
                IASElementSet[] iASElems = GetElementsFromID();
                IASElementSet firstElem = iASElems[0];
                IASElementSet secondElem = iASElems[1];

                //scenarios.Scenario scenario1 = new scenarios.Scenario(firstElem.AnalysisYear, );
                //scenarios.Scenario scenario2 = new scenarios.Scenario(firstElem.AnalysisYear, );
                //long por = firstElem.AnalysisYear;
                //int id = 99;
                //alternatives.Alternative alt = new alternatives.Alternative(scenario1, scenario2, por, id);
                //alt.ComputeEEAD();
                //alt.AnnualizationCompute()

            }
            else
            {
                MessageBox.Show(vr.ErrorMessage, "Cannot Compute", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }


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
                int setID = set.ID;
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
            IASElementSet[] iASElems = GetElementsFromID();
            IASElementSet firstElem = iASElems[0];
            IASElementSet secondElem = iASElems[1];

            vr.AddErrorMessage(DoBothScenariosExist(firstElem, secondElem).ErrorMessage);
            vr.AddErrorMessage(AreScenarioYearsDifferent(firstElem, secondElem).ErrorMessage);
            vr.AddErrorMessage(DoScenariosHaveResults(firstElem, secondElem).ErrorMessage);

            return vr;
        }
        private FdaValidationResult DoScenariosHaveResults(IASElementSet firstElem, IASElementSet secondElem)
        {
            FdaValidationResult vr = new FdaValidationResult();
            bool firstElemHasResults = false;
            bool secondElemHasResults = false;
            if(firstElem != null && firstElem.GetResults().Count>0)
            {
                firstElemHasResults = true;
            }
            if (secondElem != null && secondElem.GetResults().Count > 0)
            {
                secondElemHasResults = true;
            }

            if(!firstElemHasResults)
            {
                vr.AddErrorMessage("Scenario '" + firstElem.Name + "' has no compute results.");
            }
            if (!secondElemHasResults)
            {
                vr.AddErrorMessage("Scenario '" + secondElem.Name + "' has no compute results.");
            }
            return vr;
        }

        private FdaValidationResult AreScenarioYearsDifferent(IASElementSet firstElem, IASElementSet secondElem)
        {
            FdaValidationResult vr = new FdaValidationResult();
            int firstYear = firstElem.AnalysisYear;
            int secondYear = secondElem.AnalysisYear;
            if (firstYear == secondYear)
            {
                vr.AddErrorMessage("The selected impact area scenarios both have the same analysis year.");
                vr.AddErrorMessage("Different years are required to run the calculation.");
            }
            return vr;
        }

        private FdaValidationResult DoBothScenariosExist(IASElementSet firstElem, IASElementSet secondElem)
        {
            FdaValidationResult vr = new FdaValidationResult();            
            if (firstElem == null || secondElem == null)
            {
                vr.AddErrorMessage("There are no longer two impact area scenarios linked to this alternative.");
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

        private FdaValidationResult CanViewResults()
        {
            FdaValidationResult vr = new FdaValidationResult();

            //todo: check if we have results that can be viewed.

            return vr;
        }

        public void ViewResults(object arg1, EventArgs arg2)
        {
            FdaValidationResult vr = CanViewResults();
            if (vr.IsValid)
            {
                AlternativeResultsVM vm = new AlternativeResultsVM(CreateAlternativeResult());
                string header = "Alternative Results: " + Name;
                DynamicTabVM tab = new DynamicTabVM(header, vm, "AlternativeResults" + Name);
                Navigate(tab, false, true);
            }
            else
            {
                MessageBox.Show(vr.ErrorMessage, "No Results", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
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
            if(elementToClone is AlternativeElement elem)
            {
                AlternativeElement elemToReturn = new AlternativeElement(elem.Name, elem.Description,elem.LastEditDate, elem.IASElementSets, elem.ID);
                return elemToReturn;
            }
            return null;
        }

        public string WriteToXML()
        {
            XElement altElement = new XElement(ALTERNATIVE);
            altElement.SetAttributeValue(NAME, Name);
            altElement.SetAttributeValue(DESCRIPTION, Description);
            altElement.SetAttributeValue(LAST_EDIT_DATE, LastEditDate);

            foreach (int elemID in IASElementSets)
            {
                XElement setElement = new XElement(IAS_SET);
                setElement.SetAttributeValue(ID_STRING, elemID);
                altElement.Add(setElement);
            }
            return altElement.ToString();
        }
    }
}
