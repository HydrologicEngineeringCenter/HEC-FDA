using HEC.FDA.Model.alternatives;
using HEC.FDA.Model.compute;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using Utility;
using Utility.Progress;
using Visual.Observables;
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
            if(isOldXMLStyle)
            {
                IEnumerable<XElement> iasElements = altElement.Elements(IAS_SET);

                int i = 0;
                foreach(XElement elem in iasElements)
                {
                    int iasID = int.Parse(elem.Attribute(ID_STRING).Value);
                    //get the element from the id and grab the year from it. If no year that make it the current year.
                    IASElement iasElem = GetElementFromID(iasID);
                    bool yearSuccess = int.TryParse(iasElem.AnalysisYear, out int year);
                    if (!yearSuccess)
                    {
                        year = DateTime.Now.Year;
                    }
                    if(i== 0)
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
                FutureScenario = new AlternativeScenario(futureElem.Element(AlternativeScenario.ALTERNATIVE_SCENARIO));
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

            XElement futureElem = new(FUTURE_SCENARIO);
            futureElem.Add(FutureScenario.ToXML());

            altElement.Add(baseElem);
            altElement.Add(futureElem);

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
            IASElement futureElem = FutureScenario.GetElement();

            FdaValidationResult scenariosExistResults = DoBothScenariosExist(baseElem, futureElem);
            vr.AddErrorMessage(scenariosExistResults.ErrorMessage);
            if (scenariosExistResults.IsValid)
            {
                vr.AddErrorMessage(DoScenariosHaveResults(baseElem, futureElem).ErrorMessage);
            }

            return vr;
        }

        private static FdaValidationResult DoScenariosHaveResults(IASElement firstElem, IASElement secondElem)
        {
            FdaValidationResult vr = new();
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

        private static FdaValidationResult DoBothScenariosExist(IASElement firstElem, IASElement secondElem)
        {
            FdaValidationResult vr = new();
            if (firstElem == null || secondElem == null)
            {
                vr.AddErrorMessage("There are no longer two scenarios linked to this alternative.");
            }
            return vr;
        }

        private AlternativeResult CreateAlternativeResult(AlternativeResults results)
        {
            StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement();

            double discountRate = studyPropElem.DiscountRate;
            int period = studyPropElem.PeriodOfAnalysis;

            List<int> analysisYears = results.AnalysisYears;

            YearResult yr1 = new(analysisYears.Min(), new DamageWithUncertaintyVM(results, DamageMeasureYear.Base), new DamageByImpactAreaVM(results, DamageMeasureYear.Base), new DamageByDamCatVM(results, DamageMeasureYear.Base));
            YearResult yr2 = new(analysisYears.Max(), new DamageWithUncertaintyVM(results, DamageMeasureYear.Future), new DamageByImpactAreaVM(results, DamageMeasureYear.Future), new DamageByDamCatVM(results, DamageMeasureYear.Future));

            EADResult eadResult = new(new List<YearResult>() { yr1, yr2 });
            EqadResult aaeqResult = new(new DamageWithUncertaintyVM( results, DamageMeasureYear.Eqad, discountRate, period), new DamageByImpactAreaVM( results, DamageMeasureYear.Eqad, discountRate, period), new DamageByDamCatVM(results, discountRate, period));
            AlternativeResult altResult = new(Name, eadResult, aaeqResult);

            return altResult;
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
                ViewResults();
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
