using FdaViewModel.GeoTech;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class LeveePersistenceManager : SavingBase, IPersistable
    {

        private const string TableName = "Levee Features";
        private static readonly string[] TableColumnNames = { "Levee Feature", "Description", "Elevation" };
        private static readonly Type[] TableColumnTypes = { typeof(string), typeof(string), typeof(double) };

        internal override string ChangeTableConstant
        {
           get { return ""; }
        }

        public LeveePersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(LeveeFeatureElement element)
        {
            return new object[] { element.Name, element.Description, element.Elevation };

        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            return new LeveeFeatureElement((string)rowData[0], (string)rowData[1], (double)rowData[2]);
        }
        #endregion

        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(LeveeFeatureElement))
            {
                SaveNewElementToParentTable(GetRowDataFromElement((LeveeFeatureElement)element), TableName, TableColumnNames, TableColumnTypes);
                StudyCacheForSaving.AddLeveeElement((LeveeFeatureElement)element);
            }
        }
        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            StudyCacheForSaving.RemoveLeveeElement((LeveeFeatureElement)element);

        }
        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex  )
        {
            if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((LeveeFeatureElement)elementToSave), oldElement.Name, TableName))
            {
                UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((LeveeFeatureElement)elementToSave), oldElement.Name, TableName, false, ChangeTableConstant);
                StudyCacheForSaving.UpdateLeveeElement((LeveeFeatureElement)oldElement, (LeveeFeatureElement)elementToSave);
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
