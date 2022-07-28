using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class StageDamagePersistenceManager : SavingBase
    {
        private const int NAME_COL = 1;
        private const int LAST_EDIT_DATE_COL = 2;
        private const int DESC_COL = 3;
        private const int IS_MANUAL_COL = 4;
        private const int SELECTED_WSE_COL = 5;
        private const int SELECTED_STRUCTURE_COL = 6;
        private const int CURVES_COL = 7;
        private const int IMP_AREA_FREQ_ROWS_COL = 8;

        private const String STAGE_DAMAGE_CURVES_TAG = "StageDamageCurves";
        private const String IMPACT_AREA_FREQ_ROWS_TAG = "ImpactAreaFrequencyRows";

        private const string TABLE_NAME = "stage_damage_relationships";

        private static readonly string[] TableColNames = { NAME, LAST_EDIT_DATE, DESCRIPTION, "is_manual", "selected_wse", "selected_structures", "curves", "impact_area_frequency_rows" };
    
        private static readonly Type[] TableColTypes = { typeof(string), typeof(string), typeof(string), typeof(bool), typeof(int), typeof(int), typeof(string), typeof(string) };
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }

        public override string TableName { get { return TABLE_NAME; } }

        public override string[] TableColumnNames
        {
            get{ return TableColNames;}
        }

        public StageDamagePersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(AggregatedStageDamageElement element)
        {
            return new object[] { element.Name, element.LastEditDate, element.Description,
               element.IsManual, element.SelectedWSE, element.SelectedStructures,  WriteCurvesToXML(element.Curves), WriteImpactAreaFrequencyRows(element.ImpactAreaFrequencyRows)};
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            bool isManual = Convert.ToBoolean( rowData[IS_MANUAL_COL]);
            int selectedWSE = Convert.ToInt32(rowData[SELECTED_WSE_COL]);
            int selectedStructs = Convert.ToInt32( rowData[SELECTED_STRUCTURE_COL]);
            string curvesXmlString = (string)rowData[CURVES_COL];
            List<StageDamageCurve> stageDamageCurves = LoadCurvesFromXML(curvesXmlString);

            int id = Convert.ToInt32(rowData[ID_COL]);

            List<ImpactAreaFrequencyFunctionRowItem> impactAreaFrequencyRows = new List<ImpactAreaFrequencyFunctionRowItem>();
         
            if (rowData.Length > IMP_AREA_FREQ_ROWS_COL)
            {
                //this is for backwards compatibility
                impactAreaFrequencyRows = LoadImpactAreaFreqRows((string)rowData[IMP_AREA_FREQ_ROWS_COL]);
            }

            AggregatedStageDamageElement asd = new AggregatedStageDamageElement((string)rowData[NAME_COL], (string)rowData[LAST_EDIT_DATE_COL],
            (string)rowData[DESC_COL], selectedWSE, selectedStructs,stageDamageCurves, impactAreaFrequencyRows, isManual, id);
            return asd;
        }
        #endregion   

        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(AggregatedStageDamageElement))
            {
                //save to parent table
                base.SaveNew(element);
            }
        }
        public void Remove(ChildElement element)
        {
            base.Remove(element);
        }

        public override void Load()
        {
            List<ChildElement> stageDamages = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (AggregatedStageDamageElement elem in stageDamages)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((AggregatedStageDamageElement)elem);
        }
      
        private XElement WriteCurvesToXML(List<StageDamageCurve> curves)
        {
            XElement curvesElement = new XElement(STAGE_DAMAGE_CURVES_TAG);
            foreach(StageDamageCurve curve in curves)
            {
                curvesElement.Add(curve.WriteToXML());
            }

            return curvesElement;
        }

        private XElement WriteImpactAreaFrequencyRows(List<ImpactAreaFrequencyFunctionRowItem> impactAreaFrequencyRows)
        {
            XElement impAreaFreqRowsElement = new XElement(IMPACT_AREA_FREQ_ROWS_TAG);
            if (impactAreaFrequencyRows != null)
            {
                foreach (ImpactAreaFrequencyFunctionRowItem row in impactAreaFrequencyRows)
                {
                    impAreaFreqRowsElement.Add(row.WriteToXML());
                }
            }
            return impAreaFreqRowsElement;
        }

        private List<StageDamageCurve> LoadCurvesFromXML(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            XElement curvesElement = doc.Element(STAGE_DAMAGE_CURVES_TAG);
            IEnumerable<XElement> curveElems = curvesElement.Elements(StageDamageCurve.STAGE_DAMAGE_CURVE_TAG);
            List<StageDamageCurve> curves = new List<StageDamageCurve>();
            foreach(XElement elem in curveElems)
            {
                curves.Add(new StageDamageCurve(elem));
            }

            return curves;
        }

        private List<ImpactAreaFrequencyFunctionRowItem> LoadImpactAreaFreqRows(string xml)
        {
            List<ImpactAreaFrequencyFunctionRowItem> impactAreaFrequencyRows = new List<ImpactAreaFrequencyFunctionRowItem>();

            XDocument doc = XDocument.Parse(xml);
            XElement rowsElem = doc.Element(IMPACT_AREA_FREQ_ROWS_TAG);
            IEnumerable<XElement> rowElems = rowsElem.Elements(ImpactAreaFrequencyFunctionRowItem.IMPACT_AREA_FREQUENCY_ROW);
            foreach (XElement elem in rowElems)
            {
                impactAreaFrequencyRows.Add(new ImpactAreaFrequencyFunctionRowItem(elem));
            }

            return impactAreaFrequencyRows;
        }
    }
}
