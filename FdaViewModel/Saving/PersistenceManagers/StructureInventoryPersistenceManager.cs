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
        private const string _TableConstant = "Structure Inventory - ";
        public  string TableName
        {
            get   {       return "Structure Inventories";   }
        }
        public  string[] TableColumnNames()
        {
            return new string[] { "Name", "Description" };
        }

        public  Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string) };
        }



        public StructureInventoryPersistenceManager(Study.FDACache studyCache)
        {
            StudyCache = studyCache;
        }

        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(InventoryElement))
            {
                SaveNewElementToParentTable(element, TableName, TableColumnNames(), TableColumnTypes());
                StudyCache.AddStructureInventoryElement((InventoryElement)element);
            }
        }

        public void SaveExisting(Utilities.ChildElement element, string oldName, Statistics.UncertainCurveDataCollection oldCurve)
        {
            //does not have an editor
        }

        public List<ChildElement> Load()
        {
            return CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
        }




        public  ChildElement CreateElementFromRowData(object[] rowData)
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


    }
}
