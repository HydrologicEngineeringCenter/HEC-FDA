using alternatives;
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
            WithoutProjAltID = Int32.Parse(altElement.Attribute(WITHOUT_PROJ_ID).Value);
            LastEditDate = altElement.Attribute(LAST_EDIT_DATE).Value;

            IEnumerable<XElement> altElements = altElement.Elements(WITH_PROJ_ELEM);
            foreach (XElement elem in altElements)
            {
                int iasID = Int32.Parse(elem.Attribute(ID_STRING).Value);
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
            if (_Results != null)
            {
                //AlternativeComparisonReportResultsVM vm = new AlternativeComparisonReportResultsVM(CreateAlternativeResult());
                //string header = "Alternative Comparison Report Results: " + Name;
                //DynamicTabVM tab = new DynamicTabVM(header, vm, "AlternativeComparisonReportResults" + Name);
                //Navigate(tab, false, true);
            }
            else
            {
                MessageBox.Show("There are no results to view.", "No Results", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
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

                _Results = alternativeComparisonReport.AlternativeComparisonReport.ComputeDistributionOfAAEQDamageReduced(randomProvider, cc, withoutAltResults, withResults);
               // _Results.WriteToXML();
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

            if(WithProjAltIDs.Count==0)
            {
                vr.AddErrorMessage("There are no longer any with project alternatives.");
            }

            return vr;
        }

        /// <summary>
        /// This is a dummy result object that Cody created to fill the results UI with dummy data.
        /// </summary>
        /// <returns></returns>
        private AlternativeResult CreateAlternativeResult(AlternativeResults alternativeResults)
        {
            AlternativeResult altResult = null;
            StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement(); 

            double discountRate = studyPropElem.DiscountRate;
            int period = studyPropElem.PeriodOfAnalysis;
            //YearResult yr1 = new YearResult(2021, new DamageWithUncertaintyVM(alternativeResults), new DamageByImpactAreaVM(), new DamageByDamCatVM());
            //YearResult yr2 = new YearResult(2022, new DamageWithUncertaintyVM(alternativeResults), new DamageByImpactAreaVM(), new DamageByDamCatVM());

            //EADResult eadResult = new EADResult(new List<YearResult>() { yr1, yr2 });
            //AAEQResult aaeqResult = new AAEQResult(new DamageWithUncertaintyVM(discountRate, period, .6, 7), new DamageByImpactAreaVM(discountRate, period), new DamageByDamCatVM());
            //altResult = new AlternativeResult(eadResult, aaeqResult);

            return altResult;
        }
    }
}
