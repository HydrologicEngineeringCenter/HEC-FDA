using alternatives;
using compute;
using HEC.FDA.ViewModel.Alternatives.Results;
using HEC.FDA.ViewModel.Alternatives.Results.ResultObject;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using metrics;
using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
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

        //todo: i think i can get rid of this once richard gets me the new code.
        private AlternativeResults _Results;

        #region properties
        public List<int> IASElementSets { get; } = new List<int>();
        public AlternativeResults Results
        {
            get { return _Results; }
        }
        #endregion
        #region Constructors

        /// <summary>
        /// Ctor for constructing new alternative element.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="IASElements"></param>
        public AlternativeElement(string name, string description, string creationDate, List<int> IASElements, int id) : base(id)
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

            if(altElement.Elements("AlternativeResults").Any())
            {
                XElement resultElem = altElement.Elements("AlternativeResults").First();
                //TODO: uncomment when richard fixes the bug
                _Results = AlternativeResults.ReadFromXML(resultElem);
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

            //NamedAction compute = new NamedAction();
            //compute.Header = StringConstants.CALCULATE_AED_MENU;
            //compute.Action = ComputeAlternative;

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
           // localActions.Add(compute);
            localActions.Add(viewResults);
            localActions.Add(removeCondition);
            localActions.Add(renameElement);

            Actions = localActions;
        }

        

        /// <summary>
        /// These elements will be returned in year order. The lower year will be the first element.
        /// If the element cannot be found then it will be null.
        /// </summary>
        /// <returns></returns>
        private IASElementSet[] GetElementsFromID()
        {
            IASElementSet[] iASElems = new IASElementSet[] { null, null };

            bool firstElemFound = false;
            bool secondElemFound = false;

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
                    firstElemFound = true;
                }
                else if (setID == secondID)
                {
                    iASElems[1] = set;
                    secondElemFound = true;
                }
            }

            //put them in the correct order
            if(firstElemFound && secondElemFound)
            {
                IASElementSet firstElem = iASElems[0];
                IASElementSet secondElem = iASElems[1];
                int firstYear = firstElem.AnalysisYear;
                int secondYear = secondElem.AnalysisYear;
                if(firstYear > secondYear)
                {
                    //todo: test this 
                    //switch them 
                    iASElems[0] = secondElem;
                    iASElems[1] = firstElem;
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
            if (firstElem != null && firstElem.Results != null)
            {
                firstElemHasResults = true;
            }
            if (secondElem != null && secondElem.Results != null)
            {
                secondElemHasResults = true;
            }

            if (!firstElemHasResults)
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
                vr.AddErrorMessage("There are no longer two scenarios linked to this alternative.");
            }
            return vr;
        }

        private AlternativeResult CreateAlternativeResult(AlternativeResults results)
        {
            AlternativeResult altResult = null;
            StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement();

            double discountRate = studyPropElem.DiscountRate;
            int period = studyPropElem.PeriodOfAnalysis;

            IASElementSet[] iASElems = GetElementsFromID();
            IASElementSet firstElem = iASElems[0];
            IASElementSet secondElem = iASElems[1];


            YearResult yr1 = new YearResult(firstElem.AnalysisYear, new DamageWithUncertaintyVM(firstElem.Results), new DamageByImpactAreaVM(firstElem.Results), new DamageByDamCatVM(firstElem.Results));
            YearResult yr2 = new YearResult(secondElem.AnalysisYear, new DamageWithUncertaintyVM(secondElem.Results), new DamageByImpactAreaVM(secondElem.Results), new DamageByDamCatVM(secondElem.Results));

            EADResult eadResult = new EADResult(new List<YearResult>() { yr1, yr2 });
            AAEQResult aaeqResult = new AAEQResult(new DamageWithUncertaintyVM(discountRate, period, results), new DamageByImpactAreaVM(discountRate, period, results), new DamageByDamCatVM(results));
            altResult = new AlternativeResult(Name, eadResult, aaeqResult);

            return altResult;
        }

        public AlternativeResults ComputeAlternative(object arg1, EventArgs arg2)
        {
            IASElementSet[] iASElems = GetElementsFromID();
            IASElementSet firstElem = iASElems[0];
            IASElementSet secondElem = iASElems[1];

            int firstElemYear = firstElem.AnalysisYear;
            int secondElemYear = secondElem.AnalysisYear;
            ScenarioResults firstResults = firstElem.Results;
            ScenarioResults secondResults = secondElem.Results;

            int seed = 99;
            RandomProvider randomProvider = new RandomProvider(seed);
            StudyPropertiesElement studyProperties = StudyCache.GetStudyPropertiesElement();

            double discountRate = studyProperties.DiscountRate;
            int periodOfAnalysis = studyProperties.PeriodOfAnalysis;

            _Results = Alternative.AnnualizationCompute(randomProvider, discountRate, periodOfAnalysis, ID, firstResults, secondResults);

            Saving.PersistenceFactory.GetAlternativeManager().SaveExisting(this);

            MessageBoxResult messageBoxResult = MessageBox.Show("Compute completed. Would you like to view the results?", "Compute Complete", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                ViewResults(this, new EventArgs());
            }

            return _Results;
        }

        public void ViewResults(object arg1, EventArgs arg2)
        {

            FdaValidationResult vr = RunPreComputeValidation();
            if (vr.IsValid)
            {

                AlternativeResults results = ComputeAlternative(arg1, arg2);

                if (results != null)
                {
                    AlternativeResultsVM vm = new AlternativeResultsVM(CreateAlternativeResult(results));
                    string header = "Alternative Results: " + Name;
                    DynamicTabVM tab = new DynamicTabVM(header, vm, "AlternativeResults" + Name);
                    Navigate(tab, false, true);
                }
                else
                {
                    MessageBox.Show("There are no results to view", "No Results", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show(vr.ErrorMessage, "Cannot Compute", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
            if (elementToClone is AlternativeElement elem)
            {
                AlternativeElement elemToReturn = new AlternativeElement(elem.Name, elem.Description, elem.LastEditDate, elem.IASElementSets, elem.ID);
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

            if(_Results != null)
            {
                XElement resultsElem = _Results.WriteToXML();
                altElement.Add(resultsElem);
            }
            return altElement.ToString();
        }
    }
}
