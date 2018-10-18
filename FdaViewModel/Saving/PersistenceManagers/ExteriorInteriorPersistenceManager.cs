using FdaViewModel.StageTransforms;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class ExteriorInteriorPersistenceManager : SavingBase, IPersistable
    {
        private const string _TableConstant = "Exterior Interior - ";

        public  string TableName
        {
            get { return "Interior Exterior Curves";  }
        }
        public  string[] TableColumnNames()
        {
            return new string[] { "Name", "Last Edit Date", "Description", "Curve Distribution Type" };
        }
        public  Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(string), typeof(string) };
        }

        public ExteriorInteriorPersistenceManager(Study.FDACache studyCache)
        {
            StudyCache = studyCache;
        }



        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(ExteriorInteriorElement))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;
                SaveNewElementToParentTable(element, TableName, TableColumnNames(), TableColumnTypes());
                //save the individual table
                SaveExteriorInteriorCurve((ExteriorInteriorElement)element, editDate);
                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCache.AddExteriorInteriorElement((ExteriorInteriorElement)element);
            }
        }

        public void SaveExisting(Utilities.ChildElement element, string oldName, Statistics.UncertainCurveDataCollection oldCurve)
        {
            SaveExistingElement(oldName, oldCurve, element, TableName);
        }

        public List<ChildElement> Load()
        {
            return CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
             
        }


        private void SaveExteriorInteriorCurve(ExteriorInteriorElement element, string lastEditDate)
        {
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            element.Curve.toSqliteTable(_TableConstant + lastEditDate);
        }


        public  ChildElement CreateElementFromRowData(object[] rowData)
        {
            Statistics.UncertainCurveDataCollection ucdc = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            ExteriorInteriorElement ele = new ExteriorInteriorElement((string)rowData[0], (string)rowData[1], (string)rowData[2], ucdc);
            ele.ExteriorInteriorCurve.fromSqliteTable(ele.TableName);
            return ele;
        }
    }
}
