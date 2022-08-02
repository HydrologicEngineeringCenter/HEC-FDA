using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class ImpactAreaPersistenceManager : SavingBase
    {
        private static readonly string[] TableColNames = { "xml" };
        private static readonly Type[] TableColTypes = { typeof(string) };

        private const string TABLE_NAME = "impact_area_set";
        private const int XML_COL = 1;

        public override string TableName
        {
            get { return TABLE_NAME; }
        }

        public override string[] TableColumnNames
        {
            get{return TableColNames;}
        }

        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }

        public ImpactAreaPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(ImpactAreaElement element)
        {
            return new object[] { element.ToXML() };
        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            int id = Convert.ToInt32(rowData[ID_COL]);
            string xmlString = (string)rowData[XML_COL];
            return new ImpactAreaElement(xmlString, id);
        }
    

        #endregion      

        public override void Load()
        {
            List<ChildElement> impAreas = CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (ImpactAreaElement elem in impAreas)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return new object[] { ((ImpactAreaElement)elem).ToXML() };
        }
    }
}
