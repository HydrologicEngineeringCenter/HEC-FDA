using FdaViewModel.FrequencyRelationships;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class FlowFrequencyPersistenceManager :SavingBase, IPersistableWithUndoRedo
    {

        private const string TableName = "Analyitical Frequency Curves";
        internal override string ChangeTableConstant { get { return "Analytical Frequency - "; } }
        private static readonly string[] TableColumnNames = { "Name", "Last Edit Date", "Description", "Mean (of Log)", "Standard Deviation (of Log)", "Skew (of Log)", "Equivalent Years of Record" };
        private static readonly Type[] TableColumnTypes = { typeof(string), typeof(string), typeof(string), typeof(double), typeof(double), typeof(double), typeof(int) };



        public FlowFrequencyPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(AnalyticalFrequencyElement element)
        {
            return new object[] { element.Name, element.LastEditDate, element.Description, element.Distribution.GetMean, element.Distribution.GetStDev, element.Distribution.GetG, element.Distribution.GetSampleSize };
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            double mean = (double)rowData[3];
            double stdev = (double)rowData[4];
            double skew = (double)rowData[5];
            Int64 n = (Int64)rowData[6];
            return new AnalyticalFrequencyElement((string)rowData[0], (string)rowData[1], (string)rowData[2], new Statistics.LogPearsonIII(mean, stdev, skew, (int)n));

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
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;

                SaveNewElementToParentTable(GetRowDataFromElement((AnalyticalFrequencyElement)element), TableName, TableColumnNames, TableColumnTypes);
                SaveElementToChangeTable(element.Name, GetRowDataFromElement((AnalyticalFrequencyElement)element), ChangeTableConstant, TableColumnNames, TableColumnTypes);
                //SaveCurveTable(element.Curve, ChangeTableConstant, editDate);
                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCacheForSaving.AddElement((AnalyticalFrequencyElement)element);
            }
        }

        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            StudyCacheForSaving.RemoveElement((AnalyticalFrequencyElement)element);

        }

        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex )
        {
            string editDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = editDate;

            if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((AnalyticalFrequencyElement)elementToSave), oldElement.Name, TableName) )
            {
                UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((AnalyticalFrequencyElement)elementToSave), oldElement.Name, TableName, true, ChangeTableConstant);
                // SaveCurveTable(elementToSave.Curve, ChangeTableConstant, editDate);
                StudyCacheForSaving.UpdateFlowFrequencyElement((AnalyticalFrequencyElement)oldElement, (AnalyticalFrequencyElement)elementToSave);
            }
        }

        public void Load()
        {
            List<ChildElement> flowFreqs = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (AnalyticalFrequencyElement elem in flowFreqs)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public override void AddValidationRules()
        {
           // throw new NotImplementedException();
        }
    }
}
