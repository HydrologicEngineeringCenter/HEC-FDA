
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class FlowFrequencyPersistenceManager : SavingBase
    {
        public static readonly string FLOW_FREQUENCY = "FlowFrequency";
        public static readonly string NAME = "Name";
        public static readonly string DESCRIPTION = "Description";
        public static readonly string LAST_EDIT_DATE = "LastEditDate";
        public static readonly string IS_ANALYTICAL = "IsAnalytical";
        public static readonly string ANALYTICAL_DATA = "AnalyticalData";
        public static readonly string USES_MOMENTS = "UsesMoments";
        public static readonly string POR = "POR";
        public static readonly string MOMENTS = "Moments";
        public static readonly string MEAN = "Mean";
        public static readonly string ST_DEV = "StDev";
        public static readonly string SKEW = "Skew";
        public static readonly string FIT_TO_FLOWS = "FitToFlows";
        public static readonly string FLOWS = "Flows";
        public static readonly string GRAPHICAL = "GRAPHICAL";

        private const int DESC_COL = 2;
        private const int XML_COL = 3;

        /// <summary>
        /// The name of the parent table that will hold all elements of this type
        /// </summary>
        public override string TableName { get { return "analytical_frequency_curves"; } }

        public override string[] TableColumnNames
        {
            get { return new string[] { NAME, DESCRIPTION, "XML" }; }
        }

        public override Type[] TableColumnTypes
        {
            get { return new Type[] { typeof(string), typeof(string), typeof(string) }; }
        }

        public FlowFrequencyPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(AnalyticalFrequencyElement element)
        {
            return new object[] { element.Name, element.Description, WriteFlowFrequencyToXML(element) };
        }

        private string ConvertFlowsToString(List<double> flows)
        {
            if (flows.Count == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            foreach (double d in flows)
            {
                sb.Append(d + ",");
            }
            //remove the last comma
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            int id = Convert.ToInt32(rowData[ID_COL]);
            return new AnalyticalFrequencyElement((string)rowData[NAME_COL], (string)rowData[DESC_COL], (string)rowData[XML_COL], id);
        }

        #endregion

        /// <summary>
        /// Flow frequency doesn not save to its own table. All is contained in the parent row
        /// </summary>
        /// <param name="element"></param>
        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(AnalyticalFrequencyElement))
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
            List<ChildElement> flowFreqs = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (AnalyticalFrequencyElement elem in flowFreqs)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((AnalyticalFrequencyElement)elem);
        }

        public string WriteFlowFrequencyToXML(AnalyticalFrequencyElement elem)
        {
            XElement flowFreqElem = new XElement(FLOW_FREQUENCY);
            flowFreqElem.SetAttributeValue(NAME, elem.Name);
            flowFreqElem.SetAttributeValue(DESCRIPTION, elem.Description);
            flowFreqElem.SetAttributeValue(LAST_EDIT_DATE, elem.LastEditDate);
            flowFreqElem.SetAttributeValue(IS_ANALYTICAL, elem.IsAnalytical);
            XElement analyticalElem = new XElement(ANALYTICAL_DATA);
            flowFreqElem.Add(analyticalElem);
            analyticalElem.SetAttributeValue(USES_MOMENTS, elem.IsStandard);
            analyticalElem.SetAttributeValue(POR, elem.POR);
            //if (elem.IsStandard)
            {
                XElement momentsElem = new XElement(MOMENTS);
                analyticalElem.Add(momentsElem);
                momentsElem.SetAttributeValue(MEAN, elem.Mean);
                momentsElem.SetAttributeValue(ST_DEV, elem.StDev);
                momentsElem.SetAttributeValue(SKEW, elem.Skew);
            }
            //else //fit to flows
            {
                XElement fitToFlowsElem = new XElement(FIT_TO_FLOWS);
                analyticalElem.Add(fitToFlowsElem);
                fitToFlowsElem.SetAttributeValue(FLOWS, ConvertFlowsToString(elem.AnalyticalFlows));
            }
            //do Graphical
            XElement graphicalElem = elem.MyGraphicalVM.ToXML();
            flowFreqElem.Add(graphicalElem);

            return flowFreqElem.ToString();
        }

    }
}
