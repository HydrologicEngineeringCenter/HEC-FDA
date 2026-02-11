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
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Utility;
using Visual.Observables;
using static HEC.FDA.ViewModel.ImpactAreaScenario.Results.UncertaintyControlConfigs;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport;

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

    public async void ComputeAltCompReport(object arg1, EventArgs arg2)
    {
        FdaValidationResult canComputeValidationResult = GetValidationResult();
        if (canComputeValidationResult.IsValid)
        {
            AlternativeElement withoutAlt = GetAlternativeElementFromID(WithoutProjAltID);
            List<AlternativeElement> withProjAlts = GetWithProjectAlternatives();

            ISynchronizationContext context = new SynchronizationContext(action => Application.Current.Dispatcher.BeginInvoke(action));
            BatchJob batchJob = new(uiThreadSyncContext: context);
            ComputeAltCompReportVM compVM = new(batchJob);
            string header = "Compute Log For Alternative: " + Name;
            DynamicTabVM tab = new(header, compVM, "ComputeLog" + Name);
            Navigate(tab, false, false);

            var props = StudyCache.GetStudyPropertiesElement();
            var woAltResult = await AlternativeComputer.RunAnnualizationCompute(withoutAlt, props);
            AlternativeResults[] withProjAltsResults = new AlternativeResults[withProjAlts.Count];
            for (int i = 0; i < withProjAlts.Count; i++)
            {
                withProjAltsResults[i] = await AlternativeComputer.RunAnnualizationCompute(withProjAlts[i], props);
            }

            _Results = await Task.Run(() => Model.alternativeComparisonReport.AlternativeComparisonReport.ComputeAlternativeComparisonReport(woAltResult, withProjAltsResults, batchJob.Reporter));
            ViewResults();
        }
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
        List<EqadSummaryRowItem> eqadSummaryRowItems = CreateEqadSummaryTable(_Results);
        List<EADSummaryRowItem> eADBaseSummaryRowItems = CreateEADBaseYearSummaryTable(_Results);
        List<EADSummaryRowItem> eADFutureSummaryRowItems = CreateEADFutureYearSummaryTable(_Results);

        List<AggregatedEqadSummaryRowItem> eqadAggregatedSummaryRowItems = CreateAggregatedEqadSummaryTable(_Results);
        List<AggregatedEADSummaryRowItem> eADAggregatedSummaryRowItems = CreateAggregatedEADFutureYearSummaryTable(_Results);
        List<AggregatedAALLSummaryRowItem> aALLAggregatedSummaryRowItems = CreateAggregatedAALLFutureYearSummaryTable(_Results);
        List<AggregatedEADSummaryRowItem> eADAggregatedBaseSummaryRowItems = CreateAggregatedEADBaseYearSummaryTable(_Results);
        List<AggregatedAALLSummaryRowItem> aALLAggregatedBaseSummaryRowItems = CreateAggregatedAALLBaseYearSummaryTable(_Results);

        // Check for available result types
        bool hasDamageResults = HasDamageResults();
        bool hasLifeLossResults = HasLifeLossResults();

        foreach (int altID in WithProjAltIDs)
        {
            SpecificAltCompReportResultsVM specificAltCompReportResultsVM = CreateAlternativeComparisonResult(altID, GetAlternativeElementFromID(altID).Name, hasDamageResults, hasLifeLossResults);
            results.Add(specificAltCompReportResultsVM);
        }

        SpecificAltCompReportResultsVM summaryOption = new SummaryVM(
            eADBaseSummaryRowItems, eADFutureSummaryRowItems, eqadSummaryRowItems,
            eADAggregatedBaseSummaryRowItems, aALLAggregatedBaseSummaryRowItems, eADAggregatedSummaryRowItems, aALLAggregatedSummaryRowItems,
            eqadAggregatedSummaryRowItems, _Results.Years, hasDamageResults, hasLifeLossResults);
        results.Add(summaryOption);
        return results;

    }

    private bool HasDamageResults()
    {
        return _Results.GetImpactAreaIDs(ConsequenceType.Damage).Count > 0;
    }

    private bool HasLifeLossResults()
    {
        return _Results.GetImpactAreaIDs(ConsequenceType.LifeLoss).Count > 0;
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
                        double withProjEAD = results.SampleMeanWithProjectFutureYearEAD(altID, impactAreaID, damcat, assetType);
                        double eadReduced = results.SampleMeanFutureYearEADReduced(altID, impactAreaID, damcat, assetType);
                        double point75 = results.FutureYearEADReducedExceededWithProbabilityQ(.75, altID, impactAreaID, damcat, assetType);
                        double point5 = results.FutureYearEADReducedExceededWithProbabilityQ(.5, altID, impactAreaID, damcat, assetType);
                        double point25 = results.FutureYearEADReducedExceededWithProbabilityQ(.25, altID, impactAreaID, damcat, assetType);
                        double eadWithoutProjDamage = results.SampleMeanWithoutProjectFutureYearEAD(impactAreaID, damcat, assetType);
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
                double withProjEAD = results.SampleMeanWithProjectFutureYearEAD(altID, impactAreaID);
                double eadReduced = results.SampleMeanFutureYearEADReduced(altID, impactAreaID);
                double point75 = results.FutureYearEADReducedExceededWithProbabilityQ(.75, altID, impactAreaID);
                double point5 = results.FutureYearEADReducedExceededWithProbabilityQ(.5, altID, impactAreaID);
                double point25 = results.FutureYearEADReducedExceededWithProbabilityQ(.25, altID, impactAreaID);
                double eadWithoutProjDamage = results.SampleMeanWithoutProjectFutureYearEAD(impactAreaID);
                AggregatedEADSummaryRowItem row = new(impactAreaNames[impactAreaID], withoutProjName, eadWithoutProjDamage, withProjName, withProjEAD, eadReduced, point75, point5, point25);
                eadSummaryRowItems.Add(row);
            }
        }
        return eadSummaryRowItems;
    }

    private List<AggregatedAALLSummaryRowItem> CreateAggregatedAALLFutureYearSummaryTable(AlternativeComparisonReportResults results)
    {
        List<AggregatedAALLSummaryRowItem> eallSummaryRowItems = new();
        Dictionary<int, string> impactAreaNames = IASElement.GetImpactAreaNamesFromIDs();
        string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;


        foreach (int altID in WithProjAltIDs)
        {
            string withProjName = GetAlternativeElementFromID(altID).Name;
            foreach (int impactAreaID in results.GetImpactAreaIDs())
            {
                double withProjEALL = results.SampleMeanWithProjectFutureYearEAD(altID, impactAreaID, consequenceType: ConsequenceType.LifeLoss);
                double eallReduced = results.SampleMeanFutureYearEADReduced(altID, impactAreaID, consequenceType: ConsequenceType.LifeLoss);
                double point75 = results.FutureYearEADReducedExceededWithProbabilityQ(.75, altID, impactAreaID, consequenceType: ConsequenceType.LifeLoss);
                double point5 = results.FutureYearEADReducedExceededWithProbabilityQ(.5, altID, impactAreaID, consequenceType: ConsequenceType.LifeLoss);
                double point25 = results.FutureYearEADReducedExceededWithProbabilityQ(.25, altID, impactAreaID, consequenceType: ConsequenceType.LifeLoss);
                double eallWithoutProjDamage = results.SampleMeanWithoutProjectFutureYearEAD(impactAreaID, consequenceType: ConsequenceType.LifeLoss);
                AggregatedAALLSummaryRowItem row = new(impactAreaNames[impactAreaID], withoutProjName, eallWithoutProjDamage, withProjName, withProjEALL, eallReduced, point75, point5, point25);
                eallSummaryRowItems.Add(row);
            }
        }
        return eallSummaryRowItems;
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
                        double withProjEAD = results.SampleMeanWithProjectBaseYearEAD(altID, impactAreaID, damcat, assetType);
                        double eadReduced = results.SampleMeanBaseYearEADReduced(altID, impactAreaID, damcat, assetType);
                        double point75 = results.BaseYearEADReducedExceededWithProbabilityQ(.75, altID, impactAreaID, damcat, assetType);
                        double point5 = results.BaseYearEADReducedExceededWithProbabilityQ(.5, altID, impactAreaID, damcat, assetType);
                        double point25 = results.BaseYearEADReducedExceededWithProbabilityQ(.25, altID, impactAreaID, damcat, assetType);
                        double eadWithoutProjDamage = results.SampleMeanWithoutProjectBaseYearEAD(impactAreaID, damcat, assetType);
                        EADSummaryRowItem row = new(impactAreaNames[impactAreaID], damcat, assetType, withoutProjName, eadWithoutProjDamage, withProjName, withProjEAD, eadReduced, point75, point5, point25);
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
                double withProjEAD = results.SampleMeanWithProjectBaseYearEAD(altID, impactAreaID);
                double eadReduced = results.SampleMeanBaseYearEADReduced(altID, impactAreaID);
                double point75 = results.BaseYearEADReducedExceededWithProbabilityQ(.75, altID, impactAreaID);
                double point5 = results.BaseYearEADReducedExceededWithProbabilityQ(.5, altID, impactAreaID);
                double point25 = results.BaseYearEADReducedExceededWithProbabilityQ(.25, altID, impactAreaID);
                double eadWithoutProjDamage = results.SampleMeanWithoutProjectBaseYearEAD(impactAreaID);
                AggregatedEADSummaryRowItem row = new(impactAreaNames[impactAreaID], withoutProjName, eadWithoutProjDamage, withProjName, withProjEAD, eadReduced, point75, point5, point25);
                eadSummaryRowItems.Add(row);
            }
        }
        return eadSummaryRowItems;
    }

    private List<AggregatedAALLSummaryRowItem> CreateAggregatedAALLBaseYearSummaryTable(AlternativeComparisonReportResults results)
    {
        List<AggregatedAALLSummaryRowItem> eallSummaryRowItems = new();
        Dictionary<int, string> impactAreaNames = IASElement.GetImpactAreaNamesFromIDs();
        string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;

        foreach (int altID in WithProjAltIDs)
        {
            string withProjName = GetAlternativeElementFromID(altID).Name;
            foreach (int impactAreaID in results.GetImpactAreaIDs())
            {
                double withProjEALL = results.SampleMeanWithProjectBaseYearEAD(altID, impactAreaID, consequenceType: ConsequenceType.LifeLoss);
                double eallReduced = results.SampleMeanBaseYearEADReduced(altID, impactAreaID, consequenceType: ConsequenceType.LifeLoss);
                double point75 = results.BaseYearEADReducedExceededWithProbabilityQ(.75, altID, impactAreaID, consequenceType: ConsequenceType.LifeLoss);
                double point5 = results.BaseYearEADReducedExceededWithProbabilityQ(.5, altID, impactAreaID, consequenceType: ConsequenceType.LifeLoss);
                double point25 = results.BaseYearEADReducedExceededWithProbabilityQ(.25, altID, impactAreaID, consequenceType: ConsequenceType.LifeLoss);
                double eallWithoutProjLL = results.SampleMeanWithoutProjectBaseYearEAD(impactAreaID, consequenceType: ConsequenceType.LifeLoss);
                AggregatedAALLSummaryRowItem row = new(impactAreaNames[impactAreaID], withoutProjName, eallWithoutProjLL, withProjName, withProjEALL, eallReduced, point75, point5, point25);
                eallSummaryRowItems.Add(row);
            }
        }
        return eallSummaryRowItems;
    }

    private List<EqadSummaryRowItem> CreateEqadSummaryTable(AlternativeComparisonReportResults results)
    {
        List<EqadSummaryRowItem> eqadSummaryRowItems = [];
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
                        double withProjEqad = results.SampleMeanWithProjectEqad(altID, impactAreaID, damcat, assetType);
                        double eqadReduced = results.SampleMeanEqadReduced(altID, impactAreaID, damcat, assetType);
                        double point75 = results.EqadReducedExceededWithProbabilityQ(.75, altID, impactAreaID, damcat, assetType);
                        double point5 = results.EqadReducedExceededWithProbabilityQ(.5, altID, impactAreaID, damcat, assetType);
                        double point25 = results.EqadReducedExceededWithProbabilityQ(.25, altID, impactAreaID, damcat, assetType);
                        double eqadWithoutProjDamage = results.SampleMeanWithoutProjectEqad(impactAreaID, damcat, assetType);
                        EqadSummaryRowItem row = new(impactAreaNames[impactAreaID], damcat, assetType, withoutProjName, eqadWithoutProjDamage, withProjName, withProjEqad, eqadReduced, point75, point5, point25);
                        eqadSummaryRowItems.Add(row);
                    }
                }

            }
        }
        return eqadSummaryRowItems;
    }
    private List<AggregatedEqadSummaryRowItem> CreateAggregatedEqadSummaryTable(AlternativeComparisonReportResults results)
    {
        List<AggregatedEqadSummaryRowItem> eqadSummaryRowItems = [];
        Dictionary<int, string> impactAreaNames = IASElement.GetImpactAreaNamesFromIDs();
        string withoutProjName = GetAlternativeElementFromID(WithoutProjAltID).Name;


        foreach (int altID in WithProjAltIDs)
        {
            string withProjName = GetAlternativeElementFromID(altID).Name;
            foreach (int impactAreaID in results.GetImpactAreaIDs())
            {
                double withProjEqad = results.SampleMeanWithProjectEqad(altID, impactAreaID);
                double eqadReduced = results.SampleMeanEqadReduced(altID, impactAreaID);
                double point75 = results.EqadReducedExceededWithProbabilityQ(.75, altID, impactAreaID);
                double point5 = results.EqadReducedExceededWithProbabilityQ(.5, altID, impactAreaID);
                double point25 = results.EqadReducedExceededWithProbabilityQ(.25, altID, impactAreaID);
                double eqadWithoutProjDamage = results.SampleMeanWithoutProjectEqad(impactAreaID);
                AggregatedEqadSummaryRowItem row = new(impactAreaNames[impactAreaID], withoutProjName, eqadWithoutProjDamage, withProjName, withProjEqad, eqadReduced, point75, point5, point25);
                eqadSummaryRowItems.Add(row);
            }
        }
        return eqadSummaryRowItems;
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

    private SpecificAltCompReportResultsVM CreateAlternativeComparisonResult(int withProjID, string withProjName, bool hasDamageResults, bool hasLifeLossResults)
    {
        StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement();

        double discountRate = studyPropElem.DiscountRate;
        int period = studyPropElem.PeriodOfAnalysis;

        List<AlternativeElement> withProjAlts = GetWithProjectAlternatives();

        int baseYear = _Results.Years[0];
        int futureYear = _Results.Years[1];

        YearResult yr1 = new YearResult(
            baseYear,
            new DamageWithUncertaintyVM(_Results, withProjID, DamageMeasureYear.Base, new DamageReducedWithUncertaintyControlConfig()),
            new DamageByImpactAreaVM(_Results, withProjID, DamageMeasureYear.Base),
            new DamageByDamCatVM(_Results, DamageMeasureYear.Base, withProjID),
            new DamageWithUncertaintyVM(_Results, withProjID, DamageMeasureYear.Base, new LifeLossReducedWithUncertaintyControlConfig()),
            new DamageByImpactAreaVM(_Results, withProjID, DamageMeasureYear.Base, consequenceType: ConsequenceType.LifeLoss));
        YearResult yr2 = new YearResult(futureYear,
            new DamageWithUncertaintyVM(_Results, withProjID, DamageMeasureYear.Future, new DamageReducedWithUncertaintyControlConfig()),
            new DamageByImpactAreaVM(_Results, withProjID, DamageMeasureYear.Future),
            new DamageByDamCatVM(_Results, DamageMeasureYear.Future, withProjID),
            new DamageWithUncertaintyVM(_Results, withProjID, DamageMeasureYear.Future, new LifeLossReducedWithUncertaintyControlConfig()),
            new DamageByImpactAreaVM(_Results, withProjID, DamageMeasureYear.Future, consequenceType: ConsequenceType.LifeLoss));

        EqadResult eqadResult = new EqadResult(
            new DamageWithUncertaintyVM(_Results, withProjID, DamageMeasureYear.Eqad, new EqADReducedWithUncertaintyControlConfig(), discountRate, period),
            new DamageByImpactAreaVM(_Results, withProjID, DamageMeasureYear.Eqad, discountRate, period),
            new DamageByDamCatVM(_Results, DamageMeasureYear.Eqad, withProjID, discountRate, period));

        EADResult eadResult = new EADResult(new List<YearResult>() { yr1, yr2 });
        AlternativeResult altResult = new AlternativeResult(withProjName, eadResult, eqadResult);

        return new SpecificAltCompReportResultsVM(altResult, hasDamageResults, hasLifeLossResults);
    }
}
