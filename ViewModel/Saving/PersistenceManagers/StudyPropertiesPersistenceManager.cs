using System;
using System.Collections.Generic;
using ViewModel.Storage;
using ViewModel.Study;
using ViewModel.Utilities;

namespace ViewModel.Saving.PersistenceManagers
{
    public class StudyPropertiesPersistenceManager : SavingBase
    {
        private const int XML_COL = 2;
        public override string TableName => "study_properties";

        public override string[] TableColumnNames => new string[] { NAME, "XML" };

        public override Type[] TableColumnTypes => new Type[] { typeof(string), typeof(string) };

        public StudyPropertiesPersistenceManager(FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        public void Load()
        {
            //there should only ever be one item in this list.
            List<ChildElement> studyProperties = CreateElementsFromRows(TableName, rowData => CreateElementFromRowData(rowData));
            foreach (ChildElement elem in studyProperties)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
        }

        public void SaveNew(ChildElement element)
        {
            SaveNewElement(element);
        }

        //todo: how to rename the whole study?
        private void RenameStudy(string oldName, string newName)
        {
            string oldDirectory = Connection.Instance.ProjectDirectory;
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(oldDirectory);
            string newDirectory = di.Parent.FullName + "\\" + newName;

            if (System.IO.Directory.Exists(oldDirectory))
            {
                //the internet says i need to have it open with some kind of fileshare option
                //System.IO.Directory.Move(oldDirectory, newDirectory);
            }
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            object[] rowData = null;
            if(elem is StudyPropertiesElement propElem)
            {
                rowData = new object[] { propElem.Name, propElem.WriteToXML() };
            }
            return rowData;
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            return new StudyPropertiesElement(rowData[XML_COL].ToString());
        }
    }
}
