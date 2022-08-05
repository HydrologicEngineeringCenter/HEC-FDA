using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class InflowOutflowPersistenceManager : SavingBase
    {
        private const int LAST_EDIT_DATE_COL = 2;
        private const int DESCRIPTION_COL = 3;
        private const int CURVE_COL = 4;

        public override string TableName { get { return "inflow_outflow_relationships"; } }

        public InflowOutflowPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            int id = Convert.ToInt32(rowData[ID_COL]);
            string xmlString = (string)rowData[XML_COL];
            XDocument doc = XDocument.Parse(xmlString);
            XElement itemElem = doc.Element(IndexPointsElement.INDEX_POINTS_TAG);
            return new InflowOutflowElement(itemElem, id);

        }

        #endregion

        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(InflowOutflowElement))
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
            List<ChildElement> inflowOutflows = CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (InflowOutflowElement elem in inflowOutflows)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((InflowOutflowElement)elem);
        }
    }
}
