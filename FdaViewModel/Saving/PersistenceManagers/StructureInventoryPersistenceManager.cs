using FdaViewModel.Inventory;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class StructureInventoryPersistenceManager : SavingBase, IPersistable
    {


        private const string TableName = "Structure Inventories";
        internal override string ChangeTableConstant { get { return "Structure Inventory - "; } }
        private static readonly string[] TableColumnNames = { "Name", "Description" };
        private static readonly Type[] TableColumnTypes = { typeof(string), typeof(string) };



        public StructureInventoryPersistenceManager(Study.FDACache studyCache)
        {
            StudyCache = studyCache;
        }


        #region utilities
        private object[] GetRowDataFromElement(InventoryElement element)
        {
            return new object[] { element.Name, element.Description };

        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            //name, path, description
            if (StructureInventoryLibrary.SharedData.StudyDatabase == null)
            {
                StructureInventoryLibrary.SharedData.StudyDatabase = new DataBase_Reader.SqLiteReader(Storage.Connection.Instance.ProjectFile);
            }
            StructureInventoryBaseElement baseElement = new StructureInventoryBaseElement((string)rowData[0], (string)rowData[1]);

            InventoryElement invEle = new InventoryElement(baseElement);
            return invEle;
        }
        #endregion


        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            StudyCache.RemoveStructureInventoryElement((InventoryElement)element);

        }



        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(InventoryElement))
            {

                SaveNewElementToParentTable(GetRowDataFromElement((InventoryElement)element), TableName, TableColumnNames, TableColumnTypes);
                StudyCache.AddStructureInventoryElement((InventoryElement)element);
            }
        }

       

        public List<ChildElement> Load()
        {
            return CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
        }

        public void SaveExisting(ChildElement oldElement, ChildElement element, int changeTableIndex )
        {
            //does not have an editor
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
    }
}
