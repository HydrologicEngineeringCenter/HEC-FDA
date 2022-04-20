using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class AlternativePersistenceManager : SavingBase
    {
        public override string TableName => "alternatives";

        public override string[] TableColumnNames => new string[]{NAME, "xml"};

        public override Type[] TableColumnTypes => new Type[] { typeof(string), typeof(string) };

        public AlternativePersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            string xml = (string)rowData[2];
            int id = Convert.ToInt32(rowData[ID_COL]);
            AlternativeElement elem = new AlternativeElement(xml, id);
            return elem;
        }        

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            object[] retval = new object[] { elem.Name, ((AlternativeElement)elem).WriteToXML() };
            return retval;
        }

        public override void Load()
        {
            List<ChildElement> iasElems = CreateElementsFromRows(TableName, (rowData) => CreateElementFromRowData(rowData));
            foreach (AlternativeElement elem in iasElems)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }   

        public void Remove(ChildElement element)
        {
            //remove from the cache first while you can still get the element's id.
            if (element is AlternativeElement altElem)
            {
                StudyCacheForSaving.RemoveElement(altElem);
                RemoveFromParentTable(element, TableName);
            }
        }

        public void SaveExisting(ChildElement elementToSave, int changeTableIndex)
        {
            base.SaveExisting( elementToSave);
        }

        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(AlternativeElement))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;
                SaveNewElementToParentTable(GetRowDataFromElement((AlternativeElement)element), TableName, TableColumnNames, TableColumnTypes);
                StudyCacheForSaving.AddElement((AlternativeElement)element);
            }
        }
    }
}
