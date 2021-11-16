using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ViewModel.AlternativeComparisonReport.Results;
using ViewModel.Alternatives.Results;
using ViewModel.Alternatives.Results.ResultObject;
using ViewModel.Utilities;

namespace ViewModel.AlternativeComparisonReport
{
    public class AlternativeComparisonReportElement: ChildElement
    {
        private const string ALTERNATIVE_COMP_REPORT = "AlternativeComparisonReport";
        private const string NAME = "Name";
        private const string DESCRIPTION = "Description";
        private const string ID = "ID";
        private const string WITHOUT_PROJ_ID = "WithoutProjID";
        private const string WITH_PROJ_ELEM = "WithProjectElement";

        public int WithoutProjAltID { get; }
        public List<int> WithProjAltIDs { get; } = new List<int>();

        public AlternativeComparisonReportElement(string name, string desc,int withoutProjectAltId, List<int> withProjAlternativeIds)
        {
            Name = name;
            Description = desc;
            WithoutProjAltID = withoutProjectAltId;
            WithProjAltIDs = withProjAlternativeIds;
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/Condition.png");
            AddActions();
        }

        public AlternativeComparisonReportElement(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            XElement altElement = doc.Element(ALTERNATIVE_COMP_REPORT);
            Name = altElement.Attribute(NAME).Value;
            Description = altElement.Attribute(DESCRIPTION).Value;
            WithoutProjAltID = Int32.Parse(altElement.Attribute(WITHOUT_PROJ_ID).Value);

            IEnumerable<XElement> altElements = altElement.Elements(WITH_PROJ_ELEM);
            foreach (XElement elem in altElements)
            {
                int iasID = Int32.Parse(elem.Attribute(ID).Value);
                WithProjAltIDs.Add(iasID);
            }
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/Condition.png");
            AddActions();
        }

        private void AddActions()
        {
            NamedAction edit = new NamedAction();
            edit.Header = "Edit Alternative Comparison Report...";
            edit.Action = EditAlternative;

            NamedAction compute = new NamedAction();
            compute.Header = "Calculate Average Annual Equivalent Damage";
            compute.Action = ComputeAlternative;

            NamedAction viewResults = new NamedAction();
            viewResults.Header = "View Results...";
            viewResults.Action = ViewResults;

            NamedAction removeCondition = new NamedAction();
            removeCondition.Header = "Remove";
            removeCondition.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = "Rename...";
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
                elemToReturn = new AlternativeComparisonReportElement(elem.Name, elem.Description, elem.WithoutProjAltID, elem.WithProjAltIDs);
            }
            return elemToReturn;
        }

        public string WriteToXML()
        {
            XElement altElement = new XElement(ALTERNATIVE_COMP_REPORT);
            altElement.SetAttributeValue(NAME, Name);
            altElement.SetAttributeValue(DESCRIPTION, Description);
            altElement.SetAttributeValue(WITHOUT_PROJ_ID, WithoutProjAltID);
            foreach (int elemID in WithProjAltIDs)
            {
                XElement setElement = new XElement(WITH_PROJ_ELEM);
                setElement.SetAttributeValue(ID, elemID);
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
            //FdaValidationResult vr = RunPreComputeValidation();
            //if (!vr.IsValid)
            //{
            //    MessageBox.Show(vr.ErrorMessage, "Invalid Setup", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //}

            ////todo: Run calculations. waiting for hec to put the new model in.
            //IASElementSet[] elems = GetElementsFromID();
            ////grab the result objects off the ias elements and run the calculation.

        }
        private AlternativeResult CreateAlternativeResult()
        {
            YearResult yr1 = new YearResult(2021, new DamageWithUncertaintyVM(), new DamageByImpactAreaVM(), new DamageByDamCatVM());
            YearResult yr2 = new YearResult(2022, new DamageWithUncertaintyVM(), new DamageByImpactAreaVM(), new DamageByDamCatVM());

            EADResult eadResult = new EADResult(new List<YearResult>() { yr1, yr2 });
            AAEQResult aaeqResult = new AAEQResult(new DamageWithUncertaintyVM(), new DamageByImpactAreaVM(), new DamageByDamCatVM());
            AlternativeResult altResult = new AlternativeResult(eadResult, aaeqResult);

            return altResult;
        }

        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetAlternativeCompReportManager().Remove(this);
        }

    }
}
