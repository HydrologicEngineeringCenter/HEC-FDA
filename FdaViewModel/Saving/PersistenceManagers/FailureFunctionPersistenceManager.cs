using FdaViewModel.GeoTech;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class FailureFunctionPersistenceManager : SavingBase, IPersistableWithUndoRedo
    {

        private const string TableName = "Failure Functions";
        internal override string ChangeTableConstant { get { return "Failure Function - "; } }
        private static readonly string[] TableColumnNames = { "Name", "Last Edit Date", "Description", "Associated Levee Feature", "Curve Distribution Type", "Curve" };
        private static readonly Type[] TableColumnTypes = { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };


        public FailureFunctionPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(FailureFunctionElement element)
        {
            return new object[] { element.Name, element.LastEditDate, element.Description, element.SelectedLateralStructure.Name, element.Curve.Distribution, ExtentionMethods.CreateXMLCurveString(element.Curve.Distribution, element.Curve.XValues, element.Curve.YValues) };

        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            List<LeveeFeatureElement> ele = StudyCacheForSaving.LeveeElements;
            LeveeFeatureElement lfe = null;
            foreach (LeveeFeatureElement element in ele)
            {
                if (element.Name == (string)rowData[3])
                {
                    lfe = element;
                }
            }
            Statistics.UncertainCurveDataCollection ucdc = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[4]));
            FailureFunctionElement failure = new FailureFunctionElement((string)rowData[0], (string)rowData[1], (string)rowData[2], ucdc, lfe);
            //failure.Curve.fromSqliteTable(ChangeTableConstant + (string)rowData[1]);
            failure.Curve = ExtentionMethods.GetCurveFromXMLString((string)rowData[5], (Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[4]));
            return failure;
        }
        #endregion



        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(FailureFunctionElement))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;

                SaveNewElementToParentTable(GetRowDataFromElement((FailureFunctionElement)element), TableName, TableColumnNames, TableColumnTypes);
                SaveElementToChangeTable(element.Name, GetRowDataFromElement((FailureFunctionElement)element), ChangeTableConstant, TableColumnNames, TableColumnTypes);
                //SaveCurveTable(element.Curve, ChangeTableConstant, editDate);
                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCacheForSaving.AddFailureFunctionElement((FailureFunctionElement)element);
            }
        }
        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            StudyCacheForSaving.RemoveFailureFunctionElement((FailureFunctionElement)element);

        }
        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex  )
        {
            string editDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = editDate;

            if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((FailureFunctionElement)elementToSave), oldElement.Name, TableName) || AreCurvesDifferent(oldElement.Curve, elementToSave.Curve))
            {
                UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((FailureFunctionElement)elementToSave), oldElement.Name, TableName, true, ChangeTableConstant);
                // SaveCurveTable(elementToSave.Curve, ChangeTableConstant, editDate);
                // update the existing element. This will actually remove the old element and do an insert at that location with the new element.
                StudyCacheForSaving.UpdateFailureFunctionElement((FailureFunctionElement)oldElement, (FailureFunctionElement)elementToSave);
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
