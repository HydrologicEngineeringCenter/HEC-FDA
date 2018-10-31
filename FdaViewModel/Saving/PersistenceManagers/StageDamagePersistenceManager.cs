using FdaViewModel.AggregatedStageDamage;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class StageDamagePersistenceManager : SavingBase, IPersistableWithUndoRedo
    {


        private const string TableName = "Aggregated Stage Damage Relationships";
        internal override string ChangeTableConstant { get { return "Aggregated Stage Damage Function - "; } }
        private static readonly string[] TableColumnNames = { "Name", "Last Edit Date", "Description", "Curve Uncertainty Type", "Creation Method", "Curve" };
        private static readonly Type[] TableColumnTypes = { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };


        public StageDamagePersistenceManager(Study.FDACache studyCache)
        {
            StudyCache = studyCache;
        }



        #region utilities
        private object[] GetRowDataFromElement(AggregatedStageDamageElement element)
        {
            return new object[] { element.Name, element.LastEditDate, element.Description, element.Curve.Distribution, element.Method, ExtentionMethods.CreateXMLCurveString(element.Curve.Distribution, element.Curve.XValues, element.Curve.YValues) };

        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            Statistics.UncertainCurveDataCollection emptyCurve = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            AggregatedStageDamageElement asd = new AggregatedStageDamageElement((string)rowData[0], (string)rowData[1], (string)rowData[2], emptyCurve, (CreationMethodEnum)Enum.Parse(typeof(CreationMethodEnum), (string)rowData[4]));
            //asd.Curve.fromSqliteTable(ChangeTableConstant + (string)rowData[1]);
            asd.Curve = ExtentionMethods.GetCurveFromXMLString((string)rowData[5], (Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            return asd;
        }
        #endregion


        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(AggregatedStageDamageElement))
            {
                if(element.Description == null) { element.Description = ""; }
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;

                SaveNewElementToParentTable(GetRowDataFromElement((AggregatedStageDamageElement)element), TableName, TableColumnNames, TableColumnTypes);
                SaveElementToChangeTable(element.Name, GetRowDataFromElement((AggregatedStageDamageElement)element), ChangeTableConstant, TableColumnNames, TableColumnTypes);
                //SaveCurveTable(element.Curve, ChangeTableConstant, editDate);
                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCache.AddStageDamageElement((AggregatedStageDamageElement)element);
            }
        }
        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            StudyCache.RemoveStageDamageElement((AggregatedStageDamageElement)element);
        }
        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex)
        {
            if (elementToSave.Description == null) { elementToSave.Description = ""; }

            string editDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = editDate;

            if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((AggregatedStageDamageElement)elementToSave), oldElement.Name, TableName) || AreCurvesDifferent(oldElement.Curve, elementToSave.Curve))
            {
                UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((AggregatedStageDamageElement)elementToSave), oldElement.Name, TableName, true, ChangeTableConstant);
                //SaveCurveTable(elementToSave.Curve, ChangeTableConstant, editDate);
                // update the existing element. This will actually remove the old element and do an insert at that location with the new element.
                StudyCache.UpdateStageDamageElement((AggregatedStageDamageElement)oldElement, (AggregatedStageDamageElement)elementToSave);
            }
        }

        public List<ChildElement> Load()
        {
            return CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
    }
}
