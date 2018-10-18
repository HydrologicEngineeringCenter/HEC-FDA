using FdaViewModel.FrequencyRelationships;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class FlowFrequencyPersistenceManager :SavingBase, IPersistable
    {
        public string TableName  {   get { return "Analyitical Frequency Curves"; } }
        public string[] TableColumnNames()
        {
            return new string[] { "Name", "Last Edit Date", "Description", "Mean (of Log)", "Standard Deviation (of Log)", "Skew (of Log)", "Equivalent Years of Record" };
        }
        private const string _TableConstant = "Analytical Frequency - ";

        public Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(string), typeof(double), typeof(double), typeof(double), typeof(int) };
        }

        public FlowFrequencyPersistenceManager(Study.FDACache studyCache)
        {
            StudyCache = studyCache;
        }


        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(AnalyticalFrequencyElement))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;
                SaveNewElementToParentTable(element, TableName, TableColumnNames(), TableColumnTypes());
                //save the individual table
                SaveFlowFreqCurveTable((AnalyticalFrequencyElement)element, editDate);
                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCache.AddFlowFrequencyElement((AnalyticalFrequencyElement)element);
            }
        }

        public void SaveExisting(Utilities.ChildElement element, string oldName, Statistics.UncertainCurveDataCollection oldCurve)
        {
            SaveExistingElement(oldName, oldCurve, element, TableName);
        }

        public List<ChildElement> Load()
        {
            return CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
        }


        private void SaveFlowFreqCurveTable(AnalyticalFrequencyElement element, string lastEditDate)
        {
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            element.Curve.toSqliteTable(_TableConstant + lastEditDate);
        }

        public  ChildElement CreateElementFromRowData(object[] rowData)
        {
            double mean = (double)rowData[3];
            double stdev = (double)rowData[4];
            double skew = (double)rowData[5];
            Int64 n = (Int64)rowData[6];
            return new AnalyticalFrequencyElement((string)rowData[0], (string)rowData[1], (string)rowData[2], new Statistics.LogPearsonIII(mean, stdev, skew, (int)n));

        }


    }
}
