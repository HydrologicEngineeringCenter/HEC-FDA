using alternatives;
using compute;
using HEC.FDA.ViewModel.Alternatives.Results;
using HEC.FDA.ViewModel.Alternatives.Results.ResultObject;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using metrics;
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

        #region properties

        public AlternativeResults Results { get; set; }

        public List<int> IASElementSets { get; } = new List<int>();
        #endregion

        #region Constructors

        /// <summary>
        /// Ctor for constructing new alternative element.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="IASElements"></param>
        public AlternativeElement(string name, string description, string creationDate, List<int> IASElements, int id) : base(name, creationDate, description, id)
        {
            IASElementSets.AddRange(IASElements);

            AddDefaultActions(EditAlternative, StringConstants.EDIT_ALTERNATIVE_MENU);
            NamedAction viewResults = new NamedAction();
            viewResults.Header = StringConstants.VIEW_RESULTS_MENU;
            viewResults.Action = ComputeAlternative;
            Actions.Insert(1, viewResults);
        }

        /// <summary>
        /// Ctor for loading an element from the database.
        /// </summary>
        /// <param name="xml"></param>
        public AlternativeElement(XElement altElement, int id) : base(altElement, id)
        {
            IEnumerable<XElement> iasElements = altElement.Elements(IAS_SET);
            foreach (XElement elem in iasElements)
            {
                int iasID = int.Parse(elem.Attribute(ID_STRING).Value);
                IASElementSets.Add(iasID);
            }

            AddDefaultActions(EditAlternative, StringConstants.EDIT_ALTERNATIVE_MENU);
            NamedAction viewResults = new NamedAction();
            viewResults.Header = StringConstants.VIEW_RESULTS_MENU;
            viewResults.Action = ComputeAlternative;
            Actions.Insert(1, viewResults);
        }
        #endregion       

        /// <summary>
        /// These elements will be returned in year order. The lower year will be the first element.
        /// If the element cannot be found then it will be null.
        /// </summary>
        /// <returns></returns>
        private IASElement[] GetElementsFromID()
        {
            IASElement[] iASElems = new IASElement[] { null, null };

            bool firstElemFound = false;
            bool secondElemFound = false;

            int firstID = IASElementSets[0];
            int secondID = IASElementSets[1];
            //get the current ias elements in the study
            List<IASElement> currentElementSets = StudyCache.GetChildElementsOfType<IASElement>();
            foreach (IASElement set in currentElementSets)
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
                IASElement firstElem = iASElems[0];
                IASElement secondElem = iASElems[1];
                int firstYear = firstElem.AnalysisYear;
                int secondYear = secondElem.AnalysisYear;
                if(firstYear > secondYear)
                {
                    iASElems[0] = secondElem;
                    iASElems[1] = firstElem;
                }
            }

            return iASElems;
        }

        public FdaValidationResult RunPreComputeValidation()
        {
            FdaValidationResult vr = new FdaValidationResult();
            IASElement[] iASElems = GetElementsFromID();
            IASElement firstElem = iASElems[0];
            IASElement secondElem = iASElems[1];

            FdaValidationResult scenariosExistResults = DoBothScenariosExist(firstElem, secondElem);
            vr.AddErrorMessage(scenariosExistResults.ErrorMessage);
            if (scenariosExistResults.IsValid)
            {
                vr.AddErrorMessage(AreScenarioYearsDifferent(firstElem, secondElem).ErrorMessage);
                vr.AddErrorMessage(DoScenariosHaveResults(firstElem, secondElem).ErrorMessage);
            }

            return vr;
        }

        private FdaValidationResult DoScenariosHaveResults(IASElement firstElem, IASElement secondElem)
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

        private FdaValidationResult AreScenarioYearsDifferent(IASElement firstElem, IASElement secondElem)
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

        private FdaValidationResult DoBothScenariosExist(IASElement firstElem, IASElement secondElem)
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

            List<int> analysisYears = results.AnalysisYears;

            YearResult yr1 = new YearResult(analysisYears.Min(), new DamageWithUncertaintyVM(results, DamageMeasureYear.Base), new DamageByImpactAreaVM(results, DamageMeasureYear.Base), new DamageByDamCatVM(results, DamageMeasureYear.Base));
            YearResult yr2 = new YearResult(analysisYears.Max(), new DamageWithUncertaintyVM(results, DamageMeasureYear.Future), new DamageByImpactAreaVM(results, DamageMeasureYear.Future), new DamageByDamCatVM(results, DamageMeasureYear.Future));

            EADResult eadResult = new EADResult(new List<YearResult>() { yr1, yr2 });
            AAEQResult aaeqResult = new AAEQResult(new DamageWithUncertaintyVM( results, DamageMeasureYear.AAEQ, discountRate, period), new DamageByImpactAreaVM( results, DamageMeasureYear.AAEQ, discountRate, period), new DamageByDamCatVM(results, discountRate, period));
            altResult = new AlternativeResult(Name, eadResult, aaeqResult);

            return altResult;
        }

        public void ComputeAlternative(object arg1 = null, EventArgs arg2 = null)
        {
            //This is the new entry point for the "view results" menu item
            //when the compute is completed it will call ComputeCompleted which will then call the "ViewResults".
            FdaValidationResult vr = RunPreComputeValidation();
            if (vr.IsValid)
            {
                IASElement[] iASElems = GetElementsFromID();

                ComputeAlternativeVM vm = new ComputeAlternativeVM(iASElems, ID, this, ComputeCompleted);
                string header = "Compute Log For Alternative: " + Name;
                DynamicTabVM tab = new DynamicTabVM(header, vm, "ComputeLog" + Name);
                Navigate(tab, false, false);
            }
            else
            {
                MessageBox.Show(vr.ErrorMessage, "Cannot Compute Alternative Results", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public void ComputeAlternative(Action<AlternativeResults> callback)
        {
            IASElement[] iASElems = GetElementsFromID();

            IASElement firstElem = iASElems[0];
            IASElement secondElem = iASElems[1];

            ScenarioResults firstResults = firstElem.Results;
            ScenarioResults secondResults = secondElem.Results;

            int seed = 99;
            RandomProvider randomProvider = new RandomProvider(seed);
            StudyPropertiesElement studyProperties = StudyCache.GetStudyPropertiesElement();

            double discountRate = studyProperties.DiscountRate;
            int periodOfAnalysis = studyProperties.PeriodOfAnalysis;

            //todo: register somthing with the message hub?
            AlternativeResults results = Alternative.AnnualizationCompute(randomProvider, discountRate, periodOfAnalysis, ID, firstResults, secondResults);
            callback?.Invoke(results);
        }

        private void ComputeCompleted(AlternativeResults results)
        {
            Results = results;
            Application.Current.Dispatcher.Invoke(
            (Action)(() =>
            {
                    ViewResults();
            }));
        }

        private void ViewResults()
        {
            if (Results != null)
            {
                AlternativeResultsVM vm = new AlternativeResultsVM(CreateAlternativeResult(Results));
                string header = "Alternative Results: " + Name;
                DynamicTabVM tab = new DynamicTabVM(header, vm, "AlternativeResults" + Name);
                Navigate(tab, false, true);
            }
            else
            {
                MessageBox.Show("There are no results to view", "No Results", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        public override XElement ToXML()
        {
            XElement altElement = new XElement(ALTERNATIVE);
            altElement.Add(CreateHeaderElement());

            foreach (int elemID in IASElementSets)
            {
                XElement setElement = new XElement(IAS_SET);
                setElement.SetAttributeValue(ID_STRING, elemID);
                altElement.Add(setElement);
            }
            return altElement;
        }

    }
}
