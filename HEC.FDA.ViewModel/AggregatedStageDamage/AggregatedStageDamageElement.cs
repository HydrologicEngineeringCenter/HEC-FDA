using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageElement : ChildElement
    {
        private const string SELECTED_STRUCTURES = "SelectedStructures";
        private const string SELECTED_HYDRO = "SelectedHydraulics";
        private const string IS_MANUAL = "IsManual";
        private const string WRITE_DETAILS = "WriteDetailsFile";
        private const string STAGE_DAMAGE_CURVES = "StageDamageCurves";
        private const string STAGE_DAMAGE_CURVE = "StageDamageCurve";
        private const string IMPACT_AREA_ROWS = "ImpactAreaRows";
        private const string IMPACT_AREA_FREQUENCY_ROWS = "ImpactAreaFrequencyRow";
        private const string COMPUTE_DATE = "ComputeDate";
        private const string SOFTWARE_VERSION = "SoftwareVersion";

        #region Properties
        public int AnalysisYear { get; }
        public int SelectedWSE { get; }
        public int SelectedStructures { get; }
        public List<StageDamageCurve> Curves { get; } = new List<StageDamageCurve>();
        public bool IsManual { get; }
        public List<ImpactAreaFrequencyFunctionRowItem> ImpactAreaFrequencyRows { get; } = new List<ImpactAreaFrequencyFunctionRowItem>();
        public bool WriteDetailsOut { get; }
        public string ComputeDate { get; set; }
        public string SoftwareVersion { get; set; }

        #endregion
        #region Constructors

        public AggregatedStageDamageElement(String name, string lastEditDate, string description, int analysisYear, int selectedWSE, int selectedStructs,
             List<StageDamageCurve> curves, List<ImpactAreaFrequencyFunctionRowItem> impactAreaRows, bool isManual, bool writeDetailsOut, int id,
             string computeDate = null, string softwareVersion = null)
            : base(name, lastEditDate, description, id)
        {
            AnalysisYear = analysisYear;
            ImpactAreaFrequencyRows = impactAreaRows;
            WriteDetailsOut = writeDetailsOut;

            Curves = curves;
            IsManual = isManual;
            SelectedWSE = selectedWSE;
            SelectedStructures = selectedStructs;
            ComputeDate = computeDate;
            SoftwareVersion = softwareVersion;

            AddDefaultActions(EditDamageCurve, StringConstants.EDIT_STAGE_DAMAGE_MENU);
        }

        public AggregatedStageDamageElement(XElement elementXML, int id):base(elementXML, id)
        {
            //This is for backwards compatability. The analysis year is new.
            if(elementXML.Attribute("AnalysisYear") != null)
            {
                AnalysisYear = Convert.ToInt32(elementXML.Attribute("AnalysisYear").Value);
            }
            else
            {
                AnalysisYear = DateTime.Now.Year;
            }

            SelectedStructures = Convert.ToInt32( elementXML.Attribute(SELECTED_STRUCTURES).Value);
            SelectedWSE = Convert.ToInt16(elementXML.Attribute(SELECTED_HYDRO).Value);
            IsManual = Convert.ToBoolean(elementXML.Attribute(IS_MANUAL).Value);
            bool writeToFileElementExists = elementXML.Attribute(WRITE_DETAILS) != null;
            if(writeToFileElementExists)
            {
                WriteDetailsOut = Convert.ToBoolean(elementXML.Attribute(WRITE_DETAILS).Value);
            }
            else
            {
                WriteDetailsOut = false;
            }

            XElement stageDamageCurves = elementXML.Element(STAGE_DAMAGE_CURVES);
            IEnumerable<XElement> curves = stageDamageCurves.Elements(STAGE_DAMAGE_CURVE);
            foreach (XElement curve in curves)
            {
                Curves.Add(new StageDamageCurve(curve));
            }

            XElement impAreaRows = elementXML.Element(IMPACT_AREA_ROWS);
            IEnumerable<XElement> freqRows = impAreaRows.Elements(IMPACT_AREA_FREQUENCY_ROWS);
            foreach (XElement impAreaRow in freqRows)
            {
                ImpactAreaFrequencyRows.Add(new ImpactAreaFrequencyFunctionRowItem(impAreaRow));
            }

            if (elementXML.Attribute(COMPUTE_DATE) != null)
            {
                ComputeDate = elementXML.Attribute(COMPUTE_DATE).Value;
            }
            if (elementXML.Attribute(SOFTWARE_VERSION) != null)
            {
                SoftwareVersion = elementXML.Attribute(SOFTWARE_VERSION).Value;
            }

            AddDefaultActions(EditDamageCurve, StringConstants.EDIT_STAGE_DAMAGE_MENU);
            UpdateTooltip();
        }
        #endregion
        #region Voids

        public void EditDamageCurve(object arg1, EventArgs arg2)
        {    
            //create action manager
            EditorActionManager actionManager = new EditorActionManager()
                 .WithSiblingRules(this);

            AggregatedStageDamageEditorVM vm = new AggregatedStageDamageEditorVM(this, actionManager);
            vm.CalculatedVM.RequestNavigation += Navigate;

            string title = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(title, vm, "EditStageDamageElement" + Name);
            Navigate(tab, false, true);
        }

        public void UpdateTooltip()
        {
            StringBuilder sb = new();
            sb.AppendLine("Last Edited: " + LastEditDate);
            if (ComputeDate != null)
            {
                sb.AppendLine("Last Computed: " + ComputeDate);
                sb.AppendLine("Computed with: HEC-FDA " + (SoftwareVersion ?? "NA"));
            }
            CustomTreeViewHeader.Tooltip = sb.ToString().Trim();
        }

        public override XElement ToXML()
        {
            XElement stageDamageElem = new XElement(StringConstants.ELEMENT_XML_TAG);
            stageDamageElem.Add(CreateHeaderElement());

            stageDamageElem.SetAttributeValue("AnalysisYear", AnalysisYear);
            stageDamageElem.SetAttributeValue(SELECTED_STRUCTURES, SelectedStructures);
            stageDamageElem.SetAttributeValue(SELECTED_HYDRO, SelectedWSE);
            stageDamageElem.SetAttributeValue(IS_MANUAL, IsManual);
            stageDamageElem.SetAttributeValue(WRITE_DETAILS, WriteDetailsOut);
            if (ComputeDate != null)
            {
                stageDamageElem.SetAttributeValue(COMPUTE_DATE, ComputeDate);
            }
            if (SoftwareVersion != null)
            {
                stageDamageElem.SetAttributeValue(SOFTWARE_VERSION, SoftwareVersion);
            }

            XElement curveElements = new XElement(STAGE_DAMAGE_CURVES);
            foreach (StageDamageCurve curve in Curves)
            {
                curveElements.Add(curve.WriteToXML());
            }
            stageDamageElem.Add(curveElements);

            XElement impactAreaRowsElem = new XElement(IMPACT_AREA_ROWS);
            foreach (ImpactAreaFrequencyFunctionRowItem row in ImpactAreaFrequencyRows)
            {
                impactAreaRowsElem.Add(row.WriteToXML());
            }
            stageDamageElem.Add(impactAreaRowsElem);

            return stageDamageElem;
        }

        public bool Equals(AggregatedStageDamageElement elem)
        {
            bool isEqual = true;

            if (!AreHeaderDataEqual(elem))
            {
                isEqual = false;
            }
            if (SelectedStructures != elem.SelectedStructures)
            {
                isEqual = false;
            }
            if (IsManual != elem.IsManual)
            {
                isEqual = false;
            }

            for(int i = 0;i< Curves.Count; i++)
            {
                if (Curves[i].Equals(elem.Curves[i]))
                {
                    isEqual = false;
                    break;
                }
            }

            for(int i = 0;i< ImpactAreaFrequencyRows.Count;i++)
            {
                //todo: I don't want to write this equals method right now
                //if(!ImpactAreaFrequencyRows[i].Equals(elem.ImpactAreaFrequencyRows[i]))
                //{
                //    isEqual = false;
                //    break;
                //}
            }

            return isEqual;
        }
        #endregion

    }
}
