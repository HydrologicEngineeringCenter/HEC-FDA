using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class StudyPropertiesPersistenceManager 
    {

        public static readonly string TableName = "Study Properties";

        public Study.FDACache StudyCacheForSaving { get; set; }
        public StudyPropertiesPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        public List<ChildElement> Load()
        {
            throw new NotImplementedException();
        }

        public void Remove(ChildElement element)
        {
            throw new NotImplementedException();
        }

        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex)
        {
            throw new NotImplementedException();
        }

        public void SaveNew(ChildElement element)
        {
            throw new NotImplementedException();
        }
        public void SaveNew(Study.PropertiesVM vm)
        {
            string[] names = new string[2];
            names[0] = "Parameter";
            names[1] = "Value";
            Type[] types = new Type[2];
            types[0] = typeof(string);
            types[1] = typeof(string);
            Storage.Connection.Instance.CreateTable(TableName, names, types);
            DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);
            tbl.AddRow(new object[] { "Study Name: ", vm.StudyName });
            tbl.AddRow(new object[] { "Study Path: ", vm.StudyPath });
            tbl.AddRow(new object[] { "Description: ", vm.StudyDescription });
            tbl.AddRow(new object[] { "Created By: ", vm.CreatedBy });
            tbl.AddRow(new object[] { "Created Date: ", vm.CreatedDate });
            tbl.AddRow(new object[] { "Study Notes: ", vm.StudyNotes });
            tbl.AddRow(new object[] { "Monetary Unit: ", vm.MonetaryUnit });
            tbl.AddRow(new object[] { "Surveyed Year: ", vm.SurveyedYear });
            tbl.AddRow(new object[] { "Updated Year: ", vm.UpdatedYear });
            tbl.AddRow(new object[] { "Updated Price Index: ", vm.UpdatedPriceIndex });
            tbl.ApplyEdits();
        }
        public void SaveExisting(Study.PropertiesVM vm)
        {
            DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);

            string oldName = tbl.GetCell(0, 1).ToString();
            if(!oldName.Equals(vm.StudyName))
            {
                RenameStudy(oldName, vm.StudyName);
            }

            tbl.EditCell(0, 1, vm.StudyName);
            tbl.EditCell(1, 1, vm.StudyPath);
            tbl.EditCell(2, 1, vm.StudyDescription);
            tbl.EditCell(3, 1, vm.CreatedBy);
            tbl.EditCell(4, 1, vm.CreatedDate);
            tbl.EditCell(5, 1, vm.StudyNotes);
            tbl.EditCell(6, 1, vm.MonetaryUnit);
            tbl.EditCell(7, 1, vm.SurveyedYear);
            tbl.EditCell(8, 1, vm.UpdatedYear);
            tbl.EditCell(9, 1, vm.UpdatedPriceIndex);
            if (!Storage.Connection.Instance.IsOpen) Storage.Connection.Instance.Open();
            tbl.ApplyEdits();
        }

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
    }
}
