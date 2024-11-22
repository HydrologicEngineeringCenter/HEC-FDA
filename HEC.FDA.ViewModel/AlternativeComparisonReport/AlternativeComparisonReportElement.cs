using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.AlternativeComparisonReport.Results;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.Alternatives.Results;
using HEC.FDA.ViewModel.Alternatives.Results.ResultObject;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport
{
    public class AlternativeComparisonReportElement : ChildElement
    {
        private const string ALTERNATIVE_COMP_REPORT = "AlternativeComparisonReport";
        private const string ID_STRING = "ID";
        private const string WITHOUT_PROJ_ID = "WithoutProjID";
        private const string WITH_PROJ_ELEM = "WithProjectElement";

        private AlternativeComparisonReportResults _Results;

        #region Properties
        public int WithoutProjAltID { get; }
        public List<int> WithProjAltIDs { get; } = new List<int>();
        public AlternativeComparisonReportResults Results { get; }

        #endregion

        public AlternativeComparisonReportElement(string name, string desc, string creationDate, int withoutProjectAltId, List<int> withProjAlternativeIds, int id)
            : base(name, creationDate, desc, id)
        {
            WithoutProjAltID = withoutProjectAltId;
            WithProjAltIDs = withProjAlternativeIds;

            AddDefaultActions(EditAlternative, StringConstants.EDIT_ALTERNATIVE_COMP_REPORTS_MENU);

            NamedAction viewResults = new NamedAction();
            viewResults.Header = StringConstants.VIEW_RESULTS_MENU;
            viewResults.Action = ComputeAltCompReport;

            Actions.Insert(1, viewResults);
        }

        /// <summary>
        /// The ctor used to load an element from the database.
        /// </summary>
        /// <param name="xml"></param>
        public AlternativeComparisonReportElement(XElement altElement, int id) : base(altElement, id)
        {
            WithoutProjAltID = int.Parse(altElement.Attribute(WITHOUT_PROJ_ID).Value);

            IEnumerable<XElement> altElements = altElement.Elements(WITH_PROJ_ELEM);
            foreach (XElement elem in altElements)
            {
                int iasID = int.Parse(elem.Attribute(ID_STRING).Value);
                WithProjAltIDs.Add(iasID);
            }

            AddDefaultActions(EditAlternative, StringConstants.EDIT_ALTERNATIVE_COMP_REPORTS_MENU);

            NamedAction viewResults = new NamedAction();
            viewResults.Header = StringConstants.VIEW_RESULTS_MENU;
            viewResults.Action = ComputeAltCompReport;

            Actions.Insert(1, viewResults);
        }

        public override XElement ToXML()
        {
            XElement altElement = new XElement(ALTERNATIVE_COMP_REPORT);
            altElement.SetAttributeValue(WITHOUT_PROJ_ID, WithoutProjAltID);

            altElement.Add(CreateHeaderElement());

            foreach (int elemID in WithProjAltIDs)
            {
                XElement setElement = new XElement(WITH_PROJ_ELEM);
                setElement.SetAttributeValue(ID_STRING, elemID);
                altElement.Add(setElement);
            }
            return altElement;
        }

        public void EditAlternative(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);

            CreateNewAlternativeComparisonReportVM vm = new CreateNewAlternativeComparisonReportVM(this, actionManager);
            string header = "Edit Alternative Comparison Report" + Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditAlternative" + Name);
            Navigate(tab, false, true);
        }

        public FdaValidationResult GetValidationResult()
        {
            FdaValidationResult doAlternativesExistResult = DoAlternativesStillExistResult();
            if (!doAlternativesExistResult.IsValid)
            {
                MessageBox.Show(doAlternativesExistResult.ErrorMessage, "Cannot Compute Alternative Comparison Report", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                FdaValidationResult canComputeValidationResult = GetCanComputeResults();
                if (!canComputeValidationResult.IsValid)
                {
                    MessageBox.Show(canComputeValidationResult.ErrorMessage, "Cannot Compute Alternative Comparison Report", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                doAlternativesExistResult.AddErrorMessage(canComputeValidationResult.ErrorMessage);
            }
            return doAlternativesExistResult;
        }

        public void ComputeAltCompReport(object arg1, EventArgs arg2)
        {
            FdaValidationResult canComputeValidationResult = GetValidationResult();
            if (canComputeValidationResult.IsValid)
            {
                AlternativeElement withoutAlt = GetAlternativeElementFromID(WithoutProjAltID);
                List<AlternativeElement> withProjAlts = GetWithProjectAlternatives();

                ComputeAltCompReportVM compVM = new ComputeAltCompReportVM(withoutAlt, withProjAlts, ComputeCompleted);
                string header = "Compute Log For Alternative Comp Report: " + Name;
                DynamicTabVM tab = new DynamicTabVM(header, compVM, "ComputeLogAltCompReport" + Name);
                Navigate(tab, false, false);
            }
        }

        private void ComputeCompleted(AlternativeComparisonReportResults results)
        {
            _Results = results;
            Application.Current.Dispatcher.Invoke(
            (Action)(() =>
            {
                ViewResults();
            }));
        }

        public void ViewResults()
        {
            if (_Results != null)
            {
                AltCompReportResultsVM vm = new AltCompReportResultsVM(CreateResults());
                string header = "Alternative Comparison Report Results: " + Name;
                DynamicTabVM tab = new DynamicTabVM(header, vm, "AlternativeComparisonReportResults" + Name);
                Navigate(tab, false, true);
            }
            else
            {
                MessageBox.Show("There are no results to view.", "No Results", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private List<SpecificAltCompReportResultsVM> CreateResults()
        {
            List<SpecificAltCompReportResultsVM> results = new();
            List<AAEQSummaryRowItem> aAEQSummaryRowItems = CreateAAEQSummaryTable(_Results);
            List<EADSummaryRowItem> eADBaseSummaryRowItems = CreateEADBaseYearSummaryTable(_Results);
            List<EADSummaryRowItem> eADFutureSummaryRowItems = CreateEADFutureYearSummaryTable(_Results);

            List<AggregatedAAEQSummaryRowItem> aAEQAggregatedSummaryRowItems = CreateAggregatedAAEQSummaryTable(_Results);
            List<AggregatedEADSummaryRowItem> eADAggregatedSummaryRowItems = CreateAggregatedEADFutureYearSummaryTable(_Results);
            List<AggregatedEADSummaryRowItem> eADAggregatedBaseSummaryRowItems = CreateAggregatedEADBaseYearSummaryTable(_Results);
            foreach (int altID in WithProjAltIDs)
            {
                SpecificAltCompReportResultsVM specificAltCompReportResultsVM = CreateAlternativeComparisonResult(altID, GetAlternativeElementFromID(altID).Name);
                results.Add(specificAltCompReportResultsVM);
            }

            SpecificAltCompReportResultsVM summaryOption = new SummaryVM(eADBaseSummaryRowItems, eADFutureSummaryRowItems, aAEQSummaryRowItems, eADAggregatedBaseSummaryRowItems, eADAggregatedSummaryRowItems,  aAEQAggregatedSummaryRowItems, _Results.Years);
            results.Add(summaryOption);
            return results;
            
        }
 
        private List<EADSummaryRowItem> CreateEADFutureYearSummaryTable(AlternativeComparisonReportResults results)
        {
            List<EADSummaryRowItem> eadSummaryRowItems = new();
            Dictionary<int, string> impactAreaNames = IASElement.GetImpactAreaNamesFromIDs();
            string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;
            
            foreach (int altID in WithProjAltIDs)
            {
                string withProjName = GetAlternativeElementFromID(altID).Name;
                foreach (int impactAreaID in results.GetImpactAreaIDs())
                {
                    foreach (string damcat in results.GetDamageCategories())
                    {
                        foreach (string assetType in results.GetAssetCategories())
                        {
                            double withProjEAD = results.MeanWithProjectFutureYearEAD(altID, impactAreaID, damcat, assetType);
                            double eadReduced = results.MeanFutureYearEADReduced(altID, impactAreaID, damcat, assetType);
                            double point75 = results.FutureYearEADReducedExceededWithProbabilityQ(.75, altID, impactAreaID, damcat, assetType);
                            double point5 = results.FutureYearEADReducedExceededWithProbabilityQ(.5, altID, impactAreaID, damcat, assetType);
                            double point25 = results.FutureYearEADReducedExceededWithProbabilityQ(.25, altID, impactAreaID, damcat, assetType);
                            double eadWithoutProjDamage = results.MeanWithoutProjectFutureYearEAD(impactAreaID, damcat, assetType);
                            EADSummaryRowItem row = new(impactAreaNames[impactAreaID], damcat, assetType, withoutProjName, eadWithoutProjDamage, withProjName, withProjEAD, eadReduced, point75, point5, point25);
                            eadSummaryRowItems.Add(row);
                        }
                    }
                }
            }
            return eadSummaryRowItems;
        }
        private List<AggregatedEADSummaryRowItem> CreateAggregatedEADFutureYearSummaryTable(AlternativeComparisonReportResults results)
        {
            List<AggregatedEADSummaryRowItem> eadSummaryRowItems = new();
            Dictionary<int, string> impactAreaNames = IASElement.GetImpactAreaNamesFromIDs();
            string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;


            foreach (int altID in WithProjAltIDs)
            {
                string withProjName = GetAlternativeElementFromID(altID).Name;
                foreach (int impactAreaID in results.GetImpactAreaIDs())
                {
                    double withProjEAD = results.MeanWithProjectFutureYearEAD(altID, impactAreaID);
                    double eadReduced = results.MeanFutureYearEADReduced(altID, impactAreaID);
                    double point75 = results.FutureYearEADReducedExceededWithProbabilityQ(.75, altID, impactAreaID);
                    double point5 = results.FutureYearEADReducedExceededWithProbabilityQ(.5, altID, impactAreaID);
                    double point25 = results.FutureYearEADReducedExceededWithProbabilityQ(.25, altID, impactAreaID);
                    double eadWithoutProjDamage = results.MeanWithoutProjectFutureYearEAD();
                    AggregatedEADSummaryRowItem row = new(impactAreaNames[impactAreaID], withoutProjName, eadWithoutProjDamage, withProjName, withProjEAD, eadReduced, point75, point5, point25);
                    eadSummaryRowItems.Add(row);
                }
            }
            return eadSummaryRowItems;
        }

 
        private List<EADSummaryRowItem> CreateEADBaseYearSummaryTable(AlternativeComparisonReportResults results)
        {
            List<EADSummaryRowItem> eadSummaryRowItems = new();
            Dictionary<int, string> impactAreaNames = IASElement.GetImpactAreaNamesFromIDs();
            string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;

            
            foreach (int altID in WithProjAltIDs)
            {
                string withProjName = GetAlternativeElementFromID(altID).Name;
                foreach (int impactAreaID in results.GetImpactAreaIDs())
                {
                    foreach (string damcat in results.GetDamageCategories())
                    {
                        foreach (string assetType in results.GetAssetCategories())
                        {
                            double withProjEAD = results.MeanWithProjectBaseYearEAD(altID, impactAreaID, damcat, assetType);
                            double eadReduced = results.MeanBaseYearEADReduced(altID, impactAreaID, damcat, assetType);
                            double point75 = results.BaseYearEADReducedExceededWithProbabilityQ(.75, altID, impactAreaID, damcat, assetType);
                            double point5 = results.BaseYearEADReducedExceededWithProbabilityQ(.5, altID, impactAreaID, damcat, assetType);
                            double point25 = results.BaseYearEADReducedExceededWithProbabilityQ(.25, altID, impactAreaID, damcat, assetType);
                            double eadWithoutProjDamage = results.MeanWithoutProjectBaseYearEAD(impactAreaID, damcat, assetType);
                            EADSummaryRowItem row = new(impactAreaNames[impactAreaID],damcat, assetType, withoutProjName, eadWithoutProjDamage, withProjName, withProjEAD, eadReduced, point75, point5, point25);
                            eadSummaryRowItems.Add(row);
                        }
                    }
                }
            }
            return eadSummaryRowItems;
        }
        private List<AggregatedEADSummaryRowItem> CreateAggregatedEADBaseYearSummaryTable(AlternativeComparisonReportResults results)
        {
            List<AggregatedEADSummaryRowItem> eadSummaryRowItems = new();
            Dictionary<int, string> impactAreaNames = IASElement.GetImpactAreaNamesFromIDs();
            string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;

            foreach (int altID in WithProjAltIDs)
            {
                string withProjName = GetAlternativeElementFromID(altID).Name;
                foreach (int impactAreaID in results.GetImpactAreaIDs())
                {
                    double withProjEAD = results.MeanWithProjectBaseYearEAD(altID, impactAreaID);
                    double eadReduced = results.MeanBaseYearEADReduced(altID, impactAreaID);
                    double point75 = results.BaseYearEADReducedExceededWithProbabilityQ(.75, altID, impactAreaID);
                    double point5 = results.BaseYearEADReducedExceededWithProbabilityQ(.5, altID, impactAreaID);
                    double point25 = results.BaseYearEADReducedExceededWithProbabilityQ(.25, altID, impactAreaID);
                    double eadWithoutProjDamage = results.MeanWithoutProjectBaseYearEAD(); 
                    AggregatedEADSummaryRowItem row = new(impactAreaNames[impactAreaID], withoutProjName, eadWithoutProjDamage, withProjName, withProjEAD, eadReduced, point75, point5, point25);
                    eadSummaryRowItems.Add(row);
                }
            }
            return eadSummaryRowItems;
        }

        private List<AAEQSummaryRowItem> CreateAAEQSummaryTable(AlternativeComparisonReportResults results)
        {
            List<AAEQSummaryRowItem> aaeqSummaryRowItems = [];
            Dictionary<int, string> impactAreaNames = IASElement.GetImpactAreaNamesFromIDs();
            string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;
            
            foreach (int altID in WithProjAltIDs)
            {
                string withProjName = GetAlternativeElementFromID(altID).Name;
                foreach (int impactAreaID in results.GetImpactAreaIDs())
                {
                    foreach (string damcat in results.GetDamageCategories())
                    {
                        foreach (string assetType in results.GetAssetCategories())
                        {
                            double withProjAAEQ = results.MeanWithProjectAAEQDamage(altID, impactAreaID, damcat, assetType);
                            double aaeqReduced = results.MeanAAEQDamageReduced(altID, impactAreaID, damcat, assetType);
                            double point75 = results.AAEQDamageReducedExceededWithProbabilityQ(.75, altID, impactAreaID, damcat, assetType);
                            double point5 = results.AAEQDamageReducedExceededWithProbabilityQ(.5, altID, impactAreaID, damcat, assetType);
                            double point25 = results.AAEQDamageReducedExceededWithProbabilityQ(.25, altID, impactAreaID, damcat, assetType);
                            double aaeqWithoutProjDamage = results.MeanWithoutProjectAAEQDamage(impactAreaID, damcat, assetType);
                            AAEQSummaryRowItem row = new(impactAreaNames[impactAreaID], damcat, assetType, withoutProjName, aaeqWithoutProjDamage, withProjName, withProjAAEQ, aaeqReduced, point75, point5, point25);
                            aaeqSummaryRowItems.Add(row);
                        }
                    }

                }
            }
            return aaeqSummaryRowItems;
        }
        private List<AggregatedAAEQSummaryRowItem> CreateAggregatedAAEQSummaryTable(AlternativeComparisonReportResults results)
        {
            List<AggregatedAAEQSummaryRowItem> aaeqSummaryRowItems = [];
            Dictionary<int, string> impactAreaNames = IASElement.GetImpactAreaNamesFromIDs();
            string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;


            foreach (int altID in WithProjAltIDs)
            {
                string withProjName = GetAlternativeElementFromID(altID).Name;
                foreach (int impactAreaID in results.GetImpactAreaIDs())
                {
                    double withProjAAEQ = results.MeanWithProjectAAEQDamage(altID, impactAreaID);
                    double aaeqReduced = results.MeanAAEQDamageReduced(altID, impactAreaID);
                    double point75 = results.AAEQDamageReducedExceededWithProbabilityQ(.75, altID, impactAreaID);
                    double point5 = results.AAEQDamageReducedExceededWithProbabilityQ(.5, altID, impactAreaID);
                    double point25 = results.AAEQDamageReducedExceededWithProbabilityQ(.25, altID, impactAreaID);
                    double aaeqWithoutProjDamage = results.MeanWithoutProjectAAEQDamage();
                    AggregatedAAEQSummaryRowItem row = new(impactAreaNames[impactAreaID], withoutProjName, aaeqWithoutProjDamage, withProjName, withProjAAEQ, aaeqReduced, point75, point5, point25);
                    aaeqSummaryRowItems.Add(row);
                }
            }
            return aaeqSummaryRowItems;
        }

        public List<AlternativeElement> GetWithProjectAlternatives()
        {
            List<AlternativeElement> alts = new List<AlternativeElement>();
            foreach (int id in WithProjAltIDs)
            {
                AlternativeElement altElement = GetAlternativeElementFromID(id);
                if (altElement != null)
                {
                    alts.Add(altElement);
                }
            }
            return alts;
        }

        public AlternativeElement GetAlternativeElementFromID(int id)
        {
            AlternativeElement alt = null;
            //get the current ias elements in the study
            List<AlternativeElement> currentElementSets = StudyCache.GetChildElementsOfType<AlternativeElement>();
            foreach (AlternativeElement altElem in currentElementSets)
            {
                int setID = altElem.ID;
                if (setID == id)
                {
                    alt = altElem;
                }
            }
            return alt;
        }

        private FdaValidationResult DoAlternativesStillExistResult()
        {
            FdaValidationResult vr = new FdaValidationResult();

            AlternativeElement withoutAlt = GetAlternativeElementFromID(WithoutProjAltID);
            List<AlternativeElement> withProjAlts = GetWithProjectAlternatives();

            if (withoutAlt == null)
            {
                vr.AddErrorMessage("The without project alternative no longer exists.");
            }

            if (withProjAlts.Count == 0)
            {
                vr.AddErrorMessage("There are no longer any with project alternatives.");
            }

            foreach (int altID in WithProjAltIDs)
            {
                bool foundAlt = withProjAlts.Where(alt => alt.ID == altID).Any();
                if (!foundAlt)
                {
                    vr.AddErrorMessage("An alternative has been removed. Edit this alternative comparison report and try again.");
                    break;
                }
            }
            return vr;
        }

        private FdaValidationResult GetCanComputeResults()
        {
            FdaValidationResult vr = new FdaValidationResult();

            AlternativeElement withoutAlt = GetAlternativeElementFromID(WithoutProjAltID);
            List<AlternativeElement> withProjAlts = GetWithProjectAlternatives();

            FdaValidationResult withoutAltValidationResult = withoutAlt.RunPreComputeValidation();
            if (!withoutAltValidationResult.IsValid)
            {
                vr.AddErrorMessage("Alternative " + withoutAlt.Name + ":");
                vr.AddErrorMessage(withoutAltValidationResult.ErrorMessage);
                vr.AddErrorMessage(Environment.NewLine);
            }

            foreach (AlternativeElement altElem in withProjAlts)
            {
                FdaValidationResult withProjectValidationResult = altElem.RunPreComputeValidation();
                if (!withProjectValidationResult.IsValid)
                {
                    vr.AddErrorMessage("Alternative " + altElem.Name + ":");
                    vr.AddErrorMessage(withProjectValidationResult.ErrorMessage);
                    vr.AddErrorMessage(Environment.NewLine);
                }
            }

            return vr;
        }

        private SpecificAltCompReportResultsVM CreateAlternativeComparisonResult(int withProjID, string withProjName)
        {
            StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement();

            double discountRate = studyPropElem.DiscountRate;
            int period = studyPropElem.PeriodOfAnalysis;

            List<AlternativeElement> withProjAlts = GetWithProjectAlternatives();

            int baseYear = _Results.Years[0];
            int futureYear = _Results.Years[1];

            YearResult yr1 = new YearResult(baseYear, new DamageWithUncertaintyVM(_Results, withProjID, DamageMeasureYear.Base), new DamageByImpactAreaVM(_Results, withProjID, DamageMeasureYear.Base), new DamageByDamCatVM(_Results, DamageMeasureYear.Base, withProjID));
            YearResult yr2 = new YearResult(futureYear, new DamageWithUncertaintyVM(_Results, withProjID, DamageMeasureYear.Future), new DamageByImpactAreaVM(_Results, withProjID, DamageMeasureYear.Future), new DamageByDamCatVM(_Results, DamageMeasureYear.Future, withProjID));

            AAEQResult aaeqResult = new AAEQResult(new DamageWithUncertaintyVM(_Results, withProjID, DamageMeasureYear.AAEQ, discountRate, period), new DamageByImpactAreaVM(_Results, withProjID, DamageMeasureYear.AAEQ, discountRate, period), new DamageByDamCatVM(_Results, DamageMeasureYear.AAEQ, withProjID, discountRate, period));

            EADResult eadResult = new EADResult(new List<YearResult>() { yr1, yr2 });
            AlternativeResult altResult = new AlternativeResult(withProjName, eadResult, aaeqResult);

            return new SpecificAltCompReportResultsVM(altResult);
        }
    }
}
