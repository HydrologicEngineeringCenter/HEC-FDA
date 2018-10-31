using FdaViewModel.FlowTransforms;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class InflowOutflowPersistenceManager : SavingBase, IPersistableWithUndoRedo
    {


        private const string TableName = "Inflow Outflow Relationships";
        internal override string ChangeTableConstant { get { return "Inflow Outflow - "; } }
        private static readonly string[] TableColumnNames = { "Name", "Last Edit Date", "Description", "Curve Distribution Type" , "Curve"};
        public static readonly Type[] TableColumnTypes = { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };


        public InflowOutflowPersistenceManager(Study.FDACache studyCache)
        {
            StudyCache = studyCache;
        }


        #region utilities
        private object[] GetRowDataFromElement(InflowOutflowElement element)
        {
            return new object[] { element.Name, element.LastEditDate, element.Description, element.Curve.Distribution, ExtentionMethods.CreateXMLCurveString(element.Curve.Distribution, element.Curve.XValues, element.Curve.YValues) };

        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            Statistics.UncertainCurveDataCollection ucdc = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            InflowOutflowElement inout = new InflowOutflowElement((string)rowData[0], (string)rowData[1], (string)rowData[2], ucdc);
            //inout.Curve.fromSqliteTable(ChangeTableConstant + (string)rowData[1]);
            inout.Curve = ExtentionMethods.GetCurveFromXMLString((string)rowData[4], (Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            return inout;
        }

        #endregion


        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(InflowOutflowElement))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;

                SaveNewElementToParentTable(GetRowDataFromElement((InflowOutflowElement)element), TableName, TableColumnNames, TableColumnTypes);
                SaveElementToChangeTable(element.Name, GetRowDataFromElement((InflowOutflowElement)element), ChangeTableConstant, TableColumnNames, TableColumnTypes);
                //SaveCurveTable(element.Curve, ChangeTableConstant, editDate);                //save the individual table
                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCache.AddInflowOutflowElement((InflowOutflowElement)element);
            }
        }

        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            StudyCache.RemoveInflowOutflowElement((InflowOutflowElement)element);
        }

        public void SaveExisting(ChildElement oldElement, Utilities.ChildElement elementToSave, int changeTableIndex  )
        {
            string editDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = editDate;

            if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((InflowOutflowElement)elementToSave), oldElement.Name, TableName) || AreCurvesDifferent(oldElement.Curve, elementToSave.Curve))
            {
                UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((InflowOutflowElement)elementToSave), oldElement.Name, TableName, true, ChangeTableConstant);
                //
                SaveCurveTable(elementToSave.Curve, ChangeTableConstant, editDate);
                // update the existing element. This will actually remove the old element and do an insert at that location with the new element.
                StudyCache.UpdateInflowOutflowElement((InflowOutflowElement)oldElement, (InflowOutflowElement)elementToSave);
            }
        }

        public List<ChildElement> Load()
        {
            return CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
        }

      
        private void SaveFlowFreqCurveTable(InflowOutflowElement element, string lastEditDate)
        {
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            element.Curve.toSqliteTable(ChangeTableConstant + lastEditDate);
        }

    

        public override void AddValidationRules()
        {
           // throw new NotImplementedException();
        }
    }
}
