using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        //public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex)
        //{
        //    base.SaveExisting(oldElement, elementToSave);
        //}

        public void SaveNew(ChildElement element)
        {
            SaveNewElement(element);
        }
        //public void SaveNew(Study.PropertiesVM vm)
        //{
        //    string[] names = new string[2];
        //    names[0] = "Parameter";
        //    names[1] = "Value";
        //    Type[] types = new Type[2];
        //    types[0] = typeof(string);
        //    types[1] = typeof(string);
        //    Storage.Connection.Instance.CreateTable(TableName, names, types);
        //    DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);
        //    tbl.AddRow(new object[] { "Study Name: ", vm.StudyName });
        //    tbl.AddRow(new object[] { "Study Path: ", vm.StudyPath });
        //    tbl.AddRow(new object[] { "Description: ", vm.StudyDescription });
        //    tbl.AddRow(new object[] { "Created By: ", vm.CreatedBy });
        //    tbl.AddRow(new object[] { "Created Date: ", vm.CreatedDate });
        //    tbl.AddRow(new object[] { "Study Notes: ", vm.StudyNotes });
        //    tbl.AddRow(new object[] { "Monetary Unit: ", vm.MonetaryUnit });
        //    tbl.AddRow(new object[] { "Surveyed Year: ", vm.SurveyedYear });
        //    tbl.AddRow(new object[] { "Updated Year: ", vm.UpdatedYear });
        //    tbl.AddRow(new object[] { "Updated Price Index: ", vm.UpdatedPriceIndex });
        //    tbl.AddRow(new object[] { "Discount Rate:", vm.DiscountRate });
        //    tbl.AddRow(new object[] { "Period of Analysis:", vm.PeriodOfAnalysis });

        //    tbl.ApplyEdits();
        //}

        //public void SaveExisting(Study.PropertiesVM vm)
        //{
        //    DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);

        //    string oldName = tbl.GetCell(0, 1).ToString();
        //    if(!oldName.Equals(vm.StudyName))
        //    {
        //        RenameStudy(oldName, vm.StudyName);
        //    }

        //    tbl.EditCell(0, 1, vm.StudyName);
        //    tbl.EditCell(1, 1, vm.StudyPath);
        //    tbl.EditCell(2, 1, vm.StudyDescription);
        //    tbl.EditCell(3, 1, vm.CreatedBy);
        //    tbl.EditCell(4, 1, vm.CreatedDate);
        //    tbl.EditCell(5, 1, vm.StudyNotes);
        //    tbl.EditCell(6, 1, vm.MonetaryUnit);
        //    tbl.EditCell(7, 1, vm.SurveyedYear);
        //    tbl.EditCell(8, 1, vm.UpdatedYear);
        //    tbl.EditCell(9, 1, vm.UpdatedPriceIndex);
        //    tbl.EditCell(10, 1, vm.DiscountRate);
        //    tbl.EditCell(11, 1, vm.PeriodOfAnalysis);
        //    if (!Storage.Connection.Instance.IsOpen) Storage.Connection.Instance.Open();
        //    tbl.ApplyEdits();
        //}

        //todo: how to rename the whole study?
        private void RenameStudy(string oldName, string newName)
        {
            string oldDirectory = Storage.Connection.Instance.ProjectDirectory;
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
