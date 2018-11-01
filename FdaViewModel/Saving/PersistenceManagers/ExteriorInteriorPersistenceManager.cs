using FdaViewModel.StageTransforms;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class ExteriorInteriorPersistenceManager : SavingBase, IPersistableWithUndoRedo
    {

        private const string TableName = "Interior Exterior Curves";
        internal override string ChangeTableConstant { get { return "Exterior Interior - "; } }
        private static readonly string[] TableColumnNames = { "Name", "Last Edit Date", "Description", "Curve Distribution Type", "Curve" };
        private static readonly Type[] TableColumnTypes = { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };



        public ExteriorInteriorPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }


        #region utilities
        private object[] GetRowDataFromElement(ExteriorInteriorElement element)
        {
            return new object[] { element.Name, element.LastEditDate, element.Description, element.Curve.Distribution, ExtentionMethods.CreateXMLCurveString(element.Curve.Distribution, element.Curve.XValues, element.Curve.YValues) };
        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            Statistics.UncertainCurveDataCollection ucdc = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            ExteriorInteriorElement ele = new ExteriorInteriorElement((string)rowData[0], (string)rowData[1], (string)rowData[2], ucdc);
            //ele.Curve.fromSqliteTable(ChangeTableConstant + (string)rowData[1]);
            ele.Curve = ExtentionMethods.GetCurveFromXMLString((string)rowData[4], (Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            return ele;
        }
        #endregion


        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(ExteriorInteriorElement))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;               

                SaveNewElementToParentTable(GetRowDataFromElement((ExteriorInteriorElement)element), TableName, TableColumnNames, TableColumnTypes);
                SaveElementToChangeTable(element.Name, GetRowDataFromElement((ExteriorInteriorElement)element), ChangeTableConstant, TableColumnNames, TableColumnTypes);
                //SaveCurveTable(element.Curve, ChangeTableConstant, editDate);

                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCacheForSaving.AddExteriorInteriorElement((ExteriorInteriorElement)element);
            }
        }

        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            StudyCacheForSaving.RemoveExteriorInteriorElement((ExteriorInteriorElement)element);

        }

       
        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex )
        {
            string editDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = editDate;

            if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((ExteriorInteriorElement)elementToSave), oldElement.Name, TableName) || AreCurvesDifferent(oldElement.Curve, elementToSave.Curve))
            {
                UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((ExteriorInteriorElement)elementToSave), oldElement.Name, TableName, true, ChangeTableConstant);
                //SaveCurveTable(elementToSave.Curve, ChangeTableConstant, editDate);
                // update the existing element. This will actually remove the old element and do an insert at that location with the new element.
                StudyCacheForSaving.UpdateExteriorInteriorElement((ExteriorInteriorElement)oldElement, (ExteriorInteriorElement)elementToSave);
            }
        }

        public List<ChildElement> Load()
        {
            return CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
             
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
    }
}
