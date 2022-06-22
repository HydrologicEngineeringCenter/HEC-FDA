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

        private AlternativeComparisonReportResults _AAEQResults;
        private AlternativeComparisonReportResults _EADBaseYearResults; 
        private AlternativeComparisonReportResults _EADFutureYearResults;
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

        public void ViewResults(object arg1, EventArgs arg2)
        {
            if (_AAEQResults != null)
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
            List < SpecificAltCompReportResultsVM > results = new List < SpecificAltCompReportResultsVM >();
            List<AAEQSummaryRowItem> aAEQSummaryRowItems = CreateAAEQSummaryTable();
            List<EADSummaryRowItem> eADBaseSummaryRowItems = CreateEADBaseYearSummaryTable();
            List<EADSummaryRowItem> eADFutureSummaryRowItems = CreateEADFutureYearSummaryTable();
            foreach (int altID in WithProjAltIDs)
            {
                SpecificAltCompReportResultsVM specificAltCompReportResultsVM = CreateAlternativeComparisonResult(altID, GetAlternativeElementFromID(altID).Name, eADBaseSummaryRowItems, eADFutureSummaryRowItems, aAEQSummaryRowItems);

                results.Add(specificAltCompReportResultsVM);
            }


            return results;
        }

        private List<EADSummaryRowItem> CreateEADFutureYearSummaryTable(AlternativeResults withoutProjResults, AlternativeResults futureYearResults)
        {
            List<EADSummaryRowItem> eadSummaryRowItems = new List<EADSummaryRowItem>();

            string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;
  
            double eadWithoutProjDamage = withoutProjResults.MeanConsequence();
            foreach (int altID in WithProjAltIDs)
            {
                string withProjName = GetAlternativeElementFromID(altID).Name;
 
                double withProjEAD = futureYearResults.MeanConsequence();

                double eadReduced = _EADFutureYearResults.MeanConsequencesReduced(altID);


                double point75 = _EADFutureYearResults.ConsequencesReducedExceededWithProbabilityQ( .75, altID);
                double point5 = _EADFutureYearResults.ConsequencesReducedExceededWithProbabilityQ(.5, altID);
                double point25 = _EADFutureYearResults.ConsequencesReducedExceededWithProbabilityQ(.25, altID);

                EADSummaryRowItem row = new EADSummaryRowItem(withoutProjName, eadWithoutProjDamage, withProjName, withProjEAD, eadReduced, point75, point5, point25);

                eadSummaryRowItems.Add(row);

            }
            return eadSummaryRowItems;
        }

        private List<EADSummaryRowItem> CreateEADBaseYearSummaryTable(AlternativeResults withoutProjResults, AlternativeResults baseYearResults)
        {
            List<EADSummaryRowItem> eadSummaryRowItems = new List<EADSummaryRowItem>();

            string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;
            //todo: get aaeq damage
            //double aaeqWithoutProjDamage = withoutProjAlt.ConsequenceResults.
            double eadWithoutProjDamage = _EADBaseYearResults.MeanConsequencesReduced(WithoutProjAltID);
            foreach (int altID in WithProjAltIDs)
            {
                string withProjName = GetAlternativeElementFromID(altID).Name;
                AlternativeResults withProjAlt = _EADBaseYearResults.GetAlternativeResults(altID);

                //todo: is this the right call?
                double withProjEAD = withProjAlt.MeanConsequence();

                //todo: what is aaeq reduced?
                double eadReduced = .222;

                double point75 = withProjAlt.ConsequencesExceededWithProbabilityQ(.75);
                double point5 = withProjAlt.ConsequencesExceededWithProbabilityQ(.75);
                double point25 = withProjAlt.ConsequencesExceededWithProbabilityQ(.75);

                EADSummaryRowItem row = new EADSummaryRowItem(withoutProjName, eadWithoutProjDamage, withProjName, withProjEAD, eadReduced, point75, point5, point25);

                eadSummaryRowItems.Add(row);

            }
            return eadSummaryRowItems;
        }

        private List<AAEQSummaryRowItem> CreateAAEQSummaryTable(AlternativeResults withoutProjResults, AlternativeResults futureYearResults)
        {
            List<AAEQSummaryRowItem> aAEQSummaryRowItems = new List<AAEQSummaryRowItem>();

            string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;
            //todo: get aaeq damage
            //double aaeqWithoutProjDamage = withoutProjAlt.ConsequenceResults.
            double aaeqWithoutProjDamage = _AAEQResults.MeanConsequencesReduced(WithoutProjAltID);
            foreach (int altID in WithProjAltIDs)
            {
                string withProjName = GetAlternativeElementFromID(altID).Name;
                AlternativeResults withProjAlt = _AAEQResults.GetAlternativeResults(altID);

                //todo: is this the right call?
                double withProjAAEQ = withProjAlt.MeanConsequence();

                //todo: what is aaeq reduced?
                double aaeqReduced = .222;

                double point75 = withProjAlt.ConsequencesExceededWithProbabilityQ(.75);
                double point5 = withProjAlt.ConsequencesExceededWithProbabilityQ(.75);
                double point25 = withProjAlt.ConsequencesExceededWithProbabilityQ(.75);

                AAEQSummaryRowItem row = new AAEQSummaryRowItem(withoutProjName, aaeqWithoutProjDamage, withProjName, withProjAAEQ, aaeqReduced, point75, point5, point25);

                aAEQSummaryRowItems.Add(row);

            }
            return aAEQSummaryRowItems;
        }

        public void ComputeAlternative(object arg1, EventArgs arg2)
        {
            FdaValidationResult vr = GetCanComputeResults();
            if(vr.IsValid)
            {
                AlternativeElement withoutAlt = GetAlternativeElementFromID(WithoutProjAltID);
                AlternativeResults withoutAltResults = withoutAlt.Results;
                List<AlternativeElement> withProjAlts = GetWithProjectAlternatives();
                List<AlternativeResults> withResults = new List<AlternativeResults>();
                foreach(AlternativeElement elem in withProjAlts)
                {
                    withResults.Add(elem.Results);
                }

                int seed = 99;
                RandomProvider randomProvider = new RandomProvider(seed);
                ConvergenceCriteria cc = new ConvergenceCriteria();

                _AAEQResults = alternativeComparisonReport.AlternativeComparisonReport.ComputeDistributionOfAAEQDamageReduced(randomProvider, cc, withoutAltResults, withResults);
                _EADBaseYearResults =  alternativeComparisonReport.AlternativeComparisonReport.ComputeDistributionEADReduced(randomProvider, withoutAltResults, withResults, true);
                _EADFutureYearResults = alternativeComparisonReport.AlternativeComparisonReport.ComputeDistributionEADReduced(randomProvider, withoutAltResults, withResults, false);

                Saving.PersistenceFactory.GetAlternativeCompReportManager().SaveExisting(this);

                MessageBoxResult messageBoxResult = MessageBox.Show("Compute completed. Would you like to view the results?", "Compute Complete", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    ViewResults(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show(vr.ErrorMessage, "Cannot Compute", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
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

            if(withoutAlt.Results == null)
            {
                vr.AddErrorMessage("The without project alternative, " + withoutAlt.Name + ", has no results.");
            }
            foreach(AlternativeElement altElem in withProjAlts)
            {
                if(altElem.Results == null)
                {
                    vr.AddErrorMessage("The with project alternative, " + altElem.Name + ", has no results.");
                }
            }

            return vr;
        }


        /// <summary>
        /// This is a dummy result object that Cody created to fill the results UI with dummy data.
        /// </summary>
        /// <returns></returns>
        private SpecificAltCompReportResultsVM CreateAlternativeComparisonResult(int withProjID, string withProjName, List<EADSummaryRowItem> baseSummary, List<EADSummaryRowItem> futureSummary, List<AAEQSummaryRowItem> aaeqSummary)
        { 
            StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement();

            double discountRate = studyPropElem.DiscountRate;
            int period = studyPropElem.PeriodOfAnalysis;

            List<AlternativeElement> withProjAlts = GetWithProjectAlternatives();

            //TODO: richard will add a method to get the year results.
            //_EADBaseYearResults.BaseYear
            //todo: delete these hard coded years.
            int baseYear = _EADBaseYearResults.Years[0];
            int futureYear = _EADFutureYearResults.Years[0];

            YearResult yr1 = new YearResult(baseYear, new DamageWithUncertaintyVM(discountRate, period, _AAEQResults, withProjID), new DamageByImpactAreaVM(discountRate, period, _EADBaseYearResults, withProjID), new DamageByDamCatVM(_EADBaseYearResults, withProjID));
            YearResult yr2 = new YearResult(futureYear, new DamageWithUncertaintyVM(discountRate, period, _AAEQResults, withProjID), new DamageByImpactAreaVM(discountRate, period, _EADFutureYearResults, withProjID), new DamageByDamCatVM(_EADFutureYearResults, withProjID));

            AAEQResult aaeqResult = new AAEQResult(new DamageWithUncertaintyVM(discountRate, period, _AAEQResults, withProjID), new DamageByImpactAreaVM(discountRate, period, _AAEQResults, withProjID), new DamageByDamCatVM(_AAEQResults, withProjID));

            EADResult eadResult = new EADResult(new List<YearResult>() { yr1, yr2 });
            AlternativeResult altResult = new AlternativeResult(withProjName, eadResult, aaeqResult);


            return new SpecificAltCompReportResultsVM(altResult, baseSummary, futureSummary, aaeqSummary);
        }
    }
}
