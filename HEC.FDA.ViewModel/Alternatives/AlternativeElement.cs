using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.Alternatives.Results;
using HEC.FDA.ViewModel.Alternatives.Results.ResultObject;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Utility;
using Visual.Observables;
using static HEC.FDA.ViewModel.ImpactAreaScenario.Results.UncertaintyControlConfigs;
using SynchronizationContext = Utility.SynchronizationContext;

namespace HEC.FDA.ViewModel.Alternatives
{
    public class AlternativeElement : ChildElement
    {
        private const string ALTERNATIVE = "Alternative";
        public const string LAST_EDIT_DATE = "LastEditDate";
        private const string IAS_SET = "IASSet";
        private const string ID_STRING = "ID";

        private const string BASE_SCENARIO = "BaseScenario";
        private const string FUTURE_SCENARIO = "FutureScenario";


        #region properties
        public AlternativeResults Results { get; set; }
        public AlternativeScenario BaseScenario { get; }
        public AlternativeScenario FutureScenario { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Ctor for constructing new alternative element.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="IASElements"></param>
        public AlternativeElement(string name, string description, string creationDate, AlternativeScenario baseScenario, AlternativeScenario futureScenario, int id) : base(name, creationDate, description, id)
        {
            BaseScenario = baseScenario;
            FutureScenario = futureScenario;

            AddDefaultActions(EditAlternative, StringConstants.EDIT_ALTERNATIVE_MENU);
            NamedAction viewResults = new()
            {
                Header = StringConstants.VIEW_RESULTS_MENU,
                Action = ComputeAlternative
            };
            Actions.Insert(1, viewResults);
        }

        /// <summary>
        /// Ctor for loading an element from the database.
        /// </summary>
        /// <param name="xml"></param>
        public AlternativeElement(XElement altElement, int id) : base(altElement, id)
        {
            bool isOldXMLStyle = altElement.Elements(IAS_SET).Any();
            if (isOldXMLStyle)
            {
                IEnumerable<XElement> iasElements = altElement.Elements(IAS_SET);

                int i = 0;
                foreach (XElement elem in iasElements)
                {
                    int iasID = int.Parse(elem.Attribute(ID_STRING).Value);
                    //get the element from the id and grab the year from it. If no year that make it the current year.
                    IASElement iasElem = GetElementFromID(iasID);
                    bool yearSuccess = int.TryParse(iasElem.AnalysisYear, out int year);
                    if (!yearSuccess)
                    {
                        year = DateTime.Now.Year;
                    }
                    if (i == 0)
                    {
                        BaseScenario = new AlternativeScenario(iasID, year);
                    }
                    else
                    {
                        FutureScenario = new AlternativeScenario(iasID, year);
                    }
                    i++;
                }
            }
            else
            {
                XElement baseElem = altElement.Element(BASE_SCENARIO);
                BaseScenario = new AlternativeScenario(baseElem.Element(AlternativeScenario.ALTERNATIVE_SCENARIO));
                XElement futureElem = altElement.Element(FUTURE_SCENARIO);
                if (futureElem != null)
                {
                    FutureScenario = new AlternativeScenario(futureElem.Element(AlternativeScenario.ALTERNATIVE_SCENARIO));
                }
            }

            AddDefaultActions(EditAlternative, StringConstants.EDIT_ALTERNATIVE_MENU);
            NamedAction viewResults = new()
            {
                Header = StringConstants.VIEW_RESULTS_MENU,
                Action = ComputeAlternative
            };
            Actions.Insert(1, viewResults);
        }
        #endregion

        public override XElement ToXML()
        {
            XElement altElement = new(ALTERNATIVE);
            altElement.Add(CreateHeaderElement());

            XElement baseElem = new(BASE_SCENARIO);
            baseElem.Add(BaseScenario.ToXML());
            altElement.Add(baseElem);

            if (FutureScenario != null)
            {
                XElement futureElem = new(FUTURE_SCENARIO);
                futureElem.Add(FutureScenario.ToXML());
                altElement.Add(futureElem);
            }

            return altElement;
        }

        public static IASElement GetElementFromID(int id)
        {
            IASElement elem = null;
            List<IASElement> currentElementSets = StudyCache.GetChildElementsOfType<IASElement>();
            foreach (IASElement set in currentElementSets)
            {
                int setID = set.ID;
                if (setID == id)
                {
                    elem = set;
                }
            }
            return elem;
        }

        public FdaValidationResult RunPreComputeValidation()
        {
            FdaValidationResult vr = new();
            IASElement baseElem = BaseScenario.GetElement();
            vr.AddErrorMessage(ValidateScenarioExists(baseElem, "base").ErrorMessage);
            if (baseElem != null)
                vr.AddErrorMessage(ValidateScenarioHasResults(baseElem).ErrorMessage);

            if (FutureScenario != null)
            {
                IASElement futureElem = FutureScenario.GetElement();
                vr.AddErrorMessage(ValidateScenarioExists(futureElem, "future").ErrorMessage);
                if (futureElem != null)
                    vr.AddErrorMessage(ValidateScenarioHasResults(futureElem).ErrorMessage);
            }
            return vr;
        }

        private static FdaValidationResult ValidateScenarioExists(IASElement elem, string label)
        {
            FdaValidationResult vr = new();
            if (elem == null)
                vr.AddErrorMessage($"The {label} scenario linked to this alternative no longer exists.");
            return vr;
        }

        private static FdaValidationResult ValidateScenarioHasResults(IASElement elem)
        {
            FdaValidationResult vr = new();
            if (elem.Results == null)
                vr.AddErrorMessage($"Scenario '{elem.Name}' has no compute results.");
            return vr;
        }

        private AlternativeResult CreateAlternativeResult(AlternativeResults results)
        {
            StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement();

            double discountRate = studyPropElem.DiscountRate;
            int period = studyPropElem.PeriodOfAnalysis;

            List<int> analysisYears = results.AnalysisYears;

            YearResult yr1 = new(
                analysisYears.Min(),
                new DamageWithUncertaintyVM(results, DamageMeasureYear.Base, new DamageWithUncertaintyControlConfig()),
                new DamageByImpactAreaVM(results, DamageMeasureYear.Base),
                new DamageByDamCatVM(results, DamageMeasureYear.Base),
                null,
                null);

            List<YearResult> yearResults = [yr1];

            if (FutureScenario != null)
            {
                YearResult yr2 = new(
                    analysisYears.Max(),
                    new DamageWithUncertaintyVM(results, DamageMeasureYear.Future, new DamageWithUncertaintyControlConfig()),
                    new DamageByImpactAreaVM(results, DamageMeasureYear.Future),
                    new DamageByDamCatVM(results, DamageMeasureYear.Future),
                    null,
                    null);
                yearResults.Add(yr2);
            }

            EADResult eadResult = new(yearResults);
            EqadResult eqadResult = new(
                new DamageWithUncertaintyVM(results, DamageMeasureYear.Eqad, new EqADWithUncertaintyControlConfig(), discountRate, period),
                new DamageByImpactAreaVM(results, DamageMeasureYear.Eqad, discountRate, period),
                new DamageByDamCatVM(results, discountRate, period));
            return new AlternativeResult(Name, eadResult, eqadResult);
        }

        public async void ComputeAlternative(object arg1 = null, EventArgs arg2 = null)
        {
            //This is the new entry point for the "view results" menu item
            //when the compute is completed it will call ComputeCompleted which will then call the "ViewResults".
            FdaValidationResult vr = RunPreComputeValidation();
            if (vr.IsValid)
            {
                ISynchronizationContext context = new SynchronizationContext(action => Application.Current.Dispatcher.BeginInvoke(action));
                BatchJob batchJob = new(uiThreadSyncContext: context);
                ComputeAlternativeVM vm = new(batchJob);
                string header = "Compute Log For Alternative: " + Name;
                DynamicTabVM tab = new(header, vm, "ComputeLog" + Name);
                Navigate(tab, false, false);
                StudyPropertiesElement props = StudyCache.GetStudyPropertiesElement();
                Results = await AlternativeComputer.RunAnnualizationCompute(this, props, batchJob.Reporter);
                if (Results.EqadResults.ConsequenceResultList.Count < 1)
                {
                    MessageBox.Show("No economic damages were found for the equivalent annual damage calculation. No results are available. Any life loss results will still be available in the alternative comparison report.", "No EqAD", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ViewResults();
                }
            }
            else
            {
                MessageBox.Show(vr.ErrorMessage, "Cannot Compute Alternative Results", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void ViewResults()
        {
            if (Results != null)
            {
                AlternativeResult res = CreateAlternativeResult(Results);
                AlternativeResultsVM vm = new(res);
                string header = "Alternative Results: " + Name;
                DynamicTabVM tab = new(header, vm, "AlternativeResults" + Name);
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

            CreateNewAlternativeVM vm = new(this, actionManager);
            string header = "Edit Alternative " + Name;
            DynamicTabVM tab = new(header, vm, "EditAlternative" + Name);
            Navigate(tab, false, true);
        }

    }
}
