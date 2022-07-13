using compute;
using HEC.FDA.ViewModel.AlternativeComparisonReport.Results;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.Alternatives.Results;
using HEC.FDA.ViewModel.Alternatives.Results.ResultObject;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using metrics;
using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport
{
    public class AlternativeComparisonReportElement : ChildElement
    {
        private const string ALTERNATIVE_COMP_REPORT = "AlternativeComparisonReport";
        private const string NAME = "Name";
        private const string DESCRIPTION = "Description";
        private const string ID_STRING = "ID";
        private const string WITHOUT_PROJ_ID = "WithoutProjID";
        private const string WITH_PROJ_ELEM = "WithProjectElement";
        private const string LAST_EDIT_DATE = "LastEditDate";

        private AlternativeComparisonReportResults _Results;

        #region Properties
        public int WithoutProjAltID { get; }
        public List<int> WithProjAltIDs { get; } = new List<int>();
        public AlternativeComparisonReportResults Results { get; }

        #endregion

        public AlternativeComparisonReportElement(string name, string desc, string creationDate, int withoutProjectAltId, List<int> withProjAlternativeIds, int id) : base(id)
        {
            Name = name;
            Description = desc;
            WithoutProjAltID = withoutProjectAltId;
            WithProjAltIDs = withProjAlternativeIds;
            CustomTreeViewHeader = new CustomHeaderVM(Name)
            {
                ImageSource = ImageSources.ALTERNATIVE_COMPARISON_REPORT_IMAGE,
                Tooltip = StringConstants.CreateLastEditTooltip(creationDate)
            };
            AddActions();
        }

        /// <summary>
        /// The ctor used to load an element from the database.
        /// </summary>
        /// <param name="xml"></param>
        public AlternativeComparisonReportElement(string xml, int id) : base(id)
        {
            XDocument doc = XDocument.Parse(xml);
            XElement altElement = doc.Element(ALTERNATIVE_COMP_REPORT);
            Name = altElement.Attribute(NAME).Value;
            Description = altElement.Attribute(DESCRIPTION).Value;
            WithoutProjAltID = int.Parse(altElement.Attribute(WITHOUT_PROJ_ID).Value);
            LastEditDate = altElement.Attribute(LAST_EDIT_DATE).Value;

            IEnumerable<XElement> altElements = altElement.Elements(WITH_PROJ_ELEM);
            foreach (XElement elem in altElements)
            {
                int iasID = int.Parse(elem.Attribute(ID_STRING).Value);
                WithProjAltIDs.Add(iasID);
            }
            CustomTreeViewHeader = new CustomHeaderVM(Name)
            {
                ImageSource = ImageSources.ALTERNATIVE_COMPARISON_REPORT_IMAGE,
                Tooltip = StringConstants.CreateLastEditTooltip(LastEditDate)
            };

            AddActions();
        }

        private void AddActions()
        {
            NamedAction edit = new NamedAction();
            edit.Header = StringConstants.EDIT_ALTERNATIVE_COMP_REPORTS_MENU;
            edit.Action = EditAlternative;

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
            localActions.Add(viewResults);
            localActions.Add(removeCondition);
            localActions.Add(renameElement);

            Actions = localActions;
        }

        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            AlternativeComparisonReportElement elemToReturn = null;
            if (elementToClone is AlternativeComparisonReportElement)
            {
                AlternativeComparisonReportElement elem = (AlternativeComparisonReportElement)elementToClone;
                elemToReturn = new AlternativeComparisonReportElement(elem.Name, elem.Description, elem.LastEditDate, elem.WithoutProjAltID, elem.WithProjAltIDs, elem.ID);
            }
            return elemToReturn;
        }

        public string WriteToXML()
        {
            XElement altElement = new XElement(ALTERNATIVE_COMP_REPORT);
            altElement.SetAttributeValue(NAME, Name);
            altElement.SetAttributeValue(DESCRIPTION, Description);
            altElement.SetAttributeValue(WITHOUT_PROJ_ID, WithoutProjAltID);
            altElement.SetAttributeValue(LAST_EDIT_DATE, LastEditDate);

            foreach (int elemID in WithProjAltIDs)
            {
                XElement setElement = new XElement(WITH_PROJ_ELEM);
                setElement.SetAttributeValue(ID_STRING, elemID);
                altElement.Add(setElement);
            }
            return altElement.ToString();
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

        public void ComputeAltCompReport(AlternativeResults withoutAltResults, List<AlternativeResults> withResults)
        {
            int seed = 99;
            RandomProvider randomProvider = new RandomProvider(seed);
            ConvergenceCriteria cc = new ConvergenceCriteria();

            _Results = alternativeComparisonReport.AlternativeComparisonReport.ComputeAlternativeComparisonReport(randomProvider, cc, withoutAltResults, withResults);
        }       

        public void ViewResults(object arg1, EventArgs arg2)
        {
            FdaValidationResult canComputeValidationResult = GetCanComputeResults();
            if (canComputeValidationResult.IsValid)
            {
                List<AlternativeResults> withResults = new List<AlternativeResults>();

                //everything should be good to start computing. Start by computing alternatives
                AlternativeElement withoutAlt = GetAlternativeElementFromID(WithoutProjAltID);
                List<AlternativeElement> withProjAlts = GetWithProjectAlternatives();

                AlternativeResults withoutProjResults = withoutAlt.ComputeAlternative();
                if (withoutProjResults == null)
                {
                    //This should never happen.
                    canComputeValidationResult.AddErrorMessage(withoutAlt.Name + " compute produced no results.");
                }
                foreach(AlternativeElement withProjElem in withProjAlts)
                {
                    AlternativeResults withProjResults = withProjElem.ComputeAlternative();
                    if (withProjResults == null)
                    {
                        //This should never happen.
                        canComputeValidationResult.AddErrorMessage(withProjElem.Name + " compute produced no results.");
                    }
                    else
                    {
                        withResults.Add(withProjResults);
                    }
                }

                if (canComputeValidationResult.IsValid)
                {
                    ComputeAltCompReport(withoutProjResults, withResults);
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
                else
                {
                    MessageBox.Show("There are no results to view.", "No Results", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show(canComputeValidationResult.ErrorMessage, "Cannot Compute Alternative Comparison Report", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private List<SpecificAltCompReportResultsVM> CreateResults()
        {
            List < SpecificAltCompReportResultsVM > results = new List < SpecificAltCompReportResultsVM >();
            List<AAEQSummaryRowItem> aAEQSummaryRowItems = CreateAAEQSummaryTable(_Results);
            List<EADSummaryRowItem> eADBaseSummaryRowItems = CreateEADBaseYearSummaryTable(_Results);
            List<EADSummaryRowItem> eADFutureSummaryRowItems = CreateEADFutureYearSummaryTable(_Results);
            foreach (int altID in WithProjAltIDs)
            {
                SpecificAltCompReportResultsVM specificAltCompReportResultsVM = CreateAlternativeComparisonResult(altID, GetAlternativeElementFromID(altID).Name, eADBaseSummaryRowItems, eADFutureSummaryRowItems, aAEQSummaryRowItems);

                results.Add(specificAltCompReportResultsVM);
            }


            return results;
        }

        private List<EADSummaryRowItem> CreateEADFutureYearSummaryTable(AlternativeComparisonReportResults results)
        {
            List<EADSummaryRowItem> eadSummaryRowItems = new List<EADSummaryRowItem>();

            string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;

            double eadWithoutProjDamage = results.MeanWithoutProjectFutureYearEAD();
            foreach (int altID in WithProjAltIDs)
            {
                string withProjName = GetAlternativeElementFromID(altID).Name;

                double withProjEAD = results.MeanWithProjectFutureYearEAD(altID);

                double eadReduced = results.MeanFutureYearEADReduced(altID);


                double point75 = results.FutureYearEADReducedExceededWithProbabilityQ( .75, altID);
                double point5 = results.FutureYearEADReducedExceededWithProbabilityQ(.5, altID);
                double point25 = results.FutureYearEADReducedExceededWithProbabilityQ(.25, altID);

                EADSummaryRowItem row = new EADSummaryRowItem(withoutProjName, eadWithoutProjDamage, withProjName, withProjEAD, eadReduced, point75, point5, point25);

                eadSummaryRowItems.Add(row);

            }
            return eadSummaryRowItems;
        }

        private List<EADSummaryRowItem> CreateEADBaseYearSummaryTable(AlternativeComparisonReportResults results)
        {
            List<EADSummaryRowItem> eadSummaryRowItems = new List<EADSummaryRowItem>();

            string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;

            double eadWithoutProjDamage = results.MeanWithoutProjectBaseYearEAD();
            foreach (int altID in WithProjAltIDs)
            {
                string withProjName = GetAlternativeElementFromID(altID).Name;

                double withProjEAD = results.MeanWithProjectBaseYearEAD(altID);

                double eadReduced = results.MeanBaseYearEADReduced(altID);


                double point75 = results.BaseYearEADReducedExceededWithProbabilityQ(.75, altID);
                double point5 = results.BaseYearEADReducedExceededWithProbabilityQ(.5, altID);
                double point25 = results.BaseYearEADReducedExceededWithProbabilityQ(.25, altID);

                EADSummaryRowItem row = new EADSummaryRowItem(withoutProjName, eadWithoutProjDamage, withProjName, withProjEAD, eadReduced, point75, point5, point25);

                eadSummaryRowItems.Add(row);

            }
            return eadSummaryRowItems;
        }

        private List<AAEQSummaryRowItem> CreateAAEQSummaryTable(AlternativeComparisonReportResults results)
        {
            List<AAEQSummaryRowItem> aaeqSummaryRowItems = new List<AAEQSummaryRowItem>();

            string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;

            double aaeqWithoutProjDamage = results.MeanWithoutProjectAAEQDamage();
            foreach (int altID in WithProjAltIDs)
            {
                string withProjName = GetAlternativeElementFromID(altID).Name;

                double withProjEAD = results.MeanWithoutProjectAAEQDamage(altID);

                double aaeqReduced = results.MeanAAEQDamageReduced(altID);


                double point75 = results.AAEQDamageReducedExceededWithProbabilityQ(.75, altID);
                double point5 = results.AAEQDamageReducedExceededWithProbabilityQ(.5, altID);
                double point25 = results.AAEQDamageReducedExceededWithProbabilityQ(.25, altID);

                AAEQSummaryRowItem row = new AAEQSummaryRowItem(withoutProjName, aaeqWithoutProjDamage, withProjName, withProjEAD, aaeqReduced, point75, point5, point25);

                aaeqSummaryRowItems.Add(row);

            }
            return aaeqSummaryRowItems;
        }
  
        private List<AlternativeElement> GetWithProjectAlternatives()
        {
            List<AlternativeElement> alts = new List<AlternativeElement>();
            foreach(int id in WithProjAltIDs )
            {
                AlternativeElement altElement = GetAlternativeElementFromID(id);
                if(altElement != null)
                {
                    alts.Add(altElement);
                }
            }
            return alts;
        }

        private AlternativeElement GetAlternativeElementFromID(int id)
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

        private FdaValidationResult GetCanComputeResults()
        {
            FdaValidationResult vr = new FdaValidationResult();
            
            AlternativeElement withoutAlt = GetAlternativeElementFromID(WithoutProjAltID);
            List<AlternativeElement> withProjAlts = GetWithProjectAlternatives();

            if(withoutAlt == null)
            {
                vr.AddErrorMessage("The without project alternative no longer exists.");
            }

            if(withProjAlts.Count==0)
            {
                vr.AddErrorMessage("There are no longer any with project alternatives.");
            }

            foreach(int altID in WithProjAltIDs)
            {
                bool foundAlt = withProjAlts.Where(alt => alt.ID == altID).Any();
                if(!foundAlt)
                {
                    vr.AddErrorMessage("An alternative has been removed. Edit this alternative comparison report and try again.");
                    break;
                }
            }

            FdaValidationResult withoutAltValidationResult = withoutAlt.RunPreComputeValidation();
            if(!withoutAltValidationResult.IsValid)
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

        private SpecificAltCompReportResultsVM CreateAlternativeComparisonResult(int withProjID, string withProjName, List<EADSummaryRowItem> baseSummary, List<EADSummaryRowItem> futureSummary, List<AAEQSummaryRowItem> aaeqSummary)
        { 
            StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement();

            double discountRate = studyPropElem.DiscountRate;
            int period = studyPropElem.PeriodOfAnalysis;

            List<AlternativeElement> withProjAlts = GetWithProjectAlternatives();

            int baseYear = _Results.Years[0];
            int futureYear = _Results.Years[1];

            YearResult yr1 = new YearResult(baseYear, new DamageWithUncertaintyVM( _Results, withProjID, DamageMeasureYear.Base), new DamageByImpactAreaVM( _Results, withProjID, DamageMeasureYear.Base), new DamageByDamCatVM(_Results, DamageMeasureYear.Base, withProjID));
            YearResult yr2 = new YearResult(futureYear, new DamageWithUncertaintyVM( _Results, withProjID, DamageMeasureYear.Future), new DamageByImpactAreaVM( _Results, withProjID, DamageMeasureYear.Future), new DamageByDamCatVM(_Results, DamageMeasureYear.Future, withProjID));

            AAEQResult aaeqResult = new AAEQResult(new DamageWithUncertaintyVM( _Results, withProjID, DamageMeasureYear.AAEQ, discountRate, period), new DamageByImpactAreaVM( _Results, withProjID, DamageMeasureYear.AAEQ, discountRate, period), new DamageByDamCatVM(_Results, DamageMeasureYear.AAEQ, withProjID, discountRate, period));

            EADResult eadResult = new EADResult(new List<YearResult>() { yr1, yr2 });
            AlternativeResult altResult = new AlternativeResult(withProjName, eadResult, aaeqResult);

            return new SpecificAltCompReportResultsVM(altResult, baseSummary, futureSummary, aaeqSummary);
        }
    }
}
