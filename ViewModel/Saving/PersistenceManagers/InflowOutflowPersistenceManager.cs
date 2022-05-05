using HEC.FDA.ViewModel.FlowTransforms;
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

        private const string TABLE_NAME = "inflow_outflow_relationships";
        private static readonly string[] TableColNames = { NAME, LAST_EDIT_DATE, DESCRIPTION, CURVE};
        public static readonly Type[] TableColTypes = { typeof(string), typeof(string), typeof(string), typeof(string) };

        public override string TableName { get { return TABLE_NAME; } }

        public override string[] TableColumnNames
        {
            get { return TableColNames; }
        }
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }
        public InflowOutflowPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(InflowOutflowElement element)
        {
            return new object[] { element.Name, element.LastEditDate, element.Description, element.ComputeComponentVM.ToXML().ToString() };
        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            string curveXML = (string)rowData[CURVE_COL];
            int id = Convert.ToInt32(rowData[ID_COL]);
            ComputeComponentVM computeControlVM = new ComputeComponentVM(XElement.Parse(curveXML));
            InflowOutflowElement inout = new InflowOutflowElement((string)rowData[NAME_COL], 
                (string)rowData[LAST_EDIT_DATE_COL], (string)rowData[DESCRIPTION_COL], computeControlVM, id);
            return inout;
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
