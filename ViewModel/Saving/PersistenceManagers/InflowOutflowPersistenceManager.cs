using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class InflowOutflowPersistenceManager : SavingBase
    {
        public override string TableName { get { return "inflow_outflow_relationships"; } }

        public InflowOutflowPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            int id = Convert.ToInt32(rowData[ID_COL]);
            string xmlString = (string)rowData[XML_COL];
            XDocument doc = XDocument.Parse(xmlString);
            XElement itemElem = doc.Element(CurveChildElement.CHILD_ELEMENT);
            return new InflowOutflowElement(itemElem, id);
        }

    }
}
