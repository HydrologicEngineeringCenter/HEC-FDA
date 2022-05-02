using HEC.FDA.ViewModel.AlternativeComparisonReport.Results;
using HEC.FDA.ViewModel.Alternatives.Results;
using HEC.FDA.ViewModel.Alternatives.Results.ResultObject;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport
{
    public class AlternativeComparisonReportElement: ChildElement
    {
        private const string ALTERNATIVE_COMP_REPORT = "AlternativeComparisonReport";
        private const string NAME = "Name";
        private const string DESCRIPTION = "Description";
        private const string ID_STRING = "ID";
        private const string WITHOUT_PROJ_ID = "WithoutProjID";
        private const string WITH_PROJ_ELEM = "WithProjectElement";
        private const string LAST_EDIT_DATE = "LastEditDate";


        public int WithoutProjAltID { get; }
        public List<int> WithProjAltIDs { get; } = new List<int>();

        public AlternativeComparisonReportElement(string name, string desc, string creationDate, int withoutProjectAltId, List<int> withProjAlternativeIds, int id):base(id)
        {
            Name = name;
            Description = desc;
            WithoutProjAltID = withoutProjectAltId;
            WithProjAltIDs = withProjAlternativeIds;
            CustomTreeViewHeader = new CustomHeaderVM(Name)
            {
                ImageSource = "pack://application:,,,/View;component/Resources/AlternativeComparisonReport_20x20.png",
                Tooltip = StringConstants.CreateChildNodeTooltip(creationDate)
            };
            AddActions();
        }

        /// <summary>
        /// The ctor used to load an element from the database.
        /// </summary>
        /// <param name="xml"></param>
        public AlternativeComparisonReportElement(string xml, int id):base(id)
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
                ImageSource = "pack://application:,,,/View;component/Resources/AlternativeComparisonReport_20x20.png",
                Tooltip = StringConstants.CreateChildNodeTooltip(LastEditDate)
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
            AlternativeComparisonReportResultsVM vm = new AlternativeComparisonReportResultsVM(CreateAlternativeResult());
            string header = "Alternative Comparison Report Results: " + Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "AlternativeComparisonReportResults" + Name);
            Navigate(tab, false, true);
        }

        public void ComputeAlternative(object arg1, EventArgs arg2)
        {
            //waiting for HEC to get the new model plugged in. 11/16/21
        }

        /// <summary>
        /// This is a dummy result object that Cody created to fill the results UI with dummy data.
        /// </summary>
        /// <returns></returns>
        private AlternativeResult CreateAlternativeResult()
        {
            AlternativeResult altResult = null;
            StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement(); 

            double discountRate = studyPropElem.DiscountRate;
            int period = studyPropElem.PeriodOfAnalysis;
            YearResult yr1 = new YearResult(2021, new DamageWithUncertaintyVM(.123, 1), new DamageByImpactAreaVM(), new DamageByDamCatVM());
            YearResult yr2 = new YearResult(2022, new DamageWithUncertaintyVM(.456, 4), new DamageByImpactAreaVM(), new DamageByDamCatVM());

            EADResult eadResult = new EADResult(new List<YearResult>() { yr1, yr2 });
            AAEQResult aaeqResult = new AAEQResult(new DamageWithUncertaintyVM(discountRate, period, .6, 7), new DamageByImpactAreaVM(discountRate, period), new DamageByDamCatVM());
            altResult = new AlternativeResult(eadResult, aaeqResult);

            return altResult;
        }
    }
}
